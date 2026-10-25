using Microsoft.EntityFrameworkCore;
using StudyToTech.Api.Entities;

namespace StudyToTech.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Perfil> Perfis => Set<Perfil>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<StatusPedido> StatusPedidos => Set<StatusPedido>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- perfil ---
        modelBuilder.Entity<Perfil>(e =>
        {
            e.ToTable("perfil");
            e.HasKey(x => x.IdPerfil);
            e.Property(x => x.IdPerfil).HasColumnName("idperfil");
            e.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(45).IsRequired();
        });

        // --- usuario ---
        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("usuario");
            e.HasKey(x => x.IdUsuario);
            e.Property(x => x.IdUsuario).HasColumnName("idusuario");
            e.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
            e.Property(x => x.SenhaHash).HasColumnName("senha_hash").HasMaxLength(255).IsRequired();
            e.Property(x => x.Status).HasColumnName("status").HasColumnType("tinyint(1)");
            e.Property(x => x.DataCadastro).HasColumnName("data_cadastro");
            e.Property(x => x.PerfilIdPerfil).HasColumnName("perfil_idperfil");

            e.HasIndex(x => x.Email).IsUnique();

            e.HasOne(x => x.Perfil)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(x => x.PerfilIdPerfil)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- categoria ---
        modelBuilder.Entity<Categoria>(e =>
        {
            e.ToTable("categoria");
            e.HasKey(x => x.IdCategoria);
            e.Property(x => x.IdCategoria).HasColumnName("idcategoria");
            e.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(45).IsRequired();
        });

        // --- produto ---
        modelBuilder.Entity<Produto>(e =>
        {
            e.ToTable("produto");
            e.HasKey(x => x.IdProduto);
            e.Property(x => x.IdProduto).HasColumnName("idproduto");
            e.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(100).IsRequired();
            e.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(250);
            e.Property(x => x.CategoriaIdCategoria).HasColumnName("categoria_idcategoria");
            e.Property(x => x.Preco).HasColumnName("preco").HasColumnType("decimal(10,2)");
            e.Property(x => x.Estoque).HasColumnName("estoque");
            e.Property(x => x.EstoqueMinimo).HasColumnName("estoque_minimo");
            e.Property(x => x.Status).HasColumnName("status").HasColumnType("tinyint(1)");
            e.Property(x => x.DataCadastro).HasColumnName("data_cadastro");

            e.HasOne(x => x.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(x => x.CategoriaIdCategoria)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- status_pedido ---
        modelBuilder.Entity<StatusPedido>(e =>
        {
            e.ToTable("status_pedido");
            e.HasKey(x => x.IdStatusPedido);
            e.Property(x => x.IdStatusPedido).HasColumnName("idstatus_pedido");
            e.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(45).IsRequired();
        });

        // --- pedido ---
        modelBuilder.Entity<Pedido>(e =>
        {
            e.ToTable("pedido");
            e.HasKey(x => x.IdPedido);
            e.Property(x => x.IdPedido).HasColumnName("idpedido");
            e.Property(x => x.UsuarioIdUsuario).HasColumnName("usuario_idusuario");
            e.Property(x => x.StatusIdStatusPedido).HasColumnName("status_idstatus_pedido");
            e.Property(x => x.DataCriacao).HasColumnName("data_criacao");
            e.Property(x => x.DataFinalizacao).HasColumnName("data_finalizacao");
            e.Property(x => x.ClienteEmElaboracao).HasColumnName("cliente_em_elaboracao");

            e.HasIndex(x => x.ClienteEmElaboracao)
                .IsUnique()
                .HasDatabaseName("uk_pedido_cliente_em_elaboracao");

            e.HasOne(x => x.Usuario)
                .WithMany(u => u.Pedidos)
                .HasForeignKey(x => x.UsuarioIdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Status)
                .WithMany(s => s.Pedidos)
                .HasForeignKey(x => x.StatusIdStatusPedido)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- item_pedido ---
        modelBuilder.Entity<ItemPedido>(e =>
        {
            e.ToTable("item_pedido");
            e.HasKey(x => x.IdItemPedido);
            e.Property(x => x.IdItemPedido).HasColumnName("iditem_pedido");
            e.Property(x => x.PedidoIdPedido).HasColumnName("pedido_idpedido");
            e.Property(x => x.ProdutoIdProduto).HasColumnName("produto_idproduto");
            e.Property(x => x.PrecoUnitario).HasColumnName("preco_unitario").HasColumnType("decimal(10,2)");
            e.Property(x => x.Quantidade).HasColumnName("quantidade");

            e.HasIndex(x => new { x.PedidoIdPedido, x.ProdutoIdProduto })
                .IsUnique()
                .HasDatabaseName("uk_item_pedido_pedido_produto");

            e.HasOne(x => x.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(x => x.PedidoIdPedido)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Produto)
                .WithMany(p => p.ItensPedido)
                .HasForeignKey(x => x.ProdutoIdProduto)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- movimentacao_estoque ---
        modelBuilder.Entity<MovimentacaoEstoque>(e =>
        {
            e.ToTable("movimentacao_estoque");
            e.HasKey(x => x.IdMovimentacaoEstoque);
            e.Property(x => x.IdMovimentacaoEstoque).HasColumnName("idmovimentacao_estoque");
            e.Property(x => x.ProdutoIdProduto).HasColumnName("produto_idproduto");
            e.Property(x => x.UsuarioIdUsuario).HasColumnName("usuario_idusuario");
            e.Property(x => x.Tipo).HasColumnName("tipo").HasMaxLength(10).IsRequired();
            e.Property(x => x.Quantidade).HasColumnName("quantidade");
            e.Property(x => x.DataHora).HasColumnName("data_hora");

            e.HasOne(x => x.Produto)
                .WithMany(p => p.Movimentacoes)
                .HasForeignKey(x => x.ProdutoIdProduto)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Usuario)
                .WithMany(u => u.Movimentacoes)
                .HasForeignKey(x => x.UsuarioIdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed básico de perfis, categorias e status
        modelBuilder.Entity<Perfil>().HasData(
            new Perfil { IdPerfil = 1, Nome = "Cliente" },
            new Perfil { IdPerfil = 2, Nome = "Funcionário" },
            new Perfil { IdPerfil = 3, Nome = "Administrador" }
        );

        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { IdCategoria = 1, Nome = "Computadores" },
            new Categoria { IdCategoria = 2, Nome = "Notebooks" },
            new Categoria { IdCategoria = 3, Nome = "Tablets" },
            new Categoria { IdCategoria = 4, Nome = "Acessórios" }
        );

        modelBuilder.Entity<StatusPedido>().HasData(
            new StatusPedido { IdStatusPedido = 1, Nome = "Em elaboração" },
            new StatusPedido { IdStatusPedido = 2, Nome = "Finalizado" },
            new StatusPedido { IdStatusPedido = 3, Nome = "Em separação" },
            new StatusPedido { IdStatusPedido = 4, Nome = "Concluído" },
            new StatusPedido { IdStatusPedido = 5, Nome = "Cancelado" }
        );
    }
}
