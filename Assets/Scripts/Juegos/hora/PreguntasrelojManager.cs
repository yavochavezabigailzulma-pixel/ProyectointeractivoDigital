using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PreguntasRelojManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI preguntaTexto;

    [Header("Reloj")]
    [SerializeField] RectTransform agujaHoras;
    [SerializeField] RectTransform agujaMinutos;

    [Header("Opciones")]
    [SerializeField] GameObject opcion1;
    [SerializeField] GameObject opcion2;
    [SerializeField] GameObject opcion3;

    [Header("Feedback")]
    public GameObject panelCorrecto;
    public GameObject panelIncorrecto;
    public float tiempoFeedback = 1.5f;

    [Header("Fin del juego")]
    public GameObject panelFinJuego;

    [Header("Preguntas")]
    public DatosPreguntasReloj[] preguntas;

    [Header("Display hora")]
    public TMP_Text displayHora;

    private string respuestaCorrecta;
    private GameObject[] opciones;
    private List<int> indicesRestantes = new List<int>();
    private bool aceptandoRespuesta = true;

    void Start()
    {
        opciones = new GameObject[] { opcion1, opcion2, opcion3 };

        if (panelCorrecto) panelCorrecto.SetActive(false);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);
        if (panelFinJuego) panelFinJuego.SetActive(false);

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

        int pos = Random.Range(0, indicesRestantes.Count);
        int idx = indicesRestantes[pos];
        indicesRestantes.RemoveAt(pos);

        CargarPregunta(idx);
    }

    void CargarPregunta(int idx)
    {
        DatosPreguntasReloj actual = preguntas[idx];

        preguntaTexto.text = actual.pregunta;

        // Aplicar rotaciones de las agujas según la pregunta
        agujaHoras.localRotation = Quaternion.Euler(0f, 0f, actual.rotacionAgujaHoras);
        agujaMinutos.localRotation = Quaternion.Euler(0f, 0f, actual.rotacionAgujaMinutos);

        respuestaCorrecta = actual.opcion1;

        List<string> lista = new List<string> { actual.opcion1, actual.opcion2, actual.opcion3 };

        // Mezclar opciones
        for (int i = 0; i < lista.Count; i++)
        {
            int rand = Random.Range(i, lista.Count);
            (lista[i], lista[rand]) = (lista[rand], lista[i]);
        }

        for (int i = 0; i < opciones.Length; i++)
        {
            opciones[i].SetActive(true);
            opciones[i].GetComponentInChildren<TextMeshProUGUI>().text = lista[i];
        }

        if (panelCorrecto) panelCorrecto.SetActive(false);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);

        aceptandoRespuesta = true;
        if (displayHora) displayHora.text = actual.horaEnNumeros;
    }

    // Este método se llama desde el botón, pasando su propio GameObject
    public void SeleccionarOpcion(GameObject botonPresionado)
    {
        if (!aceptandoRespuesta) return; // evita doble clic durante el feedback

        string respuesta = botonPresionado.GetComponentInChildren<TextMeshProUGUI>().text;

        if (respuesta == respuestaCorrecta)
            StartCoroutine(FeedbackCorrecto());
        else
            StartCoroutine(FeedbackIncorrecto());
    }

    IEnumerator FeedbackCorrecto()
    {
        aceptandoRespuesta = false;
        if (panelCorrecto) panelCorrecto.SetActive(true);
        yield return new WaitForSeconds(tiempoFeedback);
        if (panelCorrecto) panelCorrecto.SetActive(false);

        CargarSiguientePreguntaAleatoria();
    }

    IEnumerator FeedbackIncorrecto()
    {
        aceptandoRespuesta = false;
        if (panelIncorrecto) panelIncorrecto.SetActive(true);
        yield return new WaitForSeconds(tiempoFeedback);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);
        aceptandoRespuesta = true; // permitir reintentar
    }

    void FinDelJuego()
    {
        foreach (var op in opciones)
            op.SetActive(false);

        if (panelFinJuego) panelFinJuego.SetActive(true);
        Debug.Log("Juego terminado");
    }
}