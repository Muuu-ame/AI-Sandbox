
using System;
using UnityEngine;

public class CommandValidator : MonoBehaviour
{
    public bool TryParseCommandType(
        string commandName,
        out CommandType commandType)
    {
        commandType = default;

        if (string.IsNullOrWhiteSpace(commandName))
        {
            Debug.LogWarning("命令名が空です");
            return false;
        }

        string enumName = ToEnumName(commandName);

        if (!Enum.TryParse(enumName, out commandType))
        {
            Debug.LogWarning($"未定義の命令です: {commandName}");
            return false;
        }

        if (!Enum.IsDefined(typeof(CommandType), commandType))
        {
            Debug.LogWarning($"許可されていない命令です: {commandName}");
            return false;
        }

        return true;
    }

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
}
