using GoogleDrive.Database;
using GoogleDrive.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace GoogleDrive.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private string _salt = "92T//FF0CMgjbMEE6XpbbXok6y0Jcjpns2ejw6MDe+HJwyadiGDXqbz/NZNjRroZj5VXuB2OAy6dWr5PxoQbbpoUkRHK1g==";
    private GoogleDriveDbContext _context;

    public UserController(GoogleDriveDbContext context)
    {
        _context = context;
    }

    [HttpGet("register")]
    public async Task<ActionResult> Register(string login, string password)
    {
        string passwordHashed = HashPassword(password, _salt, 10101, 70);
        await _context.Users.AddAsync(new User { UserName = login, PasswordHash = passwordHashed });
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("login")]
    public async Task<ActionResult> Login(string login, string password)
    {
        var user = await _context.Users.AsNoTracking().SingleOrDefaultAsync(x => x.UserName == login);
        if (user == null)
        {
            return NotFound();
        }

        string passwordHashed = HashPassword(password, _salt, 10101, 70);
        if (user.PasswordHash != passwordHashed)
        {
            return BadRequest();
        }
        var token = "";
        return Ok(token);
    }

    [HttpGet("add-user")]
    public async Task<ActionResult> AddUser(string username, string filePath)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username);
        var file = await _context.Files.FirstOrDefaultAsync(x => x.Path == filePath);

        await _context.FileUsers.AddAsync(new FileUser { FileId = file.Id, UserId = user.Id });
        await _context.SaveChangesAsync();
        var token = "";
        return Ok(token);
    }

    private string GenerateSalt(int nSalt)
    {
        var saltBytes = new byte[nSalt];

        using (var provider = new RNGCryptoServiceProvider())
        {
            provider.GetNonZeroBytes(saltBytes);
        }

        return Convert.ToBase64String(saltBytes);
    }

    private string HashPassword(string password, string salt, int nIterations, int nHash)
    {
        var saltBytes = Convert.FromBase64String(salt);

        using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, nIterations))
        {
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(nHash));
        }
    }
}
