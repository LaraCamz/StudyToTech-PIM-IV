namespace StudyToTech.Api.Entities;

public class StatusPedido
{
    public int IdStatusPedido { get; set; }
    public string Nome { get; set; } = string.Empty;

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
