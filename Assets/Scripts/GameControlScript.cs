using UnityEngine;
using UnityEngine.SceneManagement;
using ShipWeapons;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControlScript : MonoBehaviour {

    public const int numberOfPowerUps = 4;
    public const int numberOfWeapons = 4;
    public const int numberOfStartZones = 11;

    public static GameControlScript gameControl;
    public int scrapCount = 0;
    public int researchMaterialCount = 0;
    public int currentLevel;
    public int scrapRequiredForNextLevel;

    public int[] Experience;
    public int[] WeaponSkill;
    public int ExpForSkillUp;
    private string GameVersion = "0.6c";
    

    public int[] WeaponScrapCost;
    public int[] WeaponRMCost;

    public DateTime DateDailyResearchTime;
    public DateTime DateDailyScrapBoostTime;

    public bool ScrapBoostActive;


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



    //Player status
    public bool PLAYER_ALIVE = true;

    //Player attributes
    public int shipArmor;
    public bool[] StartZoneUnlocked;
    public int[] startZones;
    public bool ShipRepairBots;
    public bool ShipShieldGenerator;
    public bool ShipReactiveArmor;


    //Weapon types
    //0 = Basic Cannon
    //1 = Laser Cannon
    //2 = Mass Driver
    //3 = Plasma Cannon
    public int SelectedWeapon = 0;

    //Player upgrades
    public int ArmorUpgrades;

    //Player weapons
    public Turret[] Weapons;
    public Turret BlasterTurret;
    public Turret PulseLaserTurret;
    public Turret MassDriverTurret;
    public Turret PlasmaTurret;
    public bool[] WeaponUnlocked;
    public string[] WeaponNames = new string[] { "Blaster", "Pulse Laser", "Mass Driver", "Plasma" };
    public float[] AttackSpeedReductions;
    public int[] WeaponUpgradeCosts;
    public int[] WeaponUpgradeRMCosts;

    //Preferences
    public bool AUDIO_SOUNDS = false;
    public bool AUDIO_MUSIC = false;

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
        

        if (gameControl == null)
        {
            DontDestroyOnLoad(gameObject);
            gameControl = this;

            

        }
        else if (gameControl != this)
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
        WeaponUpgradeCosts = new int[] { 1000, 10000, 100000, 1000000 };
        WeaponUpgradeRMCosts = new int[] { 0, 1, 5, 10 };
        WeaponScrapCost = new int[numberOfWeapons] { 0, 20000, 100000, 1000000 };
        WeaponRMCost = new int[numberOfWeapons] { 0, 1, 5, 50 };
        Experience = new int[numberOfWeapons];
        PowerUps = new bool[numberOfPowerUps];
        PowerUpNames = new string[numberOfPowerUps] { "Kinetic Bomb", "Repel Shield", "Gravity Bomb", "Max Weapon Power" }; //, "Slow Meteors", "Cluster Projectile", "Repel Shield", "Attack Speed" };
        startZones = new int[] { 1, 5, 11, 21, 31, 41, 51, 61, 71, 81, 91 };
        WeaponSkill = new int[numberOfWeapons];
        WeaponUnlocked = new bool[numberOfWeapons] { true, false, false, false };
        
        AttackSpeedReductions = new float[] { 0.15f, 0.05f, 0.025f, 0 };
        ExpForSkillUp = 10;
        currentLevel = 1;
        scrapRequiredForNextLevel = 200;
        DateDailyResearchTime = new DateTime(2001, 1, 1, 6, 0, 0);
        DateDailyScrapBoostTime = new DateTime(2001, 1, 1, 6, 0, 0);
        ScrapBoostActive = false;

        AUDIO_SOUNDS = false;
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

        MassDriverTurret = new Turret(30f, 50, 0.1f, 0.2f, 5f, 2f, 2, SpecialType.Shrapnel, "Shrapnel", "THINK OF SOMETHING");
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
            if (playerData.GameVersion == null)
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
            }

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

        PlayerData playerData = GetPlayerData(true);

        bf.Serialize(file, playerData);
        file.Close();
        Debug.Log("Player data RESET!");
        LoadData();
        SaveData();

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
        }
        data.WeaponSkill = WeaponSkill;
        data.Experience = Experience;
        data.WeaponUnlocked = WeaponUnlocked;
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

        if (data.WeaponSkill != null)
            WeaponSkill = data.WeaponSkill;
        if (data.Experience != null)
            Experience = data.Experience;
        researchMaterialCount = data.researchMaterialCount;
        if (data.WeaponUnlocked != null)
            WeaponUnlocked = data.WeaponUnlocked;
        if (data.StartZoneUnlocked != null)
            StartZoneUnlocked = data.StartZoneUnlocked;
        if (data.WeaponUpgrades != null)
            WeaponUpgrades = data.WeaponUpgrades;
        if (data.WeaponUpgradePointsTotal != null)
            WeaponUpgradePointsTotal = data.WeaponUpgradePointsTotal;
        if (data.WeaponUpgradePointsAvailable != null)
            WeaponUpgradePointsAvailable = data.WeaponUpgradePointsAvailable;
        if (data.DateDailyResearchTime != null)
            DateDailyResearchTime = data.DateDailyResearchTime;
        if (data.DateDailyScrapBoostTime != null)
            DateDailyScrapBoostTime = data.DateDailyScrapBoostTime;
            
        
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
}

[Serializable]
class PlayerData
{
    public int scrapCount;

    public int researchMaterialCount;

    public int shipArmor;

    public int ArmorUpgrades;

    public int[] WeaponSkill = new int[GameControlScript.gameControl.GetNumberOfWeapons()];

    public int[] Experience = new int[GameControlScript.gameControl.GetNumberOfWeapons()];

    public bool[] WeaponUnlocked = new bool[GameControlScript.gameControl.GetNumberOfWeapons()];

    public bool[] StartZoneUnlocked = new bool[GameControlScript.gameControl.GetNumberOfStartZones()];

    public int[,] WeaponUpgrades = new int[GameControlScript.gameControl.GetNumberOfWeapons(), 7];

    public int[] WeaponUpgradePointsTotal = new int[GameControlScript.gameControl.GetNumberOfWeapons()];

    public int[] WeaponUpgradePointsAvailable = new int[GameControlScript.gameControl.GetNumberOfWeapons()];

    public DateTime DateDailyResearchTime;

    public DateTime DateDailyScrapBoostTime;

    public bool ScrapBoostActive;

    public bool ShipRepairBots;

    public bool ShipShieldGenerator;

    public bool ShipReactiveArmor;

    public int selectedWeapon;

    

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

namespace ShipUpgrades
{
    public enum ShipUpgradeType { RepairBots, ShieldGenerator, ReactiveArmor }
}

namespace ShipWeapons
{
    public enum SpecialType { NONE, GravityDamage, Piercing, Shrapnel }

    public class Turret
    {
        //Base stats
        private float baseSpeed;
        private int baseDamage;
        private float baseMass;
        private float baseROF;
        private float baseCritChance;
        private float baseCritMultiplier;
        private float baseSpecialChance;
        private SpecialType specialType;
        private int WeaponType;
        private float fireTime;
        private int bounces;
        private int skillCap;

        //Special names
        string[] specials;

        //Upgrades per skill point
        private int uDamage;
        private float uMass;
        private float uROF;
        private float uCritChance;
        private float uSpecialChance;

        //Upgraded stats
        private int totalDamage;
        private float totalMass;
        private float totalROF;
        private float totalCritChance;
        private float totalSpecialChance;
        private float totalSpecial2Chance;
        private float totalCritMultiplier;

        public Turret(float speed, int damage, float mass, float rof, float crit_chance,
            float crit_multiplier, int weapon_type, SpecialType special_type = SpecialType.NONE, string special1 = "UNDEFINED", string special2="UNDEFINED")
        {
            specials = new string[] { special1, special2 };
            baseSpeed = speed;
            baseDamage = damage;
            baseMass = mass;
            baseROF = rof;
            baseCritChance = crit_chance;
            baseCritMultiplier = crit_multiplier;
            baseSpecialChance = 25;
            WeaponType = weapon_type;
            specialType = special_type;
            bounces = 0;
            skillCap = 100;
            fireTime = Time.time;
        }

        public void SetUpgradeValuesPerSkillPoint(int damage, float mass, float crit)
        {
            uDamage = damage;
            uMass = mass;
            uCritChance = crit;
        }

        public void UpdateValues(int weaponNumber)
        {
            //Debug.Log("Updating weapon number " + weaponNumber.ToString());
            int skillLevel = GameControlScript.gameControl.WeaponSkill[weaponNumber];
            totalDamage = baseDamage + skillLevel * uDamage;
            totalMass = baseMass + skillLevel * uMass;
            totalCritChance = baseCritChance + skillLevel * uCritChance;
            UpdateUpgrades();

            
        }

        private void UpdateUpgrades()
        {
            //0 = attack speed
            //1 = mass
            //2 = damage
            //3 = crit multiplier
            //4 = unique ability
            //5 = special chance
            //6 = skill cap increase
            totalROF = baseROF - GameControlScript.gameControl.AttackSpeedReductions[WeaponType] * GameControlScript.gameControl.WeaponUpgrades[WeaponType, 0];
            totalMass += totalMass * GameControlScript.gameControl.WeaponUpgrades[WeaponType, 1];
            totalDamage += (int)((float)totalDamage * (float)GameControlScript.gameControl.WeaponUpgrades[WeaponType, 2] * 0.25f);
            totalCritMultiplier = baseCritMultiplier + GameControlScript.gameControl.WeaponUpgrades[WeaponType, 3] * 0.5f;
            totalSpecialChance = baseSpecialChance * GameControlScript.gameControl.WeaponUpgrades[WeaponType, 5];

            skillCap = 100 + 5 * GameControlScript.gameControl.WeaponUpgrades[WeaponType, 6];
            //Debug.Log("Skill cap: " + skillCap.ToString());
   

            switch (WeaponType) //0 = blaster, 1 = laser, 2 = mass driver, 3 = plasma
            {
                case 0:
                    bounces = GameControlScript.gameControl.WeaponUpgrades[WeaponType, 4];
                    break;
                case 1:
                    totalSpecial2Chance = baseSpecialChance * GameControlScript.gameControl.WeaponUpgrades[WeaponType, 4];
                    break;
                default:
                    break;
            }
        }

        public int SkillCap
        {
            get { return skillCap; }
            //set { skillCap = value; }
        }

        public int Bounces
        {
            get { return bounces; }
            //set { bounces = value; }
        }

        public float Speed
        {
            get { return baseSpeed; }
            //set { baseSpeed = value; }
        }

        public int Damage
        {
            get { return totalDamage; }
            //set { totalDamage = value; }
        }

        public float Mass
        {
            get { return totalMass; }
            //set { totalMass = value; }
        }

        public float RateOfFire
        {
            get { return totalROF; }
            //set { totalROF = value; }
        }

        public float CriticalChance
        {
            get { return totalCritChance; }
            //set { totalCritChance = value; }
        }

        public float CriticalMultiplier
        {
            get { return totalCritMultiplier; }
            //set { baseCritMultiplier = value; }
        }

        public float SpecialChance
        {
            get { return totalSpecialChance; }
            //set { totalSpecialChance = value; }
        }

        public float Special2Chance
        {
            get { return totalSpecial2Chance; }
        }

        public SpecialType SpecialType
        {
            get { return specialType; }
            //set { specialType = value; }
        }

        public float FireTime
        {
            get { return fireTime; }
            set { fireTime = value; }
        }

        public string[] SpecialNames
        {
            get { return specials; }
        }
    }
}
