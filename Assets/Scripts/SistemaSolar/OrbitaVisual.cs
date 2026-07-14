using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class OrbitaVisual : MonoBehaviour
{
    [Header("Configuración")]
    public int segmentos = 128;
    public Color colorLinea = new Color(0f, 0f, 1f, 0.4f);
    public Transform planetaHijo;

    [Header("Ajuste Manual")]
    public float ajusteRadio = 0f;  // suma o resta al radio calculado
    public float offsetAltura = 0f; // sube o baja la órbita en Y

    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = segmentos + 1;
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
    }

    void Update()
    {
        float radio = new Vector2(planetaHijo.localPosition.x,
                                  planetaHijo.localPosition.z).magnitude + ajusteRadio;

        lr.startColor = colorLinea;
        lr.endColor = colorLinea;

        float angulo = 0f;
        float paso = 2f * Mathf.PI / segmentos;

        for (int i = 0; i <= segmentos; i++)
        {
            float x = Mathf.Cos(angulo) * radio;
            float z = Mathf.Sin(angulo) * radio;
            lr.SetPosition(i, new Vector3(x, offsetAltura, z));
            angulo += paso;
        }
    }
}