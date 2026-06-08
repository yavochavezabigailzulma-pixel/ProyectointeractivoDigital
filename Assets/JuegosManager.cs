using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuegosManager : MonoBehaviour
{
    public GameObject bienvenida;
    public GameObject menu;
    public GameObject perguntadosPanel;
    public GameObject horaPanel;
    public GameObject sopaPanel;
    public GameObject oracionPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CerrarBienvenida()
    {
        bienvenida.SetActive(false);
    }
    public void AbrirJuego(int juego)
    {
        menu.SetActive(false);
        switch (juego)
        {
            case 0: perguntadosPanel.SetActive(true); break;
            case 1: horaPanel.SetActive(true); break;
            case 2: sopaPanel.SetActive(true); break;
            case 3: oracionPanel.SetActive(true); break;
        }
    }
    public void CerrarJuego()
    {
        menu.SetActive(true);

        perguntadosPanel.SetActive(false);
        horaPanel.SetActive(false);
        sopaPanel.SetActive(false);
        oracionPanel.SetActive(true);
        }
}
