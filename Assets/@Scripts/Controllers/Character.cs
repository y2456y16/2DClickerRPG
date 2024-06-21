using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Character : BaseObject
{
    public float Speed { get; protected set; } = 1.0f;

    public ECharacterType CharacterType { get; protected set; } = ECharacterType.None;

    protected ECharacterState _characterState = ECharacterState.None;
    public virtual ECharacterState CharacterState
    {
        get { return _characterState; }
        set
        {
            if (_characterState != value)
            {
                _characterState = value;
                UpdateAnimation();
            }
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Character;
        CharacterState = ECharacterState.Idle;
        return true;
    }

    protected override void UpdateAnimation()
    {
        switch (CharacterState)
        {
            case ECharacterState.Idle:
                PlayAnimation(0, AnimName.IDLE, true);
                break;
            case ECharacterState.Skill:
                PlayAnimation(0, AnimName.ATTACK, true);
                break;
            case ECharacterState.Walk:
                PlayAnimation(0, AnimName.WALK, true);
                break;
            case ECharacterState.Dead:
                PlayAnimation(0, AnimName.DEAD, true);
                RigidBody.simulated = false;
                break;
            default:
                break;
        }
    }

    #region AI
    public float UpdateAITick { get; protected set; } = 0.0f;

    protected IEnumerator CoUpdateAI()
    {
        while (true)
        {
            switch (CharacterState)
            {
                case ECharacterState.Idle:
                    UpdateIdle();
                    break;
                case ECharacterState.Walk:
                    UpdateMove();
                    break;
                case ECharacterState.Skill:
                    UpdateSkill();
                    break;
                case ECharacterState.Dead:
                    UpdateDead();
                    break;
            }

            if (UpdateAITick > 0)
                yield return new WaitForSeconds(UpdateAITick);
            else
                yield return null;
        }
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateMove() { }
    protected virtual void UpdateSkill() { }
    protected virtual void UpdateDead() { }
    #endregion

    #region Wait
    protected Coroutine _coWait;

    protected void StartWait(float seconds)
    {
        CancelWait();
        _coWait = StartCoroutine(CoWait(seconds));
    }

    IEnumerator CoWait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _coWait = null;
    }

    protected void CancelWait()
    {
        if (_coWait != null)
            StopCoroutine(_coWait);
        _coWait = null;
    }
    #endregion
}
