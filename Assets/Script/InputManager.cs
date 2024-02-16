using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public PlayerInput inputAsset;
    public InputAction horizontalMovement;
    public InputAction aim;
    public InputAction down;
    public InputAction jump;
    public InputAction rightClick;
    public InputAction leftClick;
    public InputAction interact;

    private void Awake()
    {
        aim = inputAsset.actions.FindAction("Look");
    }

    private void OnEnable()
    {
        horizontalMovement.Enable();
        jump.Enable();
        rightClick.Enable();
        leftClick.Enable();
        down.Enable();
        interact.Enable();
        aim.Enable();
    }

    private void OnDisable()
    {
        horizontalMovement.Disable();
        jump.Disable();
        rightClick.Disable();
        leftClick.Disable();
        down.Disable();
        interact.Disable();
        aim.Disable();
    }

    void Start()
    {
       
    }

    void Update()
    {

    }
}
