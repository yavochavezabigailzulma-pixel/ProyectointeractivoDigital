using System.Globalization;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISistemaSolar : MonoBehaviour
{
    public EventReference musicaSistemaSolar;
    EventInstance musicaSistemaSolarInstance;

    [Header("Tutorial al cerrar panel de información")]
    [Tooltip("Su paso actual se completa cuando el jugador cierra el panel " +
             "manualmente (botón volver) mientras sigue en modo selección.")]
    [SerializeField] private HintSequencer hintSequenceAlCerrarPanel;

    [Tooltip("Se invoca apenas se muestra el panel de info, para que cualquier hint de selección en curso se corte.")]
    public System.Action AlMostrarInfo;

    public GameObject panelInfoPlanetas;
    public TextMeshProUGUI textoTitulo;
    public TextMeshProUGUI textoInfo;

    public GameObject botonInfoDesplegable;
    public GameObject botonVolverDesplegable;

    private string planetaActual;

    public Animator animator;

    public EventReference clicVolver;
    public EventReference abrirDeslizable;
    public EventReference cerrarDeslizable;

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
    }

    private void Start()
    {
        if (!musicaSistemaSolar.IsNull)
            musicaSistemaSolarInstance = AudioManager.Instance.CreateLoop(musicaSistemaSolar);
    }

    void OnDestroy()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopLoop(musicaSistemaSolarInstance);
    }

    public void MostrarInfo(string planeta)
    {
        planetaActual = planeta;
        panelInfoPlanetas.SetActive(true);

        AudioManager.Instance.Play(abrirDeslizable);
        // Avisa a quien esté escuchando (por ejemplo, el SeleccionPlaneta activo)
        // para que corte cualquier hint de selección que siguiera en curso.
        AlMostrarInfo?.Invoke();

        animator.Rebind();
        animator.Update(0f);
        animator.SetBool("InfoOn", true);

        botonInfoDesplegable.SetActive(false);
        botonVolverDesplegable.SetActive(true);

        switch (planeta)
        {
            case "Mercurio":
                textoTitulo.text =
                    "<align=center><size=175%><b>MERCURIO</b></size></align>\n" +
                    "<line-height=25%>\n</line-height>" +
                    "<b>Posición:</b> 1\n" +
                    "<b>Tipo:</b> Rocoso\n" +
                    "N° de Lunas: Ninguna\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "- Es el planeta más cercano al Sol.\n" +
                    "- Tiene temperaturas muy extremas.";
                break;

            case "Venus":
                textoTitulo.text =
                    "<align=center><size=175%><b>VENUS</b></size></align>\n" +
                    "<line-height=25%>\n</line-height>" +
                    "<b>Posición:</b> 2\n" +
                    "<b>Tipo:</b> Rocoso\n" +
                    "N° de Lunas: Ninguna\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "- Tiene una atmósfera densa que atrapa el calor, lo que lo hace el planeta más caliente.\n" +
                    "- Gira en sentido contrario a la mayoría de los planetas.";
                break;

            case "Tierra":
                textoTitulo.text =
                    "<align=center><size=175%><b>TIERRA</b></size></align>\n" +
                    "<line-height=25%>\n</line-height>" +
                    "<b>Posición:</b> 3\n" +
                    "<b>Tipo:</b> Rocoso\n" +
                    "N° de Lunas: 1\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "- Es el único planeta conocido que alberga vida.\n" +
                    "- Casi tres cuartas partes de su superficie están cubiertas por agua.";
                break;

            case "Marte":
                textoTitulo.text =
                    "<align=center><size=175%><b>MARTE</b></size></align>\n" +
                    "<line-height=25%>\n</line-height>" +
                    "<b>Posición:</b> 4\n" +
                    "<b>Tipo:</b> Rocoso\n" +
                    "N° de Lunas: 2\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "- Es conocido como el \"planeta rojo\" debido al óxido de hierro en su superficie.\n" +
                    "- Tiene el volcán más grande del sistema solar, el Monte Olimpo.";
                break;

            case "Júpiter":
                textoTitulo.text =
                    "<align=center><size=175%><b>JÚPITER</b></size></align>\n" +
                    "<line-height=25%>\n</line-height>" +
                    "<b>Posición:</b> 5\n" +
                    "<b>Tipo:</b> Gaseoso\n" +
                    "N° de Lunas: 95\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "- Es el planeta más grande de todo el sistema solar.\n" +
                    "- Tiene una gran mancha roja que es una tormenta gigante.";
                break;

            case "Saturno":
                textoTitulo.text =
                    "<align=center><size=175%><b>SATURNO</b></size></align>\n" +
                    "<line-height=25%>\n</line-height>" +
                    "<b>Posición:</b> 6\n" +
                    "<b>Tipo:</b> Gaseoso\n" +
                    "N° de Lunas: 146\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "- Es famoso por su complejo y visible sistema de anillos.\n" +
                    "- Es el segundo planeta más grande del sistema solar.";
                break;

            case "Urano":
                textoTitulo.text =
                    "<align=center><size=175%><b>URANO</b></size></align>\n" +
                    "<line-height=25%>\n</line-height>" +
                    "<b>Posición:</b> 7\n" +
                    "<b>Tipo:</b> Gigante helado\n" +
                    "N° de Lunas: 28\n";

                textoInfo.text =
                    "<b>Curiosidades:</b>\n" +
                    "- Gira de lado, casi paralelo a su órbita.\n" +
                    "- Su color azul verdoso se debe al metano en su atmósfera.";
                break;

            case "Neptuno":
                textoTitulo.text =
                    "<align=center><size=175%><b>NEPTUNO</b></size></align>\n" +
                    "<line-height=25%>\n</line-height>" +
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

    /// <summary>
    /// Cierra el panel de info manualmente (botón "volver"), mientras el
    /// planeta sigue seleccionado. Inicia la secuencia de "cómo cerrar"
    /// desde cero — usar solo aquí, nunca al abandonar la selección.
    /// </summary>
    public void OcultarPopupInfo()
    {
        animator.SetBool("InfoOn", false);

        botonInfoDesplegable.SetActive(true);
        botonVolverDesplegable.SetActive(false);

        hintSequenceAlCerrarPanel?.IniciarSecuencia();

        AudioManager.Instance.Play(cerrarDeslizable);
    }

    /// <summary>
    /// Cierra el panel de info SIN iniciar ningún hint nuevo, y corta
    /// cualquier hint de "cómo cerrar" que estuviera en curso.
    /// Usar cuando el usuario abandona la selección del planeta por completo.
    /// </summary>
    public void CerrarPopupInfoSinHint()
    {
        animator.SetBool("InfoOn", false);

        botonInfoDesplegable.SetActive(true);
        botonVolverDesplegable.SetActive(false);

        AudioManager.Instance.Play(clicVolver);

        if (hintSequenceAlCerrarPanel != null)
        {
            bool completado = hintSequenceAlCerrarPanel.CompletarPasoActual();
            Debug.Log(completado
                ? $"[Hint] Paso completado correctamente en '{hintSequenceAlCerrarPanel.name}' (al cerrar panel sin hint)."
                : $"[Hint] CompletarPasoActual() NO tuvo efecto en '{hintSequenceAlCerrarPanel.name}' (¿ya estaba detenida o sin hint activo?)");
        }
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
    }
}