using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Transform cam;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;
    private float dampingSpeed = 1.0f;

    private Vector3 initialPos;

    void Awake()
    {
        if (Instance == null) Instance = this;
        cam = GetComponent<Transform>();
        initialPos = cam.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            cam.localPosition = initialPos + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            cam.localPosition = initialPos;
        }
    }

    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}