using TMPro;
using UnityEngine;

public class NavegadorCuerpos : MonoBehaviour
{
    public GameObject canvas;
    //public static NavegadorCuerpos Instance;

    public GameObject pantallaMenu;
    public GameObject pantallaContenido;

    public GameObject[] paneles; // arrastra los 6 paneles en orden
    public Sprite[] infoBG;
    //void Awake() => Instance = this;

    //public GameObject pantallasBienvenida; // padre de las 3 pantallas
    public GameObject bienvenidaEstrellas;
    public GameObject bienvenidaGalaxias;
    public GameObject bienvenidaPlanetas;

    [Header("Objetos 3D")]
    public GameObject nebulosa;
    public GameObject secuenciaPrincipal;
    public GameObject muerteEstrella;

    public GameObject galEspirales;
    public GameObject galElipticas;
    public GameObject galIrregulares;

    public GameObject satNaturales;
    public GameObject satArtificiales;

    public GameObject cometas;

    public GameObject asteroides;

    public GameObject planetMercurio;
    public GameObject planetVenus;
    public GameObject planetTierra;
    public GameObject planetMarte;
    public GameObject planetJupiter;
    public GameObject planetSaturno;
    public GameObject planetUrano;
    public GameObject planetNeptuno;

    [Header ("Paneles")]
    public GameObject panelInfo;
    public TextMeshProUGUI textoInfo;

    public void AbrirSeccion(int index)
    {
        pantallaMenu.SetActive(false);
        pantallaContenido.SetActive(true);

        for (int i = 0; i < paneles.Length; i++)
        {
            paneles[i].SetActive(i == index);
            if(i == index)
                panelInfo.GetComponent<UnityEngine.UI.Image>().sprite = infoBG[i];
        }

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
            case 1: nebulosa.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(nebulosa.transform); break;
            case 2: secuenciaPrincipal.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(secuenciaPrincipal.transform); break;
            case 3: muerteEstrella.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(muerteEstrella.transform); break;
            case 4: galEspirales.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(galEspirales.transform); break;
            case 5: galElipticas.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(galElipticas.transform); break;
            case 6: galIrregulares.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(galIrregulares.transform); break;
            case 7: satNaturales.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(satNaturales.transform); break;
            case 8: satArtificiales.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(satArtificiales.transform); break;
            case 9: cometas.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(cometas.transform); break;
            case 10: asteroides.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(asteroides.transform); break;

            case 11: planetMercurio.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(planetMercurio.transform); break;
            case 12: planetVenus.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(planetVenus.transform); break;
            case 13: planetTierra.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(planetTierra.transform); break;
            case 14: planetMarte.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(planetMarte.transform); break;
            case 15: planetJupiter.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(planetJupiter.transform); break;
            case 16: planetSaturno.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(planetSaturno.transform); break;
            case 17: planetUrano.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(planetUrano.transform); break;
            case 18: planetNeptuno.SetActive(true);
                ZoomCuerpos.Instance.SetObjetivo(planetNeptuno.transform); break;
        }
    }

    public void Volver3D(int tipo)
    {
        canvas.SetActive(true);
        ZoomCuerpos.Instance.SetObjetivo(null);
        switch (tipo)
        {
            case 1: nebulosa.SetActive(false); break;
            case 2: secuenciaPrincipal.SetActive(false); break;
            case 3: muerteEstrella.SetActive(false); break;
            case 4: galEspirales.SetActive(false); break;
            case 5: galElipticas.SetActive(false); break;
            case 6: galIrregulares.SetActive(false); break;
            case 7: satNaturales.SetActive(false); break;
            case 8: satArtificiales.SetActive(false); break;
            case 9: cometas.SetActive(false); break;
            case 10: asteroides.SetActive(false); break;

            case 11: planetMercurio.SetActive(false); break;
            case 12: planetVenus.SetActive(false); break;
            case 13: planetTierra.SetActive(false); break;
            case 14: planetMarte.SetActive(false); break;
            case 15: planetJupiter.SetActive(false); break;
            case 16: planetSaturno.SetActive(false); break;
            case 17: planetUrano.SetActive(false); break;
            case 18: planetNeptuno.SetActive(false); break;
        }
    }

    public void mostrarInfo(int tipo)
    {
        panelInfo.SetActive(true);
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
            case 4:
                textoInfo.text =
                    "<b> 1.\tEspirales. </b>\n" +
                    "\n" +
                    "Tienen forma de remolino con brazos llenos de estrellas. La Vía Láctea, donde vivimos, es una galaxia espiral."; break;
            case 5:
                textoInfo.text =
                    "<b> 2.\tElípticas. </b>\n" +
                    "\n" +
                    "Tienen forma redonda u ovalada. Contienen muchas estrellas antiguas y poco gas."; break;
            case 6:
                textoInfo.text =
                    "<b> 3.\tIrregulares. </b>\n" +
                    "\n" +
                    "No tienen una forma definida. Parecen desordenadas porque han cambiado por choques o movimientos en el espacio."; break;
            case 7:
                textoInfo.text =
                    "<b> 1.\tNaturales. </b>\n" +
                    "\n" +
                    "Son cuerpos que giran alrededor de un planeta. La Luna es el satélite natural de la Tierra."; break;
            case 8:
                textoInfo.text =
                    "<b> 2.\tArtificiales. </b>\n" +
                    "\n" +
                    "Son máquinas creadas por las personas y enviadas al espacio para orbitar junto con la Tierra. Sirven para comunicarnos, tomar fotos de la Tierra y estudiar el universo."; break;
            case 9:
                textoInfo.text =
                    "<b> Cometas. </b>\n" +
                    "\n" +
                    "Los cometas son cuerpos de hielo, polvo y roca que viajan por el espacio. Cuando se acercan al Sol, el calor forma una brillante cola."; break;
            case 10:
                textoInfo.text =
                    "<b> Asteroides. </b>\n" +
                    "\n" +
                    "Los asteroides son grandes rocas espaciales que giran alrededor del Sol. La mayoría se encuentra entre Marte y Júpiter."; break;
            case 11:
                textoInfo.text =
                    "<b> Mercurio. </b>\n" +
                    "\n" +
                    "."; break;
            case 12:
                textoInfo.text =
                    "<b> Venus. </b>\n" +
                    "\n" +
                    "."; break;
            case 13:
                textoInfo.text =
                    "<b> Tierra. </b>\n" +
                    "\n" +
                    "."; break;
            case 14:
                textoInfo.text =
                    "<b> Marte. </b>\n" +
                    "\n" +
                    "."; break;
            case 15:
                textoInfo.text =
                    "<b> Júpiter. </b>\n" +
                    "\n" +
                    "."; break;
            case 16:
                textoInfo.text =
                    "<b> Saturno. </b>\n" +
                    "\n" +
                    "."; break;
            case 17:
                textoInfo.text =
                    "<b> Urano. </b>\n" +
                    "\n" +
                    "."; break;
            case 18:
                textoInfo.text =
                    "<b> Neptuno. </b>\n" +
                    "\n" +
                    "."; break;
        }
    }
    public void ocultarInfo()
    {
        panelInfo.SetActive(false);
    }
}