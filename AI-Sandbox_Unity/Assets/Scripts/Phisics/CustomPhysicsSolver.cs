using System.Collections.Generic;
using UnityEngine;

public class CustomPhysicsSolver : MonoBehaviour
{
    private CollisionResolver collisionResolver = new();
    private readonly List<PhysicsBody> bodies = new();

    [SerializeField]
    private Vector3 gravity = new Vector3(0f, -9.81f, 0f);

    [Header("Collision Detection")]
    [SerializeField] private BroadPhaseManager broadPhaseManager;

    [Header("Solver Settings")]
    [SerializeField, Range(1, 50)]
    private int solverIterations = 6;

    private SequentialImpulseSolver impulseSolver = new();

    private CollisionDetector collisionDetector = new();

    private readonly List<CollisionInfo> detectedCollisions = new();

    public IReadOnlyList<CollisionInfo> DetectedCollisions
        => detectedCollisions;

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

        // ① 物体の運動を更新
        foreach (PhysicsBody body in bodies)
        {
            if (body == null) continue;

            body.Acceleration += gravity;

            body.Velocity += body.Acceleration * deltaTime;

            body.Velocity *= Mathf.Max(
                0f,
                1f - body.LinearDamping * deltaTime
            );

            body.transform.position += body.Velocity * deltaTime;

            body.AngularVelocity *= Mathf.Max(
                0f,
                1f - body.AngularDamping * deltaTime
            );

            body.transform.Rotate(
                body.AngularVelocity * deltaTime,
                Space.World
            );

            body.ResetAcceleration();
        }

        // 衝突検出
        DetectCollisions();

        // 位置補正（従来の処理を一旦維持）
        foreach (CollisionInfo info in detectedCollisions)
        {
            collisionResolver.ResolvePosition(info);
        }

        // 速度応答（SequentialImpulseSolverに移管）
        impulseSolver.Solve(detectedCollisions);
    }

    public void SetBodiesPhysicsMode(bool enabled)
    {
        foreach (PhysicsBody body in bodies)
        {
            if (body == null) continue;

            body.SetCustomPhysics(enabled);
        }
    }

    private void DetectCollisions()
    {
        detectedCollisions.Clear();

        if (broadPhaseManager == null)
            return;

        // Broad Phaseで衝突候補を取得
        var candidates = broadPhaseManager.GenerateCandidates(bodies);

        // Narrow Phaseで実際の衝突を判定
        foreach (CollisionCandidate candidate in candidates)
        {
            if (collisionDetector.TryDetect(
                candidate.BodyA,
                candidate.BodyB,
                out CollisionInfo info))
            {
                detectedCollisions.Add(info);
            }
        }
    }
}
