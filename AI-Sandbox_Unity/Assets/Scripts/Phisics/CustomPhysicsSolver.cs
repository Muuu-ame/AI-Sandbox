using UnityEngine;

public class CustomPhysicsSolver : MonoBehaviour
{
    public void Simulate(float deltaTime)
    {
        if (deltaTime <= 0f) return;

        // 独自物理演算の更新処理をここに実装する
    }
}
