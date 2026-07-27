using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using System.Collections;

public class HintSequencer : MonoBehaviour
{
    [Header("Persistencia de la secuencia completa")]
    [Tooltip("Identificador único de ESTA secuencia. Si se define, una vez completada " +
         "todos sus pasos, IniciarSecuencia() no volverá a ejecutarla en esta sesión, " +
         "sin importar cuántas veces se llame ni qué configuración tengan los pasos individuales.")]
    [SerializeField] private string claveSecuencia;
    public enum TipoActivacion { Manual, AlIniciar, AlHabilitarse, AlCargarEscena }

    [Serializable]
    public class Paso
    {
        public GameObject objeto;

        [Tooltip("Opcional: si se define, este paso se saltea si ya fue completado antes en esta sesión.")]
        public string claveGuardado;

        [Tooltip("Si es mayor a 0, el paso se completa solo tras este tiempo, aunque nadie llame a CompletarPasoActual().")]
        public float duracionMaximaSegundos = 0f;
    }

    [Header("Pasos de la secuencia (en orden)")]
    [Tooltip("Cada elemento es el GameObject del hint. Deben empezar DESACTIVADOS en la escena.")]
    [SerializeField] private Paso[] pasos;

    [Header("Activación")]
    [SerializeField] private TipoActivacion tipoActivacion = TipoActivacion.Manual;
    [Tooltip("Solo se usa si el tipo de activación es 'AlCargarEscena'.")]
    [SerializeField] private string nombreEscena;
    [SerializeField] private bool unaSolaVez = false;
    [Tooltip("Si está activo, cada IniciarSecuencia() olvida el progreso previo de todos los pasos.")]
    [SerializeField] private bool repetible = false;

    [Header("Al completar toda la secuencia")]
    [Tooltip("Se invoca cuando el ÚLTIMO paso se completa y no quedan más pasos. " +
             "Útil para encadenar otro HintSequencer independiente desde otro script.")]
    [SerializeField] private UnityEvent onSecuenciaCompletada;
    public UnityEvent OnSecuenciaCompletada => onSecuenciaCompletada;

    private int indiceActual = -1;
    private bool yaCompletada = false;
    private IHintAnimado hintActual;
    private Coroutine timeoutCoroutine;

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

    public void IniciarSecuencia()
    {
        // Chequeo infalible, independiente de 'repetible', de los pasos individuales,
        // y de si la instancia del componente fue destruida/recreada por un reload de escena.
        if (!string.IsNullOrEmpty(claveSecuencia) && RegistroHintsSesion.EstaCompletado(claveSecuencia))
        {
            Debug.Log($"[HintSequencer] '{name}' ya fue completado antes (clave: {claveSecuencia}), no se reinicia.");
            return;
        }

        if (unaSolaVez && yaCompletada) return;

        DetenerTodoInmediato();

        if (repetible)
            foreach (var p in pasos)
                if (!string.IsNullOrEmpty(p.claveGuardado))
                    RegistroHintsSesion.Resetear(p.claveGuardado);

        indiceActual = -1;
        AvanzarAlSiguientePaso();
    }
    public void DetenerSecuencia()
    {
        DetenerTodoInmediato();
        indiceActual = -1;
    }

    /// <summary>
    /// Variante segura: solo completa el paso actual si su GameObject coincide
    /// con el que se pasa como parámetro. Evita que un gesto fuera de orden
    /// complete el paso equivocado (por ejemplo, un pinch que sigue disparándose
    /// cuando el paso activo ya avanzó a swipe).
    /// </summary>
    public bool CompletarPaso(GameObject hintEsperado)
    {
        if (indiceActual < 0 || indiceActual >= pasos.Length) return false;
        if (pasos[indiceActual].objeto != hintEsperado) return false;

        return CompletarPasoActual();
    }

    /// <summary>
    /// ÚNICO punto de entrada para avisar que el jugador cumplió la acción
    /// del paso actual. Debe llamarse desde scripts de gameplay externos
    /// (SeleccionPlaneta, ZoomCamara, un botón, etc.), nunca desde el propio hint.
    /// </summary>
    public bool CompletarPasoActual()
    {
        if (indiceActual < 0 || indiceActual >= pasos.Length) return false;
        if (hintActual == null) return false;

        var paso = pasos[indiceActual];
        if (!string.IsNullOrEmpty(paso.claveGuardado))
            RegistroHintsSesion.MarcarCompletado(paso.claveGuardado);

        if (timeoutCoroutine != null) StopCoroutine(timeoutCoroutine);

        var terminado = hintActual;
        hintActual = null;
        terminado.Ocultar(AvanzarAlSiguientePaso);

        return true;
    }

    void DetenerTodoInmediato()
    {
        if (timeoutCoroutine != null) StopCoroutine(timeoutCoroutine);
        if (indiceActual >= 0 && indiceActual < pasos.Length && pasos[indiceActual].objeto != null)
            pasos[indiceActual].objeto.SetActive(false);
        hintActual = null;
    }

    void AvanzarAlSiguientePaso()
    {
        if (indiceActual >= 0 && indiceActual < pasos.Length && pasos[indiceActual].objeto != null)
            pasos[indiceActual].objeto.SetActive(false);

        indiceActual++;

        if (indiceActual >= pasos.Length)
        {
            yaCompletada = true;

            if (!string.IsNullOrEmpty(claveSecuencia))
                RegistroHintsSesion.MarcarCompletado(claveSecuencia);

            hintActual = null;
            onSecuenciaCompletada?.Invoke();
            return;
        }

        Paso paso = pasos[indiceActual];

        if (paso.objeto == null)
        {
            Debug.LogWarning($"[HintSequencer] Paso {indiceActual} vacío, se salta.", this);
            AvanzarAlSiguientePaso();
            return;
        }

        if (!string.IsNullOrEmpty(paso.claveGuardado) && RegistroHintsSesion.EstaCompletado(paso.claveGuardado))
        {
            AvanzarAlSiguientePaso();
            return;
        }

        var animado = paso.objeto.GetComponent<IHintAnimado>();
        if (animado == null)
        {
            Debug.LogWarning($"[HintSequencer] '{paso.objeto.name}' no implementa IHintAnimado, se salta.", paso.objeto);
            AvanzarAlSiguientePaso();
            return;
        }

        hintActual = animado;
        paso.objeto.SetActive(true);

        if (paso.duracionMaximaSegundos > 0f)
            timeoutCoroutine = StartCoroutine(TimeoutDelPaso(paso.duracionMaximaSegundos));
    }

    IEnumerator TimeoutDelPaso(float segundos)
    {
        yield return new WaitForSecondsRealtime(segundos);
        CompletarPasoActual();
    }
}