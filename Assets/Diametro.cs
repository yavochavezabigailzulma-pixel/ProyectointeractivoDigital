using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Diametro : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        float diametro = GetComponent<Renderer>().bounds.size.x;
        Debug.Log(diametro);
    }
}
