
using UnityEngine;

public class PhysicsShape : MonoBehaviour
{
    public enum ShapeType
    {
        Sphere,
        Box,
        Capsule,
        Cylinder,
        Cone,
        Mesh
    }

    [Header("Shape Settings")]
    [SerializeField]
    private ShapeType shapeType = ShapeType.Box;

    [SerializeField]
    private Vector3 size = Vector3.one;

    public ShapeType Type => shapeType;

    public Vector3 Size => size;

    public Vector3 WorldCenter => transform.TransformPoint(Vector3.zero);

    public Bounds WorldBounds
    {
        get
        {
            Vector3 scale = transform.lossyScale;

            Vector3 scaledSize = new Vector3(
                size.x * Mathf.Abs(scale.x),
                size.y * Mathf.Abs(scale.y),
                size.z * Mathf.Abs(scale.z)
            );

            // 回転した物体も包含できるワールドAABB
            Vector3 axisX = transform.right * scaledSize.x;
            Vector3 axisY = transform.up * scaledSize.y;
            Vector3 axisZ = transform.forward * scaledSize.z;

            Vector3 worldSize = new Vector3(
                Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x),
                Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y),
                Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z)
            );

            return new Bounds(WorldCenter, worldSize);
        }
    }
}
