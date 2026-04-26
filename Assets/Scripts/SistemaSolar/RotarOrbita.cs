using UnityEngine;

public class RotarOrbita : MonoBehaviour
{
    [Header("Órbita")]
    public float velocidad = 10f;
    public Vector3 ejeRotacion = Vector3.up;

    [Header("Posición Inicial")]
    public bool rotacionAleatoria = true;

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
        transform.Rotate(ejeRotacion * velocidad * Time.deltaTime);
    }
}