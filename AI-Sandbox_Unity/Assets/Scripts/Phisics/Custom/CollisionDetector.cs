using UnityEngine;

public class CollisionDetector
{
    public bool TryDetect(
        PhysicsBody bodyA,
        PhysicsBody bodyB,
        out CollisionInfo info)
    {
        info = default;

        if (bodyA == null || bodyB == null)
            return false;

        PhysicsShape shapeA = bodyA.GetComponent<PhysicsShape>();
        PhysicsShape shapeB = bodyB.GetComponent<PhysicsShape>();

        if (shapeA == null || shapeB == null)
            return false;

        var typeA = shapeA.Type;
        var typeB = shapeB.Type;

        // Sphere × Sphere
        if (typeA == PhysicsShape.ShapeType.Sphere &&
            typeB == PhysicsShape.ShapeType.Sphere)
        {
            return SphereSphereDetector.TryDetect(
                bodyA, shapeA, bodyB, shapeB, out info);
        }

        // Sphere × Box
        if (typeA == PhysicsShape.ShapeType.Sphere &&
            typeB == PhysicsShape.ShapeType.Box)
        {
            return SphereBoxDetector.TryDetect(
                bodyA, shapeA, bodyB, shapeB, out info);
        }

        // Box × Sphere
        if (typeA == PhysicsShape.ShapeType.Box &&
            typeB == PhysicsShape.ShapeType.Sphere)
        {
            bool hit = SphereBoxDetector.TryDetect(
                bodyB, shapeB, bodyA, shapeA, out info);

            if (!hit) return false;

            info = new CollisionInfo(
                bodyA,
                bodyB,
                -info.Normal,
                info.Point,
                info.Penetration
            );

            return true;
        }

        // Box × Box
        if (typeA == PhysicsShape.ShapeType.Box &&
            typeB == PhysicsShape.ShapeType.Box)
        {
            return BoxBoxDetector.TryDetect(
                bodyA, shapeA, bodyB, shapeB, out info);
        }

        return false;
    }
}
