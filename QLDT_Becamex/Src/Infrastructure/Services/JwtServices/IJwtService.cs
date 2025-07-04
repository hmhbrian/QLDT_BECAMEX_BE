namespace QLDT_Becamex.Src.Infrastructure.Services.JwtServices
{
    public interface IJwtService
    {
        public string GenerateJwtToken(string id, string email, string role);
    }
}
