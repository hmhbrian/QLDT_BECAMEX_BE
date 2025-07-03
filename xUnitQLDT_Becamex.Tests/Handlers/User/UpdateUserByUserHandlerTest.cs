using Microsoft.AspNetCore.Http;
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

namespace xUnitQLDT_Becamex.Tests.Handlers.User
{
    public class UpdateUserByUserHandlerTest
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<ICloudinaryService> _cloudinaryServiceMock;
        private readonly Mock<IBaseService> _baseServiceMock;

        private readonly UpdateUserByUserCommandHandler _handler; 
        public UpdateUserByUserHandlerTest()
        {
            // Khởi tạo giả UserManager và RoleManager bằng Moq
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _baseServiceMock = new Mock<IBaseService>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();

            // Khởi tạo handler cần test
            _handler = new UpdateUserByUserCommandHandler(_userManagerMock.Object, _cloudinaryServiceMock.Object ,_baseServiceMock.Object);
        }
        [Fact]
        public async Task Handle_Should_ReturnUserId_When_UpdateSucceeds()
        {
            // Arrange
            var userId = "user123";
            var user = new ApplicationUser { Id = userId, FullName = "Old Name", PhoneNumber = "Old Phone", UrlAvatar = "old_url" };
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns("test.jpg");
            var dto = new UserUserUpdateDto
            {
                FullName = "New Name",
                PhoneNumber = "New Phone",
                UrlAvatar = formFileMock.Object
            };
            var command = new UpdateUserByUserCommand(dto);

            _baseServiceMock.Setup(s => s.GetCurrentUserAuthenticationInfo())
                .Returns((userId, "someRole"));

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _cloudinaryServiceMock.Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("new_image_url");

            _userManagerMock.Setup(m => m.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(userId, result);
            Assert.Equal("New Name", user.FullName);
            Assert.Equal("New Phone", user.PhoneNumber);
            Assert.Equal("new_image_url", user.UrlAvatar);
        }

        [Fact]
        public async Task Handle_Should_ThrowAppException_When_UserNotFound()
        {
            // Arrange
            var userId = "user123";
            var dto = new UserUserUpdateDto
            {
                FullName = "New Name",
            };
            var command = new UpdateUserByUserCommand(dto);

            _baseServiceMock.Setup(s => s.GetCurrentUserAuthenticationInfo())
                .Returns((userId, "someRole"));

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() =>
                _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Unauthorized", exception.Message);
            Assert.Equal(403, exception.StatusCode);
        }

        [Fact]
        public async Task Handle_Should_ThrowAppException_When_UpdateFails()
        {
            // Arrange
            var userId = "user123";
            var user = new ApplicationUser { Id = userId };
            var dto = new UserUserUpdateDto
            {
                FullName = "New Name",
            };
            var command = new UpdateUserByUserCommand(dto);

            _baseServiceMock.Setup(s => s.GetCurrentUserAuthenticationInfo())
                .Returns((userId, "someRole"));

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _userManagerMock.Setup(m => m.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() =>
                _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Cập nhật thông tin cá nhân thất bại", exception.Message);
            Assert.Equal(400, exception.StatusCode);
        }

        [Fact]
        public async Task Handle_Should_UpdateOnlyProvidedFields_When_SomeFieldsAreNull()
        {
            // Arrange
            var userId = "user123";
            var user = new ApplicationUser { Id = userId, FullName = "Old Name", PhoneNumber = "Old Phone", UrlAvatar = "old_url" };
            var dto = new UserUserUpdateDto
            {
                FullName = "New Name",
                PhoneNumber = null,
                UrlAvatar = null,
            };
            var command = new UpdateUserByUserCommand(dto);

            _baseServiceMock.Setup(s => s.GetCurrentUserAuthenticationInfo())
                .Returns((userId, "someRole"));

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _userManagerMock.Setup(m => m.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(userId, result);
            Assert.Equal("New Name", user.FullName);
            Assert.Equal("Old Phone", user.PhoneNumber); // Should remain unchanged
            Assert.Equal("old_url", user.UrlAvatar); // Should remain unchanged
        }

        [Fact]
        public async Task Handle_Should_ThrowOperationCanceledException_When_CancellationRequested()
        {
            // Arrange
            var userId = "user123";
            var dto = new UserUserUpdateDto
            {
                FullName = "New Name",
            };
            var command = new UpdateUserByUserCommand(dto);
            var cancellationToken = new CancellationToken(true); // Cancellation requested

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _handler.Handle(command, cancellationToken));
        }
    }
}
