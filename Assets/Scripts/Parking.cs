using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parking : MonoBehaviour
{
    [HideInInspector] public GameObject[] cars; // Array to hold the car prefabs


    // Method to spawn a car
    public GameObject SpawnCar()
    {
        if (cars.Length == 0) return null;

        // Randomly select a car from the array
        int randomIndex = Random.Range(0, cars.Length);
        GameObject selectedCar = cars[randomIndex];

        // Instantiate the car at the spawn point (or use the object's position as spawn point)
        return Instantiate(selectedCar, transform.position, Quaternion.Euler(0, 0, 0));
    }
}
