using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MainMenuScript : MonoBehaviour {

    public Text TextVersion;
    public Text textSounds;
    public Text textMusic;

	void Start ()
    {
        GameControl.gc.PauseGame(false);
        GameObject.Find("Canvas").transform.FindChild("CreditsPanel").gameObject.SetActive(false);
        TextVersion.text = "Version " + GameControl.GameVersion;
    }

    

    public void RateAppClicked()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.ookoointeractive.sgc");
    }

    public void CreditsClicked()
    {
        GameObject.Find("Canvas").transform.FindChild("CreditsPanel").gameObject.SetActive(true);
    }

    public void CloseCredits()
    {
        GameObject.Find("Canvas").transform.FindChild("CreditsPanel").gameObject.SetActive(false);
    }




}
