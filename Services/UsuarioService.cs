<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
using StudyToTech.Api.Data;
using StudyToTech.Api.DTOs;
using StudyToTech.Api.Entities;
using StudyToTech.Api.Helpers;

namespace StudyToTech.Api.Services;

public class UsuarioService
{
    private readonly AppDbContext _db;

    public UsuarioService(AppDbContext db) => _db = db;

    public async Task<PaginaDto<UsuarioDto>> ListarAsync(
        string? nome, int? perfilId, bool? ativo, int page, int size)
    {
        var q = _db.Usuarios.Include(u => u.Perfil).AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
            q = q.Where(u => u.Nome.Contains(nome) || u.Email.Contains(nome));
        if (perfilId.HasValue)
            q = q.Where(u => u.PerfilIdPerfil == perfilId.Value);
        if (ativo.HasValue)
            q = q.Where(u => u.Status == ativo.Value);

        var total = await q.CountAsync();
        var items = await q
            .OrderBy(u => u.Nome)
            .Skip(page * size)
            .Take(size)
            .ToListAsync();

        return new PaginaDto<UsuarioDto>(
            page, size, total,
            (int)Math.Ceiling(total / (double)size),
            items.Select(AuthService.MapUsuario)
        );
    }

    public async Task<UsuarioDto> ObterAsync(int id)
    {
        var u = await _db.Usuarios.Include(x => x.Perfil)
            .FirstOrDefaultAsync(x => x.IdUsuario == id)
            ?? throw new ApiException("NAO_ENCONTRADO", "Usuário não encontrado.", 404);
        return AuthService.MapUsuario(u);
    }

    public async Task<UsuarioDto> CriarAsync(UsuarioCreate req)
    {
        if (await _db.Usuarios.AnyAsync(u => u.Email == req.Email))
            throw new ApiException("EMAIL_JA_CADASTRADO", "E-mail já cadastrado.", 409);

        if (!await _db.Perfis.AnyAsync(p => p.IdPerfil == req.PerfilId))
            throw new ApiException("VALIDACAO", "Perfil inválido.", 400);

        var u = new Usuario
        {
            Nome = req.Nome,
            Email = req.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(req.Senha),
            Status = req.Ativo,
            DataCadastro = DateTime.UtcNow,
            PerfilIdPerfil = req.PerfilId
        };
        _db.Usuarios.Add(u);
        await _db.SaveChangesAsync();
        await _db.Entry(u).Reference(x => x.Perfil).LoadAsync();
        return AuthService.MapUsuario(u);
    }

    public async Task<UsuarioDto> AtualizarAsync(int id, UsuarioUpdate req)
    {
        var u = await _db.Usuarios.Include(x => x.Perfil)
            .FirstOrDefaultAsync(x => x.IdUsuario == id)
            ?? throw new ApiException("NAO_ENCONTRADO", "Usuário não encontrado.", 404);

        if (await _db.Usuarios.AnyAsync(x => x.Email == req.Email && x.IdUsuario != id))
            throw new ApiException("EMAIL_JA_CADASTRADO", "E-mail já cadastrado.", 409);

        if (!await _db.Perfis.AnyAsync(p => p.IdPerfil == req.PerfilId))
            throw new ApiException("VALIDACAO", "Perfil inválido.", 400);

        u.Nome = req.Nome;
        u.Email = req.Email;
        u.PerfilIdPerfil = req.PerfilId;
        if (!string.IsNullOrWhiteSpace(req.Senha))
            u.SenhaHash = BCrypt.Net.BCrypt.HashPassword(req.Senha);

        await _db.SaveChangesAsync();
        await _db.Entry(u).Reference(x => x.Perfil).LoadAsync();
        return AuthService.MapUsuario(u);
    }

    public async Task<UsuarioDto> AlterarStatusAsync(int id, bool ativo)
    {
        var u = await _db.Usuarios.Include(x => x.Perfil)
            .FirstOrDefaultAsync(x => x.IdUsuario == id)
            ?? throw new ApiException("NAO_ENCONTRADO", "Usuário não encontrado.", 404);

        u.Status = ativo;
        await _db.SaveChangesAsync();
        return AuthService.MapUsuario(u);
    }

    public async Task<IEnumerable<PerfilDto>> ListarPerfisAsync()
    {
        return await _db.Perfis
            .OrderBy(p => p.IdPerfil)
            .Select(p => new PerfilDto(p.IdPerfil, p.Nome))
            .ToListAsync();
    }
}
=======
using StudyToTech.Models;
using StudyToTech.Repositories;

namespace StudyToTech.Services;

public class UsuarioService
{
    private readonly UsuarioRepository usuarioRepository;

    public UsuarioService()
    {
        usuarioRepository = new UsuarioRepository();
    }

    public void CadastrarUsuario()
    {
        Usuario usuario = new Usuario();

        Console.Write("Nome: ");
        usuario.Nome = Console.ReadLine()!;

        Console.Write("Email: ");
        usuario.Email = Console.ReadLine()!;

        Console.Write("Senha: ");
        usuario.SenhaHash = Console.ReadLine()!;

        Console.Write("ID do perfil: ");
        usuario.PerfilIdPerfil = int.Parse(Console.ReadLine()!);

        usuario.Status = true;
        usuario.DataCadastro = DateTime.Now;

        usuarioRepository.CadastrarUsuario(usuario);

        Console.WriteLine("Usuário cadastrado com sucesso!");
    }

    public void ListarUsuarios()
    {
        List<Usuario> usuarios = usuarioRepository.ListarUsuarios();

        if (usuarios.Count == 0)
        {
            Console.WriteLine("Nenhum usuário cadastrado.");
            return;
        }

        foreach (Usuario usuario in usuarios)
        {
            Console.WriteLine($"ID: {usuario.IdUsuario}");
            Console.WriteLine($"Nome: {usuario.Nome}");
            Console.WriteLine($"Email: {usuario.Email}");
            Console.WriteLine($"Perfil: {usuario.PerfilIdPerfil}");
            Console.WriteLine($"Status: {(usuario.Status ? "Ativo" : "Inativo")}");
            Console.WriteLine($"Data de cadastro: {usuario.DataCadastro}");
            Console.WriteLine("-----------------------------");
        }
    }

    public Usuario? BuscarUsuario(int id)
    {
        return usuarioRepository.BuscarUsuario(id);
    }

    public List<Usuario> ObterUsuarios()
    {
        return usuarioRepository.ListarUsuarios();
    }
}
>>>>>>> 3d83e06e01e5904b6dc92b64aff78a5fe34e53e2
