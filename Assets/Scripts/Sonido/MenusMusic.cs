using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MenusMusic : MonoBehaviour
{
    public EventReference musicaCuerposCelestes;
    EventInstance musicaCuerposCelestesInstance;

    public EventReference musicaCuerpos3D;
    EventInstance musicaCuerpos3DInstance;

    [Header("Duraciones de fade")]
    [SerializeField] private float duracionFadeEntrada = 0.5f; // al activarse (OnEnable)
    [SerializeField] private float duracionFadeSalida = 0.5f;  // al desactivarse (OnDisable)

    private void Start()
    {
        if (!musicaCuerposCelestes.IsNull)
        {
            musicaCuerposCelestesInstance = AudioManager.Instance.CreateLoop(musicaCuerposCelestes);
            AudioManager.Instance.SetVolume(musicaCuerposCelestesInstance, 1f);
        }

        if (!musicaCuerpos3D.IsNull)
        {
            musicaCuerpos3DInstance = AudioManager.Instance.CreateLoop(musicaCuerpos3D);
            AudioManager.Instance.SetVolume(musicaCuerpos3DInstance, 0f);
        }
    }

    private void OnEnable()
    {
        AudioManager.Instance.FadeTo(musicaCuerposCelestesInstance, 1f, duracionFadeEntrada);
        AudioManager.Instance.FadeTo(musicaCuerpos3DInstance, 0f, duracionFadeEntrada);
    }

    private void OnDisable()
    {
        AudioManager.Instance.FadeTo(musicaCuerposCelestesInstance, 0f, duracionFadeSalida);
        AudioManager.Instance.FadeTo(musicaCuerpos3DInstance, 1f, duracionFadeSalida);
    }

    void OnDestroy()
    {
        if (AudioManager.Instance == null) return;

        AudioManager.Instance.StopLoop(musicaCuerposCelestesInstance);
        AudioManager.Instance.StopLoop(musicaCuerpos3DInstance);
    }
}