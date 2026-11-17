using UnityEngine;

public class PhysicsBody : MonoBehaviour
{
    public void SetMass(float value)
    {
        mass = value;
    }
    private Rigidbody rb;
    private bool originalKinematic;

    [SerializeField]
    private float linearDamping = 0f;

    [SerializeField]
    private float angularDamping = 0f;

    [SerializeField]
    private Vector3 angularVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            originalKinematic = rb.isKinematic;
        }
    }

    public void SetCustomPhysics(bool enabled)
    {
        if (rb == null) return;

        rb.isKinematic = enabled ? true : originalKinematic;
    }

    [Header("Physical Properties")]
    [SerializeField]
    private float mass = 1f;

    [SerializeField]
    private Vector3 velocity;

    [SerializeField]
    private Vector3 acceleration;

    public float Mass
    {
        get => mass;
        set => mass = value;
    }

    public Vector3 Velocity
    {
        get => velocity;
        set => velocity = value;
    }

    public Vector3 Acceleration
    {
        get => acceleration;
        set => acceleration = value;
    }

    public bool IsNegativeMass => mass < 0f;

    public void ApplyForce(Vector3 force)
    {
        if (Mathf.Approximately(mass, 0f)) return;

        acceleration += force / mass;
    }

    public void ResetAcceleration()
    {
        acceleration = Vector3.zero;
    }

    public float LinearDamping
    {
        get => linearDamping;
        set => linearDamping = Mathf.Max(0f, value);
    }

    public float AngularDamping
    {
        get => angularDamping;
        set => angularDamping = Mathf.Max(0f, value);
    }

    public Vector3 AngularVelocity
    {
        get => angularVelocity;
        set => angularVelocity = value;
    }
    public void ApplyTorque(Vector3 torque)
    {
        if (Mathf.Approximately(mass, 0f)) return;

        angularVelocity += torque / Mathf.Abs(mass);
    }
}
