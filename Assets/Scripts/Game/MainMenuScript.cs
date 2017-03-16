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
        TextVersion.text = "Version " + GameControl.gc.GameVersion;
    }

    

    public void RateAppClicked()
    {
        //#if UNITY_ANDROID
        //Application.OpenURL("market://details?id=YOUR_ID");
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.ookoointeractive.sgc");
    }

    public void CreditsClicked()
    {

    }

    


}
