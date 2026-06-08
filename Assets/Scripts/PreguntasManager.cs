using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;


// ?? Script principal ??????????????????????????????????????????
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

    [Header("Preguntas")]
    public DatosPreguntas[] preguntas;
    public int indice = 0;

    private DatosPreguntas preguntaActual;
    private GameObject[] opciones;

    void Start()
    {
        opciones = new GameObject[] { opcion1, opcion2, opcion3 };
        CargarPregunta(indice);
    }

    public void CargarPregunta(int idx)
    {
        if (idx >= preguntas.Length) return;

        preguntaActual = preguntas[idx];
        preguntaTexto.text = preguntaActual.pregunta;

        // Armar lista mezclada: correcta + incorrectas
        List<(string texto, bool correcta)> lista = new List<(string, bool)>
        {
            (preguntaActual.opcion1, true),  // opcion1 en DatosPreguntas es la correcta
            (preguntaActual.opcion2, false),
            (preguntaActual.opcion3, false)
        };

        // Mezclar aleatoriamente
        for (int i = 0; i < lista.Count; i++)
        {
            int rand = Random.Range(i, lista.Count);
            (lista[i], lista[rand]) = (lista[rand], lista[i]);
        }

        // Asignar a cada GameObject
        for (int i = 0; i < opciones.Length; i++)
        {
            opciones[i].SetActive(true);
            opciones[i].GetComponentInChildren<TextMeshProUGUI>().text = lista[i].texto;

            OpcionArrastrable arrastrable = opciones[i].GetComponent<OpcionArrastrable>();
            arrastrable.esCorrecta = lista[i].correcta;
            arrastrable.manager = this;
        }

        // Asegura que los paneles de feedback estén ocultos
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
        opcion.gameObject.SetActive(false);
        if (panelCorrecto) panelCorrecto.SetActive(true);
        yield return new WaitForSeconds(tiempoFeedback);
        if (panelCorrecto) panelCorrecto.SetActive(false);

        // Avanzar a la siguiente pregunta
        indice++;
        if (indice < preguntas.Length)
            CargarPregunta(indice);
        else
            FinDelJuego();
    }

    IEnumerator FeedbackIncorrecta(OpcionArrastrable opcion)
    {
        if (panelIncorrecto) panelIncorrecto.SetActive(true);
        yield return new WaitForSeconds(tiempoFeedback);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);
        opcion.RestablecerPosicion(); // vuelve a su lugar
    }

    void FinDelJuego()
    {
        Debug.Log("Juego terminado");
        // aquí puedes cargar otra escena o mostrar un panel final
    }
}