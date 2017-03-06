using UnityEngine;
using UnityEngine.SceneManagement;
using ShipWeapons;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour {

    public const int numberOfPowerUps = 4;
    public const int numberOfWeapons = 4;
    public const int numberOfStartZones = 11;
    public const int numberOfResearches = 5;

    public static GameControl gc;
    public int scrapCount = 0;
    public int researchMaterialCount = 0;
    public int currentLevel;
    public int scrapRequiredForNextLevel;

    public int[] Experience;
    public int[] WeaponSkill;
    public int ExpForSkillUp;
    public string GameVersion = "0.8";
    

    public int[] ResearchScrapCost;
    public int[] ResearchRMCost;

    public DateTime DateDailyResearchTime;
    public DateTime DateDailyScrapBoostTime;
    public DateTime[] ResearchStartTimes;
    public bool[] ResearchStarted;

    public bool ScrapBoostActive;
    public bool GAME_PAUSED;

    public int highestLevelAchieved;
    
    



    //Player status
    public bool PLAYER_ALIVE = true;

    //Player attributes
    public int shipArmor;
    public bool[] StartZoneUnlocked;
    public int[] startZones;
    


    //Weapon types
    //0 = Basic Cannon
    //1 = Laser Cannon
    //2 = Mass Driver
    //3 = Plasma Cannon
    public int SelectedWeapon = 0;

    //Player upgrades
    public int ArmorUpgrades;
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
    public bool AUDIO_SOUNDS;
    public bool AUDIO_MUSIC;

    public const string playerSound = "playerSound";
    public const string playerMusic  = "playerMusic";

    //Power Ups
    //0 = Gravity Gun
    //1 = Gravity Bomb
    //2 = Scrap Bonus
    //3 = Slow Meteors
    //4 = Cluster Projectile
    //5 = Repel Shield,
    //6 = Attack speed

    public bool[] PowerUps;
    public string[] PowerUpNames;

    void Awake()
    {
        

        if (gc == null)
        {
            DontDestroyOnLoad(gameObject);
            gc = this;

            

        }
        else if (gc != this)
        {
            Destroy(gameObject);
        }

    }

    void OnApplicationQuit()
    {
        SaveData();
        Debug.Log("Application quit!");
    }

    
    void Start()
    {
        //Debug.Log("GameControl START()!");
        GAME_PAUSED = false;

        WeaponUpgradeCosts = new int[] { 1000, 10000, 100000, 1000000 };
        WeaponUpgradeRMCosts = new int[] { 0, 1, 5, 10 };

        highestLevelAchieved = 1;

        /*
         * RepairBots = 0
         * ShieldGenerator = 1
         * ReactiveArmor = 2
         * PulseLaser = 3
         * MassDriver = 4
         */
        ResearchScrapCost = new int[numberOfResearches] { 100000, 200000, 50000, 100000, 1000000 };
        ResearchRMCost = new int[numberOfResearches] { 10, 20, 5, 10, 50 };

        Experience = new int[numberOfWeapons];
        PowerUps = new bool[numberOfPowerUps];
        PowerUpNames = new string[numberOfPowerUps] { "Kinetic Bomb", "Shield", "Gravity Bomb", "Max Weapon Skill" }; //, "Slow Meteors", "Cluster Projectile", "Repel Shield", "Attack Speed" };
        startZones = new int[] { 1, 5, 11, 21, 31, 41, 51, 61, 71, 81, 91 };
        WeaponSkill = new int[numberOfWeapons];
        WeaponUnlocked = new bool[numberOfWeapons] { true, false, false, false };
        ResearchStarted = new bool[numberOfResearches] { false, false, false, false, false };
        
        AttackSpeedReductions = new float[] { 0.15f, 0.05f, 0.025f, 0 };
        ExpForSkillUp = 10;
        currentLevel = 1;
        scrapRequiredForNextLevel = 200;
        DateDailyResearchTime = new DateTime(2001, 1, 1, 6, 0, 0);
        DateDailyScrapBoostTime = new DateTime(2001, 1, 1, 6, 0, 0);
        ResearchStartTimes = new DateTime[numberOfResearches];
        
        ScrapBoostActive = false;

        if (PlayerPrefs.GetInt(playerSound, 1) == 1)
            AUDIO_SOUNDS = true;
        else
            AUDIO_SOUNDS = false;

        if (PlayerPrefs.GetInt(playerMusic, 1) == 1)
            AUDIO_MUSIC = true;
        else
            AUDIO_MUSIC = false;

        ShipRepairBots = false;
        ShipShieldGenerator = false;
        ShipReactiveArmor = false;

        WeaponUpgrades = new int[numberOfWeapons, 7];
        WeaponUpgradePointsTotal = new int[numberOfWeapons];
        WeaponUpgradePointsAvailable = new int[numberOfWeapons];

        StartZoneUnlocked = new bool[numberOfStartZones] { true, false, false, false, false, false, false, false, false, false, false };
        

        ClearArrays();

        Weapons = new Turret[numberOfWeapons];

        //Create weapons
        BlasterTurret = new Turret(10f, 2, 1, 1, 5, 2f, 0, SpecialType.GravityDamage, "Gravity Blast", "Bounces");
        BlasterTurret.SetUpgradeValuesPerSkillPoint(2, 1, 0.2f);

        PulseLaserTurret = new Turret(20f, 100, 0.0001f, 0.4f, 5, 2f, 1, SpecialType.Piercing, "Piercing", "Beam Split");
        PulseLaserTurret.SetUpgradeValuesPerSkillPoint(3, 0, 0.2f);

        MassDriverTurret = new Turret(30f, 50, 0.1f, 0.2f, 5f, 2f, 2, SpecialType.Shrapnel, "Shrapnel", "Armor Piercing");
        MassDriverTurret.SetUpgradeValuesPerSkillPoint(4, 0.1f, 0.2f);

        PlasmaTurret = new Turret(10f, 10, 3, 1, 5, 5, 3, SpecialType.Shrapnel);
        PlasmaTurret.SetUpgradeValuesPerSkillPoint(10, 3, 0.2f);

        Weapons[0] = BlasterTurret;
        Weapons[1] = PulseLaserTurret;
        Weapons[2] = MassDriverTurret;
        Weapons[3] = PlasmaTurret;


        ResetPowerUps();
        LoadData();

        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].UpdateValues(i);
        }

    }

    public void UpdatePlayerAttributes()
    {
        shipArmor = ArmorUpgrades;
    }
    

    public void SaveData()
    {
        
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SGC.dat");

        PlayerData playerData = GetPlayerData();

     

        bf.Serialize(file, playerData);
        file.Close();
        Debug.Log("Player data saved!");
        
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
            /*if (playerData.GameVersion == null || playerData.GameVersion.Equals(""))
            {
                Debug.Log("New game version found, resetting data!");
                ResetData();
                return;
            }
            else
            {
                if (!playerData.GameVersion.Equals(GameVersion))
                {
                    Debug.Log("New game version found, resetting data!");
                    ResetData();
                    return;
                }
            }*/

        }
        else
        {
            Debug.Log("Player data not found!");
            ResetData();
            return;
        }
    }

    public void ResetData()
    {
        
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SGC.dat");

        //WeaponUpgrades = new int[4, 7];
        ClearArrays();
        WeaponUnlocked = new bool[numberOfWeapons] { true, false, false, false };
        StartZoneUnlocked = new bool[numberOfStartZones] { true, false, false, false, false, false, false, false, false, false, false };
        ResearchStarted = new bool[numberOfResearches] { false, false, false, false, false };

        PlayerData playerData = GetPlayerData(true);

        bf.Serialize(file, playerData);
        file.Close();
        Debug.Log("Player data RESET!");
        LoadData();
        //SaveData();

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
        Array.Clear(WeaponUpgradePointsAvailable, 0, WeaponUpgradePointsAvailable.Length);
        Array.Clear(ResearchStartTimes, 0, ResearchStartTimes.Length);
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
            data.shipArmor = 0;
            data.selectedWeapon = 0;
            data.ArmorUpgrades = 0;
            data.researchMaterialCount = 0;
            data.highestLevelAchieved = 1;
            data.ScrapBoostActive = false;
            data.DateDailyResearchTime = new DateTime(2001, 1, 1, 6, 0, 0);
            data.DateDailyScrapBoostTime = new DateTime(2001, 1, 1, 6, 0, 0);
            data.ShipShieldGenerator = false;
            data.ShipReactiveArmor = false;
            data.ShipRepairBots = false;
        }
        else
        {
            data.scrapCount = scrapCount;
            data.shipArmor = shipArmor;
            data.selectedWeapon = SelectedWeapon;
            data.ArmorUpgrades = ArmorUpgrades;
            data.researchMaterialCount = researchMaterialCount;
            data.DateDailyResearchTime = DateDailyResearchTime;
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
        data.GameVersion = GameVersion;

        return data;
    }

    private void SetPlayerData(PlayerData data)
    {
        scrapCount = data.scrapCount;
        shipArmor = data.shipArmor;
        SelectedWeapon = data.selectedWeapon;
        ArmorUpgrades = data.ArmorUpgrades;
        ScrapBoostActive = data.ScrapBoostActive;
        ShipReactiveArmor = data.ShipReactiveArmor;
        ShipRepairBots = data.ShipRepairBots;
        ShipShieldGenerator = data.ShipShieldGenerator;
        researchMaterialCount = data.researchMaterialCount;

        if (data.highestLevelAchieved != 0)
            highestLevelAchieved = data.highestLevelAchieved;

        if (data.WeaponSkill != null)
            WeaponSkill = data.WeaponSkill;
        else
            Debug.Log("WeaponSkill[] is null");
        if (data.Experience != null)
            Experience = data.Experience;
        else
            Debug.Log("Experience[] is null");
        if (data.WeaponUnlocked != null)
            WeaponUnlocked = data.WeaponUnlocked;
        else
            Debug.Log("WeaponUnlocked[] is null");
        if (data.StartZoneUnlocked != null)
            StartZoneUnlocked = data.StartZoneUnlocked;
        else
            Debug.Log("StartZoneUnlocked[] is null");
        if (data.WeaponUpgrades != null)
            WeaponUpgrades = data.WeaponUpgrades;
        else
            Debug.Log("WeaponUpgrades[] is null");
        if (data.WeaponUpgradePointsTotal != null)
            WeaponUpgradePointsTotal = data.WeaponUpgradePointsTotal;
        else
            Debug.Log("WeaponUpgradePointsTotal[] is null");
        if (data.WeaponUpgradePointsAvailable != null)
            WeaponUpgradePointsAvailable = data.WeaponUpgradePointsAvailable;
        else
            Debug.Log("WeaponUpgradePointsAvailable[] is null");
        if (data.DateDailyResearchTime != null)
            DateDailyResearchTime = data.DateDailyResearchTime;
        else
            Debug.Log("DateDailyResearchTime is null");
        if (data.DateDailyScrapBoostTime != null)
            DateDailyScrapBoostTime = data.DateDailyScrapBoostTime;
        else
            Debug.Log("DateDailyScrapBoostTime is null");
        if (data.ResearchStartTimes != null)
            ResearchStartTimes = data.ResearchStartTimes;
        else
            Debug.Log("ResearchStartTimes[] is null");
        if (data.ResearchStarted != null)
            ResearchStarted = data.ResearchStarted;
        else
            Debug.Log("ResearchStarted is null");


    }

    public void ResetPowerUps()
    {
        
        for (int i = 0; i < PowerUps.Length; i++)
            PowerUps[i] = false;
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
    public int scrapCount;

    public int researchMaterialCount;

    public int shipArmor;

    public int ArmorUpgrades;

    public int[] WeaponSkill = new int[GameControl.gc.GetNumberOfWeapons()];

    public int[] Experience = new int[GameControl.gc.GetNumberOfWeapons()];

    public bool[] WeaponUnlocked = new bool[GameControl.gc.GetNumberOfWeapons()];

    public bool[] StartZoneUnlocked = new bool[GameControl.gc.GetNumberOfStartZones()];

    public bool[] ResearchStarted = new bool[GameControl.gc.GetNumberOfResearches()];

    public int[,] WeaponUpgrades = new int[GameControl.gc.GetNumberOfWeapons(), 7];

    public int[] WeaponUpgradePointsTotal = new int[GameControl.gc.GetNumberOfWeapons()];

    public int[] WeaponUpgradePointsAvailable = new int[GameControl.gc.GetNumberOfWeapons()];

    public DateTime DateDailyResearchTime;

    public DateTime DateDailyScrapBoostTime;

    public bool ScrapBoostActive;

    public bool ShipRepairBots;

    public bool ShipShieldGenerator;

    public bool ShipReactiveArmor;

    public DateTime[] ResearchStartTimes = new DateTime[GameControl.gc.GetNumberOfResearches()];

    public int selectedWeapon;

    public int highestLevelAchieved;

    public string GameVersion;
}

namespace Asteroids
{
    public enum AsteroidType { NONE, Scrap, Medium, Big, Huge, Anomaly1 }
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


