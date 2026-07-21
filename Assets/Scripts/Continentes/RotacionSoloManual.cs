using UnityEngine;

public class RotacionSoloManual : MonoBehaviour
{
    [Header("Configuración")]
    public float sensibilidad = 0.3f;

    private Vector2 ultimaPosicion;

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
                ultimaPosicion = t.position;

            if (t.phase == TouchPhase.Moved)
            {
                Vector2 delta = t.position - ultimaPosicion;
                ultimaPosicion = t.position;

                // Yaw: alrededor del eje vertical del MUNDO.
                transform.Rotate(Vector3.up, -delta.x * sensibilidad, Space.World);

                // Pitch: alrededor del eje X del MUNDO, aplicado DESPUÉS del yaw
                // (Space.World, en ese orden). Ese orden es el que evita que el
                // giro se invierta al combinarse con el yaw acumulado.
                transform.Rotate(Vector3.right, delta.y * sensibilidad, Space.World);
            }
        }
    }
}