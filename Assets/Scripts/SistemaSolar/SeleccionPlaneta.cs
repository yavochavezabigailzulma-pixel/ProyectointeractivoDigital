using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SeleccionPlaneta : MonoBehaviour
{
    [Header("Botones de volver (se intercambian según selección)")]
    [Tooltip("Botón visible por defecto: lleva al menú principal.")]
    [SerializeField] private GameObject botonVolverMenu;
    [Tooltip("Botón visible mientras hay un planeta seleccionado: deselecciona en vez de ir al menú.")]
    [SerializeField] private GameObject botonVolverDeseleccion;

    [Header("Secuencia principal: pinch -> swipe -> tap")]
    [SerializeField] private HintSequencer hintSequencer;
    [SerializeField] private GameObject hintTapEsperado;

    [Header("Secuencia encadenada (arranca al terminar la de arriba)")]
    [SerializeField] private HintSequencer hintSequenceAlSeleccionar;

    public string nombrePlaneta;

    [Header("Zoom por planeta")]
    public float minZoomPlaneta = 3f;
    public float maxZoomPlaneta = 20f;

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

    void OnDisable()
    {
        if (UISistemaSolar.Instance != null)
            UISistemaSolar.Instance.AlMostrarInfo -= DetenerHintDeSeleccion;

    }

    void DetenerHintDeSeleccion()
    {
        hintSequencer?.DetenerSecuencia(); // este sí se corta, no se completa

        if (hintSequenceAlSeleccionar != null)
        {
            bool completado = hintSequenceAlSeleccionar.CompletarPasoActual();
            Debug.Log(completado
                ? $"[Hint] Paso completado correctamente en '{hintSequenceAlSeleccionar.name}' (al abrir panel de info)."
                : $"[Hint] CompletarPasoActual() NO tuvo efecto en '{hintSequenceAlSeleccionar.name}' (¿ya estaba detenida o sin hint activo?)");
        }
    }
    private void Update()
    {
        seleccionado = (ZoomCamara.Instance.planetaSeguido == transform);

        if (seleccionado)
        {
            GetComponent<CapsuleCollider>().enabled = false;

            if (botonVolverMenu != null) botonVolverMenu.SetActive(false);
            if (botonVolverDeseleccion != null) botonVolverDeseleccion.SetActive(true);
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

                UISistemaSolar.Instance.CerrarPopupInfoSinHint();

                hintSequencer?.DetenerSecuencia(); // este sí se corta

                if (hintSequenceAlSeleccionar != null)
                {
                    bool completado = hintSequenceAlSeleccionar.CompletarPasoActual();
                    Debug.Log(completado
                        ? $"[Hint] Paso completado correctamente en '{hintSequenceAlSeleccionar.name}' (al deseleccionar)."
                        : $"[Hint] CompletarPasoActual() NO tuvo efecto en '{hintSequenceAlSeleccionar.name}' (¿ya estaba detenida o sin hint activo?)");
                }

                if (botonVolverMenu != null) botonVolverMenu.SetActive(true);
                if (botonVolverDeseleccion != null) botonVolverDeseleccion.SetActive(false);
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
            UISistemaSolar.Instance.SetPlanetaActual(nombrePlaneta);
            ZoomCamara.Instance.EnfocarEn(transform, minZoomPlaneta, maxZoomPlaneta);
            StartCoroutine(MostrarBotonInfo());

            bool completado = hintSequencer.CompletarPaso(hintTapEsperado);
            Debug.Log(completado
                ? $"[Hint] Paso 3 (tap) completado correctamente en '{hintSequencer.name}'."
                : $"[Hint] Tap detectado pero NO era el paso activo en '{hintSequencer.name}'.");

            // En el MISMO instante, arranca la secuencia independiente en paralelo
            if (hintSequenceAlSeleccionar != null)
            {
                hintSequenceAlSeleccionar.IniciarSecuencia();
                Debug.Log($"[Hint] '{hintSequenceAlSeleccionar.name}' iniciada en paralelo al tocar el planeta.");
            }
            else
            {
                Debug.LogWarning("[Hint] hintSequenceAlSeleccionar no está asignado en el Inspector.", this);
            }

        }
    }

    public float retrasoInfo = 1f; // ajustable en Inspector

    System.Collections.IEnumerator MostrarBotonInfo()
    {
        yield return new WaitForSeconds(retrasoInfo);
        foreach (var boton in botonesInfoList)
            boton.SetActive(true);
    }
}