using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OracionManager : MonoBehaviour
{
    [Header("Texto de la oracion")]
    [SerializeField] TextMeshProUGUI textoAntes;
    [SerializeField] TextMeshProUGUI textoDespues;

    [Header("Tarjetas arrastrables")]
    [SerializeField] GameObject tarjeta1;
    [SerializeField] GameObject tarjeta2;
    [SerializeField] GameObject tarjeta3;

    [Header("Feedback")]
    public GameObject panelCorrecto;
    public GameObject panelIncorrecto;
    public float tiempoFeedback = 1.5f;

    [Header("Fin del juego")]
    public GameObject panelFinJuego; // asignar en el inspector

    [Header("Preguntas")]
    public DatosOracion[] preguntas;

    private string respuestaCorrecta;
    private GameObject[] tarjetas;
    private List<int> indicesRestantes = new List<int>();

    void Start()
    {
        tarjetas = new GameObject[] { tarjeta1, tarjeta2, tarjeta3 };

        if (panelCorrecto) panelCorrecto.SetActive(false);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);
        if (panelFinJuego) panelFinJuego.SetActive(false);

        StartCoroutine(IniciarAlFrame());
    }

    IEnumerator IniciarAlFrame()
    {
        // Esperar un frame para que el Canvas calcule posiciones reales
        yield return null;
        InicializarIndices();
        CargarSiguientePreguntaAleatoria();
    }

    void InicializarIndices()
    {
        indicesRestantes.Clear();
        for (int i = 0; i < preguntas.Length; i++)
            indicesRestantes.Add(i);
    }

    void CargarSiguientePreguntaAleatoria()
    {
        if (indicesRestantes.Count == 0)
        {
            FinDelJuego();
            return;
        }

        // Elegir un indice aleatorio de los que quedan sin repetir
        int pos = Random.Range(0, indicesRestantes.Count);
        int idx = indicesRestantes[pos];
        indicesRestantes.RemoveAt(pos);

        CargarPregunta(idx);
    }

    void CargarPregunta(int idx)
    {
        DatosOracion actual = preguntas[idx];

        textoAntes.text = actual.parteAntes;
        textoDespues.text = actual.parteDespues;
        respuestaCorrecta = actual.respuestaCorrecta;

        List<string> lista = new List<string>
        {
            actual.respuestaCorrecta,
            actual.opcion2,
            actual.opcion3
        };

        // Mezclar opciones aleatoriamente
        for (int i = 0; i < lista.Count; i++)
        {
            int rand = Random.Range(i, lista.Count);
            (lista[i], lista[rand]) = (lista[rand], lista[i]);
        }

        for (int i = 0; i < tarjetas.Length; i++)
        {
            tarjetas[i].SetActive(true);

            TarjetaOracion tarjeta = tarjetas[i].GetComponent<TarjetaOracion>();
            tarjeta.ResetearEstado(); // restaura posicion y blocksRaycasts
            tarjeta.valorRespuesta = lista[i];
            tarjeta.manager = this;

            tarjetas[i].GetComponentInChildren<TextMeshProUGUI>().text = lista[i];
        }

        if (panelCorrecto) panelCorrecto.SetActive(false);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);
    }

    public void VerificarRespuesta(TarjetaOracion tarjeta)
    {
        if (tarjeta.valorRespuesta == respuestaCorrecta)
            StartCoroutine(FeedbackCorrecto(tarjeta));
        else
            StartCoroutine(FeedbackIncorrecto(tarjeta));
    }

    IEnumerator FeedbackCorrecto(TarjetaOracion tarjeta)
    {
        tarjeta.ResetearEstado(); // asegurar blocksRaycasts = true antes de desactivar
        tarjeta.gameObject.SetActive(false);
        if (panelCorrecto) panelCorrecto.SetActive(true);
        yield return new WaitForSeconds(tiempoFeedback);
        if (panelCorrecto) panelCorrecto.SetActive(false);

        CargarSiguientePreguntaAleatoria();
    }

    IEnumerator FeedbackIncorrecto(TarjetaOracion tarjeta)
    {
        if (panelIncorrecto) panelIncorrecto.SetActive(true);
        yield return new WaitForSeconds(tiempoFeedback);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);
        tarjeta.ResetearEstado();
    }

    void FinDelJuego()
    {
        // Ocultar tarjetas al terminar
        foreach (var t in tarjetas)
            t.SetActive(false);

        if (panelFinJuego) panelFinJuego.SetActive(true);
        Debug.Log("Juego terminado");
    }
}