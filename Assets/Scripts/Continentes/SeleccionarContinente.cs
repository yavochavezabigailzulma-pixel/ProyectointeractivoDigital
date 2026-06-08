using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeleccionarContinente : MonoBehaviour
{
    [Header("Imágenes panorámicas por continente")]
    public RectTransform[] imagenesPanoramicas; // Índice 0=América, 1=Europa, etc.

    public PanoramicaScroll panoramicaScroll;

    public GameObject panelContent;
    public GameObject panelMenu;

    public GameObject[] contents;
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

        panelContent.SetActive(true);

        contents[catego].SetActive(true);
        // Asignar la imagen correspondiente ANTES de activar el content
        if (panoramicaScroll != null)
        {
            panoramicaScroll.CambiarImagen(imagenesPanoramicas[catego]);
        }

    }
    public void CerrarPanelMenu()
    {
        panelMenu.SetActive(false);
        CollidersManager.Instance.SwitchInMap(true);
    }
    public void CerrarPanelContent()
    {
        panelContent.SetActive(false); 
        foreach (GameObject content in contents)
        {
            content.SetActive(false);
        }
    }
}
