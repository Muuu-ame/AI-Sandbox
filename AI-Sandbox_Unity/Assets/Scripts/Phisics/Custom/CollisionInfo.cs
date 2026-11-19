using UnityEngine;

public readonly struct CollisionInfo
{
    public PhysicsBody BodyA { get; }
    public PhysicsBody BodyB { get; }

    // BodyAからBodyBへ向かう衝突法線
    public Vector3 Normal { get; }

    // 衝突位置
    public Vector3 Point { get; }

    // めり込み量
    public float Penetration { get; }

    public CollisionInfo(
        PhysicsBody bodyA,
        PhysicsBody bodyB,
        Vector3 normal,
        Vector3 point,
        float penetration)
    {
        BodyA = bodyA;
        BodyB = bodyB;
        Normal = normal;
        Point = point;
        Penetration = penetration;
    }
}