using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeleccionarContinente : MonoBehaviour
{
    public GameObject panelContent;
    public GameObject panelMenu;

    public Sprite[] contents;
    public Image contentImage;
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
        if (CollidersManager.Instance.inMap)
        {
            Debug.Log("¡Hiciste clic en el objeto: " + gameObject.name + "!");
            AbrirPanelMenu();
        }
    }

    public void AbrirPanelMenu()
    {
        panelMenu.SetActive(true);
        CollidersManager.Instance.SwitchInMap(false);
    }
    public void AbrirPanelContent(int catego)
    {
        contentImage.sprite = contents[catego];        
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
