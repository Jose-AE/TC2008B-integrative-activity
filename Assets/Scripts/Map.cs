using UnityEngine;
using UnityEngine.Networking; // Required for UnityWebRequest
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    [System.Serializable]
    public class MapData
    {
        public Car[] cars;

        public Light[] lights;
    }

    [System.Serializable]
    public class Car
    {
        public int dirX;

        public int dirY;

        public int id;

        public int posX;

        public int posY;
    }

    [System.Serializable]
    public class Light
    {
        public int id;

        public string state;
    }




    private List<MapData> steps;
    private List<GameObject> cars;


    [SerializeField] GameObject[] carPrefabs;

    [SerializeField] StopSign[] stoplightSigns;
    [SerializeField] Parking[] parkingSpots;



    private string url = "http://127.0.0.1:5000/map-data"; // Set your API URL here

    void Start()
    {
        steps = new List<MapData>();
        cars = new List<GameObject>();
        InitParkingSpots();
        StartCoroutine(Main());
    }

    void InitParkingSpots()
    {
        foreach (var p in parkingSpots)
        {
            p.cars = carPrefabs;
            cars.Add(p.SpawnCar());
        }
    }

    IEnumerator Main()
    {
        while (true) // Infinite loop to keep sending requests
        {
            yield return StartCoroutine(MakeStepRequest(url)); // Make the request
            UpdateMap();
            yield return new WaitForSeconds(0.5f); // Wait for 1 second
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
                    //Debug.Log("Received Step Data: " + JsonUtility.ToJson(mapData));
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error parsing JSON: " + e.Message);
                }
            }
        }
    }

    void UpdateMap()
    {
        UpdateCarPositions();
        UpdateStopSigns();

    }


    void UpdateStopSigns()
    {


        for (int lightIndex = 0; lightIndex < steps[steps.Count - 1].lights.Length; lightIndex++)
        {

            Light lightData = steps[steps.Count - 1].lights[lightIndex];
            stoplightSigns[lightData.id].SetColor(lightData.state);

        }
    }


    void UpdateCarPositions()
    {
        Vector3 offset = new Vector3(5, 0, 5);
        int cellSize = 10;

        for (int carIndex = 0; carIndex < steps[steps.Count - 1].cars.Length; carIndex++)
        {


            Car carData = steps[steps.Count - 1].cars[carIndex];
            int carObjIndex = carData.id - 1;

            cars[carObjIndex].transform.position = new Vector3(carData.posX, 0, carData.posY) * cellSize + offset;

            // Set the car's rotation based on dirX and dirY
            SetCarRotation(cars[carObjIndex], carData.dirX, carData.dirY);

        }
    }


    void SetCarRotation(GameObject car, int dirX, int dirY)
    {
        Quaternion rotation = Quaternion.identity;

        if (dirX == 1 && dirY == 0)
        {
            // Facing right
            rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (dirX == -1 && dirY == 0)
        {
            // Facing left
            rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (dirX == 0 && dirY == 1)
        {
            // Facing up
            rotation = Quaternion.Euler(0, -90, 0);
        }
        else if (dirX == 0 && dirY == -1)
        {
            // Facing down
            rotation = Quaternion.Euler(0, 90, 0);
        }

        // Apply the rotation
        car.transform.rotation = rotation;
    }



}
