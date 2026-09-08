using StudyToTech.Models;
using StudyToTech.Repositories;

namespace StudyToTech.Services;

public class EstoqueService
{
    private readonly EstoqueRepository estoqueRepository;

    public EstoqueService()
    {
        estoqueRepository = new EstoqueRepository();
    }

    public void ConsultarEstoque()
    {
        List<Produto> produtos = estoqueRepository.ConsultarEstoque();

        if (produtos.Count == 0)
        {
            Console.WriteLine("Nenhum produto encontrado.");
            return;
        }

        foreach (Produto produto in produtos)
        {
            Console.WriteLine($"ID: {produto.IdProduto}");
            Console.WriteLine($"Produto: {produto.Nome}");
            Console.WriteLine($"Estoque: {produto.Estoque}");
            Console.WriteLine($"Estoque mínimo: {produto.EstoqueMinimo}");

            if (produto.Estoque <= produto.EstoqueMinimo)
            {
                Console.WriteLine("ATENÇÃO: estoque baixo!");
            }

            Console.WriteLine("-----------------------------");
        }
    }

    public void RegistrarEntrada()
    {
        Console.Write("ID do produto: ");
        int produtoId = int.Parse(Console.ReadLine()!);

        Console.Write("ID do usuário: ");
        int usuarioId = int.Parse(Console.ReadLine()!);

        Console.Write("Quantidade: ");
        int quantidade = int.Parse(Console.ReadLine()!);

        estoqueRepository.RegistrarEntrada(
            produtoId,
            usuarioId,
            quantidade
        );

        Console.WriteLine("Entrada registrada com sucesso!");
    }

    public void RegistrarSaida()
    {
        Console.Write("ID do produto: ");
        int produtoId = int.Parse(Console.ReadLine()!);

        Console.Write("ID do usuário: ");
        int usuarioId = int.Parse(Console.ReadLine()!);

        Console.Write("Quantidade: ");
        int quantidade = int.Parse(Console.ReadLine()!);

        estoqueRepository.RegistrarSaida(
            produtoId,
            usuarioId,
            quantidade
        );

        Console.WriteLine("Saída registrada com sucesso!");
    }
}