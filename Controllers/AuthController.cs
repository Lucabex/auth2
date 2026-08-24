using Microsoft.AspNetCore.Mvc;
using auth2.Data;
using auth2.Models;

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
}