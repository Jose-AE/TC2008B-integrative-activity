using UnityEngine;
using UnityEngine.Networking; // Required for UnityWebRequest
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    [System.Serializable]
    public class PosData
    {
        public int id;
        public int x;
        public int y;
    }

    [System.Serializable]
    public class StoplightData
    {
        public int id;
        public string color;
    }

    [System.Serializable]
    public class MapData
    {
        public PosData[] positions;
        public StoplightData[] lights;
    }

    private List<MapData> steps;


    [SerializeField] StopSign[] stopSigns;

    [SerializeField]
    private string url = "https://example.com/api/step"; // Set your API URL here

    void Start()
    {
        steps = new List<MapData>();
        StartCoroutine(Main());
    }

    IEnumerator Main()
    {
        while (true) // Infinite loop to keep sending requests
        {
            yield return StartCoroutine(MakeStepRequest(url)); // Make the request
            yield return new WaitForSeconds(1f); // Wait for 1 second
        }
    }

    IEnumerator MakeStepRequest(string url)
    {
        // Create the request
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Send the request and wait for a response
            yield return request.SendWebRequest();

            // Check for errors
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Request Error: " + request.error);
            }
            else
            {
                // Parse the JSON response into the MapData variable
                try
                {
                    MapData mapData = JsonUtility.FromJson<MapData>(request.downloadHandler.text);
                    steps.Add(mapData);
                    Debug.Log("Received Step Data: " + JsonUtility.ToJson(mapData));
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error parsing JSON: " + e.Message);
                }
            }
        }
    }





}
