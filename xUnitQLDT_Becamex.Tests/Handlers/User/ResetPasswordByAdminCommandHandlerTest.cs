using Microsoft.AspNetCore.Identity;
using Moq;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Handlers;
using QLDT_Becamex.Src.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitQLDT_Becamex.Tests.Handlers.UserChangePasswordCommand
{
    public class ResetPasswordByAdminCommandHandlerTest
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly ResetPasswordByAdminCommandHandler _handler;

        public ResetPasswordByAdminCommandHandlerTest()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _handler = new ResetPasswordByAdminCommandHandler(_userManagerMock.Object);
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsAppException()
        {
            // Arrange
            var command = new ResetPasswordByAdminCommand("non-existent", new UserResetPasswordDto
            {
                NewPassword = "newpass123",
                ConfirmNewPassword = "newpass123"
            });

            _userManagerMock.Setup(x => x.FindByIdAsync("non-existent"))
                .ReturnsAsync((ApplicationUser)null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal(404, ex.StatusCode);
            Assert.Equal("Không tìm thấy người dùng.", ex.Message);
        }

        [Fact]
        public async Task Handle_ResetPasswordFailed_ThrowsAppException()
        {
            // Arrange
            var user = new ApplicationUser { Id = "123" };

            var command = new ResetPasswordByAdminCommand("123", new UserResetPasswordDto
            {
                NewPassword = "newpass123",
                ConfirmNewPassword = "newpass123"
            });

            _userManagerMock.Setup(x => x.FindByIdAsync("123"))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("fake-token");

            _userManagerMock.Setup(x => x.ResetPasswordAsync(user, "fake-token", "newpass123"))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal(400, ex.StatusCode);
            Assert.Contains("Đặt lại mật khẩu thất bại", ex.Message);
            Assert.Contains("Password too weak", ex.Message);
        }

        [Fact]
        public async Task Handle_ResetPasswordSuccess_ReturnsUserId()
        {
            // Arrange
            var user = new ApplicationUser { Id = "456" };

            var command = new ResetPasswordByAdminCommand("456", new UserResetPasswordDto
            {
                NewPassword = "validpassword",
                ConfirmNewPassword = "validpassword"
            });

            _userManagerMock.Setup(x => x.FindByIdAsync("456"))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("token");

            _userManagerMock.Setup(x => x.ResetPasswordAsync(user, "token", "validpassword"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("456", result);
        }
    }
}
