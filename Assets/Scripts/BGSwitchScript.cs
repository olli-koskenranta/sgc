using UnityEngine;
using System.Collections;

public class BGSwitchScript : MonoBehaviour
{
    public GameObject[] backgrounds;
    public int backgroundIndex = 0;
    private bool switchingStarted;
    private float alphaValue1;
    private float alphaValue2;


    void Start()
    {
        backgroundIndex = GetBackGroundIndex();


        alphaValue1 = 1f;
        alphaValue2 = 0f;
        backgrounds = new GameObject[5];
        backgrounds[0] = GameObject.Find("Background1");
        backgrounds[1] = GameObject.Find("Background2");
        backgrounds[2] = GameObject.Find("Background3");
        backgrounds[3] = GameObject.Find("Background4");
        backgrounds[4] = GameObject.Find("Background5");
        foreach (GameObject go in backgrounds)
        {
            go.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0f);
        }
        backgrounds[backgroundIndex].GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
    }

    void Update()
    {
        if (backgroundIndex != GetBackGroundIndex() && !switchingStarted)
        {
            switchingStarted = true;
        }

        if (switchingStarted)
        {
            
            alphaValue1 -= 0.005f;
            if (alphaValue1 <= 0f)
                alphaValue1 = 0f;
                
            alphaValue2 += 0.005f;
            if (alphaValue2 >= 1f)
                alphaValue2 = 1f;

            backgrounds[backgroundIndex].GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, alphaValue1);

            if (backgroundIndex < backgrounds.Length - 1)
            {
                backgrounds[backgroundIndex + 1].GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, alphaValue2);
            }
            else
            {
                backgrounds[0].GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, alphaValue2);
            }

            if (alphaValue1 == 0f && alphaValue2 == 1f)
            {
                switchingStarted = false;
                alphaValue1 = 1f;
                alphaValue2 = 0f;

                backgroundIndex = GetBackGroundIndex();
            }
            
        }

    }

    private int GetBackGroundIndex()
    {
        int index = GameControl.gc.currentLevel;
        if (index >= 50)
        {
            index -= 50 * index / 50;
        }
        index /= 10;
        return index;
    }


}
