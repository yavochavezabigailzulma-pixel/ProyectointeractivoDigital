using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
public class MusicPlayer : MonoBehaviour
{
    public EventReference musicaRotacion;
    EventInstance musicaRotacionInstance;

    private void Start()
    {
        if (!musicaRotacion.IsNull)
            musicaRotacionInstance = AudioManager.Instance.CreateLoop(musicaRotacion);
    }
    void OnDestroy()
    {
        // Se llama cuando este objeto se destruye (por ejemplo, al descargarse la escena)
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopLoop(musicaRotacionInstance);
    }
}
