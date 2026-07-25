using UnityEngine;

public class PanelAudioStopper : MonoBehaviour
{
    void OnDisable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusicaEstacion();
    }
}