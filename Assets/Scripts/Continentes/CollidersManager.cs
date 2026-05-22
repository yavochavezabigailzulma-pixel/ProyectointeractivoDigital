using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidersManager : MonoBehaviour
{
    public static CollidersManager Instance;

    public bool inMap = true;
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }
    public void SwitchInMap(bool valor)
    {
        inMap = valor;
    }
}
