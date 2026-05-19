using TMPro;
using UnityEngine;

public class NavegadorCuerpos : MonoBehaviour
{
    public GameObject canvas;
    //public static NavegadorCuerpos Instance;

    public GameObject pantallaMenu;
    public GameObject pantallaContenido;

    public GameObject[] paneles; // arrastra los 6 paneles en orden

    //void Awake() => Instance = this;

    //public GameObject pantallasBienvenida; // padre de las 3 pantallas
    public GameObject bienvenidaEstrellas;
    public GameObject bienvenidaGalaxias;
    public GameObject bienvenidaPlanetas;

    public GameObject nebulosa;
    //public GameObject nebulosaContenido;
    public GameObject secuenciaPrincipal;
    //public GameObject secuenciaContenido;
    public GameObject muerteEstrella;
    //public GameObject muerteEstrContenido;
    public GameObject estrellaInfo;
    public TextMeshProUGUI textoInfo;

    public void AbrirSeccion(int index)
    {
        pantallaMenu.SetActive(false);
        pantallaContenido.SetActive(true);

        for (int i = 0; i < paneles.Length; i++)
            paneles[i].SetActive(i == index);

        // Verifica si debe mostrar bienvenida
        if (index == 0 && MenuManager.Instance.primeraVezEstrellas)
        {
            bienvenidaEstrellas.SetActive(true);
            MenuManager.Instance.primeraVezEstrellas = false;
        }
        else if (index == 1 && MenuManager.Instance.primeraVezGalaxias)
        {
            bienvenidaGalaxias.SetActive(true);
            MenuManager.Instance.primeraVezGalaxias = false;
        }
        else if (index == 5 && MenuManager.Instance.primeraVezPlanetas)
        {
            bienvenidaPlanetas.SetActive(true);
            MenuManager.Instance.primeraVezPlanetas = false;
        }
    }

    public void Volver()
    {
        pantallaContenido.SetActive(false);
        pantallaMenu.SetActive(true);
    }
    public void Continuar()
    {
        if (bienvenidaEstrellas)
            bienvenidaEstrellas.SetActive(false);
        if (bienvenidaGalaxias)
            bienvenidaGalaxias.SetActive(false);
        if (bienvenidaPlanetas)
            bienvenidaPlanetas.SetActive(false);
    }

    public void Ir3D(int tipo)
    {
        canvas.SetActive(false);
        
        switch (tipo){
            case 1: nebulosa.SetActive(true); break;
            case 2: secuenciaPrincipal.SetActive(true); break;
            case 3: muerteEstrella.SetActive(true); break;
        }
    }

    public void Volver3D(int tipo)
    {
        canvas.SetActive(true);

        switch (tipo)
        {
            case 1: nebulosa.SetActive(false); break;
            case 2: secuenciaPrincipal.SetActive(false); break;
            case 3: muerteEstrella.SetActive(false); break;
        }
    }

    public void mostrarInfo(int tipo)
    {
        estrellaInfo.SetActive(true);
        switch (tipo)
        {
            case 1: 
                textoInfo.text =
                    "<b> 1.\tNebulosa. </b>\n" +
                    "\n" +
                    "Las estrellas nacen en enormes nubes de gas y polvo llamadas nebulosas. Con el tiempo, la gravedad junta estos materiales hasta formar una nueva estrella."; break;
            case 2:
                textoInfo.text =
                    "<b> 2.\tSecuencia Principal. </b>\n" +
                    "\n" +
                    "Es la etapa más larga de la vida de una estrella. Durante este tiempo, produce luz y calor, como nuestro Sol."; break;
            case 3:
                textoInfo.text =
                    "<b> 3.\tMuerte. </b>\n" +
                    "\n" +
                    "Cuando una estrella agota su energía, cambia según su tamaño. Algunas se convierten en enanas blancas y otras explotan formando supernovas o agujeros negros."; break;
        }
    }
    public void ocultarInfo()
    {
        estrellaInfo.SetActive(false);
    }
}