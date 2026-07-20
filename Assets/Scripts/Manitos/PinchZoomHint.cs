using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class PinchZoomHint : MonoBehaviour, INotificaHintCompletado
{
    public enum TipoGesto { ZoomIn, ZoomOut, Alternar }
    public enum TipoTrayectoria { Recta, Curva }
    public enum TipoEscala { Ninguna, Achicar, Agrandar }
    public enum TipoRotacion { Ninguna, Inclinacion, Direccional, DeAaB }

    [Header("Persistencia")]
    [SerializeField] private string claveGuardado = "hint_pinch_tutorial"; // única por cada hint/ventana
    [SerializeField] private bool resetearParaPruebas = false;

    [Header("Detección de gesto del usuario")]
    [SerializeField] private bool detectarPinchDelUsuario = true;
    [SerializeField] private float cambioMinimoDistancia = 60f; // en pixeles de pantalla
    [SerializeField] private float velocidadFadeOutRapido = 6f;

    [Header("Referencias")]
    [SerializeField] private Transform mano1;
    [SerializeField] private Transform mano2;
    [SerializeField] private Vector3 centro = Vector3.zero;

    [Header("Gesto")]
    [SerializeField] private TipoGesto gesto = TipoGesto.Alternar;
    [SerializeField] private float distanciaCercana = 0.4f;
    [SerializeField] private float distanciaLejana = 1.6f;

    [Header("Movimiento")]
    [SerializeField] private TipoTrayectoria trayectoria = TipoTrayectoria.Recta;
    [SerializeField] private float alturaCurva = 0.3f;
    [SerializeField] private float duracionMovimiento = 0.7f;
    [SerializeField] private float pausaEntreCiclos = 0.4f;
    [SerializeField]
    private AnimationCurve curvaMovimiento =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Fade")]
    [SerializeField] private bool usarFade = true;
    [SerializeField] private float duracionFade = 0.15f;

    [Header("Feedback de escala")]
    [SerializeField] private TipoEscala tipoEscala = TipoEscala.Achicar;
    [SerializeField] private float intensidadEscala = 0.12f;

    [Header("Rotación")]
    [SerializeField] private TipoRotacion tipoRotacion = TipoRotacion.Ninguna;
    [SerializeField] private float anguloRotacion = 15f;
    [SerializeField] private float anguloInicial = 0f;
    [SerializeField] private float anguloFinal = -20f;

    [Header("Eventos")]
    [Tooltip("Se dispara cuando el hint termina de ocultarse: al completarlo el usuario, o al detectarse que ya estaba visto anteriormente.")]
    [SerializeField] private UnityEvent onHintCompletado;
    public UnityEvent OnHintCompletado => onHintCompletado;

    private SpriteRenderer sr1, sr2;
    private Image img1, img2;
    private Vector3 escalaInicial1, escalaInicial2;
    private Quaternion rotInicial1, rotInicial2;
    private bool cicloZoomIn = true;

    private Coroutine loopCoroutine;
    private bool ocultoPermanentemente = false;
    private float distanciaInicialToques = -1f;

    void Awake()
    {
        sr1 = mano1.GetComponent<SpriteRenderer>();
        sr2 = mano2.GetComponent<SpriteRenderer>();
        img1 = mano1.GetComponent<Image>();
        img2 = mano2.GetComponent<Image>();
        escalaInicial1 = mano1.localScale;
        escalaInicial2 = mano2.localScale;
        rotInicial1 = mano1.localRotation;
        rotInicial2 = mano2.localRotation;
    }

    void OnEnable()
    {
        if (resetearParaPruebas)
            RegistroHintsSesion.Resetear(claveGuardado);

        if (RegistroHintsSesion.EstaCompletado(claveGuardado))
        {
            gameObject.SetActive(false);
            onHintCompletado?.Invoke(); // ya estaba visto: avisamos igual para no trabar una secuencia
            return;
        }

        ocultoPermanentemente = false;
        loopCoroutine = StartCoroutine(LoopPinch());
    }

    void OnDisable()
    {
        if (loopCoroutine != null) StopCoroutine(loopCoroutine);
    }

    void Update()
    {
        if (!detectarPinchDelUsuario || ocultoPermanentemente) return;
        DetectarGestoUsuario();
    }

    void DetectarGestoUsuario()
    {
        // Touch real (móvil): dos dedos
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);
            float distanciaActual = Vector2.Distance(t0.position, t1.position);

            if (distanciaInicialToques < 0f)
            {
                distanciaInicialToques = distanciaActual;
            }
            else if (Mathf.Abs(distanciaActual - distanciaInicialToques) >= cambioMinimoDistancia)
            {
                MarcarComoCompletadoYOcultar();
            }
        }
        else
        {
            distanciaInicialToques = -1f;
        }

        // Simulación en editor/PC: scroll del mouse actúa como pinch (estándar en la industria para pruebas)
#if UNITY_EDITOR
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            MarcarComoCompletadoYOcultar();
        }
#endif
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
        if (sr1 != null) return sr1.color.a;
        if (img1 != null) return img1.color.a;
        return 0f;
    }

    IEnumerator LoopPinch()
    {
        while (!ocultoPermanentemente)
        {
            bool haciaAfuera = gesto == TipoGesto.ZoomIn ||
                                (gesto == TipoGesto.Alternar && cicloZoomIn);

            yield return StartCoroutine(HacerPinch(haciaAfuera));

            if (gesto == TipoGesto.Alternar) cicloZoomIn = !cicloZoomIn;

            yield return new WaitForSeconds(pausaEntreCiclos);
        }
    }

    IEnumerator HacerPinch(bool haciaAfuera)
    {
        float distInicio = haciaAfuera ? distanciaCercana : distanciaLejana;
        float distFin = haciaAfuera ? distanciaLejana : distanciaCercana;

        PosicionarEnDistancia(distInicio);
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

        while (t < duracionMovimiento)
        {
            t += Time.deltaTime;
            float progreso = Mathf.Clamp01(t / duracionMovimiento);
            float curva = curvaMovimiento.Evaluate(progreso);

            float distanciaActual = Mathf.Lerp(distInicio, distFin, curva);
            float arco = trayectoria == TipoTrayectoria.Curva
                ? alturaCurva * Mathf.Sin(progreso * Mathf.PI)
                : 0f;

            Vector3 pos1 = centro + new Vector3(-distanciaActual * 0.5f, arco, 0f);
            Vector3 pos2 = centro + new Vector3(distanciaActual * 0.5f, arco, 0f);
            mano1.localPosition = pos1;
            mano2.localPosition = pos2;

            if (tipoEscala != TipoEscala.Ninguna)
            {
                float pulso = Mathf.Sin(progreso * Mathf.PI);
                float factor = tipoEscala == TipoEscala.Achicar
                    ? 1f - intensidadEscala * pulso
                    : 1f + intensidadEscala * pulso;
                mano1.localScale = escalaInicial1 * factor;
                mano2.localScale = escalaInicial2 * factor;
            }

            switch (tipoRotacion)
            {
                case TipoRotacion.Inclinacion:
                    float pulsoRot = Mathf.Sin(progreso * Mathf.PI);
                    mano1.localRotation = rotInicial1 * Quaternion.Euler(0, 0, anguloRotacion * pulsoRot);
                    mano2.localRotation = rotInicial2 * Quaternion.Euler(0, 0, -anguloRotacion * pulsoRot);
                    break;
                case TipoRotacion.Direccional:
                    float signoDir = haciaAfuera ? 1f : -1f;
                    float anguloDireccional = signoDir * 180f;
                    mano1.localRotation = rotInicial1 * Quaternion.Euler(0, 0, anguloDireccional);
                    mano2.localRotation = rotInicial2 * Quaternion.Euler(0, 0, 0f);
                    break;
                case TipoRotacion.DeAaB:
                    float anguloActual = Mathf.Lerp(anguloInicial, anguloFinal, curva);
                    mano1.localRotation = rotInicial1 * Quaternion.Euler(0, 0, anguloActual);
                    mano2.localRotation = rotInicial2 * Quaternion.Euler(0, 0, -anguloActual);
                    break;
            }

            if (usarFade && progreso > 1f - (duracionFade / duracionMovimiento))
            {
                float fadeOutT = (progreso - (1f - duracionFade / duracionMovimiento)) / (duracionFade / duracionMovimiento);
                SetAlpha(1f - Mathf.Clamp01(fadeOutT));
            }

            yield return null;
        }

        mano1.localScale = escalaInicial1;
        mano2.localScale = escalaInicial2;
        mano1.localRotation = rotInicial1;
        mano2.localRotation = rotInicial2;
        SetAlpha(0f);
    }

    void PosicionarEnDistancia(float dist)
    {
        mano1.localPosition = centro + new Vector3(-dist * 0.5f, 0f, 0f);
        mano2.localPosition = centro + new Vector3(dist * 0.5f, 0f, 0f);
    }

    void SetAlpha(float a)
    {
        if (sr1 != null) { var c = sr1.color; c.a = a; sr1.color = c; }
        if (sr2 != null) { var c = sr2.color; c.a = a; sr2.color = c; }
        if (img1 != null) { var c = img1.color; c.a = a; img1.color = c; }
        if (img2 != null) { var c = img2.color; c.a = a; img2.color = c; }
    }
}