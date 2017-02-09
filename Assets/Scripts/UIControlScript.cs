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
        //LoadingText = GameObject.Find("/Canvas/LoadingText");
        //if (LoadingText == null)
        //    Debug.Log("LoadingText null!");
        LoadingText.GetComponent<Text>().enabled = false;
    }
    public void PlayGameClicked()
    {
        LoadingText.GetComponent<Text>().enabled = true;
        GameControlScript.gameControl.LoadData();
        SceneManager.LoadScene("Armory");
    }

    public void ExitClicked()
    {
        GameControlScript.gameControl.SaveData();
        Application.Quit();
    }

    public void ReturnToBaseClicked()
    {
        //GameObject canvas;
        //canvas = GameObject.Find("Canvas");
        //LoadingText = canvas.transform.FindChild("LoadingText").gameObject;

        //LoadingText = GameObject.Find("/Canvas/LoadingText");
        //if (LoadingText == null)
        //    Debug.Log("LoadingText null!");
        LoadingText.GetComponent<Text>().enabled = true;
        GameControlScript.gameControl.SaveData();
        GameControlScript.gameControl.currentLevel = 1;
        GameControlScript.gameControl.ResetPowerUps();
        SceneManager.LoadScene("Armory");
    }

    public void ArmoryPlayClicked()
    {
        //GameObject canvas;
        //canvas = GameObject.Find("Canvas");
        //LoadingText = canvas.transform.FindChild("LoadingText").gameObject;
        LoadingText.GetComponent<Text>().enabled = true;
        GameControlScript.gameControl.SaveData();
        SceneManager.LoadScene("GameWorld1");
    }

    public void ArmoryExitClicked()
    {
        //GameObject canvas;
        //canvas = GameObject.Find("Canvas");
        //LoadingText = canvas.transform.FindChild("LoadingText").gameObject;
        LoadingText.GetComponent<Text>().enabled = true;
        GameControlScript.gameControl.SaveData();
        SceneManager.LoadScene("MainMenu");
    }

    public void SelectWeaponClicked(int weaponNumber)
    {
        if (GameControlScript.gameControl.WeaponUnlocked[weaponNumber])
        { 
            GameControlScript.gameControl.SelectedWeapon = weaponNumber;
            GameObject.Find("ArmoryScript").GetComponent<ArmoryScript>().SetTextsAndUpdate();
        }
        else
        {
            if (GameControlScript.gameControl.scrapCount >= GameControlScript.gameControl.WeaponScrapCost[weaponNumber]
                && GameControlScript.gameControl.researchMaterialCount >= GameControlScript.gameControl.WeaponRMCost[weaponNumber])
            {
                GameControlScript.gameControl.scrapCount -= GameControlScript.gameControl.WeaponScrapCost[weaponNumber];
                GameControlScript.gameControl.researchMaterialCount -= GameControlScript.gameControl.WeaponRMCost[weaponNumber];
                GameControlScript.gameControl.WeaponUnlocked[weaponNumber] = true;
                SelectWeaponClicked(weaponNumber);
            }
        }
    }

    public void ShipUpgradesClicked()
    {
        //Show ship upgrades UI
    }

    public void ZonePlusClicked()
    {
        GameControlScript.gameControl.currentLevel += 1;
        GameObject.Find("ArmoryScript").GetComponent<ArmoryScript>().SetTextsAndUpdate();
    }

    public void ZoneMinusClicked()
    {
        if (GameControlScript.gameControl.currentLevel > 2)
            GameControlScript.gameControl.currentLevel -= 1;

        GameObject.Find("ArmoryScript").GetComponent<ArmoryScript>().SetTextsAndUpdate();
    }

    public void ResetClicked()
    {
        GameControlScript.gameControl.ResetData();
    }

    public void SetBossBarsActive(bool value)
    {
        if (SceneManager.GetActiveScene().name == "GameWorld1")
        {
            GameObject canvas;
            canvas = GameObject.Find("Canvas");// as GameObject;
            canvas.transform.FindChild("BossHPBG").gameObject.SetActive(value);
            canvas.transform.FindChild("BossHP").gameObject.SetActive(value);
            canvas.transform.FindChild("BossText").gameObject.SetActive(value);
        }
    }

    public void BuyPointsClicked()
    {
        int scrapCost = GameControlScript.gameControl.WeaponUpgradeCosts[GameControlScript.gameControl.SelectedWeapon]
            + GameControlScript.gameControl.WeaponUpgradeCosts[GameControlScript.gameControl.SelectedWeapon] * GameControlScript.gameControl.WeaponUpgradePointsTotal[GameControlScript.gameControl.SelectedWeapon];
        int RMCost = GameControlScript.gameControl.WeaponUpgradeRMCosts[GameControlScript.gameControl.SelectedWeapon]
            + GameControlScript.gameControl.WeaponUpgradeRMCosts[GameControlScript.gameControl.SelectedWeapon] * GameControlScript.gameControl.WeaponUpgradePointsTotal[GameControlScript.gameControl.SelectedWeapon];

        if (GameControlScript.gameControl.scrapCount >= scrapCost && GameControlScript.gameControl.researchMaterialCount >= RMCost)
        {
            GameControlScript.gameControl.scrapCount -= scrapCost;
            GameControlScript.gameControl.researchMaterialCount -= RMCost;

            GameControlScript.gameControl.WeaponUpgradePointsTotal[GameControlScript.gameControl.SelectedWeapon] += 1;
            GameObject.Find("ArmoryScript").GetComponent<ArmoryScript>().SetTextsAndUpdate();
        }
        else
            Debug.Log("Not enough materials!");
    }
}
