using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyToTech.Api.DTOs;
using StudyToTech.Api.Helpers;
using StudyToTech.Api.Services;

namespace StudyToTech.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly PedidoService _svc;

    public PedidosController(PedidoService svc) => _svc = svc;

    [HttpGet("pedidos")]
    public async Task<ActionResult<PaginaDto<PedidoResumoDto>>> Listar(
        [FromQuery] int? statusId,
        [FromQuery] int? clienteId,
        [FromQuery] bool? apenasPendentes,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] int page = 0,
        [FromQuery] int size = 20)
    {
        var isCliente = CurrentUser.IsCliente(User);
        return Ok(await _svc.ListarAsync(statusId, clienteId, apenasPendentes,
            dataInicio, dataFim, page, Math.Min(size, 100),
            CurrentUser.GetUserId(User), isCliente));
    }

    [HttpPost("pedidos")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<PedidoDto>> Criar()
    {
        var p = await _svc.CriarEmElaboracaoAsync(CurrentUser.GetUserId(User));
        return Created($"/api/v1/pedidos/{p.Id}", p);
    }

    [HttpGet("pedidos/em-elaboracao")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<PedidoDto>> EmElaboracao()
        => Ok(await _svc.ObterEmElaboracaoAsync(CurrentUser.GetUserId(User)));

    [HttpGet("pedidos/{pedidoId:int}")]
    public async Task<ActionResult<PedidoDto>> Obter(int pedidoId)
        => Ok(await _svc.ObterAsync(pedidoId, CurrentUser.GetUserId(User), CurrentUser.IsCliente(User)));

    [HttpPost("pedidos/{pedidoId:int}/itens")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<PedidoDto>> AdicionarItem(int pedidoId, [FromBody] ItemPedidoCreate req)
        => StatusCode(201, await _svc.AdicionarItemAsync(pedidoId, req, CurrentUser.GetUserId(User)));

    [HttpPatch("pedidos/{pedidoId:int}/itens/{itemId:int}")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<PedidoDto>> AlterarItem(int pedidoId, int itemId, [FromBody] ItemPedidoUpdate req)
        => Ok(await _svc.AlterarItemAsync(pedidoId, itemId, req, CurrentUser.GetUserId(User)));

    [HttpDelete("pedidos/{pedidoId:int}/itens/{itemId:int}")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<PedidoDto>> RemoverItem(int pedidoId, int itemId)
        => Ok(await _svc.RemoverItemAsync(pedidoId, itemId, CurrentUser.GetUserId(User)));

    [HttpPost("pedidos/{pedidoId:int}/finalizar")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<PedidoDto>> Finalizar(int pedidoId)
        => Ok(await _svc.FinalizarAsync(pedidoId, CurrentUser.GetUserId(User)));

    [HttpPatch("pedidos/{pedidoId:int}/status")]
    public async Task<ActionResult<PedidoDto>> AlterarStatus(int pedidoId, [FromBody] PedidoStatusRequest req)
        => Ok(await _svc.AlterarStatusAsync(pedidoId, req.StatusId,
            CurrentUser.GetUserId(User), CurrentUser.GetPerfil(User)));

    [HttpGet("status-pedido")]
    public async Task<ActionResult<IEnumerable<StatusPedidoDto>>> Status()
        => Ok(await _svc.ListarStatusAsync());

    [HttpGet("indicadores")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<IndicadoresDto>> Indicadores()
        => Ok(await _svc.IndicadoresAsync());
}
