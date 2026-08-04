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
    public GameObject panelFinJuego;

    [Header("Preguntas")]
    public DatosOracion[] preguntas;

    [Header("Nivel 2 - Temporizador")]
    public TextMeshProUGUI textoTemporizador;
    public ModoTemporizador modoTemporizador = ModoTemporizador.PorPregunta;
    [Tooltip("Segundos por pregunta. Solo aplica si el modo es 'Por Pregunta'.")]
    public float tiempoPorPregunta = 15f;
    [Tooltip("Segundos totales para todo el intento. Solo aplica si el modo es 'Tiempo Total Intento'.")]
    public float tiempoTotalIntento = 60f;

    private string respuestaCorrecta;
    private GameObject[] tarjetas;
    private List<int> indicesRestantes = new List<int>();

    private int nivelActual = 1;
    private bool aceptandoRespuesta = true;

    private Coroutine temporizadorPreguntaCoroutine;
    private Coroutine temporizadorTotalCoroutine;
    private float tiempoRestante;

    [Header("Puntaje")]
    private int respuestasCorrectas = 0;
    private int totalPreguntas = 0;

    private int totalPreguntasDelTema;

    private TemaPregunta temaActual;
    void Awake()
    {
        tarjetas = new GameObject[] { tarjeta1, tarjeta2, tarjeta3 };
    }

    public void ReiniciarJuego(int nivel, TemaPregunta tema)
    {
        StopAllCoroutines();
        temporizadorPreguntaCoroutine = null;
        temporizadorTotalCoroutine = null;

        nivelActual = nivel;
        temaActual = tema;

        foreach (var t in tarjetas)
            t.SetActive(true);

        if (panelCorrecto) panelCorrecto.SetActive(false);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);
        if (panelFinJuego) panelFinJuego.SetActive(false);

        bool usaTemporizador = nivelActual == 2;
        if (textoTemporizador)
            textoTemporizador.gameObject.SetActive(usaTemporizador);

        respuestasCorrectas = 0;
        aceptandoRespuesta = true;

        if (usaTemporizador && modoTemporizador == ModoTemporizador.TiempoTotalIntento)
            temporizadorTotalCoroutine = StartCoroutine(TemporizadorTotal(tiempoTotalIntento));

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
        {
            if (preguntas[i].tema == temaActual)
                indicesRestantes.Add(i);
        }

        totalPreguntasDelTema = indicesRestantes.Count;

        if (totalPreguntasDelTema == 0)
            Debug.LogWarning($"[Oracion] No hay preguntas cargadas para el tema {temaActual}");
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

        for (int i = 0; i < lista.Count; i++)
        {
            int rand = Random.Range(i, lista.Count);
            (lista[i], lista[rand]) = (lista[rand], lista[i]);
        }

        for (int i = 0; i < tarjetas.Length; i++)
        {
            tarjetas[i].SetActive(true);

            TarjetaOracion tarjeta = tarjetas[i].GetComponent<TarjetaOracion>();
            tarjeta.ResetearEstado();
            tarjeta.valorRespuesta = lista[i];
            tarjeta.manager = this;

            tarjetas[i].GetComponentInChildren<TextMeshProUGUI>().text = lista[i];
        }

        if (panelCorrecto) panelCorrecto.SetActive(false);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);

        aceptandoRespuesta = true;

        if (nivelActual == 2 && modoTemporizador == ModoTemporizador.PorPregunta)
            IniciarTemporizadorPorPregunta();
    }

    void IniciarTemporizadorPorPregunta()
    {
        if (temporizadorPreguntaCoroutine != null)
            StopCoroutine(temporizadorPreguntaCoroutine);

        if (tiempoPorPregunta > 0)
            temporizadorPreguntaCoroutine = StartCoroutine(TemporizadorPregunta(tiempoPorPregunta));
    }

    IEnumerator TemporizadorPregunta(float tiempoTotal)
    {
        tiempoRestante = tiempoTotal;
        ActualizarTextoTemporizador();

        while (tiempoRestante > 0f)
        {
            yield return null;
            tiempoRestante -= Time.deltaTime;
            ActualizarTextoTemporizador();
        }

        tiempoRestante = 0f;
        ActualizarTextoTemporizador();
        TiempoAgotadoPregunta();
    }

    IEnumerator TemporizadorTotal(float tiempoTotal)
    {
        tiempoRestante = tiempoTotal;
        ActualizarTextoTemporizador();

        while (tiempoRestante > 0f)
        {
            yield return null;
            tiempoRestante -= Time.deltaTime;
            ActualizarTextoTemporizador();
        }

        tiempoRestante = 0f;
        ActualizarTextoTemporizador();
        TiempoAgotadoTotal();
    }

    void ActualizarTextoTemporizador()
    {
        if (!textoTemporizador) return;

        int segundosTotales = Mathf.CeilToInt(tiempoRestante);
        int minutos = segundosTotales / 60;
        int segundos = segundosTotales % 60;

        textoTemporizador.text = $"{minutos:00}:{segundos:00}";
    }

    void TiempoAgotadoPregunta()
    {
        if (!aceptandoRespuesta) return;
        aceptandoRespuesta = false;
        StartCoroutine(FeedbackTiempoAgotado());
    }

    IEnumerator FeedbackTiempoAgotado()
    {
        foreach (var t in tarjetas)
        {
            TarjetaOracion tarjeta = t.GetComponent<TarjetaOracion>();
            tarjeta.ResetearEstado();
            t.SetActive(false);
        }

        if (panelIncorrecto) panelIncorrecto.SetActive(true);
        yield return new WaitForSeconds(tiempoFeedback);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);

        CargarSiguientePreguntaAleatoria();
    }

    void TiempoAgotadoTotal()
    {
        aceptandoRespuesta = false;

        if (temporizadorPreguntaCoroutine != null)
            StopCoroutine(temporizadorPreguntaCoroutine);

        FinDelJuego();
    }

    public void VerificarRespuesta(TarjetaOracion tarjeta)
    {
        if (!aceptandoRespuesta) return;

        aceptandoRespuesta = false;

        if (tarjeta.valorRespuesta == respuestaCorrecta)
            StartCoroutine(FeedbackCorrecto(tarjeta));
        else
            StartCoroutine(FeedbackIncorrecto(tarjeta));
    }

    IEnumerator FeedbackCorrecto(TarjetaOracion tarjeta)
    {
        if (temporizadorPreguntaCoroutine != null)
            StopCoroutine(temporizadorPreguntaCoroutine);

        tarjeta.ResetearEstado();
        tarjeta.gameObject.SetActive(false);
        if (panelCorrecto) panelCorrecto.SetActive(true);

        respuestasCorrectas++;

        yield return new WaitForSeconds(tiempoFeedback);
        if (panelCorrecto) panelCorrecto.SetActive(false);

        CargarSiguientePreguntaAleatoria();
    }

    IEnumerator FeedbackIncorrecto(TarjetaOracion tarjeta)
    {
        if (temporizadorPreguntaCoroutine != null)
            StopCoroutine(temporizadorPreguntaCoroutine);

        foreach (var t in tarjetas)
        {
            TarjetaOracion tar = t.GetComponent<TarjetaOracion>();
            tar.ResetearEstado();
            t.SetActive(false);
        }

        if (panelIncorrecto) panelIncorrecto.SetActive(true);
        yield return new WaitForSeconds(tiempoFeedback);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);

        CargarSiguientePreguntaAleatoria();
    }

    void FinDelJuego()
    {
        if (temporizadorPreguntaCoroutine != null)
            StopCoroutine(temporizadorPreguntaCoroutine);
        if (temporizadorTotalCoroutine != null)
            StopCoroutine(temporizadorTotalCoroutine);

        if (textoTemporizador)
            textoTemporizador.gameObject.SetActive(false);

        foreach (var t in tarjetas)
            t.SetActive(false);

        int puntaje = totalPreguntasDelTema > 0
            ? Mathf.RoundToInt((float)respuestasCorrectas / totalPreguntasDelTema * 100)
            : 0;

        if (JuegosManager.Instance != null)
            JuegosManager.Instance.MostrarPuntaje(puntaje);
    }

    public void DetenerJuego()
    {
        StopAllCoroutines();
    }
}