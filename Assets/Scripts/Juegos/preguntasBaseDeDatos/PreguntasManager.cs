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
    public GameObject panelFinJuego;

    [Header("Preguntas")]
    public DatosPreguntas[] preguntas;

    [Header("Nivel 2 - Temporizador")]
    public TextMeshProUGUI textoTemporizador;
    public ModoTemporizador modoTemporizador = ModoTemporizador.PorPregunta;
    [Tooltip("Segundos por pregunta. Solo aplica si el modo es 'Por Pregunta'.")]
    public float tiempoPorPregunta = 15f;
    [Tooltip("Segundos totales para todo el intento. Solo aplica si el modo es 'Tiempo Total Intento'.")]
    public float tiempoTotalIntento = 60f;

    private DatosPreguntas preguntaActual;
    private GameObject[] opciones;
    private List<int> indicesRestantes = new List<int>();

    private int nivelActual = 1;
    private bool aceptandoRespuesta = true;

    // Temporizador por pregunta (se reinicia en cada pregunta)
    private Coroutine temporizadorPreguntaCoroutine;
    // Temporizador total (arranca una vez por partida, corre de fondo)
    private Coroutine temporizadorTotalCoroutine;
    private float tiempoRestante;

    [Header("Puntaje")]
    private int preguntasCorrectas = 0;
    private int totalPreguntas = 0;

    private TemaPregunta temaActual;

    private int totalPreguntasDelTema;
    void Awake()
    {
        opciones = new GameObject[] { opcion1, opcion2, opcion3 };
    }

    // Llamado explícitamente por JuegosManager. nivel: 1 = sin temporizador, 2 = con temporizador
    public void ReiniciarJuego(int nivel, TemaPregunta tema)
    {
        StopAllCoroutines();
        temporizadorPreguntaCoroutine = null;
        temporizadorTotalCoroutine = null;

        nivelActual = nivel;
        temaActual = tema;

        foreach (var op in opciones)
            op.SetActive(true);

        if (panelCorrecto) panelCorrecto.SetActive(false);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);
        if (panelFinJuego) panelFinJuego.SetActive(false);

        bool usaTemporizador = nivelActual == 2;
        if (textoTemporizador)
            textoTemporizador.gameObject.SetActive(usaTemporizador);

        preguntasCorrectas = 0;
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
            Debug.LogWarning($"[Preguntados] No hay preguntas cargadas para el tema {temaActual}");
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

        aceptandoRespuesta = true;

        // Solo el modo "por pregunta" reinicia el contador en cada pregunta.
        // El modo "tiempo total" sigue corriendo de fondo sin tocarse acá.
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

    // Se acabó el tiempo de ESTA pregunta (modo por pregunta) → cuenta como fallo, avanza
    void TiempoAgotadoPregunta()
    {
        if (!aceptandoRespuesta) return;
        aceptandoRespuesta = false;
        StartCoroutine(FeedbackTiempoAgotado());
    }

    IEnumerator FeedbackTiempoAgotado()
    {
        foreach (var op in opciones)
        {
            OpcionArrastrable arrastrable = op.GetComponent<OpcionArrastrable>();
            arrastrable.ResetearEstado();
            op.SetActive(false);
        }

        if (panelIncorrecto) panelIncorrecto.SetActive(true);
        yield return new WaitForSeconds(tiempoFeedback);
        if (panelIncorrecto) panelIncorrecto.SetActive(false);

        CargarSiguientePreguntaAleatoria();
    }

    // Se acabó el tiempo TOTAL del intento → corta el juego ahí mismo, con el puntaje acumulado hasta el momento
    void TiempoAgotadoTotal()
    {
        aceptandoRespuesta = false;

        if (temporizadorPreguntaCoroutine != null)
            StopCoroutine(temporizadorPreguntaCoroutine);

        StopCoroutine(nameof(IniciarAlFrame)); // por si quedara pendiente, no debería pero por seguridad

        FinDelJuego();
    }

    public void RespuestaCorrecta(OpcionArrastrable opcion)
    {
        if (!aceptandoRespuesta) return;
        aceptandoRespuesta = false;
        StartCoroutine(FeedbackCorrecta(opcion));
    }

    public void RespuestaIncorrecta(OpcionArrastrable opcion)
    {
        if (!aceptandoRespuesta) return;
        aceptandoRespuesta = false;
        StartCoroutine(FeedbackIncorrecta(opcion));
    }
    IEnumerator FeedbackCorrecta(OpcionArrastrable opcion)
    {
        // Solo detiene el temporizador de PREGUNTA. El de tiempo TOTAL sigue corriendo.
        if (temporizadorPreguntaCoroutine != null)
            StopCoroutine(temporizadorPreguntaCoroutine);

        opcion.ResetearEstado();
        opcion.gameObject.SetActive(false);
        if (panelCorrecto) panelCorrecto.SetActive(true);

        preguntasCorrectas++;

        yield return new WaitForSeconds(tiempoFeedback);
        if (panelCorrecto) panelCorrecto.SetActive(false);

        CargarSiguientePreguntaAleatoria();
    }

    IEnumerator FeedbackIncorrecta(OpcionArrastrable opcion)
    {
        if (temporizadorPreguntaCoroutine != null)
            StopCoroutine(temporizadorPreguntaCoroutine);

        foreach (var op in opciones)
        {
            OpcionArrastrable arrastrable = op.GetComponent<OpcionArrastrable>();
            arrastrable.ResetearEstado();
            op.SetActive(false);
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

        foreach (var op in opciones)
            op.SetActive(false);

        int puntaje = totalPreguntasDelTema > 0
            ? Mathf.RoundToInt((float)preguntasCorrectas / totalPreguntasDelTema * 100)
            : 0;

        if (JuegosManager.Instance != null)
            JuegosManager.Instance.MostrarPuntaje(puntaje);
    }

    public void DetenerJuego()
    {
        StopAllCoroutines();
    }
}