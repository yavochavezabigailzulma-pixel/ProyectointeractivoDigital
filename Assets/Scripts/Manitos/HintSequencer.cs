using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Muestra una lista de hints en orden: uno se activa recién cuando el anterior
/// termina de ocultarse. Funciona con cualquier hint que implemente
/// INotificaHintCompletado (PinchZoomHint, TapHint, o los que agregues después).
///
/// Modos de activación disponibles (elegir en el Inspector):
///  - Manual: llamá a IniciarSecuencia() desde un botón, un evento, o tu propio código
///            cuando se cumpla la condición que quieras.
///  - AlIniciar: arranca solo al cargar la escena (Start).
///  - AlHabilitarse: arranca cada vez que este GameObject se activa
///    (útil poniendo el sequencer como hijo de un panel: se dispara al abrir el panel).
///  - AlCargarEscena: arranca cuando se carga la escena indicada en "nombreEscena".
/// </summary>
public class HintSequencer : MonoBehaviour
{
    public enum TipoActivacion { Manual, AlIniciar, AlHabilitarse, AlCargarEscena }

    [Header("Pasos de la secuencia (en orden)")]
    [Tooltip("Cada elemento es el GameObject del hint (el que tiene PinchZoomHint, TapHint, etc). Deben empezar DESACTIVADOS en la escena.")]
    [SerializeField] private GameObject[] pasos;

    [Header("Activación")]
    [SerializeField] private TipoActivacion tipoActivacion = TipoActivacion.Manual;
    [Tooltip("Solo se usa si el tipo de activación es 'AlCargarEscena'.")]
    [SerializeField] private string nombreEscena;
    [Tooltip("Si está tildado, una vez completada la secuencia no se puede volver a disparar (protección extra, además del guardado propio de cada hint).")]
    [SerializeField] private bool unaSolaVez = false;

    private int indiceActual = -1;
    private bool yaCompletada = false;

    void Start()
    {
        if (tipoActivacion == TipoActivacion.AlIniciar)
            IniciarSecuencia();
    }

    void OnEnable()
    {
        if (tipoActivacion == TipoActivacion.AlCargarEscena)
            SceneManager.sceneLoaded += OnEscenaCargada;

        if (tipoActivacion == TipoActivacion.AlHabilitarse)
            IniciarSecuencia();
    }

    void OnDisable()
    {
        if (tipoActivacion == TipoActivacion.AlCargarEscena)
            SceneManager.sceneLoaded -= OnEscenaCargada;
    }

    void OnEscenaCargada(Scene escena, LoadSceneMode modo)
    {
        if (escena.name == nombreEscena)
            IniciarSecuencia();
    }

    /// <summary>
    /// Punto de entrada manual: llamar desde un botón (OnClick),
    /// desde tu propio script cuando se cumpla una condición, desde un
    /// Animation Event, etc.
    /// </summary>
    public void IniciarSecuencia()
    {
        if (unaSolaVez && yaCompletada) return;

        // Si esta secuencia ya se había iniciado antes (ej: se vuelve a llamar
        // al reingresar a la escena) y quedó "colgada" en algún paso intermedio,
        // hay que desuscribir ese paso ANTES de resetear el índice. Si no,
        // el listener viejo queda vivo y el evento se dispara dos veces,
        // haciendo que se salteen pasos.
        DesuscribirPasoActual();

        indiceActual = -1;
        AvanzarAlSiguientePaso();
    }

    void AvanzarAlSiguientePaso()
    {
        DesuscribirPasoActual();

        indiceActual++;

        if (indiceActual >= pasos.Length)
        {
            yaCompletada = true;
            return; // secuencia terminada
        }

        GameObject actual = pasos[indiceActual];
        if (actual == null)
        {
            Debug.LogWarning($"[HintSequencer] Paso {indiceActual} está vacío, se salta.", this);
            AvanzarAlSiguientePaso();
            return;
        }

        var notificador = actual.GetComponent<INotificaHintCompletado>();
        if (notificador == null)
        {
            Debug.LogWarning($"[HintSequencer] El objeto '{actual.name}' no implementa INotificaHintCompletado, se salta.", actual);
            AvanzarAlSiguientePaso();
            return;
        }

        notificador.OnHintCompletado.AddListener(AvanzarAlSiguientePaso);
        actual.SetActive(true);
    }

    void DesuscribirPasoActual()
    {
        if (indiceActual < 0 || indiceActual >= pasos.Length) return;

        GameObject anterior = pasos[indiceActual];
        if (anterior == null) return;

        var notificador = anterior.GetComponent<INotificaHintCompletado>();
        notificador?.OnHintCompletado.RemoveListener(AvanzarAlSiguientePaso);
    }
}