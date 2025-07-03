using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Controllers;
using System.Threading;
using Xunit;
using QLDT_Becamex.Src.Application.Common.Dtos;

namespace xUnitQLDT_Becamex.Tests.Controllers
{
    public class UsersControllerTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly UsersController _userController;

        public UsersControllerTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _userController = new UsersController(_mediatorMock.Object);

            // Thiết lập ControllerContext với ClaimsPrincipal
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Role, "ADMIN")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };
            _userController.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task CreateUser_ReturnsOk()
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

            var randomId = Guid.NewGuid().ToString();

            _mediatorMock
                .Setup(m => m.Send(It.Is<CreateUserCommand>(c => c.Request == userDtoRq), It.IsAny<CancellationToken>()))
                .ReturnsAsync(randomId);

            // Act
            var response = await _userController.CreateUser(userDtoRq, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, okResult.StatusCode);
            var responseValue = okResult.Value;
            var apiResponse = Assert.IsType<ApiResponse<string>>(responseValue); // Kiểm tra kiểu ApiResponse<string>
            Assert.IsType<string>(apiResponse.Data); // Kiểm tra Data là string
            Assert.True(Guid.TryParse(apiResponse.Data, out _)); // Kiểm tra định dạng GUID
            _mediatorMock.Verify(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once());
        }
        
    }
}