public class MonsterStateForward : MonsterStateAbstract
{
    public MonsterStateForward(MonsterStateModule ownerStateModule_, Monster ownerMonster_)
        : base(ownerStateModule_, ownerMonster_) { }

    public override void EnterState(ParamsAbstract params_)
    {
        canTransitionToOtherState = true;
        _ownerMonster.MovementModule.StartMoveForward();
    }

    public override void UpdateState()
    {
        // 공격이 가능하면 공격 상태로 전환
        if (_ownerMonster.CanAttack())
        {
            _ownerStateModule.ChangeState(MonsterStateType.ATTACK);
        }
        else
        {
            if (_ownerMonster.MovementModule.IsGrounded == false)
            {
                Monster belowMonster = _ownerMonster.FindBelowMonster();
                if (!belowMonster || !belowMonster.FindForwardMonsterWithoutJumpOrFall() || !belowMonster.FindBehindMonster())
                {
                    float targetPosY = _ownerMonster.transform.position.y - (_ownerMonster.GetCollisionRadius() * 2f);
                    _ownerMonster.MovementModule.Fall(targetPosY);
                }
            }
            Monster forwardMonster = _ownerMonster.FindForwardMonsterWithoutJumpOrFall();
            if (forwardMonster)
            {
                _ownerStateModule.ChangeState(MonsterStateType.JUMP);
            }
        }
    }

    public override MonsterStateType GetStateType()
    {
        return MonsterStateType.FORWARD;
    }
}