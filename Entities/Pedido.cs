namespace StudyToTech.Api.Entities;

public class Pedido
{
    public int IdPedido { get; set; }
    public int UsuarioIdUsuario { get; set; }
    public int StatusIdStatusPedido { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataFinalizacao { get; set; }
    /// <summary>
    /// Coluna gerada/virtual no banco para unicidade de pedido em elaboração por cliente.
    /// Preenchida apenas quando status = 1 (Em elaboração).
    /// </summary>
    public int? ClienteEmElaboracao { get; set; }

    public Usuario Usuario { get; set; } = null!;
    public StatusPedido Status { get; set; } = null!;
    public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
}
