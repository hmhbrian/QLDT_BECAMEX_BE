using Microsoft.AspNetCore.Identity;
using Moq;
using QLDT_Becamex.Src.Application.Commands.Users.CreateUser;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitQLDT_Becamex.Tests.Handlers.UserChangePasswordCommand
{
    public class ChangePasswordUserCommandHandlerTest
    {
        //Mock dêpndencies
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IBaseService> _baseServiceMock;

        //Handler to test
        private readonly ChangePasswordUserCommandHandler _handler;

        public ChangePasswordUserCommandHandlerTest()
        {
            // Khởi tạo giả UserManager và RoleManager bằng Moq
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _baseServiceMock = new Mock<IBaseService>();

            // Khởi tạo handler cần test
            _handler = new ChangePasswordUserCommandHandler(_userManagerMock.Object, _baseServiceMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnUserId_When_PasswordChangeSucceeds()
        {
            // Arrange
            var userId = "user123";
            var user = new ApplicationUser { Id = userId };
            var dto = new UserChangePasswordDto
            {
                OldPassword = "OldPassword123",
                NewPassword = "NewPassword123"
            };
            var command = new ChangePasswordUserCommand(dto);
            _baseServiceMock.Setup(s => s.GetCurrentUserAuthenticationInfo())
                .Returns((userId, "someRole"));

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _userManagerMock.Setup(m => m.ChangePasswordAsync(user, command.Request.OldPassword, command.Request.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(userId, result);
        }

        [Fact]
        public async Task Handle_Should_ThrowAppException_When_UserNotFound()
        {
            // Arrange
            var userId = "user123";
            var dto = new UserChangePasswordDto
            {
                OldPassword = "OldPassword123",
                NewPassword = "NewPassword123"
            };
            var command = new ChangePasswordUserCommand(dto);

            _baseServiceMock.Setup(s => s.GetCurrentUserAuthenticationInfo())
                .Returns((userId, "someRole"));

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() =>
                _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Không tìm thấy người dùng", exception.Message);
            Assert.Equal(404, exception.StatusCode);
        }

        [Fact]
        public async Task Handle_Should_ThrowAppException_When_PasswordChangeFails()
        {
            // Arrange
            var userId = "user123";
            var user = new ApplicationUser { Id = userId };
            var dto = new UserChangePasswordDto
            {
                OldPassword = "OldPassword123",
                NewPassword = "NewPassword123"
            };
            var command = new ChangePasswordUserCommand(dto);

            _baseServiceMock.Setup(s => s.GetCurrentUserAuthenticationInfo())
                .Returns((userId, "someRole"));

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _userManagerMock.Setup(m => m.ChangePasswordAsync(user, command.Request.OldPassword, command.Request.NewPassword))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Mật khẩu cũ không đúng" }));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() =>
                _handler.Handle(command, CancellationToken.None));
            Assert.Contains("Đổi mật khẩu thất bại: Mật khẩu cũ không đúng", exception.Message);
            Assert.Equal(400, exception.StatusCode);
        }
    }
}
