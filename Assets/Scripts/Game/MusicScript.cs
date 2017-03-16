using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicScript : MonoBehaviour {

    public static MusicScript music;
    public AudioSource trackMenuArmory;
    public AudioSource trackGame;
    public AudioSource trackBoss;
    private int currentTrackNumber;

    void Awake()
    {
        if (music == null)
        {
            DontDestroyOnLoad(gameObject);
            music = this;
        }
        else if (music != this)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
            case "Armory":
                if (trackMenuArmory.isPlaying)
                    return;
                else
                {
                    if (trackGame.isPlaying || trackBoss.isPlaying)
                    {
                        StopTheMusic();
                        PlayTrack(1);
                    }
                    else
                        PlayTrack(1);
                }
                break;
            case "GameWorld1":
                StopTheMusic();
                PlayTrack(2);
                break;
            default:
                break;
        }
    }

    public void PlayTrack(int number)
    {
        currentTrackNumber = number;

        if (PlayerPrefs.GetInt(GameControl.gc.GetMusicKey(), 1) == 0)
            return;


        StopTheMusic();

        switch (number)
        {
            case 1:
                trackMenuArmory.Play();
                break;
            case 2:
                trackGame.Play();
                break;
            case 3:
                trackBoss.Play();
                break;
            default:
                break;
        }
    }

    public void StopTheMusic()
    {
        trackMenuArmory.Stop();
        trackGame.Stop();
        trackBoss.Stop();
    }

    public void PlayTheMusic()
    {
        if (PlayerPrefs.GetInt(GameControl.gc.GetMusicKey(), 1) == 0)
            return;
        
        PlayTrack(currentTrackNumber);
    }
}
