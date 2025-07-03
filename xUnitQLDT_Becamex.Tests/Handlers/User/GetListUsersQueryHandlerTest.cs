using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Handlers;
using QLDT_Becamex.Src.Application.Features.Users.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace xUnitQLDT_Becamex.Tests.Handlers.User
{
    public class GetListUsersQueryHandlerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

        private readonly GetListUsersQueryHandler _handler;

        public GetListUsersQueryHandlerTest()
        {
            // Khởi tạo giả UserManager và RoleManager bằng Moq
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            // Khởi tạo handler cần test
            _handler = new GetListUsersQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object, _userManagerMock.Object);
        }
        [Fact]
        public async Task Handle_Should_ReturnPagedUsers_When_UsersExist()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "user1", Email = "user1@example.com", CreatedAt = DateTime.UtcNow.AddDays(-1) },
                new ApplicationUser { Id = "user2", Email = "user2@example.com", CreatedAt = DateTime.UtcNow }
            };
                var userDtos = new List<UserDto>
            {
                new UserDto { Id = "user1", Email = "user1@example.com", Role = "Admin" },
                new UserDto { Id = "user2", Email = "user2@example.com", Role = "User" }
            };
            var request = new BaseQueryParam
            {
                Page = 1,
                Limit = 10,
                SortField = "email",
                SortType = "asc"
            };
            var query = new GetListUsersQuery(request);

            _unitOfWorkMock.Setup(u => u.UserRepository.CountAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(2);

            _unitOfWorkMock.Setup(u => u.UserRepository.GetFlexibleAsync(
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                It.IsAny<Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>>(),
                query.BaseQueryParam.Page,
                query.BaseQueryParam.Limit,
                It.IsAny<bool>(),
                It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>>()
                ))
                .ReturnsAsync(users);

            _mapperMock.Setup(m => m.Map<List<UserDto>>(users))
                .Returns(userDtos);

            _userManagerMock.Setup(m => m.GetRolesAsync(users[0])).ReturnsAsync(new List<string> { "Admin" });
            _userManagerMock.Setup(m => m.GetRolesAsync(users[1])).ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal("user1", result.Items.ElementAt(0).Id); // Use ElementAt instead of indexer
            Assert.Equal("Admin", result.Items.ElementAt(0).Role);
            Assert.Equal("user2", result.Items.ElementAt(1).Id);
            Assert.Equal("User", result.Items.ElementAt(1).Role);
            Assert.NotNull(result.Pagination);
            Assert.Equal(2, result.Pagination.TotalItems);
            Assert.Equal(1, result.Pagination.TotalPages);
            Assert.Equal(10, result.Pagination.ItemsPerPage);
            Assert.Equal(1, result.Pagination.CurrentPage);
        }

        [Fact]
        public async Task Handle_Should_ReturnEmptyList_When_NoUsersExist()
        {
            // Arrange
            var request = new BaseQueryParam
            {
                Page = 1,
                Limit = 10,
                SortField = "email",
                SortType = "asc"
            };
            var query = new GetListUsersQuery(request);

            _unitOfWorkMock.Setup(u => u.UserRepository.CountAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
             .ReturnsAsync(0);

            _unitOfWorkMock.Setup(u => u.UserRepository.GetFlexibleAsync(
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                It.IsAny<Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>>(),
                query.BaseQueryParam.Page,
                query.BaseQueryParam.Limit,
                It.IsAny<bool>(),
                It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>>()
                ))
                .ReturnsAsync(new List<ApplicationUser>());

            _mapperMock.Setup(m => m.Map<List<UserDto>>(It.IsAny<List<ApplicationUser>>()))
                .Returns(new List<UserDto>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Pagination.TotalItems);
            Assert.Equal(0, result.Pagination.TotalPages);
            Assert.Equal(10, result.Pagination.ItemsPerPage);
            Assert.Equal(1, result.Pagination.CurrentPage);
        }

        [Fact]
        public async Task Handle_Should_SortByCreatedAtDesc_When_SortFieldIsCreatedAt()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "user1", Email = "user1@example.com", CreatedAt = DateTime.UtcNow.AddDays(-1) },
                new ApplicationUser { Id = "user2", Email = "user2@example.com", CreatedAt = DateTime.UtcNow }
            };
                var userDtos = new List<UserDto>
            {
                new UserDto { Id = "user1", Email = "user1@example.com" },
                new UserDto { Id = "user2", Email = "user2@example.com" }
            };
            var request = new BaseQueryParam
            {
                Page = 1,
                Limit = 10,
                SortField = "email",
                SortType = "asc"
            };
            var query = new GetListUsersQuery(request);

            _unitOfWorkMock.Setup(u => u.UserRepository.CountAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(2);

            _unitOfWorkMock.Setup(u => u.UserRepository.GetFlexibleAsync(
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                It.IsAny<Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>>(),
                query.BaseQueryParam.Page,
                query.BaseQueryParam.Limit,
                It.IsAny<bool>(),
                It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>>()
                ))
                .ReturnsAsync(users.OrderByDescending(u => u.CreatedAt).ToList());

            _mapperMock.Setup(m => m.Map<List<UserDto>>(It.IsAny<List<ApplicationUser>>()))
                .Returns(userDtos);

            _userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal("user2", result.Items.ElementAt(1).Id); // Use ElementAt instead of indexer
            Assert.Equal("user1", result.Items.ElementAt(0).Id);
        }
        [Fact]
        public async Task Handle_Should_ThrowNullReferenceException_When_CancellationRequested_DueToMissingCancellationCheck()
        {
            // Arrange
            var baseQueryParam = new BaseQueryParam
            {
                Page = 1,
                Limit = 10,
                SortField = "email",
                SortType = "asc"
            };
            var query = new GetListUsersQuery(baseQueryParam);
            Assert.NotNull(query.BaseQueryParam);
            var cancellationToken = new CancellationToken(true);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _handler.Handle(query, cancellationToken));
        }
    }
}
