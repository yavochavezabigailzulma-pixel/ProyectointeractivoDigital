using UnityEngine;

public class NavegadorCuerpos : MonoBehaviour
{
    //public static NavegadorCuerpos Instance;

    public GameObject pantallaMenu;
    public GameObject pantallaContenido;

    public GameObject[] paneles; // arrastra los 6 paneles en orden

    //void Awake() => Instance = this;

    //public GameObject pantallasBienvenida; // padre de las 3 pantallas
    public GameObject bienvenidaEstrellas;
    public GameObject bienvenidaGalaxias;
    public GameObject bienvenidaPlanetas;

    public void AbrirSeccion(int index)
    {
        pantallaMenu.SetActive(false);
        pantallaContenido.SetActive(true);

        for (int i = 0; i < paneles.Length; i++)
            paneles[i].SetActive(i == index);

        // Verifica si debe mostrar bienvenida
        if (index == 0 && MenuManager.Instance.primeraVezEstrellas)
        {
            bienvenidaEstrellas.SetActive(true);
            MenuManager.Instance.primeraVezEstrellas = false;
        }
        else if (index == 1 && MenuManager.Instance.primeraVezGalaxias)
        {
            bienvenidaGalaxias.SetActive(true);
            MenuManager.Instance.primeraVezGalaxias = false;
        }
        else if (index == 5 && MenuManager.Instance.primeraVezPlanetas)
        {
            bienvenidaPlanetas.SetActive(true);
            MenuManager.Instance.primeraVezPlanetas = false;
        }
    }

    public void Volver()
    {
        pantallaContenido.SetActive(false);
        pantallaMenu.SetActive(true);
    }
    public void Continuar()
    {
        if (bienvenidaEstrellas)
            bienvenidaEstrellas.SetActive(false);
        if (bienvenidaGalaxias)
            bienvenidaGalaxias.SetActive(false);
        if (bienvenidaPlanetas)
            bienvenidaPlanetas.SetActive(false);
    }
}