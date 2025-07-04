using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Shared.Helpers;
using System.Linq;

namespace QLDT_Becamex.Src.Application.Features.Users.Handlers
{
    public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, PagedResult<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public SearchUsersQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<PagedResult<UserDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            var queryParams = request.QueryParam;
            var keyword = request.Keyword;

            // 1. Lấy user chưa xoá
            var users = await _userManager.Users
                .Where(u => !u.IsDeleted)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // 2. Lọc theo keyword (không dấu)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalized = StringHelper.RemoveDiacritics(keyword).ToLowerInvariant();
                users = users.Where(u =>
                    StringHelper.RemoveDiacritics(u.FullName ?? "").ToLower().Contains(normalized) ||
                    (u.Email ?? "").ToLower().Contains(normalized)
                ).ToList();
            }

            int total = users.Count;

            // 3. Sắp xếp
            users = queryParams.SortField?.ToLower() switch
            {
                "createdat" => queryParams.SortType?.ToLower() == "asc"
                    ? users.OrderBy(u => u.CreatedAt).ToList()
                    : users.OrderByDescending(u => u.CreatedAt).ToList(),
                _ => users.OrderByDescending(u => u.CreatedAt).ToList()
            };

            // 4. Phân trang
            var skip = (queryParams.Page - 1) * queryParams.Limit;
            var pagedUsers = users.Skip(skip).Take(queryParams.Limit).ToList();

            // 5. Map sang DTO
            var userDtos = _mapper.Map<List<UserDto>>(pagedUsers);

            foreach (var dto in userDtos)
            {
                var user = pagedUsers.FirstOrDefault(u => u.Id == dto.Id);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    dto.Role = roles.FirstOrDefault();
                }
            }

            var pagination = new Pagination(queryParams.Page, queryParams.Limit, total);
            var pagedResult = new PagedResult<UserDto>(userDtos, pagination);
            return pagedResult;

        }
    }
}
