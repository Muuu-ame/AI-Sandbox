using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// Pythonから受け取るJSON
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

        using (UnityWebRequest request = new UnityWebRequest(API_URL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                CommandResponse response =
                    JsonUtility.FromJson<CommandResponse>(
                        request.downloadHandler.text
                    );

                Debug.Log("Command: " + response.command);
                Debug.Log("Shape: " + response.parameters.shape);
                Debug.Log("Position: " + string.Join(", ", response.parameters.position));
                Debug.Log("Scale: " + string.Join(", ", response.parameters.scale));
                Debug.Log("Color: " + response.parameters.color);
            }
            else
            {
                Debug.LogError("通信エラー: " + request.error);
            }
        }
    }
    [ContextMenu("Test AI Command")]
    private void TestAICommand()
    {
        SendMessageToAI("目の前に赤い大きな箱を作って");
    }
}

[Serializable]
public class ChatRequest
{
    public string message;
}