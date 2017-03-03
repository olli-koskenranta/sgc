using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MainMenuScript : MonoBehaviour {

    public Text TextVersion;

	void Start ()
    {
        TextVersion.text = "Version " + GameControl.gc.GameVersion;
	}
	

}
