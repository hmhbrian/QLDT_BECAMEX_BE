using QLDT_Becamex.Src.Repostitories.Interfaces;

namespace QLDT_Becamex.Src.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        public IUserRepository UserRepository { get; }
        public IDepartmentRepository DepartmentRepository { get; }

        public Task<int> CompleteAsync();
    }
}
