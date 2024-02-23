using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSendToMaster : MonoBehaviour
{
    AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = source.volume * AudioManager.instance.masterVolume;
    }

}
