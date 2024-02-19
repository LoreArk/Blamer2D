using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationOnce : MonoBehaviour
{
    Animator animator;
    [SerializeField] private string animationName;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation()
    {
        animator.Play(animationName);
    }
}
