using UnityEngine;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour {

    private int page = 0;
    private Text textTutorial;
    private int armoryPages = 3;
    private int gamePages = 0;
    private GameObject borders;
    
	// Use this for initialization
	void Start () {
        if (PlayerPrefs.GetInt(GameControl.gc.GetTutorialKey(), 0) == 1)
        {
            Destroy(gameObject);
            return;
        }
        textTutorial = GameObject.Find("TutorialText").GetComponent<Text>();
        borders = GameObject.Find("ImageBorder");
        if (GameControl.gc.GetSceneName().Equals("Armory"))
        {
            textTutorial.text = "Welcome to the experimental mining vessel USS Artemis, Captain!"
                + "\n\nWe have lost contact to Earth and are in the process of returning back.";
            
        }
    }

    public void TutorialOKClicked()
    {
        

        string scenename;
        scenename = GameControl.gc.GetSceneName();
        
        switch (scenename)
        {
            case "Armory":
                if (page < armoryPages)
                {
                    page++;
                    Vector2 newPos;
                    switch (page)
                    {
                        case 1:
                            textTutorial.text = "Here are your mining tools.\n\nYou can commence research on experimental equipment once you have gathered enough resources.";
                            borders.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1138);
                            borders.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 130);

                            newPos = borders.GetComponent<RectTransform>().anchoredPosition;
                            newPos.x = 20;
                            newPos.y = 125;
                            borders.GetComponent<RectTransform>().anchoredPosition = newPos;
                            
                            break;
                        case 2:
                            textTutorial.text = "Resources can also be used to upgrade the vessel's weapon and ship systems.";
                            borders.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 763);
                            borders.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 130);
                            newPos = borders.GetComponent<RectTransform>().anchoredPosition;
                            newPos.x = 1145;
                            newPos.y = 125;
                            borders.GetComponent<RectTransform>().anchoredPosition = newPos;
                            break;
                        case 3:
                            textTutorial.text = "Improve your vessels perfomance and research capabilities by using Boosts.\n\nPress Launch once you are ready to begin our mission back to Earth!";
                            borders.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 763);
                            borders.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 130);
                            newPos = borders.GetComponent<RectTransform>().anchoredPosition;
                            newPos.x = 1145;
                            newPos.y = -794;
                            borders.GetComponent<RectTransform>().anchoredPosition = newPos;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    PlayerPrefs.SetInt(GameControl.gc.GetTutorialKey(), 1);
                    Destroy(gameObject);
                }
                break;
            case "GameWorld1":
                break;
            default:
                break;
        }


    }
}
