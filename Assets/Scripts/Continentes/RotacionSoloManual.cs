using UnityEngine;
using FMODUnity;

public class RotacionSoloManual : MonoBehaviour
{
    [Header("Sonidos")]
    public EventReference clicMatraca;

    [Header("Configuración")]
    public float sensibilidad = 0.3f;

    [Header("Matraca")]
    [Tooltip("Grados (combinando yaw y pitch) que hay que girar para que suene un clic.")]
    public float gradosPorClic = 5f;

    [Header("Tutorial")]
    [SerializeField] private HintSequencer secuenciaHintsAlSeleccionar;
    [SerializeField] private GameObject hintSwipeEsperado;
    private bool swipeYaNotificadoEsteGesto = false;

    private Vector2 ultimaPosicion;
    private float gradosAcumulados = 0f;

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                ultimaPosicion = t.position;
                gradosAcumulados = 0f;
                swipeYaNotificadoEsteGesto = false;
            }
            if (t.phase == TouchPhase.Moved)
            {
                Vector2 delta = t.position - ultimaPosicion;
                ultimaPosicion = t.position;

                if (!swipeYaNotificadoEsteGesto && delta.sqrMagnitude > 4f)
                {
                    swipeYaNotificadoEsteGesto = true;
                    secuenciaHintsAlSeleccionar.CompletarPaso(hintSwipeEsperado);
                }

                float gradosYaw = -delta.x * sensibilidad;
                float gradosPitch = delta.y * sensibilidad;
                // Yaw: alrededor del eje vertical del MUNDO.
                transform.Rotate(Vector3.up, gradosYaw, Space.World);
                // Pitch: alrededor del eje X del MUNDO, aplicado DESPUÉS del yaw
                // (Space.World, en ese orden). Ese orden es el que evita que el
                // giro se invierta al combinarse con el yaw acumulado.
                transform.Rotate(Vector3.right, gradosPitch, Space.World);
                // Magnitud combinada del giro real (diagonal incluida),
                // no solo el eje horizontal.
                float gradosEsteFrame = new Vector2(gradosYaw, gradosPitch).magnitude;
                gradosAcumulados += gradosEsteFrame;
                while (gradosAcumulados >= gradosPorClic)
                {
                    AudioManager.Instance.Play(clicMatraca);
                    gradosAcumulados -= gradosPorClic;
                }
            }
        }
    }
}