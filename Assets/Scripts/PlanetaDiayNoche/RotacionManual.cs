using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RotacionManual : MonoBehaviour
{
    [Header("Configuración")]
    public float sensibilidad = 0.3f;
    public bool manual = true; // el botón externo cambia este valor
    public TextMeshProUGUI btnText;

    private RotarPropio rotarPropio;
    private Vector2 ultimaPosicion;

    public Sprite toggleOn;
    public Sprite toggleOff;
    public Button buttonSwitch;

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
        if (manual) { btnText.text = "Auto"; buttonSwitch.image.sprite = toggleOn; manual = false; }
        else { btnText.text = "Manual"; buttonSwitch.image.sprite = toggleOff; manual = true; }
    }
}