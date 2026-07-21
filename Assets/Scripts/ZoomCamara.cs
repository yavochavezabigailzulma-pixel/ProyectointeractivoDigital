using UnityEngine;

public class ZoomCamara : MonoBehaviour
{
    [Header("Zoom global")]
    public float zoomSpeed = 0.05f;
    public float minZoom = 5f;
    public float maxZoom = 60f;

    [Header("Deslizamiento")]
    public float dragSpeed = 0.012f;

    [Header("Suavizado")]
    public float smoothTime = 0.12f;

    [Header("Enfoque en planeta")]
    public float duracionEnfoque = 1f;
    public float toleranciaPinch = 0.2f;

    public static ZoomCamara Instance;

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;
    private Vector3 focusTarget = Vector3.zero;

    public Transform planetaSeguido = null;
    private float minZoomActual;
    private float maxZoomActual;
    private float zoomActual;

    private bool enfoqueCompletado = false;
    private bool animando = false;

    private Vector3 posicionInicial;
    private bool esperandoSoltarDedos = false;
    private float distanciaLibre;
    void Awake() => Instance = this;

    void Start()
    {
        targetPosition = transform.position;
        posicionInicial = transform.position;
        minZoomActual = minZoom;
        maxZoomActual = maxZoom;

        distanciaLibre = Vector3.Distance(transform.position, Vector3.zero);
    }

    Vector3 PuntoMira => focusTarget - Vector3.up * 0.25f;
    void Update()
    {
        if (Input.touchCount == 0) esperandoSoltarDedos = false;
        if (Input.touchCount == 1) ManejarDrag();
        if (Input.touchCount == 2)
        {
            if (!esperandoSoltarDedos) ManejarZoom();
        }

        if (planetaSeguido != null && !animando)
        {
            SeguirPlaneta();

            // Corrector global de polo (igual que ZoomCuerpos)
            float limite = 0.8f;
            Vector3 dir = (transform.position - focusTarget).normalized;
            if (dir.y < -limite || dir.y > limite)
            {
                dir.y = Mathf.Clamp(dir.y, -limite, limite);
                transform.position = focusTarget + dir.normalized * zoomActual;
                targetPosition = transform.position;
            }
        }
        // Corrector global de polo en espacio libre
        if (planetaSeguido == null && !animando)
        {
            float limite = 0.8f;
            Vector3 dir = transform.position.normalized;
            if (dir.y < -limite || dir.y > limite)
            {
                dir.y = Mathf.Clamp(dir.y, -limite, limite);
                float distancia = Vector3.Distance(transform.position, Vector3.zero);
                transform.position = dir.normalized * distancia;
                targetPosition = transform.position;
            }
        }
        if (!animando)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position, targetPosition, ref velocity, smoothTime);
            transform.LookAt(PuntoMira);

            if (planetaSeguido == null)
            {
                if (Input.touchCount == 2)
                    distanciaLibre = Vector3.Distance(transform.position, Vector3.zero);
                else
                {
                    transform.position = transform.position.normalized * distanciaLibre;
                    targetPosition = transform.position;
                }
            }
        }
    }
    void ManejarDrag()
    {
        Touch t = Input.GetTouch(0);
        if (t.phase != TouchPhase.Moved) return;

        if (enfoqueCompletado && planetaSeguido != null)
        {
            // Rotación horizontal libre
            transform.RotateAround(focusTarget, Vector3.up, t.deltaPosition.x * dragSpeed * 50f);

            // Simula rotación vertical antes de aplicar para verificar límite
            float limite = 0.8f;
            float deltaY = -t.deltaPosition.y * dragSpeed * 50f;
            Quaternion rotSimulada = Quaternion.AngleAxis(deltaY, transform.right);
            Vector3 posSimulada = focusTarget + rotSimulada * (transform.position - focusTarget);
            Vector3 dirSimulada = (posSimulada - focusTarget).normalized;

            if (dirSimulada.y >= -limite && dirSimulada.y <= limite)
                transform.RotateAround(focusTarget, transform.right, deltaY);

            transform.position = focusTarget + (transform.position - focusTarget).normalized * zoomActual;
            targetPosition = transform.position;
        }
        else
        {
            // Deslizamiento libre — igual que antes
            float distanciaActual = Vector3.Distance(transform.position, Vector3.zero);
            float factor = distanciaActual * dragSpeed;

            targetPosition += -transform.right * t.deltaPosition.x * factor
                            + transform.up * -t.deltaPosition.y * factor;

            targetPosition = targetPosition.normalized * distanciaActual;
        }
    }
    void ManejarZoom()
    {
        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);

        float prevDist = ((t0.position - t0.deltaPosition) - (t1.position - t1.deltaPosition)).magnitude;
        float currDist = (t0.position - t1.position).magnitude;
        float diff = currDist - prevDist;
        float factor = Vector3.Distance(transform.position, Vector3.zero) * zoomSpeed * Time.deltaTime;

        // Al iniciar pinch sobre un planeta, abandona el modo órbita
        if (enfoqueCompletado && planetaSeguido != null)
        {
            zoomActual = Vector3.Distance(transform.position, focusTarget);
            enfoqueCompletado = false;
        }

        targetPosition += transform.forward * diff * factor;

        // Aplica límites
        float dist = Vector3.Distance(targetPosition, focusTarget);
        if (dist > maxZoomActual)
        {
            if (planetaSeguido != null)
            {
                // Supera el máximo: desenfoca y vuelve al sistema solar
                EnfocarEn(null);
                //UISistemaSolar.Instance.CerrarPopupInfoSinHint(); // corta hintSequenceAlCerrarPanel sin volver a lanzarlo
                targetPosition = posicionInicial;
                esperandoSoltarDedos = true;
            }
            else
            {
                targetPosition = targetPosition.normalized * maxZoomActual;
            }
        }
        if (dist < minZoomActual)
            targetPosition = (targetPosition - focusTarget).normalized * minZoomActual + focusTarget;
    }

    void SeguirPlaneta()
    {
        focusTarget = planetaSeguido.position;
        Vector3 posicionDeseada = focusTarget + (transform.position - focusTarget).normalized * zoomActual;

        if (!enfoqueCompletado)
        {
            // Esperando que la animación termine
            if (Mathf.Abs(Vector3.Distance(transform.position, focusTarget) - maxZoomActual) > 0.5f)
                enfoqueCompletado = true;
            transform.position = posicionDeseada;
        }
        else
        {
            // Mantiene la distancia al planeta cada frame
            targetPosition = posicionDeseada;
        }
    }

    public void EnfocarEn(Transform planetaTransform, float minLocal = -1f, float maxLocal = -1f)
    {
        planetaSeguido = planetaTransform;

        if (planetaTransform == null)
        {
            transform.SetParent(null);
            focusTarget = Vector3.zero;
            minZoomActual = minZoom;
            maxZoomActual = maxZoom;
            return;
        }
        focusTarget = planetaTransform.position;
        transform.SetParent(planetaTransform.parent);

        minZoomActual = minLocal >= 0 ? minLocal : minZoom;
        maxZoomActual = maxLocal >= 0 ? maxLocal : maxZoom;
        zoomActual = minZoomActual;
        enfoqueCompletado = false;

        Vector3 destino = focusTarget + (transform.position - focusTarget).normalized * minZoomActual;
        targetPosition = destino;

        StopAllCoroutines();
        StartCoroutine(AnimarEnfoque(destino));
    }

    System.Collections.IEnumerator AnimarEnfoque(Vector3 destino)
    {
        animando = true;
        velocity = Vector3.zero;

        Vector3 origen = transform.position;
        Quaternion rotacionOrigen = transform.rotation;
        float tiempo = 0f;

        while (tiempo < duracionEnfoque)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.Clamp01(tiempo / duracionEnfoque);
            t = 1f - (1f - t) * (1f - t); // easeOut

            // Actualiza destino si el planeta se mueve
            if (planetaSeguido != null)
            {
                focusTarget = planetaSeguido.position;
                destino = focusTarget + (origen - focusTarget).normalized * zoomActual;
            }

            transform.position = Vector3.Lerp(origen, destino, t);
            transform.rotation = Quaternion.Slerp(rotacionOrigen,
                Quaternion.LookRotation(PuntoMira - transform.position), t);

            yield return null;
        }

        transform.position = destino;
        targetPosition = destino;
        animando = false;
        enfoqueCompletado = true;
    }
}