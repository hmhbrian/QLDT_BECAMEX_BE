namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(string id, string email, string role);
    }
}
