using QLDT_Becamex.Src.Dtos;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IDepartmentService
    {
        public Task<Result<DepartmentDto>> CreateDepartmentAsync(CreateDepartmentDto dto);
        public Task<Result<List<DepartmentDto>>> GetAllDepartmentsAsync();
    }
}
