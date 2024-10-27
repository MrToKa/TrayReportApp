using ACSReportApp.Data.Common;

namespace TrayReportApp.Data.Repositories
{
    public class TrayReportAppDbRepository : Repository, ITrayReportAppDbRepository
    {
        public TrayReportAppDbRepository(TrayReportAppDbContext context)
        {
            this.Context = context;
        }
    }
}
