using UnityEngine;
using System.Collections;

public class BGSwitchScript : MonoBehaviour {

    GameObject bg1, bg2;
    Color newColor1, newColor2;
    MeshRenderer mr;
    Material nm;
	// Use this for initialization
	void Start () {
        bg1 = GameObject.Find("Background1");
        bg2 = GameObject.Find("Background2");

        mr = bg1.GetComponent<MeshRenderer>();

        newColor1 = Color.red;
        nm = new Material(Shader.Find("Unlit/Transparent"));
        nm.color = newColor1;
        mr.material = nm;
        //newColor1 = bg1.GetComponent<MeshRenderer>().material.shader;
        //newColor2 = bg2.GetComponent<MeshRenderer>().material.color;

        newColor2.a = 0;
        //bg2.GetComponent<MeshRenderer>().material.color = newColor2;
    }
	
	// Update is called once per frame
	void Update () {
        //bg1.GetComponent<MeshRenderer>().material.color = newColor1;
        //bg2.GetComponent<MeshRenderer>().material.color = newColor2;

        newColor1.a -= 0.001f;
        //newColor2.a += 0.001f;


    }
}
