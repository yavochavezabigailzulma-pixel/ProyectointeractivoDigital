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

    [Header("Display hora (solo Nivel 1)")]
    public TMP_Text displayHora;

    [Header("Fuentes según tipo de opción")]
    public TMP_FontAsset fuenteTexto;      // usada en Nivel 1 (ej: "Dos en punto")
    public TMP_FontAsset fuenteNumerica;   // usada en Nivel 2 (ej: "2:00")

    [Header("Tamaño de fuente según tipo de opción")]
    public float tamanoFuenteTexto = 32f;
    public float tamanoFuenteNumerica = 48f;

    private string respuestaCorrecta;
    private GameObject[] opciones;
    private List<int> indicesRestantes = new List<int>();
    private bool aceptandoRespuesta = true;
    private int nivelActual = 1;

    [Header("Puntaje")]
    private int respuestasCorrectas = 0;
    private int totalPreguntas = 0;

    void Awake()
    {
        opciones = new GameObject[] { opcion1, opcion2, opcion3 };
    }

    // Llamado explícitamente por JuegosManager
    public void ReiniciarJuego(int nivel)
    {
        StopAllCoroutines();

        nivelActual = nivel;

        foreach (var op in opciones)
            op.SetActive(true);

        if (panelCorrecto) panelCorrecto.SetActive(false);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);
        if (panelFinJuego) panelFinJuego.SetActive(false);

        // El recuadro numérico solo se muestra en Nivel 1
        if (displayHora)
            displayHora.gameObject.SetActive(nivelActual == 1);

        respuestasCorrectas = 0;
        aceptandoRespuesta = true;

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

        agujaHoras.localRotation = Quaternion.Euler(0f, 0f, actual.rotacionAgujaHoras);
        agujaMinutos.localRotation = Quaternion.Euler(0f, 0f, actual.rotacionAgujaMinutos);

        bool mostrarNumerico = nivelActual == 2;

        List<(string texto, string numerica, bool correcta)> lista = new List<(string, string, bool)>
    {
        (actual.opcion1, actual.opcion1Numerica, true),
        (actual.opcion2, actual.opcion2Numerica, false),
        (actual.opcion3, actual.opcion3Numerica, false)
    };

        for (int i = 0; i < lista.Count; i++)
        {
            int rand = Random.Range(i, lista.Count);
            (lista[i], lista[rand]) = (lista[rand], lista[i]);
        }

        for (int i = 0; i < opciones.Length; i++)
        {
            opciones[i].SetActive(true);

            string valorMostrado = mostrarNumerico ? lista[i].numerica : lista[i].texto;

            TextMeshProUGUI tmp = opciones[i].GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = valorMostrado;
            tmp.font = mostrarNumerico ? fuenteNumerica : fuenteTexto;
            tmp.fontSize = mostrarNumerico ? tamanoFuenteNumerica : tamanoFuenteTexto;

            if (lista[i].correcta)
                respuestaCorrecta = valorMostrado;
        }

        if (panelCorrecto) panelCorrecto.SetActive(false);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);

        aceptandoRespuesta = true;

        if (displayHora && nivelActual == 1)
            displayHora.text = actual.horaEnNumeros;
    }

    public void SeleccionarOpcion(GameObject botonPresionado)
    {
        if (!aceptandoRespuesta) return;

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

        respuestasCorrectas++;

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
        aceptandoRespuesta = true;
    }

    void FinDelJuego()
    {
        foreach (var op in opciones)
            op.SetActive(false);

        totalPreguntas = preguntas.Length;
        int puntaje = totalPreguntas > 0
            ? Mathf.RoundToInt((float)respuestasCorrectas / totalPreguntas * 100)
            : 0;

        if (JuegosManager.Instance != null)
            JuegosManager.Instance.MostrarPuntaje(puntaje);
    }
}