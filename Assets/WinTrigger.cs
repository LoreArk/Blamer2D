using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WinTrigger : MonoBehaviour
{
    public UnityEvent onWin;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision)
        {
            PlayerStateManager player = collision.gameObject.GetComponent<PlayerStateManager>();
            if (player != null)
                onWin.Invoke();
        }
    }
}
