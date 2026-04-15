using UnityEngine;

public class RotarOrbita : MonoBehaviour
{
    public float velocidad = 10f;

    void Update()
    {
        transform.Rotate(Vector3.up * velocidad * Time.deltaTime);
    }
}