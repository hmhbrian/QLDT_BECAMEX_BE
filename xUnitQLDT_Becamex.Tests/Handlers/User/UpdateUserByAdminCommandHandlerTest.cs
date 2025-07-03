using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;
using Moq.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Handlers;
using QLDT_Becamex.Src.Domain.Entities;

namespace xUnitQLDT_Becamex.Tests.Handlers.UserChangePasswordCommand
{
    public class UpdateUserByAdminCommandHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly UpdateUserByAdminCommandHandler _handler;

        public UpdateUserByAdminCommandHandlerTests()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

            _handler = new UpdateUserByAdminCommandHandler(_userManagerMock.Object, _roleManagerMock.Object);
        }

        // Test case "Không tìm thấy người dùng"
        [Fact]
        public async Task Handle_UserNotFound_ThrowsAppException()
        {
            // Arrange
            _userManagerMock.Setup(x => x.Users)
                .Returns((new List<ApplicationUser>()).AsQueryable().BuildMock());

            var command = new UpdateUserByAdminCommand("nonexistent-id", new UserAdminUpdateDto());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal(404, ex.StatusCode);
            Assert.Equal("Không tìm thấy người dùng", ex.Message);
        }

        // Test case "Cập nhật email đã tồn tại"
        [Fact]
        public async Task Handle_DuplicateEmail_ThrowsAppException()
        {
            // Arrange
            var existingUser = new ApplicationUser { Id = "1", Email = "old@becamex.com" };
            var command = new UpdateUserByAdminCommand("1", new UserAdminUpdateDto { Email = "new@becamex.com" });

            var userList = new List<ApplicationUser>
        {
            existingUser,
            new ApplicationUser { Id = "2", Email = "new@becamex.com" } // duplicate
        };

            _userManagerMock.Setup(x => x.Users)
                .Returns(userList.AsQueryable().BuildMock());

            _userManagerMock.Setup(x => x.SetEmailAsync(existingUser, "new@becamex.com"))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.SetUserNameAsync(existingUser, "new@becamex.com"))
                .ReturnsAsync(IdentityResult.Success);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Email đã được sử dụng", ex.Message);
            Assert.Equal(409, ex.StatusCode);
        }

        // Test case "Cập nhật thành công"
        [Fact]
        public async Task Handle_ValidUpdate_ReturnsUserId()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", Email = "test@becamex.com" };
            var command = new UpdateUserByAdminCommand("1", new UserAdminUpdateDto
            {
                FullName = "New Name",
                Email = "test@becamex.com"
            });

            var users = new List<ApplicationUser> { user };
            _userManagerMock.Setup(x => x.Users)
                .Returns(users.AsQueryable().BuildMock());

            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.SetEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.SetUserNameAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("1", result);
        }

        //Test case "CCCD đã tồn tại"
        [Fact]
        public async Task Handle_DuplicateIdCard_ThrowsAppException()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", IdCard = "1234567890" };
            var command = new UpdateUserByAdminCommand("1", new UserAdminUpdateDto { IdCard = "9999999999" });

            var users = new List<ApplicationUser>
            {
                user,
                new ApplicationUser { Id = "2", IdCard = "9999999999" } // bị trùng
            };

            _userManagerMock.Setup(x => x.Users)
                .Returns(users.AsQueryable().BuildMock());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("CCCD đã tồn tại", ex.Message);
            Assert.Equal(409, ex.StatusCode);
        }

        //Test case "Mã nhân viên đã tồn tại"
        [Fact]
        public async Task Handle_DuplicateEmployeeCode_ThrowsAppException()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", Code = "EMP001" };
            var command = new UpdateUserByAdminCommand("1", new UserAdminUpdateDto { Code = "EMP999" });

            var users = new List<ApplicationUser>
            {
                user,
                new ApplicationUser { Id = "2", Code = "EMP999" } // trùng mã
            };

            _userManagerMock.Setup(x => x.Users)
                .Returns(users.AsQueryable().BuildMock());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Mã nhân viên đã tồn tại", ex.Message);
            Assert.Equal(409, ex.StatusCode);
        }

        // Test case "Vai trò không hợp lệ"
        [Fact]
        public async Task Handle_InvalidRoleId_ThrowsAppException()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1" };
            var command = new UpdateUserByAdminCommand("1", new UserAdminUpdateDto { RoleId = "invalid-role-id" });

            _userManagerMock.Setup(x => x.Users)
                .Returns(new List<ApplicationUser> { user }.AsQueryable().BuildMock());

            _roleManagerMock.Setup(x => x.FindByIdAsync("invalid-role-id"))
                .ReturnsAsync((IdentityRole)null); // không tìm thấy role

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Vai trò không hợp lệ", ex.Message);
            Assert.Equal(400, ex.StatusCode);
        }

    }
}