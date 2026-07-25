using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
public class JuegosManager : MonoBehaviour
{
    public GameObject bienvenida;
    public GameObject menu;
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

    void Awake()
    {
        musicasJuegos = new EventReference[]
        {
            musicPerguntados, musicHora, musicSopa, musicOracion
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        // Sonido general del menú de selección, arranca sonando a volumen completo
        if (!musicaGeneral.IsNull)
        {
            musicaGeneralInstance = AudioManager.Instance.CreateLoop(musicaGeneral);
            AudioManager.Instance.SetVolume(musicaGeneralInstance, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopLoop(musicaGeneralInstance);
    }

    public void CerrarBienvenida()
    {
        bienvenida.SetActive(false);
    }
    public void AbrirJuego(int juego)
    {
        menu.SetActive(false);
        switch (juego)
        {
            case 0: perguntadosPanel.SetActive(true); break;
            case 1: horaPanel.SetActive(true); break;
            case 2: sopaPanel.SetActive(true); break;
            case 3: oracionPanel.SetActive(true); break;
        }

        // --- Música ---
        if (juego >= 0 && juego < musicasJuegos.Length)
            AudioManager.Instance.PlayMusicaConFade(musicasJuegos[juego]);

        // El sonido general baja mientras suena el juego
        AudioManager.Instance.FadeTo(musicaGeneralInstance, 0f, duracionFadeSalidaGeneral);
    }
    public void CerrarJuego()
    {
        menu.SetActive(true);
        perguntadosPanel.SetActive(false);
        horaPanel.SetActive(false);
        sopaPanel.SetActive(false);
        oracionPanel.SetActive(false);

        // --- Música ---
        AudioManager.Instance.StopMusicaEstacion();

        // El sonido general vuelve a subir al volver al menú de selección
        AudioManager.Instance.FadeTo(musicaGeneralInstance, 1f, duracionFadeEntradaGeneral);
    }
}