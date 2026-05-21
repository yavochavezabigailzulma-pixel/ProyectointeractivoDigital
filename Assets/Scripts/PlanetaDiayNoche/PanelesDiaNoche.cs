using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelesDiaNoche : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelInfo;
    public TextMeshProUGUI textoInfo;

    public void mostrarInfo()
    {
        panelInfo.SetActive(true);
    }
    public void ocultarInfo()
    {
        panelInfo.SetActive(false);
    }
}