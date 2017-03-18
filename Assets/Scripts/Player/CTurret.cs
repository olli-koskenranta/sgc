using UnityEngine;

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
        private float damageAccumulation;
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
            float crit_multiplier, int weapon_type, SpecialType special_type = SpecialType.NONE, string special1 = "UNDEFINED", string special2 = "UNDEFINED")
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
            damageAccumulation = 0;
        }

        public void SetUpgradeValuesPerSkillPoint(int damage, float mass, float crit)
        {
            uDamage = damage;
            uMass = mass;
            uCritChance = crit;
        }

        public void UpdateValues(int weaponNumber)
        {
            int skillLevel = GameControl.gc.WeaponSkill[weaponNumber];
            totalDamage = baseDamage + skillLevel * uDamage;

            if (weaponNumber == 1) //Pulse Laser has no mass
                totalMass = 0;
            else
                totalMass = baseMass + skillLevel * uMass;


            totalCritChance = baseCritChance + skillLevel * uCritChance;
            float overflow = 0;
            if (totalCritChance > 100)
            {
                overflow = totalCritChance - 100;
                totalCritChance = 100;
            }
            UpdateUpgrades(overflow);
        }

        private void UpdateUpgrades(float critOverflow)
        {
            //0 = attack speed
            //1 = mass
            //2 = damage
            //3 = crit multiplier
            //4 = unique ability
            //5 = special chance
            //6 = skill cap increase
            totalROF = baseROF - GameControl.gc.AttackSpeedReductions[WeaponType] * GameControl.gc.WeaponUpgrades[WeaponType, 0];
            totalMass += totalMass * GameControl.gc.WeaponUpgrades[WeaponType, 1];
            totalDamage += (int)((float)totalDamage * (float)GameControl.gc.WeaponUpgrades[WeaponType, 2] * 0.25f);
            totalCritMultiplier = baseCritMultiplier + GameControl.gc.WeaponUpgrades[WeaponType, 3] * 0.5f + critOverflow / 100;
            totalSpecialChance = baseSpecialChance * GameControl.gc.WeaponUpgrades[WeaponType, 5];

            skillCap = 100 + 10 * GameControl.gc.WeaponUpgrades[WeaponType, 6];
            //Debug.Log("Skill cap: " + skillCap.ToString());


            switch (WeaponType) //0 = blaster, 1 = laser, 2 = mass driver, 3 = plasma
            {
                case 0:
                    bounces = GameControl.gc.WeaponUpgrades[WeaponType, 4];
                    break;
                case 1:
                    totalSpecial2Chance = baseSpecialChance * GameControl.gc.WeaponUpgrades[WeaponType, 4];
                    break;
                case 2:
                    damageAccumulation = 0.01f * GameControl.gc.WeaponUpgrades[WeaponType, 4];
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

        public float DamageAccumulation
        {
            get { return damageAccumulation; }
        }

        public string[] SpecialNames
        {
            get { return specials; }
        }
    }
}
