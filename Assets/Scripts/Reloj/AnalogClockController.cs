using UnityEngine;

public class AnalogClockController : MonoBehaviour
{
    public Transform hourHand;
    public Transform minuteHand;

    void OnEnable()
    {
        ClockManager.OnTimeChanged += UpdateHands;
    }
    void OnDisable()
    {
        ClockManager.OnTimeChanged -= UpdateHands;
    }
    void Start()
    {
        UpdateHands(ClockManager.Instance.hours, ClockManager.Instance.minutes);
    }

    void UpdateHands(int h, int m)
    {
        // Minutos: 360° / 60 min = 6° por minuto (negativo porque Unity rota en sentido contrario)
        float minAngle = m * -6f;

        // Horas: 360° / 12h = 30° por hora, + fracción de los minutos
        float hourAngle = (h % 12) * -30f + m * -0.5f;

        minuteHand.localRotation = Quaternion.Euler(0, 0, minAngle);
        hourHand.localRotation = Quaternion.Euler(0, 0, hourAngle);
    }
}