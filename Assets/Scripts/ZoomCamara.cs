using UnityEngine;

public class ZoomCamara : MonoBehaviour
{
    [Header("Zoom")]
    public float zoomSpeed = 0.3f;
    public float minZoom = 5f;
    public float maxZoom = 60f;

    [Header("Deslizamiento")]
    public float dragSpeed = 0.05f;

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Moved)
            {
                float moveX = -t.deltaPosition.x * dragSpeed;
                float moveY = -t.deltaPosition.y * dragSpeed; //vertical mueve en Y
                transform.position += new Vector3(moveX, moveY, 0);
            }
        }

        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prev0 = t0.position - t0.deltaPosition;
            Vector2 prev1 = t1.position - t1.deltaPosition;

            float prevDist = (prev0 - prev1).magnitude;
            float currDist = (t0.position - t1.position).magnitude;

            float diff = (currDist - prevDist) * zoomSpeed * Time.deltaTime;

            transform.position += transform.forward * diff;

            float dist = Vector3.Distance(transform.position, Vector3.zero);
            if (dist < minZoom)
                transform.position = transform.position.normalized * minZoom;
            if (dist > maxZoom)
                transform.position = transform.position.normalized * maxZoom;
        }
    }
}