using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PreguntasRelojManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI preguntaTexto;
    [SerializeField] Image imagenReloj;

    [Header("Opciones")]
    [SerializeField] GameObject opcion1;
    [SerializeField] GameObject opcion2;
    [SerializeField] GameObject opcion3;

    [Header("Feedback")]
    public GameObject panelCorrecto;
    public GameObject panelIncorrecto;
    public float tiempoFeedback = 1.5f;

    [Header("Preguntas")]
    public DatosPreguntasReloj[] preguntas;
    private int indice = 0;

    private string respuestaCorrecta;
    private GameObject[] opciones;

    void Start()
    {
        opciones = new GameObject[] { opcion1, opcion2, opcion3 };
        CargarPregunta(indice);
    }

    void CargarPregunta(int idx)
    {
        if (idx >= preguntas.Length) return;

        DatosPreguntasReloj  actual = preguntas[idx];

        preguntaTexto.text = actual.pregunta;
        imagenReloj.sprite = actual.imagenReloj;

        // Guardar cuál es la correcta
        respuestaCorrecta = actual.opcion1;

        // Mezclar opciones
        List<string> lista = new List<string> { actual.opcion1, actual.opcion2, actual.opcion3 };
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
    }

    // Este método se llama desde el botón, pasando su propio texto
    public void SeleccionarOpcion(GameObject botonPresionado)
    {
        string respuesta = botonPresionado.GetComponentInChildren<TextMeshProUGUI>().text;

        if (respuesta == respuestaCorrecta)
            StartCoroutine(FeedbackCorrecto());
        else
            StartCoroutine(FeedbackIncorrecto());
    }

    IEnumerator FeedbackCorrecto()
    {
        if (panelCorrecto) panelCorrecto.SetActive(true);
        yield return new WaitForSeconds(tiempoFeedback);
        if (panelCorrecto) panelCorrecto.SetActive(false);

        indice++;
        if (indice < preguntas.Length)
            CargarPregunta(indice);
        else
            FinDelJuego();
    }

    IEnumerator FeedbackIncorrecto()
    {
        if (panelIncorrecto) panelIncorrecto.SetActive(true);
        yield return new WaitForSeconds(tiempoFeedback);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);
    }

    void FinDelJuego()
    {
        Debug.Log("Juego terminado");
    }
}