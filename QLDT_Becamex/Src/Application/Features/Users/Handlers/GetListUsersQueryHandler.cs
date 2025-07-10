using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Application.Features.Users.Queries;

namespace QLDT_Becamex.Src.Application.Features.Users.Handlers
{
    public class GetListUsersQueryHandler : IRequestHandler<GetListUsersQuery, PagedResult<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetListUsersQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<PagedResult<UserDto>> Handle(GetListUsersQuery request, CancellationToken cancellationToken)
        {
            var queryParams = request.BaseQueryParam;

            // 1. Tổng số bản ghi
            int totalItems = await _unitOfWork.UserRepository.CountAsync(u => !u.IsDeleted);

            // 2. Hàm sắp xếp
            Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>> orderByFunc = query =>
            {
                bool isDesc = queryParams.SortType?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;
                return queryParams.SortField?.ToLower() switch
                {
                    "email" => isDesc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                    "created.at" => isDesc ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
                    _ => query.OrderBy(u => u.Email)
                };
            };

            // 3. Lấy dữ liệu có phân trang
            var users = await _unitOfWork.UserRepository.GetFlexibleAsync(
                predicate: u => !u.IsDeleted,
                orderBy: orderByFunc,
                page: queryParams.Page,
                pageSize: queryParams.Limit,
                includes: q => q.Include(u => u.Position)
                                .Include(u => u.Department)
                                .Include(u => u.ManagerU)
                                .Include(u => u.UserStatus)
                                .Include(u => u.CreateBy)
                                .Include(u => u.UpdateBy),

                asNoTracking: true
            );

            // 4. Mapping
            var userDtos = _mapper.Map<List<UserDto>>(users);

            foreach (var userDto in userDtos)
            {
                var user = users.FirstOrDefault(u => u.Id == userDto.Id);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDto.Role = roles.FirstOrDefault();
                }
            }

            var pagination = new Pagination(queryParams.Page, queryParams.Limit, totalItems);
            // 5. Tạo kết quả phân trang
            var result = new PagedResult<UserDto>(
                userDtos,
                pagination
            );
            return result;
        }
    }
}
