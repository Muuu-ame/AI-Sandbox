using System;
using System.Collections;
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
        ChatRequest requestData = new ChatRequest
        {
            message = message
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request =
            new UnityWebRequest(API_URL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader(
                "Content-Type",
                "application/json"
            );

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text;

                Debug.Log("[Python] JSON Response: " + responseJson);

                try
                {
                    CommandResponse response =
                        JsonUtility.FromJson<CommandResponse>(
                            responseJson
                        );

                    if (response == null || string.IsNullOrEmpty(response.command))
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

                    if (response.parameters != null)
                    {
                        if (response.parameters.position != null)
                            Debug.Log("Position: " + string.Join(", ", response.parameters.position));

                        if (response.parameters.scale != null)
                            Debug.Log("Scale: " + string.Join(", ", response.parameters.scale));

                        Debug.Log("Color: " + response.parameters.color);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("JSON解析エラー: " + e.Message);
                }
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

    [ContextMenu("Test AI Command")]
    private void TestAICommand()
    {
        SendMessageToAI("目の前に赤い大きな箱を作って");
    }
    [ContextMenu("Test AI Command2")]
    private void TestAICommand2()
    {
        SendMessageToAI("目の前の赤い大きな箱を軽くして");
    }
    [ContextMenu("Test AI Command3")]
    private void TestAICommand3()
    {
        SendMessageToAI("目の前の赤い大きな箱を消して");
    }
}