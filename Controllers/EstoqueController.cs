using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyToTech.Api.DTOs;
using StudyToTech.Api.Helpers;
using StudyToTech.Api.Services;

namespace StudyToTech.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Authorize(Roles = "Funcionário,Administrador")]
public class EstoqueController : ControllerBase
{
    private readonly EstoqueService _svc;

    public EstoqueController(EstoqueService svc) => _svc = svc;

    [HttpGet("estoque")]
    public async Task<ActionResult<PaginaDto<EstoqueItemDto>>> Consultar(
        [FromQuery] string? nome,
        [FromQuery] int? categoriaId,
        [FromQuery] bool? abaixoDoMinimo,
        [FromQuery] int page = 0,
        [FromQuery] int size = 20)
        => Ok(await _svc.ConsultarAsync(nome, categoriaId, abaixoDoMinimo, page, Math.Min(size, 100)));

    [HttpGet("estoque/alertas")]
    public async Task<ActionResult<IEnumerable<EstoqueItemDto>>> Alertas()
        => Ok(await _svc.AlertasAsync());

    [HttpGet("movimentacoes-estoque")]
    public async Task<ActionResult<PaginaDto<MovimentacaoDto>>> ListarMovimentacoes(
        [FromQuery] int? produtoId,
        [FromQuery] int? usuarioId,
        [FromQuery] string? tipo,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] int page = 0,
        [FromQuery] int size = 20)
        => Ok(await _svc.ListarMovimentacoesAsync(produtoId, usuarioId, tipo, dataInicio, dataFim, page, Math.Min(size, 100)));

    [HttpPost("movimentacoes-estoque")]
    public async Task<ActionResult<MovimentacaoDto>> Registrar([FromBody] MovimentacaoCreate req)
    {
        var m = await _svc.RegistrarAsync(req, CurrentUser.GetUserId(User));
        return Created($"/api/v1/movimentacoes-estoque/{m.Id}", m);
    }

    [HttpGet("movimentacoes-estoque/{movimentacaoId:int}")]
    public async Task<ActionResult<MovimentacaoDto>> Obter(int movimentacaoId)
        => Ok(await _svc.ObterMovimentacaoAsync(movimentacaoId));
}
