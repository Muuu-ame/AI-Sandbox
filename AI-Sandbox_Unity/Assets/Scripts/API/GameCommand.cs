using Newtonsoft.Json.Linq;
using System;
using System.Xml.Linq;
using UnityEngine;

[System.Serializable]
public class GameCommand
{
    public string type;
    public JObject parameters;
}

[Serializable]
public class CommandParameters
{
    // オブジェクト関連
    public string objectType;
    public string objectId;
    public string objectName;
    public bool isActive;

    // Transform関連
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    // 物理関連
    public Vector3 gravity;
    public Vector3 velocity;
    public Vector3 angularVelocity;

    public float mass;
    public float linearDamping;
    public float angularDamping;

    public float dynamicFriction;
    public float staticFriction;
    public float bounciness;

    public string frictionCombine;
    public string bounceCombine;

    public bool useGravity;
    public bool isKinematic;

    // ゲームルール関連
    public string state;
    public float jumpForce;
    public float moveSpeed;
    public float rotationSpeed;
    public float health;
    public float maxHealth;
    public float timeLimit;

    public string winCondition;
    public string loseCondition;

    // 環境・演出関連
    public string color;
    public float brightness;
    public Color lightColor;
    public float fogDensity;
    public bool fogEnabled;

    // カメラ関連
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;
    public float fieldOfView;

    // 時間関連
    public float timeScale;
}
