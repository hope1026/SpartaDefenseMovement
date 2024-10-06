using System.Collections.Generic;
using UnityEngine;

public class MonsterStateModule
{
    private MonsterStateAbstract _currentState;
    private readonly Dictionary<MonsterStateType, MonsterStateAbstract> _statesByType = new Dictionary<MonsterStateType, MonsterStateAbstract>();
    private readonly Monster _ownerMonster;

    public MonsterStateModule(Monster ownerMonster_)
    {
        _ownerMonster = ownerMonster_;
        _statesByType.Add(MonsterStateType.FORWARD, new MonsterStateForward(this, _ownerMonster));
        _statesByType.Add(MonsterStateType.BACKWARD, new MonsterStateBackward(this, _ownerMonster));
        _statesByType.Add(MonsterStateType.JUMP, new MonsterStateJump(this, _ownerMonster));
        _statesByType.Add(MonsterStateType.ATTACK, new MonsterStateAttack(this, _ownerMonster));
    }

    public void Update()
    {
        _currentState.UpdateState();
    }

    public void ChangeState(MonsterStateType newStateType_)
    {
        if (CanTransition(newStateType_) == false)
            return;

        MonsterStateType oldState = MonsterStateType.FORWARD;
        if (_currentState != null)
        {
            oldState = _currentState.GetStateType();
            _currentState.ExitState();
        }

        if (_statesByType.TryGetValue(newStateType_, out MonsterStateAbstract newState))
        {
            newState.EnterState();
            _currentState = newState;
            Debug.Log($"Change State {oldState} -> {newStateType_}");
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

    // 현재 상태의 타입을 반환하는 메소드
    public MonsterStateType GetCurrentStateType()
    {
        return _currentState.GetStateType();
    }
}