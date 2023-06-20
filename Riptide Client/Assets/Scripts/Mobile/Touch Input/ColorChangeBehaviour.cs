using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeBehaviour : MonoBehaviour
{
    void OnTouchDown()
    {
        this.GetComponent<MeshRenderer>().material.color = Color.blue;
    }
    void OnTouchStay()
    {
        this.GetComponent<MeshRenderer>().material.color = Color.green;
    }
    void OnTouchUp()
    {
        this.GetComponent<MeshRenderer>().material.color = Color.white;
    }
    void OnTouchExit()
    {
        this.GetComponent<MeshRenderer>().material.color = Color.yellow;
    }
}
