using System.Security.Claims;

namespace StudyToTech.Api.Helpers;

public static class CurrentUser
{
    public static int GetUserId(ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? user.FindFirst("sub")?.Value;
        return int.TryParse(claim, out var id) ? id : 0;
    }

    public static string GetPerfil(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value
            ?? user.FindFirst("perfil")?.Value
            ?? string.Empty;
    }

    public static bool IsAdmin(ClaimsPrincipal user) => GetPerfil(user) == "Administrador";
    public static bool IsFuncionario(ClaimsPrincipal user) => GetPerfil(user) == "Funcionário";
    public static bool IsCliente(ClaimsPrincipal user) => GetPerfil(user) == "Cliente";
    public static bool IsFuncionarioOuAdmin(ClaimsPrincipal user) => IsFuncionario(user) || IsAdmin(user);
}
