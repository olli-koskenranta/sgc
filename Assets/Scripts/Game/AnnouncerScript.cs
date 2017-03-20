using UnityEngine;
using FloatingText;

public class AnnouncerScript : MonoBehaviour {

    public void Announce(string announcement, FTType type)
    {
        GameObject textInstance;
        textInstance = Instantiate(GameControl.gc.floatingText, transform.position, Quaternion.identity);
        textInstance.GetComponent<FloatingTextScript>().text = announcement;
        textInstance.GetComponent<FloatingTextScript>().fttype = type;
        textInstance.GetComponent<FloatingTextScript>().KILL_ME = true;
        textInstance.transform.position = transform.position;
        //textInstance.SetActive(true);
    }

}
