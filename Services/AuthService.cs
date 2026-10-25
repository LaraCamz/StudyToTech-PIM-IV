using Microsoft.EntityFrameworkCore;
using StudyToTech.Api.Data;
using StudyToTech.Api.DTOs;
using StudyToTech.Api.Entities;
using StudyToTech.Api.Helpers;

namespace StudyToTech.Api.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;

    public AuthService(AppDbContext db, JwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest req)
    {
        var usuario = await _db.Usuarios
            .Include(u => u.Perfil)
            .FirstOrDefaultAsync(u => u.Email == req.Email);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(req.Senha, usuario.SenhaHash))
            throw new ApiException("CREDENCIAIS_INVALIDAS", "E-mail ou senha inválidos.", 401);

        if (!usuario.Status)
            throw new ApiException("USUARIO_INATIVO", "Usuário inativo.", 401);

        var (token, expiresIn) = _jwt.GenerateToken(usuario);

        return new LoginResponse(
            token,
            "Bearer",
            expiresIn,
            MapUsuario(usuario)
        );
    }

    public async Task<UsuarioDto> RegistrarClienteAsync(RegistroClienteRequest req)
    {
        if (await _db.Usuarios.AnyAsync(u => u.Email == req.Email))
            throw new ApiException("EMAIL_JA_CADASTRADO", "E-mail já cadastrado.", 409);

        var usuario = new Usuario
        {
            Nome = req.Nome,
            Email = req.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(req.Senha),
            Status = true,
            DataCadastro = DateTime.UtcNow,
            PerfilIdPerfil = 1 // Cliente
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        await _db.Entry(usuario).Reference(u => u.Perfil).LoadAsync();
        return MapUsuario(usuario);
    }

    public async Task<UsuarioDto> GetMeAsync(int userId)
    {
        var usuario = await _db.Usuarios
            .Include(u => u.Perfil)
            .FirstOrDefaultAsync(u => u.IdUsuario == userId)
            ?? throw new ApiException("NAO_ENCONTRADO", "Usuário não encontrado.", 404);

        return MapUsuario(usuario);
    }

    public static UsuarioDto MapUsuario(Usuario u) => new(
        u.IdUsuario,
        u.Nome,
        u.Email,
        u.Status,
        new PerfilDto(u.Perfil.IdPerfil, u.Perfil.Nome),
        u.DataCadastro
    );
}
