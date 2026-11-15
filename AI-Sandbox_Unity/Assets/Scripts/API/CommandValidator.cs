using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class CommandValidator : MonoBehaviour
{
    // 命令名の変換・確認
    public bool TryParseCommandType(
        string commandName,
        out CommandType commandType)
    {
        commandType = default;

        if (string.IsNullOrWhiteSpace(commandName))
            return false;

        string enumName = ToEnumName(commandName);

        if (!Enum.TryParse(enumName, out commandType))
            return false;

        return Enum.IsDefined(typeof(CommandType), commandType);
    }

    // 命令名を変換
    private string ToEnumName(string commandName)
    {
        string[] words = commandName.Split('_');

        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length == 0)
                continue;

            words[i] =
                char.ToUpperInvariant(words[i][0]) +
                words[i].Substring(1);
        }

        return string.Join("", words);
    }

    // 命令全体の検証
    public bool TryValidate(
        GameCommand command,
        out string error)
    {
        error = null;

        if (command == null)
        {
            error = "命令が存在しません";
            return false;
        }

        if (!TryParseCommandType(command.type, out _))
        {
            error = $"未定義の命令です: {command.type}";
            return false;
        }

        if (command.parameters == null)
        {
            error = "parametersが存在しません";
            return false;
        }

        switch (command.type)
        {
            case "set_mass":
                return ValidateMass(command.parameters, out error);

            case "set_gravity":
                return ValidateGravity(command.parameters, out error);

            default:
                error = "この命令の検証はまだ実装されていません";
                return false;
        }
    }

    // 質量の検証
    private bool ValidateMass(
        JObject parameters,
        out string error)
    {
        error = null;

        JToken idToken;
        JToken massToken;

        if (!parameters.TryGetValue("objectId", out idToken) ||
            idToken.Type != JTokenType.String ||
            string.IsNullOrWhiteSpace(idToken.Value<string>()))
        {
            error = "objectIdが指定されていません";
            return false;
        }

        if (!parameters.TryGetValue("mass", out massToken) ||
            (massToken.Type != JTokenType.Integer &&
             massToken.Type != JTokenType.Float))
        {
            error = "massが数値で指定されていません";
            return false;
        }

        float mass = massToken.Value<float>();

        if (float.IsNaN(mass) || float.IsInfinity(mass) ||
            mass <= 0f || mass > 10000f)
        {
            error = "massは0より大きく10000以下にしてください";
            return false;
        }

        return true;
    }

    // 重力の検証
    private bool ValidateGravity(
        JObject parameters,
        out string error)
    {
        error = null;

        if (!parameters.TryGetValue("gravity", out JToken token) ||
            token.Type != JTokenType.Object)
        {
            error = "gravityが指定されていません";
            return false;
        }

        Vector3Data gravity;

        try
        {
            gravity = token.ToObject<Vector3Data>();
        }
        catch (Exception)
        {
            error = "gravityの形式が正しくありません";
            return false;
        }

        if (gravity == null)
        {
            error = "gravityを読み取れません";
            return false;
        }

        if (!IsValidNumber(gravity.x) ||
            !IsValidNumber(gravity.y) ||
            !IsValidNumber(gravity.z))
        {
            error = "gravityに不正な数値が含まれています";
            return false;
        }

        if (Mathf.Abs(gravity.x) > 100f ||
            Mathf.Abs(gravity.y) > 100f ||
            Mathf.Abs(gravity.z) > 100f)
        {
            error = "重力の各成分は-100から100の範囲にしてください";
            return false;
        }

        return true;
    }

    private bool IsValidNumber(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
