using UnityEngine;

public class CinturonAsteroides : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject asteroidePrefab;

    [Header("Geometría del cinturón")]
    public float radioMinimo = 5f;
    public float radioMaximo = 7f;
    public float dispersionY = 0.3f;      // grosor vertical del cinturón

    [Header("Cantidad")]
    public int cantidadAsteroides = 150;

    [Header("Escala")]
    public float escalaMin = 0.05f;
    public float escalaMax = 0.25f;

    [Header("Rotación del cinturón")]
    public float velocidadOrbita = 3f;    // igual que RotarOrbita, gira el padre

    void Start()
    {
        // Genera los asteroides como hijos de este objeto
        for (int i = 0; i < cantidadAsteroides; i++)
        {
            float angulo = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float radio = Random.Range(radioMinimo, radioMaximo);
            float y = Random.Range(-dispersionY, dispersionY);

            Vector3 pos = new Vector3(
                Mathf.Cos(angulo) * radio,
                y,
                Mathf.Sin(angulo) * radio
            );

            GameObject ast = Instantiate(asteroidePrefab, transform);
            ast.transform.localPosition = pos;
            ast.transform.localRotation = Quaternion.Euler(
                Random.Range(0f, 360f),
                Random.Range(0f, 360f),
                Random.Range(0f, 360f)
            );
            float escala = Random.Range(escalaMin, escalaMax);
            ast.transform.localScale = Vector3.one * escala;
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up * velocidadOrbita * Time.deltaTime);

        // Reposiciona asteroides en tiempo real si cambias los radios en el Inspector
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform ast = transform.GetChild(i);
            Vector3 pos = ast.localPosition;
            float angulo = Mathf.Atan2(pos.z, pos.x);
            float radio = Mathf.Clamp(new Vector2(pos.x, pos.z).magnitude, radioMinimo, radioMaximo);

            ast.localPosition = new Vector3(
                Mathf.Cos(angulo) * radio,
                Mathf.Clamp(pos.y, -dispersionY, dispersionY),
                Mathf.Sin(angulo) * radio
            );
        }
    }
}