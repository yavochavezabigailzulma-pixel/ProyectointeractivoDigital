using UnityEngine;

public class CinturonAsteroides : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject asteroidePrefab;

    [Header("Geometría del cinturón")]
    public float radio = 6f;
    public float dispersionXZ = 1f;
    public float dispersionY = 0.3f;

    [Header("Cantidad")]
    public int cantidadAsteroides = 150;

    [Header("Escala")]
    public float escalaMin = 0.05f;
    public float escalaMax = 0.25f;

    [Header("Rotación del cinturón")]
    public float velocidadOrbita = 3f;

    // Guarda el ángulo y dispersión de cada asteroide
    private float[] angulos;
    private float[] offsetsXZ;
    private float[] offsetsY;
    private float[] escalas;

    void Start()
    {
        angulos = new float[cantidadAsteroides];
        offsetsXZ = new float[cantidadAsteroides];
        offsetsY = new float[cantidadAsteroides];
        escalas = new float[cantidadAsteroides];

        for (int i = 0; i < cantidadAsteroides; i++)
        {
            // Guarda valores aleatorios fijos por asteroide
            angulos[i] = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            offsetsXZ[i] = Random.Range(-1f, 1f); // normalizado, se multiplica por dispersionXZ
            offsetsY[i] = Random.Range(-1f, 1f); // normalizado, se multiplica por dispersionY
            escalas[i] = Random.Range(escalaMin, escalaMax);

            GameObject ast = Instantiate(asteroidePrefab, transform);
            ast.transform.localRotation = Quaternion.Euler(
                Random.Range(0f, 360f),
                Random.Range(0f, 360f),
                Random.Range(0f, 360f)
            );
            ast.transform.localScale = Vector3.one * escalas[i];
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up * velocidadOrbita * Time.deltaTime);

        // Recalcula posición de cada asteroide con los valores actuales del Inspector
        for (int i = 0; i < transform.childCount; i++)
        {
            float radioFinal = radio + offsetsXZ[i] * dispersionXZ;
            float y = offsetsY[i] * dispersionY;

            transform.GetChild(i).localPosition = new Vector3(
                Mathf.Cos(angulos[i]) * radioFinal,
                y,
                Mathf.Sin(angulos[i]) * radioFinal
            );
        }
    }
}