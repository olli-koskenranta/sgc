using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManagerScript : MonoBehaviour {

    private string GameID = "1303958";
    private string PlacementID = "rewardedVideo";
    private ArmoryScript armory;


    void Awake()
    {
        Advertisement.Initialize(GameID, true);
    }

    void Start()
    {
        armory = GameObject.Find("ArmoryScript").GetComponent<ArmoryScript>();
        if (armory)
        {
            Debug.Log("Armory found!");
        }
    }

    public IEnumerator ShowAd(string type = "rewardedVideo")
    {
    //#if UNITY_EDITOR
    //        StartCoroutine(WaitForAd());
    //#endif

        if (string.Equals(type, ""))
        {
            type = null;
        }

        ShowOptions options = new ShowOptions();
        options.resultCallback = AdCallbackHandler;

        while (!Advertisement.isInitialized || !Advertisement.IsReady())
        {
            yield return new WaitForSeconds(0.5f);
        }

        Advertisement.Show(type, options);
        
    }

    IEnumerator WaitForAd()
    {
        Debug.Log("Editor mode detected. Pausing game.");
        float currentTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        yield return null;

        while (Advertisement.isShowing)
        {
            yield return null;
        }

        Time.timeScale = currentTimeScale;
    }

    void AdCallbackHandler(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("Ad finished!");
                armory.AdFinished("FINISHED");
                break;

            case ShowResult.Skipped:
                armory.AdFinished("SKIPPED");
                Debug.Log("Ad skipped!");
                break;

            case ShowResult.Failed:
                armory.AdFinished("FAILED");
                Debug.Log("Ad failed!");
                break;

            default:
                armory.AdFinished("UNKNOWN");
                Debug.Log("AD ERROR!? UNKNOWN RESULT!");
                break;
        }
    }


}
