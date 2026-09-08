using StudyToTech.Models;
using StudyToTech.Repositories;

namespace StudyToTech.Services;

public class ProdutoService
{
    private readonly ProdutoRepository produtoRepository;

    public ProdutoService()
    {
        produtoRepository = new ProdutoRepository();
    }

    public void CadastrarProduto()
    {
        Produto produto = new Produto();

        Console.Write("Nome: ");
        produto.Nome = Console.ReadLine()!;

        Console.Write("Descrição: ");
        produto.Descricao = Console.ReadLine()!;

        Console.Write("ID da categoria: ");
        produto.CategoriaIdCategoria = int.Parse(Console.ReadLine()!);

        Console.Write("Preço: ");
        produto.Preco = decimal.Parse(Console.ReadLine()!);

        Console.Write("Estoque: ");
        produto.Estoque = int.Parse(Console.ReadLine()!);

        Console.Write("Estoque mínimo: ");
        produto.EstoqueMinimo = int.Parse(Console.ReadLine()!);

        produto.Status = true;
        produto.DataCadastro = DateTime.Now;

        produtoRepository.CadastrarProduto(produto);

        Console.WriteLine("Produto cadastrado com sucesso!");
    }

    public void ListarProdutos()
    {
        List<Produto> produtos = produtoRepository.ListarProdutos();

        if (produtos.Count == 0)
        {
            Console.WriteLine("Nenhum produto cadastrado.");
            return;
        }

        foreach (Produto produto in produtos)
        {
            Console.WriteLine($"ID: {produto.IdProduto}");
            Console.WriteLine($"Nome: {produto.Nome}");
            Console.WriteLine($"Descrição: {produto.Descricao}");
            Console.WriteLine($"Categoria: {produto.CategoriaIdCategoria}");
            Console.WriteLine($"Preço: R$ {produto.Preco:F2}");
            Console.WriteLine($"Estoque: {produto.Estoque}");
            Console.WriteLine($"Estoque mínimo: {produto.EstoqueMinimo}");
            Console.WriteLine($"Status: {(produto.Status ? "Ativo" : "Inativo")}");
            Console.WriteLine("-----------------------------");
        }
    }

    public Produto? BuscarProduto(int id)
    {
        return produtoRepository.BuscarProduto(id);
    }

    public List<Produto> ObterProdutos()
    {
        return produtoRepository.ListarProdutos();
    }
}