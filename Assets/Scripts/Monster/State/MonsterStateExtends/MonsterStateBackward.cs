using System.Collections;
using UnityEngine;

public class MonsterStateBackward : MonsterStateAbstract
{
    private Coroutine _backwardCoroutine = null;
    public MonsterStateBackward(MonsterStateModule ownerStateModule_, Monster ownerMonster_)
        : base(ownerStateModule_, ownerMonster_)
    {
    }

    public override void EnterState()
    {
        canTransitionToOtherState = false;
        PlayBackward();
        _ownerMonster.MovementModule.StartMoveBackward();
    }

    public override void ExitState()
    {
        if (_backwardCoroutine != null)
        {
            _ownerMonster.StopCoroutine(_backwardCoroutine);
            _backwardCoroutine = null;
        }
    }

    public override MonsterStateType GetStateType()
    {
        return MonsterStateType.BACKWARD;
    }

    private void PlayBackward()
    {
        _backwardCoroutine = _ownerMonster.StartCoroutine(BackwardCoroutine());
    }
    
    private IEnumerator BackwardCoroutine()
    {
        // 이동이 완료될 때까지 대기(에니메이션이 없어 시간으로 처리)
        yield return new WaitForSeconds(0.3f);
        
        //뒤에 몬스터가 있으면 뒤로 계속이동, 없으면 FORWARD 로 변경하여 점프처리 
        Monster behindMonster = _ownerMonster.FindBehindMonster();
        if (behindMonster != null)
        {
            canTransitionToOtherState = false;
            PlayBackward();
        }
        else
        {
            canTransitionToOtherState = true;
            _ownerStateModule.ChangeState(MonsterStateType.FORWARD);
        }
    }
}