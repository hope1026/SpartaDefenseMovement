using System.Collections;
using UnityEngine;

public class MonsterStateBackward : MonsterStateAbstract
{
    public class Params : ParamsAbstract
    {
        public float targetPosX;
    }

    private Coroutine _backwardCoroutine = null;

    public MonsterStateBackward(MonsterStateModule ownerStateModule_, Monster ownerMonster_)
        : base(ownerStateModule_, ownerMonster_) { }

    public override void EnterState(ParamsAbstract params_)
    {
        canTransitionToOtherState = false;
        _backwardCoroutine = _ownerMonster.StartCoroutine(WaitingForBackwardCoroutine());
        if (params_ is Params backwardParams)
        {
            _ownerMonster.MovementModule.StartMoveBackward(backwardParams.targetPosX);
            Monster behindMonster = _ownerMonster.FindBehindMonster();
            if (behindMonster != null &&
                (behindMonster.StateModule.GetCurrentStateType() == MonsterStateType.FORWARD ||
                 behindMonster.StateModule.GetCurrentStateType() == MonsterStateType.BACKWARD))
            {
                Params nextMonsterBackwardParams = new Params();
                nextMonsterBackwardParams.targetPosX = backwardParams.targetPosX + (_ownerMonster.GetCollisionRadius() * 1.5f);
                behindMonster.StateModule.ChangeState(MonsterStateType.BACKWARD, nextMonsterBackwardParams);
            }
        }
    }

    public override void ExitState()
    {
        if (_backwardCoroutine != null)
        {
            _ownerMonster.StopCoroutine(_backwardCoroutine);
            _backwardCoroutine = null;
        }
    }

    private IEnumerator WaitingForBackwardCoroutine()
    {
        // 이동이 완료될 때까지 대기(에니메이션이 없어 시간으로 처리)
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            Monster behindMonster = _ownerMonster.FindBehindOrAboveMonster();
            if (behindMonster == null)
            {
                canTransitionToOtherState = true;
                _ownerStateModule.ChangeState(MonsterStateType.FORWARD);
            }
        }
    }

    public override MonsterStateType GetStateType()
    {
        return MonsterStateType.BACKWARD;
    }
}