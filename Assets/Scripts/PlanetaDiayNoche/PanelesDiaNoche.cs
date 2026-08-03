using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelesDiaNoche : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelInfo;
    public TextMeshProUGUI textoInfo;

    public EventReference abrirDeslizable;
    public EventReference cerrarDeslizable;

    public void mostrarInfo()
    {
        panelInfo.SetActive(true);

        AudioManager.Instance.Play(abrirDeslizable);
    }
    public void ocultarInfo()
    {
        panelInfo.SetActive(false);

        AudioManager.Instance.Play(cerrarDeslizable);
    }
}