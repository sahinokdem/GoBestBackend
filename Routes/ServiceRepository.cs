using GoBest.Data;

namespace GoBest.Routes
{
    
    public class ServiceRepository
    {
        private readonly MyDbContext _db;

        public ServiceRepository(MyDbContext db)
        {
            _db = db;
        }

    }
}