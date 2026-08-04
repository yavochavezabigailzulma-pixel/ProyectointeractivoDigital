using UnityEngine;

[CreateAssetMenu(fileName = "NuevaPreguntaOracion", menuName = "Preguntas/DatosOracion")]
public class DatosOracion : ScriptableObject
{
    public TemaPregunta tema;
    public string parteAntes;    // "Las estrellas son esferas de"
    public string parteDespues;  // "muy grandes que emiten luz y calor."
    public string respuestaCorrecta; // "fuego y gas"
    public string opcion2;
    public string opcion3;
}