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



    [System.Serializable]
    public struct CarObject
    {
        public Matrix4x4 mem;
        public Mesh mesh;
    }


    public List<CarObject> cars;

    private MapData currentMapData;
    private MapData nextMapData;



    [SerializeField] GameObject[] carPrefabs;

    [SerializeField] StopSign[] stoplightSigns;
    [SerializeField] Parking[] parkingSpots;



    private float interpolationDuration = 1f; // Duration of the interpolation
    private float interpolationElapsedTime = 0f;


    private string url = "http://127.0.0.1:5000/map-data"; // Set your API URL here

    void Start()
    {

        SpawnCars();
        StartCoroutine(Main());
    }

    void SpawnCars()
    {
        cars = new List<CarObject>();
        foreach (var p in parkingSpots)
        {
            p.cars = carPrefabs;
            GameObject car = p.SpawnCar();

            cars.Add(new CarObject
            {
                mesh = car.GetComponent<MeshFilter>().mesh,
                mem = Matrix4x4.identity
            });

        }
    }

    IEnumerator Main()
    {
        while (true) // Infinite loop to keep sending requests
        {
            yield return StartCoroutine(MakeStepRequest(url, false)); // Make the request
            yield return StartCoroutine(MakeStepRequest(url, true)); // Make the request
            UpdateMap();
            yield return new WaitForSeconds(1f); // Wait for 1 second
        }
    }

    void Update()
    {
        //UpdateMap();
    }


    IEnumerator MakeStepRequest(string url, bool next)
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

                    if (next) nextMapData = mapData;
                    else currentMapData = mapData;

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

        for (int lightIndex = 0; lightIndex < currentMapData.lights.Length; lightIndex++)
        {

            Light lightData = currentMapData.lights[lightIndex];
            stoplightSigns[lightData.id].SetColor(lightData.state);

        }
    }


    void UpdateCarPositions()
    {
        Vector3 offset = new Vector3(5, 0, 5);
        int cellSize = 10;

        for (int carIndex = 0; carIndex < currentMapData.cars.Length; carIndex++)
        {
            Car carData = currentMapData.cars[carIndex];
            int carObjIndex = carData.id - 1;

            //set it to origin
            Matrix4x4 posM = VectorOperations.GetMoveToMatrix(cars[carObjIndex].mem, new Vector3(carData.posX, 0, carData.posY) * cellSize + offset);


            //move it to current pos

            //lerp it to next pos 

            //if currdir is diff from next dir lerp rotation



            Matrix4x4 mem = VectorOperations.ApplyTransformMatrixToMesh(posM, cars[carObjIndex].mesh);

            cars[carObjIndex] = new CarObject
            {
                mem = mem,
                mesh = cars[carObjIndex].mesh,
            };

            //transform.position = new Vector3(carData.posX, 0, carData.posY) * cellSize + offset;

            // Set the car's rotation based on dirX and dirY
            //SetCarRotation(cars[carObjIndex], carData.dirX, carData.dirY);

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
