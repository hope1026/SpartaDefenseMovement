using UnityEngine;

public class MonsterMovementModule
{
    //바닥 위치 임시로 지정
    private const float GROUND_POS_Y = -6.5f;
    private const float GRAVITY = -9.81f;

    private Monster _ownerMonster;
    private float _groundPosY;
    private float _moveSpeed = 1f;
    private float _moveSpeedRate = 1f;
    private float _normalizedMoveDirection;
    private float _gravityVelocityY = 0f; // Gravity 적용
    private float _jumpTargetPosY = 0f;
    private bool _isGravityEnabled = false;
    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider2D;
    private bool _isGrounded = true;
    public bool IsJumping { get; private set; } = false;
    public MonsterMovementStateType MovementStateType { get; private set; } = MonsterMovementStateType.IDLE;

    public MonsterMovementModule(Monster monster_)
    {
        _ownerMonster = monster_;
        _rigidbody2D = _ownerMonster.GetComponent<Rigidbody2D>();
        _collider2D = _ownerMonster.GetComponent<Collider2D>();
    }

    public void Update()
    {
        CheckGroundOrJump();
        // UpdateGravityIfEnabled();
        // UpdatePosition();
    }

    public void StartMoveForward()
    {
        MovementStateType = MonsterMovementStateType.FORWARD;
        _collider2D.isTrigger = false;
        _rigidbody2D.velocity = new Vector2(-_moveSpeed, _rigidbody2D.velocity.y);
        // _moveSpeedRate = 1f;
        // _normalizedMoveDirection = -1f; //left 샘플을 위해 하드코딩
    }

    public void StartMoveBackward()
    {
        MovementStateType = MonsterMovementStateType.BACKWARD;
        _collider2D.isTrigger = false;
        _rigidbody2D.velocity = new Vector2(_moveSpeed, _rigidbody2D.velocity.y);
        // _moveSpeedRate = 1f;
        // _normalizedMoveDirection = 1f; //right 샘플을 위해 하드코딩
    }

    public void JumpTo(float jumpTargetPosY_)
    {
        MovementStateType = MonsterMovementStateType.JUMP;
        _rigidbody2D.constraints = RigidbodyConstraints2D.None;
        float height = jumpTargetPosY_ - _ownerMonster.transform.position.y;

        const float JUMP_FACTOR = 1f;
        float jumpVelocity = Mathf.Sqrt(2 * -Physics2D.gravity.y * height) * _rigidbody2D.mass;
        IsJumping = true;
        _rigidbody2D.velocity = new Vector2(-_moveSpeed * 2f, jumpVelocity * JUMP_FACTOR);
    }

    public void Stop()
    {
        _moveSpeedRate = 1f;
        _rigidbody2D.velocity = new Vector2(0f, _rigidbody2D.velocity.y);
        MovementStateType = MonsterMovementStateType.IDLE;
    }

    public void CalculateGroundPos()
    {
        Vector2 belowDirection = Vector2.down;
        Vector2 origin = _ownerMonster.transform.position;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, belowDirection, distance: float.MaxValue, UnityLayerDefines.LayerMask.GROUND);
        if (raycastHit2D.collider != null)
        {
            _groundPosY = raycastHit2D.point.y;
        }
    }

    public bool IsGrounded()
    {
        return (_ownerMonster.transform.position.y + _ownerMonster.GetCollisionRadius()) <= _groundPosY;
    }

    private void CheckGroundOrJump()
    {
        _isGrounded = (_ownerMonster.transform.position.y + _ownerMonster.GetCollisionRadius()) <= _groundPosY;

        if (IsJumping)
        {
            if (_isGrounded || _rigidbody2D.velocity.y <= 0f && _rigidbody2D.transform.position.y <= _jumpTargetPosY)
            {
                IsJumping = false;
                _collider2D.isTrigger = false;
            }
        }

        if (_isGrounded)
        {
            _collider2D.isTrigger = false;
        }
    }

    private void UpdatePosition()
    {
        float targetPosX = _ownerMonster.transform.position.x;
        float targetPosY = _ownerMonster.transform.position.y;

        if (MovementStateType == MonsterMovementStateType.FORWARD || MovementStateType == MonsterMovementStateType.BACKWARD)
        {
            float moveSpeed = _moveSpeed * _moveSpeedRate;
            targetPosX += (_normalizedMoveDirection * moveSpeed * Time.deltaTime);
        }

        if (MovementStateType == MonsterMovementStateType.JUMP)
        {
            targetPosY += (_gravityVelocityY * Time.deltaTime);
            Debug.Log($"TargetPos: {_ownerMonster.transform.position.y} -> {targetPosY} velocity:{_gravityVelocityY}");
            if (_gravityVelocityY <= 0f && targetPosY <= _jumpTargetPosY)
            {
                targetPosY = _jumpTargetPosY;
                MovementStateType = MonsterMovementStateType.IDLE;
            }
        }

        _ownerMonster.transform.position = new Vector3(targetPosX, targetPosY, _ownerMonster.transform.position.z);
    }

    private void ResetGravityVelocity(float height_)
    {
        if (0f < height_)
        {
            _gravityVelocityY = Mathf.Sqrt(2f * -GRAVITY * height_);
        }
        else
        {
            _gravityVelocityY = 0f;
        }
    }

    //중력 적용
    private void UpdateGravityIfEnabled()
    {
        if (_isGravityEnabled)
        {
            _gravityVelocityY += GRAVITY * Time.deltaTime;
            if (IsGrounded())
            {
                _gravityVelocityY = 0f;
                _isGravityEnabled = false;
            }
        }
    }
}