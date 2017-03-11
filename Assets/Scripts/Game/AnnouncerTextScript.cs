using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnnouncerTextScript : MonoBehaviour {

    private float announcementTime;
    private float announcementDuration = 3f;
	void Start () {
        GetComponent<Text>().text = "";
        gameObject.SetActive(false);
        announcementTime = 0;
	}
	
	void Update () {
        if (gameObject.activeSelf)
        {
            if (Time.time - announcementTime > announcementDuration)
            {
                GetComponent<Text>().text = "";
                gameObject.SetActive(false);
            }
        }
	
	}

    public void Announce(string announcement)
    {
        announcementTime = Time.time;
        if (GetComponent<Text>().text.Equals(""))
            GetComponent<Text>().text = announcement;
        else
            GetComponent<Text>().text = GetComponent<Text>().text + "\n" + announcement;
        gameObject.SetActive(true);
        
    }


}
