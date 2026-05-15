using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public bool primeraVez = true;
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void setPrimeraVez(bool check)
    {
        primeraVez=check;
    }
    public bool getPrimeraVez()
    {
        return primeraVez;
    }
}


//    public EventReference musicaIntroMenu;
//    EventInstance musicaInstance;
//    private void Start()
//    {
//        musicaInstance = AudioManager.Instance.CreateLoop(musicaIntroMenu);
//    }
//    public void IrAMenu()
//    {
//        SceneManager.LoadScene("Menu");
//    }