using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;

    private Dictionary<string, ManagedObject> objects = new();

    // オブジェクト生成
    public ManagedObject CreateObject(Vector3 position)
    {
        GameObject obj = Instantiate(
            cubePrefab,
            position,
            Quaternion.identity
        );

        ManagedObject managedObject = obj.GetComponent<ManagedObject>();

        if (managedObject == null)
        {
            Destroy(obj);
            Debug.LogError("ManagedObjectがPrefabにありません");
            return null;
        }

        string id = System.Guid.NewGuid().ToString();

        managedObject.Initialize(id);
        objects.Add(id, managedObject);

        return managedObject;
    }

    // オブジェクト削除
    public void DeleteObject(string id)
    {
        if (!objects.TryGetValue(id, out ManagedObject obj))
        {
            Debug.LogWarning($"ID: {id} のオブジェクトが見つかりません");
            return;
        }

        objects.Remove(id);
        Destroy(obj.gameObject);
    }

    // IDからオブジェクトを検索
    public ManagedObject FindObject(string id)
    {
        objects.TryGetValue(id, out ManagedObject obj);
        return obj;
    }

    // 管理中のオブジェクト一覧
    public List<ManagedObject> GetAllObjects()
    {
        return new List<ManagedObject>(objects.Values);
    }
}