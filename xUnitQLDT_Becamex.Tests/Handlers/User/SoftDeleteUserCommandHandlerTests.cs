using Microsoft.AspNetCore.Identity;
using Moq;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Application.Features.Users.Handlers;
using QLDT_Becamex.Src.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitQLDT_Becamex.Tests.Handlers.UserChangePasswordCommand
{
    public class SoftDeleteUserCommandHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly SoftDeleteUserCommandHandler _handler;

        public SoftDeleteUserCommandHandlerTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _handler = new SoftDeleteUserCommandHandler(_userManagerMock.Object);
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsAppException()
        {
            // Arrange
            var command = new SoftDeleteUserCommand("nonexistent-id");
            _userManagerMock.Setup(m => m.FindByIdAsync("nonexistent-id"))
                .ReturnsAsync((ApplicationUser)null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal(404, ex.StatusCode);
            Assert.Equal("Người dùng không tồn tại", ex.Message);
        }

        [Fact]
        public async Task Handle_UserAlreadyDeleted_ThrowsAppException()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "123",
                IsDeleted = true
            };

            _userManagerMock.Setup(m => m.FindByIdAsync("123"))
                .ReturnsAsync(user);

            var command = new SoftDeleteUserCommand("123");

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal(409, ex.StatusCode);
            Assert.Equal("Người dùng này đã bị xóa rồi", ex.Message);
        }

        [Fact]
        public async Task Handle_UpdateFailed_ThrowsAppException()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "456",
                IsDeleted = false
            };

            _userManagerMock.Setup(m => m.FindByIdAsync("456"))
                .ReturnsAsync(user);

            _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "DB error" }));

            var command = new SoftDeleteUserCommand("456");

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal(400, ex.StatusCode);
            Assert.Contains("DB error", ex.Message);
        }

        [Fact]
        public async Task Handle_ValidUser_DeletesSuccessfully()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "789",
                IsDeleted = false
            };

            _userManagerMock.Setup(m => m.FindByIdAsync("789"))
                .ReturnsAsync(user);

            _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var command = new SoftDeleteUserCommand("789");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("789", result);
            Assert.True(user.IsDeleted);
            Assert.True(user.ModifiedAt <= DateTime.UtcNow); // kiểm tra có cập nhật thời gian
        }
    }
}
