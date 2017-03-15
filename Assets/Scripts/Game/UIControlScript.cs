using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIControlScript : MonoBehaviour {

    private GameObject LoadingText;
    
    

    void Start()
    {
        GameObject canvas;
        canvas = GameObject.Find("Canvas");
        LoadingText = canvas.transform.FindChild("LoadingText").gameObject;
        LoadingText.GetComponent<Text>().enabled = false;

        GameObject btnCloseOptions = GameObject.Find("ButtonCloseOptions");
        if (btnCloseOptions != null)
        {
            btnCloseOptions.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width + 500);
            btnCloseOptions.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height + 500);
        }

        if (SceneManager.GetActiveScene().name.Equals("GameWorld1"))
            CloseOptions();
        else if (SceneManager.GetActiveScene().name.Equals("MainMenu"))
            CloseOptions();

        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name.Equals("GameWorld1"))
            {
                if (GameObject.Find("Canvas").transform.FindChild("OptionsPanel").gameObject.activeSelf)
                    CloseOptions();
                else
                    ShowOptions();
            }
            else if (SceneManager.GetActiveScene().name.Equals("Armory"))
            {
                ArmoryExitClicked();
            }
            else if (SceneManager.GetActiveScene().name.Equals("MainMenu"))
            {
                if (GameObject.Find("Canvas").transform.FindChild("OptionsPanel").gameObject.activeSelf)
                    CloseOptions();
                else
                    ExitClicked();
            }
        
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetClicked();
        }
    }

    public void PlayGameClicked()
    {
        LoadingText.GetComponent<Text>().enabled = true;
        //GameControl.gc.LoadData();
        SceneManager.LoadScene("Armory");
    }

    public void ExitClicked()
    {
        Application.Quit();
    }

    public void ReturnToBaseClicked()
    {
        GameObject canvas;
        canvas = GameObject.Find("Canvas");
        LoadingText = canvas.transform.FindChild("LoadingText").gameObject;

        
        GameControl.gc.SaveData();
        GameControl.gc.currentLevel = 1;
        GameControl.gc.ResetPowerUps();
        LoadingText.GetComponent<Text>().enabled = true;
        SceneManager.LoadScene("Armory");
    }

    public void ArmoryPlayClicked()
    {
        LoadingText.GetComponent<Text>().enabled = true;
        GameControl.gc.SaveData();
        SceneManager.LoadScene("GameWorld1");
    }

    public void ArmoryExitClicked()
    {
        LoadingText.GetComponent<Text>().enabled = true;
        GameControl.gc.SaveData();
        SceneManager.LoadScene("MainMenu");
    }

    public void ResetClicked()
    {
        GameControl.gc.ResetData();
    }

    public void SetBossBarsActive(bool value)
    {
        if (SceneManager.GetActiveScene().name.Equals("GameWorld1"))
        {
            GameObject canvas;
            canvas = GameObject.Find("Canvas");
            canvas.transform.FindChild("SliderBossHP").gameObject.SetActive(value);
        }
    }

    public void ShowOptions()
    {
        if (GameObject.Find("Canvas").transform.FindChild("OptionsPanel").gameObject.activeSelf)
            return;

        if (SceneManager.GetActiveScene().name.Equals("GameWorld1"))
            GameControl.gc.PauseGame();

        GameObject.Find("Canvas").transform.FindChild("OptionsPanel").gameObject.SetActive(true);
        Text textSounds = GameObject.Find("Canvas/OptionsPanel/ButtonSound").GetComponentInChildren<Text>();
        Text textMusic = GameObject.Find("Canvas/OptionsPanel/ButtonMusic").GetComponentInChildren<Text>();

        if (PlayerPrefs.GetInt(GameControl.gc.GetSoundKey(), 1) == 1)
        {
            textSounds.text = "Sound ON";
        }
        else
            textSounds.text = "Sound OFF";

        if (PlayerPrefs.GetInt(GameControl.gc.GetMusicKey(), 1) == 1)
        {
            textMusic.text = "Music ON";
        }
        else
            textMusic.text = "Music OFF";

    }

    public void SoundClicked()
    {
        Text textSounds = GameObject.Find("Canvas/OptionsPanel/ButtonSound").GetComponentInChildren<Text>();

        if (PlayerPrefs.GetInt(GameControl.gc.GetSoundKey(), 1) == 1)
        {
            PlayerPrefs.SetInt(GameControl.gc.GetSoundKey(), 0);
            GameControl.gc.AUDIO_SOUNDS = false;
            textSounds.text = "Sound OFF";
        }
        else
        {
            PlayerPrefs.SetInt(GameControl.gc.GetSoundKey(), 1);
            GameControl.gc.AUDIO_SOUNDS = true;
            textSounds.text = "Sound ON";
        }

    }

    public void MusicClicked()
    {
        Text textMusic = GameObject.Find("Canvas/OptionsPanel/ButtonMusic").GetComponentInChildren<Text>();

        if (PlayerPrefs.GetInt(GameControl.gc.GetMusicKey(), 1) == 1)
        {
            PlayerPrefs.SetInt(GameControl.gc.GetMusicKey(), 0);
            GameControl.gc.AUDIO_MUSIC = false;
            textMusic.text = "Music OFF";
            GameObject.Find("Music").GetComponent<MusicScript>().StopTheMusic();

        }
        else
        {
            PlayerPrefs.SetInt(GameControl.gc.GetMusicKey(), 1);
            GameControl.gc.AUDIO_MUSIC = true;
            textMusic.text = "Music ON";
            GameObject.Find("Music").GetComponent<MusicScript>().PlayTheMusic();
        }
    }

    public void CloseOptions()
    {
        GameObject.Find("Canvas").transform.FindChild("OptionsPanel").gameObject.SetActive(false);
        if (SceneManager.GetActiveScene().name.Equals("GameWorld1"))
            GameControl.gc.PauseGame(false);
    }


}
