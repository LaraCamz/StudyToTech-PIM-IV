using StudyToTech.Models;
using StudyToTech.Repositories;

namespace StudyToTech.Services;

public class ItemPedidoService
{
    private readonly ItemPedidoRepository itemPedidoRepository;
    private readonly ProdutoRepository produtoRepository;
    private readonly PedidoRepository pedidoRepository;

    public ItemPedidoService()
    {
        itemPedidoRepository = new ItemPedidoRepository();
        produtoRepository = new ProdutoRepository();
        pedidoRepository = new PedidoRepository();
    }

    public void AdicionarItemAoPedido()
    {
        Console.Write("ID do pedido: ");
        int pedidoId = int.Parse(Console.ReadLine()!);

        Pedido? pedido = pedidoRepository.BuscarPedido(pedidoId);

        if (pedido == null)
        {
            Console.WriteLine("Pedido não encontrado.");
            return;
        }

        Console.Write("ID do produto: ");
        int produtoId = int.Parse(Console.ReadLine()!);

        Produto? produto = produtoRepository.BuscarProduto(produtoId);

        if (produto == null)
        {
            Console.WriteLine("Produto não encontrado.");
            return;
        }

        Console.Write("Quantidade: ");
        int quantidade = int.Parse(Console.ReadLine()!);

        ItemPedido item = new ItemPedido();

        item.PedidoIdPedido = pedidoId;
        item.ProdutoIdProduto = produtoId;
        item.PrecoUnitario = produto.Preco;
        item.Quantidade = quantidade;

        itemPedidoRepository.AdicionarItemAoPedido(item);

        Console.WriteLine("Item adicionado ao pedido com sucesso!");
    }

    public void ListarItensDoPedido()
    {
        Console.Write("ID do pedido: ");
        int pedidoId = int.Parse(Console.ReadLine()!);

        List<ItemPedido> itens =
            itemPedidoRepository.ListarItensDoPedido(pedidoId);

        if (itens.Count == 0)
        {
            Console.WriteLine("Nenhum item encontrado.");
            return;
        }

        foreach (ItemPedido item in itens)
        {
            Console.WriteLine($"ID do item: {item.IdItemPedido}");
            Console.WriteLine($"Produto: {item.ProdutoIdProduto}");
            Console.WriteLine($"Preço unitário: R$ {item.PrecoUnitario:F2}");
            Console.WriteLine($"Quantidade: {item.Quantidade}");
            Console.WriteLine("-----------------------------");
        }
    }
}