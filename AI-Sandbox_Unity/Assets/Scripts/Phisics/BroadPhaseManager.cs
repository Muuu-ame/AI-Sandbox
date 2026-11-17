using System.Collections.Generic;
using UnityEngine;

public class BroadPhaseManager : MonoBehaviour
{
    [Header("Spatial Hash Settings")]
    [SerializeField]
    private float cellSize = 5f;

    private SpatialHash spatialHash;

    private readonly List<PhysicsBody> nearbyBuffer = new();

    private readonly List<CollisionCandidate> candidates = new();

    private readonly HashSet<ulong> candidateKeys = new();

    private void Awake()
    {
        spatialHash = new SpatialHash(cellSize);
    }

    public IReadOnlyList<CollisionCandidate> GenerateCandidates(
        List<PhysicsBody> bodies
    )
    {
        spatialHash.Clear();

        candidates.Clear();
        candidateKeys.Clear();

        // 空間に物体を登録
        foreach (PhysicsBody body in bodies)
        {
            if (body == null) continue;

            spatialHash.Insert(body);
        }

        // 衝突候補を抽出
        foreach (PhysicsBody body in bodies)
        {
            if (body == null) continue;

            PhysicsShape shape = body.GetComponent<PhysicsShape>();

            if (shape == null) continue;

            spatialHash.GetNearby(shape.WorldBounds, nearbyBuffer);

            foreach (PhysicsBody other in nearbyBuffer)
            {
                if (other == null || body == other) continue;

                int idA = body.GetInstanceID();
                int idB = other.GetInstanceID();

                int minID = Mathf.Min(idA, idB);
                int maxID = Mathf.Max(idA, idB);

                ulong key = ((ulong)(uint)minID << 32) | (uint)maxID;

                if (!candidateKeys.Add(key)) continue;

                candidates.Add(
                    new CollisionCandidate(body, other)
                );
            }
        }

        return candidates;
    }
}
