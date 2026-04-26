using UnityEngine;

public class SkyController : MonoBehaviour
{
    private Renderer rend;

    private Color[] skyColors = new Color[]
    {
        new Color(0.02f, 0.04f, 0.12f),  // 0h  - noche
        new Color(0.06f, 0.08f, 0.24f),  // 5h  - madrugada
        new Color(1.00f, 0.63f, 0.31f),  // 6h  - amanecer
        new Color(0.53f, 0.75f, 0.94f),  // 8h  - mañana
        new Color(0.31f, 0.67f, 1.00f),  // 12h - mediodía
        new Color(1.00f, 0.78f, 0.31f),  // 17h - tarde
        //new Color(1.00f, 0.39f, 0.16f),  // 19h - atardecer        
        new Color(0.12f, 0.12f, 0.31f),  // 19h - noche
        new Color(0.12f, 0.12f, 0.31f),  // 21h - noche
        new Color(0.02f, 0.04f, 0.12f),  // 24h - noche
    };

    private float[] skyHours = { 0, 5, 6, 8, 12, 17, 19, 21, 24 };

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    void OnEnable()
    {
        ClockManager.OnTimeChanged += UpdateSky;
    }

    void OnDisable()
    {
        ClockManager.OnTimeChanged -= UpdateSky;
    }

    void Start()
    {
        UpdateSky(ClockManager.Instance.hours, ClockManager.Instance.minutes);
    }

    void UpdateSky(int h, int m)
    {
        float t = h + m / 60f;

        for (int i = 0; i < skyHours.Length - 1; i++)
        {
            if (t >= skyHours[i] && t <= skyHours[i + 1])
            {
                float frac = (t - skyHours[i]) / (skyHours[i + 1] - skyHours[i]);
                Color c = Color.Lerp(skyColors[i], skyColors[i + 1], frac);
                rend.material.color = c;
                return;
            }
        }
    }
}