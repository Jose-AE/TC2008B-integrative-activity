using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StopSignColor
{
    RED,
    YELLOW,
    GREEN,
}


public class StopSign : MonoBehaviour
{



    Material redMaterial;
    Material greenMaterial;
    Material yellowMaterial;



    public void SetColor(StopSignColor color)
    {
        redMaterial.DisableKeyword("_EMISSION");
        greenMaterial.DisableKeyword("_EMISSION");
        yellowMaterial.DisableKeyword("_EMISSION");

        switch (color)
        {
            case StopSignColor.RED:
                redMaterial.EnableKeyword("_EMISSION");
                break;
            case StopSignColor.YELLOW:
                yellowMaterial.EnableKeyword("_EMISSION");
                break;
            case StopSignColor.GREEN:
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
