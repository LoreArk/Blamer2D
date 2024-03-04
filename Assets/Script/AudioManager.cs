using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Settings")]
    [Range(0, 1)] public float masterVolume;

    [Header("Audioclips")]
    [SerializeField] private PlayerSounds playerSounds;
    [SerializeField] private Sound deathSound;
    [SerializeField] private EnemySounds enemySounds;

    [SerializeField] private FallingTilesSounds fallingTilesSounds;
    [SerializeField] private UISounds uiSounds;

    AudioSource ambienceSource;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ambienceSource = GetComponentInChildren<AudioSource>();
    }

    public void PauseAmbience()
    {
        ambienceSource.Pause();
    }

    public void ResumeAmbience()
    {
        ambienceSource.Play();
    }

    public void PlayDeathAudio()
    {
        ambienceSource.clip = deathSound.audioClip;
        ambienceSource.pitch = deathSound.maxPitch;
        ambienceSource.volume = deathSound.maxVolume * masterVolume;
        ambienceSource.loop = false;
        ambienceSource.Play();

    }

    public void PlayerFootsteps(AudioSource source)
    {
        Sound sound = playerSounds.footsteps[Random.Range(0, playerSounds.footsteps.Count)];

        source.pitch = Random.Range(sound.minPitch, sound.maxPitch);
        source.volume = Random.Range(sound.minVolume, sound.maxVolume) * masterVolume;
        source.clip = sound.audioClip;
        if(!source.isPlaying)
        source.Play();
    }

    public void PlayerJump(AudioSource source)
    {
        Sound sound = playerSounds.jump[Random.Range(0, playerSounds.footsteps.Count)];

        source.clip = sound.audioClip;
        source.pitch = sound.maxPitch;
        source.volume = sound.maxVolume * masterVolume;
        source.Play();

    }

    public void PlayerLanded(AudioSource source)
    {
        Sound sound = playerSounds.landing[Random.Range(0, playerSounds.footsteps.Count)];

        source.clip = sound.audioClip;
        source.pitch = sound.maxPitch;
        source.volume = sound.maxVolume * masterVolume;

        source.PlayOneShot(sound.audioClip);
        //source.Play();
    }

    public void PlayerDeathLand(AudioSource source)
    {
        Sound sound = playerSounds.deathLanding;

        source.clip = sound.audioClip;
        source.pitch = sound.maxPitch;
        source.volume = sound.maxVolume * masterVolume;

        source.PlayOneShot(sound.audioClip);
        //source.Play();
    }

    public void PlayerDamaged(AudioSource source)
    {
        Sound sound = playerSounds.damage;

        source.clip = sound.audioClip;
        source.pitch = sound.maxPitch;
        source.volume = sound.maxVolume * masterVolume;
        source.Play();
    }

    public void PlayerDeathDamage(AudioSource source)
    {
        Sound sound = playerSounds.deathDamage;

        source.clip = sound.audioClip;
        source.pitch = sound.maxPitch;
        source.volume = sound.maxVolume * masterVolume;
        source.Play();
    }

    public void PlayerChargingShot(AudioSource source)
    {
        Sound sound = playerSounds.chargingShot;

        source.clip = sound.audioClip;
        source.pitch = sound.maxPitch;
        source.volume = sound.maxVolume * masterVolume;
        source.loop = true;
        source.Play();
    }

    public void PlayGunshot(AudioSource source, int chargeLevel)
    {
        Sound sound = playerSounds.shoot[chargeLevel];

        source.clip = sound.audioClip;
        source.pitch = Random.Range(sound.minPitch, sound.maxPitch);
        source.volume = Random.Range(sound.minVolume, sound.maxVolume) * masterVolume;
        source.loop = false;
        source.Play();

    }

    public void PlayerMeleeSlash(AudioSource source)
    {
        Sound sound = playerSounds.meleeSlash;

        source.clip = sound.audioClip;
        source.pitch =  sound.maxPitch;
        source.volume =  sound.maxVolume * masterVolume;
        source.Play();
    }

    public void EnemyAlarm(AudioSource source)
    {
        Sound sound = enemySounds.alarm;

        source.clip = sound.audioClip;
        source.pitch = sound.maxPitch;
        source.volume = sound.maxVolume * masterVolume;

        source.Play();
    }

    public void EnemyDamage(AudioSource source)
    {
        Sound sound = enemySounds.damage;

        source.clip = sound.audioClip;
        source.pitch = sound.maxPitch;
        source.volume = sound.maxVolume * masterVolume;

        source.Play();
    }

    public void EnemyDeath(AudioSource source)
    {
        Sound sound = enemySounds.death;

        source.clip = sound.audioClip;
        source.pitch = sound.maxPitch;
        source.volume = sound.maxVolume * masterVolume;

        source.Play();
    }

    public void EnemyImpact(AudioSource source)
    {
        Sound sound = enemySounds.impact;

        source.clip = sound.audioClip;
        source.pitch = sound.maxPitch;
        source.volume = sound.maxVolume * masterVolume;

        source.Play();
    }

    public void FallingGroupLanded(AudioSource source)
    {
        Sound sound = fallingTilesSounds.groupLanded;

        source.clip = sound.audioClip;
        source.pitch = sound.maxPitch;
        source.volume = sound.maxVolume * masterVolume;

        source.Play();
    }

    public void PlayButtonSound(AudioSource source)
    {
        Sound sound = uiSounds.buttonPressed;

        source.clip = sound.audioClip;
        source.pitch = sound.maxPitch;
        source.volume = sound.maxVolume * masterVolume;

        source.Play();
    }

    public void PlayStartButtonSound(AudioSource source)
    {
        Sound sound = uiSounds.startGameButton;

        source.clip = sound.audioClip;
        source.pitch = sound.maxPitch;
        source.volume = sound.maxVolume * masterVolume;

        source.Play();
    }

    public void PlayButtonSelectionSound(AudioSource source)
    {
        Sound sound = uiSounds.buttonSelected;

        source.clip = sound.audioClip;
        source.pitch = sound.maxPitch;
        source.volume = sound.maxVolume * masterVolume;

        source.PlayOneShot(sound.audioClip);
    }

}

[System.Serializable]
class PlayerSounds
{
    public List<Sound> footsteps;
    public List<Sound> landing;
    public List<Sound> jump;
    public List<Sound> shoot;
    public Sound chargingShot;
    public Sound damage;
    public Sound deathLanding;
    public Sound deathDamage;
    public Sound death;
    public Sound meleeSlash;
    public Sound heal;
}

[System.Serializable]
class EnemySounds
{
    public Sound alarm;
    public Sound attack;
    public Sound death;
    public Sound damage;
    public Sound impact;
}

[System.Serializable]
class FallingTilesSounds
{
    public Sound tileActivated;
    public Sound tileLanded;
    public Sound groupActivated;
    public Sound groupLanded;
}

[System.Serializable]
class Sound
{
    [SerializeField] public AudioClip audioClip;
    [Range(0, 1)] [SerializeField] public float minPitch, maxPitch;
    [Range(0, 1)] [SerializeField] public float minVolume, maxVolume;
}

[System.Serializable]
class UISounds
{
   public Sound buttonSelected;
    public Sound buttonPressed;
    public Sound startGameButton;
}
