using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioClip boltSound;
    public AudioSource source;

    private static SFXManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static void PlayBoltSound()
    {
        instance.source.PlayOneShot(instance.boltSound);
    }
}