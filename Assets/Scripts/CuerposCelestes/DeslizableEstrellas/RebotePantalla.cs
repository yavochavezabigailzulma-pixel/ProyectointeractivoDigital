using System.Collections;
using UnityEngine;

/// <summary>
/// Anima un GameObject (pensado para botones UI) en un rebote cíclico
/// hacia el borde izquierdo o derecho de la pantalla.
///
/// Principios de animación aplicados:
/// - Anticipación: antes de salir disparado, se "carga" hacia atrás y se comprime,
///   como un resorte antes de soltarse.
/// - Estiramiento (stretch): al viajar rápido se estira en la dirección del movimiento.
/// - Aplastamiento (squash): al chocar contra el borde se aplasta en el eje de viaje
///   y se expande en el perpendicular (conservación de volumen).
/// - Acción secundaria / seguimiento: pequeño retroceso elástico tras el impacto.
/// - Ease-out con overshoot al asentarse de vuelta en la posición original.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class RebotePantalla : MonoBehaviour
{
    public enum Direccion { Izquierda, Derecha }

    [Header("Dirección y distancia")]
    public Direccion direccion = Direccion.Derecha;
    [Tooltip("Distancia en unidades locales del RectTransform que recorre hacia el borde.")]
    public float distanciaViaje = 300f;

    [Header("Timings (segundos)")]
    public float duracionAnticipacion = 0.12f;
    public float duracionIda = 0.35f;
    public float duracionImpacto = 0.15f;
    public float duracionVuelta = 0.45f;
    public float duracionAsentado = 0.15f;
    public float pausaEntreCiclos = 0.4f;

    [Header("Squash & Stretch")]
    [Tooltip("Cuánto se comprime hacia atrás en la anticipación (0-0.5).")]
    [Range(0f, 0.5f)] public float intensidadAnticipacion = 0.15f;
    [Tooltip("Estiramiento en el eje de movimiento mientras viaja a máxima velocidad.")]
    public float estiramientoViaje = 1.25f;
    [Tooltip("Aplastamiento en el eje de movimiento al chocar contra el borde.")]
    public float aplastamientoImpacto = 0.6f;
    [Tooltip("Expansión en el eje perpendicular al chocar.")]
    public float expansionImpacto = 1.3f;
    [Tooltip("Fracción de la distancia total que retrocede elásticamente tras el impacto.")]
    [Range(0f, 0.2f)] public float retrocesoTrasImpacto = 0.05f;

    [Header("Easing")]
    public AnimationCurve curvaIda = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve curvaVuelta = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private RectTransform rect;
    private Vector2 posicionInicial;
    private Vector3 escalaInicial;
    private float signo;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        posicionInicial = rect.anchoredPosition;
        escalaInicial = rect.localScale;
        signo = direccion == Direccion.Derecha ? 1f : -1f;
    }

    void OnEnable()
    {
        StartCoroutine(CicloRebote());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        rect.anchoredPosition = posicionInicial;
        rect.localScale = escalaInicial;
    }

    IEnumerator CicloRebote()
    {
        while (true)
        {
            yield return Anticipacion();
            yield return Viaje();
            yield return Impacto();
            yield return Vuelta();
            yield return Asentado();

            if (pausaEntreCiclos > 0f)
                yield return new WaitForSeconds(pausaEntreCiclos);
        }
    }

    // Se comprime y retrocede levemente antes de salir disparado, como un resorte cargándose.
    IEnumerator Anticipacion()
    {
        float t = 0f;
        Vector2 posAnticipada = posicionInicial - new Vector2(signo * distanciaViaje * 0.08f, 0f);

        while (t < duracionAnticipacion)
        {
            t += Time.deltaTime;
            float p = t / duracionAnticipacion;

            rect.anchoredPosition = Vector2.Lerp(posicionInicial, posAnticipada, p);

            float escalaX = Mathf.Lerp(1f, 1f - intensidadAnticipacion, p);
            float escalaY = Mathf.Lerp(1f, 1f + intensidadAnticipacion * 0.6f, p);
            rect.localScale = new Vector3(escalaInicial.x * escalaX, escalaInicial.y * escalaY, escalaInicial.z);

            yield return null;
        }
    }

    // Viaje rápido hacia el borde, estirándose en la dirección del movimiento.
    IEnumerator Viaje()
    {
        float t = 0f;
        Vector2 posInicio = rect.anchoredPosition;
        Vector2 posDestino = posicionInicial + new Vector2(signo * distanciaViaje, 0f);

        while (t < duracionIda)
        {
            t += Time.deltaTime;
            float pLineal = Mathf.Clamp01(t / duracionIda);
            float p = curvaIda.Evaluate(pLineal);
            rect.anchoredPosition = Vector2.LerpUnclamped(posInicio, posDestino, p);

            // Máximo estiramiento a mitad de camino (máxima velocidad aparente).
            float velocidadRelativa = Mathf.Sin(pLineal * Mathf.PI);
            float escalaX = Mathf.Lerp(1f - intensidadAnticipacion, estiramientoViaje, velocidadRelativa);
            float escalaY = Mathf.Lerp(1f + intensidadAnticipacion * 0.6f, 1f / estiramientoViaje, velocidadRelativa);
            rect.localScale = new Vector3(escalaInicial.x * escalaX, escalaInicial.y * escalaY, escalaInicial.z);

            yield return null;
        }

        rect.anchoredPosition = posDestino;
    }

    // Choca contra el borde: se aplasta, se expande en el eje perpendicular,
    // y retrocede un poco elásticamente (acción secundaria del impacto).
    IEnumerator Impacto()
    {
        float t = 0f;
        Vector2 posBorde = posicionInicial + new Vector2(signo * distanciaViaje, 0f);
        Vector2 posRetroceso = posBorde - new Vector2(signo * distanciaViaje * retrocesoTrasImpacto, 0f);

        while (t < duracionImpacto)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duracionImpacto);
            float curvaImpacto = Mathf.Sin(p * Mathf.PI); // 0 -> 1 -> 0

            float escalaX = Mathf.Lerp(estiramientoViaje, aplastamientoImpacto, curvaImpacto);
            float escalaY = Mathf.Lerp(1f / estiramientoViaje, expansionImpacto, curvaImpacto);
            rect.localScale = new Vector3(escalaInicial.x * escalaX, escalaInicial.y * escalaY, escalaInicial.z);

            rect.anchoredPosition = Vector2.Lerp(posBorde, posRetroceso, curvaImpacto);

            yield return null;
        }

        rect.anchoredPosition = posBorde;
    }

    // Vuelve a la posición original, revirtiendo el estiramiento.
    IEnumerator Vuelta()
    {
        float t = 0f;
        Vector2 posInicio = rect.anchoredPosition;

        while (t < duracionVuelta)
        {
            t += Time.deltaTime;
            float pLineal = Mathf.Clamp01(t / duracionVuelta);
            float p = curvaVuelta.Evaluate(pLineal);
            rect.anchoredPosition = Vector2.LerpUnclamped(posInicio, posicionInicial, p);

            float velocidadRelativa = Mathf.Sin(pLineal * Mathf.PI);
            float estiramientoVuelta = 1f + (estiramientoViaje - 1f) * 0.7f;
            float escalaX = Mathf.Lerp(aplastamientoImpacto, estiramientoVuelta, velocidadRelativa);
            float escalaY = Mathf.Lerp(expansionImpacto, 1f / estiramientoVuelta, velocidadRelativa);
            rect.localScale = new Vector3(escalaInicial.x * escalaX, escalaInicial.y * escalaY, escalaInicial.z);

            yield return null;
        }

        rect.anchoredPosition = posicionInicial;
    }

    // Asienta la escala final con un leve overshoot elástico (ease-out-back).
    IEnumerator Asentado()
    {
        float t = 0f;
        Vector3 escalaInicio = rect.localScale;

        while (t < duracionAsentado)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duracionAsentado);
            rect.localScale = Vector3.LerpUnclamped(escalaInicio, escalaInicial, EaseOutBack(p));
            yield return null;
        }

        rect.localScale = escalaInicial;
    }

    private static float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}
