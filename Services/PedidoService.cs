<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
using StudyToTech.Api.Data;
using StudyToTech.Api.DTOs;
using StudyToTech.Api.Entities;
using StudyToTech.Api.Helpers;
using System.Globalization;

namespace StudyToTech.Api.Services;

public class PedidoService
{
    private readonly AppDbContext _db;

    public PedidoService(AppDbContext db) => _db = db;

    public async Task<PaginaDto<PedidoResumoDto>> ListarAsync(
        int? statusId, int? clienteId, bool? apenasPendentes,
        DateTime? dataInicio, DateTime? dataFim,
        int page, int size, int userId, bool isCliente)
    {
        var q = _db.Pedidos
            .Include(p => p.Usuario)
            .Include(p => p.Status)
            .Include(p => p.Itens)
            .AsQueryable();

        if (isCliente)
            q = q.Where(p => p.UsuarioIdUsuario == userId);
        else if (clienteId.HasValue)
            q = q.Where(p => p.UsuarioIdUsuario == clienteId.Value);

        if (statusId.HasValue)
            q = q.Where(p => p.StatusIdStatusPedido == statusId.Value);
        if (apenasPendentes == true)
            q = q.Where(p => p.StatusIdStatusPedido != 4 && p.StatusIdStatusPedido != 5);
        if (dataInicio.HasValue)
            q = q.Where(p => p.DataCriacao >= dataInicio.Value);
        if (dataFim.HasValue)
            q = q.Where(p => p.DataCriacao <= dataFim.Value);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(p => p.DataCriacao)
            .Skip(page * size).Take(size).ToListAsync();

        return new PaginaDto<PedidoResumoDto>(
            page, size, total,
            (int)Math.Ceiling(total / (double)size),
            items.Select(MapResumo)
        );
    }

    public async Task<PedidoDto> ObterAsync(int id, int userId, bool isCliente)
    {
        var p = await LoadPedido(id);
        if (isCliente && p.UsuarioIdUsuario != userId)
            throw new ApiException("NAO_ENCONTRADO", "Pedido não encontrado.", 404);
        return MapPedido(p);
    }

    public async Task<PedidoDto> CriarEmElaboracaoAsync(int userId)
    {
        if (await _db.Pedidos.AnyAsync(p =>
            p.UsuarioIdUsuario == userId && p.StatusIdStatusPedido == 1))
        {
            var existente = await _db.Pedidos
                .Where(p => p.UsuarioIdUsuario == userId && p.StatusIdStatusPedido == 1)
                .Select(p => p.IdPedido)
                .FirstAsync();

            throw new ApiException("PEDIDO_EM_ELABORACAO_EXISTENTE",
                "Já existe um pedido em elaboração.", 409,
                new[] { new { pedidoId = existente } });
        }

        var pedido = new Pedido
        {
            UsuarioIdUsuario = userId,
            StatusIdStatusPedido = 1,
            DataCriacao = DateTime.UtcNow,
            ClienteEmElaboracao = userId
        };
        _db.Pedidos.Add(pedido);
        await _db.SaveChangesAsync();
        return MapPedido(await LoadPedido(pedido.IdPedido));
    }

    public async Task<PedidoDto> ObterEmElaboracaoAsync(int userId)
    {
        var p = await _db.Pedidos
            .Include(x => x.Usuario)
            .Include(x => x.Status)
            .Include(x => x.Itens).ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(x => x.UsuarioIdUsuario == userId && x.StatusIdStatusPedido == 1)
            ?? throw new ApiException("NAO_ENCONTRADO", "Nenhum pedido em elaboração.", 404);

        return MapPedido(p);
    }

    public async Task<PedidoDto> AdicionarItemAsync(int pedidoId, ItemPedidoCreate req, int userId)
    {
        var pedido = await LoadPedido(pedidoId);
        ValidarDonoEEditavel(pedido, userId);

        var produto = await _db.Produtos.FindAsync(req.ProdutoId)
            ?? throw new ApiException("NAO_ENCONTRADO", "Produto não encontrado.", 404);

        if (!produto.Status)
            throw new ApiException("PRODUTO_INATIVO", "Produto inativo não pode ser adicionado.", 409);

        if (pedido.Itens.Any(i => i.ProdutoIdProduto == req.ProdutoId))
            throw new ApiException("ITEM_DUPLICADO", "Produto já está no pedido. Altere a quantidade.", 409);

        var item = new ItemPedido
        {
            PedidoIdPedido = pedidoId,
            ProdutoIdProduto = req.ProdutoId,
            PrecoUnitario = produto.Preco,
            Quantidade = req.Quantidade
        };
        _db.ItensPedido.Add(item);
        await _db.SaveChangesAsync();
        return MapPedido(await LoadPedido(pedidoId));
    }

    public async Task<PedidoDto> AlterarItemAsync(int pedidoId, int itemId, ItemPedidoUpdate req, int userId)
    {
        var pedido = await LoadPedido(pedidoId);
        ValidarDonoEEditavel(pedido, userId);

        var item = pedido.Itens.FirstOrDefault(i => i.IdItemPedido == itemId)
            ?? throw new ApiException("NAO_ENCONTRADO", "Item não encontrado.", 404);

        item.Quantidade = req.Quantidade;
        await _db.SaveChangesAsync();
        return MapPedido(await LoadPedido(pedidoId));
    }

    public async Task<PedidoDto> RemoverItemAsync(int pedidoId, int itemId, int userId)
    {
        var pedido = await LoadPedido(pedidoId);
        ValidarDonoEEditavel(pedido, userId);

        var item = pedido.Itens.FirstOrDefault(i => i.IdItemPedido == itemId)
            ?? throw new ApiException("NAO_ENCONTRADO", "Item não encontrado.", 404);

        _db.ItensPedido.Remove(item);
        await _db.SaveChangesAsync();
        return MapPedido(await LoadPedido(pedidoId));
    }

    public async Task<PedidoDto> FinalizarAsync(int pedidoId, int userId)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        var pedido = await LoadPedido(pedidoId);
        ValidarDonoEEditavel(pedido, userId);

        if (!pedido.Itens.Any())
            throw new ApiException("PEDIDO_VAZIO", "Pedido sem itens.", 422);

        var insuficientes = new List<object>();

        foreach (var item in pedido.Itens)
        {
            // Bloqueio de linha por produto (RNF01)
            var produto = await _db.Produtos
                .FromSqlRaw("SELECT * FROM produto WHERE idproduto = {0} FOR UPDATE", item.ProdutoIdProduto)
                .FirstOrDefaultAsync()
                ?? throw new ApiException("NAO_ENCONTRADO", "Produto não encontrado.", 404);

            if (produto.Estoque < item.Quantidade)
            {
                insuficientes.Add(new
                {
                    produtoId = produto.IdProduto,
                    solicitado = item.Quantidade,
                    disponivel = produto.Estoque
                });
            }
        }

        if (insuficientes.Count > 0)
        {
            throw new ApiException("ESTOQUE_INSUFICIENTE",
                "Um ou mais itens não têm estoque suficiente.", 409, insuficientes);
        }

        foreach (var item in pedido.Itens)
        {
            var produto = await _db.Produtos.FindAsync(item.ProdutoIdProduto);
            produto!.Estoque -= item.Quantidade;

            _db.MovimentacoesEstoque.Add(new MovimentacaoEstoque
            {
                ProdutoIdProduto = item.ProdutoIdProduto,
                UsuarioIdUsuario = userId,
                Tipo = "SAIDA",
                Quantidade = item.Quantidade,
                DataHora = DateTime.UtcNow
            });
        }

        pedido.StatusIdStatusPedido = 2; // Finalizado
        pedido.DataFinalizacao = DateTime.UtcNow;
        pedido.ClienteEmElaboracao = null;

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        return MapPedido(await LoadPedido(pedidoId));
    }

    public async Task<PedidoDto> AlterarStatusAsync(int pedidoId, int novoStatusId, int userId, string perfil)
    {
        var pedido = await LoadPedido(pedidoId);
        var atual = pedido.StatusIdStatusPedido;

        // Valida transições conforme contrato
        var permitido = (atual, novoStatusId, perfil) switch
        {
            (1, 5, _) => true, // Em elaboração → Cancelado (qualquer dono/func/admin)
            (2, 3, "Funcionário" or "Administrador") => true, // Finalizado → Em separação
            (3, 4, "Funcionário" or "Administrador") => true, // Em separação → Concluído
            (2, 5, "Funcionário" or "Administrador") => true, // Finalizado → Cancelado
            (3, 5, "Funcionário" or "Administrador") => true, // Em separação → Cancelado
            _ => false
        };

        // Cliente só cancela o próprio em elaboração
        if (perfil == "Cliente")
        {
            if (pedido.UsuarioIdUsuario != userId || atual != 1 || novoStatusId != 5)
                throw new ApiException("ACESSO_NEGADO", "Operação não permitida.", 403);
        }

        if (!permitido)
            throw new ApiException("TRANSICAO_STATUS_INVALIDA", "Transição de status inválida.", 409);

        await using var tx = await _db.Database.BeginTransactionAsync();

        // Ao cancelar pedido já finalizado/em separação, devolve estoque
        if (novoStatusId == 5 && (atual == 2 || atual == 3))
        {
            foreach (var item in pedido.Itens)
            {
                var produto = await _db.Produtos
                    .FromSqlRaw("SELECT * FROM produto WHERE idproduto = {0} FOR UPDATE", item.ProdutoIdProduto)
                    .FirstAsync();

                produto.Estoque += item.Quantidade;

                _db.MovimentacoesEstoque.Add(new MovimentacaoEstoque
                {
                    ProdutoIdProduto = item.ProdutoIdProduto,
                    UsuarioIdUsuario = userId,
                    Tipo = "ENTRADA",
                    Quantidade = item.Quantidade,
                    DataHora = DateTime.UtcNow
                });
            }
        }

        pedido.StatusIdStatusPedido = novoStatusId;
        if (novoStatusId == 5 || novoStatusId == 4)
            pedido.ClienteEmElaboracao = null;

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        return MapPedido(await LoadPedido(pedidoId));
    }

    public async Task<IEnumerable<StatusPedidoDto>> ListarStatusAsync()
    {
        return await _db.StatusPedidos
            .OrderBy(s => s.IdStatusPedido)
            .Select(s => new StatusPedidoDto(s.IdStatusPedido, s.Nome))
            .ToListAsync();
    }

    public async Task<IndicadoresDto> IndicadoresAsync()
    {
        var totalProdutos = await _db.Produtos.CountAsync();
        var estoqueBaixo = await _db.Produtos.CountAsync(p => p.Status && p.Estoque < p.EstoqueMinimo);
        var totalPedidos = await _db.Pedidos.CountAsync();
        var pendentes = await _db.Pedidos.CountAsync(p =>
            p.StatusIdStatusPedido != 4 && p.StatusIdStatusPedido != 5);

        return new IndicadoresDto(totalProdutos, estoqueBaixo, totalPedidos, pendentes);
    }

    // --- helpers ---

    private async Task<Pedido> LoadPedido(int id)
    {
        return await _db.Pedidos
            .Include(p => p.Usuario)
            .Include(p => p.Status)
            .Include(p => p.Itens).ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.IdPedido == id)
            ?? throw new ApiException("NAO_ENCONTRADO", "Pedido não encontrado.", 404);
    }

    private static void ValidarDonoEEditavel(Pedido pedido, int userId)
    {
        if (pedido.UsuarioIdUsuario != userId)
            throw new ApiException("ACESSO_NEGADO", "Pedido não pertence ao usuário.", 403);
        if (pedido.StatusIdStatusPedido != 1)
            throw new ApiException("PEDIDO_NAO_EDITAVEL", "Pedido não está em elaboração.", 409);
    }

    private static PedidoResumoDto MapResumo(Pedido p)
    {
        var totalItens = p.Itens.Sum(i => i.Quantidade);
        var valor = p.Itens.Sum(i => i.PrecoUnitario * i.Quantidade);
        return new PedidoResumoDto(
            p.IdPedido,
            new { id = p.Usuario.IdUsuario, nome = p.Usuario.Nome },
            new StatusPedidoDto(p.Status.IdStatusPedido, p.Status.Nome),
            totalItens,
            valor.ToString("F2", CultureInfo.InvariantCulture),
            p.DataCriacao,
            p.DataFinalizacao
        );
    }

    private static PedidoDto MapPedido(Pedido p)
    {
        var itens = p.Itens.Select(i => new ItemPedidoDto(
            i.IdItemPedido,
            i.ProdutoIdProduto,
            i.Produto.Nome,
            i.PrecoUnitario.ToString("F2", CultureInfo.InvariantCulture),
            i.Quantidade,
            (i.PrecoUnitario * i.Quantidade).ToString("F2", CultureInfo.InvariantCulture)
        ));

        var totalItens = p.Itens.Sum(i => i.Quantidade);
        var valor = p.Itens.Sum(i => i.PrecoUnitario * i.Quantidade);

        return new PedidoDto(
            p.IdPedido,
            new { id = p.Usuario.IdUsuario, nome = p.Usuario.Nome },
            new StatusPedidoDto(p.Status.IdStatusPedido, p.Status.Nome),
            totalItens,
            valor.ToString("F2", CultureInfo.InvariantCulture),
            p.DataCriacao,
            p.DataFinalizacao,
            itens
        );
    }
}
=======
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
>>>>>>> 3d83e06e01e5904b6dc92b64aff78a5fe34e53e2
