using UnityEngine;
using FloatingText;
using System.Collections;

public class AnnouncerScript : MonoBehaviour {

    private GameObject text;
    private GameObject textInstance;


    void Start () {
        text = Resources.Load("FloatingText") as GameObject;
	}

    public void Announce(string announcement, FTType type)
    {
        textInstance = Instantiate(text, transform.position, Quaternion.identity) as GameObject;
        textInstance.GetComponent<FloatingTextScript>().text = announcement;
        textInstance.GetComponent<FloatingTextScript>().fttype = type;
    }

}
