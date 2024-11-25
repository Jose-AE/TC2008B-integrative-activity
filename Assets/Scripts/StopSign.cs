using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StopSign : MonoBehaviour
{



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
                break;
            case "YELLOW":
                yellowMaterial.EnableKeyword("_EMISSION");
                break;
            case "GREEN":
                greenMaterial.EnableKeyword("_EMISSION");
                break;
            default:
                Debug.LogWarning("Unhandled StopSignColor: " + color);
                break;
        }

    }



    // Start is called before the first frame update
    void Start()
    {
        redMaterial = transform.Find("Red").GetComponent<MeshRenderer>().material;
        greenMaterial = transform.Find("Green").GetComponent<MeshRenderer>().material;
        yellowMaterial = transform.Find("Yellow").GetComponent<MeshRenderer>().material;

    }


}
