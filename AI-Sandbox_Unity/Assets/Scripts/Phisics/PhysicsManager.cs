using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    // 全体の重力
    public void SetGravity(Vector3 gravity)
    {
        Physics.gravity = gravity;
    }

    // 個別の質量
    public void SetMass(Rigidbody rb, float mass)
    {
        rb.mass = mass;
    }

    // 個別の空気抵抗
    public void SetDamping(Rigidbody rb, float damping)
    {
        rb.linearDamping = damping;
    }

    public void SetAngularDamping(Rigidbody rb, float angularDamping)
    {
        rb.angularDamping = angularDamping;
    }

    public void SetUseGravity(Rigidbody rb, bool useGravity)
    {
        rb.useGravity = useGravity;
    }

    public void SetKinematic(Rigidbody rb, bool isKinematic)
    {
        rb.isKinematic = isKinematic;
    }

    public void SetConstraints(Rigidbody rb, RigidbodyConstraints constraints)
    {
        rb.constraints = constraints;
    }

    // 動摩擦
    public void SetDynamicFriction(Collider col, float friction)
    {
        if (col == null) return;

        col.material.dynamicFriction = friction;
    }

    // 静摩擦
    public void SetStaticFriction(Collider col, float friction)
    {
        if (col == null) return;

        col.material.staticFriction = friction;
    }

    // 反発力
    public void SetBounciness(Collider col, float bounciness)
    {
        if (col == null) return;

        col.material.bounciness = bounciness;
    }
}