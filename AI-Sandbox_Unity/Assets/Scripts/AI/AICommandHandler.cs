using UnityEngine;

public class AICommandHandler : MonoBehaviour
{
    public void ExecuteCommand(CommandResponse command)
    {
        if (command == null || command.parameters == null)
        {
            Debug.LogError("命令データが不正です");
            return;
        }

        switch (command.command)
        {
            case "create_object":
                CreateObject(command.parameters);
                break;

            default:
                Debug.LogWarning(
                    $"未対応の命令です: {command.command}"
                );
                break;
        }
    }

    private void CreateObject(AICommandParameters parameters)
    {
        // パラメータの確認
        if (parameters.position == null ||
            parameters.position.Length != 3 ||
            parameters.scale == null ||
            parameters.scale.Length != 3)
        {
            Debug.LogError("位置または大きさの指定が不正です");
            return;
        }

        if (parameters.shape != "cube")
        {
            Debug.LogWarning("現在はCubeのみ生成できます");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManagerが見つかりません");
            return;
        }

        Vector3 position = new Vector3(
            parameters.position[0],
            parameters.position[1],
            parameters.position[2]
        );

        Vector3 scale = new Vector3(
            parameters.scale[0],
            parameters.scale[1],
            parameters.scale[2]
        );

        // Cubeを生成
        ManagedObject obj =
            GameManager.Instance.ObjectManager.CreateObject(position);

        if (obj == null)
        {
            Debug.LogError("オブジェクトの生成に失敗しました");
            return;
        }

        // 大きさを変更
        GameManager.Instance.ObjectController.SetScale(obj, scale);

        // 色を変更
        Renderer objectRenderer =
            obj.GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            if (ColorUtility.TryParseHtmlString(
                parameters.color,
                out Color color))
            {
                objectRenderer.material.color = color;
            }
            else
            {
                Debug.LogWarning(
                    $"色を認識できません: {parameters.color}"
                );
            }
        }

        Debug.Log($"オブジェクトを生成しました: {obj.ObjectID}");
    }
}