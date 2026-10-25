<<<<<<< HEAD
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudyToTech.Api.Data;
using StudyToTech.Api.Helpers;
using StudyToTech.Api.Middleware;
using StudyToTech.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Database (MySQL) ---
var conn = builder.Configuration.GetConnectionString("Default")
    ?? "Server=localhost;Port=3306;Database=studytotech;User=root;Password=root;";

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(conn, ServerVersion.AutoDetect(conn)));

// --- JWT ---
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings
{
    Secret = "Study_to_Tech_Unip_III_2026_10"
};

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// --- Services ---
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<EstoqueService>();
builder.Services.AddScoped<PedidoService>();

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StudyToTech API — Módulo de Produtos, Estoque e Pedidos",
        Version = "1.0.0",
        Description = "Contrato alinhado ao OpenAPI do PIM IV"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Ex: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// CORS para frontend Web/Mobile
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("Frontend", p =>
        p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Migração automática + seed admin em desenvolvimento
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.EnsureCreated();

        // Admin padrão se não existir
        if (!db.Usuarios.Any(u => u.Email == "admin@studytotech.com"))
        {
            db.Usuarios.Add(new StudyToTech.Api.Entities.Usuario
            {
                Nome = "Administrador",
                Email = "admin@studytotech.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Status = true,
                DataCadastro = DateTime.UtcNow,
                PerfilIdPerfil = 3
            });
            db.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Não foi possível aplicar EnsureCreated (verifique a conexão MySQL).");
    }
}

app.Run();
=======
﻿using StudyToTech.Services;

ProdutoService produtoService = new ProdutoService();
UsuarioService usuarioService = new UsuarioService();
PedidoService pedidoService = new PedidoService();
ItemPedidoService itemPedidoService = new ItemPedidoService();
EstoqueService estoqueService = new EstoqueService();

int opcao;

do
{
    //Console.Clear();

    Console.WriteLine("===== SISTEMA DE ESTOQUE =====");
    Console.WriteLine("1 - Cadastrar produto");
    Console.WriteLine("2 - Listar produtos");
    Console.WriteLine("3 - Cadastrar usuário");
    Console.WriteLine("4 - Listar usuários");
    Console.WriteLine("5 - Criar pedido");
    Console.WriteLine("6 - Adicionar item ao pedido");
    Console.WriteLine("7 - Consultar estoque");
    Console.WriteLine("8 - Registrar entrada/saída de estoque");
    Console.WriteLine("9 - Finalizar pedido");
    Console.WriteLine("0 - Sair");
    Console.WriteLine("==============================");

    Console.Write("Escolha uma opção: ");
    opcao = int.Parse(Console.ReadLine()!);

    Console.Clear();

    switch (opcao)
    {
        case 1:
            produtoService.CadastrarProduto();
            break;

        case 2:
            produtoService.ListarProdutos();
            break;

        case 3:
            usuarioService.CadastrarUsuario();
            break;

        case 4:
            usuarioService.ListarUsuarios();
            break;

        case 5:
            pedidoService.CriarPedido();
            break;

        case 6:
            itemPedidoService.AdicionarItemAoPedido();
            break;

        case 7:
            estoqueService.ConsultarEstoque();
            break;

        case 8:
            Console.WriteLine("===== MOVIMENTAÇÃO DE ESTOQUE =====");
            Console.WriteLine("1 - Registrar entrada");
            Console.WriteLine("2 - Registrar saída");
            Console.Write("Escolha uma opção: ");

            int tipoMovimentacao = int.Parse(Console.ReadLine()!);

            Console.Clear();

            if (tipoMovimentacao == 1)
            {
                estoqueService.RegistrarEntrada();
            }
            else if (tipoMovimentacao == 2)
            {
                estoqueService.RegistrarSaida();
            }
            else
            {
                Console.WriteLine("Opção inválida.");
            }

            break;

        case 9:
            pedidoService.FinalizarPedido();
            break;

        case 0:
            Console.WriteLine("Encerrando o sistema...");
            break;

        default:
            Console.WriteLine("Opção inválida.");
            break;
    }

    if (opcao != 0)
    {
        Console.WriteLine();
        Console.WriteLine("Pressione ENTER para continuar...");
        Console.ReadLine();
    }

} while (opcao != 0);
>>>>>>> 3d83e06e01e5904b6dc92b64aff78a5fe34e53e2
