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
    public Button buttonShipUpgrades;
    public Button zonePlus;
    public Button zoneMinus;
    public Button btnBuyPoints;

    public enum ArmoryView { Weapon, Ship };
    public ArmoryView view;

    private GameObject[] WeaponUpgradeUIElements;
    private GameObject[] ShipUpgradeUIElements;

    public Slider[] upgradeSliders = new Slider[6];

	void Start ()
    {
        view = ArmoryView.Weapon;
        WeaponUpgradeUIElements = FindGameObjectsWithLayer(12);
        ShipUpgradeUIElements = FindGameObjectsWithLayer(13);
        ShowShipUI(false);
        SetTextsAndUpdate();
        UpdateSliders();
	}

    public void SetTextsAndUpdate()
    {

        textInfo.text = "Scrap Count: " + GameControlScript.gameControl.scrapCount.ToString()
            + "\nResearch Material: " + GameControlScript.gameControl.researchMaterialCount.ToString()
            + "\n\nSelected Weapon: " + GameControlScript.gameControl.WeaponNames[GameControlScript.gameControl.SelectedWeapon]
            + "\nWeapon Skill: " + GameControlScript.gameControl.WeaponSkill[GameControlScript.gameControl.SelectedWeapon].ToString() + "/" + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].SkillCap.ToString()
                + "(+" + (GameControlScript.gameControl.WeaponUpgrades[GameControlScript.gameControl.SelectedWeapon, 6] * 100).ToString() + ")"
            + "\nCritical Chance: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].CriticalChance.ToString() + "%"
            + "\nRate of Fire: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].RateOfFire.ToString()
            + "\nProjectile Mass: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].Mass.ToString()
                + "(+" + (GameControlScript.gameControl.WeaponUpgrades[GameControlScript.gameControl.SelectedWeapon, 1] * 100).ToString() + "%)"
            + "\nWeapon Damage: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].Damage.ToString()
                + "(+" + (GameControlScript.gameControl.WeaponUpgrades[GameControlScript.gameControl.SelectedWeapon, 2] * 25).ToString() + "%)"
            + "\nCritical Multiplier: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].CriticalMultiplier.ToString()
            + "\nSpecial Chance: " + GameControlScript.gameControl.Weapons[GameControlScript.gameControl.SelectedWeapon].SpecialChance.ToString() + "%"
            
            
            
            
            
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

    public void SelectWeaponClicked(int weaponNumber)
    {
        if (view != ArmoryView.Weapon)
        {
            view = ArmoryScript.ArmoryView.Weapon;
            ShowWeaponUI(true);
            ShowShipUI(false);
        }

        if (GameControlScript.gameControl.WeaponUnlocked[weaponNumber])
        {
            GameControlScript.gameControl.SelectedWeapon = weaponNumber;
            SetTextsAndUpdate();
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
            SetTextsAndUpdate();
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

    public void ZonePlusClicked()
    {
        GameControlScript.gameControl.currentLevel += 1;
        SetTextsAndUpdate();
    }

    public void ZoneMinusClicked()
    {
        if (GameControlScript.gameControl.currentLevel > 2)
            GameControlScript.gameControl.currentLevel -= 1;

        SetTextsAndUpdate();
    }

    public void BuyPointsClicked()
    {
        int scrapCost = GameControlScript.gameControl.UpgradePointCost(0);
        int RMCost = GameControlScript.gameControl.UpgradePointCost(1);

        if (GameControlScript.gameControl.scrapCount >= scrapCost && GameControlScript.gameControl.researchMaterialCount >= RMCost)
        {
            GameControlScript.gameControl.scrapCount -= scrapCost;
            GameControlScript.gameControl.researchMaterialCount -= RMCost;

            GameControlScript.gameControl.WeaponUpgradePointsTotal[GameControlScript.gameControl.SelectedWeapon] += 1;
            SetTextsAndUpdate();
        }
        else
            Debug.Log("Not enough materials!");
    }
}
