using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotarEje : MonoBehaviour
{
    public enum Eje { X, Y, Z }

    public float velocidad = 30f;
    public Eje eje = Eje.Y;
    public bool isWorld = false;
    public float multiplicadorVel = 1f;

    void Update()
    {
        Vector3 direccion = eje == Eje.X ? Vector3.right :
                            eje == Eje.Y ? Vector3.up : Vector3.forward;

        Space espacio = isWorld ? Space.World : Space.Self;

        transform.Rotate(direccion * velocidad * multiplicadorVel * Time.deltaTime, espacio);
    }
}
