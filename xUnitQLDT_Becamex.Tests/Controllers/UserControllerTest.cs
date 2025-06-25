using Moq;
using QLDT_Becamex.Src.Services.Interfaces;
using QLDT_Becamex.Src.Services.Implementations;
using QLDT_Becamex.Src.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Text;
using Newtonsoft.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using QLDT_Becamex.Src.Application.Dtos;

namespace xUnitQLDT_Becamex.Tests.Controllers
{
    public class UserControllerTest
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UsersController _userController;
        public UserControllerTest()
        {
            // Tạo IConfiguration giả
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:Key", "your_fake_jwt_key_128bit_long_enough"},
                {"Jwt:Issuer", "your_fake_issuer"},
                {"Jwt:Audience", "your_fake_audience"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var jwtService = new JwtService(configuration);
            _userServiceMock = new Mock<IUserService>();
            _userController = new UsersController(_userServiceMock.Object, jwtService);

            // Thiết lập ControllerContext với ClaimsPrincipal
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Role, "USER")
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
            var userDtoRq = new UserDtoRq
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

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(userDtoRq);
            bool isValid = Validator.TryValidateObject(userDtoRq, validationContext, validationResults, true);

            Assert.True(isValid, "DTO should be valid: " + string.Join(", ", validationResults.Select(r => r.ErrorMessage)));

            var expectedResult = Result.Success(
                message: "Đăng ký thành công.",
                code: "REGISTER_SUCCESS",
                statusCode: 200
            );

            _userServiceMock
                .Setup(s => s.CreateUserAsync(It.Is<UserDtoRq>(dto => dto.Email == userDtoRq.Email)))
                .ReturnsAsync(expectedResult);

            // Act
            var response = await _userController.CreateUser(userDtoRq);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, okResult.StatusCode);

            var responseValue = okResult.Value;
            Assert.NotNull(responseValue);

            var message = responseValue.GetType().GetProperty("message")?.GetValue(responseValue)?.ToString();
            var statusCode = responseValue.GetType().GetProperty("statusCode")?.GetValue(responseValue)?.ToString();
            var code = responseValue.GetType().GetProperty("code")?.GetValue(responseValue)?.ToString();

            Assert.Equal("Đăng ký thành công.", message);
            Assert.Equal("200", statusCode);
            Assert.Equal("REGISTER_SUCCESS", code);

            _userServiceMock.Verify(s => s.CreateUserAsync(It.IsAny<UserDtoRq>()), Times.Once());
        }
        [Fact]
        public async Task CreateUser_ServiceFailure_ReturnsBadRequest()
        {
            // Arrange
            var userDtoRq = new UserDtoRq
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

            var expectedResult = Result.Failure(
                error: "Email đã được sử dụng.",
                message: "Email đã tồn tại.",
                code: "EMAIL_EXISTS",
                statusCode: 400
            );

            _userServiceMock
                .Setup(s => s.CreateUserAsync(It.Is<UserDtoRq>(dto => dto.Email == userDtoRq.Email)))
                .ReturnsAsync(expectedResult);

            // Act
            var response = await _userController.CreateUser(userDtoRq);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal(400, badRequestResult.StatusCode);

            var responseValue = badRequestResult.Value;
            var message = responseValue.GetType().GetProperty("message")?.GetValue(responseValue)?.ToString();
            var errors = responseValue.GetType().GetProperty("errors")?.GetValue(responseValue) as IEnumerable<string>;
            var code = responseValue.GetType().GetProperty("code")?.GetValue(responseValue)?.ToString();
            var statusCode = responseValue.GetType().GetProperty("statusCode")?.GetValue(responseValue)?.ToString();

            Assert.Equal("Email đã tồn tại.", message);
            Assert.Contains("Email đã được sử dụng.", errors);
            Assert.Equal("EMAIL_EXISTS", code);
            Assert.Equal("400", statusCode);

            _userServiceMock.Verify(s => s.CreateUserAsync(It.IsAny<UserDtoRq>()), Times.Once());
        }
        [Fact]
        public async Task Login_ReturnsOk()
        {
            // Arrange
            var request = new UserLoginRq
            {
                Email = "user@becamex.com",
                Password = "Password123!"
            };

            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);
            Assert.True(isValid,
                $"ModelState should be valid. Errors: {string.Join(", ", validationResults.Select(r => r.ErrorMessage))}");

            var userDto = new UserDto
            {
                Id = "123",
                Email = "user@becamex.com",
                FullName = "Nguyen Van Admin",
                Role = "ADMIN"
            };

            var expectedResult = Result<UserDto>.Success(
                message: "Đăng nhập thành công.",
                code: "SUCCESS",
                statusCode: 200,
                data: userDto
            );

            _userServiceMock
                .Setup(x => x.LoginAsync(It.Is<UserLoginRq>(r => r.Email == request.Email && r.Password == request.Password)))
                .ReturnsAsync(expectedResult);

            // Act
            IActionResult result;
            try
            {
                result = await _userController.Login(request);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception: {ex.Message}\n{ex.StackTrace}");
                return;
            }

            // Assert
            var objectResult = Assert.IsAssignableFrom<ObjectResult>(result);
            if (objectResult.StatusCode != 200)
            {
                Console.WriteLine($"Response: {JsonConvert.SerializeObject(objectResult.Value)}");
                Assert.Fail($"Expected StatusCode 200, but got {objectResult.StatusCode}");
            }
            Assert.Equal(200, objectResult.StatusCode);

            var responseValue = objectResult.Value;
            Assert.NotNull(responseValue);

            var message = responseValue.GetType().GetProperty("message")?.GetValue(responseValue)?.ToString();
            var statusCode = responseValue.GetType().GetProperty("statusCode")?.GetValue(responseValue)?.ToString();
            var code = responseValue.GetType().GetProperty("code")?.GetValue(responseValue)?.ToString();
            var data = responseValue.GetType().GetProperty("data")?.GetValue(responseValue) as UserDto;
            var accessToken = responseValue.GetType().GetProperty("accessToken")?.GetValue(responseValue)?.ToString();

            Assert.Equal("Đăng nhập thành công.", message);
            Assert.Equal("200", statusCode);
            Assert.Equal("SUCCESS", code);
            Assert.NotNull(data);
            Assert.Equal(userDto.Id, data.Id);
            Assert.Equal(userDto.Email, data.Email);
            Assert.Equal(userDto.FullName, data.FullName);
            Assert.Equal(userDto.Role, data.Role);
            Assert.NotNull(accessToken);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(accessToken) as JwtSecurityToken;
            Assert.NotNull(jwtToken);
            var claims = jwtToken.Claims;
            Assert.Equal("123", claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            Assert.Equal("user@becamex.com", claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value);
            Assert.Equal("ADMIN", claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);

            _userServiceMock.Verify(x => x.LoginAsync(It.Is<UserLoginRq>(r => r.Email == request.Email && r.Password == request.Password)), Times.Once());
        }
        [Fact]
        public async Task GetUserById_ReturnsOk()
        {
            // Arrange
            var userId = "1";
            var userDto = new UserDto
            {
                Id = "1",
                FullName = "Test",
                IdCard = "123456789012",
                Code = "EMP123456789",
                StartWork = DateTime.Now,
                EndWork = null,
                Email = "test@becamex.com"
            };

            var expectedResult = Result<UserDto>.Success(
                message: "Lấy thông tin người dùng thành công.",
                code: "SUCCESS",
                statusCode: 200,
                data: userDto
            );

            _userServiceMock
                .Setup(x => x.GetUserAsync(userId))
                .ReturnsAsync(expectedResult);

            // Kiểm tra id không null hoặc rỗng
            Assert.False(string.IsNullOrEmpty(userId), "User ID should not be null or empty");

            // Act
            IActionResult response;
            try
            {
                response = await _userController.GetUserById(userId);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Unexpected exception: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return;
            }

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, okResult.StatusCode);

            var responseValue = okResult.Value;
            Assert.NotNull(responseValue);

            var message = responseValue.GetType().GetProperty("message")?.GetValue(responseValue)?.ToString();
            var statusCode = responseValue.GetType().GetProperty("statusCode")?.GetValue(responseValue)?.ToString();
            var code = responseValue.GetType().GetProperty("code")?.GetValue(responseValue)?.ToString();
            var data = responseValue.GetType().GetProperty("data")?.GetValue(responseValue) as UserDto;

            Assert.Equal("Lấy thông tin người dùng thành công.", message);
            Assert.Equal("200", statusCode);
            Assert.Equal("SUCCESS", code);
            Assert.NotNull(data);
            Assert.Equal("Test", data.FullName);
            Assert.Equal("1", data.Id);
            Assert.Equal("test@becamex.com", data.Email);

            _userServiceMock.Verify(x => x.GetUserAsync(userId), Times.Once());
        }
        [Fact]
        public async Task UpdateMyProfile_ReturnsOk()
        {
            // Arrange
            var request = new UserUpdateSelfDtoRq
            {
                FullName = "Ann",
                PhoneNumber = "0123456789",
                UrlAvatar = CreateMockFormFile("avatar.jpg", "image/jpeg")
            };

            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            Assert.True(isValid,
                $"ModelState should be valid for valid input. Errors: {string.Join(", ", validationResults.Select(r => r.ErrorMessage ?? "Unknown error"))}");

            var expectedResult = Result.Success(
                message: "Cập nhật người dùng thành công.",
                code: "USER_UPDATE_SUCCESS",
                statusCode: 200
            );

            _userServiceMock
                .Setup(x => x.GetCurrentUserAuthenticationInfo())
                .Returns(("test-user-id", null));

            _userServiceMock
                .Setup(x => x.UpdateMyProfileAsync("test-user-id", It.Is<UserUpdateSelfDtoRq>(rq =>
                    rq.FullName == request.FullName &&
                    rq.PhoneNumber == request.PhoneNumber &&
                    rq.UrlAvatar != null)))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _userController.UpdateMyProfile(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var responseValue = okResult.Value;
            Assert.NotNull(responseValue);

            var message = responseValue.GetType().GetProperty("message")?.GetValue(responseValue)?.ToString();
            var statusCode = responseValue.GetType().GetProperty("statusCode")?.GetValue(responseValue)?.ToString();
            var code = responseValue.GetType().GetProperty("code")?.GetValue(responseValue)?.ToString();

            Assert.Equal("Cập nhật người dùng thành công.", message);
            Assert.Equal("200", statusCode);
            Assert.Equal("USER_UPDATE_SUCCESS", code);

            _userServiceMock.Verify(x => x.UpdateMyProfileAsync("test-user-id", It.IsAny<UserUpdateSelfDtoRq>()), Times.Once());
            _userServiceMock.Verify(x => x.GetCurrentUserAuthenticationInfo(), Times.Once());
        }
        // Helper method to create a mock IFormFile
        private IFormFile CreateMockFormFile(string fileName, string contentType)
        {
            var content = "Fake image content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.ContentType).Returns(contentType);
            mockFile.Setup(f => f.Length).Returns(stream.Length);
            mockFile.Setup(f => f.Name).Returns(fileName); // Thêm mock cho Name
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream target, CancellationToken ct) => stream.CopyToAsync(target, ct));
            mockFile.Setup(f => f.CopyTo(It.IsAny<Stream>()))
                .Callback((Stream target) => stream.CopyTo(target));
            return mockFile.Object;
        }
        [Fact]
        public async Task UpdateUserByAdmin_ReturnsOk()
        {
            // Arrange
            string userId = "1";
            var request = new AdminUpdateUserDtoRq
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
                EndWork = DateTime.Now,
                Email = "test@becamex.com"
            };
            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            Assert.True(isValid,
                $"ModelState should be valid. Errors: {string.Join(", ", validationResults.Select(r => r.ErrorMessage))}");

            var expectedResult = Result.Success(
                message: "Cập nhật người dùng thành công.",
                code: "USER_UPDATE_SUCCESS",
                statusCode: 200
            );

            _userServiceMock
                .Setup(x => x.UpdateUserByAdmin(userId, request))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _userController.UpdateUserByAdmin(userId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var json = JsonConvert.SerializeObject(okResult.Value);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json)!;

            Assert.Equal("Cập nhật người dùng thành công.", dict["message"]!.ToString());
            Assert.Equal("USER_UPDATE_SUCCESS", dict["code"]!.ToString());
            Assert.Equal("200", dict["statusCode"]!.ToString());

            _userServiceMock.Verify(x => x.UpdateUserByAdmin(userId, request), Times.Once);
        }
        [Fact]
        public async Task GetAllUsers_ReturnsOk()
        {
            // Arrange
            var queryParams = new BaseQueryParam
            {
                Page = 1,
                Limit = 10,
                SortField = "created.at",
                SortType = "desc"
            };

            var data = new List<UserDto>
            {
                new UserDto
                {
                    Id = "1",
                    FullName = "Nguyen Van Test 1",
                    IdCard = "123456789012",
                    Code = "EMP123456789",
                    StartWork = DateTime.Now,
                    EndWork = null,
                    Email = "test1@becamex.com"
                },
                new UserDto
                {
                    Id = "2",
                    FullName = "Nguyen Van Test 2",
                    IdCard = "987654321098",
                    Code = "EMP987654321",
                    StartWork = DateTime.Now,
                    EndWork = null,
                    Email = "test2@becamex.com"
                }
            };

            var pagedResult = new PagedResult<UserDto>
            {
                Items = data,
                Pagination = new Pagination
                {
                    CurrentPage = queryParams.Page,
                    ItemsPerPage = queryParams.Limit,
                    TotalItems = data.Count,
                    TotalPages = (int)Math.Ceiling((double)data.Count / queryParams.Limit)
                }
            };

            var expectedResult = Result<PagedResult<UserDto>>.Success(
                message: "Lấy danh sách người dùng thành công.",
                code: "SUCCESS",
                statusCode: 200,
                data: pagedResult
            );

            _userServiceMock
                .Setup(x => x.GetUsersAsync(It.Is<BaseQueryParam>(q => q.Page == queryParams.Page && q.Limit == queryParams.Limit && q.SortField == queryParams.SortField && q.SortType == queryParams.SortType)))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _userController.GetAllUsers(queryParams);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode); // Kiểm tra mã trạng thái HTTP

            var responseValue = okResult.Value;
            Assert.NotNull(responseValue);

            var message = responseValue.GetType().GetProperty("message")?.GetValue(responseValue)?.ToString();
            var code = responseValue.GetType().GetProperty("code")?.GetValue(responseValue)?.ToString();
            var dataResult = responseValue.GetType().GetProperty("data")?.GetValue(responseValue) as PagedResult<UserDto>;

            Assert.Equal("Lấy danh sách người dùng thành công.", message);
            Assert.Equal("SUCCESS", code);
            Assert.NotNull(dataResult);
            Assert.Equal(2, dataResult.Items?.Count());
            Assert.Equal(2, dataResult.Pagination?.TotalItems);
            Assert.Equal(1, dataResult.Pagination?.CurrentPage);
            Assert.Equal(10, dataResult.Pagination?.ItemsPerPage);
            Assert.Equal(1, dataResult.Pagination?.TotalPages);
        }
        [Fact]
        public async Task ChangePassword_ReturnsOk()
        {
            // Arrange
            string userId = "4960bac1-35ae-4fdf-ae54-191d067267e2";
            var request = new UserChangePasswordRq
            {
                OldPassword = "123456",
                NewPassword = "test12344",
                ConfirmNewPassword = "test12344"
            };
            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            Assert.True(isValid,
                $"ModelState should be valid. Errors: {string.Join(", ", validationResults.Select(r => r.ErrorMessage))}");

            var expectedResult = Result.Success(
                message: "Cập nhật mật khẩu thành công.",
                code: "USER_UPDATE_SUCCESS",
                statusCode: 200
            );

            _userServiceMock
                .Setup(x => x.GetCurrentUserAuthenticationInfo())
                .Returns((userId, "USER"));

            _userServiceMock
                .Setup(x => x.ChangePasswordUserAsync(userId, request))
                .ReturnsAsync(expectedResult);


            // Act
            var result = await _userController.ChangePassword(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var json = JsonConvert.SerializeObject(okResult.Value);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json)!;

            Assert.Equal("Cập nhật mật khẩu thành công.", dict["message"]!.ToString());
            Assert.Equal("USER_UPDATE_SUCCESS", dict["code"]!.ToString());
            Assert.Equal("200", dict["statusCode"]!.ToString());

            _userServiceMock.Verify(x => x.ChangePasswordUserAsync(userId, request), Times.Once);
        }
        [Fact]
        public async Task ResetPasswordByAdmin_ReturnsOk()
        {
            // Arrange
            string userId = "4960bac1-35ae-4fdf-ae54-191d067267e2";
            var request = new UserResetPasswordRq
            {
                NewPassword = "test12344",
                ConfirmNewPassword = "test12344"
            };
            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            Assert.True(isValid,
                $"ModelState should be valid. Errors: {string.Join(", ", validationResults.Select(r => r.ErrorMessage))}");

            var expectedResult = Result.Success(
                message: "Cập nhật mật khẩu thành công.",
                code: "USER_UPDATE_SUCCESS",
                statusCode: 200
            );

            _userServiceMock
                .Setup(x => x.GetCurrentUserAuthenticationInfo())
                .Returns((userId, "USER"));

            _userServiceMock
                .Setup(x => x.ResetPasswordByAdminAsync(userId, request))
                .ReturnsAsync(expectedResult);


            // Act
            var result = await _userController.ResetPasswordByAdmin(userId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var json = JsonConvert.SerializeObject(okResult.Value);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json)!;

            Assert.Equal("Cập nhật mật khẩu thành công.", dict["message"]!.ToString());
            Assert.Equal("USER_UPDATE_SUCCESS", dict["code"]!.ToString());
            Assert.Equal("200", dict["statusCode"]!.ToString());

            _userServiceMock.Verify(x => x.ResetPasswordByAdminAsync(userId, request), Times.Once);
        }
        [Fact]
        public async Task SearchUser_ReturnsOk()
        {
            string keyword = "van";
            var queryParams = new BaseQueryParam
            {
                Page = 1,
                Limit = 10,
                SortField = "createdat",
                SortType = "desc"
            };

            var mockData = new List<UserDto>
            {
                new UserDto { Id = "1", Email = "van@becamex.com", FullName = "Nguyen Van A", Role = "ADMIN" },
                new UserDto { Id = "2", Email = "test@becamex.com", FullName = "Tran Van B", Role = "HR" }
            };

            var pagination = new Pagination
            {
                CurrentPage = 1,
                ItemsPerPage = 10,
                TotalItems = 2,
                TotalPages = 1
            };

            var pagedResult = new PagedResult<UserDto>
            {
                Items = mockData,
                Pagination = pagination
            };

            var expectedResult = Result<PagedResult<UserDto>>.Success(
                data: pagedResult,
                message: "User list retrieved successfully.",
                code: "SUCCESS",
                statusCode: 200
            );

            _userServiceMock
                .Setup(x => x.SearchUserAsync(keyword, queryParams))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _userController.SearchUser(keyword, queryParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var json = JsonConvert.SerializeObject(okResult.Value);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json)!;

            Assert.Equal("User list retrieved successfully.", dict["message"]!.ToString());
            Assert.Equal("SUCCESS", dict["code"]!.ToString());
            Assert.Equal("200", dict["statusCode"]!.ToString());
            Assert.NotNull(dict["data"]);

            _userServiceMock.Verify(x => x.SearchUserAsync(keyword, queryParams), Times.Once);
        }
        [Fact]
        public async Task SoftDetelePasswordByAdmin_ReturnsOk()
        {
            // Arrange
            string targetUserId = "user-to-delete-id";
            string currentUserId = "admin-id";

            var expectedResult = Result.Success(
                message: "Xóa người dùng thành công (soft delete).",
                code: "SOFT_DELETE_USER_SUCCESS",
                statusCode: 200
            );
            // can't test HR, TRAINEE
            _userServiceMock
                .Setup(x => x.GetCurrentUserAuthenticationInfo())
                .Returns((currentUserId, "ADMIN"));

            _userServiceMock
                .Setup(x => x.SoftDeleteUserAsync(targetUserId))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _userController.SoftDeleteUser(targetUserId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var json = JsonConvert.SerializeObject(okResult.Value);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json)!;

            Assert.Equal("Xóa người dùng thành công (soft delete).", dict["message"]!.ToString());
            Assert.Equal("SOFT_DELETE_USER_SUCCESS", dict["code"]!.ToString());
            Assert.Equal("200", dict["statusCode"]!.ToString());

            _userServiceMock.Verify(x => x.GetCurrentUserAuthenticationInfo(), Times.Once);
            _userServiceMock.Verify(x => x.SoftDeleteUserAsync(targetUserId), Times.Once);
        }
    }
}