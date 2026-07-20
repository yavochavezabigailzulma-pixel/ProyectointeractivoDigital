using System.Collections.Generic;

/// <summary>
/// Lleva registro de qué hints ya se mostraron/completaron, pero SOLO en memoria (RAM).
/// A diferencia de PlayerPrefs, esto NO se guarda en disco: al cerrar el Play en el editor
/// o al cerrar la aplicación, se pierde por completo y en la próxima sesión los hints
/// vuelven a aparecer. Dentro de una misma sesión, una vez marcado, no se repite.
/// </summary>
public static class RegistroHintsSesion
{
    private static readonly HashSet<string> completados = new HashSet<string>();

    public static bool EstaCompletado(string clave)
    {
        return completados.Contains(clave);
    }

    public static void MarcarCompletado(string clave)
    {
        completados.Add(clave);
    }

    public static void Resetear(string clave)
    {
        completados.Remove(clave);
    }

    public static void ResetearTodo()
    {
        completados.Clear();
    }
}
