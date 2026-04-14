using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject panelPopup;

    public TextMeshProUGUI textoBoton1;
    public TextMeshProUGUI textoBoton2;

    public GameObject boton2;

    public RectTransform boton1Transform; //NUEVO

    private string contextoActual;

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
}