using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using static Spine.Unity.Examples.BasicPlatformerController;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Monster : Character
{
    public override ECharacterState CharacterState
    {
        get { return base.CharacterState; }
        set
        {
            if (_characterState != value)
            {
                base.CharacterState = value;
                switch (value)
                {
                    case ECharacterState.Idle:
                        UpdateAITick = 0.5f;
                        break;
                    case ECharacterState.Walk:
                        UpdateAITick = 0.0f;
                        break;
                    case ECharacterState.Skill:
                        UpdateAITick = 0.0f;
                        break;
                    case ECharacterState.Dead:
                        UpdateAITick = 1.0f;
                        break;
                }
            }
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CharacterType = ECharacterType.Monster;
        CharacterState = ECharacterState.Idle;
        Speed = 3.0f;

        StartCoroutine(CoUpdateAI());
        return true;
    }

    void Start()
    {
        _initPos = transform.position;
    }

    #region AI
    public float SearchDistance { get; private set; } = 8.0f;
    public float AttackDistance { get; private set; } = 4.0f;
    Character _target;
    Vector3 _destPos;
    Vector3 _initPos;

    protected override void UpdateIdle()
    {
        Debug.Log("Idle");

        // Patrol
        {
            int patrolPercent = 10;
            int rand = Random.Range(0, 100);
            if (rand <= patrolPercent)
            {
                _destPos = _initPos + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
                CharacterState = ECharacterState.Walk;
                return;
            }
        }

        // Search Player
        {
            Character target = null;
            float bestDistanceSqr = float.MaxValue;
            float searchDistanceSqr = SearchDistance * SearchDistance;

            foreach (Player player in Managers.ObjManage.Players)
            {
                Vector3 dir = player.transform.position - transform.position;
                float distToTargetSqr = dir.sqrMagnitude;

                Debug.Log(distToTargetSqr);

                if (distToTargetSqr > searchDistanceSqr)
                    continue;

                if (distToTargetSqr > bestDistanceSqr)
                    continue;

                target = player ;
                bestDistanceSqr = distToTargetSqr;
            }

            _target = target;

            if (_target != null)
                CharacterState = ECharacterState.Walk;
        }
    }

    protected override void UpdateMove()
    {
        Debug.Log("Move");

        if (_target == null)
        {
            // Patrol or Return
            Vector3 dir = (_destPos - transform.position);
            float moveDist = Mathf.Min(dir.magnitude, Time.deltaTime * Speed);
            transform.TranslateEx(dir.normalized * moveDist);

            if (dir.sqrMagnitude <= 0.01f)
            {
                CharacterState = ECharacterState.Idle;
            }
        }
        else
        {
            // Chase
            Vector3 dir = (_target.transform.position - transform.position);
            float distToTargetSqr = dir.sqrMagnitude;
            float attackDistanceSqr = AttackDistance * AttackDistance;

            if (distToTargetSqr < attackDistanceSqr)
            {
                // 공격 범위 이내로 들어왔으면 공격.
                CharacterState = ECharacterState.Skill;
                StartWait(2.0f);
            }
            else
            {
                // 공격 범위 밖이라면 추적.
                float moveDist = Mathf.Min(dir.magnitude, Time.deltaTime * Speed);
                transform.TranslateEx(dir.normalized * moveDist);

                // 너무 멀어지면 포기.
                float searchDistanceSqr = SearchDistance * SearchDistance;
                if (distToTargetSqr > searchDistanceSqr)
                {
                    _destPos = _initPos;
                    _target = null;
                    CharacterState = ECharacterState.Walk;
                }
            }
        }
    }

    protected override void UpdateSkill()
    {
        Debug.Log("Skill");

        if (_coWait != null)
            return;

        CharacterState = ECharacterState.Walk;
    }

    protected override void UpdateDead()
    {
        Debug.Log("Dead");

    }
    #endregion
}
