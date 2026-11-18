using UnityEngine;

public static class BoxBoxDetector
{
    public static bool TryDetect(
        PhysicsBody bodyA,
        PhysicsShape shapeA,
        PhysicsBody bodyB,
        PhysicsShape shapeB,
        out CollisionInfo info)
    {
        info = default;

        Transform transformA = shapeA.transform;
        Transform transformB = shapeB.transform;

        Vector3 centerA = shapeA.WorldCenter;
        Vector3 centerB = shapeB.WorldCenter;
        Vector3 delta = centerB - centerA;

        Vector3 scaleA = transformA.lossyScale;
        Vector3 scaleB = transformB.lossyScale;

        Vector3 halfA = Vector3.Scale(
            shapeA.Size,
            new Vector3(
                Mathf.Abs(scaleA.x),
                Mathf.Abs(scaleA.y),
                Mathf.Abs(scaleA.z)
            )
        ) * 0.5f;

        Vector3 halfB = Vector3.Scale(
            shapeB.Size,
            new Vector3(
                Mathf.Abs(scaleB.x),
                Mathf.Abs(scaleB.y),
                Mathf.Abs(scaleB.z)
            )
        ) * 0.5f;

        Vector3 axisA0 = transformA.right;
        Vector3 axisA1 = transformA.up;
        Vector3 axisA2 = transformA.forward;

        Vector3 axisB0 = transformB.right;
        Vector3 axisB1 = transformB.up;
        Vector3 axisB2 = transformB.forward;

        float minOverlap = float.PositiveInfinity;
        Vector3 bestAxis = Vector3.zero;

        // Aの3軸
        if (!TestAxis(axisA0, delta, halfA, halfB,
            axisA0, axisA1, axisA2,
            axisB0, axisB1, axisB2,
            ref minOverlap, ref bestAxis)) return false;

        if (!TestAxis(axisA1, delta, halfA, halfB,
            axisA0, axisA1, axisA2,
            axisB0, axisB1, axisB2,
            ref minOverlap, ref bestAxis)) return false;

        if (!TestAxis(axisA2, delta, halfA, halfB,
            axisA0, axisA1, axisA2,
            axisB0, axisB1, axisB2,
            ref minOverlap, ref bestAxis)) return false;

        // Bの3軸
        if (!TestAxis(axisB0, delta, halfA, halfB,
            axisA0, axisA1, axisA2,
            axisB0, axisB1, axisB2,
            ref minOverlap, ref bestAxis)) return false;

        if (!TestAxis(axisB1, delta, halfA, halfB,
            axisA0, axisA1, axisA2,
            axisB0, axisB1, axisB2,
            ref minOverlap, ref bestAxis)) return false;

        if (!TestAxis(axisB2, delta, halfA, halfB,
            axisA0, axisA1, axisA2,
            axisB0, axisB1, axisB2,
            ref minOverlap, ref bestAxis)) return false;

        // AとBの軸の外積による9軸
        if (!TestCrossAxis(axisA0, axisB0, delta, halfA, halfB,
            axisA0, axisA1, axisA2,
            axisB0, axisB1, axisB2,
            ref minOverlap, ref bestAxis)) return false;

        if (!TestCrossAxis(axisA0, axisB1, delta, halfA, halfB,
            axisA0, axisA1, axisA2,
            axisB0, axisB1, axisB2,
            ref minOverlap, ref bestAxis)) return false;

        if (!TestCrossAxis(axisA0, axisB2, delta, halfA, halfB,
            axisA0, axisA1, axisA2,
            axisB0, axisB1, axisB2,
            ref minOverlap, ref bestAxis)) return false;

        if (!TestCrossAxis(axisA1, axisB0, delta, halfA, halfB,
            axisA0, axisA1, axisA2,
            axisB0, axisB1, axisB2,
            ref minOverlap, ref bestAxis)) return false;

        if (!TestCrossAxis(axisA1, axisB1, delta, halfA, halfB,
            axisA0, axisA1, axisA2,
            axisB0, axisB1, axisB2,
            ref minOverlap, ref bestAxis)) return false;

        if (!TestCrossAxis(axisA1, axisB2, delta, halfA, halfB,
            axisA0, axisA1, axisA2,
            axisB0, axisB1, axisB2,
            ref minOverlap, ref bestAxis)) return false;

        if (!TestCrossAxis(axisA2, axisB0, delta, halfA, halfB,
            axisA0, axisA1, axisA2,
            axisB0, axisB1, axisB2,
            ref minOverlap, ref bestAxis)) return false;

        if (!TestCrossAxis(axisA2, axisB1, delta, halfA, halfB,
            axisA0, axisA1, axisA2,
            axisB0, axisB1, axisB2,
            ref minOverlap, ref bestAxis)) return false;

        if (!TestCrossAxis(axisA2, axisB2, delta, halfA, halfB,
            axisA0, axisA1, axisA2,
            axisB0, axisB1, axisB2,
            ref minOverlap, ref bestAxis)) return false;

        info = new CollisionInfo(
            bodyA,
            bodyB,
            bestAxis,
            (centerA + centerB) * 0.5f,
            minOverlap
        );

        return true;
    }

    private static bool TestCrossAxis(
        Vector3 axisA,
        Vector3 axisB,
        Vector3 delta,
        Vector3 halfA,
        Vector3 halfB,
        Vector3 a0, Vector3 a1, Vector3 a2,
        Vector3 b0, Vector3 b1, Vector3 b2,
        ref float minOverlap,
        ref Vector3 bestAxis)
    {
        return TestAxis(
            Vector3.Cross(axisA, axisB),
            delta,
            halfA,
            halfB,
            a0, a1, a2,
            b0, b1, b2,
            ref minOverlap,
            ref bestAxis
        );
    }

    private static bool TestAxis(
        Vector3 axis,
        Vector3 delta,
        Vector3 halfA,
        Vector3 halfB,
        Vector3 a0, Vector3 a1, Vector3 a2,
        Vector3 b0, Vector3 b1, Vector3 b2,
        ref float minOverlap,
        ref Vector3 bestAxis)
    {
        float lengthSq = axis.sqrMagnitude;

        if (lengthSq < 0.000001f)
            return true;

        axis /= Mathf.Sqrt(lengthSq);

        float radiusA =
            halfA.x * Mathf.Abs(Vector3.Dot(axis, a0)) +
            halfA.y * Mathf.Abs(Vector3.Dot(axis, a1)) +
            halfA.z * Mathf.Abs(Vector3.Dot(axis, a2));

        float radiusB =
            halfB.x * Mathf.Abs(Vector3.Dot(axis, b0)) +
            halfB.y * Mathf.Abs(Vector3.Dot(axis, b1)) +
            halfB.z * Mathf.Abs(Vector3.Dot(axis, b2));

        float distance = Mathf.Abs(Vector3.Dot(delta, axis));
        float overlap = radiusA + radiusB - distance;

        if (overlap <= 0f)
            return false;

        if (overlap < minOverlap)
        {
            minOverlap = overlap;
            bestAxis = Vector3.Dot(delta, axis) >= 0f
                ? axis
                : -axis;
        }

        return true;
    }
}