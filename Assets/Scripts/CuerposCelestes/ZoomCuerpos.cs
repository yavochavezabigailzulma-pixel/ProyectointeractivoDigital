using UnityEngine;

public class ZoomCuerpos : MonoBehaviour
{
    [Header("Zoom")]
    public float zoomSpeed = 0.05f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    [Header("Deslizamiento")]
    public float dragSpeed = 0.3f;

    public static ZoomCuerpos Instance;

    private Transform objetivo = null;
    private float distanciaActual;

    void Awake() => Instance = this;

    void Update()
    {
        if (objetivo == null) return;

        if (Input.touchCount == 1) ManejarDrag();
        if (Input.touchCount == 2) ManejarZoom();

        // Mantiene la cámara mirando siempre al objeto
        transform.LookAt(objetivo);
    }

    // 1 dedo: orbitar alrededor del objeto
    void ManejarDrag()
    {
        Touch t = Input.GetTouch(0);
        if (t.phase != TouchPhase.Moved) return;

        transform.RotateAround(objetivo.position, Vector3.up, t.deltaPosition.x * dragSpeed);
        transform.RotateAround(objetivo.position, transform.right, -t.deltaPosition.y * dragSpeed);

        // Preserva la distancia fija al objeto
        transform.position = objetivo.position + (transform.position - objetivo.position).normalized * distanciaActual;
    }

    // 2 dedos: zoom con pinch
    void ManejarZoom()
    {
        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);

        float prevDist = ((t0.position - t0.deltaPosition) - (t1.position - t1.deltaPosition)).magnitude;
        float currDist = (t0.position - t1.position).magnitude;
        float diff = currDist - prevDist;

        distanciaActual -= diff * zoomSpeed;
        distanciaActual = Mathf.Clamp(distanciaActual, minZoom, maxZoom);

        // Aplica el zoom manteniendo la dirección actual
        transform.position = objetivo.position + (transform.position - objetivo.position).normalized * distanciaActual;
    }

    // Asigna el objeto central
    public void SetObjetivo(Transform nuevoObjetivo)
    {
        objetivo = nuevoObjetivo;

        if (objetivo == null) return;

        // Inicializa la distancia actual al activar el objeto
        distanciaActual = Vector3.Distance(transform.position, objetivo.position);
        distanciaActual = Mathf.Clamp(distanciaActual, minZoom, maxZoom);
    }
}