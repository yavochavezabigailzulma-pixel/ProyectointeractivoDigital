using UnityEngine;

[CreateAssetMenu(fileName = "NuevaPregunta", menuName = "SO")]
public class DatosPreguntas : ScriptableObject
   {
    public string pregunta;
    public string opcion1;
    public string opcion2;
    public string opcion3;
    public int correcto;
    }