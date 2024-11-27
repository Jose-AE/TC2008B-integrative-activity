using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StopSign : MonoBehaviour
{

    Light pointLight;


    Material redMaterial;
    Material greenMaterial;
    Material yellowMaterial;



    public void SetColor(string color)
    {
        redMaterial.DisableKeyword("_EMISSION");
        greenMaterial.DisableKeyword("_EMISSION");
        yellowMaterial.DisableKeyword("_EMISSION");

        switch (color)
        {
            case "RED":
                redMaterial.EnableKeyword("_EMISSION");
                pointLight.color = Color.red;
                break;
            case "YELLOW":
                yellowMaterial.EnableKeyword("_EMISSION");
                pointLight.color = Color.yellow;
                break;
            case "GREEN":
                greenMaterial.EnableKeyword("_EMISSION");
                pointLight.color = Color.green;
                break;
            default:
                Debug.LogWarning("Unhandled StopSignColor: " + color);
                break;
        }

    }



    // Start is called before the first frame update
    void Start()
    {
        pointLight = GetComponentInChildren<Light>();
        redMaterial = transform.Find("Red").GetComponent<MeshRenderer>().material;
        greenMaterial = transform.Find("Green").GetComponent<MeshRenderer>().material;
        yellowMaterial = transform.Find("Yellow").GetComponent<MeshRenderer>().material;

    }


}
