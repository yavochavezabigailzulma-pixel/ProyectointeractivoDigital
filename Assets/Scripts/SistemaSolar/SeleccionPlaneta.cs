using UnityEditor.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SeleccionPlaneta : MonoBehaviour
{
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
        Button[] botonesInfo = transform.parent.GetComponentsInChildren<Button>(true);
        Debug.Log($"[{nombrePlaneta}] Botones encontrados: {botonesInfo.Length}");

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
                Debug.Log($"[{nombrePlaneta}] PointerDown en {botonGO.name}");
                tiempoPresion = Time.time;
            });
            trigger.triggers.Add(down);

            EventTrigger.Entry up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            up.callback.AddListener(_ => {
                float duracion = Time.time - tiempoPresion;
                Debug.Log($"[{nombrePlaneta}] PointerUp en {botonGO.name} | duracion: {duracion}");
                if (duracion < ZoomCamara.Instance.toleranciaPinch)
                    UISistemaSolar.Instance.MostrarInfo(nombrePlaneta);
            });
            trigger.triggers.Add(up);

            Debug.Log($"[{nombrePlaneta}] Listeners asignados a: {botonGO.name}");
        }

        // Guardar referencia a todos para activar/desactivar
        botonesInfoList = new System.Collections.Generic.List<GameObject>();
        foreach (Button b in botonesInfo)
            botonesInfoList.Add(b.gameObject);
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
                // Ocultar también el panel de info desplegado al deseleccionar
                UISistemaSolar.Instance.OcultarPopupInfo();
            }
        }

        estabaSeleccionado = seleccionado;
    }
    void OnMouseDown()
    {
        tiempoPresion = Time.time;
        Debug.Log("Touch detectado");
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