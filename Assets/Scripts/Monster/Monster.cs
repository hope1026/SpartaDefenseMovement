using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _collisionRadius = 0.5f;
    [SerializeField] private float _attackRange = 0.1f;
    [SerializeField] private float _collisionEpsilon = 0.1f;
    private static RaycastHit2D[] TEMP_RAYCAST_HIT_2DS = new RaycastHit2D[20];

    public MonsterMovementModule MovementModule { get; private set; }
    public MonsterStateModule StateModule { get; private set; }

    private void Awake()
    {
        MovementModule = new MonsterMovementModule(this);
        StateModule = new MonsterStateModule(this);
    }

    private void Start()
    {
        MovementModule.CalculateGroundPos();
        StateModule.ChangeState(MonsterStateType.FORWARD);
        CircleCollider2D circleCollider2D = base.GetComponent<CircleCollider2D>();
        if (circleCollider2D != null)
        {
            _collisionRadius = circleCollider2D.radius;
        }
    }

    private void Update()
    {
        MovementModule.Update();
        StateModule.Update();
    }

    public bool CanAttack()
    {
        Vector2 attackDirection = Vector2.left;
        Vector2 origin = transform.position;
        float rayDistance = _collisionRadius + _attackRange;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, attackDirection, rayDistance, UnityLayerDefines.LayerMask.TOWER);
        return raycastHit2D.collider != null;
    }

    // 아래 몬스터가 있는지 판단
    public Monster FindBelowMonster()
    {
        Vector2 belowDirection = Vector2.down;
        Vector2 origin = transform.position;
        float rayDistance = _collisionRadius + _collisionEpsilon;
        int resultCount = Physics2D.RaycastNonAlloc(origin, belowDirection, TEMP_RAYCAST_HIT_2DS, rayDistance, UnityLayerDefines.LayerMask.MONSTER);
        if (0 < resultCount)
        {
            for (int index = 0; index < resultCount; index++)
            {
                Monster hitMonster = TEMP_RAYCAST_HIT_2DS[index].collider.GetComponent<Monster>();
                if (hitMonster != this)
                    return hitMonster;
            }
        }

        return null;
    }

    // 앞에 몬스터가 있는지 판단
    public Monster FindForwardMonster()
    {
        Vector2 forwardDirection = Vector2.left;
        Vector2 origin = transform.position;
        float rayDistance = _collisionRadius + _collisionEpsilon;
        int resultCount = Physics2D.RaycastNonAlloc(origin, forwardDirection, TEMP_RAYCAST_HIT_2DS, rayDistance, UnityLayerDefines.LayerMask.MONSTER);
        if (0 < resultCount)
        {
            for (int index = 0; index < resultCount; index++)
            {
                Monster hitMonster = TEMP_RAYCAST_HIT_2DS[index].collider.GetComponent<Monster>();
                if (hitMonster != this)
                    return hitMonster;
            }
        }
        
        return null;
    }

    // 뒤에 몬스터가 있는지 판단
    public Monster FindBehindMonster()
    {
        Vector2 behindDirection = Vector2.right;
        Vector2 origin = transform.position;
        float rayDistance = _collisionRadius + _collisionEpsilon;
        int resultCount = Physics2D.RaycastNonAlloc(origin, behindDirection, TEMP_RAYCAST_HIT_2DS, rayDistance, UnityLayerDefines.LayerMask.MONSTER);
        if (0 < resultCount)
        {
            for (int index = 0; index < resultCount; index++)
            {
                Monster hitMonster = TEMP_RAYCAST_HIT_2DS[index].collider.GetComponent<Monster>();
                if (hitMonster != this)
                    return hitMonster;
            }
        }
        
        return null;
    }

    public float GetCollisionRadius()
    {
        return _collisionRadius;
    }
}