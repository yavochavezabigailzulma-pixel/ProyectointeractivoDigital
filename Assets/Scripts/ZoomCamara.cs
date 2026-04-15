using UnityEngine;

public class ZoomCamara : MonoBehaviour
{
    public float zoomSpeed = 0.1f;
    public float minZoom = 5f;
    public float maxZoom = 30f;

    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prev0 = t0.position - t0.deltaPosition;
            Vector2 prev1 = t1.position - t1.deltaPosition;

            float prevDist = (prev0 - prev1).magnitude;
            float currDist = (t0.position - t1.position).magnitude;

            float diff = currDist - prevDist;

            Vector3 pos = transform.position;
            pos.z -= diff * zoomSpeed * Time.deltaTime;
            pos.z = Mathf.Clamp(pos.z, -maxZoom, -minZoom);

            transform.position = pos;
        }
    }
}