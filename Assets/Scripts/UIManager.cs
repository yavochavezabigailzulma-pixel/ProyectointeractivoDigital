using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Globalization;

public class UIManager : MonoBehaviour
{
    public GameObject panelPopup;

    public TextMeshProUGUI textoBoton1;
    public TextMeshProUGUI textoBoton2;

    public TextMeshProUGUI textoInfo;

    public GameObject boton2;

    public RectTransform boton1Transform;

    public Image imagenPlaneta;  // NUEVO: arrastra el objeto Image del panel aquí

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
    // UNIVERSO (2 botones)
    public void PopupUniverso()
    {
        contextoActual = "Universo";
        panelPopup.SetActive(true);

        textoBoton1.text = "Explorando el Sistema Solar";
        textoBoton2.text = "Cuerpos Celestes";

        boton2.SetActive(true);

        // posición normal (arriba)
        boton1Transform.anchoredPosition = new Vector2(0, 200);
    }

    // TIERRA (2 botones)
    public void PopupTierra()
    {
        contextoActual = "Tierra";
        panelPopup.SetActive(true);

        textoBoton1.text = "El Planeta Tierra";
        textoBoton2.text = "Las Estaciones";

        boton2.SetActive(true);

        boton1Transform.anchoredPosition = new Vector2(0, 200);
    }

    // TIEMPO (1 botón)
    public void PopupTiempo()
    {
        contextoActual = "Tiempo";
        panelPopup.SetActive(true);

        textoBoton1.text = "Reloj Analógico y Digital";

        boton2.SetActive(false);

        // CENTRAR
        boton1Transform.anchoredPosition = new Vector2(0, 0);
    }

    // MUNDO (1 botón)
    public void PopupMundo()
    {
        contextoActual = "Mundo";
        panelPopup.SetActive(true);

        textoBoton1.text = "Los Continentes";

        boton2.SetActive(false);

        //CENTRAR
        boton1Transform.anchoredPosition = new Vector2(0, 0);
    }

    public void OcultarPopup()
    {
        panelPopup.SetActive(false);
    }

    public void IrACuerposCelestes()
    {
        SceneManager.LoadScene("CuerposCelestes");
    }

    public void Boton1Popup()
    {
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
            SceneManager.LoadScene("Reloj");
        }
        else if (contextoActual == "Mundo")
        {
            SceneManager.LoadScene("Continentes");
        }
    }
    public void Boton2Popup()
    {
        if (contextoActual == "Universo")
        {
            SceneManager.LoadScene("CuerposCelestes");
        }
        else if (contextoActual == "Tierra")
        {
            SceneManager.LoadScene("Estaciones");
        }
    }
    public void VolverMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void MostrarInfo(string planeta)
    {
        panelPopup.SetActive(true);

        switch (planeta)
        {
            case "Mercurio":
                textoInfo.text = "Mercurio es el planeta más cercano al Sol y el más pequeño del sistema solar.";
                imagenPlaneta.sprite = spriteMercurio;
                break;

            case "Venus":
                textoInfo.text = "Venus es el segundo planeta del sistema solar y el más caliente, con temperaturas de hasta 465°C.";
                imagenPlaneta.sprite = spriteVenus;
                break;

            case "Tierra":
                textoInfo.text = "La Tierra es el tercer planeta del sistema solar, formado hace unos 4.500 millones de años, y el único conocido que alberga vida.";
                imagenPlaneta.sprite = spriteTierra;
                break;

            case "Marte":
                textoInfo.text = "Marte es el planeta rojo...";
                imagenPlaneta.sprite = spriteMarte;
                break;

            case "Júpiter":
                textoInfo.text = "Júpiter es el planeta más grande del sistema solar, una gigante gaseosa con la famosa Gran Mancha Roja.";
                imagenPlaneta.sprite = spriteJupiter;
                break;

            case "Saturno":
                textoInfo.text = "Saturno es conocido por su impresionante sistema de anillos, compuestos principalmente de hielo y roca.";
                imagenPlaneta.sprite = spriteSaturno;
                break;

            case "Urano":
                textoInfo.text = "Urano es un gigante de hielo que rota sobre su lado, con un eje de inclinación de casi 98 grados.";
                imagenPlaneta.sprite = spriteUrano;
                break;

            case "Neptuno":
                textoInfo.text = "Neptuno es el planeta más lejano del sistema solar, conocido por sus vientos más veloces, superando los 2.000 km/h.";
                imagenPlaneta.sprite = spriteNeptuno;
                break;

            default:
                textoInfo.text = "Información no disponible.";
                imagenPlaneta.sprite = spriteDefault;
                panelPopup.SetActive(false);
                break;
        }
    }
}