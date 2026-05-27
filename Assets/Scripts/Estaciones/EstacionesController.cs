using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EstacionesController : MonoBehaviour
{
    [Header("Panel principal")]
    public Sprite[] fondos;
    public Image fondo;
    public GameObject panelFondo;

    [Header("Panel información")]
    public Sprite[] fondosInfo;
    //public string[] textosInfo;

    //public Image imagenInfo;
    public TMP_Text textoInfo;

    public GameObject panelInfo;

    // Guarda la estación actualmente seleccionada
    private int estacionActual = -1;

    public void abrirFondo(int estacion)
    {
        if (estacion < 0 || estacion >= fondos.Length)
            return;

        estacionActual = estacion;

        fondo.sprite = fondos[estacion];

        panelFondo.SetActive(true);
    }

    // Botón común para abrir información
    public void abrirInfo()
    {

        Debug.Log(estacionActual);
        if (estacionActual < 0)
            return;

        // Seguridad
        if (estacionActual >= fondosInfo.Length)
            return;

        panelInfo.GetComponent<Image>().sprite = fondosInfo[estacionActual];

        switch (estacionActual)
        {
            case 0:
                textoInfo.text =
                    "<align=center><size=150%><b>PRIMAVERA</b></size></align>\n" +
                    "\n" +
                    "<b>¿Cuándo ocurre en nuestro hemisferio?</b>\n" +
                    "De septiembre a diciembre\n" +
                    "\n" +
                    "<b>Inicio: Equinoccio de primavera - 21 de septiembre</b>\n" +
                    "\n" +
                    "• Día y noche duran casi lo mismo\n" +
                    "\n" +
                    "<b>¿Cómo es el clima?</b>\n" +
                    "\n" +
                    "• Temperatura agradable\n" +
                    "• Empieza a hacer más calor\n" +
                    "\n" +
                    "<b>¿Qué pasa en la naturaleza?</b>\n" +
                    "\n" +
                    "• Florecen las plantas\n" +
                    "• Todo se vuelve verde y colorido";
                break;

            case 1:
                textoInfo.text =
                    "<align=center><size=150%><b>VERANO</b></size></align>\n" +
                    "\n" +
                    "<b>¿Cuándo ocurre en nuestro hemisferio?</b>\n" +
                    "De diciembre a marzo\n" +
                    "\n" +
                    "<b>Inicio: Solsticio de verano - 21 de diciembre</b>\n" +
                    "\n" +
                    "• Día más largo del año\n" +
                    "\n" +
                    "<b>¿Cómo es el clima?</b>\n" +
                    "\n" +
                    "• Mucho calor\n" +
                    "• Época de lluvias\n" +
                    "\n" +
                    "<b>¿Qué pasa en la naturaleza?</b>\n" +
                    "\n" +
                    "• Las plantas crecen rápido\n" +
                    "• Hay tormentas frecuentes";
                break;

            case 2:
                textoInfo.text =
                    "<align=center><size=150%><b>OTOÑO</b></size></align>\n" +
                    "\n" +
                    "<b>¿Cuándo ocurre en nuestro hemisferio?</b>\n" +
                    "De marzo a junio\n" +
                    "\n" +
                    "<b>Inicio: Equinoccio de otoño - 21 de marzo</b>\n" +
                    "\n" +
                    "• Día y noche duran lo mismo\n" +
                    "\n" +
                    "<b>¿Cómo es el clima?</b>\n" +
                    "\n" +
                    "• Empieza a hacer más frío\n" +
                    "• Menos lluvias\n" +
                    "\n" +
                    "<b>¿Qué pasa en la naturaleza?</b>\n" +
                    "\n" +
                    "• Caen las hojas\n" +
                    "• Cambian de color";
                break;

            case 3:
                textoInfo.text =
                    "<align=center><size=150%><b>INVIERNO</b></size></align>\n" +
                    "\n" +
                    "<b>¿Cuándo ocurre en nuestro hemisferio?</b>\n" +
                    "De junio a septiembre\n" +
                    "\n" +
                    "<b>Inicio: Solsticio de invierno - 21 de junio</b>\n" +
                    "\n" +
                    "• Día más corto del año\n" +
                    "\n" +
                    "<b>¿Cómo es el clima?</b>\n" +
                    "\n" +
                    "• Hace frío\n" +
                    "• Puede haber heladas\n" +
                    "\n" +
                    "<b>¿Qué pasa en la naturaleza?</b>\n" +
                    "\n" +
                    "• Las plantas crecen menos\n" +
                    "• Los animales buscan refugio";
                break;
        }
        panelInfo.SetActive(true);
    }

    public void cerrarFondo()
    {
        panelFondo.SetActive(false);
    }

    public void cerrarInfo()
    {
        panelInfo.SetActive(false);
    }
}