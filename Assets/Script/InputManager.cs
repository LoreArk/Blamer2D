using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public PlayerInput inputAsset;
    public InputAction horizontalMovement;
    public InputAction aim;
    public InputAction down;
    public InputAction jump;
    public InputAction rightClick;
    public InputAction leftClick;
    public InputAction interact;
    public InputAction run;
    public InputAction exit;

    private void Awake()
    {
        instance = this;

        aim = inputAsset.actions.FindAction("Look");
        exit = inputAsset.actions.FindAction("Exit");
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
        run.Enable();
        exit.Enable();
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
        run.Disable();
        exit.Disable();
    }

    public void DisableGameInput()
    {
        horizontalMovement.Disable();
        jump.Disable();
        rightClick.Disable();
        leftClick.Disable();
        down.Disable();
        interact.Disable();
        aim.Disable();
        run.Disable();
    }

    public void EnableGameInput()
    {
        horizontalMovement.Enable();
        jump.Enable();
        rightClick.Enable();
        leftClick.Enable();
        down.Enable();
        interact.Enable();
        aim.Enable();
        run.Enable();
    }

    void Start()
    {
       
    }

    void Update()
    {

    }
}
