using UnityEngine;

public class RotarPropio : MonoBehaviour
{
    public float velocidad = 20f;

    void Update()
    {
        transform.Rotate(Vector3.up * velocidad * Time.deltaTime);
    }
}