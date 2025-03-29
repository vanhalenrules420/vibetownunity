using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DuskLightController : MonoBehaviour
{
    [SerializeField] private Light2D globalLight; // Assign your Global Light 2D here
    [SerializeField] private float fadeDuration = 60f; // Time in seconds to fade to night
    [SerializeField] private float minIntensity = 0.1f; // Minimum light intensity (night)
    [SerializeField] private float maxShadowsIntensity = 0.1f; // Minimum light intensity (night)

    private float startIntensity;
    private float startShadowsIntensity;
    private float elapsedTime;

    void Start()
    {
        if (globalLight == null)
        {
            globalLight = GetComponent<Light2D>();
        }
        startIntensity = globalLight.intensity; // Store initial intensity (0.4)
        elapsedTime = 0f;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / fadeDuration); // Progress from 0 to 1
        globalLight.intensity = Mathf.Lerp(startIntensity, minIntensity, t); // Fade intensity
        globalLight.shadowIntensity = Mathf.Lerp(startShadowsIntensity, maxShadowsIntensity, t); // Fade intensity
    }
}