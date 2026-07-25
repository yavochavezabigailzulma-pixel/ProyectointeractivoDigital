using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    Dictionary<int, Coroutine> fadesActivos = new Dictionary<int, Coroutine>();
    EventInstance musicaActual;
    EventInstance musicaEstacionInstance;
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // --- Música adaptativa ---
    public void PlayMusica(string eventPath)
    {
        if (musicaActual.isValid())
        {
            musicaActual.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicaActual.release();
        }
        musicaActual = RuntimeManager.CreateInstance(eventPath);
        musicaActual.start();
    }
    public void StopMusica(bool fadeOut = true)
    {
        if (!musicaActual.isValid()) return;
        musicaActual.stop(
            fadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT
                    : FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicaActual.release();
    }
    public void SetMusicaParametro(string parametro, float valor)
    {
        if (musicaActual.isValid())
            musicaActual.setParameterByName(parametro, valor);
    }
    // --- Sonidos de una sola vez ---
    public void Play(EventReference eventRef, Vector3 position = default)
    {
        if (eventRef.IsNull) return;
        RuntimeManager.PlayOneShot(eventRef, position);
    }
    public EventInstance CreateLoop(EventReference eventRef) // <- sobrecarga nueva
    {
        if (eventRef.IsNull) return default;
        EventInstance instance = RuntimeManager.CreateInstance(eventRef);
        instance.start();
        return instance;
    }
    //public EventInstance CreateLoop(string eventPath, Vector3 position = default)
    //{
    //    EventInstance instance = RuntimeManager.CreateInstance(eventPath);
    //    instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
    //    instance.start();
    //    return instance;
    //}
    public void StopLoop(EventInstance instance, bool fadeOut = true)
    {
        if (!instance.isValid()) return;
        instance.stop(fadeOut
            ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT
            : FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
    }
    public void SetVolume(EventInstance instance, float volumen)
    {
        if (instance.isValid())
            instance.setVolume(volumen);
    }
    public void FadeTo(EventInstance instance, float volumenObjetivo, float duracion)
    {
        if (!instance.isValid()) return;
        int key = instance.handle.GetHashCode();
        if (fadesActivos.TryGetValue(key, out Coroutine anterior) && anterior != null)
            StopCoroutine(anterior);
        fadesActivos[key] = StartCoroutine(FadeVolumenRoutine(instance, volumenObjetivo, duracion, key));
    }
    IEnumerator FadeVolumenRoutine(EventInstance instance, float volumenObjetivo, float duracion, int key)
    {
        instance.getVolume(out float volumenActual);
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion;
            instance.setVolume(Mathf.Lerp(volumenActual, volumenObjetivo, t));
            yield return null;
        }
        instance.setVolume(volumenObjetivo);
        fadesActivos.Remove(key);
    }
    // --- Música por estación (Primavera/Verano/Otoño/Invierno) ---
    public void PlayMusicaConFade(EventReference eventRef)
    {
        if (musicaEstacionInstance.isValid())
        {
            musicaEstacionInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicaEstacionInstance.release();
        }

        if (eventRef.IsNull) return;

        musicaEstacionInstance = RuntimeManager.CreateInstance(eventRef);
        musicaEstacionInstance.start();
    }
    public void StopMusicaEstacion(bool fadeOut = true)
    {
        if (!musicaEstacionInstance.isValid()) return;

        musicaEstacionInstance.stop(fadeOut
            ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT
            : FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicaEstacionInstance.release();
    }
}