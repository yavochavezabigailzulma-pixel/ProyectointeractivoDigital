using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeleccionarContinente : MonoBehaviour
{
    public GameObject panelContent;
    public GameObject panelMenu;

    public Sprite[] contents;
    //public Image contentImage;
    public TMP_Text textoInfo;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnMouseDown()
    {
        if (!CollidersManager.Instance.inMap) return;

            Debug.Log("¡Hiciste clic en el objeto: " + gameObject.name + "!");
            AbrirPanelMenu();
    }

    public void AbrirPanelMenu()
    {
        panelMenu.SetActive(true);
        CollidersManager.Instance.SwitchInMap(false);
    }
    public void AbrirPanelContent(int catego)
    {
        switch (catego)
        {
            case 0:
                textoInfo.text =
                    "<b>- Es un continente muy grande</b>\n" +
                    "\n" +
                    "<b>- Está en el oeste del planeta</b>\n" +
                    "\n" +
                    "<b>- Se divide en: Norte, Centro y Sur</b>\n";
                break;

            case 1:
                textoInfo.text =
                    "<b>- Frío como en la cordillera de los Andes</b>\n" +
                    "\n" +
                    "<b>- Lluvioso como en la Amazonía</b>\n" +
                    "\n" +
                    "<b>- Desértico como en México</b>\n" +
                    "\n" +
                    "<b>- Animales como llamas y jaguares</b>\n";
                break;

            case 2:
                textoInfo.text =
                    "<b>- Machu Picchu</b>\n" +
                    "\n" +
                    "<b>- Cataratas del Iguazú</b>\n" +
                    "\n" +
                    "<b>- Estatua de la Libertad</b>\n" +
                    "\n" +
                    "<b>- Pirámides mayas</b>\n" +
                    "\n" +
                    "<b>- Amazonía</b>\n";
                break;
        }
        panelContent.SetActive(true);
    }
    public void CerrarPanelMenu()
    {
        panelMenu.SetActive(false);
        CollidersManager.Instance.SwitchInMap(true);
    }
    public void CerrarPanelContent()
    {
        panelContent.SetActive(false);
    }
}
