using QLDT_Becamex.Src.Dtos;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IPositionService
    {
        // Create
        public Task<ApiResponse> CreatePositionAsync(PositionRq dto);

        // Read
        public Task<Result<PositionDto>> GetPositionByIdAsync(int id);

        public Task<Result<IEnumerable<PositionDto>>> GetAllPositionsAsync();

        // Update
        public Task<ApiResponse> UpdatePositionAsync(int id, PositionRq dto);

        // Delete 
        public Task<ApiResponse> DeletePositionAsync(int id);
    }
}
