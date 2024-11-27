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


    private List<Mesh> carMeshes;
    private List<Mesh> originalCarMeshes;

    private MapData currentMapData;
    private MapData nextMapData;


    [SerializeField] GameObject[] carPrefabs;

    [SerializeField] StopSign[] stoplightSigns;


    [Range(0.1f, 1f)]
    [SerializeField] float interpolationDuration = 1f; // Duration of the interpolation
    private float interpolationElapsedTime = 0f;


    private string url = "http://127.0.0.1:5000/map-data"; // Set your API URL here
    private bool carsInitialized;
    private bool isUpdatingSteps;


    void Start()
    {
        isUpdatingSteps = false;
        carsInitialized = false;
    }

    void InitializeCars()
    {
        carsInitialized = true;
        carMeshes = new List<Mesh>();
        originalCarMeshes = new List<Mesh>();

        for (int i = 0; i < nextMapData.cars.Length; i++)
        {
            GameObject selectedCar = carPrefabs[i % carPrefabs.Length];
            MeshFilter meshFilter = Instantiate(selectedCar, Vector3.zero, Quaternion.identity).GetComponent<MeshFilter>();
            Mesh originalMeshCopy = Instantiate(meshFilter.sharedMesh);

            originalCarMeshes.Add(originalMeshCopy);
            carMeshes.Add(meshFilter.mesh);
        }
    }

    IEnumerator UpdateSteps()
    {
        isUpdatingSteps = true;
        yield return StartCoroutine(MakeStepRequest(url)); // Make the request

        if (!carsInitialized)
        {
            InitializeCars();
            yield return StartCoroutine(MakeStepRequest(url)); // Make the request
        };
        interpolationElapsedTime = 0;
        isUpdatingSteps = false;
        UpdateStopSigns();

    }

    void Update()
    {
        if (carsInitialized)
            UpdateCarPositions();
        else if (!isUpdatingSteps)
            StartCoroutine(UpdateSteps());

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

                    // On the first request (next is false), just set the current map data
                    if (!carsInitialized)
                    {
                        nextMapData = mapData;
                    }
                    // On subsequent requests (next is true), update both current and next
                    else
                    {
                        // Shift the data: current becomes the previous next
                        currentMapData = nextMapData;
                        // New data becomes the next
                        nextMapData = mapData;
                    }




                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error parsing JSON: " + e.Message);
                }
            }
        }
    }


    void UpdateStopSigns()
    {

        for (int lightIndex = 0; lightIndex < nextMapData.lights.Length; lightIndex++)
        {

            Light lightData = currentMapData.lights[lightIndex];
            stoplightSigns[lightData.id].SetColor(lightData.state);

        }
    }


    void UpdateCarPositions()
    {
        if (nextMapData == null || currentMapData == null) return;


        if (interpolationElapsedTime < interpolationDuration)
        {

            // Update interpolation time
            interpolationElapsedTime += Time.deltaTime;
            float t = interpolationElapsedTime / interpolationDuration;

            Vector3 offset = new Vector3(5, 0, 5);
            int cellSize = 10;


            for (int i = 0; i < currentMapData.cars.Length; i++)
            {
                Car currentCarData = currentMapData.cars[i];
                Car nextCarData = nextMapData.cars[i];
                Vector3 currentPos = new Vector3(currentCarData.posX, 0, currentCarData.posY) * cellSize + offset;
                Vector3 nextPos = new Vector3(nextCarData.posX, 0, nextCarData.posY) * cellSize + offset;


                // Reset the mesh to its original state at the origin
                carMeshes[i].vertices = originalCarMeshes[i].vertices;
                carMeshes[i].RecalculateBounds();



                //if currdir is diff from next dir lerp rotation
                // Rotation interpolation
                float currentRotation = GetRotationFromDirection(currentCarData.dirX, currentCarData.dirY);
                float nextRotation = GetRotationFromDirection(nextCarData.dirX, nextCarData.dirY);
                Matrix4x4 rotM;

                // Lerp the rotation if directions are different
                if (currentRotation != nextRotation)
                    rotM = VectorOperations.GetYRotationMatrix(VectorOperations.LerpAngle(currentRotation, nextRotation, t));
                else
                    rotM = VectorOperations.GetYRotationMatrix(currentRotation);


                //lerp it to next pos 
                Vector3 interpolatedPos = VectorOperations.Lerp(currentPos, nextPos, t);
                Matrix4x4 m = VectorOperations.GetTranslationMatrix(interpolatedPos);
                VectorOperations.ApplyTransformMatrixToMesh(m * rotM, carMeshes[i]);
            }
        }
        else if (!isUpdatingSteps)
        {
            StartCoroutine(UpdateSteps());
        }


    }


    float GetRotationFromDirection(int dirX, int dirY)
    {
        if (dirX == 1 && dirY == 0)
        {
            // Facing right
            return 0;
        }
        else if (dirX == -1 && dirY == 0)
        {
            // Facing left
            return 180;
        }
        else if (dirX == 0 && dirY == 1)
        {
            // Facing up
            return 270;
        }
        else if (dirX == 0 && dirY == -1)
        {
            // Facing down
            return 90;
        }

        return 0;

    }



}
