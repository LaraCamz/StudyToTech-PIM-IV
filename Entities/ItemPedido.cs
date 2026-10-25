namespace StudyToTech.Api.Entities;

public class ItemPedido
{
    public int IdItemPedido { get; set; }
    public int PedidoIdPedido { get; set; }
    public int ProdutoIdProduto { get; set; }
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }

    public Pedido Pedido { get; set; } = null!;
    public Produto Produto { get; set; } = null!;
}
