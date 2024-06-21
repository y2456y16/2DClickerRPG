using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum ECharacterState
    {
        None,
        Idle,
        Walk,
        Skill,
        Dead
    }
    public enum EObjectType
    {
        None,
        Character,
        Projectile,
        Env,
    }
    public enum ECharacterType
    {
        None,
        Player,
        Companion,
        Monster,
        Npc,
    }
    public enum EScene
    {
        Unknown,
        TitleScene,
        GameScene,
    }

    public enum EUIEvent
    {
        Click,
        PointerDown,
        PointerUp,
        Drag,
    }

    public enum ESound
    {
        Bgm,
        Effect,
        Max,
    }

    public enum EJoystickState
    {
        PointerDown,
        PointerUp,
        Drag,
    }

}

public static class AnimName
{
    public const string IDLE = "idle";
    public const string ATTACK = "attack";
    public const string ATTACK_B = "attack_b";
    public const string WALK = "walk";
    public const string DEAD = "dead";
}
