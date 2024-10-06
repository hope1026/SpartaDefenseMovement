using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private float _attackRange = 0.1f;
    [SerializeField] private float _collisionEpsilon = 0.1f;
    private float _collisionRadius = 0.5f;
    private static readonly RaycastHit2D[] TEMP_RAY_CAST_HIT_2DS = new RaycastHit2D[20];

    public MonsterMovementModule MovementModule { get; private set; }
    public MonsterStateModule StateModule { get; private set; }

    private void Awake()
    {
        MovementModule = new MonsterMovementModule(this);
        StateModule = new MonsterStateModule(this);
    }

    private void Start()
    {
        MovementModule.FindAndSetGroundAndTower();
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

    private void FixedUpdate()
    {
        StateModule.FixedUpdate();
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
    public Monster FindBelowMonster(bool onlyGrounded_ = false)
    {
        Vector2 belowDirection = Vector2.down;
        Vector2 origin = transform.position;
        float rayDistance = _collisionRadius + _collisionEpsilon;
        int resultCount = Physics2D.CircleCastNonAlloc(origin, _collisionRadius, belowDirection, TEMP_RAY_CAST_HIT_2DS, rayDistance, UnityLayerDefines.LayerMask.MONSTER);
        if (0 < resultCount)
        {
            for (int index = 0; index < resultCount; index++)
            {
                Monster hitMonster = TEMP_RAY_CAST_HIT_2DS[index].collider.GetComponent<Monster>();
                if (hitMonster != this)
                {
                    if (onlyGrounded_ && hitMonster.MovementModule.IsGrounded == false)
                        continue;

                    return hitMonster;
                }
            }
        }

        return null;
    }

    // 앞에 몬스터가 있는지 판단
    public Monster FindForwardMonsterWithoutJumpOrFall()
    {
        Vector2 forwardDirection = Vector2.left;
        Vector2 origin = transform.position;
        float rayDistance = _collisionRadius + _collisionEpsilon;
        int resultCount = Physics2D.RaycastNonAlloc(origin, forwardDirection, TEMP_RAY_CAST_HIT_2DS, rayDistance, UnityLayerDefines.LayerMask.MONSTER);
        if (0 < resultCount)
        {
            for (int index = 0; index < resultCount; index++)
            {
                Monster hitMonster = TEMP_RAY_CAST_HIT_2DS[index].collider.GetComponent<Monster>();
                if (hitMonster != this && base.transform.position.x > hitMonster.transform.position.x &&
                    hitMonster.MovementModule.MovementStateType != MonsterMovementStateType.JUMP)
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
        int resultCount = Physics2D.RaycastNonAlloc(origin, behindDirection, TEMP_RAY_CAST_HIT_2DS, rayDistance, UnityLayerDefines.LayerMask.MONSTER);
        if (0 < resultCount)
        {
            for (int index = 0; index < resultCount; index++)
            {
                Monster hitMonster = TEMP_RAY_CAST_HIT_2DS[index].collider.GetComponent<Monster>();
                if (hitMonster != this && base.transform.position.x < hitMonster.transform.position.x)
                    return hitMonster;
            }
        }

        return null;
    }

    // 뒤에 or 위에 몬스터가 있는지 판단
    public Monster FindBehindOrAboveMonster()
    {
        Vector2 direction = Vector2.right;
        Vector2 origin = transform.position;
        float rayDistance = _collisionRadius + _collisionEpsilon;
        int resultCount = Physics2D.RaycastNonAlloc(origin, direction, TEMP_RAY_CAST_HIT_2DS, rayDistance, UnityLayerDefines.LayerMask.MONSTER);
        if (0 < resultCount)
        {
            for (int index = 0; index < resultCount; index++)
            {
                Monster hitMonster = TEMP_RAY_CAST_HIT_2DS[index].collider.GetComponent<Monster>();
                if (hitMonster != this && base.transform.position.x < hitMonster.transform.position.x)
                    return hitMonster;
            }
        }

        direction = Vector2.up;
        rayDistance = _collisionRadius + _collisionRadius;
        resultCount = Physics2D.RaycastNonAlloc(origin, direction, TEMP_RAY_CAST_HIT_2DS, rayDistance, UnityLayerDefines.LayerMask.MONSTER);
        if (0 < resultCount)
        {
            for (int index = 0; index < resultCount; index++)
            {
                Monster hitMonster = TEMP_RAY_CAST_HIT_2DS[index].collider.GetComponent<Monster>();
                if (hitMonster != this && base.transform.position.y < hitMonster.transform.position.y)
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