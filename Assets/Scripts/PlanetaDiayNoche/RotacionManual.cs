using UnityEngine;

public class RotacionManual : MonoBehaviour
{
    [Header("Configuración")]
    public float sensibilidad = 0.3f;
    public bool manual = true; // el botón externo cambia este valor

    private RotarPropio rotarPropio;
    private Vector2 ultimaPosicion;

    void Start()
    {
        rotarPropio = GetComponent<RotarPropio>();
    }

    void Update()
    {
        // En lugar de desactivar el componente, pausa la velocidad
        rotarPropio.multiplicadorVel = manual ? 0f : 1f;

        if (!manual) return;

        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
                ultimaPosicion = t.position;

            if (t.phase == TouchPhase.Moved)
            {
                Vector2 delta = t.position - ultimaPosicion;
                ultimaPosicion = t.position;
                transform.Rotate(Vector3.up, -delta.x * sensibilidad, Space.World);
            }
        }
    }

    public void SetManual()
    {
        if (manual)
            manual = false;
        else manual = true;
    }
}