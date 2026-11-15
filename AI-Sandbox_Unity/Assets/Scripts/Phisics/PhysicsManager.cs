using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    [Header("Physics Mode")]
    [SerializeField]
    private PhysicsMode currentMode = PhysicsMode.UnityStandard;

    public PhysicsMode CurrentMode => currentMode;

    [Header("Physics Solvers")]
    [SerializeField]
    private StandardPhysicsSolver standardSolver;

    [SerializeField]
    private CustomPhysicsSolver customSolver;


    // 物理演算モード切り替え
    public void SetPhysicsMode(PhysicsMode mode)
    {
        currentMode = mode;
    }


    // 独自物理演算の更新
    private void FixedUpdate()
    {
        if (currentMode == PhysicsMode.Custom)
        {
            customSolver?.Simulate(Time.fixedDeltaTime);
        }
    }


    // 全体の重力
    public void SetGravity(Vector3 gravity)
    {
        if (currentMode == PhysicsMode.UnityStandard)
        {
            standardSolver?.SetGravity(gravity);
        }
        else
        {
            // 独自物理側の重力設定は後で実装
        }
    }


    // 個別の質量
    public void SetMass(Rigidbody rb, float mass)
    {
        if (currentMode == PhysicsMode.UnityStandard)
        {
            standardSolver?.SetMass(rb, mass);
        }
        else
        {
            // 独自物理側の質量設定は後で実装
        }
    }


    // 個別の空気抵抗
    public void SetDamping(Rigidbody rb, float damping)
    {
        if (currentMode == PhysicsMode.UnityStandard)
        {
            standardSolver?.SetLinearDamping(rb, damping);
        }
        else
        {
            // 独自物理側の空気抵抗設定は後で実装
        }
    }


    // 個別の角速度減衰
    public void SetAngularDamping(Rigidbody rb, float angularDamping)
    {
        if (currentMode == PhysicsMode.UnityStandard)
        {
            standardSolver?.SetAngularDamping(rb, angularDamping);
        }
        else
        {
            // 独自物理側の角速度減衰設定は後で実装
        }
    }


    // 重力の使用設定
    public void SetUseGravity(Rigidbody rb, bool useGravity)
    {
        if (currentMode == PhysicsMode.UnityStandard)
        {
            standardSolver?.SetUseGravity(rb, useGravity);
        }
        else
        {
            // 独自物理側の重力使用設定は後で実装
        }
    }


    // Kinematic設定
    public void SetKinematic(Rigidbody rb, bool isKinematic)
    {
        if (currentMode == PhysicsMode.UnityStandard)
        {
            standardSolver?.SetKinematic(rb, isKinematic);
        }
        else
        {
            // 独自物理側の運動状態設定は後で実装
        }
    }


    // Rigidbodyの制約
    public void SetConstraints(
        Rigidbody rb,
        RigidbodyConstraints constraints)
    {
        if (currentMode == PhysicsMode.UnityStandard)
        {
            standardSolver?.SetConstraints(rb, constraints);
        }
        else
        {
            // 独自物理側の制約設定は後で実装
        }
    }


    // 動摩擦
    public void SetDynamicFriction(Collider col, float friction)
    {
        if (currentMode != PhysicsMode.UnityStandard || col == null) return;

        col.material.dynamicFriction = friction;
    }


    // 静摩擦
    public void SetStaticFriction(Collider col, float friction)
    {
        if (currentMode != PhysicsMode.UnityStandard || col == null) return;

        col.material.staticFriction = friction;
    }


    // 反発力
    public void SetBounciness(Collider col, float bounciness)
    {
        if (currentMode != PhysicsMode.UnityStandard || col == null) return;

        col.material.bounciness = bounciness;
    }
}
