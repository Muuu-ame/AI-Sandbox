using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 input_Move;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 2f;

    [Header("Ground Movement")]
    [SerializeField] private float _groundAcceleration = 5f;
    [SerializeField] private float _speedLimit = 10f;

    [Header("Air Movement")]
    [SerializeField] private float _airAcceleration = 2f;
    [SerializeField] private float _airDrag = 0.5f;

    [Header("Jump")]
    [SerializeField] private float _jumpSpeed = 5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;
    private bool canJump = true;
    private bool hasLeftGround = false;

    private float rotationY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        input_Move = context.ReadValue<Vector2>();
    }

    // ジャンプ
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!canJump) return;

        Vector3 velocity = rb.linearVelocity;
        velocity.y = _jumpSpeed;
        rb.linearVelocity = velocity;

        canJump = false;
        hasLeftGround = false;
    }

    private void FixedUpdate()
    {
        CheckGround();
        Move();
        LimitSpeed();
        RotatePlayer();
    }

    private void RotatePlayer()
    {
        Vector3 moveDirection = new Vector3(
            input_Move.x,
            0f,
            input_Move.y
        );

        // 入力がない場合は向きを維持
        if (moveDirection.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(moveDirection);

        rb.MoveRotation(
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                _rotationSpeed * Time.fixedDeltaTime
            )
        );
    }

    // 接地判定とジャンプ権の管理
    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        // 一度でも地面から離れたことを記録
        if (!isGrounded)
        {
            hasLeftGround = true;
        }

        // 空中に出た後、着地した場合のみジャンプ可能にする
        if (isGrounded && hasLeftGround)
        {
            canJump = true;
            hasLeftGround = false;
        }
    }

    // 移動
    private void Move()
    {
        // プレイヤーの向きを基準に移動方向を計算
        Vector3 moveDirection =
            transform.forward * input_Move.y +
            transform.right * input_Move.x;

        moveDirection.y = 0f;
        moveDirection.Normalize();

        float acceleration = isGrounded
            ? _groundAcceleration
            : _airAcceleration;

        rb.AddForce(
            moveDirection * acceleration,
            ForceMode.Acceleration
        );

        // 空中抵抗
        if (!isGrounded)
        {
            Vector3 horizontalVelocity = new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );

            rb.AddForce(
                -horizontalVelocity * _airDrag,
                ForceMode.Acceleration
            );
        }
    }

    // 水平方向の速度制限
    private void LimitSpeed()
    {
        Vector3 velocity = rb.linearVelocity;

        Vector3 horizontalVelocity = new Vector3(
            velocity.x,
            0f,
            velocity.z
        );

        if (horizontalVelocity.magnitude > _speedLimit)
        {
            horizontalVelocity =
                horizontalVelocity.normalized * _speedLimit;

            rb.linearVelocity = new Vector3(
                horizontalVelocity.x,
                velocity.y,
                horizontalVelocity.z
            );
        }
    }

    // 接地判定の可視化
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
    private void Update()
    {
        float mouseX = Mouse.current != null
            ? Mouse.current.delta.ReadValue().x
            : 0f;

        rotationY += mouseX * _rotationSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }
}
