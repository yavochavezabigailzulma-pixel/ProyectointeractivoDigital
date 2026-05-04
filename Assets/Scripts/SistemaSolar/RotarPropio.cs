using UnityEngine;
public class RotarPropio : MonoBehaviour
{
    public float velocidad = 30f;
    public bool isWorld = false;
    public float multiplicadorVel = 1;
    void Update()
    {
        if (name == "urano")
            transform.Rotate(Vector3.forward * (velocidad * multiplicadorVel) * Time.deltaTime);
        else
        {
            if (isWorld)
                transform.Rotate(Vector3.up * (velocidad * multiplicadorVel) * Time.deltaTime, Space.World);
            else transform.Rotate(Vector3.up * (velocidad * multiplicadorVel) * Time.deltaTime);
        }
    }
}   