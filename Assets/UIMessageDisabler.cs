using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMessageDisabler : MonoBehaviour
{
    bool triggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered)
            return;

        if (collision)
        {
            if (collision.GetComponent<PlayerStateManager>())
            {
                triggered = true;
                UIManger.instance.DisableUIMessage();
            }
        }
    }
}
