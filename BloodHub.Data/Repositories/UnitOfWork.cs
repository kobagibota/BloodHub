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
