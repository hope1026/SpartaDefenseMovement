using System.Collections.Generic;
using UnityEngine;

public class MonsterStateModule
{
    private MonsterStateAbstract _currentState;
    private readonly Dictionary<MonsterStateType, MonsterStateAbstract> _statesByType = new Dictionary<MonsterStateType, MonsterStateAbstract>();

    public MonsterStateModule(Monster ownerMonster_)
    {
        _statesByType.Add(MonsterStateType.FORWARD, new MonsterStateForward(this, ownerMonster_));
        _statesByType.Add(MonsterStateType.BACKWARD, new MonsterStateBackward(this, ownerMonster_));
        _statesByType.Add(MonsterStateType.JUMP, new MonsterStateJump(this, ownerMonster_));
        _statesByType.Add(MonsterStateType.ATTACK, new MonsterStateAttack(this, ownerMonster_));
    }

    public void Update()
    {
        _currentState.UpdateState();
    }

    public void FixedUpdate()
    {
        _currentState.FixedUpdateState();
    }

    public void ChangeState(MonsterStateType newStateType_, MonsterStateAbstract.ParamsAbstract params_ = null)
    {
        if (CanTransition(newStateType_) == false)
            return;

        MonsterStateType oldState = _currentState != null ? _currentState.GetStateType() : MonsterStateType.FORWARD;
        if (_currentState != null)
        {
            _currentState.ExitState();
        }

        if (_statesByType.TryGetValue(newStateType_, out MonsterStateAbstract newState))
        {
            newState.EnterState(params_);
            _currentState = newState;
        }
        else
        {
            Debug.LogWarning($"MonsterStateModule::ChangeState() Failed change state:{newStateType_}");
        }
    }

    private bool CanTransition(MonsterStateType newStateType_)
    {
        if (_currentState == null || _currentState.canTransitionToOtherState)
            return true;

        if (newStateType_ == MonsterStateType.ATTACK || newStateType_ == MonsterStateType.BACKWARD)
            return true;

        return false;
    }

    public MonsterStateType GetCurrentStateType()
    {
        return _currentState.GetStateType();
    }
}