
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QLDT_Becamex.Src.Infrastructure.Services.JwtServices
{
    /// <summary>
    /// Dịch vụ để tạo và quản lý JSON Web Tokens (JWT).
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Khởi tạo một phiên bản mới của lớp <see cref="JwtService"/>.
        /// </summary>
        /// <param name="configuration">Cấu hình ứng dụng để truy cập các khóa JWT.</param>
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Tạo một JWT (JSON Web Token) cho người dùng được chỉ định.
        /// </summary>
        /// <param name="id">ID của người dùng.</param>
        /// <param name="email">Email của người dùng.</param>
        /// <param name="role">Vai trò của người dùng.</param>
        /// <returns>Một chuỗi JWT đã được tạo.</returns>
        public string GenerateJwtToken(string id, string email, string role)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, id),
                    new Claim(JwtRegisteredClaimNames.Sub, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                if (!string.IsNullOrEmpty(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.ToUpper()));
                }

                // Đảm bảo rằng "Jwt:Key" không null hoặc rỗng
                var jwtKey = _configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(jwtKey))
                {
                    // Trong trường hợp này, vì phương thức trả về string, bạn có thể ném một ngoại lệ
                    // hoặc trả về một chuỗi rỗng/null, tùy thuộc vào cách bạn muốn xử lý lỗi cấu hình.
                    // Ném ngoại lệ là cách tốt hơn để báo hiệu lỗi cấu hình nghiêm trọng.
                    throw new InvalidOperationException("Cấu hình JWT Key không được tìm thấy.");
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.Now.AddDays(1);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("SYSTEM_ERROR: Đã xảy ra lỗi hệ thống khi tạo token.", ex);
            }
        }
    }
}