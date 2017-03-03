using UnityEngine;
using UnityEngine.UI;
using ShipWeapons;
using Research;
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

    private ResearchType researchType = 0;
    private AnnouncerScript announcer;

    public enum ArmoryView { Weapon, Ship };
    public ArmoryView view;

    private GameObject[] WeaponUpgradeUIElements;
    private GameObject[] ShipUpgradeUIElements;

    private Text textResearchName;
    private Text textResearchCost;

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
        temp = GameObject.Find("Canvas").transform.FindChild("PanelResearch").gameObject;
        temp.SetActive(false);


        UpdateSliders();
        UpdateStartingZones();
        UpdateArmoryUI();

        announcer = GameObject.Find("Announcements").GetComponent<AnnouncerScript>();

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
                if (GameControl.gc.SelectedWeapon == 0)
                {
                    specialText2 = GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].SpecialNames[1] + ": "
                        + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].Bounces;
                }
                else
                {
                    specialText2 = GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].SpecialNames[1] + " Chance: "
                       + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].Special2Chance.ToString() + "%";
                }


                textInfo.text = "Scrap Count: " + GameControl.gc.scrapCount.ToString()
                    + "\nResearch Material: " + GameControl.gc.researchMaterialCount.ToString()
                    + "\n\nSelected Weapon: " + GameControl.gc.WeaponNames[GameControl.gc.SelectedWeapon]
                    + "\nWeapon Power: " + GameControl.gc.WeaponSkill[GameControl.gc.SelectedWeapon].ToString() + "/" + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].SkillCap.ToString()
                        + "(+" + (GameControl.gc.WeaponUpgrades[GameControl.gc.SelectedWeapon, 6] * 5).ToString() + ")"
                    + "\nCritical Chance: " + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].CriticalChance.ToString() + "%"
                    + "\nRate of Fire: " + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].RateOfFire.ToString()
                    + "\nProjectile Mass: " + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].Mass.ToString()
                        + "(+" + (GameControl.gc.WeaponUpgrades[GameControl.gc.SelectedWeapon, 1] * 100).ToString() + "%)"
                    + "\nWeapon Damage: " + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].Damage.ToString()
                        + "(+" + (GameControl.gc.WeaponUpgrades[GameControl.gc.SelectedWeapon, 2] * 25).ToString() + "%)"
                    + "\nCritical Multiplier: " + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].CriticalMultiplier.ToString()
                    + "\n" + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].SpecialNames[0] + " Chance: " + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].SpecialChance.ToString() + "%"
                    + "\n" + specialText2;
                    //+ "\nStart Zone: " + GameControlScript.gameControl.currentLevel.ToString();
                break;

            case ArmoryView.Ship:
                textInfo.text = "Scrap Count: " + GameControl.gc.scrapCount.ToString()
                    + "\nResearch Material: " + GameControl.gc.researchMaterialCount.ToString();
                    /*+ "\n\n\nRepair hull over time"
                    + "\n\n\nGenerates shields over time"
                    + "\n\n\nDestroys asteroids when they hit the hull";*/
                break;
            default:
                textInfo.text = "What is happening?";
                break;
        }

        selectWeapon0.GetComponentInChildren<Text>().text = GameControl.gc.WeaponNames[0];

        if (!GameControl.gc.WeaponUnlocked[1])
        {
            selectWeapon1.GetComponentInChildren<Text>().text = "Research\n" + GameControl.gc.WeaponNames[1];
        }
            
        else
            selectWeapon1.GetComponentInChildren<Text>().text = GameControl.gc.WeaponNames[1];

        if (!GameControl.gc.WeaponUnlocked[2])
        {
            selectWeapon2.GetComponentInChildren<Text>().text = GameControl.gc.WeaponNames[2];
            selectWeapon2.GetComponentInChildren<Text>().text = "Research\n" + GameControl.gc.WeaponNames[2];
        }
            
        else
            selectWeapon2.GetComponentInChildren<Text>().text = GameControl.gc.WeaponNames[2];


        textUpgPoints.text = GameControl.gc.WeaponNames[GameControl.gc.SelectedWeapon] + " Upgrade Points: " + UpgPointsAvailable().ToString();
        if (GameControl.gc.WeaponUpgradePointsTotal[GameControl.gc.SelectedWeapon] < 24)
        {
            btnBuyPoints.GetComponentInChildren<Text>().text = "Buy Points\n" + GameControl.gc.UpgradePointCost(0).ToString() + "S/" + GetRMCost().ToString() + "RM";
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

        textSpecial1.text = GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].SpecialNames[0];
        textSpecial2.text = GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].SpecialNames[1];


        //GameControlScript.gameControl.UpdatePlayerAttributes();



    }

    public void BeginResearch(int type)
    {
        /*
         * RepairBots = 0
         * ShieldGenerator = 1
         * ReactiveArmor = 2
         * PulseLaser = 3
         * MassDriver = 4
         */
        researchType = (ResearchType)type;
        bool instant = true;
        switch (researchType)
        {
            case ResearchType.RepairBots:
                if (instant)
                {
                    GameControl.gc.ShipRepairBots = true;
                }
                
                break;
            case ResearchType.ShieldGenerator:
                if (instant)
                {
                    GameControl.gc.ShipShieldGenerator = true;
                }
                break;
            case ResearchType.ReactiveArmor:
                if (instant)
                {
                    GameControl.gc.ShipReactiveArmor = true;
                }
                break;
            case ResearchType.PulseLaser:
                break;
            case ResearchType.MassDriver:
                break;
            default:
                break;
        }
    }

    public void ResearchButtonClicked(int value)
    {
        researchType = (ResearchType)value;
        ShowResearchPanel(true);
    }

    public void ShowResearchPanel(bool value)
    {
        GameObject temp;
        temp = GameObject.Find("Canvas").transform.FindChild("PanelResearch").gameObject;
        temp.SetActive(value);
        if (value == true)
        {
            Text[] texts = temp.GetComponentsInChildren<Text>();
            textResearchName = texts[0];
            textResearchCost = texts[1];

            switch (researchType)
            {
                case ResearchType.RepairBots:
                    textResearchName.text = "Research Repair Bots";
                    textResearchCost.text = "Cost: " + GameControl.gc.ResearchScrapCost[0].ToString() + "S/" + GameControl.gc.ResearchRMCost[0].ToString() + "RM";

                    break;
                case ResearchType.ShieldGenerator:
                    textResearchName.text = "Research Shield Generator";
                    textResearchCost.text = "Cost: " + GameControl.gc.ResearchScrapCost[1].ToString() + "S/" + GameControl.gc.ResearchRMCost[1].ToString() + "RM";

                    break;
                case ResearchType.ReactiveArmor:
                    textResearchName.text = "Research Reactive Armor";
                    textResearchCost.text = "Cost: " + GameControl.gc.ResearchScrapCost[2].ToString() + "S/" + GameControl.gc.ResearchRMCost[2].ToString() + "RM";
                    break;
                case ResearchType.PulseLaser:
                    textResearchName.text = "Research Pulse Laser";
                    textResearchCost.text = "Cost: " + GameControl.gc.ResearchScrapCost[3].ToString() + "S/" + GameControl.gc.ResearchRMCost[3].ToString() + "RM";
                    break;
                case ResearchType.MassDriver:
                    textResearchName.text = "Research Mass Driver";
                    textResearchCost.text = "Cost: " + GameControl.gc.ResearchScrapCost[4].ToString() + "S/" + GameControl.gc.ResearchRMCost[4].ToString() + "RM";
                    break;
                default:
                    break;
            }

        }

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
            upgradeSliders[i].value = GameControl.gc.WeaponUpgrades[GameControl.gc.SelectedWeapon, i];
        }
    }

    public void UpdateStartingZones()
    {
        ddSelectZone.ClearOptions();
        RectTransform rect = ddSelectZone.transform.FindChild("Template").GetComponent<RectTransform>();
        float size = 0;
        for (int i = 0; i < GameControl.gc.startZones.Length; i++)
        {
            if (GameControl.gc.StartZoneUnlocked[i])
            {
                if (size < 600)
                    size += 120;
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                string newText = "Start Zone " + GameControl.gc.startZones[i].ToString();
                ddSelectZone.options.Add(new Dropdown.OptionData() { text = newText });
            }
            else continue;
        }
        ddSelectZone.value = 0;
        ddSelectZone.RefreshShownValue();
    }

    public void DropDownZoneSelected()
    {
        GameControl.gc.currentLevel = GameControl.gc.startZones[ddSelectZone.value];
        UpdateArmoryUI();
    }

    public void UpgradeSliderChanged(int sliderNumber)
    {
        GameControl.gc.WeaponUpgrades[GameControl.gc.SelectedWeapon, sliderNumber] = (int)upgradeSliders[sliderNumber].value;
        GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].UpdateValues(GameControl.gc.SelectedWeapon);
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
        int availablePoints = GameControl.gc.WeaponUpgradePointsTotal[GameControl.gc.SelectedWeapon] - usedPoints;
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

        if (GameControl.gc.WeaponUnlocked[weaponNumber])
        {
            GameControl.gc.SelectedWeapon = weaponNumber;
            UpdateSliders();
            UpdateArmoryUI();
        }
        else
        {
            researchType = (ResearchType)(weaponNumber + 2);
            ShowResearchPanel(true);
            /*if (GameControl.gc.scrapCount >= GameControl.gc.ResearchScrapCost[weaponNumber]
                && GameControl.gc.researchMaterialCount >= GameControl.gc.ResearchRMCost[weaponNumber])
            {
                GameControl.gc.scrapCount -= GameControl.gc.ResearchScrapCost[weaponNumber];
                GameControl.gc.researchMaterialCount -= GameControl.gc.ResearchRMCost[weaponNumber];
                GameControl.gc.WeaponUnlocked[weaponNumber] = true;
                SelectWeaponClicked(weaponNumber);
            }*/
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
        if (GameControl.gc.WeaponUpgradePointsTotal[GameControl.gc.SelectedWeapon] == 24)
            return;

        int scrapCost = GameControl.gc.UpgradePointCost(0);
        int RMCost = GetRMCost();


        if (GameControl.gc.scrapCount >= scrapCost && GameControl.gc.researchMaterialCount >= RMCost)
        {
            GameControl.gc.scrapCount -= scrapCost;
            GameControl.gc.researchMaterialCount -= RMCost;

            GameControl.gc.WeaponUpgradePointsTotal[GameControl.gc.SelectedWeapon] += 1;
            UpdateArmoryUI();
        }
        else
        {
            Debug.Log("Not enough resources");
            announcer.Announce("Not enough resources", FloatingText.FTType.Danger);
        }
    }

    private int GetRMCost()
    {
        int RMCost = GameControl.gc.UpgradePointCost(1);
        if (GameControl.gc.WeaponUpgradePointsTotal[GameControl.gc.SelectedWeapon] >= 10)
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
                    GameControl.gc.researchMaterialCount += 5;
                    GameControl.gc.DateDailyResearchTime = System.DateTime.Now;
                    UpdateArmoryUI();
                    GameControl.gc.SaveData();
                }
                if (DAILY_SCRAPBOOST)
                {
                    DAILY_SCRAPBOOST = false;
                    GameControl.gc.ScrapBoostActive = true;
                    GameControl.gc.DateDailyScrapBoostTime = System.DateTime.Now;
                    UpdateArmoryUI();
                    GameControl.gc.SaveData();
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
        if (dateTime.Day != GameControl.gc.DateDailyResearchTime.Day)
            return true;
        else
            return false;
    }

    private bool IsDailyScrapBoostAvailable()
    {
        System.DateTime dateTime = System.DateTime.Now;
        if (dateTime.Day != GameControl.gc.DateDailyScrapBoostTime.Day)
        {
            GameControl.gc.ScrapBoostActive = false;
            return true;
        }
        else
        {
            GameControl.gc.ScrapBoostActive = true;
            return false;
        }
    }


}
