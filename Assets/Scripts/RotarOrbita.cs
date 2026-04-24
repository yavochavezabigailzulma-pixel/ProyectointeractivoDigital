using UnityEngine;

public class RotarOrbita : MonoBehaviour
{
    [Header("Órbita")]
    public float velocidad = 10f;
    public Vector3 ejeRotacion = Vector3.up;

    void Update()
    {
        transform.Rotate(ejeRotacion * velocidad * Time.deltaTime);
    }
}