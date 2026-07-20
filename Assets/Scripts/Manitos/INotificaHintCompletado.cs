using UnityEngine.Events;

/// <summary>
/// Cualquier hint que quiera poder encadenarse con HintSequencer
/// debe implementar esta interfaz y exponer su UnityEvent de finalización.
/// </summary>
public interface INotificaHintCompletado
{
    UnityEvent OnHintCompletado { get; }
}
