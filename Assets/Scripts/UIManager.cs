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


    public GameObject boton2;

    public RectTransform boton1Transform;

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
        AudioManager.Instance.Play(selectBoton);
        AudioManager.Instance.StopLoop(musicaInstance);
        MenuManager.Instance.indiceCarrusel = 1; // ajusta el índice que corresponda
        SceneManager.LoadScene("minijuegos");
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
            MenuManager.Instance.indiceCarrusel = 0; // índice del carrusel
            SceneManager.LoadScene("SistemaSolar");
        }
        else if (contextoActual == "Tierra")
        {
            MenuManager.Instance.indiceCarrusel = 4;
            SceneManager.LoadScene("PlanetaTierra");
        }
        else if (contextoActual == "Tiempo")
        {
            MenuManager.Instance.indiceCarrusel = 3;
            SceneManager.LoadScene("JuegoReloj");
        }
        else if (contextoActual == "Mundo")
        {
            MenuManager.Instance.indiceCarrusel = 2;
            SceneManager.LoadScene("Continentes");
        }
    }

    public void Boton2Popup()
    {
        AudioManager.Instance.Play(selectBoton);
        AudioManager.Instance.StopLoop(musicaInstance);

        if (contextoActual == "Universo")
        {
            MenuManager.Instance.indiceCarrusel = 0;
            SceneManager.LoadScene("CuerposCelestes");
        }
        else if (contextoActual == "Tierra")
        {
            MenuManager.Instance.indiceCarrusel = 4;
            SceneManager.LoadScene("LasEstaciones");
        }
    }
    public void VolverMenu()
    {
        //AudioManager.Instance.Play(clicVolver);
        SceneManager.LoadScene("Menu");
    }

    
    public void MostrarDesplegable()
    {
        Debug.Log("Desplegable aun vacio");
    }
}