using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OracionManager : MonoBehaviour
{
    [Header("Texto de la oración")]
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

    [Header("Preguntas")]
    public DatosOracion[] preguntas;
    private int indice = 0;

    private string respuestaCorrecta;
    private GameObject[] tarjetas;

    void Start()
    {
        tarjetas = new GameObject[] { tarjeta1, tarjeta2, tarjeta3 };
        CargarPregunta(indice);
    }

    void CargarPregunta(int idx)
    {
        if (idx >= preguntas.Length) return;

        DatosOracion actual = preguntas[idx];

        textoAntes.text = actual.parteAntes;
        textoDespues.text = actual.parteDespues;
        respuestaCorrecta = actual.respuestaCorrecta;

        // Mezclar opciones
        List<string> lista = new List<string>
        {
            actual.respuestaCorrecta,
            actual.opcion2,
            actual.opcion3
        };

        for (int i = 0; i < lista.Count; i++)
        {
            int rand = Random.Range(i, lista.Count);
            (lista[i], lista[rand]) = (lista[rand], lista[i]);
        }

        for (int i = 0; i < tarjetas.Length; i++)
        {
            tarjetas[i].SetActive(true);
            tarjetas[i].GetComponentInChildren<TextMeshProUGUI>().text = lista[i];

            TarjetaOracion tarjeta = tarjetas[i].GetComponent<TarjetaOracion>();
            tarjeta.valorRespuesta = lista[i];
            tarjeta.manager = this;
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
        tarjeta.gameObject.SetActive(false);
        if (panelCorrecto) panelCorrecto.SetActive(true);
        yield return new WaitForSeconds(tiempoFeedback);
        if (panelCorrecto) panelCorrecto.SetActive(false);

        indice++;
        if (indice < preguntas.Length)
            CargarPregunta(indice);
        else
            Debug.Log("Juego terminado");
    }

    IEnumerator FeedbackIncorrecto(TarjetaOracion tarjeta)
    {
        if (panelIncorrecto) panelIncorrecto.SetActive(true);
        yield return new WaitForSeconds(tiempoFeedback);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);
        tarjeta.RestablecerPosicion();
    }
}
