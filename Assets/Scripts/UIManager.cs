using System.Globalization;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public GameObject panelInicio;
    public GameObject panelMenu;

    public EventReference musicaIntroMenu;
    EventInstance musicaInstance;

    public GameObject panelPopup;

    public TextMeshProUGUI textoBoton1;
    public TextMeshProUGUI textoBoton2;

    public TextMeshProUGUI textoInfo;

    public GameObject boton2;

    public RectTransform boton1Transform;

    public Image imagenPlaneta;  // NUEVO: arrastra el objeto Image del panel aquí
    public EventReference selectBoton;
    public EventReference clicVolver;

    [Header("Fondos de Popup")]
    public Image fondoPopup;
    public Sprite fondoUniverso;
    public Sprite fondoTierra;
    public Sprite fondoTiempo;
    public Sprite fondoMundo;
    //public Sprite fondoPlay;

    public Color colorUniverso = Color.white;
    public Color colorTierra = Color.white;
    public Color colorTiempo = Color.white;
    public Color colorMundo = Color.white;
    //public Color colorPlay = Color.white;

    public Image imagenBoton1;
    public Image imagenBoton2;

    public Sprite[] spritesBoton1;

    [Header("Sprites Planetas")]  // NUEVO: asigna cada sprite en el Inspector
    public Sprite spriteMercurio;
    public Sprite spriteVenus;
    public Sprite spriteTierra;
    public Sprite spriteMarte;
    public Sprite spriteJupiter;
    public Sprite spriteSaturno;
    public Sprite spriteUrano;
    public Sprite spriteNeptuno;
    public Sprite spriteDefault;

    private string contextoActual;
    public static UIManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if(!musicaIntroMenu.IsNull)
            musicaInstance = AudioManager.Instance.CreateLoop(musicaIntroMenu);

        if (SceneManager.GetActiveScene().name == "Menu" && !MenuManager.Instance.getPrimeraVez())
        {
            panelInicio.SetActive(false);
            panelMenu.SetActive(true);
        }
    }
    public void IrAMenu()
    {
        AudioManager.Instance.Play(selectBoton);
        panelMenu.SetActive(true);
        panelInicio.SetActive(false);
        MenuManager.Instance.setPrimeraVez(false);
    }

    // UNIVERSO (2 botones)
    public void PopupUniverso()
    {
        contextoActual = "Universo";
        panelPopup.SetActive(true);
        AudioManager.Instance.Play(selectBoton);

        textoBoton1.text = "Explorando el Sistema Solar";
        textoBoton2.text = "Cuerpos Celestes";

        boton2.SetActive(true);

        // posición normal (arriba)
        boton1Transform.anchoredPosition = new Vector2(0, 200);
        AplicarEstiloPopup(fondoUniverso, colorUniverso, 0, 1);
    }

    // TIERRA (2 botones)
    public void PopupTierra()
    {
        contextoActual = "Tierra";
        panelPopup.SetActive(true);
        AudioManager.Instance.Play(selectBoton);

        textoBoton1.text = "El Planeta Tierra";
        textoBoton2.text = "Las Estaciones";

        boton2.SetActive(true);

        boton1Transform.anchoredPosition = new Vector2(0, 200);
        AplicarEstiloPopup(fondoTierra, colorTierra, 2, 3);
    }

    // TIEMPO (1 botón)
    public void PopupTiempo()
    {
        contextoActual = "Tiempo";
        panelPopup.SetActive(true);
        AudioManager.Instance.Play(selectBoton);

        textoBoton1.text = "Reloj Analógico y Digital";

        boton2.SetActive(false);

        // CENTRAR
        boton1Transform.anchoredPosition = new Vector2(0, 0);
        AplicarEstiloPopup(fondoTiempo, colorTiempo, 4);
    }

    // MUNDO (1 botón)
    public void PopupMundo()
    {
        contextoActual = "Mundo";
        panelPopup.SetActive(true);
        AudioManager.Instance.Play(selectBoton);

        textoBoton1.text = "Los Continentes";

        boton2.SetActive(false);

        //CENTRAR
        boton1Transform.anchoredPosition = new Vector2(0, 0);
        AplicarEstiloPopup(fondoMundo, colorMundo, 5);
    }

    // PLAY (1 botón)
    public void PopupPlay()
    {
        contextoActual = "play";
        panelPopup.SetActive(true);
        AudioManager.Instance.Play(selectBoton);

        textoBoton1.text = "Aprendamos Jugando";

        boton2.SetActive(false);

        //CENTRAR
        boton1Transform.anchoredPosition = new Vector2(0, 0);
        fondoPopup.sprite = fondoMundo;
    }

    public void OcultarPopup()
    {
        panelPopup.SetActive(false);
        AudioManager.Instance.Play(clicVolver);
    }

    public void IrACuerposCelestes()
    {
        SceneManager.LoadScene("CuerposCelestes");
    }

    void AplicarEstiloPopup(Sprite fondo, Color colorTexto, int idxBoton1 = -1, int idxBoton2 = -1)
    {
        fondoPopup.sprite = fondo;
        textoBoton1.color = colorTexto;
        textoBoton2.color = colorTexto;

        if (imagenBoton1 != null && idxBoton1 >= 0 && idxBoton1 < spritesBoton1.Length)
            imagenBoton1.sprite = spritesBoton1[idxBoton1];

        if (imagenBoton2 != null && idxBoton2 >= 0 && idxBoton2 < spritesBoton1.Length)
            imagenBoton2.sprite = spritesBoton1[idxBoton2];
    }

    public void Boton1Popup()
    {
        AudioManager.Instance.Play(selectBoton);
        AudioManager.Instance.StopLoop(musicaInstance);

        if (contextoActual == "Universo")
        {
            SceneManager.LoadScene("SistemaSolar");
        }
        else if (contextoActual == "Tierra")
        {
            SceneManager.LoadScene("PlanetaTierra");
        }
        else if (contextoActual == "Tiempo")
        {
            SceneManager.LoadScene("JuegoReloj");
        }
        else if (contextoActual == "Mundo")
        {
            SceneManager.LoadScene("Continentes");
        }
    }
    public void Boton2Popup()
    {
        AudioManager.Instance.Play(selectBoton);
        AudioManager.Instance.StopLoop(musicaInstance);

        if (contextoActual == "Universo")
        {
            SceneManager.LoadScene("CuerposCelestes");
        }
        else if (contextoActual == "Tierra")
        {
            SceneManager.LoadScene("LasEstaciones");
        }
    }
    public void VolverMenu()
    {
        AudioManager.Instance.Play(clicVolver);
        SceneManager.LoadScene("Menu");
    }

    public void MostrarInfo(string planeta)
    {
        panelPopup.SetActive(true);

        switch (planeta)
        {
            case "Mercurio":
                textoInfo.text = 
                    "<b>MERCURIO</b>\n" +
                    "\n" +
                    "<b>Posición:</b> 1\n" +
                    "<b>Tipo:</b> Rocoso\n" +
                    "N° de Lunas: Ninguna\n" +
                    "<b>Curiosidades:</b>\n" +
                    "   - Es el planeta más cercano al Sol.\n" +
                    "   - Tiene temperaturas muy extremas.";
                imagenPlaneta.sprite = spriteMercurio;
                break;

            case "Venus":
                textoInfo.text =
                    "<b>VENUS</b>\n" +
                    "\n" +
                    "<b>Posición:</b> 2\n" +
                    "<b>Tipo:</b> Rocoso\n" +
                    "N° de Lunas: Ninguna\n" +
                    "<b>Curiosidades:</b>\n" +
                    "   - Tiene una atmósfera densa que atrapa el calor, lo que lo hace el planeta más caliente.\n" +
                    "   - Gira en sentido contrario a la mayoría de los planetas.";
                imagenPlaneta.sprite = spriteVenus;
                break;

            case "Tierra":
                textoInfo.text =
                    "<b>TIERRA</b>\n" +
                    "\n" +
                    "<b>Posición:</b> 3\n" +
                    "<b>Tipo:</b> Rocoso\n" +
                    "<b>Nº de Lunas:</b> 1\n" +
                    "<b>Curiosidades:</b>\n" +
                    "- Es el único planeta conocido que alberga vida.\n" +
                    "- Casi tres cuartas partes de su superficie están cubiertas por agua.";
                imagenPlaneta.sprite = spriteTierra;
                break;

            case "Marte":
                textoInfo.text =
                    "<b>MARTE</b>\n" +
                    "\n" +
                    "<b>Posición:</b> 4\n" +
                    "<b>Tipo:</b> Rocoso\n" +
                    "<b>Nº de Lunas:</b> 2\n" +
                    "<b>Curiosidades:</b>\n" +
                    "- Es conocido como el \"planeta rojo\" debido al óxido de hierro en su superficie.\n" +
                    "- Tiene el volcán más grande del sistema solar, el Monte Olimpo.";
                imagenPlaneta.sprite = spriteMarte;
                break;

            case "Júpiter":
                textoInfo.text =
                    "<b>JÚPITER</b>\n" +
                    "\n" +
                    "<b>Posición:</b> 5\n" +
                    "<b>Tipo:</b> Gaseoso\n" +
                    "<b>Nº de Lunas:</b> 95\n" +
                    "<b>Curiosidades:</b>\n" +
                    "- Es el planeta más grande de todo el sistema solar.\n" +
                    "- Tiene una gran mancha roja que es una tormenta gigante.";
                imagenPlaneta.sprite = spriteJupiter;
                break;

            case "Saturno":
                textoInfo.text =
                    "<b>SATURNO</b>\n" +
                    "\n" +
                    "<b>Posición:</b> 6\n" +
                    "<b>Tipo:</b> Gaseoso\n" +
                    "<b>Nº de Lunas:</b> 146\n" +
                    "<b>Curiosidades:</b>\n" +
                    "- Es famoso por su complejo y visible sistema de anillos.\n" +
                    "- Es el segundo planeta más grande del sistema solar.";
                imagenPlaneta.sprite = spriteSaturno;
                break;

            case "Urano":
                textoInfo.text =
                    "<b>URANO</b>\n" +
                    "\n" +
                    "<b>Posición:</b> 7\n" +
                    "<b>Tipo:</b> Gigante helado\n" +
                    "<b>Nº de Lunas:</b> 28\n" +
                    "<b>Curiosidades:</b>\n" +
                    "- Gira de lado, casi paralelo a su órbita.\n" +
                    "- Su color azul verdoso se debe al metano en su atmósfera.";
                imagenPlaneta.sprite = spriteUrano;
                break;

            case "Neptuno":
                textoInfo.text =
                    "<b>NEPTUNO</b>\n" +
                    "\n" +
                    "<b>Posición:</b> 8\n" +
                    "<b>Tipo:</b> Gigante helado\n" +
                    "<b>Nº de Lunas:</b> 16\n" +
                    "<b>Curiosidades:</b>\n" +
                    "- Es el planeta más alejado del Sol.\n" +
                    "- Tiene vientos supersónicos extremadamente rápidos.";
                imagenPlaneta.sprite = spriteNeptuno;
                break;

            default:
                textoInfo.text = "Información no disponible.";
                imagenPlaneta.sprite = spriteDefault;
                panelPopup.SetActive(false);
                break;
        }
    }

    public void MostrarDesplegable()
    {
        Debug.Log("Desplegable aun vacio");
    }
}