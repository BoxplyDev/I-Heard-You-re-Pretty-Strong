using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HeartbeatEffect : MonoBehaviour
{
    public Transform goku;
    public float minDistance = 3f;
    public float maxDistance = 20f;

    public float pulseSpeed = 3f;
    public float maxVignetteIntensity = 0.5f;
    public float maxChromaticIntensity = 1f;
    public float zoomIntensity = 0.02f;

    public Volume volume;
    private Vignette vignette;
    private ChromaticAberration chromatic;
    private Camera cam;
    private float originalFOV;

    [Header("Heartbeat Audio")]
    public AudioSource heartbeatAudio;
    public float minPitch = 0.8f;
    public float maxPitch = 2.0f;
    public float maxVolume = 0.5f;

    private bool wasActiveLastFrame = false;

    void Start()
    {
        // Cache post-processing references
        if (!volume.profile.TryGet(out vignette))
            Debug.LogWarning("Vignette not found on volume profile!");

        if (!volume.profile.TryGet(out chromatic))
            Debug.LogWarning("Chromatic Aberration not found on volume profile!");

        cam = GetComponent<Camera>();
        originalFOV = cam.fieldOfView;

        if (heartbeatAudio != null)
        {
            heartbeatAudio.loop = true;
            heartbeatAudio.playOnAwake = false;
        }
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, goku.position);

        if (distance > maxDistance)
        {
            if (wasActiveLastFrame)
                ResetEffects();

            wasActiveLastFrame = false;
            return;
        }

        wasActiveLastFrame = true;

        float t = Mathf.InverseLerp(maxDistance, minDistance, distance); // 0 = far, 1 = close
        float dynamicPulseSpeed = Mathf.Lerp(pulseSpeed, pulseSpeed * 4f, t); // Faster pulse when closer
        float pulse = Mathf.PingPong(Time.time * dynamicPulseSpeed, 1f);

        // Vignette intensity
        if (vignette != null)
            vignette.intensity.value = Mathf.Lerp(0f, maxVignetteIntensity, t);

        // Chromatic aberration pulsing
        if (chromatic != null)
            chromatic.intensity.value = pulse * t * maxChromaticIntensity;

        // FOV zoom (heartbeat-style)
        if (cam != null)
            cam.fieldOfView = originalFOV - (pulse * zoomIntensity * 100f * t);

        // Heartbeat Audio control
        if (heartbeatAudio != null)
        {
            if (!heartbeatAudio.isPlaying)
                heartbeatAudio.Play();

            heartbeatAudio.volume = Mathf.Lerp(0f, maxVolume, t);
            heartbeatAudio.pitch = Mathf.Lerp(minPitch, maxPitch, t);
        }
    }

    void OnDisable()
    {
        ResetEffects();
    }

    public void ResetEffects()
    {
        if (cam != null)
            cam.fieldOfView = originalFOV;

        if (vignette != null)
            vignette.intensity.value = 0f;

        if (chromatic != null)
            chromatic.intensity.value = 0f;

        if (heartbeatAudio != null && heartbeatAudio.isPlaying)
        {
            heartbeatAudio.Stop();
            heartbeatAudio.volume = 0f;
            heartbeatAudio.pitch = minPitch;
        }
    }
}
