using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PreguntasManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI preguntaTexto;
    [SerializeField] TextMeshProUGUI opcion1Texto;
    [SerializeField] TextMeshProUGUI opcion2Texto;
    [SerializeField] TextMeshProUGUI opcion3Texto;
    public DatosPreguntas[] preguntaseleccionada;
    public int indice;
    // Start is called before the first frame update
    void Start()
    {
        ActualizarPreguntas(preguntaseleccionada[indice]); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActualizarPreguntas(DatosPreguntas nuevaPregunta)
    {
        preguntaTexto.text = nuevaPregunta.pregunta;
        opcion1Texto.text = nuevaPregunta.opcion1;
        opcion2Texto.text = nuevaPregunta.opcion2;
        opcion3Texto.text = nuevaPregunta.opcion3;
    }
}
