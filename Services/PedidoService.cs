using StudyToTech.Models;
using StudyToTech.Repositories;

namespace StudyToTech.Services;

public class PedidoService
{
    private readonly PedidoRepository pedidoRepository;

    public PedidoService()
    {
        pedidoRepository = new PedidoRepository();
    }

    public void CriarPedido()
    {
        Pedido pedido = new Pedido();

        Console.Write("ID do usuário: ");
        pedido.UsuarioIdUsuario = int.Parse(Console.ReadLine()!);

        pedido.DataCriacao = DateTime.Now;

        pedidoRepository.CriarPedido(pedido);

        Console.WriteLine("Pedido criado com sucesso!");
    }

    public void FinalizarPedido()
    {
        Console.Write("ID do pedido: ");
        int id = int.Parse(Console.ReadLine()!);

        Pedido? pedido = pedidoRepository.BuscarPedido(id);

        if (pedido == null)
        {
            Console.WriteLine("Pedido não encontrado.");
            return;
        }

        pedidoRepository.FinalizarPedido(id);

        Console.WriteLine("Pedido finalizado com sucesso!");
    }

    public Pedido? BuscarPedido(int id)
    {
        return pedidoRepository.BuscarPedido(id);
    }

    public List<Pedido> ObterPedidos()
    {
        List<Pedido> pedidos = new();

        // Caso precise listar pedidos posteriormente,
        // será necessário criar ListarPedidos() no PedidoRepository.

        return pedidos;
    }
}