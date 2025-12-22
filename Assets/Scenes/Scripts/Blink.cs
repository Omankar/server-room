using UnityEngine;
using System.Collections;

public class BlinkingLED : MonoBehaviour
{
    public Renderer ledRenderer;

    public Color onColor = Color.blue;
    public float onIntensity = 3f;

    public float minOnTime = 0.05f;
    public float maxOnTime = 0.3f;
    public float minOffTime = 0.1f;
    public float maxOffTime = 1f;

    private Material ledMaterial;

    void Start()
    {
        // Create a unique instance so racks don't sync blink
        ledMaterial = ledRenderer.material;
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            // ON
            ledMaterial.EnableKeyword("_EMISSION");
            ledMaterial.SetColor("_EmissionColor", onColor * onIntensity);
            yield return new WaitForSeconds(Random.Range(minOnTime, maxOnTime));

            // OFF
            ledMaterial.SetColor("_EmissionColor", Color.black);
            yield return new WaitForSeconds(Random.Range(minOffTime, maxOffTime));
        }
    }
}
