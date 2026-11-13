using System;
using UnityEngine;

[Serializable]
public class GameRuleParameters
{
    // プレイヤー能力
    public float? jumpForce;
    public float? moveSpeed;
    public float? rotationSpeed;

    // HP・体力
    public float? health;
    public float? maxHealth;

    // ゲーム進行
    public float? timeLimit;
    public string state;

    // 勝敗条件
    public string winCondition;
    public string loseCondition;
}
