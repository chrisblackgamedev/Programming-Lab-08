using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager 
{
    private static Controls _controls;

    public static void Init(Player myPlayer)
    {
        _controls = new Controls();

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Confirmed;

        _controls.Game.Movement.performed += hi =>
        {
            myPlayer.SetMovementDirection(hi.ReadValue<Vector3>());
        };

        _controls.Game.Jump.started += hi => 
        {
            //Debug.Log("Is this working?");
        };

        _controls.Game.Look.performed += ctx =>
        {
            myPlayer.SetLookDirection(ctx.ReadValue<Vector2>());
        };

        _controls.Game.Shoot.performed += ctx =>
        {
            myPlayer.Shoot();
        };

        _controls.Game.Reload.performed += ctx =>
        {
            myPlayer.Reload();
        };
        _controls.Permanent.Enable();
    
    }

    public static void GameMode()
    {
        _controls.Game.Enable();
        _controls.UI.Disable();
    }

    public static void UIMode()
    {
        _controls.Game.Disable();
        _controls.UI.Enable();
    }

    
}

