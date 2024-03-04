using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputEnabler : MonoBehaviour
{
    [SerializeField] private bool enableInput;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision)
        {
            PlayerStateManager player = GetComponent<PlayerStateManager>();

            if (player)
            {
                if (enableInput)
                    InputManager.instance.EnableGameInput();
                else
                    InputManager.instance.DisableGameInput();
            }
        }
    }
}
