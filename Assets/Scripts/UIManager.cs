using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject panelPopup;

    public TextMeshProUGUI textoBoton1;
    public TextMeshProUGUI textoBoton2;

    public GameObject boton2;

    public RectTransform boton1Transform; //NUEVO

    // UNIVERSO (2 botones)
    public void PopupUniverso()
    {
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
        panelPopup.SetActive(true);

        textoBoton1.text = "El Planeta Tierra";
        textoBoton2.text = "Las Estaciones";

        boton2.SetActive(true);

        boton1Transform.anchoredPosition = new Vector2(0, 200);
    }

    // TIEMPO (1 botón)
    public void PopupTiempo()
    {
        panelPopup.SetActive(true);

        textoBoton1.text = "Reloj Analógico y Digital";

        boton2.SetActive(false);

        // CENTRAR
        boton1Transform.anchoredPosition = new Vector2(0, 0);
    }

    // MUNDO (1 botón)
    public void PopupMundo()
    {
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
}