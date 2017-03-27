using UnityEngine;
using UnityEngine.SceneManagement;
using ShipWeapons;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour {

    public const int numberOfPowerUps = 4;
    public const int numberOfWeapons = 4;
    public const int numberOfStartZones = 11;
    public const int numberOfResearches = 5;
    public const string GameVersion = "1.1";

    public static GameControl gc;
    public int scrapCount = 0;
    public int researchMaterialCount = 0;
    public int currentLevel;
    //public int scrapRequiredForNextLevel;

    public int[] Experience;
    public int[] WeaponSkill;
    public int ExpForSkillUp;
    

    public int[] ResearchScrapCost;
    public int[] ResearchRMCost;

    public DateTime DateDailyResearchTime;
    public DateTime DateDailyScrapBoostTime;
    public DateTime DateDailyTrainingTime;
    public DateTime[] ResearchStartTimes;
    public float ConvertTime;
    public bool[] ResearchStarted;

    public bool ScrapBoostActive;
    public bool GAME_PAUSED;

    public int highestLevelAchieved;



    //Player status
    public bool PLAYER_ALIVE = true;

    //Player attributes
    public System.Collections.Generic.List<int> StartZoneUnlocked;

    //Weapon types
    //0 = Basic Cannon
    //1 = Laser Cannon
    //2 = Mass Driver
    //3 = Plasma Cannon
    public int SelectedWeapon = 0;
    public int SelectedZone = 0;

    //Player upgrades
    public bool[] WeaponUnlocked;
    public bool ShipRepairBots;
    public bool ShipShieldGenerator;
    public bool ShipReactiveArmor;

    //0 = attack speed
    //1 = mass
    //2 = damage
    //3 = crit multiplier
    //4 = unique ability
    //5 = special chance
    //6 = skill cap increase
    public int[,] WeaponUpgrades;
    public int[] WeaponUpgradePointsTotal;
    public int[] WeaponUpgradePointsAvailable;

    //Player weapons
    public Turret[] Weapons;
    public Turret BlasterTurret;
    public Turret PulseLaserTurret;
    public Turret MassDriverTurret;
    public Turret PlasmaTurret;

    public string[] WeaponNames = new string[] { "Blaster", "Pulse Laser", "Mass Driver", "Plasma" };
    public float[] AttackSpeedReductions;
    public int[] WeaponUpgradeCosts;
    public int[] WeaponUpgradeRMCosts;

    //Preferences
    public const string playerSound = "playerSound";
    public const string playerMusic = "playerMusic";
    public const string tutorialKey = "Tutorial";

    //Power Ups
    //0 = Gravity Gun
    //1 = Gravity Bomb
    //2 = Scrap Bonus
    //3 = Slow Meteors
    //4 = Cluster Projectile
    //5 = Repel Shield,
    //6 = Attack speed

    public string[] PowerUpNames;

    public int[] ExtensionData;
    private const int ExtensionDataSize = 2;

    //Preloaded gameobjects
    public GameObject floatingText;
    public GameObject scrapPiece;
    public GameObject[] meteors;
    public GameObject hit_effect;

    public bool exitWithoutSaving;
    public bool firstBossDefeated;

    void Awake()
    {
        Debug.Log("GameControl Awake()");

        if (gc == null)
        {
            DontDestroyOnLoad(gameObject);
            gc = this;
            Debug.Log("gc is null, gc is this!");
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;


        }
        else if (gc != this)
        {
            Debug.Log("gc not null, destroying this!");
            Destroy(gameObject);
        }

    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log("Scene loaded: " + arg0.name);
        if (SceneManager.GetActiveScene().name.Equals("MainMenu"))
            GameObject.Find("UIControl").GetComponent<UIControlScript>().ShowErrorPanel(false);
    }

    void OnApplicationQuit()
    {
        if (exitWithoutSaving)
            return;
        SaveData();
        Debug.Log("Application quit!");
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus == true)
        {
            if (exitWithoutSaving)
                return;
            SaveData();
            Debug.Log("Application pause!");
        }
        else
            Debug.Log("Application not paused.");

    }

    
    void Start()
    {
        exitWithoutSaving = false;
        firstBossDefeated = false;
        Debug.Log("GameControl: Loading Resources...");
        floatingText = Resources.Load("FloatingText") as GameObject;
        scrapPiece = Resources.Load("ScrapPiece") as GameObject;
        meteors = new GameObject[2] { Resources.Load("medMeteor1") as GameObject, Resources.Load("bigMeteor1") as GameObject };
        hit_effect =  Resources.Load("Explosion") as GameObject;
        Debug.Log("GameControl: Resource loading complete.");

        //Debug.Log("GameControl START()!");
        Debug.Log("GameControl: Initializing everything.");
        
        ExtensionData = new int[ExtensionDataSize];
        

        WeaponUpgradeCosts = new int[] { 1000, 20000, 100000, 1000000 };
        WeaponUpgradeRMCosts = new int[] { 0, 4, 8, 10 };

        

        /*
         * RepairBots = 0
         * ShieldGenerator = 1
         * ReactiveArmor = 2
         * PulseLaser = 3
         * MassDriver = 4
         */
        ResearchScrapCost = new int[numberOfResearches] { 100000, 500000, 50000, 250000, 1000000 };
        ResearchRMCost = new int[numberOfResearches] { 10, 50, 5, 25, 100 };

        Experience = new int[numberOfWeapons];
        PowerUpNames = new string[numberOfPowerUps] { "Kinetic Bomb", "Shield", "Gravity Bomb", "Max Weapon Skill" }; //, "Slow Meteors", "Cluster Projectile", "Repel Shield", "Attack Speed" };
        WeaponSkill = new int[numberOfWeapons] { 0, 0, 0, 0 };
        WeaponUnlocked = new bool[numberOfWeapons] { true, false, false, false };
        ResearchStarted = new bool[numberOfResearches] { false, false, false, false, false };
        
        AttackSpeedReductions = new float[] { 0.15f, 0.05f, 0.025f, 0 };
        
        DateDailyResearchTime = new DateTime(2001, 1, 1, 6, 0, 0);
        DateDailyScrapBoostTime = new DateTime(2001, 1, 1, 6, 0, 0);
        DateDailyTrainingTime = new DateTime(2001, 1, 1, 6, 0, 0);
        ResearchStartTimes = new DateTime[numberOfResearches];

        WeaponUpgrades = new int[numberOfWeapons, 7];
        WeaponUpgradePointsTotal = new int[numberOfWeapons];
        WeaponUpgradePointsAvailable = new int[numberOfWeapons];
        StartZoneUnlocked = new System.Collections.Generic.List<int>();

        Debug.Log("GameControl: Initialization complete");

        Debug.Log("GameControl: Clearing Arrays...");
        ClearArrays();
        Debug.Log("GameControl: Array clear complete");


        Debug.Log("GameControl: Setting default values.");
        ExpForSkillUp = 10;
        currentLevel = 1;
        highestLevelAchieved = 1;
        ScrapBoostActive = false;
        ShipRepairBots = false;
        ShipShieldGenerator = false;
        ShipReactiveArmor = false;
        GAME_PAUSED = false;



        Debug.Log("GameControl: Creating weapons...");
        Weapons = new Turret[numberOfWeapons];

        //Create weapons
        BlasterTurret = new Turret(10f, 2, 1, 1, 5, 2f, 0, SpecialType.GravityDamage, "Gravity Blast", "Bounces");
        BlasterTurret.SetUpgradeValuesPerSkillPoint(2, 1, 0.2f);

        PulseLaserTurret = new Turret(20f, 100, 0f, 0.4f, 5, 2f, 1, SpecialType.Piercing, "Piercing", "Beam Split");
        PulseLaserTurret.SetUpgradeValuesPerSkillPoint(3, 0, 0.2f);

        MassDriverTurret = new Turret(30f, 200, 0.1f, 0.2f, 5f, 2f, 2, SpecialType.Shrapnel, "Shrapnel", "Damage Accumulation");
        MassDriverTurret.SetUpgradeValuesPerSkillPoint(5, 0.1f, 0.2f);

        PlasmaTurret = new Turret(10f, 10, 3, 1, 5, 5, 3, SpecialType.Shrapnel);
        PlasmaTurret.SetUpgradeValuesPerSkillPoint(10, 3, 0.2f);

        Weapons[0] = BlasterTurret;
        Weapons[1] = PulseLaserTurret;
        Weapons[2] = MassDriverTurret;
        Weapons[3] = PlasmaTurret;
        Debug.Log("GameControl: Weapons created.");

        Debug.Log("GameControl: Loading data...");
        LoadData();

        Debug.Log("GameControl: Updating Weapon Values...");
        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].UpdateValues(i);
        }
        Debug.Log("GameControl: Weapon Values Update complete");

    }

    public void SaveData(bool saveBackUp = false)
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/SGC.dat");

            PlayerData playerData = GetPlayerData();

            bf.Serialize(file, playerData);
            file.Close();

            if (saveBackUp)
            {
                FileStream backupFile = File.Create(Application.persistentDataPath + "/SGC.backup");
                bf.Serialize(backupFile, playerData);
                backupFile.Close();
                Debug.Log("Backup saved!");
            }
            Debug.Log("Player data saved!");
        }

        catch (Exception e)
        {
            Debug.LogError(e.Message);
            string errorMessage = e.Message;
            string errorPath = Application.persistentDataPath + "/Error.txt";
            
            using (StreamWriter outputFile = new StreamWriter(errorPath, true))
            {
                    outputFile.WriteLine(errorMessage);
            }
        }
    }

    public void LoadData()
    {

        if (File.Exists(Application.persistentDataPath + "/SGC.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SGC.dat", FileMode.Open);
            PlayerData playerData = (PlayerData)bf.Deserialize(file);
            file.Close();

            SetPlayerData(playerData);

            Debug.Log("Player data loaded!");
            GameObject.Find("UIControl").GetComponent<UIControlScript>().ShowErrorPanel(false);

        }
        else
        {
            Debug.Log("Player data not found!");
            if (PlayerPrefs.GetInt(GameControl.gc.GetTutorialKey(), 0) == 1)
                GameObject.Find("UIControl").GetComponent<UIControlScript>().ShowErrorPanel(true);
        }

    }

    public void ResetData()
    {
        
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SGC.dat");

        ClearArrays();
        WeaponUnlocked = new bool[numberOfWeapons] { true, false, false, false };
        ResearchStarted = new bool[numberOfResearches] { false, false, false, false, false };

        PlayerData playerData = GetPlayerData(true);

        bf.Serialize(file, playerData);
        file.Close();
        Debug.Log("Player data RESET!");
        LoadData();

        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].UpdateValues(i);
        }

    }

    public void ClearArrays()
    {
        
        Array.Clear(WeaponUpgrades, 0, WeaponUpgrades.Length);
        Array.Clear(WeaponSkill, 0, WeaponSkill.Length);
        Array.Clear(Experience, 0, Experience.Length);
        Array.Clear(WeaponUpgradePointsTotal, 0, WeaponUpgradePointsTotal.Length);
        WeaponUpgradePointsTotal[0] = 4;
        Array.Clear(WeaponUpgradePointsAvailable, 0, WeaponUpgradePointsAvailable.Length);
        Array.Clear(ResearchStartTimes, 0, ResearchStartTimes.Length);
        StartZoneUnlocked.Clear();
        StartZoneUnlocked.Add(1);
    }

    public int UpgradePointCost(int type)
    {
        if (type == 0)
        {
            int scrapCost = WeaponUpgradeCosts[SelectedWeapon] + WeaponUpgradeCosts[SelectedWeapon] * WeaponUpgradePointsTotal[SelectedWeapon];
            return scrapCost;
        }
        else if (type == 1)
        {
            int RMCost = WeaponUpgradeRMCosts[SelectedWeapon]; // + WeaponUpgradeRMCosts[SelectedWeapon] * WeaponUpgradePointsTotal[SelectedWeapon];
            return RMCost;
        }
        else
            return 0;
    }

    private PlayerData GetPlayerData(bool ResetData = false)
    {
        PlayerData data = new PlayerData();
        if (ResetData)
        {
            data.scrapCount = 0;
            data.selectedWeapon = 0;
            data.researchMaterialCount = 0;
            data.highestLevelAchieved = 1;
            data.ScrapBoostActive = false;
            data.DateDailyResearchTime = new DateTime(2001, 1, 1, 6, 0, 0);
            data.DateDailyScrapBoostTime = new DateTime(2001, 1, 1, 6, 0, 0);
            data.DateDailyTrainingTime = new DateTime(2001, 1, 1, 6, 0, 0);
            data.ShipShieldGenerator = false;
            data.ShipReactiveArmor = false;
            data.ShipRepairBots = false;
        }
        else
        {
            data.scrapCount = scrapCount;
            data.selectedWeapon = SelectedWeapon;
            data.researchMaterialCount = researchMaterialCount;
            data.DateDailyResearchTime = DateDailyResearchTime;
            data.DateDailyTrainingTime = DateDailyTrainingTime;
            data.DateDailyScrapBoostTime = DateDailyScrapBoostTime;
            data.ScrapBoostActive = ScrapBoostActive;
            data.ShipShieldGenerator = ShipShieldGenerator;
            data.ShipReactiveArmor = ShipReactiveArmor;
            data.ShipRepairBots = ShipRepairBots;
            data.highestLevelAchieved = highestLevelAchieved;
        }
        data.WeaponSkill = WeaponSkill;
        data.Experience = Experience;
        data.WeaponUnlocked = WeaponUnlocked;
        data.ResearchStarted = ResearchStarted;
        data.StartZoneUnlocked = StartZoneUnlocked;
        data.WeaponUpgrades = WeaponUpgrades;
        data.WeaponUpgradePointsTotal = WeaponUpgradePointsTotal;
        data.WeaponUpgradePointsAvailable = WeaponUpgradePointsAvailable;

        return data;
    }

    private void SetPlayerData(PlayerData data)
    {
        scrapCount = data.scrapCount;
        SelectedWeapon = data.selectedWeapon;
        ScrapBoostActive = data.ScrapBoostActive;
        ShipReactiveArmor = data.ShipReactiveArmor;
        ShipRepairBots = data.ShipRepairBots;
        ShipShieldGenerator = data.ShipShieldGenerator;
        researchMaterialCount = data.researchMaterialCount;
        
        if (data.highestLevelAchieved != 0)
            highestLevelAchieved = data.highestLevelAchieved;

        if (data.WeaponSkill != null)
            if (data.WeaponSkill.Length != 0)
                WeaponSkill = data.WeaponSkill;
        else
            Debug.Log("WeaponSkill[] length is 0");

        if (data.Experience != null)
            if (data.Experience.Length != 0)
                Experience = data.Experience;
        else
            Debug.Log("Experience[] length is 0");

        if (data.WeaponUnlocked != null)
            if (data.WeaponUnlocked.Length != 0)
                WeaponUnlocked = data.WeaponUnlocked;
        else
            Debug.Log("WeaponUnlocked[] length is 0");

        if (data.StartZoneUnlocked != null)
            if (data.StartZoneUnlocked.Count != 0)
                StartZoneUnlocked = data.StartZoneUnlocked;
        else
            Debug.Log("StartZoneUnlocked[] length is 0");

        if (data.WeaponUpgrades != null)
            if (data.WeaponUpgrades.Length != 0)
                WeaponUpgrades = data.WeaponUpgrades;
        else
            Debug.Log("WeaponUpgrades[] length is 0");

        if (data.WeaponUpgradePointsTotal != null)
            if (data.WeaponUpgradePointsTotal.Length != 0)
                WeaponUpgradePointsTotal = data.WeaponUpgradePointsTotal;
        else
            Debug.Log("WeaponUpgradePointsTotal[] length is 0");

        if (data.WeaponUpgradePointsAvailable != null)
            if (data.WeaponUpgradePointsAvailable.Length != 0)
                WeaponUpgradePointsAvailable = data.WeaponUpgradePointsAvailable;
        else
            Debug.Log("WeaponUpgradePointsAvailable[] length is 0");

        if (data.DateDailyResearchTime != null)
            DateDailyResearchTime = data.DateDailyResearchTime;
        else
            Debug.Log("DateDailyResearchTime is null");

        if (data.DateDailyScrapBoostTime != null)
            DateDailyScrapBoostTime = data.DateDailyScrapBoostTime;
        else
            Debug.Log("DateDailyScrapBoostTime is null");

        if (data.DateDailyTrainingTime != null)
            DateDailyTrainingTime = data.DateDailyTrainingTime;
        else
            Debug.Log("DateDailyTrainingTime is null");

        if (data.ResearchStartTimes != null)
            if (data.ResearchStartTimes.Length != 0)
                ResearchStartTimes = data.ResearchStartTimes;
        else
            Debug.Log("ResearchStartTimes[] length is 0");

        if (data.ResearchStarted != null)
            if (data.ResearchStarted.Length != 0)
                ResearchStarted = data.ResearchStarted;
        else
            Debug.Log("ResearchStarted is null");

    }

    public int GetNumberOfPowerUps()
    {
        return numberOfPowerUps;
    }

    public int GetNumberOfWeapons()
    {
        return numberOfWeapons;
    }

    public int GetNumberOfStartZones()
    {
        return numberOfStartZones;
    }

    public int GetNumberOfResearches()
    {
        return numberOfResearches;
    }

    public string GetSoundKey()
    {
        return playerSound;
    }

    public string GetMusicKey()
    {
        return playerMusic;
    }

    public string GetTutorialKey()
    {
        return tutorialKey;
    }

    public string GetSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void ExperienceGained(int amount)
    {
        Experience[SelectedWeapon] += amount * currentLevel;
        if (Experience[SelectedWeapon] >= ExpForSkillUp * WeaponSkill[SelectedWeapon]) //if true -> +Skill point
        {
            Experience[SelectedWeapon] -= ExpForSkillUp * WeaponSkill[SelectedWeapon];
            if (WeaponSkill[SelectedWeapon] < Weapons[SelectedWeapon].SkillCap)
            {
                WeaponSkill[SelectedWeapon] += 1;
                GameObject.FindWithTag("PlayerTurret").GetComponent<TurretScript>().GetTurret().UpdateValues(SelectedWeapon);
                if (SceneManager.GetActiveScene().name =="GameWorld1")
                {
                    GameObject.FindWithTag("PlayerTurret").GetComponent<TurretScript>().WeaponSkillGained();
                }
            }
            else
                WeaponSkill[SelectedWeapon] = Weapons[SelectedWeapon].SkillCap;
        }
    }

    public int ExperienceNeededForSkillUp()
    {
        int expNeeded = ExpForSkillUp * WeaponSkill[SelectedWeapon];
        return expNeeded;
    }

    public void PauseGame(bool resumeWithFalse = true)
    {
        if (resumeWithFalse == true)
        {
            Time.timeScale = 0f;
            GAME_PAUSED = true;
        }
        else
        {
            Time.timeScale = 1f;
            GAME_PAUSED = false;
        }
    }
}

[Serializable]
class PlayerData
{
    // DO NOT MAKE CHANGES HERE
    public int scrapCount;

    public int researchMaterialCount;

    public int[] WeaponSkill;

    public int[] Experience;

    public bool[] WeaponUnlocked;

    public System.Collections.Generic.List<int> StartZoneUnlocked;

    public bool[] ResearchStarted;

    public int[,] WeaponUpgrades;

    public int[] WeaponUpgradePointsTotal;

    public int[] WeaponUpgradePointsAvailable;

    public DateTime DateDailyResearchTime;

    public DateTime DateDailyScrapBoostTime;

    public bool ScrapBoostActive;

    public bool ShipRepairBots;

    public bool ShipShieldGenerator;

    public bool ShipReactiveArmor;

    public DateTime[] ResearchStartTimes;

    public int selectedWeapon;

    public int highestLevelAchieved;

    public DateTime DateDailyTrainingTime;
    
}

[Serializable]
class PlayerDataExtension
{
    public int[] ExtensionData;
}

namespace Asteroids
{
    public enum AsteroidType { NONE, Medium, Big, Huge, Golden }
}

namespace FloatingText
{
    public enum FTType { Normal, PopUp, Announcement, Danger, PowerUp }
}

namespace Scrap
{
    public enum ScrapType { Normal, ResearchMaterial }
}

namespace PUBombs
{
    public enum PUBombType { Gravity, Kinetic }
}

namespace Research
{
    public enum ResearchType { RepairBots, ShieldGenerator, ReactiveArmor, PulseLaser, MassDriver }
}


