using UnityEngine;

public static class SphereBoxDetector
{
    public static bool TryDetect(
        PhysicsBody sphereBody,
        PhysicsShape sphere,
        PhysicsBody boxBody,
        PhysicsShape box,
        out CollisionInfo info)
    {
        info = default;

        Vector3 localCenter =
            box.transform.InverseTransformPoint(sphere.WorldCenter);

        Vector3 halfSize = box.Size * 0.5f;

        Vector3 closestLocal = new Vector3(
            Mathf.Clamp(localCenter.x, -halfSize.x, halfSize.x),
            Mathf.Clamp(localCenter.y, -halfSize.y, halfSize.y),
            Mathf.Clamp(localCenter.z, -halfSize.z, halfSize.z)
        );

        Vector3 closestWorld =
            box.transform.TransformPoint(closestLocal);

        Vector3 delta = closestWorld - sphere.WorldCenter;

        float distanceSq = delta.sqrMagnitude;
        float radius = sphere.Radius;

        if (distanceSq >= radius * radius)
            return false;

        float distance = Mathf.Sqrt(distanceSq);

        Vector3 normal;
        float penetration;

        if (distance > 0.0001f)
        {
            normal = delta / distance;
            penetration = radius - distance;
        }
        else
        {
            // 球の中心がBox内部にある場合
            Vector3 scale = box.transform.lossyScale;

            float dx = (halfSize.x - Mathf.Abs(localCenter.x))
                * Mathf.Abs(scale.x);

            float dy = (halfSize.y - Mathf.Abs(localCenter.y))
                * Mathf.Abs(scale.y);

            float dz = (halfSize.z - Mathf.Abs(localCenter.z))
                * Mathf.Abs(scale.z);

            Vector3 outward;

            if (dx <= dy && dx <= dz)
            {
                outward = box.transform.right *
                    (localCenter.x >= 0f ? 1f : -1f);
                penetration = radius + dx;
            }
            else if (dy <= dz)
            {
                outward = box.transform.up *
                    (localCenter.y >= 0f ? 1f : -1f);
                penetration = radius + dy;
            }
            else
            {
                outward = box.transform.forward *
                    (localCenter.z >= 0f ? 1f : -1f);
                penetration = radius + dz;
            }

            normal = -outward.normalized;
        }

        info = new CollisionInfo(
            sphereBody,
            boxBody,
            normal,
            closestWorld,
            penetration
        );

        return true;
    }
}
