using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Handlers;
using QLDT_Becamex.Src.Application.Features.Users.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitQLDT_Becamex.Tests.Handlers.User
{
    public class GetUserByIdQueryHandlerTest
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly GetUserByIdQueryHandler _handler;
        public GetUserByIdQueryHandlerTest()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _mapperMock = new Mock<IMapper>();

            _handler = new GetUserByIdQueryHandler(_userManagerMock.Object, _mapperMock.Object);
        }
        [Fact]
        public async Task Handle_Should_ReturnUserDto_When_UserExists()
        {
            // Arrange
            var userId = "user1";
            var user = new ApplicationUser
            {
                Id = userId,
                Email = "user1@example.com",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            var userDto = new UserDto
            {
                Id = userId,
                Email = "user1@example.com",
                Role = "Admin"
            };
            var query = new GetUserByIdQuery(userId);

            _userManagerMock.Setup(u => u.Users).Returns(new[] { user }.AsQueryable().BuildMockDbSet());
            _userManagerMock.Setup(u => u.Users
                .Include(It.IsAny<string>())
                //.Include(It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>>())
                //.Include(It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>>())
                //.Include(It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>>())
                .FirstOrDefaultAsync(u => u.Id == userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);
            _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("user1@example.com", result.Email);
            Assert.Equal("Admin", result.Role);
        }

        [Fact]
        public async Task Handle_Should_ThrowAppException_When_UserDoesNotExist()
        {
            // Arrange
            var userId = "nonexistent";
            var query = new GetUserByIdQuery(userId);

            _userManagerMock.Setup(u => u.Users).Returns(new ApplicationUser[] { }.AsQueryable().BuildMockDbSet());
            _userManagerMock.Setup(u => u.Users
                .Include(It.IsAny<string>())
                //.Include(It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>>())
                //.Include(It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>>())
                //.Include(It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>>())
                .FirstOrDefaultAsync(u => u.Id == userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Không tìm thấy người dùng", exception.Message);
            Assert.Equal(404, exception.StatusCode);
        }

        [Fact]
        public async Task Handle_Should_ThrowOperationCanceledException_When_CancellationRequested()
        {
            // Arrange
            var userId = "user1";
            var query = new GetUserByIdQuery(userId);
            var cancellationToken = new CancellationToken(true);

            // No mocks needed since handler should throw before querying

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _handler.Handle(query, cancellationToken));
        }
    }
    public static class MockDbSetExtensions
    {
        public static DbSet<T> BuildMockDbSet<T>(this IQueryable<T> source) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(source.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(source.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(source.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(source.GetEnumerator());
            return mockSet.Object;
        }
    }
}
