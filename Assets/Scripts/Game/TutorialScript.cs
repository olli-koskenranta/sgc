using UnityEngine;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour {

    private int page = 0;
    private Text textTutorial;
    private int armoryPages = 4;
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

        if (GameControl.gc.GetSceneName().Equals("GameWorld1"))
        {
            
            textTutorial.text = "You can switch your weapons from the bottom left corner once you have more than one researched."
                + "\n\nS and H indicate the the state of your shields and hull integrity."
                + "\n\nTap and hold to aim and autofire your turret.";

            borders.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 840);
            borders.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 130);

            Vector2 newPos;
            newPos = borders.GetComponent<RectTransform>().anchoredPosition;
            newPos.x = 0;
            newPos.y = -840;
            borders.GetComponent<RectTransform>().anchoredPosition = newPos;
            Debug.Log("Pause this shit");
            GameControl.gc.PauseGame();

        }
    }

    public void TutorialOKClicked()
    {
        

        string scenename;
        scenename = GameControl.gc.GetSceneName();
        
        switch (scenename)
        {
            case "Armory":

                page++;
                Vector2 newPos;
                switch (page)
                {
                    case 1:
                        textTutorial.text = "Here are your mining tools which also work as weapons.\n\nYou can commence research on new equipment once you have gathered enough resources.";
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
                        textTutorial.text = "Use this panel to freely distribute your excess weapon power to improve or enable different properties.";
                        borders.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400);
                        borders.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 940);
                        newPos = borders.GetComponent<RectTransform>().anchoredPosition;
                        newPos.x = 1500;
                        newPos.y = 55;
                        borders.GetComponent<RectTransform>().anchoredPosition = newPos;
                        break;
                    case 4:
                        textTutorial.text = "Improve your vessels perfomance and research capabilities by using Boosts.\n\nPress Launch once you are ready to begin our mission back to Earth!";
                        borders.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 763);
                        borders.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 130);
                        newPos = borders.GetComponent<RectTransform>().anchoredPosition;
                        newPos.x = 1145;
                        newPos.y = -794;
                        borders.GetComponent<RectTransform>().anchoredPosition = newPos;
                        break;
                    default:
                        Destroy(gameObject);
                        break;
                }
                break;

            case "GameWorld1":
                Destroy(gameObject);
                PlayerPrefs.SetInt(GameControl.gc.GetTutorialKey(), 1);
                GameControl.gc.PauseGame(false);
                break;
               
            default:
                break;

        }


    }
}
