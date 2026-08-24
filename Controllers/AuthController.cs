using Microsoft.AspNetCore.Mvc;
using auth2.Data;
using auth2.Models;
using Microsoft.EntityFrameworkCore;
using auth2.Dto;
using Dto;

namespace auth2.Controllers;
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AppdbContext _context;

    public AuthController(AppdbContext context)
    {
        _context=context;
    
    }
  

    [HttpPost("reg")]
    public async Task <IActionResult> Register(RegisterDto dto)
    {
        if(await _context.Users.AnyAsync(u=> u.Name.ToLower() == dto.Name.ToLower())){
            return BadRequest("User already in use");
        };
        var user = new User
        {
            Name = dto.Name,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok("user Register");
    }
    [HttpPost("log")]   
    public async Task<IActionResult> Log(LogDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u=>u.Name.ToLower() == dto.Name.ToLower());

        if (user ==null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.HashedPassword))
        {
            return BadRequest("invalid user name or Password");
        }
        return Ok("User logged");
    }
}