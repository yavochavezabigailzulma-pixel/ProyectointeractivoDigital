using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TapHint : MonoBehaviour, IHintAnimado
{
    public enum TipoTrayectoriaAcercamiento { Recta, Curva }

    [Header("Ocultamiento")]
    [SerializeField] private float velocidadFadeOutRapido = 6f;

    [Header("Punto objetivo (posición local del gesto)")]
    [SerializeField] private Vector3 puntoObjetivoLocal = Vector3.zero;

    [Header("Posición de origen del gesto")]
    [SerializeField] private Vector3 offsetInicio = new Vector3(0.6f, 0.6f, 0f);
    [SerializeField] private TipoTrayectoriaAcercamiento trayectoria = TipoTrayectoriaAcercamiento.Curva;
    [SerializeField] private float alturaCurvaAcercamiento = 0.3f;

    [Header("Tiempos")]
    [SerializeField] private float duracionAcercamiento = 0.5f;
    [SerializeField] private float duracionPresion = 0.15f;
    [SerializeField] private float duracionSostenido = 0.1f;
    [SerializeField] private float duracionLiberacion = 0.2f;
    [SerializeField] private float pausaEntreCiclos = 0.5f;
    [SerializeField] private AnimationCurve curvaAcercamiento = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Fade")]
    [SerializeField] private bool usarFade = true;
    [SerializeField] private float duracionFade = 0.15f;

    [Header("Escala al presionar")]
    [SerializeField] private bool usarEscalaAlPresionar = true;
    [SerializeField] private float intensidadEscalaPresion = 0.2f;

    [Header("Inclinación al presionar")]
    [SerializeField] private bool usarInclinacion = true;
    [SerializeField] private float anguloInclinacion = -12f;

    [Header("Anillo de contacto (opcional)")]
    [SerializeField] private bool mostrarAnilloContacto = true;
    [SerializeField] private SpriteRenderer anilloContactoSprite;
    [SerializeField] private float escalaMaxAnillo = 1.8f;

    private SpriteRenderer sr;
    private Image img;
    private Vector3 escalaInicial;
    private Quaternion rotInicial;
    private Vector3 posInicioLocal;
    private Coroutine loopCoroutine;
    private bool ocultando = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        img = GetComponent<Image>();
        escalaInicial = transform.localScale;
        rotInicial = transform.localRotation;
    }

    void OnEnable()
    {
        posInicioLocal = puntoObjetivoLocal + offsetInicio;
        ocultando = false;
        loopCoroutine = StartCoroutine(LoopTap());
    }

    void OnDisable()
    {
        if (loopCoroutine != null) StopCoroutine(loopCoroutine);
    }

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
        SetAnillo(0f, 0f);
        gameObject.SetActive(false);
        alTerminar?.Invoke();
    }

    float ObtenerAlphaActual()
    {
        if (sr != null) return sr.color.a;
        if (img != null) return img.color.a;
        return 0f;
    }

    IEnumerator LoopTap()
    {
        while (true)
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

        float t = 0f;
        while (t < duracionAcercamiento)
        {
            t += Time.deltaTime;
            float progreso = Mathf.Clamp01(t / duracionAcercamiento);
            float curva = curvaAcercamiento.Evaluate(progreso);

            Vector3 posBase = Vector3.Lerp(posInicioLocal, puntoObjetivoLocal, curva);
            if (trayectoria == TipoTrayectoriaAcercamiento.Curva)
                posBase += Vector3.up * (alturaCurvaAcercamiento * Mathf.Sin(progreso * Mathf.PI));

            transform.localPosition = posBase;
            yield return null;
        }
        transform.localPosition = puntoObjetivoLocal;

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

        yield return new WaitForSeconds(duracionSostenido);

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