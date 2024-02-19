using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    LineRenderer lineRenderer;
    PlayerStateManager player;
    [SerializeField] private LayerMask layerMask;

    private void Start()
    {
        player = PlayerStateManager.Instance;
        player.laser = this;
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (player.aim)
        {
            lineRenderer.enabled = true;
        }
        else
            lineRenderer.enabled = false;


        Vector2 lineEnd = Raycast();

        lineRenderer.SetPosition(1, new Vector3(lineEnd.x, lineEnd.y, 0));
        lineRenderer.SetPosition(0, new Vector3(player.laserStart.position.x, player.laserStart.position.y, 0));
    }

    private Vector2 Raycast()
    {
        
        Vector2 direction = player.aimTarget.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 100, layerMask);
        if (hit)
        {
            //Debug.Log(hit.distance);
            return player.aimTarget.position;
        }
        else
        {
           // return new Vector2(0, 0);
            return player.aimTarget.position;
        } 
    }
}
