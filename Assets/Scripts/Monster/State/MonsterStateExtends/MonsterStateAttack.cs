using System.Collections;
using UnityEngine;

public class MonsterStateAttack : MonsterStateAbstract
{
    private Coroutine _attackCoroutine = null;

    public MonsterStateAttack(MonsterStateModule ownerStateModule_, Monster ownerMonster_)
        : base(ownerStateModule_, ownerMonster_) { }

    public override void EnterState()
    {
        canTransitionToOtherState = false;
        _ownerMonster.MovementModule.Stop();
        _ownerMonster.StartCoroutine(AttackCoroutine());
    }

    public override void ExitState()
    {
        if (_attackCoroutine != null)
        {
            _ownerMonster.StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }
    }

    public override MonsterStateType GetStateType()
    {
        return MonsterStateType.ATTACK;
    }

    private void PlayAttack()
    {
        _attackCoroutine = _ownerMonster.StartCoroutine(AttackCoroutine());
    }

    // 공격 완료를 후 상태 변경
    private IEnumerator AttackCoroutine()
    {
        // 공격이 완료될 때까지 대기(에니메이션이 없어 시간으로 처리)
        yield return new WaitForSeconds(0.3f);

        canTransitionToOtherState = true;
        Monster belowMonster = _ownerMonster.FindBelowMonster();
        if (belowMonster != null)
        {
            // 아래 몬스터의 상태를 BACKWARD 변경
            belowMonster.StateModule.ChangeState(MonsterStateType.BACKWARD);
        }

        if (_ownerMonster.CanAttack())
        {
            canTransitionToOtherState = false;
            PlayAttack();
        }
        else
        {
            _ownerMonster.StateModule.ChangeState(MonsterStateType.FORWARD);
        }
    }
}