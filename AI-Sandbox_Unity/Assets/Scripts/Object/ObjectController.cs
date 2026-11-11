using UnityEngine;

public class ObjectController : MonoBehaviour
{
    // 絶対座標で移動
    public void SetPosition(ManagedObject obj, Vector3 position)
    {
        obj.transform.position = position;
    }

    // 現在位置から相対移動
    public void Move(ManagedObject obj, Vector3 offset)
    {
        obj.transform.position += offset;
    }

    // 絶対角度で回転
    public void SetRotation(ManagedObject obj, Vector3 rotation)
    {
        obj.transform.rotation = Quaternion.Euler(rotation);
    }

    // 現在の角度から相対回転
    public void Rotate(ManagedObject obj, Vector3 rotation)
    {
        obj.transform.Rotate(rotation);
    }

    // 大きさ変更
    public void SetScale(ManagedObject obj, Vector3 scale)
    {
        obj.transform.localScale = scale;
    }

    public void SetActive(ManagedObject obj, bool isActive)
    {
        obj.gameObject.SetActive(isActive);
    }
}