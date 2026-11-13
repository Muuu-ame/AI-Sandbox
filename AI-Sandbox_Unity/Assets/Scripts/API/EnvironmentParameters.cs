using System;
using UnityEngine;

[System.Serializable]
public class EnvironmentParameters
{
    // 光源
    public Vector3Data lightPosition;
    public Vector3Data lightRotation;
    public Vector3Data lightColor;
    public float? lightIntensity;

    // 環境光
    public Vector3Data ambientColor;

    // 霧
    public bool? fogEnabled;
    public Vector3Data fogColor;
    public float? fogDensity;
    public float? fogStartDistance;
    public float? fogEndDistance;

    // 空・天候
    public string skybox;
    public string weather;
}

