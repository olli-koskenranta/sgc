using UnityEngine;
using System.Collections;

public class MusicScript : MonoBehaviour {

    public AudioSource track1;
    public AudioSource track2;
	void Start ()
    {
        if (GameControl.gc.AUDIO_MUSIC)
            track1.Play();
	}

    public void PlayTrack(int number)
    {
        if (!GameControl.gc.AUDIO_MUSIC)
            return;

        if (number == 1)
        {
            track2.Stop();
            track1.Play();
        }
        else if (number == 2)
        {
            track1.Stop();
            track2.Play();
        }
    }
}
