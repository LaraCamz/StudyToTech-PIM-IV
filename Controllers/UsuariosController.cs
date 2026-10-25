using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyToTech.Api.DTOs;
using StudyToTech.Api.Services;

namespace StudyToTech.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Authorize(Roles = "Administrador")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _svc;

    public UsuariosController(UsuarioService svc) => _svc = svc;

    [HttpGet("perfis")]
    public async Task<ActionResult<IEnumerable<PerfilDto>>> ListarPerfis()
        => Ok(await _svc.ListarPerfisAsync());

    [HttpGet("usuarios")]
    public async Task<ActionResult<PaginaDto<UsuarioDto>>> Listar(
        [FromQuery] string? nome,
        [FromQuery] int? perfilId,
        [FromQuery] bool? ativo,
        [FromQuery] int page = 0,
        [FromQuery] int size = 20)
        => Ok(await _svc.ListarAsync(nome, perfilId, ativo, page, Math.Min(size, 100)));

    [HttpPost("usuarios")]
    public async Task<ActionResult<UsuarioDto>> Criar([FromBody] UsuarioCreate req)
    {
        var u = await _svc.CriarAsync(req);
        return Created($"/api/v1/usuarios/{u.Id}", u);
    }

    [HttpGet("usuarios/{usuarioId:int}")]
    public async Task<ActionResult<UsuarioDto>> Obter(int usuarioId)
        => Ok(await _svc.ObterAsync(usuarioId));

    [HttpPut("usuarios/{usuarioId:int}")]
    public async Task<ActionResult<UsuarioDto>> Atualizar(int usuarioId, [FromBody] UsuarioUpdate req)
        => Ok(await _svc.AtualizarAsync(usuarioId, req));

    [HttpPatch("usuarios/{usuarioId:int}/status")]
    public async Task<ActionResult<UsuarioDto>> Status(int usuarioId, [FromBody] AtivoRequest req)
        => Ok(await _svc.AlterarStatusAsync(usuarioId, req.Ativo));
}
