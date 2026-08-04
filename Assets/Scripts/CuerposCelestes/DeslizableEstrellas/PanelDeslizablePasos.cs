using System.Collections;
using UnityEngine;

/// <summary>
/// Controla el deslizado progresivo del panel según el paso actual.
/// Cualquier botón (avanzar lineal, o cada opción de una bifurcación)
/// llama públicamente a IrAPaso(pasoObjetivo) desde su propio OnClick().
/// La bifurcación no requiere lógica adicional: dos botones distintos
/// simplemente apuntan a dos PasoPanel distintos.
/// </summary>
public class PanelDeslizablePasos : MonoBehaviour
{
    [Header("Panel")]
    [Tooltip("RectTransform con RectMask2D (o Mask) que se agranda progresivamente para revelar el contenido.")]
    public RectTransform mascara;
    [Tooltip("Altura total del panel completamente desplegado.")]
    public float alturaTotal = 800f;
    public float duracionAnimacion = 0.4f;

    [Header("Flujo")]
    public PasoPanel pasoInicial;

    private PasoPanel pasoActual;
    private Coroutine animacionEnCurso;

    void Start()
    {
        if (mascara != null)
            mascara.sizeDelta = new Vector2(mascara.sizeDelta.x, 0f);

        if (pasoInicial != null)
            IrAPaso(pasoInicial);
    }

    public void IrAPaso(PasoPanel siguiente)
    {
        if (siguiente == null) return;

        pasoActual?.Salir();
        pasoActual = siguiente;

        if (animacionEnCurso != null) StopCoroutine(animacionEnCurso);
        animacionEnCurso = StartCoroutine(AnimarMascara(siguiente.fraccionObjetivo));

        pasoActual.Entrar();
    }

    /// <summary>
    /// Vuelve al paso inicial. A diferencia de apagar/prender los GameObjects
    /// a mano desde afuera, esto pasa por IrAPaso para que el paso actualmente
    /// activo reciba correctamente su Salir() y el estado interno (pasoActual)
    /// quede sincronizado. Llamar esto al salir de la sección/ventana.
    /// </summary>
    public void ResetearPaneles()
    {
        if (animacionEnCurso != null) StopCoroutine(animacionEnCurso);

        if (mascara != null)
            mascara.sizeDelta = new Vector2(mascara.sizeDelta.x, 0f);

        pasoActual?.Salir();
        pasoActual = null;

        if (pasoInicial != null)
            IrAPaso(pasoInicial);
    }

    private IEnumerator AnimarMascara(float fraccionObjetivo)
    {
        float alturaInicial = mascara.sizeDelta.y;
        float alturaFinal = alturaTotal * fraccionObjetivo;
        float t = 0f;

        while (t < duracionAnimacion)
        {
            t += Time.deltaTime;
            float progreso = Mathf.SmoothStep(0f, 1f, t / duracionAnimacion);
            float altura = Mathf.Lerp(alturaInicial, alturaFinal, progreso);
            mascara.sizeDelta = new Vector2(mascara.sizeDelta.x, altura);
            yield return null;
        }

        mascara.sizeDelta = new Vector2(mascara.sizeDelta.x, alturaFinal);
    }
}