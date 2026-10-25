using Microsoft.EntityFrameworkCore;
using StudyToTech.Api.Data;
using StudyToTech.Api.DTOs;
using StudyToTech.Api.Entities;
using StudyToTech.Api.Helpers;

namespace StudyToTech.Api.Services;

public class EstoqueService
{
    private readonly AppDbContext _db;

    public EstoqueService(AppDbContext db) => _db = db;

    public async Task<PaginaDto<EstoqueItemDto>> ConsultarAsync(
        string? nome, int? categoriaId, bool? abaixoDoMinimo, int page, int size)
    {
        var q = _db.Produtos.Include(p => p.Categoria).AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
            q = q.Where(p => p.Nome.Contains(nome));
        if (categoriaId.HasValue)
            q = q.Where(p => p.CategoriaIdCategoria == categoriaId.Value);
        if (abaixoDoMinimo == true)
            q = q.Where(p => p.Estoque < p.EstoqueMinimo && p.Status);

        var total = await q.CountAsync();
        var items = await q.OrderBy(p => p.Nome)
            .Skip(page * size).Take(size).ToListAsync();

        return new PaginaDto<EstoqueItemDto>(
            page, size, total,
            (int)Math.Ceiling(total / (double)size),
            items.Select(MapEstoque)
        );
    }

    public async Task<IEnumerable<EstoqueItemDto>> AlertasAsync()
    {
        var items = await _db.Produtos
            .Include(p => p.Categoria)
            .Where(p => p.Status && p.Estoque < p.EstoqueMinimo)
            .OrderBy(p => p.Nome)
            .ToListAsync();

        return items.Select(MapEstoque);
    }

    public async Task<PaginaDto<MovimentacaoDto>> ListarMovimentacoesAsync(
        int? produtoId, int? usuarioId, string? tipo,
        DateTime? dataInicio, DateTime? dataFim, int page, int size)
    {
        var q = _db.MovimentacoesEstoque
            .Include(m => m.Produto)
            .Include(m => m.Usuario)
            .AsQueryable();

        if (produtoId.HasValue) q = q.Where(m => m.ProdutoIdProduto == produtoId.Value);
        if (usuarioId.HasValue) q = q.Where(m => m.UsuarioIdUsuario == usuarioId.Value);
        if (!string.IsNullOrWhiteSpace(tipo)) q = q.Where(m => m.Tipo == tipo.ToUpper());
        if (dataInicio.HasValue) q = q.Where(m => m.DataHora >= dataInicio.Value);
        if (dataFim.HasValue) q = q.Where(m => m.DataHora <= dataFim.Value);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(m => m.DataHora)
            .Skip(page * size).Take(size).ToListAsync();

        return new PaginaDto<MovimentacaoDto>(
            page, size, total,
            (int)Math.Ceiling(total / (double)size),
            items.Select(m => MapMov(m, null))
        );
    }

    public async Task<MovimentacaoDto> ObterMovimentacaoAsync(int id)
    {
        var m = await _db.MovimentacoesEstoque
            .Include(x => x.Produto)
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.IdMovimentacaoEstoque == id)
            ?? throw new ApiException("NAO_ENCONTRADO", "Movimentação não encontrada.", 404);

        return MapMov(m, null);
    }

    public async Task<MovimentacaoDto> RegistrarAsync(MovimentacaoCreate req, int usuarioId)
    {
        var tipo = req.Tipo.ToUpperInvariant();
        if (tipo is not ("ENTRADA" or "SAIDA"))
            throw new ApiException("VALIDACAO", "Tipo deve ser ENTRADA ou SAIDA.", 400);

        await using var tx = await _db.Database.BeginTransactionAsync();

        // Bloqueio de linha (RNF01 / RN01)
        var produto = await _db.Produtos
            .FromSqlRaw("SELECT * FROM produto WHERE idproduto = {0} FOR UPDATE", req.ProdutoId)
            .FirstOrDefaultAsync()
            ?? throw new ApiException("NAO_ENCONTRADO", "Produto não encontrado.", 404);

        if (tipo == "SAIDA" && produto.Estoque < req.Quantidade)
        {
            throw new ApiException("ESTOQUE_INSUFICIENTE",
                "Estoque insuficiente para a saída solicitada.", 409,
                new[] { new { produtoId = produto.IdProduto, solicitado = req.Quantidade, disponivel = produto.Estoque } });
        }

        produto.Estoque += tipo == "ENTRADA" ? req.Quantidade : -req.Quantidade;

        var mov = new MovimentacaoEstoque
        {
            ProdutoIdProduto = produto.IdProduto,
            UsuarioIdUsuario = usuarioId,
            Tipo = tipo,
            Quantidade = req.Quantidade,
            DataHora = DateTime.UtcNow
        };
        _db.MovimentacoesEstoque.Add(mov);
        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        await _db.Entry(mov).Reference(m => m.Produto).LoadAsync();
        await _db.Entry(mov).Reference(m => m.Usuario).LoadAsync();

        return MapMov(mov, produto.Estoque);
    }

    private static EstoqueItemDto MapEstoque(Produto p) => new(
        p.IdProduto, p.Nome, p.Categoria.Nome,
        p.Estoque, p.EstoqueMinimo,
        p.Estoque < p.EstoqueMinimo, p.Status
    );

    private static MovimentacaoDto MapMov(MovimentacaoEstoque m, int? saldo) => new(
        m.IdMovimentacaoEstoque,
        new { id = m.Produto.IdProduto, nome = m.Produto.Nome },
        m.Tipo,
        m.Quantidade,
        m.DataHora,
        new { id = m.Usuario.IdUsuario, nome = m.Usuario.Nome },
        saldo
    );
}
