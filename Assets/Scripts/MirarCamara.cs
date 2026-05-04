using UnityEngine;
[ExecuteInEditMode]
public class MirarCamara : MonoBehaviour
{
    //void Start()
    //{
    //    transform.position = transform.parent.position * -1;
    //    transform.rotation = Quaternion.Inverse(transform.parent.rotation);
    //}
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}