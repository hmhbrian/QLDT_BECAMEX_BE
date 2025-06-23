using QLDT_Becamex.Src.Dtos;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IDepartmentService
    {
        public Task<Result<DepartmentDto>> CreateDepartmentAsync(DepartmentRq dto);
        public Task<Result<List<DepartmentDto>>> GetAllDepartmentsAsync();
        public Task<Result<DepartmentDto>> GetDepartmentByIdAsync(int id);

        public Task<Result<DepartmentDto>> UpdateDepartmentAsync(int id, DepartmentRq request);
        public Task<Result<bool>> DeleteDepartmentAsync(int id);
    }
}
