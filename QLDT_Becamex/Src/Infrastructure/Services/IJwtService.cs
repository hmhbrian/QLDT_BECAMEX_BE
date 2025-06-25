namespace QLDT_Becamex.Src.Infrastructure.Services
{
    public interface IJwtService
    {
        public string GenerateJwtToken(string id, string email, string role);
    }
}
