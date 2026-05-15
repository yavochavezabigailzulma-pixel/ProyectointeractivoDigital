using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

using FMOD.Studio;
public class MenuManager : MonoBehaviour
{
    public EventReference musicaIntroMenu;
    EventInstance musicaInstance;
    private void Start()
    {
        musicaInstance = AudioManager.Instance.CreateLoop(musicaIntroMenu);
    }
    public void IrAMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}