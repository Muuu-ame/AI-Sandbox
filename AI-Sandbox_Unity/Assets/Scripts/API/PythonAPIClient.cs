
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PythonAPIClient : MonoBehaviour
{
    private const string ApiUrl = "http://127.0.0.1:8000/command";

    [System.Serializable]
    private class TestRequest
    {
        public string message;
    }

    [System.Serializable]
    private class APIResponse
    {
        public string message;
    }

    [ContextMenu("Send Test Request")]
    public void SendTestRequest()
    {
        StartCoroutine(SendRequest());
    }

    private IEnumerator SendRequest()
    {
        TestRequest requestData = new TestRequest
        {
            message = "Unityからのテストです"
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] body = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request =
            new UnityWebRequest(ApiUrl, "POST");

        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Pythonからの応答: " + request.downloadHandler.text);

            APIResponse response =
                JsonUtility.FromJson<APIResponse>(
                    request.downloadHandler.text
                );

            Debug.Log(response.message);
        }
        else
        {
            Debug.LogError("通信エラー: " + request.error);
        }
    }
}
