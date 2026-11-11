using UnityEngine;

public class ManagedObject : MonoBehaviour
{
    public string ObjectID { get; private set; }

    public void Initialize(string id)
    {
        ObjectID = id;
    }
}
