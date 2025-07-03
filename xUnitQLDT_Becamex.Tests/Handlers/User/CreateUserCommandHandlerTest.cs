using Microsoft.AspNetCore.Identity;
using Moq;
using QLDT_Becamex.Src.Application.Commands.Users.CreateUser;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitQLDT_Becamex.Tests.Handlers.UserChangePasswordCommand
{
    public class CreateUserCommandHandlerTest
    {
        //Mock dêpndencies
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<RoleManager<IdentityRole>> _roleManager;

        //Handler to test
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTest()
        {
            // Khởi tạo giả UserManager và RoleManager bằng Moq
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _roleManager = new Mock<RoleManager<IdentityRole>>(new Mock<IRoleStore<IdentityRole>>().Object, null, null, null, null);

            // Khởi tạo handler cần test
            _handler = new CreateUserCommandHandler(_userManager.Object, _roleManager.Object);
        }

        //Test case "Xử lý yêu cầu hợp lệ trả về ID người dùng"
        [Fact]
        public async Task Handle_ValidRequest_ReturnsUserId()
        {
            // Arrange
            var userDtoRq = new UserCreateDto
            {
                FullName = "Nguyen Van Test",
                IdCard = "123456789012",
                Code = "EMP123456789",
                PositionId = 1,
                RoleId = "b22145f9-3184-4e8f-9e31-b33ad0d007c0",
                ManagerUId = null,
                DepartmentId = 1,
                StatusId = 1,
                NumberPhone = "0987654321",
                StartWork = DateTime.Now,
                EndWork = null,
                Email = "test@becamex.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var command = new CreateUserCommand(userDtoRq);

            var role = new IdentityRole { Name = "HOCVIEN" };
            _roleManager.Setup(r => r.FindByNameAsync("HOCVIEN")).ReturnsAsync(role);
            //_userManager.Setup(u => u.FindByEmailAsync(userDtoRq.Email)).ReturnsAsync(new ApplicationUser { Email = userDtoRq.Email});
            _userManager.Setup(u => u.FindByEmailAsync(userDtoRq.Email)).ReturnsAsync((ApplicationUser)null);
            _userManager.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), userDtoRq.Password)).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), role.Name)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsType<string>(result);
            Assert.True(Guid.TryParse(result, out _)); // Kiểm tra định dạng GUID
            _userManager.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), userDtoRq.Password), Times.Once());
            _userManager.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), role.Name), Times.Once());
        }

        [Fact]
        public async Task Handle_EmailExists_ThrowsAppException()
        {
            // Arrange
            var userDtoRq = new UserCreateDto
            {
                FullName = "Nguyen Van Test",
                Email = "test@becamex.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
                // Các trường khác...
            };

            var command = new CreateUserCommand(userDtoRq);
            var existingUser = new ApplicationUser { Email = userDtoRq.Email };
            _roleManager.Setup(r => r.FindByNameAsync("HOCVIEN")).ReturnsAsync(new IdentityRole { Name = "HOCVIEN" });
            _userManager.Setup(u => u.FindByEmailAsync(userDtoRq.Email)).ReturnsAsync(existingUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Email đã được sử dụng", exception.Message);
            Assert.Equal(409,exception.StatusCode);
        }

        [Fact]
        public async Task Handle_RoleNotFound_ThrowsAppException()
        {
            // Arrange
            var userDtoRq = new UserCreateDto
            {
                FullName = "Nguyen Van Test",
                Email = "test@becamex.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
                // Các trường khác...
            };

            var command = new CreateUserCommand(userDtoRq);
            _roleManager.Setup(r => r.FindByNameAsync("HOCVIEN")).ReturnsAsync((IdentityRole)null);
            _userManager.Setup(u => u.FindByEmailAsync(userDtoRq.Email)).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Role mặc định không tồn tại", exception.Message);
            Assert.Equal(500, exception.StatusCode);
        }
    }
}
