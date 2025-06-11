using QLDT_Becamex.Src.Dtos;
using QLDT_Becamex.Src.Dtos.Departments;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IDepartmentService
    {
        public Task<Result<DepartmentDto>> CreateDepartmentAsync(DepartmentRq dto);
        public Task<Result<List<DepartmentDto>>> GetAllDepartmentsAsync();
    }
}
