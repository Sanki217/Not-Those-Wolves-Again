using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

public class DeathHole : MonoBehaviour
{
    public AudioSource hellAudio;
    public AudioSource heavenAudio;
    public VisualEffect hellVFX;
    public VisualEffect heavenVFX;
    public float effectDelay = 0.5f;  // Delay in seconds before effects start

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wolf") || other.CompareTag("Sheep"))
        {
            StartCoroutine(PlayHellEffectsWithDelay());
        }
        else if (other.CompareTag("Player"))
        {
            StartCoroutine(PlayHeavenEffectsWithDelay());
        }
    }

    private IEnumerator PlayHellEffectsWithDelay()
    {
        yield return new WaitForSeconds(effectDelay); // Wait for the specified delay
        hellAudio.Play();
        hellVFX.Play();
    }

    private IEnumerator PlayHeavenEffectsWithDelay()
    {
        yield return new WaitForSeconds(effectDelay); // Wait for the specified delay
        heavenAudio.Play();
        heavenVFX.Play();
    }
}
