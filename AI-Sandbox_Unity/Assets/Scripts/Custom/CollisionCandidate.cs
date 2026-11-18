using UnityEngine;

public readonly struct CollisionCandidate
{
    public PhysicsBody BodyA { get; }
    public PhysicsBody BodyB { get; }

    public CollisionCandidate(PhysicsBody bodyA, PhysicsBody bodyB)
    {
        BodyA = bodyA;
        BodyB = bodyB;
    }
}

