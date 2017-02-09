using UnityEngine;
using UnityEngine.UI;
using ShipWeapons;
using System.Collections;

public class ArmoryScript : MonoBehaviour {

    public Text textInfo;
    public Text textUpgPoints;

    public Button selectWeapon0;
    public Button selectWeapon1;
    public Button selectWeapon2;
    public Button zonePlus;
    public Button zoneMinus;
    public Button btnBuyPoints;

    public Slider[] upgradeSliders = new Slider[6];

	void Start ()
    {
        SetTextsAndUpdate();
        UpdateSliders();
	}

    public void SetTextsAndUpdate()
    {

        //int upgradeCostMass = GameControlScript.gameControl.UpgCost + GameControlScript.gameControl.UpgCost * GameControlScript.gameControl.MassUpgrades;
        //int upgradeCostArmor = GameControlScript.gameControl.UpgCost + GameControlScript.gameControl.UpgCost * GameControlScript.gameControl.ArmorUpgrades;
        //int upgradeCostDamage = GameControlScript.gameControl.UpgCost + GameControlScript.gameControl.UpgCost * GameControlScript.gameControl.DamageUpgrades;
        //int upgradeCostROF = GameControlScript.gameControl.UpgCost + GameControlScript.gameControl.UpgCost * GameControlScript.gameControl.RateOfFireUpgrades;

        textInfo.text = "Scrap Count: " + GameControlScript.gameControl.scrapCount.ToString()
            + "\nResearch Material: " + GameControlScript.gameControl.researchMaterialCount.ToString()
            + "\n\nSelected Weapon: " + GameControlScript.gameControl.WeaponNames[GameControlScript.gameControl.SelectedWeapon]
            + "\nWeapon Skill: " + GameControlScript.gameControl.WeaponSkill[GameControlScript.gameControl.SelectedWeapon].ToString() + "/" + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].SkillCap.ToString()
            + "\nWeapon Damage: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].Damage.ToString()
            + "\nCritical Chance: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].CriticalChance.ToString()
            + "\nCritical Multiplier: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].CriticalMultiplier.ToString()
            + "\nRate of Fire: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].RateOfFire.ToString()
            + "\nProjectile Mass: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].Mass.ToString()
            + "\nSpecial Chance: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].SpecialChance.ToString()
            + "\nStart Zone: " + GameControlScript.gameControl.currentLevel.ToString();

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

        zonePlus.GetComponentInChildren<Text>().text = "Start Zone+";
        zoneMinus.GetComponentInChildren<Text>().text = "Start Zone-";

        textUpgPoints.text = GameControlScript.gameControl.WeaponNames[GameControlScript.gameControl.SelectedWeapon] + " Upgrade Points: " + UpgPointsAvailable().ToString();
        btnBuyPoints.GetComponentInChildren<Text>().text = "Buy Points\n" + GameControlScript.gameControl.UpgradePointCost(0).ToString() + "S/" + GameControlScript.gameControl.UpgradePointCost(1).ToString() + "RM";


        //GameControlScript.gameControl.UpdatePlayerAttributes();
        


    }

    public void UpdateSliders()
    {
        for (int i = 0; i <= 5; i++)
        {
            upgradeSliders[i].value = GameControlScript.gameControl.WeaponUpgrades[GameControlScript.gameControl.SelectedWeapon, i];
        }
    }

    public void UpgradeSliderChanged(int sliderNumber)
    {
        //Debug.Log("Is this called");
        GameControlScript.gameControl.WeaponUpgrades[GameControlScript.gameControl.SelectedWeapon, sliderNumber] = (int)upgradeSliders[sliderNumber].value;
        GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].UpdateValues(GameControlScript.gameControl.SelectedWeapon);
        SetTextsAndUpdate();

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
}
