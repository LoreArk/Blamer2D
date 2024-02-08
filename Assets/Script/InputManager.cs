using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public InputAction horizontalMovement;
    public InputAction down;
    public InputAction jump;
    public InputAction rightClick;
    public InputAction leftClick;

    private void OnEnable()
    {
        horizontalMovement.Enable();
        jump.Enable();
        rightClick.Enable();
        leftClick.Enable();
        down.Enable();
    }

    private void OnDisable()
    {
        horizontalMovement.Disable();
        jump.Disable();
        rightClick.Disable();
        leftClick.Disable();
        down.Disable();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
