public class MonsterStateForward : MonsterStateAbstract
{
    public MonsterStateForward(MonsterStateModule ownerStateModule_, Monster ownerMonster_)
        : base(ownerStateModule_, ownerMonster_) { }

    public override void EnterState()
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
            Monster forwardMonster = _ownerMonster.FindForwardMonster();
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