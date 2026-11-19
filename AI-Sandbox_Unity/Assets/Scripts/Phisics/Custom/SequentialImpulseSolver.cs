using System.Collections.Generic;
using UnityEngine;

public class SequentialImpulseSolver
{
    private int iterations = 6;

    private readonly List<float> accumulatedImpulses = new();
    private readonly List<float> restitutionTargets = new();

    public int Iterations
    {
        get => iterations;
        set => iterations = Mathf.Clamp(value, 1, 50);
    }

    public void Solve(IReadOnlyList<CollisionInfo> contacts)
    {
        if (contacts == null || contacts.Count == 0)
            return;

        accumulatedImpulses.Clear();
        restitutionTargets.Clear();

        // 衝突ごとの初期状態を保存
        for (int i = 0; i < contacts.Count; i++)
        {
            CollisionInfo contact = contacts[i];

            accumulatedImpulses.Add(0f);

            Vector3 relativeVelocity =
                contact.BodyB.Velocity - contact.BodyA.Velocity;

            float velocityAlongNormal =
                Vector3.Dot(relativeVelocity, contact.Normal);

            float restitution =
                Mathf.Min(contact.BodyA.Restitution, contact.BodyB.Restitution);

            float targetVelocity =
                velocityAlongNormal < -0.5f
                ? -restitution * velocityAlongNormal
                : 0f;

            restitutionTargets.Add(targetVelocity);
        }

        // 反復計算
        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < contacts.Count; j++)
            {
                ResolveVelocity(contacts[j], j);
            }
        }
    }

    private void ResolveVelocity(CollisionInfo contact, int index)
    {
        PhysicsBody bodyA = contact.BodyA;
        PhysicsBody bodyB = contact.BodyB;

        if (bodyA == null || bodyB == null)
            return;

        float invMassA = bodyA.Mass > 0f
            ? 1f / bodyA.Mass : 0f;

        float invMassB = bodyB.Mass > 0f
            ? 1f / bodyB.Mass : 0f;

        float totalInvMass = invMassA + invMassB;

        if (totalInvMass <= 0f)
            return;

        Vector3 relativeVelocity =
            bodyB.Velocity - bodyA.Velocity;

        float velocityAlongNormal =
            Vector3.Dot(relativeVelocity, contact.Normal);

        float deltaImpulse =
            (restitutionTargets[index] - velocityAlongNormal)
            / totalInvMass;

        float oldImpulse = accumulatedImpulses[index];

        float newImpulse =
            Mathf.Max(oldImpulse + deltaImpulse, 0f);

        float appliedImpulse = newImpulse - oldImpulse;

        accumulatedImpulses[index] = newImpulse;

        Vector3 impulse = appliedImpulse * contact.Normal;

        bodyA.Velocity -= impulse * invMassA;
        bodyB.Velocity += impulse * invMassB;
    }
}