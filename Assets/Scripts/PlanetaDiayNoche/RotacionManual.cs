using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class RotacionManual : MonoBehaviour
{
    [Header("Sonidos")]
    public EventReference clicMatraca;

    public EventReference clicSelect;

    [Header("Configuración")]
    public float sensibilidad = 0.3f;
    public bool manual = true; // el botón externo cambia este valor
    public TextMeshProUGUI btnText;

    [Header("Matraca")]
    [Tooltip("Grados que hay que girar para que suene un clic. Menor valor = matraca más 'fina'.")]
    public float gradosPorClic = 5f;

    private RotarPropio rotarPropio;
    private Vector2 ultimaPosicion;
    private float gradosAcumulados = 0f;

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
            {
                ultimaPosicion = t.position;
                gradosAcumulados = 0f;
            }

            if (t.phase == TouchPhase.Moved)
            {
                Vector2 delta = t.position - ultimaPosicion;
                ultimaPosicion = t.position;

                float grados = -delta.x * sensibilidad;
                transform.Rotate(Vector3.up, grados, Space.World);

                gradosAcumulados += Mathf.Abs(grados);
                while (gradosAcumulados >= gradosPorClic)
                {
                    AudioManager.Instance.Play(clicMatraca);
                    gradosAcumulados -= gradosPorClic;
                }
            }
        }
    }

    public void SetManual()
    {
        AudioManager.Instance.Play(clicSelect);
        if (manual) { btnText.text = "Auto"; buttonSwitch.image.sprite = toggleOn; manual = false; }
        else { btnText.text = "Manual"; buttonSwitch.image.sprite = toggleOff; manual = true; }
    }
}