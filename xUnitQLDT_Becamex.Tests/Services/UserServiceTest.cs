using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;
using QLDT_Becamex.Src.Application.Dtos;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Models;
using QLDT_Becamex.Src.Services.Implementations;
using QLDT_Becamex.Src.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;


namespace xUnitQLDT_Becamex.Tests.Services
{
    public class UserServiceTest
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManager;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly Mock<RoleManager<IdentityRole>> _roleManager;
        private readonly Mock<ICloudinaryService> _cloudinaryService;
        private readonly UserService _userService;
        private readonly Mock<IJwtService> _jwtService;

        public UserServiceTest()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _signInManager = new Mock<SignInManager<ApplicationUser>>(
                _userManager.Object,
                contextAccessorMock.Object,
                userPrincipalFactoryMock.Object,
                null, null, null, null);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _roleManager = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);
            _mapper = new Mock<IMapper>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _cloudinaryService = new Mock<ICloudinaryService>();
            _jwtService = new Mock<IJwtService>();

            _userService = new UserService(
                _signInManager.Object,
                _userManager.Object,
                _roleManager.Object,
                _cloudinaryService.Object,
                _jwtService.Object,
                _mapper.Object,
                _unitOfWork.Object,
                _httpContextAccessor.Object


               );
        }
        [Fact]
        public async Task LoginAsync_ReturnsOk()
        {
            // Arrange
            var email = "test@becamex.com";
            var password = "Test12714";
            var userId = Guid.NewGuid().ToString();

            var user = new ApplicationUser
            {
                Id = userId,
                Email = email,
                UserName = email,
                FullName = "Test User",
                Position = new Position { PositionId = 1, PositionName = "Developer" },
                Department = new Department { DepartmentId = 1, DepartmentName = "IT" },
                ManagerU = null,
                UserStatus = new UserStatus { Id = 1, Name = "Active" }
            };

            var loginDto = new UserLoginRq
            {
                Email = email,
                Password = password
            };
            // Validation Check
            var context = new ValidationContext(loginDto, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(loginDto, context, results, true);
            if (!isValid)
            {
                var errorMessages = string.Join("; ", results.Select(r => r.ErrorMessage));
                throw new ValidationException($"Validation failed: {errorMessages}");
            }
            Assert.True(isValid);
            Assert.Empty(results);

            var roles = new List<string> { "Admin" };
            var positionDto = new PositionDto { PositionId = 1, PositionName = "Developer" };
            var userDto = new UserDto
            {
                Id = userId,
                Email = email,
                FullName = "Test User",
                Role = "Admin",
                Position = positionDto
            };

            var userList = new List<ApplicationUser> { user }.AsQueryable()
                .BuildMock();
            // Setup
            _userManager.Setup(um => um.Users).Returns(userList);
            _signInManager.Setup(sm => sm.CheckPasswordSignInAsync(user, password, false))
                .ReturnsAsync(SignInResult.Success);
            _userManager.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(roles);
            _mapper.Setup(m => m.Map<PositionDto>(user.Position)).Returns(positionDto);
            _mapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _userService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal("Đăng nhập thành công.", result.Message);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal(userDto.Email, result.Data!.Email);
            Assert.Equal("Admin", result.Data.Role);
        }
        [Fact]
        public async Task CreateUserAsync_ReturnOk()
        {
            // Arrange
            var dto = new UserDtoRq
            {
                FullName = "Test User",
                Email = "test@becamex.com",
                Password = "Test@123",
                ConfirmPassword = "Test@123",
                RoleId = "role-123",
                DepartmentId = 1,
                PositionId = 1,
                NumberPhone = "0123456789",
                IdCard = "1234567890",
                Code = "ABC1234567",
                StartWork = DateTime.Now
            };

            // Validate DTO
            var context = new ValidationContext(dto, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);
            if (!isValid)
            {
                var errors = string.Join("; ", results.Select(r => r.ErrorMessage));
                throw new ValidationException("Validation failed: " + errors);
            }

            var identityRole = new IdentityRole { Id = dto.RoleId, Name = "HOCVIEN" };
            _roleManager.Setup(r => r.FindByIdAsync(dto.RoleId)).ReturnsAsync(identityRole);
            _userManager.Setup(u => u.FindByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser?)null);
            _userManager.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password)).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), identityRole.Name)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.CreateUserAsync(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Đăng ký thành công.", result.Message);
        }
        [Fact]
        public async Task UpdateUserByAdmin_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var dto = new AdminUpdateUserDtoRq
            {
                FullName = "Updated User",
                Email = "updated@becamex.com",
                NewPassword = "NewPass@123",
                ConfirmNewPassword = "NewPass@123",
                RoleId = "role-456",
                DepartmentId = 2,
                PositionId = 2,
                NumberPhone = "0987654321",
                Code = "CODE4567890",
                IdCard = "ID45678904234",
                StartWork = DateTime.Now,
                EndWork = DateTime.Now.AddMonths(6)
            };
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);
            if (!isValid)
            {
                var errorMessages = string.Join("; ", validationResults.Select(r => r.ErrorMessage));
                throw new ValidationException($"Validation failed: {errorMessages}");
            }
            Assert.True(isValid);
            Assert.Empty(validationResults);
            var user = new ApplicationUser
            {
                Id = userId,
                Email = "old@becamex.com",
                UserName = "old@becamex.com",
                FullName = "Old Name"
            };

            var identityRole = new IdentityRole { Id = dto.RoleId!, Name = "USER" };
            // Mock user store
            _userManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManager.Setup(u => u.FindByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser?)null);
            _userManager.Setup(u => u.SetEmailAsync(It.IsAny<ApplicationUser>(), dto.Email)).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(u => u.SetUserNameAsync(It.IsAny<ApplicationUser>(), dto.Email.ToLowerInvariant())).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(u => u.RemovePasswordAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(u => u.AddPasswordAsync(It.IsAny<ApplicationUser>(), dto.NewPassword)).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(u => u.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("reset-token");
            _userManager.Setup(u => u.ResetPasswordAsync(It.IsAny<ApplicationUser>(), "reset-token", dto.NewPassword)).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string> { "OLDROLE" });
            _userManager.Setup(u => u.RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), identityRole.Name!)).ReturnsAsync(IdentityResult.Success);

            // Mock IQueryable<User> for EF async support
            var users = new List<ApplicationUser>().AsQueryable().BuildMock(); // empty: no duplicate
            _userManager.Setup(u => u.Users).Returns(users);

            // Mock role store
            _roleManager.Setup(r => r.FindByIdAsync(dto.RoleId)).ReturnsAsync(identityRole);

            // Unit of work save
            _unitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _userService.UpdateUserByAdmin(userId, dto);

            // Assert (hiện lỗi cụ thể nếu có)
            if (!result.IsSuccess)
            {
                var errorDetail = result.Errors != null && result.Errors.Any()
                    ? string.Join(" | ", result.Errors)
                    : "Không rõ lỗi";

                Assert.False(true, $"❌ UpdateUserByAdmin failed: {result.Message} → {errorDetail}");
            }

            Assert.NotNull(result);
            Assert.True(result.IsSuccess, result.Message);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Cập nhật người dùng thành công.", result.Message);
        }
        [Fact]
        public async Task UpdateMyProfileAsync_ReturnsOk()
        {
            var userId = Guid.NewGuid().ToString();
            var dto = new UserUpdateSelfDtoRq
            {
                FullName = "New Name",
                PhoneNumber = "0987654321",
                UrlAvatar = new FormFile(Stream.Null, 0, 0, "file", "avatar.jpg")
            };

            // Validate DTO 
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);
            if (!isValid)
            {
                var errorMessages = string.Join("; ", validationResults.Select(r => r.ErrorMessage));
                throw new ValidationException($"Validation failed: {errorMessages}");
            }
            Assert.True(isValid);
            Assert.Empty(validationResults);

            // Mock User
            var user = new ApplicationUser
            {
                Id = userId,
                FullName = "Old Name",
                PhoneNumber = "0123456789",
                UrlAvatar = null
            };

            _userManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);

            // Mock Cloudinary Upload
            _cloudinaryService.Setup(c => c.UploadImageAsync(dto.UrlAvatar!))
                .ReturnsAsync("http://mock.cloudinary/avatar.jpg");

            // Mock Save
            _unitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _userService.UpdateMyProfileAsync(userId, dto);

            // Assert
            if (!result.IsSuccess)
            {
                var errorDetail = result.Errors != null && result.Errors.Any()
                    ? string.Join(" | ", result.Errors)
                    : "Không rõ lỗi";

                Assert.False(true, $"❌ UpdateUserByAdmin failed: {result.Message} → {errorDetail}");
            }

            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Cập nhật người dùng thành công.", result.Message);
            Assert.Equal("New Name", user.FullName);
            Assert.Equal("0987654321", user.PhoneNumber);
            Assert.Equal("http://mock.cloudinary/avatar.jpg", user.UrlAvatar);
        }
        [Fact]
        public async Task SoftDeleteUserAsync_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new ApplicationUser
            {
                Id = userId,
                FullName = "Nguyễn Văn A",
                IsDeleted = false,
                ModifiedAt = null
            };

            _userManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManager.Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.SoftDeleteUserAsync(userId);

            // Assert
            if (!result.IsSuccess)
            {
                var errorDetail = result.Errors != null && result.Errors.Any()
                    ? string.Join(" | ", result.Errors)
                    : "Không rõ lỗi";

                Assert.False(true, $"❌ UpdateUserByAdmin failed: {result.Message} → {errorDetail}");
            }

            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("SOFT_DELETE_USER_SUCCESS", result.Code);
            Assert.True(user.IsDeleted);
            Assert.NotNull(user.ModifiedAt);
        }
        [Fact]
        public async Task SoftDeleteUserAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            _userManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _userService.SoftDeleteUserAsync(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("USER_NOT_FOUND", result.Code);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("không tồn tại", result.Errors.FirstOrDefault() ?? "", StringComparison.OrdinalIgnoreCase);
        }
        [Fact]
        public async Task SoftDeleteUserAsync_UserAlreadyDeleted_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new ApplicationUser { Id = userId, IsDeleted = true };

            _userManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.SoftDeleteUserAsync(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("USER_ALREADY_DELETED", result.Code);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("đã bị xóa", result.Errors.FirstOrDefault() ?? "", StringComparison.OrdinalIgnoreCase);
        }
        [Fact]
        public async Task GetUsersAsync_ReturnsOk()
        {
            // Arrange
            var queryParams = new BaseQueryParam
            {
                Page = 1,
                Limit = 10,
                SortField = "email",
                SortType = "asc"
            };

            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", Email = "user1@example.com" },
                new ApplicationUser { Id = "2", Email = "user2@example.com" }
            };

            var userDtos = new List<UserDto>
            {
                new UserDto { Id = "1", Email = "user1@example.com" },
                new UserDto { Id = "2", Email = "user2@example.com" }
            };

            // Mock CountAsync
            _unitOfWork.Setup(u => u.UserRepository.CountAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(users.Count);

            // Mock GetFlexibleAsync
            _unitOfWork.Setup(u => u.UserRepository.GetFlexibleAsync(
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),                                // predicate
                It.IsAny<Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>>(),  // orderBy
                queryParams.Page,                                                                   // page
                queryParams.Limit,                                                                  // pageSize
                true,                                                                                // asNoTracking
                It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>>()          // includes
            )).ReturnsAsync(users);

            // Mock mapper
            _mapper.Setup(m => m.Map<List<UserDto>>(users)).Returns(userDtos);

            // Mock roles
            _userManager.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await _userService.GetUsersAsync(queryParams);

            // Assert
            if (!result.IsSuccess)
            {
                var errorDetail = result.Errors != null && result.Errors.Any()
                    ? string.Join(" | ", result.Errors)
                    : "Không rõ lỗi";

                Assert.False(true, $"❌ UpdateUserByAdmin failed: {result.Message} → {errorDetail}");
            }

            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("GET_ALL_USERS_SUCCESS", result.Code);
            Assert.NotNull(result.Data);
            Assert.Equal(users.Count, result.Data!.Items!.Count());
            Assert.All(result.Data.Items!, item => Assert.Equal("User", item.Role));
            Assert.NotNull(result.Data.Pagination);
            Assert.Equal(users.Count, result.Data.Pagination!.TotalItems);
        }
        [Fact]
        public async Task GetUserAsync_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();

            var user = new ApplicationUser
            {
                Id = userId,
                Email = "user@example.com",
                FullName = "Nguyễn Văn A",
                Position = new Position(),
                Department = new Department(),
                UserStatus = new UserStatus(),
                ManagerU = new ApplicationUser { Id = "manager-1" }
            };

            var userDto = new UserDto
            {
                Id = userId,
                Email = user.Email,
                FullName = user.FullName,
                Role = "User"
            };

            _unitOfWork.Setup(u => u.UserRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>>()))
                .ReturnsAsync(user);

            _userManager.Setup(u => u.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "User" });

            _mapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _userService.GetUserAsync(userId);

            // Assert
            if (!result.IsSuccess)
            {
                var errorDetail = result.Errors != null && result.Errors.Any()
                    ? string.Join(" | ", result.Errors)
                    : "Không rõ lỗi";

                Assert.False(true, $"❌ UpdateUserByAdmin failed: {result.Message} → {errorDetail}");
            }

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("User", result.Data!.Role);
            Assert.Equal(user.Email, result.Data.Email);
            Assert.Equal("SUCCESS", result.Code);
            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public async Task ChangePasswordUserAsync_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var rq = new UserChangePasswordRq
            {
                OldPassword = "OldPass123!",
                NewPassword = "NewPass456!",
                ConfirmNewPassword = "NewPass456!"
            };

            // Validate DTO
            var context = new ValidationContext(rq);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(rq, context, results, true);
            if (!isValid)
            {
                var errorMessages = string.Join("; ", results.Select(r => r.ErrorMessage));
                throw new ValidationException($"Validation failed: {errorMessages}");
            }
            Assert.True(isValid);
            Assert.Empty(results);

            // Mock user
            var user = new ApplicationUser { Id = userId, Email = "user@example.com" };

            _userManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManager.Setup(u => u.ChangePasswordAsync(user, rq.OldPassword, rq.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.ChangePasswordUserAsync(userId, rq);

            // Assert
            if (!result.IsSuccess)
            {
                var errorDetail = result.Errors != null && result.Errors.Any()
                    ? string.Join(" | ", result.Errors)
                    : "Không rõ lỗi";

                Assert.False(true, $"❌ UpdateUserByAdmin failed: {result.Message} → {errorDetail}");
            }

            Assert.True(result.IsSuccess);
            Assert.Equal("SUCCESS", result.Code);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Password changed successfully!", result.Message);
        }
        [Fact]
        public async Task ResetPasswordByAdminAsync_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var rq = new UserResetPasswordRq
            {
                NewPassword = "NewPassword@123",
                ConfirmNewPassword = "NewPassword@123"
            };

            // Validate DTO
            var context = new ValidationContext(rq);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(rq, context, results, true);
            if (!isValid)
            {
                var errorMessages = string.Join("; ", results.Select(r => r.ErrorMessage));
                throw new ValidationException($"Validation failed: {errorMessages}");
            }
            Assert.True(isValid);
            Assert.Empty(results);

            // Mock User
            var user = new ApplicationUser { Id = userId, Email = "admin@example.com" };

            _userManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManager.Setup(u => u.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("reset-token");
            _userManager.Setup(u => u.ResetPasswordAsync(user, "reset-token", rq.NewPassword)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.ResetPasswordByAdminAsync(userId, rq);

            // Assert
            if (!result.IsSuccess)
            {
                var errorDetail = result.Errors != null && result.Errors.Any()
                    ? string.Join(" | ", result.Errors)
                    : "Không rõ lỗi";

                Assert.False(true, $"❌ UpdateUserByAdmin failed: {result.Message} → {errorDetail}");
            }

            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("SUCCESS", result.Code);
            Assert.Equal("Password reset successfully.", result.Message);
        }
        [Fact]
        public async Task SearchUserAsync_ReturnsOk()
        {
            // Arrange
            var keyword = "Nguyen";
            var queryParams = new BaseQueryParam
            {
                Page = 1,
                Limit = 2,
                SortField = "createdAt",
                SortType = "desc"
            };

            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", FullName = "Nguyễn Văn A", Email = "a@example.com", CreatedAt = DateTime.UtcNow.AddDays(-1), IsDeleted = false },
                new ApplicationUser { Id = "2", FullName = "Nguyen Van B", Email = "b@example.com", CreatedAt = DateTime.UtcNow, IsDeleted = false },
                new ApplicationUser { Id = "3", FullName = "Tran Van C", Email = "c@example.com", CreatedAt = DateTime.UtcNow.AddDays(-3), IsDeleted = true } // Bị lọc
            };

            var queryableUsers = users.AsQueryable().BuildMock(); // MockQueryable.Moq

            _userManager.Setup(u => u.Users).Returns(queryableUsers);

            // Mock mapping
            _mapper.Setup(m => m.Map<List<UserDto>>(It.IsAny<List<ApplicationUser>>()))
                .Returns<List<ApplicationUser>>(list => list.Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FullName
                }).ToList());

            _userManager.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await _userService.SearchUserAsync(keyword, queryParams);

            // Assert
            if (!result.IsSuccess)
            {
                var errorDetail = result.Errors != null && result.Errors.Any()
                    ? string.Join(" | ", result.Errors)
                    : "Không rõ lỗi";

                Assert.False(true, $"❌ UpdateUserByAdmin failed: {result.Message} → {errorDetail}");
            }

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data!.Items!.Count());

            Assert.All(result.Data.Items!, user =>
            {
                Assert.NotNull(user.FullName);
                Assert.Equal("User", user.Role);
            });

            Assert.NotNull(result.Data.Pagination);
            Assert.Equal(2, result.Data.Pagination!.ItemsPerPage);
        }
    }
}
