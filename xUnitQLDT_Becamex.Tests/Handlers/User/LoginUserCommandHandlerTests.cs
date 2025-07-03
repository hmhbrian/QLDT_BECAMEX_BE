using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Handlers;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitQLDT_Becamex.Tests.Handlers.UserChangePasswordCommand
{
    public class LoginUserCommandHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly LoginUserCommandHandler _handler;

        public LoginUserCommandHandlerTests()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManagerMock.Object,
                contextAccessor.Object,
                userClaimsPrincipalFactory.Object,
                null, null, null, null);

            _jwtServiceMock = new Mock<IJwtService>();
            _mapperMock = new Mock<IMapper>();

            _handler = new LoginUserCommandHandler(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _jwtServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsAppException()
        {
            // Arrange
            var request = new UserLoginDto { Email = "notfound@becamex.com", Password = "123456" };
            var command = new LoginUserCommand(request);

            _userManagerMock.Setup(x => x.Users)
                .Returns((new List<ApplicationUser>()).AsQueryable().BuildMock());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal(401, ex.StatusCode);
            Assert.Equal("Tên đăng nhập hoặc mật khẩu không chính xác.", ex.Message);
        }

        [Fact]
        public async Task Handle_WrongPassword_ThrowsAppException()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", Email = "user@becamex.com" };
            var request = new UserLoginDto { Email = "user@becamex.com", Password = "wrongpass" };
            var command = new LoginUserCommand(request);

            _userManagerMock.Setup(x => x.Users)
                .Returns(new List<ApplicationUser> { user }.AsQueryable().BuildMock());

            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, "wrongpass", false))
                .ReturnsAsync(SignInResult.Failed);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal(401, ex.StatusCode);
        }

        [Fact]
        public async Task Handle_AccountLocked_ThrowsAppException()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", Email = "user@becamex.com" };
            var request = new UserLoginDto { Email = "user@becamex.com", Password = "pass" };
            var command = new LoginUserCommand(request);

            _userManagerMock.Setup(x => x.Users)
                .Returns(new List<ApplicationUser> { user }.AsQueryable().BuildMock());

            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, "pass", false))
                .ReturnsAsync(SignInResult.LockedOut);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal(403, ex.StatusCode);
            Assert.Equal("Tài khoản của bạn đã bị khóa.", ex.Message);
        }

        [Fact]
        public async Task Handle_AccountNotAllowed_ThrowsAppException()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", Email = "user@becamex.com" };
            var request = new UserLoginDto { Email = "user@becamex.com", Password = "pass" };
            var command = new LoginUserCommand(request);

            _userManagerMock.Setup(x => x.Users)
                .Returns(new List<ApplicationUser> { user }.AsQueryable().BuildMock());

            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, "pass", false))
                .ReturnsAsync(SignInResult.NotAllowed);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal(403, ex.StatusCode);
            Assert.Equal("Tài khoản chưa được xác nhận hoặc bị hạn chế.", ex.Message);
        }

        [Fact]
        public async Task Handle_ValidLogin_ReturnsUserDtoWithToken()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", Email = "user@becamex.com", FullName = "Test User" };
            var request = new UserLoginDto { Email = "user@becamex.com", Password = "correctpass" };
            var command = new LoginUserCommand(request);

            var userDto = new UserDto { Id = "1", Email = "user@becamex.com", FullName = "Test User" };
            var roles = new List<string> { "Admin" };
            var token = "mock-token";

            _userManagerMock.Setup(x => x.Users)
                .Returns(new List<ApplicationUser> { user }.AsQueryable().BuildMock());

            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, "correctpass", false))
                .ReturnsAsync(SignInResult.Success);

            _mapperMock.Setup(x => x.Map<UserDto>(user))
                .Returns(userDto);

            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(roles);

            _jwtServiceMock.Setup(x => x.GenerateJwtToken("1", "user@becamex.com", "Admin"))
                .Returns(token);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("1", result.Id);
            Assert.Equal("user@becamex.com", result.Email);
            Assert.Equal("Admin", result.Role);
            Assert.Equal("mock-token", result.AccessToken);
        }
    }
}
