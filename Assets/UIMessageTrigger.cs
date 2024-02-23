using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMessageTrigger : MonoBehaviour
{
    [SerializeField] private string text;
    [SerializeField] private float time;
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
                UIManger.instance.SendUIMessage(text, time);
            }
        }
    }
}
