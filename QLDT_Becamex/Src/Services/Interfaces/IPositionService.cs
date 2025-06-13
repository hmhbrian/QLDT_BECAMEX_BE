using QLDT_Becamex.Src.Dtos.Positions;
using QLDT_Becamex.Src.Dtos.Results;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IPositionService
    {
        // Create
        public Task<Result> CreatePositionAsync(PositionRq dto);

        // Read
        public Task<Result<PositionDto>> GetPositionByIdAsync(int id);

        public Task<Result<IEnumerable<PositionDto>>> GetAllPositionsAsync();

        // Update
        public Task<Result> UpdatePositionAsync(int id, PositionRq dto);

        // Delete 
        public Task<Result> DeletePositionAsync(int id);
    }
}
