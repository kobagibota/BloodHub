namespace BloodHub.Shared.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IRoleRepository RoleRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        IAuthTokenRepository AuthTokenRepository { get; }
        IActivityLogRepository ActivityLogRepository { get; }

        IChangeLogRepository ChangeLogRepository { get; }

        IDoctorRepository DoctorRepository { get; }
        INursingRepository NursingRepository { get; }
        IWardRepository WardRepository { get; }
        IProductRepository ProductRepository { get; }
        IPatientRepository PatientRepository { get; }
        IOrderRepository OrderRepository { get; }
        IShiftRepository ShiftRepository { get; }
        IShiftDetailRepository ShiftDetailRepository { get; }
        IShiftUserRepository ShiftUserRepository { get; }
        ICrossmatchRepository CrossmatchRepository { get; }
        IInventoryRepository InventoryRepository { get; }
        ITransactionDetailRepository TransactionDetailRepository { get; }
        ITransactionRepository TransactionRepository { get; }


        void Save();
        Task SaveAsync();

        void Rollback();
        Task RollbackAsync();
    }
}
