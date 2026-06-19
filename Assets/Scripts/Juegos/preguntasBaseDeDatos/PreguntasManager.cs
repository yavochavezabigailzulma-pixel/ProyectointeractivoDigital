using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PreguntasManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI preguntaTexto;

    [Header("Opciones")]
    [SerializeField] GameObject opcion1;
    [SerializeField] GameObject opcion2;
    [SerializeField] GameObject opcion3;

    [Header("Feedback")]
    public GameObject panelCorrecto;
    public GameObject panelIncorrecto;
    public float tiempoFeedback = 1.5f;

    [Header("Fin del juego")]
    public GameObject panelFinJuego;  // asignar en el inspector

    [Header("Preguntas")]
    public DatosPreguntas[] preguntas;

    private DatosPreguntas preguntaActual;
    private GameObject[] opciones;
    private List<int> indicesRestantes = new List<int>();

    void Start()
    {
        opciones = new GameObject[] { opcion1, opcion2, opcion3 };

        if (panelCorrecto) panelCorrecto.SetActive(false);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);
        if (panelFinJuego) panelFinJuego.SetActive(false);

        StartCoroutine(IniciarAlFrame());
    }

    IEnumerator IniciarAlFrame()
    {
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

        // Elegir un índice aleatorio de los que quedan
        int pos = Random.Range(0, indicesRestantes.Count);
        int idx = indicesRestantes[pos];
        indicesRestantes.RemoveAt(pos);

        CargarPregunta(idx);
    }

    public void CargarPregunta(int idx)
    {
        preguntaActual = preguntas[idx];
        preguntaTexto.text = preguntaActual.pregunta;

        List<(string texto, bool correcta)> lista = new List<(string, bool)>
        {
            (preguntaActual.opcion1, true),
            (preguntaActual.opcion2, false),
            (preguntaActual.opcion3, false)
        };

        // Mezclar opciones
        for (int i = 0; i < lista.Count; i++)
        {
            int rand = Random.Range(i, lista.Count);
            (lista[i], lista[rand]) = (lista[rand], lista[i]);
        }

        for (int i = 0; i < opciones.Length; i++)
        {
            opciones[i].SetActive(true);

            OpcionArrastrable arrastrable = opciones[i].GetComponent<OpcionArrastrable>();
            arrastrable.ResetearEstado();
            arrastrable.esCorrecta = lista[i].correcta;
            arrastrable.manager = this;

            opciones[i].GetComponentInChildren<TextMeshProUGUI>().text = lista[i].texto;
        }

        if (panelCorrecto) panelCorrecto.SetActive(false);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);
    }

    public void RespuestaCorrecta(OpcionArrastrable opcion)
    {
        StartCoroutine(FeedbackCorrecta(opcion));
    }

    public void RespuestaIncorrecta(OpcionArrastrable opcion)
    {
        StartCoroutine(FeedbackIncorrecta(opcion));
    }

    IEnumerator FeedbackCorrecta(OpcionArrastrable opcion)
    {
        opcion.ResetearEstado();
        opcion.gameObject.SetActive(false);
        if (panelCorrecto) panelCorrecto.SetActive(true);
        yield return new WaitForSeconds(tiempoFeedback);
        if (panelCorrecto) panelCorrecto.SetActive(false);

        CargarSiguientePreguntaAleatoria();
    }

    IEnumerator FeedbackIncorrecta(OpcionArrastrable opcion)
    {
        if (panelIncorrecto) panelIncorrecto.SetActive(true);
        yield return new WaitForSeconds(tiempoFeedback);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);
        opcion.ResetearEstado();
    }

    void FinDelJuego()
    {
        // Ocultar opciones al terminar
        foreach (var op in opciones)
            op.SetActive(false);

        if (panelFinJuego) panelFinJuego.SetActive(true);
        Debug.Log("Juego terminado");
    }
}