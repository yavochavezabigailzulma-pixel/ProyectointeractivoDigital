using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JuegosManager : MonoBehaviour
{
    public enum Screen
    {
        Bienvenida,
        MenuPlay,
        SeleccionJuego,   // paso 3 (confirmación del juego elegido)
        SeleccionTema,    // paso 4
        SeleccionNivel,   // paso 5
        Instrucciones,    // paso 6
        Actividad,        // paso 7
        Puntaje,          // paso 8
        Retroalimentacion,// paso 9
        ReintentarVolver  // paso 10
    }

    [Header("Paneles (asignar en el Inspector, en el mismo orden que el enum)")]
    public GameObject bienvenida;
    public GameObject menuPlay;
    public GameObject seleccionJuegoPanel;
    public GameObject seleccionTemaPanel;
    public GameObject seleccionNivelPanel;
    public GameObject instruccionesPanel;
    public GameObject actividadPanel;      // aquí van perguntados/hora/sopa/oracion según el juego
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

    // --- Estado de navegación ---
    private Dictionary<Screen, GameObject> panels;
    private Screen pantallaActual;

    // --- Estado de selección del usuario ---
    private int juegoSeleccionado = -1; // índice: 0=preguntados,1=hora,2=sopa,3=oracion
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
        if (!musicaGeneral.IsNull)
        {
            //musicaGeneralInstance = AudioManager.Instance.CreateLoop(musicaGeneral);
            //AudioManager.Instance.SetVolume(musicaGeneralInstance, 1f);
        }

        Navigate(Screen.Bienvenida);
    }

    void OnDestroy()
    {
        //if (AudioManager.Instance != null)
           // AudioManager.Instance.StopLoop(musicaGeneralInstance);
    }

    // ------------------------------------------------------------
    // NAVEGACIÓN CENTRAL: apaga todo, prende solo la pantalla pedida
    // ------------------------------------------------------------
    public void Navigate(Screen destino)
    {
        foreach (var kvp in panels)
            kvp.Value.SetActive(false);

        panels[destino].SetActive(true);
        pantallaActual = destino;
    }

    // ------------------------------------------------------------
    // PASOS 1-2
    // ------------------------------------------------------------
    public void CerrarBienvenida()
    {
        Navigate(Screen.MenuPlay);
    }

    // ------------------------------------------------------------
    // PASO 3: eligió juego desde el menú Play
    // ------------------------------------------------------------
    public void SeleccionarJuego(int juego)
    {
        juegoSeleccionado = juego;
        Navigate(Screen.SeleccionJuego);
    }

    // ------------------------------------------------------------
    // PASO 4: confirmó el juego -> elige tema
    // ------------------------------------------------------------
    public void ConfirmarJuego()
    {
        // "Hora" = índice 3 → salta selección de tema
        if (juegoSeleccionado == 3)
        {
            Navigate(Screen.SeleccionNivel);
        }
        else
        {
            Navigate(Screen.SeleccionTema);
        }
    }

    public void SeleccionarTema(int tema)
    {
        temaSeleccionado = tema;
        Navigate(Screen.SeleccionNivel);
    }

    // ------------------------------------------------------------
    // PASO 5: elige nivel -> instrucciones
    // ------------------------------------------------------------
    public void SeleccionarNivel(int nivel)
    {
        nivelSeleccionado = nivel;
        Navigate(Screen.Instrucciones);
    }

    // ------------------------------------------------------------
    // PASO 6: entendió instrucciones -> arranca actividad
    // ------------------------------------------------------------
    public void EmpezarActividad()
    {
        AbrirJuego(juegoSeleccionado);
        Navigate(Screen.Actividad);

        int nivel = nivelSeleccionado + 1; // nivelSeleccionado es 0-based (0=Nivel1, 1=Nivel2)

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

        //if (juego >= 0 && juego < musicasJuegos.Length)
        //    AudioManager.Instance.PlayMusicaConFade(musicasJuegos[juego]);

        //AudioManager.Instance.FadeTo(musicaGeneralInstance, 0f, duracionFadeSalidaGeneral);
    }

    // ------------------------------------------------------------
    // PASO 7 -> 8: terminó la actividad, mostrar puntaje
    // Llamar esto desde el script propio de cada minijuego al terminar
    // ------------------------------------------------------------
    public void MostrarPuntaje(int puntaje)
    {
        if (textoPuntaje != null)
            textoPuntaje.text = puntaje.ToString();

        Navigate(Screen.Puntaje);
    }

    // ------------------------------------------------------------
    // PASO 8 -> 9
    // ------------------------------------------------------------
    public void MostrarRetroalimentacion()
    {
        Navigate(Screen.Retroalimentacion);
    }

    // ------------------------------------------------------------
    // PASO 9 -> 10
    // ------------------------------------------------------------
    public void MostrarReintentarVolver()
    {
        Navigate(Screen.ReintentarVolver);
    }

    // ------------------------------------------------------------
    // PASO 10 (a): Reintentar nivel -> vuelve directo a la actividad
    // ------------------------------------------------------------
    public void ReintentarNivel()
    {
        EmpezarActividad(); // reusa el mismo juego/nivel/tema guardados
    }

    // ------------------------------------------------------------
    // PASO 10 (b): Volver al menú Play -> cierra todo, para música del juego
    // ------------------------------------------------------------
    public void VolverAlMenuPlay()
    {
        //AudioManager.Instance.StopMusicaEstacion();
        //AudioManager.Instance.FadeTo(musicaGeneralInstance, 1f, duracionFadeEntradaGeneral);

        juegoSeleccionado = -1;
        nivelSeleccionado = -1;
        temaSeleccionado = -1;

        Navigate(Screen.MenuPlay);
    }
}