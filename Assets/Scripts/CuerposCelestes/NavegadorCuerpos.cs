using TMPro;
using UnityEngine;

public class NavegadorCuerpos : MonoBehaviour
{
    public GameObject canvas;
    //public static NavegadorCuerpos Instance;
    public Animator animator;
    public GameObject pantallaMenu;
    public GameObject pantallaContenido;
    public int infoID;
    public GameObject[] paneles; // arrastra los 6 paneles en orden
    //public Sprite[] infoBG;
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

    [Header("Ajuste Título Planetas (case 11+)")]
    public Vector2 posicionTituloPlanetas = new Vector2(0f, 0f);
    public float tamanioTituloPlanetas = 36f;

    private Vector2 posicionTituloDefault;
    private float tamanioTituloDefault;
    private bool defaultsGuardados = false;

    [Header ("Paneles")]
    public GameObject panelInfo;
    public TextMeshProUGUI textoInfo;
    public TextMeshProUGUI textoTitulo;
    public GameObject botonAbrirInfo;
    public GameObject botonCerrarInfo;

    void Start()
    {
        posicionTituloDefault = textoTitulo.rectTransform.anchoredPosition;
        tamanioTituloDefault = textoTitulo.fontSize;
    }

    public void AbrirSeccion(int index)
    {
        pantallaMenu.SetActive(false);
        pantallaContenido.SetActive(true);

        for (int i = 0; i < paneles.Length; i++)
        {
            paneles[i].SetActive(i == index);
            //if(i == index)
                //panelInfo.GetComponent<UnityEngine.UI.Image>().sprite = infoBG[i];
        }

        // Verifica si debe mostrar bienvenida
        if (index == 0 /*&& MenuManager.Instance.primeraVezEstrellas*/)
        {
            bienvenidaEstrellas.SetActive(true);
            //MenuManager.Instance.primeraVezEstrellas = false;
        }
        else if (index == 1/* && MenuManager.Instance.primeraVezGalaxias*/)
        {
            bienvenidaGalaxias.SetActive(true);
            //MenuManager.Instance.primeraVezGalaxias = false;
        }
        else if (index == 5 /*&& MenuManager.Instance.primeraVezPlanetas*/)
        {
            bienvenidaPlanetas.SetActive(true);
            //MenuManager.Instance.primeraVezPlanetas = false;
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
        
        infoID = tipo;
        botonAbrirInfo.SetActive(true);
        botonCerrarInfo.SetActive(false);
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

    public void AsignarID()
    {
        mostrarInfo(infoID);
    }
    public void mostrarInfo(int tipo)
    {
        //panelInfo.SetActive(true);
        animator.SetBool("InfoOn", true);

        botonAbrirInfo.SetActive(false);
        botonCerrarInfo.SetActive(true);

        // Ajuste independiente de tamaño/posición para planetas (case 11+)
        if (tipo >= 11)
        {
            textoTitulo.rectTransform.anchoredPosition = posicionTituloPlanetas;
            textoTitulo.fontSize = tamanioTituloPlanetas;
        }
        else
        {
            textoTitulo.rectTransform.anchoredPosition = posicionTituloDefault;
            textoTitulo.fontSize = tamanioTituloDefault;
        }

        switch (tipo)
        {
            case 1:
                textoTitulo.text =
                    "<align=center><size=150%><b>NEBULOSA</b></size></align>";
                textoInfo.text =
                    "Las estrellas nacen en enormes nubes de gas y polvo llamadas nebulosas.\n" +
                    "Con el tiempo, la gravedad junta estos materiales hasta formar una nueva estrella.";
                break;

            case 2:
                textoTitulo.text =
                    "<align=center><size=150%><b>SECUENCIA PRINCIPAL</b></size></align>";

                textoInfo.text =
                    "Es la etapa más larga de la vida de una estrella.\n" +
                    "Durante este tiempo, produce luz y calor, como nuestro Sol.";
                break;

            case 3:
                textoTitulo.text =
                    "<align=center><size=150%><b>MUERTE ESTELAR</b></size></align>";

                textoInfo.text =
                    "Cuando una estrella agota su energía, cambia según su tamaño.\n" +
                    "Algunas se convierten en enanas blancas y otras explotan formando supernovas o agujeros negros.";
                break;

            case 4:
                textoTitulo.text =
                    "<align=center><size=150%><b>GALAXIAS ESPIRALES</b></size></align>";

                textoInfo.text =
                    "Tienen forma de remolino con brazos llenos de estrellas.\n" +
                    "La Vía Láctea, donde vivimos, es una galaxia espiral.";
                break;

            case 5:
                textoTitulo.text =
                    "<align=center><size=150%><b>GALAXIAS ELÍPTICAS</b></size></align>";

                textoInfo.text =
                    "Tienen forma redonda u ovalada.\n" + "" +
                    "Contienen muchas estrellas antiguas y poco gas.";
                break;

            case 6:
                textoTitulo.text =
                    "<align=center><size=150%><b>GALAXIAS IRREGULARES</b></size></align>";

                textoInfo.text =
                    "No tienen una forma definida.\n" +"" +
                    "Parecen desordenadas porque han cambiado por choques o movimientos en el espacio.";
                break;

            case 7:
                textoTitulo.text =
                    "<align=center><size=150%><b>SATÉLITES NATURALES</b></size></align>";

                textoInfo.text =
                    "Son cuerpos que giran alrededor de un planeta.\n" +
                    "La Luna es el satélite natural de la Tierra.";
                break;

            case 8:
                textoTitulo.text =
                    "<align=center><size=150%><b>SATÉLITES ARTIFICIALES</b></size></align>";

                textoInfo.text =
                    "Son máquinas creadas por las personas y enviadas al espacio para orbitar la Tierra.\n" + 
                    "Sirven para comunicarnos, tomar fotos y estudiar el universo.";
                break;

            case 9:
                textoTitulo.text =
                    "<align=center><size=150%><b>COMETAS</b></size></align>";

                textoInfo.text =
                    "Los cometas son cuerpos de hielo, polvo y roca que viajan por el espacio.\n" +"" +
                    "Cuando se acercan al Sol, el calor forma una brillante cola.";
                break;

            case 10:
                textoTitulo.text =
                    "<align=center><size=150%><b>ASTEROIDES</b></size></align>";

                textoInfo.text =
                    "Los asteroides son grandes rocas espaciales que giran alrededor del Sol.\n" + 
                    "La mayoría se encuentra entre Marte y Júpiter.";
                break;

            case 11:
                textoTitulo.text =
                    "<align=center><size=150%><b>MERCURIO</b></size></align>\n" +
                    "\n" +
                    "\n" +
                    "<b>Posición:</b> 1\n" +
                    "<b>Tipo:</b> Rocoso\n" +
                    "N° de Lunas: Ninguna\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "- Es el planeta más cercano al Sol.\n" +
                    "- Tiene temperaturas muy extremas.";
                break;

            case 12:
                textoTitulo.text =
                    "<align=center><size=150%><b>VENUS</b></size></align>\n" +
                    "\n" +
                    "\n" +
                    "<b>Posición:</b> 2\n" +
                    "<b>Tipo:</b> Rocoso\n" +
                    "N° de Lunas: Ninguna\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "- Tiene una atmósfera densa que atrapa el calor, lo que lo hace el planeta más caliente.\n" +
                    "- Gira en sentido contrario a la mayoría de los planetas.";
                break;

            case 13:
                textoTitulo.text =
                    "<align=center><size=150%><b>TIERRA</b></size></align>\n" +
                    "\n" +
                    "\n" +
                    "<b>Posición:</b> 3\n" +
                    "<b>Tipo:</b> Rocoso\n" +
                    "N° de Lunas: 1\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "- Es el único planeta conocido que alberga vida.\n" +
                    "- Casi tres cuartas partes de su superficie están cubiertas por agua.";
                break;

            case 14:
                textoTitulo.text =
                    "<align=center><size=150%><b>MARTE</b></size></align>\n" +
                    "\n" +
                    "\n" +
                    "<b>Posición:</b> 4\n" +
                    "<b>Tipo:</b> Rocoso\n" +
                    "N° de Lunas: 2\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "- Es conocido como el \"planeta rojo\" debido al óxido de hierro en su superficie.\n" +
                    "- Tiene el volcán más grande del sistema solar, el Monte Olimpo.";
                break;

            case 15:
                textoTitulo.text =
                    "<align=center><size=150%><b>JÚPITER</b></size></align>\n" +
                    "\n" +
                    "\n" +
                    "<b>Posición:</b> 5\n" +
                    "<b>Tipo:</b> Gaseoso\n" +
                    "N° de Lunas: 95\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "- Es el planeta más grande de todo el sistema solar.\n" +
                    "- Tiene una gran mancha roja que es una tormenta gigante.";
                break;

            case 16:
                textoTitulo.text =
                    "<align=center><size=150%><b>SATURNO</b></size></align>\n" +
                    "\n" +
                    "\n" +
                    "<b>Posición:</b> 6\n" +
                    "<b>Tipo:</b> Gaseoso\n" +
                    "N° de Lunas: 146\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "- Es famoso por su complejo y visible sistema de anillos.\n" +
                    "- Es el segundo planeta más grande del sistema solar.";
                break;

            case 17:
                textoTitulo.text =
                    "<align=center><size=150%><b>URANO</b></size></align>\n" +
                    "\n" +
                    "\n" +
                    "<b>Posición:</b> 7\n" +
                    "<b>Tipo:</b> Gigante helado\n" +
                    "N° de Lunas: 28\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "- Gira de lado, casi paralelo a su órbita.\n" +
                    "- Su color azul verdoso se debe al metano en su atmósfera.";
                break;

            case 18:
                textoTitulo.text =
                    "<align=center><size=150%><b>NEPTUNO</b></size></align>\n" +
                    "\n" +
                    "\n" +
                    "<b>Posición:</b> 8\n" +
                    "<b>Tipo:</b> Gigante helado\n" +
                    "N° de Lunas: 16\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "- Es el planeta más alejado del Sol.\n" +
                    "- Tiene vientos supersónicos extremadamente rápidos.";
                break;
        }
    }
    public void ocultarInfo()
    {
        //panelInfo.SetActive(false);
        botonAbrirInfo.SetActive(true);
        botonCerrarInfo.SetActive(false);

        animator.SetBool("InfoOn", false);
    }
}