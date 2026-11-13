using System;
using UnityEngine;

[System.Serializable]
public class Vector3Data
{
    public float x;
    public float y;
    public float z;

    public Vector3 ToVector3()
    {
        return new UnityEngine.Vector3(x, y, z);
    }
}
