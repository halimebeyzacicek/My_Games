using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tahta : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "arrow")
        {
           // Debug.Log("mavi ye carptý");

        }
    }
}
