using GoBest.Data;
using GoBest.Models;
using Microsoft.EntityFrameworkCore;

namespace GoBest.Users;

public class UserRepository
{
    private readonly MyDbContext _db;

    public UserRepository(MyDbContext db)
    {
        _db = db;
    }
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _db.Users.ToListAsync();
    }
}