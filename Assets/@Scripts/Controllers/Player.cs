using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Player : Character
{
    Vector2 _moveDir = Vector2.zero;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CharacterType = ECharacterType.Player;
        CharacterState = ECharacterState.Idle;
        Speed = 5.0f;

        Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;
        Managers.Game.OnJoyStickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoyStickStateChanged += HandleOnJoystickStateChanged;

        return true;
    }

    void Update()
    {
        transform.TranslateEx(_moveDir * Time.deltaTime * Speed);
    }

    private void HandleOnMoveDirChanged(Vector2 dir)
    {
        _moveDir = dir;
        Debug.Log(dir);
    }

    private void HandleOnJoystickStateChanged(EJoystickState joystickState)
    {
        switch (joystickState)
        {
            case Define.EJoystickState.PointerDown:
                CharacterState = Define.ECharacterState.Walk;
                break;
            case Define.EJoystickState.Drag:
                break;
            case Define.EJoystickState.PointerUp:
                CharacterState = Define.ECharacterState.Idle;
                break;
            default:
                break;
        }
    }
}
