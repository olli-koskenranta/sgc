using UnityEngine;

public class BGScrollScript : MonoBehaviour {

    public float scrollspeed = 0.003f;
	void Update () {
        Vector2 offset;
        offset = GetComponent<MeshRenderer>().material.mainTextureOffset;
        offset.x = (Time.time * scrollspeed) % 1;
        GetComponent<MeshRenderer>().material.mainTextureOffset = offset;
    }
}
