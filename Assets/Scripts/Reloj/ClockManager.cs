using UnityEngine;
using System;

public class ClockManager : MonoBehaviour
{
    public static ClockManager Instance;

    public int hours = 12;
    public int minutes = 0;

    public GameObject bienvenidaPanel;

    public static event Action<int, int> OnTimeChanged;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (MenuManager.Instance.primeraVezReloj)
        {            
            bienvenidaPanel.SetActive(true);
            MenuManager.Instance.primeraVezReloj = false;
        }
    }
    public void SetTime(int h, int m)
    {
        int prevMinutes = minutes;
        int prevHours = hours;

        int deltaMin = m - prevMinutes;

        if (deltaMin > 30)
            h = (prevHours + 23) % 24;
        else if (deltaMin < -30)
            h = (prevHours + 1) % 24;

        hours = Mathf.Clamp(h, 0, 23);
        minutes = Mathf.Clamp(m, 0, 59);
        OnTimeChanged?.Invoke(hours, minutes);
    }

    public void SetTimeFromHour(int h, int m)
    {
        hours = (h + 24) % 24;
        minutes = Mathf.Clamp(m, 0, 59);
        OnTimeChanged?.Invoke(hours, minutes);
    }

    public void AddHours(int delta)
    {
        int h = (hours + delta + 24) % 24;
        SetTimeFromHour(h, minutes);
    }

    public void AddMinutes(int delta)
    {
        int m = minutes + delta;
        int h = hours;
        if (m >= 60) { m -= 60; h = (h + 1) % 24; }
        if (m < 0) { m += 60; h = (h + 23) % 24; }
        SetTimeFromHour(h, m);
    }
    public void Continuar()
    {
        if (bienvenidaPanel)
            bienvenidaPanel.SetActive(false);
    }
}