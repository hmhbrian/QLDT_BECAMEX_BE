using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet.Actions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Status.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Shared.Helpers;
using System.Linq;
using System.Linq.Expressions;

namespace QLDT_Becamex.Src.Application.Features.Users.Handlers
{
    public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, PagedResult<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SearchUsersQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<UserDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            var queryParams = request.QueryParam;

            // 1. Lấy user chưa xoá
            Expression<Func<ApplicationUser, bool>>? predicate = c => c.IsDeleted == false;

            // 2. Lọc theo keyword (không dấu)
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = StringHelper.RemoveDiacritics(request.Keyword).ToUpperInvariant().Replace(" ", "");
                predicate = predicate.And(u => u.NormalizedUserName!.Contains(keyword) || u.Email!.Contains(keyword));
                Console.WriteLine("KEYWORD:" + keyword);
            }

            //3. Đếm tổng số bản ghi
            int totalItems = await _unitOfWork.UserRepository.CountAsync(predicate);

            // 4. Sắp xếp
            Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>? orderBy = q =>
            {
                bool isDesc = request.QueryParam.SortType?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;
                return request.QueryParam.SortField?.ToLower() switch
                {
                    "created.at" => isDesc ? q.OrderByDescending(c => c.CreatedAt) : q.OrderBy(c => c.CreatedAt),
                    _ => q.OrderBy(c => c.CreatedAt)
                };
            };

            //5.Lấy ds User
            var usersQuery = _unitOfWork.UserRepository.GetFlexible(
                predicate: predicate,
                orderBy: orderBy,
                page: request.QueryParam.Page,
                asNoTracking: true,
                includes: new[] { (Expression<Func<ApplicationUser, object>>)(q => q.UserStatus)});

            // 6. Ánh xạ sang DTO với thông tin Role và Status
            var userDtos = await usersQuery
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            // 7.Lấy Role cho từng user
            var userIds = userDtos.Select(dto => dto.Id).ToList();
            var userRoles = new Dictionary<string, string>();
            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userRoles[userId] = roles.FirstOrDefault();
                }
            }

            foreach (var dto in userDtos)
            {
                dto.Role = userRoles.GetValueOrDefault(dto.Id);
            }


            //8.Phân trang
            var pagination = new Pagination(
                currentPage: request.QueryParam.Page,
                itemsPerPage: request.QueryParam.Limit > 0 ? request.QueryParam.Limit : 10,
                totalItems: totalItems);
            var pagedResult = new PagedResult<UserDto>(userDtos, pagination);
            return pagedResult;
        }
    }
}
