using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarkButton : MonoBehaviour
{
    private BarkController barkController;

    private void Start()
    {
        // Find the BarkController component on the player GameObject
        barkController = FindObjectOfType<BarkController>();
        if (barkController == null)
        {
            Debug.LogError("BarkController not found!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            barkController.Bark();
        }
    }


    public void OnBarkButtonClicked()
    {
        // Call the Bark method from the BarkController script
        barkController.Bark();
    }

}
