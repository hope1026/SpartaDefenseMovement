using UnityEngine;

public class MonsterStateJump : MonsterStateAbstract
{
    public MonsterStateJump(MonsterStateModule ownerStateModule_, Monster ownerMonster_)
        : base(ownerStateModule_, ownerMonster_) { }

    public override void EnterState()
    {
        Monster forwardMonster = _ownerMonster.FindForwardMonster();
        if (forwardMonster)
        {
            float jumpTargetY = forwardMonster.transform.position.y + (forwardMonster.GetCollisionRadius() * 2f);
            _ownerMonster.MovementModule.JumpTo(jumpTargetY);
        }
        canTransitionToOtherState = false;
    }

    public override void UpdateState()
    {
        if (!_ownerMonster.MovementModule.IsJumping)
        {
            canTransitionToOtherState = true;
            _ownerStateModule.ChangeState(MonsterStateType.FORWARD);
        }
    }

    public override MonsterStateType GetStateType()
    {
        return MonsterStateType.JUMP;
    }
}