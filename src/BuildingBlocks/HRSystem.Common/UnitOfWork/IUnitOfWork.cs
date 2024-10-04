namespace HRSystem.Common.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}
