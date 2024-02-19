using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorHook : MonoBehaviour
{
    AudioManager audioManager;
    [SerializeField] public UnityEvent onLanding;
    [SerializeField] public UnityEvent onJump;

    private void Start()
    {
        audioManager = AudioManager.instance;
    }


    public void EnableAttackCollider()
    {
        PlayerStateManager.Instance.ToggleAttackCollider(true);
    }

    public void DisableAttackCollider()
    {
        PlayerStateManager.Instance.ToggleAttackCollider(false);

    }

    public void PlayerFootstep()
    {
        //Debug.Log("PlayFootstep");
        audioManager.PlayerFootsteps(PlayerStateManager.Instance.feetAudioSource);
    }

    public void PlayerJump()
    {
        onJump.Invoke();
        audioManager.PlayerJump(PlayerStateManager.Instance.feetAudioSource);
    }

    public void PlayerLanded()
    {
        onLanding.Invoke();
        audioManager.PlayerLanded(PlayerStateManager.Instance.landingAudioSource);
    }

    public void PlayerMeleeAttack()
    {
        audioManager.PlayerMeleeSlash(PlayerStateManager.Instance.gunAudioSource);
    }
}
