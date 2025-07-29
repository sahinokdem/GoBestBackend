using GoBest.Data;
using Microsoft.AspNetCore.Mvc;

namespace GoBest.Users
{
    public class UsersController : ControllerBase
    {
        private readonly MyDbContext _context;

        public UsersController(MyDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }
    }

}