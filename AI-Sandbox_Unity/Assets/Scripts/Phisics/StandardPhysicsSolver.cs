using UnityEngine;

public class StandardPhysicsSolver : MonoBehaviour
{
    public void SetGravity(Vector3 gravity)
    {
        Physics.gravity = gravity;
    }

    public void SetMass(Rigidbody rb, float mass)
    {
        if (rb == null || mass <= 0f) return;

        rb.mass = mass;
    }

    public void SetLinearDamping(Rigidbody rb, float damping)
    {
        if (rb == null || damping < 0f) return;

        rb.linearDamping = damping;
    }

    public void SetAngularDamping(Rigidbody rb, float damping)
    {
        if (rb == null || damping < 0f) return;

        rb.angularDamping = damping;
    }

    public void SetUseGravity(Rigidbody rb, bool useGravity)
    {
        if (rb == null) return;

        rb.useGravity = useGravity;
    }

    public void SetKinematic(Rigidbody rb, bool isKinematic)
    {
        if (rb == null) return;

        rb.isKinematic = isKinematic;
    }

    public void SetConstraints(
        Rigidbody rb,
        RigidbodyConstraints constraints)
    {
        if (rb == null) return;

        rb.constraints = constraints;
    }
}
