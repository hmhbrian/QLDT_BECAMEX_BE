namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IJwtService
    {
        public string GenerateJwtToken(string id, string email, string role);
    }
}
