using UnityEngine;

public class Light_Flicker : MonoBehaviour
{
    private Light myLight;
    private float targetIntensity;
    private float flickerTimer;
    private float nextFlickerTime;

    [Header("Flicker Settings")]
    public float minFlickerDelay = 0.05f;
    public float maxFlickerDelay = 0.2f;
    public float minIntensity = 0.4f;
    public float maxIntensity = 1f;
    public float smoothSpeed = 5f;

    [Header("Player Detection")]
    public Transform player;
    public float activationDistance = 10f;

    private bool playerNearby;
    private AudioSource flickerAudio;

    void Start()
    {
        myLight = GetComponent<Light>();
        flickerAudio = GetComponent<AudioSource>();

        if (myLight == null || flickerAudio == null)
        {
            Debug.LogWarning("Missing Light or AudioSource component!");
            enabled = false;
            return;
        }

        flickerAudio.loop = true;
        targetIntensity = myLight.intensity;
        SetNextFlicker();
    }

    void Update()
    {
        bool inRange = PlayerInRange();

        if (inRange && !playerNearby)
        {
            flickerAudio.Play();
            playerNearby = true;
        }
        else if (!inRange && playerNearby)
        {
            flickerAudio.Stop();
            playerNearby = false;
        }

        if (!inRange) return;

        flickerTimer += Time.deltaTime;
        if (flickerTimer >= nextFlickerTime)
        {
            Flicker();
            SetNextFlicker();
        }

        myLight.intensity = Mathf.Lerp(myLight.intensity, targetIntensity, Time.deltaTime * smoothSpeed);
    }

    bool PlayerInRange()
    {
        if (!player) return true;
        return Vector3.Distance(player.position, transform.position) <= activationDistance;
    }

    void Flicker()
    {
        targetIntensity = Random.Range(minIntensity, maxIntensity);
    }

    void SetNextFlicker()
    {
        flickerTimer = 0f;
        nextFlickerTime = Random.Range(minFlickerDelay, maxFlickerDelay);
    }
}