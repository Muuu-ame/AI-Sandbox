using System.Collections.Generic;
using UnityEngine;

public class CustomPhysicsSolver : MonoBehaviour
{
    private readonly List<PhysicsBody> bodies = new();

    [SerializeField]
    private Vector3 gravity = new Vector3(0f, -9.81f, 0f);

    public Vector3 Gravity
    {
        get => gravity;
        set => gravity = value;
    }

    public void RegisterBody(PhysicsBody body)
    {
        if (body == null || bodies.Contains(body)) return;

        bodies.Add(body);
    }

    public void UnregisterBody(PhysicsBody body)
    {
        if (body == null) return;

        bodies.Remove(body);
    }

    public void Simulate(float deltaTime)
    {
        if (deltaTime <= 0f) return;

        foreach (PhysicsBody body in bodies)
        {
            if (body == null) continue;

            // 重力を加速度に加算
            body.Acceleration += gravity;

            // 加速度から速度を更新
            body.Velocity += body.Acceleration * deltaTime;

            // 空気抵抗による減速
            body.Velocity *= Mathf.Max(
                0f,
                1f - body.LinearDamping * deltaTime
            );

            // 速度から位置を更新
            body.transform.position += body.Velocity * deltaTime;

            // 角速度の減衰
            body.AngularVelocity *= Mathf.Max(
                0f,
                1f - body.AngularDamping * deltaTime
            );

            // 角速度から回転を更新
            body.transform.Rotate(
                body.AngularVelocity * deltaTime,
                Space.World
            );

            // 加速度をリセット
            body.ResetAcceleration();
        }
    }
    public void SetBodiesPhysicsMode(bool enabled)
    {
        foreach (PhysicsBody body in bodies)
        {
            if (body == null) continue;

            body.SetCustomPhysics(enabled);
        }
    }
}
