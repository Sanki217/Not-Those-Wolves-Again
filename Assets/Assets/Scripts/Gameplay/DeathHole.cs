using UnityEngine;
using UnityEngine.VFX;

public class DeathHole : MonoBehaviour
{
    public AudioSource hellAudio;     
    public AudioSource heavenAudio;          
    public VisualEffect hellVFX;      
    public VisualEffect heavenVFX;           

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wolf") || other.CompareTag("Sheep"))
        {
            hellAudio.Play();
            hellVFX.Play();
        }
        
        else if (other.CompareTag("Player"))
        {
            
            heavenAudio.Play();
            heavenVFX.Play();
            

           
        }
    }
}
