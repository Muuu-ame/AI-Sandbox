using System;
using UnityEngine;

[System.Serializable]
public class PhysicsParameters
{
    // 対象オブジェクト
    public string objectId;

    // 全体の物理設定
    public Vector3Data gravity;

    // Rigidbody設定
    public float? mass;
    public Vector3Data velocity;
    public Vector3Data angularVelocity;
    public float? linearDamping;
    public float? angularDamping;

    public bool? useGravity;
    public bool? isKinematic;

    // Physics Material設定
    public float? dynamicFriction;
    public float? staticFriction;
    public float? bounciness;

    public string frictionCombine;
    public string bounceCombine;
}

