using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    #region Player
    private Vector2 _moveDir;
    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set
        {
            _moveDir = value;
            OnMoveDirChanged?.Invoke(value);
        }
    }

    private Define.EJoystickState _joyStickState;
    public Define.EJoystickState JoyStickState
    {
        get { return _joyStickState; }
        set
        {
            _joyStickState = value;
            OnJoyStickStateChanged?.Invoke(_joyStickState);
        }
    }
    #endregion

    #region Action
    public event Action<Vector2> OnMoveDirChanged;
    public event Action<Define.EJoystickState> OnJoyStickStateChanged;
    #endregion
}
