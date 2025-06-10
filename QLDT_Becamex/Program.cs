using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QLDT_Becamex.Src.Config;
using QLDT_Becamex.Src.Mappings;
using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.Repostitories.GenericRepository;
using QLDT_Becamex.Src.Repostitories.Implementations;
using QLDT_Becamex.Src.Repostitories.Interfaces;
using QLDT_Becamex.Src.Services.Implementations;
using QLDT_Becamex.Src.Services.Interfaces;
using QLDT_Becamex.Src.UnitOfWork;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// 1. Cấu hình kết nối CSDL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Thêm Identity Services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Cấu hình các policy về mật khẩu: TẮT HẾT YÊU CẦU PHỨC TẠP
    options.Password.RequireDigit = false;            // Không yêu cầu chữ số
    options.Password.RequireLowercase = false;        // Không yêu cầu chữ thường
    options.Password.RequireUppercase = false;        // Không yêu cầu chữ hoa
    options.Password.RequireNonAlphanumeric = false;  // Không yêu cầu ký tự đặc biệt
    options.Password.RequiredLength = 6;              // Vẫn giữ độ dài tối thiểu là 6
    options.Password.RequiredUniqueChars = 1;         // Vẫn yêu cầu ít nhất 1 ký tự độc đáo (mặc định)

    // Cấu hình lockout (khóa tài khoản): TẮT TÍNH NĂNG KHÓA TÀI KHOẢN
    options.Lockout.AllowedForNewUsers = false;       // Không cho phép tài khoản mới bị khóa
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(0); // Thời gian khóa là 0 phút (vô hiệu hóa)
    options.Lockout.MaxFailedAccessAttempts = int.MaxValue; // Số lần đăng nhập sai tối đa rất lớn (vô hiệu hóa thực tế)

    // Cấu hình User: GIỮ LẠI YÊU CẦU EMAIL DUY NHẤT
    options.User.RequireUniqueEmail = true;           // Vẫn yêu cầu email là duy nhất

    // Cấu hình Signin: TẮT YÊU CẦU XÁC NHẬN EMAIL/TÀI KHOẢN KHI ĐĂNG NHẬP
    options.SignIn.RequireConfirmedEmail = false;     // Không yêu cầu xác nhận email để đăng nhập
    options.SignIn.RequireConfirmedAccount = false;   // Không yêu cầu xác nhận tài khoản (bao gồm email/phone)
    options.SignIn.RequireConfirmedPhoneNumber = false; // Không yêu cầu xác nhận số điện thoại

})
.AddEntityFrameworkStores<ApplicationDbContext>() // Sử dụng Entity Framework Core để lưu trữ Identity
.AddDefaultTokenProviders(); // Cần thiết cho việc tạo token (reset mật khẩu, xác nhận email)


//JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
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

builder.Services.AddAuthorization();
// 3. Đăng ký AuthRepository của bạn

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

//Service
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<JwtService>();
//AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
});
var app = builder.Build();





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
