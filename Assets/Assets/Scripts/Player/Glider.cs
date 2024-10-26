using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glider : MonoBehaviour
{
    public GameObject glider;
    public GameObject parachute;
    private bool isInDeathHole = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathHole"))
        {
            isInDeathHole = true;
            ActivateParachute();
            Debug.Log("ChuteOpened"); 
        }
    }
    private void Update()
    {
        if (!isInDeathHole && transform.position.y < -0.001f && !glider.activeSelf)
        {
            glider.SetActive(true);
        }
    }

   

    private void ActivateParachute()
    {
        parachute.SetActive(true);
    }
   
}