using System.ComponentModel.DataAnnotations;

namespace StudyToTech.Api.DTOs;

public record StatusPedidoDto(int Id, string Nome);

public class ItemPedidoCreate
{
    [Required]
    public int ProdutoId { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int Quantidade { get; set; }
}

public class ItemPedidoUpdate
{
    [Required, Range(1, int.MaxValue)]
    public int Quantidade { get; set; }
}

public record ItemPedidoDto(
    int Id,
    int ProdutoId,
    string ProdutoNome,
    string PrecoUnitario,
    int Quantidade,
    string Subtotal
);

public record PedidoResumoDto(
    int Id,
    object Cliente,
    StatusPedidoDto Status,
    int TotalItens,
    string ValorTotal,
    DateTime DataCriacao,
    DateTime? DataFinalizacao
);

public record PedidoDto(
    int Id,
    object Cliente,
    StatusPedidoDto Status,
    int TotalItens,
    string ValorTotal,
    DateTime DataCriacao,
    DateTime? DataFinalizacao,
    IEnumerable<ItemPedidoDto> Itens
);

public class PedidoStatusRequest
{
    [Required, Range(2, 5)]
    public int StatusId { get; set; }
}

public record IndicadoresDto(
    int TotalProdutos,
    int ProdutosEstoqueBaixo,
    int TotalPedidos,
    int PedidosPendentes
);
