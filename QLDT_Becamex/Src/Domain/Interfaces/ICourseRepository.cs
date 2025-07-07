using QLDT_Becamex.Src.Domain.Entities;


namespace QLDT_Becamex.Src.Domain.Interfaces
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<List<Course>> GetCoursesForStatusUpdateAsync(DateTime currentDate);
    }
}
