using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyToTech.Api.DTOs;
using StudyToTech.Api.Helpers;
using StudyToTech.Api.Services;

namespace StudyToTech.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth) => _auth = auth;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
        => Ok(await _auth.LoginAsync(req));

    [HttpPost("registro")]
    [AllowAnonymous]
    public async Task<ActionResult<UsuarioDto>> Registro([FromBody] RegistroClienteRequest req)
    {
        var usuario = await _auth.RegistrarClienteAsync(req);
        return CreatedAtAction(nameof(Me), usuario);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> Me()
        => Ok(await _auth.GetMeAsync(CurrentUser.GetUserId(User)));
}
