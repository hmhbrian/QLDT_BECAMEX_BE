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
    public class ForceDeleteUserCommandHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly ForceDeleteUserCommandHandler _handler;

        public ForceDeleteUserCommandHandlerTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _handler = new ForceDeleteUserCommandHandler(_userManagerMock.Object);
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsAppException()
        {
            // Arrange
            var command = new ForceDeleteUserCommand("notfound");

            _userManagerMock.Setup(x => x.FindByIdAsync("notfound"))
                .ReturnsAsync((ApplicationUser)null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal(404, ex.StatusCode);
            Assert.Equal("Người dùng không tồn tại.", ex.Message);
        }

        [Fact]
        public async Task Handle_DeleteFailed_ThrowsAppException()
        {
            // Arrange
            var user = new ApplicationUser { Id = "123" };
            var command = new ForceDeleteUserCommand("123");

            _userManagerMock.Setup(x => x.FindByIdAsync("123"))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Lỗi CSDL" }));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal(400, ex.StatusCode);
            Assert.Contains("Xóa vĩnh viễn thất bại", ex.Message);
            Assert.Contains("Lỗi CSDL", ex.Message);
        }

        [Fact]
        public async Task Handle_DeleteSuccess_ReturnsUserId()
        {
            // Arrange
            var user = new ApplicationUser { Id = "456" };
            var command = new ForceDeleteUserCommand("456");

            _userManagerMock.Setup(x => x.FindByIdAsync("456"))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("456", result);
        }
    }
}
