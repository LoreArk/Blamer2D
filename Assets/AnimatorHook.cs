using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHook : MonoBehaviour
{
    AudioManager audioManager;

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
        audioManager.PlayerJump(PlayerStateManager.Instance.feetAudioSource);
    }

    public void PlayerLanded()
    {

        audioManager.PlayerLanded(PlayerStateManager.Instance.landingAudioSource);
    }

    public void PlayerMeleeAttack()
    {
        audioManager.PlayerMeleeSlash(PlayerStateManager.Instance.gunAudioSource);
    }
}
