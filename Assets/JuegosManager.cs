using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SpritesTemaPorJuego
{
    public Sprite botonUniverso;
    public Sprite botonTierra;
    public Sprite botonMundo;
}

[System.Serializable]
public class SpritesNivelPorJuego
{
    public Sprite tituloSprite;
    public Sprite botonNivel1;
    public Sprite botonNivel2;
}

public class JuegosManager : MonoBehaviour
{
    public enum Screen
    {
        Bienvenida,
        MenuPlay,
        SeleccionJuego,
        SeleccionTema,
        SeleccionNivel,
        Instrucciones,
        Actividad,
        Puntaje,
        Retroalimentacion,
        ReintentarVolver
    }

    [Header("Paneles")]
    public GameObject bienvenida;
    public GameObject menuPlay;
    public GameObject seleccionJuegoPanel;
    public GameObject seleccionTemaPanel;
    public GameObject seleccionNivelPanel;
    public GameObject instruccionesPanel;
    public GameObject actividadPanel;
    public GameObject puntajePanel;
    public GameObject retroalimentacionPanel;
    public GameObject reintentarVolverPanel;

    [Header("Sub-paneles de actividad por juego")]
    public GameObject perguntadosPanel;
    public GameObject horaPanel;
    public GameObject sopaPanel;
    public GameObject oracionPanel;

    [Header("Sonidos por juego")]
    public EventReference musicPerguntados;
    public EventReference musicHora;
    public EventReference musicSopa;
    public EventReference musicOracion;
    private EventReference[] musicasJuegos;

    [Header("Sonido general (menú de selección de juegos)")]
    public EventReference musicaGeneral;
    private EventInstance musicaGeneralInstance;
    [SerializeField] private float duracionFadeEntradaGeneral = 0.5f;
    [SerializeField] private float duracionFadeSalidaGeneral = 0.5f;

    private Dictionary<Screen, GameObject> panels;
    private Screen pantallaActual;
    private Stack<Screen> historial = new Stack<Screen>();

    private int juegoSeleccionado = -1;
    private int nivelSeleccionado = -1;
    private int temaSeleccionado = -1;

    [Header("UI Puntaje")]
    public TextMeshProUGUI textoPuntaje;

    [Header("Referencias a scripts de actividad")]
    public PreguntasManager preguntasManager;
    public SopaLetrasManager sopaManager;
    public OracionManager oracionManager;
    public PreguntasRelojManager relojManager;

    public static JuegosManager Instance { get; private set; }

    [Header("Seleccion Juego - Fondos y Botón Play por juego")]
    public Image seleccionJuegoBackground;
    public Image btnPlaySeleccionJuego;
    public RectTransform btnPlayRectTransform;

    [Tooltip("Orden: 0-Preguntados, 1-Sopa, 2-Oracion, 3-Hora")]
    public Sprite[] fondosPorJuego = new Sprite[4];

    [Tooltip("Orden: 0-Preguntados, 1-Sopa, 2-Oracion, 3-Hora")]
    public Sprite[] spritesBotonPlayPorJuego = new Sprite[4];

    [Tooltip("Orden: 0-Preguntados, 1-Sopa, 2-Oracion, 3-Hora")]
    public Vector2[] posicionesBotonPlayPorJuego = new Vector2[4];

    [Header("Seleccion Tema - Fondo y Botones por juego")]
    public Image seleccionTemaBackground;
    public Image btnTemaUniverso;
    public Image btnTemaTierra;
    public Image btnTemaMundo;

    [Tooltip("Orden: 0-Preguntados, 1-Sopa, 2-Oracion. (Hora no llega a este panel)")]
    public Sprite[] fondosTemaPorJuego = new Sprite[4];

    [Tooltip("Orden: 0-Preguntados, 1-Sopa, 2-Oracion")]
    public SpritesTemaPorJuego[] botonesTemaPorJuego = new SpritesTemaPorJuego[4];

    [Header("Seleccion Nivel - Fondo, Título y Botones por juego")]
    public Image seleccionNivelBackground;
    public Image tituloSeleccionNivel;
    public Image btnNivel1;
    public Image btnNivel2;

    [Tooltip("Orden: 0-Preguntados, 1-Sopa, 2-Oracion, 3-Hora")]
    public Sprite[] fondosNivelPorJuego = new Sprite[4];

    [Tooltip("Orden: 0-Preguntados, 1-Sopa, 2-Oracion, 3-Hora")]
    public SpritesNivelPorJuego[] spritesNivelPorJuego = new SpritesNivelPorJuego[4];

    void Awake()
    {
        Instance = this;

        musicasJuegos = new[] { musicPerguntados, musicSopa, musicOracion, musicHora };

        panels = new Dictionary<Screen, GameObject>
        {
            { Screen.Bienvenida,          bienvenida },
            { Screen.MenuPlay,            menuPlay },
            { Screen.SeleccionJuego,      seleccionJuegoPanel },
            { Screen.SeleccionTema,       seleccionTemaPanel },
            { Screen.SeleccionNivel,      seleccionNivelPanel },
            { Screen.Instrucciones,       instruccionesPanel },
            { Screen.Actividad,           actividadPanel },
            { Screen.Puntaje,             puntajePanel },
            { Screen.Retroalimentacion,   retroalimentacionPanel },
            { Screen.ReintentarVolver,    reintentarVolverPanel },
        };
    }

    void Start()
    {
        Navigate(Screen.Bienvenida);
    }

    public void Navigate(Screen destino)
    {
        foreach (var kvp in panels)
            kvp.Value.SetActive(false);

        panels[destino].SetActive(true);
        pantallaActual = destino;
    }

    public void Volver()
    {
        if (historial.Count == 0) return;

        Screen anterior = historial.Pop();
        Navigate(anterior);
    }

    public void CerrarBienvenida()
    {
        historial.Push(Screen.Bienvenida);
        Navigate(Screen.MenuPlay);
    }

    public void SeleccionarJuego(int juego)
    {
        juegoSeleccionado = juego;
        AplicarEstiloSeleccionJuego(juego);

        historial.Push(Screen.MenuPlay);
        Navigate(Screen.SeleccionJuego);
    }

    private void AplicarEstiloSeleccionJuego(int juego)
    {
        if (juego < 0) return;

        if (seleccionJuegoBackground != null &&
            juego < fondosPorJuego.Length &&
            fondosPorJuego[juego] != null)
        {
            seleccionJuegoBackground.sprite = fondosPorJuego[juego];
        }

        if (btnPlaySeleccionJuego != null &&
            juego < spritesBotonPlayPorJuego.Length &&
            spritesBotonPlayPorJuego[juego] != null)
        {
            btnPlaySeleccionJuego.sprite = spritesBotonPlayPorJuego[juego];
        }

        if (btnPlayRectTransform != null &&
            juego < posicionesBotonPlayPorJuego.Length)
        {
            btnPlayRectTransform.anchoredPosition = posicionesBotonPlayPorJuego[juego];
        }
    }

    public void ConfirmarJuego()
    {
        if (juegoSeleccionado == 3)
        {
            AplicarEstiloSeleccionNivel(juegoSeleccionado);
            historial.Push(Screen.SeleccionJuego);
            Navigate(Screen.SeleccionNivel);
            return;
        }

        AplicarEstiloSeleccionTema(juegoSeleccionado);
        historial.Push(Screen.SeleccionJuego);
        Navigate(Screen.SeleccionTema);
    }

    public void SeleccionarTema(int tema)
    {
        temaSeleccionado = tema;
        AplicarEstiloSeleccionNivel(juegoSeleccionado);

        historial.Push(Screen.SeleccionTema);
        Navigate(Screen.SeleccionNivel);
    }

    private void AplicarEstiloSeleccionTema(int juego)
    {
        if (juego < 0 || juego >= fondosTemaPorJuego.Length) return;

        if (seleccionTemaBackground != null && fondosTemaPorJuego[juego] != null)
            seleccionTemaBackground.sprite = fondosTemaPorJuego[juego];

        if (juego < botonesTemaPorJuego.Length && botonesTemaPorJuego[juego] != null)
        {
            var sprites = botonesTemaPorJuego[juego];

            if (btnTemaUniverso != null && sprites.botonUniverso != null)
                btnTemaUniverso.sprite = sprites.botonUniverso;

            if (btnTemaTierra != null && sprites.botonTierra != null)
                btnTemaTierra.sprite = sprites.botonTierra;

            if (btnTemaMundo != null && sprites.botonMundo != null)
                btnTemaMundo.sprite = sprites.botonMundo;
        }
    }

    private void AplicarEstiloSeleccionNivel(int juego)
    {
        if (juego < 0 || juego >= fondosNivelPorJuego.Length) return;

        if (seleccionNivelBackground != null && fondosNivelPorJuego[juego] != null)
            seleccionNivelBackground.sprite = fondosNivelPorJuego[juego];

        if (juego < spritesNivelPorJuego.Length && spritesNivelPorJuego[juego] != null)
        {
            var sprites = spritesNivelPorJuego[juego];

            if (tituloSeleccionNivel != null && sprites.tituloSprite != null)
                tituloSeleccionNivel.sprite = sprites.tituloSprite;

            if (btnNivel1 != null && sprites.botonNivel1 != null)
                btnNivel1.sprite = sprites.botonNivel1;

            if (btnNivel2 != null && sprites.botonNivel2 != null)
                btnNivel2.sprite = sprites.botonNivel2;
        }
    }

    public void SeleccionarNivel(int nivel)
    {
        nivelSeleccionado = nivel;

        historial.Push(Screen.SeleccionNivel);
        Navigate(Screen.Instrucciones);
    }

    public void EmpezarActividad()
    {
        AbrirJuego(juegoSeleccionado);
        Navigate(Screen.Actividad);

        int nivel = nivelSeleccionado + 1;

        switch (juegoSeleccionado)
        {
            case 0:
                if (preguntasManager != null) preguntasManager.ReiniciarJuego(nivel);
                break;
            case 1:
                if (sopaManager != null) sopaManager.ReiniciarJuego(nivel);
                break;
            case 2:
                if (oracionManager != null) oracionManager.ReiniciarJuego(nivel);
                break;
            case 3:
                if (relojManager != null) relojManager.ReiniciarJuego(nivel);
                break;
        }
    }

    private void AbrirJuego(int juego)
    {
        perguntadosPanel.SetActive(false);
        horaPanel.SetActive(false);
        sopaPanel.SetActive(false);
        oracionPanel.SetActive(false);

        switch (juego)
        {
            case 0: perguntadosPanel.SetActive(true); break;
            case 1: sopaPanel.SetActive(true); break;
            case 2: oracionPanel.SetActive(true); break;
            case 3: horaPanel.SetActive(true); break;
        }
    }

    public void MostrarPuntaje(int puntaje)
    {
        if (textoPuntaje != null)
            textoPuntaje.text = puntaje.ToString();

        Navigate(Screen.Puntaje);
    }

    public void MostrarRetroalimentacion()
    {
        Navigate(Screen.Retroalimentacion);
    }

    public void MostrarReintentarVolver()
    {
        Navigate(Screen.ReintentarVolver);
    }

    public void ReintentarNivel()
    {
        EmpezarActividad();
    }

    public void VolverAlMenuPlay()
    {
        juegoSeleccionado = -1;
        nivelSeleccionado = -1;
        temaSeleccionado = -1;

        historial.Clear();

        Navigate(Screen.MenuPlay);
    }
}