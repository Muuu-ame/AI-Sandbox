using System;
using UnityEngine;

[Serializable]
public class CameraParameters
{
    // カメラのTransform
    public Vector3Data cameraPosition;
    public Vector3Data cameraRotation;

    // カメラ設定
    public float? fieldOfView;
    public float? nearClipPlane;
    public float? farClipPlane;

    // カメラ制御
    public string cameraMode;
    public string targetObjectId;
}
