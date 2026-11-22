using System.Collections.Generic;
using UnityEngine;

public class SequentialImpulseSolver
{
    private int iterations = 6;

    private readonly List<float> accumulatedImpulses = new();
    private readonly List<float> restitutionTargets = new();

    private readonly List<Vector3> accumulatedFrictionImpulses = new();
    private readonly List<Vector3> contactTangents = new();

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
        accumulatedFrictionImpulses.Clear();
        contactTangents.Clear();

        for (int i = 0; i < contacts.Count; i++)
        {
            CollisionInfo contact = contacts[i];

            accumulatedImpulses.Add(0f);
            accumulatedFrictionImpulses.Add(Vector3.zero);
            contactTangents.Add(Vector3.zero);

            Vector3 relativeVelocity =
                contact.BodyB.Velocity - contact.BodyA.Velocity;

            float velocityAlongNormal =
                Vector3.Dot(relativeVelocity, contact.Normal);

            float restitution =
                Mathf.Min(
                    contact.BodyA.Restitution,
                    contact.BodyB.Restitution
                );

            float targetVelocity =
                velocityAlongNormal < -0.5f
                ? -restitution * velocityAlongNormal
                : 0f;

            restitutionTargets.Add(targetVelocity);
        }

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

        float invMassA = bodyA.Mass > 0f ? 1f / bodyA.Mass : 0f;
        float invMassB = bodyB.Mass > 0f ? 1f / bodyB.Mass : 0f;

        float totalInvMass = invMassA + invMassB;

        if (totalInvMass <= 0f)
            return;

        // 法線方向のインパルス
        Vector3 relativeVelocity = bodyB.Velocity - bodyA.Velocity;

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

        // 摩擦方向の計算
        relativeVelocity = bodyB.Velocity - bodyA.Velocity;

        Vector3 tangentVelocity =
            relativeVelocity -
            Vector3.Dot(relativeVelocity, contact.Normal)
            * contact.Normal;

        if (tangentVelocity.sqrMagnitude < 0.000001f)
            return;

        Vector3 tangent = tangentVelocity.normalized;
        contactTangents[index] = tangent;

        // 摩擦係数
        float staticFriction = Mathf.Sqrt(
            bodyA.StaticFriction * bodyB.StaticFriction
        );

        float dynamicFriction = Mathf.Sqrt(
            bodyA.DynamicFriction * bodyB.DynamicFriction
        );

        // 摩擦インパルス
        float frictionImpulse =
            -Vector3.Dot(relativeVelocity, tangent) / totalInvMass;

        Vector3 oldFriction =
            accumulatedFrictionImpulses[index];

        Vector3 candidateFriction =
            oldFriction + frictionImpulse * tangent;

        float maxStatic =
            newImpulse * staticFriction;

        float maxDynamic =
            newImpulse * dynamicFriction;

        float candidateMagnitude = candidateFriction.magnitude;

        Vector3 newFriction;

        if (candidateMagnitude <= maxStatic)
        {
            newFriction = candidateFriction;
        }
        else if (candidateMagnitude > 0.000001f)
        {
            newFriction =
                candidateFriction.normalized * maxDynamic;
        }
        else
        {
            newFriction = Vector3.zero;
        }

        Vector3 appliedFriction =
            newFriction - oldFriction;

        accumulatedFrictionImpulses[index] = newFriction;

        bodyA.Velocity -= appliedFriction * invMassA;
        bodyB.Velocity += appliedFriction * invMassB;
    }
}