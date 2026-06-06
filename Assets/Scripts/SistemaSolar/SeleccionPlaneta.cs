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
    private GameObject botonInfo;
    private Button botonComponent;

    private bool estabaSeleccionado;
    void Start()
    {
        botonComponent = transform.parent.GetComponentInChildren<Button>(true);
        botonInfo = botonComponent.gameObject;
        botonInfo.SetActive(false);

        // Reemplaza onClick por detección manual de presión
        EventTrigger trigger = botonInfo.AddComponent<EventTrigger>();

        EventTrigger.Entry down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        down.callback.AddListener(_ => tiempoPresion = Time.time);
        trigger.triggers.Add(down);

        EventTrigger.Entry up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        up.callback.AddListener(_ => {
            if (Time.time - tiempoPresion < ZoomCamara.Instance.toleranciaPinch)
                UISistemaSolar.Instance.MostrarInfo(nombrePlaneta);
        });
        trigger.triggers.Add(up);
    }
    void OnMouseDown()
    {
        tiempoPresion = Time.time;
        Debug.Log("Touch detectado");
    }
    private void Update()
    {
        seleccionado = (ZoomCamara.Instance.planetaSeguido == transform);

        if (seleccionado)
        {
            GetComponent<CapsuleCollider>().enabled = false;

            if (!estabaSeleccionado)
            {
                UISistemaSolar.Instance.panelInfoPlanetas.SetActive(true);
            }
        }
        else
        {
            GetComponent<CapsuleCollider>().enabled = true;
            botonInfo.SetActive(false);

            if (estabaSeleccionado)
            {
                UISistemaSolar.Instance.panelInfoPlanetas.SetActive(false);
            }
        }

        estabaSeleccionado = seleccionado;
    }
    void OnMouseUp()
    {
        float duracion = Time.time - tiempoPresion;
        Debug.Log("Duracion del touch: " + duracion + ". Umbral: " + ZoomCamara.Instance.toleranciaPinch);
        if (duracion < ZoomCamara.Instance.toleranciaPinch && Input.touchCount < 2)
        {
            ZoomCamara.Instance.EnfocarEn(transform, minZoomPlaneta, maxZoomPlaneta);
            StartCoroutine(MostrarBotonInfo());
        }
    }

    public float retrasoInfo = 1f; // ajustable en Inspector

    System.Collections.IEnumerator MostrarBotonInfo()
    {
        yield return new WaitForSeconds(retrasoInfo);
        botonInfo.SetActive(true);
        //UIManager.Instance.MostrarInfo(nombrePlaneta);
    }
}