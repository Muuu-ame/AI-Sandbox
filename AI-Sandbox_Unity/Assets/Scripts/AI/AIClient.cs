using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class CommandResponse
{
    public string command;
    public AICommandParameters parameters;
}

[Serializable]
public class AICommandParameters
{
    public string shape;
    public float[] position;
    public float[] scale;
    public string color;

    public float value;
    public string target;
    public float[] offset;
}

[Serializable]
public class ChatRequest
{
    public string message;
    public List<WorldObjectInfo> objects;
}

[Serializable]
public class WorldObjectInfo
{
    public string id;
    public string shape;
    public float[] position;
    public float[] scale;
    public string color;
}

public class AIClient : MonoBehaviour
{
    private const string API_URL = "http://127.0.0.1:8000/chat";

    public void SendMessageToAI(string message)
    {
        StartCoroutine(SendRequest(message));
    }

    private IEnumerator SendRequest(string message)
    {
        // 現在のワールド情報を取得
        List<WorldObjectInfo> worldObjects =
            new List<WorldObjectInfo>();

        if (GameManager.Instance != null &&
            GameManager.Instance.ObjectManager != null)
        {
            List<ManagedObject> objects =
                GameManager.Instance.ObjectManager.GetAllObjects();

            foreach (ManagedObject obj in objects)
            {
                if (obj == null) continue;

                Vector3 pos = obj.transform.position;
                Vector3 scale = obj.transform.localScale;

                Renderer objectRenderer =
                    obj.GetComponent<Renderer>();

                string color = "unknown";

                if (objectRenderer != null)
                {
                    color = "#" +
                        ColorUtility.ToHtmlStringRGB(
                            objectRenderer.sharedMaterial.color
                        );
                }

                worldObjects.Add(new WorldObjectInfo
                {
                    id = obj.ObjectID,
                    shape = "cube",
                    position = new float[]
                    {
                        pos.x, pos.y, pos.z
                    },
                    scale = new float[]
                    {
                        scale.x, scale.y, scale.z
                    },
                    color = color
                });
            }
        }

        // リクエストデータ作成
        ChatRequest requestData = new ChatRequest
        {
            message = message,
            objects = worldObjects
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request =
            new UnityWebRequest(API_URL, "POST"))
        {
            request.uploadHandler =
                new UploadHandlerRaw(bodyRaw);

            request.downloadHandler =
                new DownloadHandlerBuffer();

            request.SetRequestHeader(
                "Content-Type",
                "application/json"
            );

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson =
                    request.downloadHandler.text;

                Debug.Log("[Python] JSON Response: " + responseJson);

                CommandResponse response = null;

                try
                {
                    response =
                        JsonUtility.FromJson<CommandResponse>(
                            responseJson
                        );
                }
                catch (Exception e)
                {
                    Debug.LogError("JSON解析エラー: " + e.Message);
                    yield break;
                }

                if (response == null ||
                    string.IsNullOrEmpty(response.command))
                {
                    Debug.LogError("AIの命令を解析できませんでした");
                    yield break;
                }

                AICommandHandler handler =
                    FindFirstObjectByType<AICommandHandler>();

                if (handler == null)
                {
                    Debug.LogError("AICommandHandlerが見つかりません");
                    yield break;
                }

                handler.ExecuteCommand(response);
            }
            else
            {
                Debug.LogError(
                    "通信エラー: " + request.error +
                    "\n" + request.downloadHandler.text
                );
            }
        }
    }

    [ContextMenu("Test Create")]
    private void TestCreate()
    {
        SendMessageToAI("目の前に赤い大きな箱を作って");
    }

    [ContextMenu("Test Delete")]
    private void TestDelete()
    {
        SendMessageToAI("赤い箱を消して");
    }
}
