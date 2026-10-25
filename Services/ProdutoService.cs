<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
using StudyToTech.Api.Data;
using StudyToTech.Api.DTOs;
using StudyToTech.Api.Entities;
using StudyToTech.Api.Helpers;
using System.Globalization;

namespace StudyToTech.Api.Services;

public class ProdutoService
{
    private readonly AppDbContext _db;

    public ProdutoService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<CategoriaDto>> ListarCategoriasAsync()
    {
        return await _db.Categorias
            .OrderBy(c => c.IdCategoria)
            .Select(c => new CategoriaDto(c.IdCategoria, c.Nome))
            .ToListAsync();
    }

    public async Task<PaginaDto<ProdutoDto>> ListarAsync(
        string? nome, int? categoriaId, bool? ativo, bool? disponivel,
        string ordenarPor, string direcao, int page, int size,
        bool isCliente)
    {
        var q = _db.Produtos.Include(p => p.Categoria).AsQueryable();

        if (isCliente)
            q = q.Where(p => p.Status); // só ativos
        else if (ativo.HasValue)
            q = q.Where(p => p.Status == ativo.Value);

        if (!string.IsNullOrWhiteSpace(nome))
            q = q.Where(p => p.Nome.Contains(nome));
        if (categoriaId.HasValue)
            q = q.Where(p => p.CategoriaIdCategoria == categoriaId.Value);
        if (disponivel == true)
            q = q.Where(p => p.Estoque > 0);

        q = (ordenarPor.ToLower(), direcao.ToLower()) switch
        {
            ("preco", "desc") => q.OrderByDescending(p => p.Preco),
            ("preco", _) => q.OrderBy(p => p.Preco),
            ("datacadastro", "desc") => q.OrderByDescending(p => p.DataCadastro),
            ("datacadastro", _) => q.OrderBy(p => p.DataCadastro),
            (_, "desc") => q.OrderByDescending(p => p.Nome),
            _ => q.OrderBy(p => p.Nome)
        };

        var total = await q.CountAsync();
        var items = await q.Skip(page * size).Take(size).ToListAsync();

        return new PaginaDto<ProdutoDto>(
            page, size, total,
            (int)Math.Ceiling(total / (double)size),
            items.Select(p => Map(p, isCliente))
        );
    }

    public async Task<ProdutoDto> ObterAsync(int id, bool isCliente)
    {
        var p = await _db.Produtos.Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.IdProduto == id)
            ?? throw new ApiException("NAO_ENCONTRADO", "Produto não encontrado.", 404);

        if (isCliente && !p.Status)
            throw new ApiException("NAO_ENCONTRADO", "Produto não encontrado.", 404);

        return Map(p, isCliente);
    }

    public async Task<ProdutoDto> CriarAsync(ProdutoCreate req, int adminId)
    {
        if (!decimal.TryParse(req.Preco, NumberStyles.Number, CultureInfo.InvariantCulture, out var preco)
            || preco < 0)
            throw new ApiException("VALIDACAO", "Preço inválido.", 400,
                new[] { new { campo = "preco", motivo = "Formato DECIMAL(10,2) esperado" } });

        if (!await _db.Categorias.AnyAsync(c => c.IdCategoria == req.CategoriaId))
            throw new ApiException("VALIDACAO", "Categoria inválida.", 400);

        await using var tx = await _db.Database.BeginTransactionAsync();

        var produto = new Produto
        {
            Nome = req.Nome,
            Descricao = req.Descricao,
            CategoriaIdCategoria = req.CategoriaId,
            Preco = preco,
            Estoque = 0,
            EstoqueMinimo = req.EstoqueMinimo,
            Status = req.Ativo,
            DataCadastro = DateTime.UtcNow
        };
        _db.Produtos.Add(produto);
        await _db.SaveChangesAsync();

        if (req.EstoqueInicial > 0)
        {
            _db.MovimentacoesEstoque.Add(new MovimentacaoEstoque
            {
                ProdutoIdProduto = produto.IdProduto,
                UsuarioIdUsuario = adminId,
                Tipo = "ENTRADA",
                Quantidade = req.EstoqueInicial,
                DataHora = DateTime.UtcNow
            });
            produto.Estoque = req.EstoqueInicial;
            await _db.SaveChangesAsync();
        }

        await tx.CommitAsync();
        await _db.Entry(produto).Reference(p => p.Categoria).LoadAsync();
        return Map(produto, false);
    }

    public async Task<ProdutoDto> AtualizarAsync(int id, ProdutoUpdate req)
    {
        var p = await _db.Produtos.Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.IdProduto == id)
            ?? throw new ApiException("NAO_ENCONTRADO", "Produto não encontrado.", 404);

        if (!decimal.TryParse(req.Preco, NumberStyles.Number, CultureInfo.InvariantCulture, out var preco)
            || preco < 0)
            throw new ApiException("VALIDACAO", "Preço inválido.", 400);

        if (!await _db.Categorias.AnyAsync(c => c.IdCategoria == req.CategoriaId))
            throw new ApiException("VALIDACAO", "Categoria inválida.", 400);

        p.Nome = req.Nome;
        p.Descricao = req.Descricao;
        p.CategoriaIdCategoria = req.CategoriaId;
        p.Preco = preco;
        p.EstoqueMinimo = req.EstoqueMinimo;

        await _db.SaveChangesAsync();
        await _db.Entry(p).Reference(x => x.Categoria).LoadAsync();
        return Map(p, false);
    }

    public async Task<ProdutoDto> AlterarStatusAsync(int id, bool ativo)
    {
        var p = await _db.Produtos.Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.IdProduto == id)
            ?? throw new ApiException("NAO_ENCONTRADO", "Produto não encontrado.", 404);

        p.Status = ativo;
        await _db.SaveChangesAsync();
        return Map(p, false);
    }

    private static ProdutoDto Map(Produto p, bool isCliente) => new(
        p.IdProduto,
        p.Nome,
        p.Descricao,
        new CategoriaDto(p.Categoria.IdCategoria, p.Categoria.Nome),
        p.Preco.ToString("F2", CultureInfo.InvariantCulture),
        p.Estoque,
        isCliente ? null : p.EstoqueMinimo,
        p.Estoque > 0,
        p.Status,
        p.DataCadastro
    );
}
=======
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
>>>>>>> 3d83e06e01e5904b6dc92b64aff78a5fe34e53e2
