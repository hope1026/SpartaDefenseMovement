using System.Collections;
using UnityEngine;

public class MonsterStateAttack : MonsterStateAbstract
{
    private Coroutine _attackCoroutine = null;

    public MonsterStateAttack(MonsterStateModule ownerStateModule_, Monster ownerMonster_)
        : base(ownerStateModule_, ownerMonster_) { }

    public override void EnterState(ParamsAbstract params_)
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

    public override void FixedUpdateState()
    {
        Monster belowMonster = _ownerMonster.FindBelowMonster(onlyGrounded_:true);
        if (belowMonster != null && belowMonster.StateModule.GetCurrentStateType() != MonsterStateType.BACKWARD)
        {
            // 아래 몬스터의 상태를 BACKWARD 변경
            MonsterStateBackward.Params backwardParams = new MonsterStateBackward.Params();
            backwardParams.targetPosX = _ownerMonster.transform.position.x + (_ownerMonster.GetCollisionRadius() * 2f);
            belowMonster.StateModule.ChangeState(MonsterStateType.BACKWARD, backwardParams);
        }

        if (_ownerMonster.MovementModule.IsGrounded == false && _ownerMonster.MovementModule.MovementStateType != MonsterMovementStateType.FALL)
            _ownerMonster.MovementModule.Fall(_ownerMonster.MovementModule.GroundPosY);
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

        if (_ownerMonster.CanAttack())
        {
            canTransitionToOtherState = false;
            PlayAttack();
        }
        else
        {
            canTransitionToOtherState = true;
            _ownerMonster.StateModule.ChangeState(MonsterStateType.FORWARD);
        }
    }
}