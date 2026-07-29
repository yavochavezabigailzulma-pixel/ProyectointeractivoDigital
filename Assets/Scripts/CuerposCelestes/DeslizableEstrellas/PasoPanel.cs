using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Representa un paso/segmento del panel deslizable.
/// No conoce a los pasos siguientes: eso se cablea desde el botón
/// correspondiente (OnClick -> PanelDeslizablePasos.IrAPaso(esteObjetivo)),
/// lo que permite bifurcaciones sin ninguna lógica extra en código.
/// </summary>
public class PasoPanel : MonoBehaviour
{
    [Header("Progreso del panel")]
    [Tooltip("Qué fracción del panel debe quedar revelada al llegar a este paso (0 = nada, 1 = completo).")]
    [Range(0f, 1f)]
    public float fraccionObjetivo = 0.25f;

    [Header("Contenido propio de este paso (opcional)")]
    [Tooltip("Se activa al entrar a este paso y se desactiva al salir. Útil si cada paso tiene textos/imágenes propias.")]
    public GameObject contenido;

    [Header("Acciones")]
    [Tooltip("Se ejecuta apenas este paso se vuelve el actual.")]
    public UnityEvent onEntrar;
    [Tooltip("Se ejecuta cuando se abandona este paso (antes de pasar al siguiente).")]
    public UnityEvent onSalir;

    public void Entrar()
    {
        if (contenido != null) contenido.SetActive(true);
        onEntrar?.Invoke();
    }

    public void Salir()
    {
        onSalir?.Invoke();
        if (contenido != null) contenido.SetActive(false);
    }
}
