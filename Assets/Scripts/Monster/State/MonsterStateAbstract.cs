public abstract class MonsterStateAbstract
{
    public abstract class ParamsAbstract { }
    protected MonsterStateModule _ownerStateModule;
    protected Monster _ownerMonster;
    public bool canTransitionToOtherState { get; protected set; } = false;

    protected MonsterStateAbstract(MonsterStateModule ownerStateModule_, Monster ownerMonster_)
    {
        _ownerStateModule = ownerStateModule_;
        _ownerMonster = ownerMonster_;
    }

    public virtual void EnterState(ParamsAbstract params_) { }

    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
    public virtual void ExitState() { }

    // 각 상태의 타입을 반환하는 메소드 추가
    public abstract MonsterStateType GetStateType();
}