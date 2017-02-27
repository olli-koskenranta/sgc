using UnityEngine;
using UnityEngine.UI;
using ShipWeapons;
using System.Collections;

public class ArmoryScript : MonoBehaviour {

    public Text textInfo;
    public Text textUpgPoints;
    public Text textSpecial1;
    public Text textSpecial2;

    public Button selectWeapon0;
    public Button selectWeapon1;
    public Button selectWeapon2;
    public Button buttonShipUpgrades;
    public Button btnBuyPoints;
    public Button btnDailyResearch;
    public Button btnDailyScrapBoost;
    public Button btnDailyBoosts;
    public Dropdown ddSelectZone;

    public enum ArmoryView { Weapon, Ship };
    public ArmoryView view;

    private GameObject[] WeaponUpgradeUIElements;
    private GameObject[] ShipUpgradeUIElements;
    

    public Slider[] upgradeSliders = new Slider[6];

    private AdManagerScript adManager;
    private bool DAILY_RESEARCH = false;
    private bool DAILY_SCRAPBOOST = false;

    private int[] StartingZones;

    void Start ()
    {
        view = ArmoryView.Weapon;
        WeaponUpgradeUIElements = FindGameObjectsWithLayer(12);
        ShipUpgradeUIElements = FindGameObjectsWithLayer(13);
        ShowShipUI(false);

        GameObject temp = btnDailyBoosts.transform.FindChild("ButtonDailyResearch").gameObject;
        temp.SetActive(false);
        temp = btnDailyBoosts.transform.FindChild("ButtonDailyScrap").gameObject;
        temp.SetActive(false);


        UpdateSliders();
        UpdateStartingZones();
        UpdateArmoryUI();
        

        adManager = GameObject.Find("AdManager").GetComponent<AdManagerScript>();
        if (adManager)
        {
            Debug.Log("Ad manager found!");
        }
    }

    public void UpdateArmoryUI()
    {
        switch (view)
        {
            case ArmoryView.Weapon:

                string specialText2;
                if (GameControlScript.gameControl.SelectedWeapon == 0)
                {
                    specialText2 = GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].SpecialNames[1] + ": "
                        + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].Bounces;
                }
                else
                {
                    specialText2 = GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].SpecialNames[1] + " Chance: "
                       + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].Special2Chance.ToString() + "%";
                }


                textInfo.text = "Scrap Count: " + GameControlScript.gameControl.scrapCount.ToString()
                    + "\nResearch Material: " + GameControlScript.gameControl.researchMaterialCount.ToString()
                    + "\n\nSelected Weapon: " + GameControlScript.gameControl.WeaponNames[GameControlScript.gameControl.SelectedWeapon]
                    + "\nWeapon Skill: " + GameControlScript.gameControl.WeaponSkill[GameControlScript.gameControl.SelectedWeapon].ToString() + "/" + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].SkillCap.ToString()
                        + "(+" + (GameControlScript.gameControl.WeaponUpgrades[GameControlScript.gameControl.SelectedWeapon, 6] * 5).ToString() + ")"
                    + "\nCritical Chance: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].CriticalChance.ToString() + "%"
                    + "\nRate of Fire: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].RateOfFire.ToString()
                    + "\nProjectile Mass: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].Mass.ToString()
                        + "(+" + (GameControlScript.gameControl.WeaponUpgrades[GameControlScript.gameControl.SelectedWeapon, 1] * 100).ToString() + "%)"
                    + "\nWeapon Damage: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].Damage.ToString()
                        + "(+" + (GameControlScript.gameControl.WeaponUpgrades[GameControlScript.gameControl.SelectedWeapon, 2] * 25).ToString() + "%)"
                    + "\nCritical Multiplier: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].CriticalMultiplier.ToString()
                    + "\n" + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].SpecialNames[0] + " Chance: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].SpecialChance.ToString() + "%"
                    + "\n" + specialText2;
                    //+ "\nStart Zone: " + GameControlScript.gameControl.currentLevel.ToString();
                break;

            case ArmoryView.Ship:
                textInfo.text = "Scrap Count: " + GameControlScript.gameControl.scrapCount.ToString()
                    + "\nResearch Material: " + GameControlScript.gameControl.researchMaterialCount.ToString()
                    + "\n\nRepair Bots: Repair hull over time"
                    + "\n\nShield Generator: Generates shields over time"
                    + "\n\nReactive Armor: Destroys asteroids when they hit the hull";
                break;
            default:
                textInfo.text = "What is happening?";
                break;
        }

        selectWeapon0.GetComponentInChildren<Text>().text = GameControlScript.gameControl.WeaponNames[0];
        if (!GameControlScript.gameControl.WeaponUnlocked[1])
        {
            selectWeapon1.GetComponentInChildren<Text>().text = GameControlScript.gameControl.WeaponNames[1];
            //selectWeapon1.GetComponentInChildren<Text>().text = "Unlock " + GameControlScript.gameControl.WeaponNames[1] + "\n(20 000 SRAP, 1 RM)";
        }
            
        else
            selectWeapon1.GetComponentInChildren<Text>().text = GameControlScript.gameControl.WeaponNames[1];
        if (!GameControlScript.gameControl.WeaponUnlocked[2])
        {
            selectWeapon2.GetComponentInChildren<Text>().text = GameControlScript.gameControl.WeaponNames[2];
            //selectWeapon2.GetComponentInChildren<Text>().text = "Unlock " + GameControlScript.gameControl.WeaponNames[2] + "\n(100 000 SCRAP, 5 RM)";
        }
            
        else
            selectWeapon2.GetComponentInChildren<Text>().text = GameControlScript.gameControl.WeaponNames[2];


        textUpgPoints.text = GameControlScript.gameControl.WeaponNames[GameControlScript.gameControl.SelectedWeapon] + " Upgrade Points: " + UpgPointsAvailable().ToString();
        if (GameControlScript.gameControl.WeaponUpgradePointsTotal[GameControlScript.gameControl.SelectedWeapon] < 24)
        {
            btnBuyPoints.GetComponentInChildren<Text>().text = "Buy Points\n" + GameControlScript.gameControl.UpgradePointCost(0).ToString() + "S/" + GetRMCost().ToString() + "RM";
        }
        else
        {
            btnBuyPoints.GetComponentInChildren<Text>().text = "Points Maxed";
        }

        if (IsDailyResearchAvailable())
        {
            btnDailyResearch.interactable = true;
            btnDailyResearch.GetComponentInChildren<Text>().text = "Research\n+5 R. Material!\nWatch Ad";
        }
        else
        {
            btnDailyResearch.interactable = false;
            btnDailyResearch.GetComponentInChildren<Text>().text = "Research\nCompleted";
        }

        if (IsDailyScrapBoostAvailable())
        {
            btnDailyScrapBoost.interactable = true;
            btnDailyScrapBoost.GetComponentInChildren<Text>().text = "Scrap Boost\n+100% Scrap!\nWatch Ad";
        }
        else
        {
            btnDailyScrapBoost.interactable = false;
            btnDailyScrapBoost.GetComponentInChildren<Text>().text = "Scrap Boost\nActive";
        }

        textSpecial1.text = GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].SpecialNames[0];
        textSpecial2.text = GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].SpecialNames[1];


        //GameControlScript.gameControl.UpdatePlayerAttributes();



    }

    public void DailyBoostsClicked()
    {
        GameObject temp = btnDailyBoosts.transform.FindChild("ButtonDailyResearch").gameObject;
        if (temp.activeSelf)
        {
            temp.SetActive(false);
            temp = btnDailyBoosts.transform.FindChild("ButtonDailyScrap").gameObject;
            temp.SetActive(false);
        }
        else
        {
            temp.SetActive(true);
            temp = btnDailyBoosts.transform.FindChild("ButtonDailyScrap").gameObject;
            temp.SetActive(true);
        }
    }

    public void UpdateSliders()
    {
        for (int i = 0; i <= 5; i++)
        {
            upgradeSliders[i].value = GameControlScript.gameControl.WeaponUpgrades[GameControlScript.gameControl.SelectedWeapon, i];
        }
    }

    public void UpdateStartingZones()
    {
        ddSelectZone.ClearOptions();
        RectTransform rect = ddSelectZone.transform.FindChild("Template").GetComponent<RectTransform>();
        float size = 0;
        for (int i = 0; i < GameControlScript.gameControl.startZones.Length; i++)
        {
            if (GameControlScript.gameControl.StartZoneUnlocked[i])
            {
                if (size < 600)
                    size += 120;
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                string newText = "Start Zone " + GameControlScript.gameControl.startZones[i].ToString();
                ddSelectZone.options.Add(new Dropdown.OptionData() { text = newText });
            }
            else continue;
        }
        ddSelectZone.value = 0;
        ddSelectZone.RefreshShownValue();
    }

    public void DropDownZoneSelected()
    {
        GameControlScript.gameControl.currentLevel = GameControlScript.gameControl.startZones[ddSelectZone.value];
        UpdateArmoryUI();
    }

    public void UpgradeSliderChanged(int sliderNumber)
    {
        GameControlScript.gameControl.WeaponUpgrades[GameControlScript.gameControl.SelectedWeapon, sliderNumber] = (int)upgradeSliders[sliderNumber].value;
        GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].UpdateValues(GameControlScript.gameControl.SelectedWeapon);
        UpdateArmoryUI();

    }

    public void PlusUpgradeClicked(int sliderNumber)
    {

        if (UpgPointsAvailable() > 0)
        {
            upgradeSliders[sliderNumber].value += 1;
        }
    }

    public void MinusUpgradeClicked(int sliderNumber)
    {
        upgradeSliders[sliderNumber].value -= 1;
    }

    private int UpgPointsAvailable()
    {
        int usedPoints = 0;
        foreach (Slider slider in upgradeSliders)
        {
            usedPoints += (int)slider.value;
        }
        int availablePoints = GameControlScript.gameControl.WeaponUpgradePointsTotal[GameControlScript.gameControl.SelectedWeapon] - usedPoints;
        return availablePoints;
    }

    public void SelectWeaponClicked(int weaponNumber)
    {
        if (view != ArmoryView.Weapon)
        {
            view = ArmoryScript.ArmoryView.Weapon;
            ShowWeaponUI(true);
            ShowShipUI(false);
        }

        if (true) //GameControlScript.gameControl.WeaponUnlocked[weaponNumber])
        {
            GameControlScript.gameControl.SelectedWeapon = weaponNumber;
            UpdateSliders();
            UpdateArmoryUI();
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
        if (view != ArmoryView.Ship)
        {
            view = ArmoryScript.ArmoryView.Ship;
            ShowShipUI(true);
            ShowWeaponUI(false);
            UpdateArmoryUI();
        }

        //Show ship upgrades UI
    }

    public GameObject[] FindGameObjectsWithLayer(int layer)
    {
        GameObject[] goArray = Object.FindObjectsOfType<GameObject>();
        System.Collections.Generic.List<GameObject> goList = new System.Collections.Generic.List<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == layer)
            {
                goList.Add(goArray[i]);
            }
        }

        if (goList.Count == 0)
        {
            return null;
        }

        return goList.ToArray();
    }

    public void ShowWeaponUI(bool value)
    {
        if (WeaponUpgradeUIElements != null)
        {
            foreach (GameObject go in WeaponUpgradeUIElements)
            {
                go.SetActive(value);
            }
        }
    }

    public void ShowShipUI(bool value)
    {
        if (ShipUpgradeUIElements != null)
        {
            foreach (GameObject go in ShipUpgradeUIElements)
            {
                go.SetActive(value);
            }
        }
    }

    public void BuyPointsClicked()
    {
        if (GameControlScript.gameControl.WeaponUpgradePointsTotal[GameControlScript.gameControl.SelectedWeapon] == 24)
            return;

        int scrapCost = GameControlScript.gameControl.UpgradePointCost(0);
        int RMCost = GetRMCost();
        

        if (GameControlScript.gameControl.scrapCount >= scrapCost && GameControlScript.gameControl.researchMaterialCount >= RMCost)
        {
            GameControlScript.gameControl.scrapCount -= scrapCost;
            GameControlScript.gameControl.researchMaterialCount -= RMCost;

            GameControlScript.gameControl.WeaponUpgradePointsTotal[GameControlScript.gameControl.SelectedWeapon] += 1;
            UpdateArmoryUI();
        }
        else
            Debug.Log("Not enough materials!");
    }

    private int GetRMCost()
    {
        int RMCost = GameControlScript.gameControl.UpgradePointCost(1);
        if (GameControlScript.gameControl.WeaponUpgradePointsTotal[GameControlScript.gameControl.SelectedWeapon] >= 10)
        {
            RMCost += 1;
        }
        return RMCost;
    }

    public void AdFinished(string result)
    {
        switch (result)
        {
            case "FINISHED":
                if (DAILY_RESEARCH)
                {
                    DAILY_RESEARCH = false;
                    GameControlScript.gameControl.researchMaterialCount += 5;
                    GameControlScript.gameControl.DateDailyResearchTime = System.DateTime.Now;
                    UpdateArmoryUI();
                    GameControlScript.gameControl.SaveData();
                }
                if (DAILY_SCRAPBOOST)
                {
                    DAILY_SCRAPBOOST = false;
                    GameControlScript.gameControl.ScrapBoostActive = true;
                    GameControlScript.gameControl.DateDailyScrapBoostTime = System.DateTime.Now;
                    UpdateArmoryUI();
                    GameControlScript.gameControl.SaveData();
                }
                break;
            case "SKIPPED":
                Debug.Log("This should never happen");
                DAILY_RESEARCH = false;
                DAILY_SCRAPBOOST = false;
                break;
            case "FAILED":
                Debug.Log("Ad failed to show");
                DAILY_RESEARCH = false;
                DAILY_SCRAPBOOST = false;
                break;
            default:
                break;
        }
    }

    public void DailyResearchClicked()
    {
        if (DAILY_RESEARCH)
            return;
        if (IsDailyResearchAvailable())
        {
            DAILY_RESEARCH = true;
            StartCoroutine(adManager.ShowAd());
        }
    }

    public void DailyScrapBoostClicked()
    {
        if (DAILY_SCRAPBOOST)
            return;
        if (IsDailyScrapBoostAvailable())
        {
            DAILY_SCRAPBOOST = true;
            StartCoroutine(adManager.ShowAd());
        }
    }

    private bool IsDailyResearchAvailable()
    {
        System.DateTime dateTime = System.DateTime.Now;
        if (dateTime.Day != GameControlScript.gameControl.DateDailyResearchTime.Day)
            return true;
        else
            return false;
    }

    private bool IsDailyScrapBoostAvailable()
    {
        System.DateTime dateTime = System.DateTime.Now;
        if (dateTime.Day != GameControlScript.gameControl.DateDailyScrapBoostTime.Day)
            return true;
        else
            return false;
    }


}
