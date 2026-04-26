using UnityEngine;
using TMPro;

public class DigitalClockController : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI timeText12;

    void OnEnable()
    {
        ClockManager.OnTimeChanged += UpdateDisplay;
    }
    void OnDisable()
    {
        ClockManager.OnTimeChanged -= UpdateDisplay;
    }
    void Start()
    {
        UpdateDisplay(ClockManager.Instance.hours, ClockManager.Instance.minutes);
    }

    void UpdateDisplay(int h, int m)
    {
        timeText.text = $"{h:D2}:{m:D2}";

        // Reloj 12h con AM/PM
        int h12 = h % 12;
        if (h12 == 0) h12 = 12;
        string period = h < 12 ? "AM" : "PM";
        timeText12.text = $"{h12:D2}:{m:D2} {period}";
    }

    // Estos métodos se conectan a los botones desde el Inspector
    public void HourUp() => ClockManager.Instance.AddHours(1);
    public void HourDown() => ClockManager.Instance.AddHours(-1);
    public void MinuteUp() => ClockManager.Instance.AddMinutes(1);
    public void MinuteDown() => ClockManager.Instance.AddMinutes(-1);
}