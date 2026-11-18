using UnityEngine;
using System.Collections.Generic;

public static class SphereSphereDetector
{
    public static bool TryDetect(
        PhysicsBody bodyA,
        PhysicsShape shapeA,
        PhysicsBody bodyB,
        PhysicsShape shapeB,
        out CollisionInfo info)
    {
        info = default;

        Vector3 centerA = shapeA.WorldCenter;
        Vector3 centerB = shapeB.WorldCenter;

        float radiusA = shapeA.Radius;
        float radiusB = shapeB.Radius;

        Vector3 delta = centerB - centerA;

        float distanceSq = delta.sqrMagnitude;
        float radiusSum = radiusA + radiusB;

        if (distanceSq >= radiusSum * radiusSum)
            return false;

        float distance = Mathf.Sqrt(distanceSq);

        Vector3 normal = distance > 0.0001f
            ? delta / distance
            : Vector3.up;

        float penetration = radiusSum - distance;

        Vector3 point =
            centerA + normal * (radiusA - penetration * 0.5f);

        info = new CollisionInfo(
            bodyA,
            bodyB,
            normal,
            point,
            penetration
        );

        return true;
    }
}
