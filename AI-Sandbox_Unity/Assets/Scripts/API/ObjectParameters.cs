using System;
using UnityEngine;

[System.Serializable]
public class ObjectParameters
{
    // オブジェクト識別
    public string objectId;
    public string objectName;
    public string objectType;

    // Transform
    public Vector3Data position;
    public Vector3Data rotation;
    public Vector3Data scale;

    // 外観・状態
    public string color;
    public bool? isActive;
}
