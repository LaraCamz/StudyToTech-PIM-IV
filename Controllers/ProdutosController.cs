using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyToTech.Api.DTOs;
using StudyToTech.Api.Helpers;
using StudyToTech.Api.Services;

namespace StudyToTech.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly ProdutoService _svc;

    public ProdutosController(ProdutoService svc) => _svc = svc;

    [HttpGet("categorias")]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> Categorias()
        => Ok(await _svc.ListarCategoriasAsync());

    [HttpGet("produtos")]
    public async Task<ActionResult<PaginaDto<ProdutoDto>>> Listar(
        [FromQuery] string? nome,
        [FromQuery] int? categoriaId,
        [FromQuery] bool? ativo,
        [FromQuery] bool? disponivel,
        [FromQuery] string ordenarPor = "nome",
        [FromQuery] string direcao = "asc",
        [FromQuery] int page = 0,
        [FromQuery] int size = 20)
    {
        var isCliente = CurrentUser.IsCliente(User);
        return Ok(await _svc.ListarAsync(nome, categoriaId, ativo, disponivel,
            ordenarPor, direcao, page, Math.Min(size, 100), isCliente));
    }

    [HttpGet("produtos/{produtoId:int}")]
    public async Task<ActionResult<ProdutoDto>> Obter(int produtoId)
        => Ok(await _svc.ObterAsync(produtoId, CurrentUser.IsCliente(User)));

    [HttpPost("produtos")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProdutoDto>> Criar([FromBody] ProdutoCreate req)
    {
        var p = await _svc.CriarAsync(req, CurrentUser.GetUserId(User));
        return Created($"/api/v1/produtos/{p.Id}", p);
    }

    [HttpPut("produtos/{produtoId:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProdutoDto>> Atualizar(int produtoId, [FromBody] ProdutoUpdate req)
        => Ok(await _svc.AtualizarAsync(produtoId, req));

    [HttpPatch("produtos/{produtoId:int}/status")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProdutoDto>> Status(int produtoId, [FromBody] AtivoRequest req)
        => Ok(await _svc.AlterarStatusAsync(produtoId, req.Ativo));
}
