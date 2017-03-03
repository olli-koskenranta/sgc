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
    }
    public void PlayGameClicked()
    {
        LoadingText.GetComponent<Text>().enabled = true;
        GameControl.gc.LoadData();
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

        LoadingText.GetComponent<Text>().enabled = true;
        GameControl.gc.SaveData();
        GameControl.gc.currentLevel = 1;
        GameControl.gc.ResetPowerUps();
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
        if (SceneManager.GetActiveScene().name == "GameWorld1")
        {
            GameObject canvas;
            canvas = GameObject.Find("Canvas");
            canvas.transform.FindChild("SliderBossHP").gameObject.SetActive(value);
        }
    }


}
