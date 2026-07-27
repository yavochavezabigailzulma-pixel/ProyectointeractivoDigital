using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class SwipeHint : MonoBehaviour, IHintAnimado
{
    public enum TipoTrayectoria { Recta, Curva }
    public enum TipoEscala { Ninguna, Achicar, Agrandar }
    public enum TipoRotacion { Ninguna, Inclinacion, Direccional, DeAaB }

    [Header("Ocultamiento")]
    [SerializeField] private float velocidadFadeOutRapido = 6f;

    [Header("Movimiento")]
    [SerializeField] private TipoTrayectoria trayectoria = TipoTrayectoria.Recta;
    [SerializeField] private float distancia = 1.5f;
    [SerializeField] private float alturaCurva = 0.5f;
    [SerializeField] private float duracionMovimiento = 0.6f;
    [SerializeField] private float pausaEntreCiclos = 0.4f;
    [SerializeField] private AnimationCurve curvaMovimiento = AnimationCurve.EaseInOut(0, 0, 1, 1);

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

    private SpriteRenderer sr;
    private Image img;
    private Vector3 posInicial, escalaInicial;
    private Quaternion rotInicial;
    private Coroutine loopCoroutine;
    private bool ocultando = false;

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
        ocultando = false;
        transform.localPosition = posInicial;
        transform.localScale = escalaInicial;
        transform.localRotation = rotInicial;
        loopCoroutine = StartCoroutine(LoopSwipe());
    }

    void OnDisable()
    {
        if (loopCoroutine != null) StopCoroutine(loopCoroutine);
    }

    /// <summary>Llamado únicamente por HintSequencer.</summary>
    public void Ocultar(Action alTerminar)
    {
        if (ocultando) return;
        ocultando = true;

        if (loopCoroutine != null) StopCoroutine(loopCoroutine);
        StartCoroutine(FadeOutYNotificar(alTerminar));
    }

    IEnumerator FadeOutYNotificar(Action alTerminar)
    {
        float alpha = ObtenerAlphaActual();
        float maximo = 1f, transcurrido = 0f;

        while (alpha > 0f && transcurrido < maximo)
        {
            float delta = Time.unscaledDeltaTime;
            alpha -= velocidadFadeOutRapido * delta;
            transcurrido += delta;
            SetAlpha(Mathf.Clamp01(alpha));
            yield return null;
        }

        SetAlpha(0f);
        gameObject.SetActive(false);
        alTerminar?.Invoke();
    }

    float ObtenerAlphaActual()
    {
        if (sr != null) return sr.color.a;
        if (img != null) return img.color.a;
        return 0f;
    }

    IEnumerator LoopSwipe()
    {
        while (true)
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