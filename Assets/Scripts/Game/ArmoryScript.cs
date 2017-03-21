using UnityEngine;
using UnityEngine.UI;
using Research;

public class ArmoryScript : MonoBehaviour {

    public Text textScrap;
    public Text textRM;
    public Text textInfo;
    public Text textUpgPoints;
    public Text textSpecial1;
    public Text textSpecial2;
    public Text textNotEnoughResources;

    public Button selectWeapon0;
    public Button selectWeapon1;
    public Button selectWeapon2;
    public Button buttonShipUpgrades;
    public Button btnBuyPoints;
    public Button btnDailyResearch;
    public Button btnDailyScrapBoost;
    public Button btnDailyTraining;
    public Button btnDailyBoosts;
    public Button btnSU0;
    public Button btnSU1;
    public Button btnSU2;

    public Dropdown ddSelectZone;

    

    private ResearchType researchType = 0;
    private AnnouncerScript announcer;


    //private GameObject[] WeaponUpgradeUIElements;
    //private GameObject[] ShipUpgradeUIElements;

    private Text textResearchName;
    private Text textResearchScrapCost;
    private Text textResearchRMCost;
    private Text textResearchDescription;
    private int researchCooldown = 1;

    public Slider[] upgradeSliders = new Slider[6];

    private AdManagerScript adManager;
    private bool DAILY_RESEARCH = false;
    private bool DAILY_SCRAPBOOST = false;
    private bool DAILY_TRAINING = false;

    private int[] StartingZones;

    private float WarningShowTime;

    void Start ()
    {
        WarningShowTime = Time.time;
        GameControl.gc.PauseGame(false);
        //WeaponUpgradeUIElements = FindGameObjectsWithLayer(12);
        //ShipUpgradeUIElements = FindGameObjectsWithLayer(13);

        btnDailyBoosts.transform.FindChild("ButtonDailyResearch").gameObject.SetActive(false);
        btnDailyBoosts.transform.FindChild("ButtonDailyScrap").gameObject.SetActive(false);
        btnDailyBoosts.transform.FindChild("ButtonDailyTraining").gameObject.SetActive(false);
        btnDailyBoosts.transform.FindChild("ImageAd").gameObject.SetActive(false);
        buttonShipUpgrades.transform.FindChild("ButtonSU0").gameObject.SetActive(false);
        buttonShipUpgrades.transform.FindChild("ButtonSU1").gameObject.SetActive(false);
        buttonShipUpgrades.transform.FindChild("ButtonSU2").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.FindChild("PanelResearch").gameObject.SetActive(false);
        textNotEnoughResources.gameObject.SetActive(false);

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

    void Update()
    {
        if (textNotEnoughResources.IsActive())
        {
            if (Time.time - WarningShowTime >= 2)
            {
                textNotEnoughResources.gameObject.SetActive(false);
            }
        }
    }

    private void ShowWarning()
    {
        textNotEnoughResources.gameObject.SetActive(true);
        WarningShowTime = Time.time;
    }

    public void UpdateArmoryUI()
    {

        string specialText2;
        if (GameControl.gc.SelectedWeapon == 0)
        {
            specialText2 = GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].SpecialNames[1] + ": "
                + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].Bounces;
        }
        else if (GameControl.gc.SelectedWeapon == 2)
        {
            specialText2 = GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].SpecialNames[1] + ": "
                + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].DamageAccumulation * 100 + "%";
        }
        else
        {
            specialText2 = GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].SpecialNames[1] + " Chance: "
                + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].Special2Chance.ToString() + "%";
        }

        if (GameControl.gc.scrapCount < 1000)
            textScrap.text = GameControl.gc.scrapCount.ToString();
        else
        {
            int sc = GameControl.gc.scrapCount / 1000;
            textScrap.text = sc.ToString() + "K";
        }
        textRM.text = GameControl.gc.researchMaterialCount.ToString();

        textInfo.text = "Selected Weapon: " + GameControl.gc.WeaponNames[GameControl.gc.SelectedWeapon]
            + "\n\nWeapon Skill: " + GameControl.gc.WeaponSkill[GameControl.gc.SelectedWeapon].ToString() + "/" + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].SkillCap.ToString()
                + "(+" + (GameControl.gc.WeaponUpgrades[GameControl.gc.SelectedWeapon, 6] * 10).ToString() + ")"
            + "\nCritical Chance: " + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].CriticalChance.ToString() + "%"
            + "\nRate of Fire: " + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].RateOfFire.ToString()
            + "\nArmor Pierce: " + (GameControl.gc.WeaponUpgrades[GameControl.gc.SelectedWeapon, 1] * 15).ToString() + "%"
            + "\nWeapon Damage: " + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].Damage.ToString()
                + "(+" + (GameControl.gc.WeaponUpgrades[GameControl.gc.SelectedWeapon, 2] * 25).ToString() + "%)"
            + "\nCritical Multiplier: " + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].CriticalMultiplier.ToString()
            + "\nProjectile Mass: " + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].Mass.ToString()
            + "\n" + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].SpecialNames[0] + " Chance: " + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].SpecialChance.ToString() + "%"
            + "\n" + specialText2
            + "\n\nHighest Zone Reached: " + GameControl.gc.highestLevelAchieved.ToString();

        textSpecial1.text = GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].SpecialNames[0];
        textSpecial2.text = GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].SpecialNames[1];

        UpdateShipSystemsTexts();
        UpdateWeaponTexts();
        UpdateDailyBoostTexts();
        HighlightSelectedWeapon();
        


        textUpgPoints.text = "Allocate " + GameControl.gc.WeaponNames[GameControl.gc.SelectedWeapon] + " Weapon Power: " + UpgPointsAvailable().ToString();
        if (GameControl.gc.WeaponUpgradePointsTotal[GameControl.gc.SelectedWeapon] < 24)
        {
            btnBuyPoints.transform.FindChild("Text").transform.FindChild("TextScrapp").gameObject.SetActive(true);
            btnBuyPoints.transform.FindChild("Text").FindChild("TextRMM").gameObject.SetActive(true);
            int scrapCost = GameControl.gc.UpgradePointCost(0) / 1000;
            Text[] children = btnBuyPoints.GetComponentsInChildren<Text>();
            foreach (Text child in children)
            {
                switch (child.gameObject.name)
                {
                    case "Text":
                        child.text = "Add W. Power";
                        Vector2 pos = child.gameObject.GetComponent<RectTransform>().anchoredPosition;
                        pos.x = 0;
                        pos.y = 20;
                        child.gameObject.GetComponent<RectTransform>().anchoredPosition = pos;
                        break;
                    case "TextScrapp":
                        child.text = scrapCost.ToString() + "K";
                        break;
                    case "TextRMM":
                        child.text = GetRMCost().ToString();
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            btnBuyPoints.transform.FindChild("Text").transform.FindChild("TextScrapp").gameObject.SetActive(false);
            btnBuyPoints.transform.FindChild("Text").FindChild("TextRMM").gameObject.SetActive(false);
            Text[] children = btnBuyPoints.GetComponentsInChildren<Text>();
            foreach (Text child in children)
            {
                switch (child.gameObject.name)
                {
                    case "Text":
                        child.text = "Weapon Power Maxed";
                        Vector2 pos = child.gameObject.GetComponent<RectTransform>().anchoredPosition;
                        pos.x = 0;
                        pos.y = 0;
                        child.gameObject.GetComponent<RectTransform>().anchoredPosition = pos;
                        break;
                    default:
                        break;
                }
            }
        }

        

        
    }

    public void BeginResearch(bool adReward)
    {
        /*
         * RepairBots = 0
         * ShieldGenerator = 1
         * ReactiveArmor = 2
         * PulseLaser = 3
         * MassDriver = 4
         */

        if (GameControl.gc.scrapCount >= GameControl.gc.ResearchScrapCost[(int)researchType]
                && GameControl.gc.researchMaterialCount >= GameControl.gc.ResearchRMCost[(int)researchType])
        {
            GameControl.gc.scrapCount -= GameControl.gc.ResearchScrapCost[(int)researchType];
            GameControl.gc.researchMaterialCount -= GameControl.gc.ResearchRMCost[(int)researchType];
            GameControl.gc.ResearchStarted[(int)researchType] = true;
            GameControl.gc.ResearchStartTimes[(int)researchType] = System.DateTime.Now;
        }
        else
        {
            ShowWarning();
            return;
        }
        
        if (adReward)
        {
            StartCoroutine(adManager.ShowAd());
        }

        ShowResearchPanel(false);
        UpdateArmoryUI();
    }

    public void UpdateShipSystemsTexts()
    {
        if (GameControl.gc.ShipRepairBots)
        {
            btnSU0.interactable = false;
            btnSU0.GetComponentInChildren<Text>().text = "Repair Bots ACTIVE";
        }
        else if (!GameControl.gc.ShipRepairBots && GameControl.gc.ResearchStarted[0])
        {
            btnSU0.interactable = false;
            btnSU0.GetComponentInChildren<Text>().text = "Researching Repair Bots\nTime left: "
                + IsResearchFinished(ResearchType.RepairBots).ToString() + "h";
        }
        else
            btnSU0.GetComponentInChildren<Text>().text = "Research Repair Bots";

        if (GameControl.gc.ShipShieldGenerator)
        {
            btnSU1.interactable = false;
            btnSU1.GetComponentInChildren<Text>().text = "Shield Generator ACTIVE";
        }
        else if (!GameControl.gc.ShipShieldGenerator && GameControl.gc.ResearchStarted[1])
        {
            btnSU1.interactable = false;
            btnSU1.GetComponentInChildren<Text>().text = "Researching Shield Generator\nTime left: "
                + IsResearchFinished(ResearchType.ShieldGenerator).ToString() + "h";
        }
        else
            btnSU1.GetComponentInChildren<Text>().text = "Research Shield Generator";

        if (GameControl.gc.ShipReactiveArmor)
        {
            btnSU2.interactable = false;
            btnSU2.GetComponentInChildren<Text>().text = "Reactive Armor ACTIVE";
        }
        else if (!GameControl.gc.ShipReactiveArmor && GameControl.gc.ResearchStarted[2])
        {
            btnSU2.interactable = false;
            btnSU2.GetComponentInChildren<Text>().text = "Researching Reactive Armor\nTime left: "
                + IsResearchFinished(ResearchType.ReactiveArmor).ToString() + "h";
        }
        else
            btnSU2.GetComponentInChildren<Text>().text = "Research Reactive Armor";
    }

    public void UpdateWeaponTexts()
    {
        selectWeapon0.GetComponentInChildren<Text>().text = GameControl.gc.WeaponNames[0];

        if (!GameControl.gc.WeaponUnlocked[1] && !GameControl.gc.ResearchStarted[3])
        {
            selectWeapon1.GetComponentInChildren<Text>().text = "Research\n" + GameControl.gc.WeaponNames[1];
        }
        else if (!GameControl.gc.WeaponUnlocked[1] && GameControl.gc.ResearchStarted[3])
        {
            selectWeapon1.GetComponentInChildren<Text>().text = "Researching\n" + GameControl.gc.WeaponNames[1]
                + "\nTime left: " + IsResearchFinished(ResearchType.PulseLaser).ToString() + "h";
        }

        else
            selectWeapon1.GetComponentInChildren<Text>().text = GameControl.gc.WeaponNames[1];

        if (!GameControl.gc.WeaponUnlocked[2])
        {
            selectWeapon2.GetComponentInChildren<Text>().text = GameControl.gc.WeaponNames[2];
            selectWeapon2.GetComponentInChildren<Text>().text = "Research\n" + GameControl.gc.WeaponNames[2];
        }
        else if (!GameControl.gc.WeaponUnlocked[2] && GameControl.gc.ResearchStarted[4])
        {
            selectWeapon2.GetComponentInChildren<Text>().text = "Researching\n" + GameControl.gc.WeaponNames[2]
                + "\nTime left: " + IsResearchFinished(ResearchType.MassDriver).ToString() + "h";
        }

        else
            selectWeapon2.GetComponentInChildren<Text>().text = GameControl.gc.WeaponNames[2];
    }

    public void UpdateDailyBoostTexts()
    {
        if (IsDailyResearchAvailable())
        {
            GameObject.Find("TextBoost").GetComponent<Text>().text = "Boosts Available!";
            btnDailyResearch.interactable = true;
            btnDailyResearch.GetComponentInChildren<Text>().text = "Research\n+5 Research Material!";
        }
        else
        {
            btnDailyResearch.interactable = false;
            btnDailyResearch.GetComponentInChildren<Text>().text = "Research\nCompleted\nCooldown " + researchCooldown.ToString() + " Hour";
        }

        if (IsDailyScrapBoostAvailable())
        {
            GameObject.Find("TextBoost").GetComponent<Text>().text = "Boosts Available!";
            btnDailyScrapBoost.interactable = true;
            btnDailyScrapBoost.GetComponentInChildren<Text>().text = "Daily Equipment Maintenance\n+100% Scrap Gained!";
        }
        else
        {
            btnDailyScrapBoost.interactable = false;
            btnDailyScrapBoost.GetComponentInChildren<Text>().text = "Scrap Boost\nActive";
        }

        if (IsDailyTrainingAvailable())
        {
            GameObject.Find("TextBoost").GetComponent<Text>().text = "Boosts Available!";
            btnDailyTraining.interactable = true;
            btnDailyTraining.GetComponentInChildren<Text>().text = "Daily Training\n+100 Max Skill for Selected Weapon!";

        }
        else
        {
            btnDailyTraining.interactable = false;
            btnDailyTraining.GetComponentInChildren<Text>().text = "Daily Training Complete";
        }

        if (!IsDailyResearchAvailable() && !IsDailyScrapBoostAvailable() && !IsDailyTrainingAvailable())
        {
            GameObject.Find("TextBoost").GetComponent<Text>().text = "No Boosts Available";
        }
    }

    public int IsResearchFinished(ResearchType rType)
    {
        if (GameControl.gc.ResearchStarted[(int)rType])
        {
            System.TimeSpan timeSpan = System.DateTime.Now - GameControl.gc.ResearchStartTimes[(int)rType];
            if (timeSpan.Hours >= 4)
            {

                CompleteStartedResearch();
            }
            else
            {
                return 4 - timeSpan.Hours;
            }
        }

        return 4;
    }

    public void ResearchButtonClicked(int value)
    {
        if (GameControl.gc.ResearchStarted[value])
            return;
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
            textResearchScrapCost = texts[1];
            textResearchRMCost = texts[2];
            textResearchDescription = texts[3];

            int scrapCost;

            switch (researchType)
            {
                case ResearchType.RepairBots:
                    textResearchName.text = "Research Repair Bots";
                    scrapCost = GameControl.gc.ResearchScrapCost[0] / 1000;
                    textResearchScrapCost.text = scrapCost.ToString() + "K";
                    textResearchRMCost.text = GameControl.gc.ResearchRMCost[0].ToString();
                    textResearchDescription.text = "Repair hull over time.";

                    break;
                case ResearchType.ShieldGenerator:
                    textResearchName.text = "Research Shield Generator";
                    scrapCost = GameControl.gc.ResearchScrapCost[1] / 1000;
                    textResearchScrapCost.text = scrapCost.ToString() + "K";
                    textResearchRMCost.text = GameControl.gc.ResearchRMCost[1].ToString();
                    textResearchDescription.text = "Generates shields over time.";

                    break;
                case ResearchType.ReactiveArmor:
                    textResearchName.text = "Research Reactive Armor";
                    scrapCost = GameControl.gc.ResearchScrapCost[2] / 1000;
                    textResearchScrapCost.text = scrapCost.ToString() + "K";
                    textResearchRMCost.text = GameControl.gc.ResearchRMCost[2].ToString();
                    textResearchDescription.text = "Destroys asteroids on collision with the hull.";
                    break;
                case ResearchType.PulseLaser:
                    textResearchName.text = "Research Pulse Laser";
                    scrapCost = GameControl.gc.ResearchScrapCost[3] / 1000;
                    textResearchScrapCost.text = scrapCost.ToString() + "K";
                    textResearchRMCost.text = GameControl.gc.ResearchRMCost[3].ToString();
                    textResearchDescription.text = "Fires energy bolts with piercing capabilities. Upgradeable with beam split technology.";
                    break;
                case ResearchType.MassDriver:
                    textResearchName.text = "Research Mass Driver";
                    scrapCost = GameControl.gc.ResearchScrapCost[4] / 1000;
                    textResearchScrapCost.text = scrapCost.ToString() + "K";
                    textResearchRMCost.text = GameControl.gc.ResearchRMCost[4].ToString();
                    textResearchDescription.text = "Accelerates projectiles to hypersonic velocities. High speed impact causes accumulating damage and shrapnels.";
                    break;
                default:
                    break;
            }

        }

    }

    public void HighlightSelectedWeapon()
    {
        Image imgButton;

        imgButton = GameObject.Find("ButtonWeapon0").GetComponent<Image>();
        imgButton.color = new Color(0.59f, 0.59f, 1f);

        imgButton = GameObject.Find("ButtonWeapon1").GetComponent<Image>();
        imgButton.color = new Color(0.59f, 0.59f, 1f);

        imgButton = GameObject.Find("ButtonWeapon2").GetComponent<Image>();
        imgButton.color = new Color(0.59f, 0.59f, 1f);

        switch (GameControl.gc.SelectedWeapon)
        {
            case 0:
                imgButton = GameObject.Find("ButtonWeapon0").GetComponent<Image>();
                imgButton.color = new Color(0.59f, 1f, 1f);
                break;
            case 1:
                imgButton = GameObject.Find("ButtonWeapon1").GetComponent<Image>();
                imgButton.color = new Color(0.59f, 1f, 1f);
                break;
            case 2:
                imgButton = GameObject.Find("ButtonWeapon2").GetComponent<Image>();
                imgButton.color = new Color(0.59f, 1f, 1f);
                break;
            default:
                break;
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
            temp = btnDailyBoosts.transform.FindChild("ButtonDailyTraining").gameObject;
            temp.SetActive(false);
            temp = btnDailyBoosts.transform.FindChild("ImageAd").gameObject;
            temp.SetActive(false);
        }
        else
        {
            temp.SetActive(true);
            temp = btnDailyBoosts.transform.FindChild("ButtonDailyScrap").gameObject;
            temp.SetActive(true);
            temp = btnDailyBoosts.transform.FindChild("ButtonDailyTraining").gameObject;
            temp.SetActive(true);
            temp = btnDailyBoosts.transform.FindChild("ImageAd").gameObject;
            temp.SetActive(true);
        }
        UpdateArmoryUI();
    }

    public void ShipSystemsClicked()
    {
        GameObject temp = buttonShipUpgrades.transform.FindChild("ButtonSU0").gameObject;
        if (temp.activeSelf)
        {
            temp.SetActive(false);
            temp = buttonShipUpgrades.transform.FindChild("ButtonSU1").gameObject;
            temp.SetActive(false);
            temp = buttonShipUpgrades.transform.FindChild("ButtonSU2").gameObject;
            temp.SetActive(false);
        }
        else
        {
            temp.SetActive(true);
            temp = buttonShipUpgrades.transform.FindChild("ButtonSU1").gameObject;
            temp.SetActive(true);
            temp = buttonShipUpgrades.transform.FindChild("ButtonSU2").gameObject;
            temp.SetActive(true);
        }
        UpdateArmoryUI();
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
        for (int i = GameControl.gc.StartZoneUnlocked.Count - 1; i >= 0; i--)
        {
            size += 120;
            if (size > 560)
                size = 560;

            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
            string newText = "Start Zone " + GameControl.gc.StartZoneUnlocked[i].ToString();
            ddSelectZone.options.Add(new Dropdown.OptionData() { text = newText });
        }
        ddSelectZone.value = GameControl.gc.SelectedZone;
        DropDownZoneSelected();
        ddSelectZone.RefreshShownValue();
    }

    public void DropDownZoneSelected()
    {
        GameControl.gc.currentLevel = GameControl.gc.StartZoneUnlocked[GameControl.gc.StartZoneUnlocked.Count - 1 - ddSelectZone.value];
        //Debug.Log((GameControl.gc.StartZoneUnlocked.Count - 1 - ddSelectZone.value).ToString());
        GameControl.gc.SelectedZone = ddSelectZone.value;
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
        if (GameControl.gc.WeaponUnlocked[weaponNumber])
        {
            GameControl.gc.SelectedWeapon = weaponNumber;
            UpdateSliders();
            UpdateArmoryUI();
        }
        else if (!GameControl.gc.WeaponUnlocked[weaponNumber])
        {
            ResearchButtonClicked(weaponNumber + 2);
        }
        else
            UpdateArmoryUI();
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

    public void AddWeaponPower()
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
            ShowWarning();

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
                    GameControl.gc.SaveData();
                    Debug.Log("Research Complete");
                }
                else if (DAILY_SCRAPBOOST)
                {
                    DAILY_SCRAPBOOST = false;
                    GameControl.gc.ScrapBoostActive = true;
                    GameControl.gc.DateDailyScrapBoostTime = System.DateTime.Now;
                    GameControl.gc.SaveData();
                    Debug.Log("ScrapBoost Complete");
                }
                else if (DAILY_TRAINING)
                {
                    DAILY_TRAINING = false;
                    GameControl.gc.WeaponUpgrades[GameControl.gc.SelectedWeapon, 6] += 10;
                    GameControl.gc.DateDailyTrainingTime = System.DateTime.Now;
                    GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].UpdateValues(GameControl.gc.SelectedWeapon);
                    GameControl.gc.SaveData();
                    Debug.Log("Training Complete");
                }
                else
                {
                    CompleteStartedResearch();
                }
                UpdateArmoryUI();

                break;
            case "SKIPPED":
                Debug.Log("This should never happen");
                DAILY_RESEARCH = false;
                DAILY_SCRAPBOOST = false;
                DAILY_TRAINING = false;
                UpdateArmoryUI();
                break;
            case "FAILED":
                Debug.Log("Ad failed to show");
                announcer.Announce("Ad failed to show!", FloatingText.FTType.Danger);
                DAILY_RESEARCH = false;
                DAILY_SCRAPBOOST = false;
                DAILY_TRAINING = false;
                UpdateArmoryUI();
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
            DailyBoostsClicked();
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
            DailyBoostsClicked();
        }
    }

    public void DailyTrainingClicked()
    {
        if (DAILY_TRAINING)
            return;
        if (IsDailyTrainingAvailable())
        {
            DAILY_TRAINING = true;
            StartCoroutine(adManager.ShowAd());
            DailyBoostsClicked();
        }
    }

    private bool IsDailyResearchAvailable()
    {
        System.TimeSpan timeSpan = System.DateTime.Now - GameControl.gc.DateDailyResearchTime;
        if (timeSpan.Hours >= researchCooldown)
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

    private bool IsDailyTrainingAvailable()
    {
        System.DateTime dateTime = System.DateTime.Now;
        if (dateTime.Day != GameControl.gc.DateDailyTrainingTime.Day)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CompleteStartedResearch()
    {
        for (int i = 0; i < GameControl.gc.ResearchStarted.Length; i++)
        {
            if (GameControl.gc.ResearchStarted[i] == true)
            {
                /*
                 * RepairBots = 0
                 * ShieldGenerator = 1
                 * ReactiveArmor = 2
                 * PulseLaser = 3
                 * MassDriver = 4
                 */
                switch (i)
                {
                    case 0:
                        GameControl.gc.ShipRepairBots = true;
                        break;
                    case 1:
                        GameControl.gc.ShipShieldGenerator = true;
                        break;
                    case 2:
                        GameControl.gc.ShipReactiveArmor = true;
                        break;
                    case 3:
                        GameControl.gc.WeaponUnlocked[1] = true;
                        break;
                    case 4:
                        GameControl.gc.WeaponUnlocked[2] = true;
                        break;
                }
            }
        }
    }


}
