using UnityEngine;

public class Title_Cam : MonoBehaviour
{
    public float driftAmount = 0.05f;
    public float driftSpeed = 0.5f;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * driftSpeed) * driftAmount;
        float y = Mathf.Cos(Time.time * driftSpeed * 0.5f) * driftAmount;

        transform.position = initialPosition + new Vector3(x, y, 0);
    }
}