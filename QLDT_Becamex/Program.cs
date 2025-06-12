using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QLDT_Becamex.Src.Config;
using QLDT_Becamex.Src.Mappings;
using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.Repostitories.Implementations;
using QLDT_Becamex.Src.Repostitories.Interfaces;
using QLDT_Becamex.Src.Services.Implementations;
using QLDT_Becamex.Src.Services.Interfaces;
using QLDT_Becamex.Src.UnitOfWork;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- Cấu hình Services ---
// Các dịch vụ được thêm vào container Dependency Injection.

// 1. Cấu hình Database Context và Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Cấu hình các policy về mật khẩu
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Cấu hình lockout (khóa tài khoản)
    options.Lockout.AllowedForNewUsers = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(0);
    options.Lockout.MaxFailedAccessAttempts = int.MaxValue;

    // Cấu hình User
    options.User.RequireUniqueEmail = true;

    // Cấu hình Signin
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 2. Cấu hình Authentication (JWT) và Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Để trả về 401 Unauthorized thay vì redirect
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization(); // Đăng ký dịch vụ ủy quyền

// 3. Đăng ký HttpContextAccessor (Cần thiết cho UserService lấy thông tin user hiện tại)
builder.Services.AddHttpContextAccessor(); // <-- Đã thêm

// 4. Đăng ký Unit of Work, Repositories và Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IPositionRepostiory, PositionRepository>();

// Services
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<JwtService>(); // Dịch vụ JWT

// 5. Cấu hình AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// 6. Cấu hình Controllers và Swagger/OpenAPI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Khám phá các endpoint cho Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "QLDT Becamex API", Version = "v1" });
    // Tùy chọn: Thêm hỗ trợ JWT cho Swagger UI để có thể thử API được bảo vệ
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập 'Bearer ' và token JWT của bạn vào đây (ví dụ: 'Bearer YOUR_TOKEN')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// --- Cấu hình HTTP Request Pipeline (Middleware) ---
// Thứ tự của các middleware rất quan trọng.

// 1. Cấu hình cho môi trường phát triển (Swagger)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. Middleware chuyển hướng HTTPS (Tùy chọn, hiện đang bị comment)
// app.UseHttpsRedirection();

// 3. Middleware định tuyến
app.UseRouting(); // Cần thiết nếu bạn muốn các middleware Authorization/Authentication hoạt động trước khi chọn endpoint

// 4. Middleware Authentication và Authorization
app.UseAuthentication(); // Xác thực người dùng (đọc token, cookie, v.v.)
app.UseAuthorization();  // Ủy quyền (kiểm tra quyền truy cập dựa trên [Authorize] attributes)

// 5. Định tuyến các Controller (Map endpoints)
app.MapControllers();

// 6. Chạy ứng dụng
app.Run();