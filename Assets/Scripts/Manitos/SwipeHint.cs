using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class SwipeHint : MonoBehaviour, INotificaHintCompletado
{
    public enum TipoTrayectoria { Recta, Curva }
    public enum TipoEscala { Ninguna, Achicar, Agrandar }
    public enum TipoRotacion { Ninguna, Inclinacion, Direccional, DeAaB }

    [Header("Persistencia")]
    [SerializeField] private string claveGuardado = "hint_swipe_tutorial"; // única por cada hint/ventana
    [SerializeField] private bool resetearParaPruebas = false; // marca esto en el Inspector para forzar que vuelva a aparecer

    [Header("Detección de gesto del usuario")]
    [SerializeField] private bool detectarSwipeDelUsuario = true;
    [SerializeField] private float distanciaMinimaSwipe = 50f; // en pixeles de pantalla
    [SerializeField] private float velocidadFadeOutRapido = 6f; // qué tan rápido desaparece al detectar el gesto

    [Header("Movimiento")]
    [SerializeField] private TipoTrayectoria trayectoria = TipoTrayectoria.Recta;
    [SerializeField] private float distancia = 1.5f;
    [SerializeField] private float alturaCurva = 0.5f;
    [SerializeField] private float duracionMovimiento = 0.6f;
    [SerializeField] private float pausaEntreCiclos = 0.4f;
    [SerializeField]
    private AnimationCurve curvaMovimiento =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Fade")]
    [SerializeField] private bool usarFade = true;
    [SerializeField] private float duracionFade = 0.15f;

    [Header("Feedback de escala")]
    [SerializeField] private TipoEscala tipoEscala = TipoEscala.Achicar;
    [SerializeField] private float intensidadEscala = 0.15f;

    [Header("Rotación")]
    [SerializeField] private TipoRotacion tipoRotacion = TipoRotacion.Ninguna;
    [SerializeField] private float anguloRotacion = 15f;
    [SerializeField] private float anguloInicial = 0f;
    [SerializeField] private float anguloFinal = -20f;

    [Header("Eventos")]
    [Tooltip("Se dispara cuando el hint termina de ocultarse: al completarlo el usuario, o al detectarse que ya estaba visto anteriormente.")]
    [SerializeField] private UnityEvent onHintCompletado;
    public UnityEvent OnHintCompletado => onHintCompletado;

    private SpriteRenderer sr;
    private Image img;
    private Vector3 posInicial;
    private Vector3 escalaInicial;
    private Quaternion rotInicial;

    private Coroutine loopCoroutine;
    private bool ocultoPermanentemente = false;
    private Vector2 posicionInicialToque;
    private bool siguiendoToque = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        img = GetComponent<Image>();
        posInicial = transform.localPosition;
        escalaInicial = transform.localScale;
        rotInicial = transform.localRotation;
    }

    void OnEnable()
    {
        if (resetearParaPruebas)
            RegistroHintsSesion.Resetear(claveGuardado);

        // Si ya se completó antes en esta misma sesión, no mostrar de nuevo
        if (RegistroHintsSesion.EstaCompletado(claveGuardado))
        {
            gameObject.SetActive(false);
            onHintCompletado?.Invoke(); // ya estaba visto: avisamos igual para no trabar una secuencia
            return;
        }

        ocultoPermanentemente = false;
        loopCoroutine = StartCoroutine(LoopSwipe());
    }

    void OnDisable()
    {
        if (loopCoroutine != null) StopCoroutine(loopCoroutine);
    }

    void Update()
    {
        if (!detectarSwipeDelUsuario || ocultoPermanentemente) return;
        DetectarGestoUsuario();
    }

    void DetectarGestoUsuario()
    {
        // Soporta mouse (editor/PC) y touch (móvil)
        if (Input.GetMouseButtonDown(0))
        {
            posicionInicialToque = Input.mousePosition;
            siguiendoToque = true;
        }
        else if (Input.GetMouseButton(0) && siguiendoToque)
        {
            float deltaX = ((Vector2)Input.mousePosition - posicionInicialToque).x;
            if (Mathf.Abs(deltaX) >= distanciaMinimaSwipe)
            {
                MarcarComoCompletadoYOcultar();
                siguiendoToque = false;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            siguiendoToque = false;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                posicionInicialToque = touch.position;
                siguiendoToque = true;
            }
            else if (touch.phase == TouchPhase.Moved && siguiendoToque)
            {
                float deltaX = (touch.position - posicionInicialToque).x;
                if (Mathf.Abs(deltaX) >= distanciaMinimaSwipe)
                {
                    MarcarComoCompletadoYOcultar();
                    siguiendoToque = false;
                }
            }
        }
    }

    // Llamable también manualmente desde otro script si detectas el swipe con tu propia lógica de gameplay
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

    IEnumerator LoopSwipe()
    {
        while (!ocultoPermanentemente)
        {
            yield return StartCoroutine(HacerSwipe());
            yield return new WaitForSeconds(pausaEntreCiclos);
        }
    }

    IEnumerator HacerSwipe()
    {
        transform.localPosition = posInicial;
        transform.localRotation = rotInicial;
        SetAlpha(0f);

        float t = 0f;

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

        Vector3 offsetAnterior = Vector3.zero;

        while (t < duracionMovimiento)
        {
            t += Time.deltaTime;
            float progreso = Mathf.Clamp01(t / duracionMovimiento);
            float curva = curvaMovimiento.Evaluate(progreso);

            Vector3 offset;
            if (trayectoria == TipoTrayectoria.Recta)
            {
                offset = Vector3.right * distancia * curva;
            }
            else
            {
                float x = distancia * curva;
                float y = alturaCurva * Mathf.Sin(progreso * Mathf.PI);
                offset = new Vector3(x, y, 0f);
            }
            transform.localPosition = posInicial + offset;

            if (tipoEscala != TipoEscala.Ninguna)
            {
                float pulso = Mathf.Sin(progreso * Mathf.PI);
                float factor = tipoEscala == TipoEscala.Achicar
                    ? 1f - intensidadEscala * pulso
                    : 1f + intensidadEscala * pulso;
                transform.localScale = escalaInicial * factor;
            }

            switch (tipoRotacion)
            {
                case TipoRotacion.Inclinacion:
                    float pulsoRot = Mathf.Sin(progreso * Mathf.PI);
                    transform.localRotation = rotInicial * Quaternion.Euler(0, 0, anguloRotacion * pulsoRot);
                    break;
                case TipoRotacion.Direccional:
                    Vector3 direccion = offset - offsetAnterior;
                    if (direccion.sqrMagnitude > 0.0001f)
                    {
                        float anguloDir = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
                        transform.localRotation = rotInicial * Quaternion.Euler(0, 0, anguloDir);
                    }
                    break;
                case TipoRotacion.DeAaB:
                    float anguloActual = Mathf.Lerp(anguloInicial, anguloFinal, curva);
                    transform.localRotation = rotInicial * Quaternion.Euler(0, 0, anguloActual);
                    break;
            }
            offsetAnterior = offset;

            if (usarFade && progreso > 1f - (duracionFade / duracionMovimiento))
            {
                float fadeOutT = (progreso - (1f - duracionFade / duracionMovimiento)) / (duracionFade / duracionMovimiento);
                SetAlpha(1f - Mathf.Clamp01(fadeOutT));
            }

            yield return null;
        }

        transform.localScale = escalaInicial;
        transform.localRotation = rotInicial;
        SetAlpha(0f);
    }

    void SetAlpha(float a)
    {
        if (sr != null) { var c = sr.color; c.a = a; sr.color = c; }
        else if (img != null) { var c = img.color; c.a = a; img.color = c; }
    }
}