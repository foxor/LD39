using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip music;
    public AudioSource source;

	void Start ()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(MusicCoroutine());
	}

    protected IEnumerator MusicCoroutine()
    {
        while (true)
        {
            source.PlayOneShot(music);
            yield return new WaitForSeconds(music.length - 0.8f); // fucking garage band!
        }
    }
}