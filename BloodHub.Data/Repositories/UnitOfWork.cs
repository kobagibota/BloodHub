using BloodHub.Data.Data;
using BloodHub.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BloodHub.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Private member

        private readonly AppDbContext _dbContext;

        private IUserRepository? _userRepository;
        private IRoleRepository? _roleRepository;

        private IAuthTokenRepository? _authTokenRepository;
        private IActivityLogRepository? _activityLogRepository;
        private IChangeLogRepository? _changeLogRepository;

        private IDoctorRepository? _doctorRepository;
        private INursingRepository? _nursingRepository;
        private IWardRepository? _wardRepository;
        private IProductRepository? _productRepository;
        private IPatientRepository? _patientRepository;


        #endregion

        #region Contrastor

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Properties

        public IUserRepository UserRepository
        {
            get { return _userRepository ??= new UserRepository(_dbContext); }
        }

        public IRoleRepository RoleRepository
        {
            get { return _roleRepository ??= new RoleRepository(_dbContext); }
        }

        public IAuthTokenRepository AuthTokenRepository
        {
            get { return _authTokenRepository ??= new AuthTokenRepository(_dbContext); }
        }

        public IActivityLogRepository ActivityLogRepository
        {
            get { return _activityLogRepository ??= new ActivityLogRepository(_dbContext); }
        }

        public IChangeLogRepository ChangeLogRepository
        {
            get { return _changeLogRepository ??= new ChangeLogRepository(_dbContext); }
        }

        public IDoctorRepository DoctorRepository
        {
            get { return _doctorRepository ??= new DoctorRepository(_dbContext); }
        }

        public INursingRepository NursingRepository
        {
            get { return _nursingRepository ??= new NursingRepository(_dbContext); }
        }

        public IWardRepository WardRepository
        {
            get { return _wardRepository ??= new WardRepository(_dbContext); }
        }

        public IProductRepository ProductRepository
        {
            get { return _productRepository ??= new ProductRepository(_dbContext); }
        }

        public IPatientRepository PatientRepository
        {
            get { return _patientRepository ??= new PatientRepository(_dbContext); }
        }

        #endregion

        #region Methods

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }
        public void Rollback()
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }
        }

        public async Task RollbackAsync()
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }
            await Task.CompletedTask;
        }

        #endregion
    }
}
