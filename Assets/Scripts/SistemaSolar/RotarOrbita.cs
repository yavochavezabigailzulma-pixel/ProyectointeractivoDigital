using UnityEngine;

public class RotarOrbita : MonoBehaviour
{
    [Header("Órbita")]
    public float velocidad = 8f;
    public Vector3 ejeRotacion = Vector3.up;
    public float multiplicador = 1f;

    [Header("Posición Inicial")]
    public bool rotacionAleatoria = true;

    [Header("Luna")]
    public Transform objetivoOrbita;

    void Start()
    {
        if (rotacionAleatoria)
        {
            float anguloInicial = Random.Range(0f, 360f);
            transform.Rotate(ejeRotacion * anguloInicial);
        }
    }

    void Update()
    {
        if (objetivoOrbita != null)
        {
            // Orbita alrededor del objetivo (Tierra)
            transform.RotateAround(
                objetivoOrbita.position,
                ejeRotacion,
                velocidad * multiplicador * Time.deltaTime
            );
        }
        else
            transform.Rotate(ejeRotacion * (velocidad*multiplicador) * Time.deltaTime);
    }
}