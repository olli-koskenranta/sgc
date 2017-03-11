using UnityEngine;
using System.Collections;

public class BGScrollScript : MonoBehaviour {

    public float scrollspeed = 0.003f;
	void Update () {
        GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2((Time.time * scrollspeed) % 1, 0f);
    }
}
