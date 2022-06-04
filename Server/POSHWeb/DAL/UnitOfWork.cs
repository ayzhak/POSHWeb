using Microsoft.EntityFrameworkCore;
using POSHWeb.Common.Model.Job;
using POSHWeb.Common.Model.Script;
using POSHWeb.Scheduler.Job;
using POSHWeb.Data;
using POSHWeb.Environment.Model;
using POSHWeb.Model;
using POSHWeb.Model.Job;
using POSHWeb.Model.Schedule;
using POSHWeb.Repository;

namespace POSHWeb.DAL
{
    public class UnitOfWork : IDisposable
    {
        private DatabaseContext context;
        private GenericRepository<JobSchedule> scheduleRepository;
        private GenericRepository<Job> jobRepository;
        private GenericRepository<JobRuns> jobRunsRepository;
        private GenericRepository<RunspaceSettings> runspaceRepository;
        private GenericRepository<PSScript> scriptRepository;
        private GenericRepository<Credentials> credentialsRepository;

        public UnitOfWork(DbContextOptions<DatabaseContext> options)
        {
            context = new DatabaseContext(options);
        }

        public GenericRepository<JobSchedule> ScheduleRepository
        {
            get
            {

                if (scheduleRepository == null)
                {
                    scheduleRepository = new GenericRepository<JobSchedule>(context);
                }
                return scheduleRepository;
            }
        }
        public GenericRepository<Job> JobRepository
        {
            get
            {

                if (jobRepository == null)
                {
                    jobRepository = new GenericRepository<Job>(context);
                }
                return jobRepository;
            }
        }
        public GenericRepository<JobRuns> JobRunsRepository
        {
            get
            {

                if (jobRunsRepository == null)
                {
                    jobRunsRepository = new GenericRepository<JobRuns>(context);
                }
                return jobRunsRepository;
            }
        }
        public GenericRepository<RunspaceSettings> RunspaceSettingsRepository
        {
            get
            {

                if (runspaceRepository == null)
                {
                    runspaceRepository = new GenericRepository<RunspaceSettings>(context);
                }
                return runspaceRepository;
            }
        }
        public GenericRepository<PSScript> ScriptRepository
        {
            get
            {

                if (scriptRepository == null)
                {
                    scriptRepository = new GenericRepository<PSScript>(context);
                }
                return scriptRepository;
            }
        }
        public GenericRepository<Credentials> CredentialsRepository
        {
            get
            {

                if (credentialsRepository == null)
                {
                    credentialsRepository = new GenericRepository<Credentials>(context);
                }
                return credentialsRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
