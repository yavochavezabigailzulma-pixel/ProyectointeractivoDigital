using UnityEngine;
using UnityEngine.EventSystems;

public class HandDragHandler : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isMinuteHand;
    private Camera cam;
    private Transform clockCenter;

    void Start()
    {
        cam = Camera.main;
        clockCenter = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPos = cam.ScreenToWorldPoint(
            new Vector3(eventData.position.x, eventData.position.y,
            Mathf.Abs(cam.transform.position.z - clockCenter.position.z))
        );
        worldPos.z = clockCenter.position.z;

        Vector2 dir = new Vector2(
            worldPos.x - clockCenter.position.x,
            worldPos.y - clockCenter.position.y
        );

        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;

        if (isMinuteHand)
        {
            int m = Mathf.RoundToInt(angle / 6f) % 60;
            ClockManager.Instance.SetTime(ClockManager.Instance.hours, m);
        }
        else
        {
            float angleDelta = angle - GetCurrentHourAngle();

            while (angleDelta > 180f) angleDelta -= 360f;
            while (angleDelta < -180f) angleDelta += 360f;

            float hourDelta = angleDelta / 30f;
            float newHourFloat = ClockManager.Instance.hours + hourDelta;

            int h = Mathf.RoundToInt(newHourFloat) % 24;
            if (h < 0) h += 24;

            ClockManager.Instance.SetTimeFromHour(h, ClockManager.Instance.minutes);
        }
    }

    public void OnEndDrag(PointerEventData eventData) { }

    float GetCurrentHourAngle()
    {
        float h = ClockManager.Instance.hours % 12;
        float m = ClockManager.Instance.minutes;
        return (h + m / 60f) * 30f;
    }
}