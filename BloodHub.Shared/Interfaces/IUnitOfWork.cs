namespace BloodHub.Shared.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IRoleRepository RoleRepository { get; }
        IAuthTokenRepository AuthTokenRepository { get; }
        IActivityLogRepository ActivityLogRepository { get; }

        IChangeLogRepository ChangeLogRepository { get; }

        IDoctorRepository DoctorRepository { get; }

        void Save();
        Task SaveAsync();

        void Rollback();
        Task RollbackAsync();
    }
}
