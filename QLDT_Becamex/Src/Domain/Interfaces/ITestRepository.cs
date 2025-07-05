using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Domain.Interfaces
{
    public interface ITestRepository : IGenericRepository<Test>
    {
        public Task UpdatePositionAsync(string courseId, int fromPosition, int toPosition, int offset);
    }
}
