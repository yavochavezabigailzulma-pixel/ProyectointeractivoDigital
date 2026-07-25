using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMOD;
using FMODUnity;

[System.Serializable]
public class ElementoEstacion
{
    public Sprite sprite;
    public Vector2 posicion;
    public Vector2 tamaño;
}

public class EstacionesController : MonoBehaviour
{
    //[Header("Elementos decorativos por estación")]
    //public Image elementoDecorativo;
    //public ElementoEstacion[] elementos;

    [Header("Sonidos")]
    public EventReference musicPrimavera;
    public EventReference musicVerano;
    public EventReference musicOtono;
    public EventReference musicInvierno;

    private EventReference[] musicasEstaciones;

    [Header("Sonido general (menú de selección de estaciones)")]
    public EventReference musicaGeneral;
    private FMOD.Studio.EventInstance musicaGeneralInstance;
    [SerializeField] private float duracionFadeEntradaGeneral = 0.5f;
    [SerializeField] private float duracionFadeSalidaGeneral = 0.5f;

    [Header("Objetos de fondo por estación")]
    // En vez de un solo "fondo" con sprites, un objeto específico por estación
    public GameObject[] objetosFondo;
    public GameObject panelFondo;
    public GameObject panelTransparente;

    [Header("Panel información")]
    public Sprite[] fondosInfo;

    public GameObject panelInfo1;
    public GameObject panelInfo2;
    public GameObject panelInfo3;

    public TMP_Text textoInfo1;
    public TMP_Text textoInfo2;
    public TMP_Text textoInfo3;

    public Animator animatorInfo1;
    public Animator animatorInfo2;
    public Animator animatorInfo3;

    [Header("Botones panel 1")]
    public GameObject botonAbrir1;
    public GameObject botonCerrar1;

    [Header("Botones panel 2")]
    public GameObject botonAbrir2;
    public GameObject botonCerrar2;

    [Header("Botones panel 3")]
    public GameObject botonAbrir3;
    public GameObject botonCerrar3;

    private int estacionActual = -1;

    void Awake()
    {
        musicasEstaciones = new EventReference[]
        {
            musicPrimavera, musicVerano, musicOtono, musicInvierno
        };
    }

    private void Start()
    {
        // Sonido general del menú de selección, arranca sonando a volumen completo
        if (!musicaGeneral.IsNull)
        {
            musicaGeneralInstance = AudioManager.Instance.CreateLoop(musicaGeneral);
            AudioManager.Instance.SetVolume(musicaGeneralInstance, 1f);
        }
    }

    void OnDestroy()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopLoop(musicaGeneralInstance);
    }

    public void abrirFondo(int estacion)
    {
        if (estacion < 0 || estacion >= objetosFondo.Length)
            return;

        estacionActual = estacion;

        // Desactiva todos los objetos de fondo y activa solo el de la estación elegida
        for (int i = 0; i < objetosFondo.Length; i++)
        {
            if (objetosFondo[i] != null)
                objetosFondo[i].SetActive(i == estacion);
        }

        panelFondo.SetActive(false);
        panelTransparente.SetActive(true);

        //if (estacion < elementos.Length && elementos[estacion] != null)
        //{
        //    elementoDecorativo.sprite = elementos[estacion].sprite;
        //    elementoDecorativo.rectTransform.anchoredPosition = elementos[estacion].posicion;
        //    elementoDecorativo.rectTransform.sizeDelta = elementos[estacion].tamaño;
        //    elementoDecorativo.gameObject.SetActive(true);
        //}

        panelInfo1.SetActive(true);
        panelInfo2.SetActive(true);
        panelInfo3.SetActive(true);

        botonAbrir1.SetActive(true);
        botonAbrir2.SetActive(true);
        botonAbrir3.SetActive(true);

        // --- Música ---
        if (estacion < musicasEstaciones.Length)
            AudioManager.Instance.PlayMusicaConFade(musicasEstaciones[estacion]);

        // El sonido general baja mientras suena la estación
        AudioManager.Instance.FadeTo(musicaGeneralInstance, 0f, duracionFadeSalidaGeneral);
    }

    public void cerrarFondo()
    {
        panelFondo.SetActive(true);
        panelTransparente.SetActive(false);

        // Desactiva todos los objetos de fondo al cerrar
        for (int i = 0; i < objetosFondo.Length; i++)
        {
            if (objetosFondo[i] != null)
                objetosFondo[i].SetActive(false);
        }

        //elementoDecorativo.gameObject.SetActive(false);

        panelInfo1.SetActive(false);
        panelInfo2.SetActive(false);
        panelInfo3.SetActive(false);

        botonAbrir1.SetActive(false);
        botonAbrir2.SetActive(false);
        botonAbrir3.SetActive(false);
        botonCerrar1.SetActive(false);
        botonCerrar2.SetActive(false);
        botonCerrar3.SetActive(false);

        // --- Música ---
        AudioManager.Instance.StopMusicaEstacion();

        // El sonido general vuelve a subir al volver al menú de selección
        AudioManager.Instance.FadeTo(musicaGeneralInstance, 1f, duracionFadeEntradaGeneral);
    }

    private void SetTextos()
    {
        switch (estacionActual)
        {
            case 0: // PRIMAVERA
                textoInfo1.text =
                    "<b>¿Cuándo ocurre en nuestro hemisferio?</b>\n" +
                    "De septiembre a diciembre\n" +
                    "<b>Inicio: Equinoccio de primavera - 21 de septiembre</b>\n" +
                    "• Día y noche duran casi lo mismo";

                textoInfo2.text =
                    "<b>¿Cómo es el clima?</b>\n" +
                    "• Temperatura agradable\n" +
                    "• Empieza a hacer más calor";

                textoInfo3.text =
                    "<b>¿Qué pasa en la naturaleza?</b>\n" +
                    "• Florecen las plantas\n" +
                    "• Todo se vuelve verde y colorido";
                break;

            case 1: // VERANO
                textoInfo1.text =
                    "<b>¿Cuándo ocurre en nuestro hemisferio?</b>\n" +
                    "De diciembre a marzo\n" +
                    "<b>Inicio: Solsticio de verano - 21 de diciembre</b>\n" +
                    "• Día más largo del año";

                textoInfo2.text =
                    "<b>¿Cómo es el clima?</b>\n" +
                    "• Mucho calor\n" +
                    "• Época de lluvias";

                textoInfo3.text =
                    "<b>¿Qué pasa en la naturaleza?</b>\n" +
                    "• Las plantas crecen rápido\n" +
                    "• Hay tormentas frecuentes";
                break;

            case 2: // OTOÑO
                textoInfo1.text =
                    "<b>¿Cuándo ocurre en nuestro hemisferio?</b>\n" +
                    "De marzo a junio\n" +
                    "<b>Inicio: Equinoccio de otoño - 21 de marzo</b>\n" +
                    "• Día y noche duran lo mismo";

                textoInfo2.text =
                    "<b>¿Cómo es el clima?</b>\n" +
                    "• Empieza a hacer más frío\n" +
                    "• Menos lluvias";

                textoInfo3.text =
                    "<b>¿Qué pasa en la naturaleza?</b>\n" +
                    "• Caen las hojas\n" +
                    "• Cambian de color";
                break;

            case 3: // INVIERNO
                textoInfo1.text =
                    "<b>¿Cuándo ocurre en nuestro hemisferio?</b>\n" +
                    "De junio a septiembre\n" +
                    "<b>Inicio: Solsticio de invierno - 21 de junio</b>\n" +
                    "• Día más corto del año";

                textoInfo2.text =
                    "<b>¿Cómo es el clima?</b>\n" +
                    "• Hace frío\n" +
                    "• Puede haber heladas";

                textoInfo3.text =
                    "<b>¿Qué pasa en la naturaleza?</b>\n" +
                    "• Las plantas crecen menos\n" +
                    "• Los animales buscan refugio";
                break;
        }
    }

    // ── Panel 1 ──────────────────────────────────────
    public void abrirInfo1()
    {
        if (estacionActual < 0 || estacionActual >= fondosInfo.Length) return;
        SetTextos();
        animatorInfo1.SetBool("InfoOn", true);
        botonAbrir1.SetActive(false);
        botonCerrar1.SetActive(true);
    }

    public void cerrarInfo1()
    {
        animatorInfo1.SetBool("InfoOn", false);
        botonAbrir1.SetActive(true);
        botonCerrar1.SetActive(false);
    }

    // ── Panel 2 ──────────────────────────────────────
    public void abrirInfo2()
    {
        if (estacionActual < 0 || estacionActual >= fondosInfo.Length) return;
        SetTextos();
        animatorInfo2.SetBool("InfoOn", true);
        botonAbrir2.SetActive(false);
        botonCerrar2.SetActive(true);
    }

    public void cerrarInfo2()
    {
        animatorInfo2.SetBool("InfoOn", false);
        botonAbrir2.SetActive(true);
        botonCerrar2.SetActive(false);
    }

    // ── Panel 3 ──────────────────────────────────────
    public void abrirInfo3()
    {
        if (estacionActual < 0 || estacionActual >= fondosInfo.Length) return;
        SetTextos();
        animatorInfo3.SetBool("InfoOn", true);
        botonAbrir3.SetActive(false);
        botonCerrar3.SetActive(true);
    }

    public void cerrarInfo3()
    {
        animatorInfo3.SetBool("InfoOn", false);
        botonAbrir3.SetActive(true);
        botonCerrar3.SetActive(false);
    }

    // ── Cerrar todo (botón Volver) ───────────────────
    public void cerrarTodosLosInfo()
    {
        animatorInfo1.SetBool("InfoOn", false);
        animatorInfo2.SetBool("InfoOn", false);
        animatorInfo3.SetBool("InfoOn", false);

        botonAbrir1.SetActive(true); botonCerrar1.SetActive(false);
        botonAbrir2.SetActive(true); botonCerrar2.SetActive(false);
        botonAbrir3.SetActive(true); botonCerrar3.SetActive(false);
    }
}