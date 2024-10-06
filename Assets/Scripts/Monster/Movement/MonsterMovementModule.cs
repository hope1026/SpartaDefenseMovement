using UnityEngine;

public class MonsterMovementModule
{
    private const float GRAVITY = -9.81f;

    private readonly Monster _ownerMonster;
    private float _towerPosX;
    private readonly float _moveSpeed = 1f;
    private float _moveSpeedRate = 1f;
    private float _normalizedMoveDirection;
    private float _gravityVelocityY = 0f;
    private float _jumpTargetPosY = 0f;
    private bool _isGravityEnabled = false;
    private float _backwardTargetPosX;
    private float _fallTargetPosY = 0f;
    public bool IsGrounded { get; private set; } = true;
    public float GroundPosY { get; private set; }

    public MonsterMovementStateType MovementStateType { get; private set; } = MonsterMovementStateType.IDLE;

    public MonsterMovementModule(Monster monster_)
    {
        _ownerMonster = monster_;
    }

    public void Update()
    {
        UpdateGravityIfEnabled();
        UpdatePosition();
    }

    public void StartMoveForward()
    {
        MovementStateType = MonsterMovementStateType.FORWARD;
        _moveSpeedRate = 1f;
        _normalizedMoveDirection = -1f; //left 샘플을 위해 하드코딩
    }

    public void StartMoveBackward(float targetPosX_)
    {
        MovementStateType = MonsterMovementStateType.BACKWARD;
        _backwardTargetPosX = targetPosX_;
        _moveSpeedRate = 1f;
        _normalizedMoveDirection = 1f; //right 샘플을 위해 하드코딩
    }

    public void JumpTo(float jumpTargetPosY_)
    {
        MovementStateType = MonsterMovementStateType.JUMP;
        _moveSpeedRate = 2f;
        _jumpTargetPosY = jumpTargetPosY_;
        float height = jumpTargetPosY_ - _ownerMonster.transform.position.y;
        StartGravity(height);
    }

    public void Fall(float fallTargetPosY_)
    {
        MovementStateType = MonsterMovementStateType.FALL;
        _moveSpeedRate = 2f;
        _fallTargetPosY = fallTargetPosY_;
        StartGravity(0f);
    }

    public void Stop()
    {
        _moveSpeedRate = 1f;
        MovementStateType = MonsterMovementStateType.IDLE;
    }

    public void FindAndSetGroundAndTower()
    {
        Vector2 belowDirection = Vector2.down;
        Vector2 origin = _ownerMonster.transform.position;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, belowDirection, distance: float.MaxValue, UnityLayerDefines.LayerMask.GROUND);
        if (raycastHit2D.collider != null)
        {
            GroundPosY = raycastHit2D.point.y;
        }

        Vector2 forwardDirection = Vector2.left;
        origin = _ownerMonster.transform.position;
        raycastHit2D = Physics2D.Raycast(origin, forwardDirection, distance: float.MaxValue, UnityLayerDefines.LayerMask.TOWER);
        if (raycastHit2D.collider != null)
        {
            _towerPosX = raycastHit2D.point.x;
        }
    }

    private void UpdatePosition()
    {
        float targetPosX = _ownerMonster.transform.position.x;
        float targetPosY = _ownerMonster.transform.position.y;

        if (MovementStateType != MonsterMovementStateType.IDLE)
        {
            float moveSpeed = _moveSpeed * _moveSpeedRate;
            targetPosX += (_normalizedMoveDirection * moveSpeed * Time.deltaTime);
            targetPosX = Mathf.Max(_towerPosX + _ownerMonster.GetCollisionRadius(), targetPosX);
        }

        if (MovementStateType == MonsterMovementStateType.BACKWARD)
        {
            if (_backwardTargetPosX <= targetPosX)
            {
                targetPosX = _backwardTargetPosX;
                MovementStateType = MonsterMovementStateType.IDLE;
            }
        }

        if (MovementStateType == MonsterMovementStateType.JUMP || MovementStateType == MonsterMovementStateType.FALL)
        {
            targetPosY += (_gravityVelocityY * Time.deltaTime);
            if (MovementStateType == MonsterMovementStateType.JUMP && _gravityVelocityY <= 0f && targetPosY <= _jumpTargetPosY)
            {
                targetPosY = _jumpTargetPosY;
                if (_ownerMonster.StateModule.GetCurrentStateType() == MonsterStateType.FORWARD)
                {
                    MovementStateType = MonsterMovementStateType.FORWARD;    
                }
                else
                {
                    MovementStateType = MonsterMovementStateType.IDLE;    
                }
            }
            else if (MovementStateType == MonsterMovementStateType.FALL && targetPosY <= _fallTargetPosY)
            {
                targetPosY = _fallTargetPosY;
                if (_ownerMonster.StateModule.GetCurrentStateType() == MonsterStateType.FORWARD)
                {
                    MovementStateType = MonsterMovementStateType.FORWARD;    
                }
                else
                {
                    MovementStateType = MonsterMovementStateType.IDLE;    
                }
            }
        }

        IsGrounded = (targetPosY - _ownerMonster.GetCollisionRadius()) <= GroundPosY;
        if (IsGrounded)
        {
            targetPosY = GroundPosY + _ownerMonster.GetCollisionRadius();
            StopGravity();
            if (MovementStateType == MonsterMovementStateType.FALL)
            {
                MovementStateType = MonsterMovementStateType.IDLE;
            }
        }

        _ownerMonster.transform.position = new Vector3(targetPosX, targetPosY, _ownerMonster.transform.position.z);
    }

    private void StartGravity(float height_)
    {
        _isGravityEnabled = true;
        if (0f < height_)
        {
            _gravityVelocityY = Mathf.Sqrt(2f * -GRAVITY * height_);
        }
        else
        {
            _gravityVelocityY = 0f;
        }
    }

    private void StopGravity()
    {
        _gravityVelocityY = 0f;
        _isGravityEnabled = false;
    }

    //중력 적용
    private void UpdateGravityIfEnabled()
    {
        if (_isGravityEnabled)
        {
            _gravityVelocityY += GRAVITY * Time.deltaTime;
        }
    }
}