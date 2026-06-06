using System.Globalization;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISistemaSolar : MonoBehaviour
{

    public GameObject panelInfoPlanetas;
    public TextMeshProUGUI textoTitulo;
    public TextMeshProUGUI textoInfo;

    public GameObject botonInfoDesplegable;
    public GameObject botonVolverDesplegable;

    private string planetaActual;

    public Animator animator;
    //[Header("Sprites Planetas")]  // NUEVO: asigna cada sprite en el Inspector
    //public Sprite spriteMercurio;
    //public Sprite spriteVenus;
    //public Sprite spriteTierra;
    //public Sprite spriteMarte;
    //public Sprite spriteJupiter;
    //public Sprite spriteSaturno;
    //public Sprite spriteUrano;
    //public Sprite spriteNeptuno;
    //public Sprite spriteDefault;

    //public Image imagenPlaneta;

    public EventReference clicVolver;

    public static UISistemaSolar Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //animator.SetBool("InfoOn", false);
    }
    public void MostrarInfo(string planeta)
    {
        planetaActual = planeta;
        // Activar primero, luego animar
        panelInfoPlanetas.SetActive(true);

        // Forzar reset del Animator por si quedó en estado sucio
        animator.Rebind();
        animator.Update(0f);

        animator.SetBool("InfoOn", true);

        botonInfoDesplegable.SetActive(false);
        botonVolverDesplegable.SetActive(true);

        switch (planeta)
        {
            case "Mercurio":
                textoTitulo.text =
                    "<align=center><size=150%><b>MERCURIO</b></size></align>\n" +
                    "\n" +
                    "<b>Posición:</b> 1\n" +
                    "<b>Tipo:</b> Rocoso\n" +
                    "N° de Lunas: Ninguna\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "   - Es el planeta más cercano al Sol.\n" +
                    "   - Tiene temperaturas muy extremas.";
                //imagenPlaneta.sprite = spriteMercurio;
                break;

            case "Venus":
                textoTitulo.text =
                    "<align=center><size=150%><b>VENUS</b></size></align>\n" +
                    "\n" +
                    "<b>Posición:</b> 2\n" +
                    "<b>Tipo:</b> Rocoso\n" +
                    "N° de Lunas: Ninguna\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "   - Tiene una atmósfera densa que atrapa el calor, lo que lo hace el planeta más caliente.\n" +
                    "   - Gira en sentido contrario a la mayoría de los planetas.";
                //imagenPlaneta.sprite = spriteVenus;
                break;

            case "Tierra":
                textoTitulo.text =
                    "<align=center><size=150%><b>TIERRA</b></size></align>\n" +
                    "\n" +
                    "<b>Posición:</b> 3\n" +
                    "<b>Tipo:</b> Rocoso\n" +
                    "N° de Lunas: 1\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "   - Es el único planeta conocido que alberga vida.\n" +
                    "   - Casi tres cuartas partes de su superficie están cubiertas por agua.";
                //imagenPlaneta.sprite = spriteTierra;
                break;

            case "Marte":
                textoTitulo.text =
                    "<align=center><size=150%><b>MARTE</b></size></align>\n" +
                    "\n" +
                    "<b>Posición:</b> 4\n" +
                    "<b>Tipo:</b> Rocoso\n" +
                    "N° de Lunas: 2\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "   - Es conocido como el \"planeta rojo\" debido al óxido de hierro en su superficie.\n" +
                    "   - Tiene el volcán más grande del sistema solar, el Monte Olimpo.";
                //imagenPlaneta.sprite = spriteMarte;
                break;

            case "Júpiter":
                textoTitulo.text =
                    "<align=center><size=150%><b>JÚPITER</b></size></align>\n" +
                    "\n" +
                    "<b>Posición:</b> 5\n" +
                    "<b>Tipo:</b> Gaseoso\n" +
                    "N° de Lunas: 95\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "   - Es el planeta más grande de todo el sistema solar.\n" +
                    "   - Tiene una gran mancha roja que es una tormenta gigante.";
                //imagenPlaneta.sprite = spriteJupiter;
                break;

            case "Saturno":
                textoTitulo.text =
                    "<align=center><size=150%><b>SATURNO</b></size></align>\n" +
                    "\n" +
                    "<b>Posición:</b> 6\n" +
                    "<b>Tipo:</b> Gaseoso\n" +
                    "N° de Lunas: 146\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "   - Es famoso por su complejo y visible sistema de anillos.\n" +
                    "   - Es el segundo planeta más grande del sistema solar.";
                //imagenPlaneta.sprite = spriteSaturno;
                break;

            case "Urano":
                textoTitulo.text =
                    "<align=center><size=150%><b>URANO</b></size></align>\n" +
                    "\n" +
                    "<b>Posición:</b> 7\n" +
                    "<b>Tipo:</b> Gigante helado\n" +
                    "N° de Lunas: 28\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "   - Gira de lado, casi paralelo a su órbita.\n" +
                    "   - Su color azul verdoso se debe al metano en su atmósfera.";
                //imagenPlaneta.sprite = spriteUrano;
                break;

            case "Neptuno":
                textoTitulo.text =
                    "<align=center><size=150%><b>NEPTUNO</b></size></align>\n" +
                    "\n" +
                    "<b>Posición:</b> 8\n" +
                    "<b>Tipo:</b> Gigante helado\n" +
                    "N° de Lunas: 16\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "   - Es el planeta más alejado del Sol.\n" +
                    "   - Tiene vientos supersónicos extremadamente rápidos.";
                //imagenPlaneta.sprite = spriteNeptuno;
                break;

            //default:
            //    textoInfo.text = "Información no disponible.";
            //    //imagenPlaneta.sprite = spriteDefault;
            //    panelInfoPlanetas.SetActive(false);
            //    break;
        }
    }
    public void OcultarPopupInfo()
    {
        animator.SetBool("InfoOn", false);

        botonInfoDesplegable.SetActive(true);
        botonVolverDesplegable.SetActive(false);

        AudioManager.Instance.Play(clicVolver);
    }
    public void AbrirInfoActual()
    {
        if (string.IsNullOrEmpty(planetaActual))
            return;

        MostrarInfo(planetaActual);
    }
    public void SetPlanetaActual(string planeta)
    {
        planetaActual = planeta;
        Debug.Log($"[SetPlanetaActual] actualizado a: '{planeta}'");
    }
}
