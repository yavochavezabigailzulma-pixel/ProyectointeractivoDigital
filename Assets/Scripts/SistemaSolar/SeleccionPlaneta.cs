using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SeleccionPlaneta : MonoBehaviour
{
    public string nombrePlaneta;

    [Header("Zoom por planeta")]
    public float minZoomPlaneta = 3f;
    public float maxZoomPlaneta = 20f;

    [Header("Tutorial al seleccionar")]
    [Tooltip("Se activa la primera vez que el jugador selecciona ESTE planeta.")]
    [SerializeField] private HintSequencer secuenciaHintsAlSeleccionar;
    [SerializeField] private bool soloUnaVezPorSesion = true;

    private float tiempoPresion = 0f;
    public bool seleccionado = false;
    private System.Collections.Generic.List<GameObject> botonesInfoList = new();
    private Button botonComponent;

    private bool estabaSeleccionado;

    void Start()
    {
        // Obtener TODOS los botones hijos
        UISistemaSolar.Instance.panelInfoPlanetas.SetActive(false);
        Button[] botonesInfo = transform.parent.GetComponentsInChildren<Button>(true);

        foreach (Button boton in botonesInfo)
        {
            GameObject botonGO = boton.gameObject;
            botonGO.SetActive(false);

            // Limpiar EventTrigger previo
            EventTrigger triggerExistente = botonGO.GetComponent<EventTrigger>();
            if (triggerExistente != null) Destroy(triggerExistente);

            EventTrigger trigger = botonGO.AddComponent<EventTrigger>();

            EventTrigger.Entry down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            down.callback.AddListener(_ => {
                tiempoPresion = Time.time;
            });
            trigger.triggers.Add(down);

            EventTrigger.Entry up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            up.callback.AddListener(_ => {
                float duracion = Time.time - tiempoPresion;
                if (duracion < ZoomCamara.Instance.toleranciaPinch)
                    UISistemaSolar.Instance.MostrarInfo(nombrePlaneta);
            });
            trigger.triggers.Add(up);
        }

        // Guardar referencia a todos para activar/desactivar
        botonesInfoList = new System.Collections.Generic.List<GameObject>();
        foreach (Button b in botonesInfo)
            botonesInfoList.Add(b.gameObject);

        if (UISistemaSolar.Instance != null)
            UISistemaSolar.Instance.AlMostrarInfo += DetenerHintDeSeleccion;
    }

    void OnEnable()
    {
        // Se suscribe a la notificación de UISistemaSolar para cortar el hint
        // de selección en cuanto se abre el panel de info (evita que ambos
        // hints se muestren superpuestos).
        //if (UISistemaSolar.Instance != null)
        //    UISistemaSolar.Instance.AlMostrarInfo += DetenerHintDeSeleccion;
    }

    void OnDisable()
    {
        if (UISistemaSolar.Instance != null)
            UISistemaSolar.Instance.AlMostrarInfo -= DetenerHintDeSeleccion;
    }

    void DetenerHintDeSeleccion()
    {
        secuenciaHintsAlSeleccionar?.DetenerSecuencia();
    }

    private void Update()
    {
        seleccionado = (ZoomCamara.Instance.planetaSeguido == transform);

        if (seleccionado)
        {
            GetComponent<CapsuleCollider>().enabled = false;

            // Garantizar que el panel esté activo SIEMPRE que el planeta esté seleccionado,
            // no solo en la transición — cubre frames perdidos y estados inconsistentes
            UISistemaSolar.Instance.panelInfoPlanetas.SetActive(true);
        }
        else
        {
            GetComponent<CapsuleCollider>().enabled = true;
            foreach (var boton in botonesInfoList)
                boton.SetActive(false);

            if (estabaSeleccionado)
            {
                UISistemaSolar.Instance.panelInfoPlanetas.SetActive(false);

                // Se abandona la selección por completo: cerrar el panel SIN
                // disparar el hint de "cómo cerrar" (ese solo aplica cuando
                // el usuario cierra el panel manualmente y sigue seleccionado).
                //UISistemaSolar.Instance.CerrarPopupInfoSinHint();

                secuenciaHintsAlSeleccionar?.DetenerSecuencia();
            }
        }

        estabaSeleccionado = seleccionado;
    }

    void OnMouseDown()
    {
        tiempoPresion = Time.time;
    }

    void OnMouseUp()
    {
        float duracion = Time.time - tiempoPresion;
        if (duracion < ZoomCamara.Instance.toleranciaPinch && Input.touchCount < 2)
        {
            // Actualiza el planeta actual inmediatamente al seleccionar
            UISistemaSolar.Instance.SetPlanetaActual(nombrePlaneta);

            ZoomCamara.Instance.EnfocarEn(transform, minZoomPlaneta, maxZoomPlaneta);
            StartCoroutine(MostrarBotonInfo());

            DispararTutorialSiCorresponde();
        }
    }

    void DispararTutorialSiCorresponde()
    {
        if (secuenciaHintsAlSeleccionar == null) return;

        string clave = $"hint_seleccion_{nombrePlaneta}";

        if (soloUnaVezPorSesion && RegistroHintsSesion.EstaCompletado(clave))
            return;

        if (soloUnaVezPorSesion)
            RegistroHintsSesion.MarcarCompletado(clave);

        secuenciaHintsAlSeleccionar.IniciarSecuencia();
    }

    public float retrasoInfo = 1f; // ajustable en Inspector

    System.Collections.IEnumerator MostrarBotonInfo()
    {
        yield return new WaitForSeconds(retrasoInfo);
        foreach (var boton in botonesInfoList)
            boton.SetActive(true);
    }
}