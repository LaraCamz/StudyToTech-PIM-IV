namespace StudyToTech.Models;

public class ItemPedido
{
    public int IdItemPedido { get; set; }
    public int PedidoIdPedido { get; set; }
    public int ProdutoIdProduto { get; set; }
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }
}