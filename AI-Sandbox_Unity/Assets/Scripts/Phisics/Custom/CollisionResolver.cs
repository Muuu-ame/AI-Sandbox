using UnityEngine;

public class CollisionResolver
{

    [SerializeField]
    private float correctionPercent = 0.8f;

    [SerializeField]
    private float penetrationSlop = 0.01f;

    public void ResolvePosition(CollisionInfo info)
    {
        PhysicsBody bodyA = info.BodyA;
        PhysicsBody bodyB = info.BodyB;

        if (bodyA == null || bodyB == null)
            return;

        float massA = bodyA.Mass;
        float massB = bodyB.Mass;

        // 現段階では負の質量は位置補正に使用しない
        float invMassA = massA > 0f ? 1f / massA : 0f;
        float invMassB = massB > 0f ? 1f / massB : 0f;

        float totalInvMass = invMassA + invMassB;

        if (totalInvMass <= 0f)
            return;

        float correctionMagnitude =
            Mathf.Max(info.Penetration - penetrationSlop, 0f)
            * correctionPercent / totalInvMass;

        Vector3 correction = info.Normal * correctionMagnitude;

        bodyA.transform.position -= correction * invMassA;
        bodyB.transform.position += correction * invMassB;
    }
    public void ResolveVelocity(CollisionInfo info)
    {
        PhysicsBody bodyA = info.BodyA;
        PhysicsBody bodyB = info.BodyB;

        if (bodyA == null || bodyB == null)
            return;

        float massA = bodyA.Mass;
        float massB = bodyB.Mass;

        // 現段階では正の質量のみ対応
        float invMassA = massA > 0f ? 1f / massA : 0f;
        float invMassB = massB > 0f ? 1f / massB : 0f;

        float totalInvMass = invMassA + invMassB;

        if (totalInvMass <= 0f)
            return;

        Vector3 relativeVelocity =
            bodyB.Velocity - bodyA.Velocity;

        float velocityAlongNormal =
            Vector3.Dot(relativeVelocity, info.Normal);

        // すでに離れている場合は何もしない
        if (velocityAlongNormal > 0f)
            return;

        float restitution = Mathf.Min(
            bodyA.Restitution,
            bodyB.Restitution
        );

        float impulseMagnitude =
            -(1f + restitution) * velocityAlongNormal
            / totalInvMass;

        Vector3 impulse = impulseMagnitude * info.Normal;

        bodyA.Velocity -= impulse * invMassA;
        bodyB.Velocity += impulse * invMassB;
    }
}
