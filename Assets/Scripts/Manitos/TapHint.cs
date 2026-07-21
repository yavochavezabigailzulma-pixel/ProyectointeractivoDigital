using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class TapHint : MonoBehaviour, INotificaHintCompletado
{
    public enum TipoTrayectoriaAcercamiento { Recta, Curva }

    [Header("Persistencia")]
    [SerializeField] private string claveGuardado = "hint_tap_tutorial"; // única por cada hint/ventana
    [SerializeField] private bool resetearParaPruebas = false;

    [Header("Detección del tap real del usuario")]
    [SerializeField] private bool detectarTapDelUsuario = true;
    [SerializeField] private RectTransform puntoObjetivoUI; // usar si el punto es un elemento UI
    [SerializeField] private Transform puntoObjetivoMundo;  // usar si el punto es un objeto en escena (usa Camera.main)
    [SerializeField] private float radioDeteccion = 80f;    // en pixeles de pantalla
    [SerializeField] private float velocidadFadeOutRapido = 6f;

    [Header("Punto objetivo (relativo, si no usas las referencias de arriba)")]
    [SerializeField] private Vector3 puntoObjetivoLocal = Vector3.zero;

    [Header("Posición de origen del gesto")]
    [SerializeField] private Vector3 offsetInicio = new Vector3(0.6f, 0.6f, 0f); // desde dónde "entra" la mano
    [SerializeField] private TipoTrayectoriaAcercamiento trayectoria = TipoTrayectoriaAcercamiento.Curva;
    [SerializeField] private float alturaCurvaAcercamiento = 0.3f;

    [Header("Tiempos")]
    [SerializeField] private float duracionAcercamiento = 0.5f;
    [SerializeField] private float duracionPresion = 0.15f;
    [SerializeField] private float duracionSostenido = 0.1f;
    [SerializeField] private float duracionLiberacion = 0.2f;
    [SerializeField] private float pausaEntreCiclos = 0.5f;
    [SerializeField]
    private AnimationCurve curvaAcercamiento =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Fade")]
    [SerializeField] private bool usarFade = true;
    [SerializeField] private float duracionFade = 0.15f;

    [Header("Escala al presionar")]
    [SerializeField] private bool usarEscalaAlPresionar = true;
    [SerializeField] private float intensidadEscalaPresion = 0.2f; // 0.2 = se achica 20%

    [Header("Inclinación al presionar")]
    [SerializeField] private bool usarInclinacion = true;
    [SerializeField] private float anguloInclinacion = -12f;

    [Header("Anillo de contacto (feedback visual opcional)")]
    [SerializeField] private bool mostrarAnilloContacto = true;
    [SerializeField] private SpriteRenderer anilloContactoSprite; // sprite circular opcional, hijo de este objeto
    [SerializeField] private float escalaMaxAnillo = 1.8f;

    [Header("Eventos")]
    [Tooltip("Se dispara cuando el hint termina de ocultarse: al completarlo el usuario, o al detectarse que ya estaba visto anteriormente.")]
    [SerializeField] private UnityEvent onHintCompletado;
    public UnityEvent OnHintCompletado => onHintCompletado;

    private SpriteRenderer sr;
    private Image img;
    private Vector3 escalaInicial;
    private Quaternion rotInicial;
    private Vector3 posObjetivoLocal;
    private Vector3 posInicioLocal;

    private Coroutine loopCoroutine;
    private bool ocultoPermanentemente = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        img = GetComponent<Image>();
        escalaInicial = transform.localScale;
        rotInicial = transform.localRotation;
    }

    void OnEnable()
    {
        if (resetearParaPruebas)
            RegistroHintsSesion.Resetear(claveGuardado);

        if (RegistroHintsSesion.EstaCompletado(claveGuardado))
        {
            StartCoroutine(NotificarYaCompletadoDiferido());
            return;
        }

        // Estas dos líneas son las que se perdieron:
        posObjetivoLocal = puntoObjetivoLocal;
        posInicioLocal = puntoObjetivoLocal + offsetInicio;

        ocultoPermanentemente = false;
        loopCoroutine = StartCoroutine(LoopTap());
    }

    IEnumerator NotificarYaCompletadoDiferido()
    {
        yield return null;
        gameObject.SetActive(false);
        onHintCompletado?.Invoke();
    }

    void OnDisable()
    {
        if (loopCoroutine != null) StopCoroutine(loopCoroutine);
    }

    void Update()
    {
        if (!detectarTapDelUsuario || ocultoPermanentemente) return;
        DetectarTapUsuario();
    }

    void DetectarTapUsuario()
    {
        Vector2? posicionInput = null;

        if (Input.GetMouseButtonDown(0))
            posicionInput = Input.mousePosition;
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            posicionInput = Input.GetTouch(0).position;

        if (posicionInput == null) return;

        Vector2 puntoObjetivoPantalla = ObtenerPuntoObjetivoEnPantalla();
        float distancia = Vector2.Distance(posicionInput.Value, puntoObjetivoPantalla);

        if (distancia <= radioDeteccion)
        {
            MarcarComoCompletadoYOcultar();
        }
    }

    Vector2 ObtenerPuntoObjetivoEnPantalla()
    {
        if (puntoObjetivoUI != null)
        {
            return RectTransformUtility.WorldToScreenPoint(null, puntoObjetivoUI.position);
        }
        else if (puntoObjetivoMundo != null)
        {
            return Camera.main.WorldToScreenPoint(puntoObjetivoMundo.position);
        }
        else
        {
            // fallback: usa la posición mundial de este mismo objeto en su punto objetivo
            return Camera.main.WorldToScreenPoint(transform.parent != null
                ? transform.parent.TransformPoint(posObjetivoLocal)
                : posObjetivoLocal);
        }
    }

    public void MarcarComoCompletadoYOcultar()
    {
        if (ocultoPermanentemente) return;
        ocultoPermanentemente = true;

        RegistroHintsSesion.MarcarCompletado(claveGuardado);

        if (loopCoroutine != null) StopCoroutine(loopCoroutine);
        StartCoroutine(FadeOutRapidoYDesactivar());
    }

    IEnumerator FadeOutRapidoYDesactivar()
    {
        float alphaActual = ObtenerAlphaActual();
        while (alphaActual > 0f)
        {
            alphaActual -= velocidadFadeOutRapido * Time.deltaTime;
            SetAlpha(Mathf.Clamp01(alphaActual));
            yield return null;
        }
        gameObject.SetActive(false);
        onHintCompletado?.Invoke();
    }

    float ObtenerAlphaActual()
    {
        if (sr != null) return sr.color.a;
        if (img != null) return img.color.a;
        return 0f;
    }

    IEnumerator LoopTap()
    {
        while (!ocultoPermanentemente)
        {
            yield return StartCoroutine(HacerTap());
            yield return new WaitForSeconds(pausaEntreCiclos);
        }
    }

    IEnumerator HacerTap()
    {
        transform.localPosition = posInicioLocal;
        transform.localRotation = rotInicial;
        transform.localScale = escalaInicial;
        SetAlpha(0f);
        SetAnillo(0f, 0f);

        // --- Fade in ---
        if (usarFade)
        {
            float fadeT = 0f;
            while (fadeT < duracionFade)
            {
                fadeT += Time.deltaTime;
                SetAlpha(Mathf.Clamp01(fadeT / duracionFade));
                yield return null;
            }
        }
        else SetAlpha(1f);

        // --- Fase 1: acercamiento hacia el punto ---
        float t = 0f;
        while (t < duracionAcercamiento)
        {
            t += Time.deltaTime;
            float progreso = Mathf.Clamp01(t / duracionAcercamiento);
            float curva = curvaAcercamiento.Evaluate(progreso);

            Vector3 posBase = Vector3.Lerp(posInicioLocal, posObjetivoLocal, curva);

            if (trayectoria == TipoTrayectoriaAcercamiento.Curva)
            {
                float arco = alturaCurvaAcercamiento * Mathf.Sin(progreso * Mathf.PI);
                posBase += Vector3.up * arco;
            }

            transform.localPosition = posBase;
            yield return null;
        }
        transform.localPosition = posObjetivoLocal;

        // --- Fase 2: presión (escala + inclinación) ---
        float tp = 0f;
        while (tp < duracionPresion)
        {
            tp += Time.deltaTime;
            float progreso = Mathf.Clamp01(tp / duracionPresion);

            if (usarEscalaAlPresionar)
                transform.localScale = escalaInicial * Mathf.Lerp(1f, 1f - intensidadEscalaPresion, progreso);

            if (usarInclinacion)
                transform.localRotation = rotInicial * Quaternion.Euler(0, 0, Mathf.Lerp(0f, anguloInclinacion, progreso));

            if (mostrarAnilloContacto)
                SetAnillo(progreso, 1f - progreso * 0.5f);

            yield return null;
        }

        // --- Fase 3: sostenido (breve pausa en el punto de contacto) ---
        yield return new WaitForSeconds(duracionSostenido);

        // --- Fase 4: liberación (vuelve a escala/rotación normal) ---
        float tl = 0f;
        Vector3 escalaAlPresionar = transform.localScale;
        Quaternion rotAlPresionar = transform.localRotation;
        while (tl < duracionLiberacion)
        {
            tl += Time.deltaTime;
            float progreso = Mathf.Clamp01(tl / duracionLiberacion);

            transform.localScale = Vector3.Lerp(escalaAlPresionar, escalaInicial, progreso);
            transform.localRotation = Quaternion.Slerp(rotAlPresionar, rotInicial, progreso);

            if (mostrarAnilloContacto)
                SetAnillo(1f, Mathf.Lerp(0.5f, escalaMaxAnillo, progreso) / escalaMaxAnillo * (1f - progreso));

            // fade out al final de la liberación
            if (usarFade && progreso > 1f - (duracionFade / duracionLiberacion))
            {
                float fadeOutT = (progreso - (1f - duracionFade / duracionLiberacion)) / (duracionFade / duracionLiberacion);
                SetAlpha(1f - Mathf.Clamp01(fadeOutT));
            }

            yield return null;
        }

        transform.localScale = escalaInicial;
        transform.localRotation = rotInicial;
        SetAlpha(0f);
        SetAnillo(0f, 0f);
    }

    void SetAnillo(float progresoEscala, float alpha)
    {
        if (anilloContactoSprite == null) return;
        anilloContactoSprite.transform.localScale = Vector3.one * Mathf.Lerp(0.5f, escalaMaxAnillo, progresoEscala);
        var c = anilloContactoSprite.color;
        c.a = Mathf.Clamp01(alpha);
        anilloContactoSprite.color = c;
    }

    void SetAlpha(float a)
    {
        if (sr != null) { var c = sr.color; c.a = a; sr.color = c; }
        else if (img != null) { var c = img.color; c.a = a; img.color = c; }
    }
}