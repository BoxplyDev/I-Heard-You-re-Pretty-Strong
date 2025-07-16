using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("Headbob Settings")]
    public float idleBobSpeed = 0.05f;       // Speed of headbob when the player is idle
    public float idleBobAmount = 0.01f;      // Amount of headbob when the player is idle
    public float walkBobSpeed = 0.15f;       // Speed of headbob when the player is walking
    public float walkBobAmount = 0.03f;      // Amount of headbob when the player is walking
    public float runBobSpeed = 0.25f;        // Speed of headbob when the player is running
    public float runBobAmount = 0.05f;
    public float xMultiplier;

    [Header("Player Movement")]
    public Rigidbody playerRigidbody;        // Reference to the player's Rigidbody
    public float walkSpeed = 5.0f;           // Threshold speed for walking
    public float runSpeed = 10.0f;           // Threshold speed for running

    private float defaultYPos = 0;           // Default Y position of the camera
    private float defaultXPos = 0;           // Default X position of the camera
    private float timer = 0;                 // Timer used to calculate the sine wave for headbob
    private float currentBobSpeed;
    private float currentBobAmount;

    void Start()
    {
        defaultYPos = transform.localPosition.y; // Store the default Y position of the camera
        defaultXPos = transform.localPosition.x; // Store the default X position of the camera
        currentBobSpeed = idleBobSpeed;
        currentBobAmount = idleBobAmount;
    }

    void Update()
    {
        float speed = new Vector3(playerRigidbody.linearVelocity.x, 0, playerRigidbody.linearVelocity.z).magnitude; // Calculate horizontal speed
        bool isRunning = speed > walkSpeed && Input.GetKey(KeyCode.LeftShift); // Check if the player is running

        if (speed < 0.1f)
        {
            currentBobSpeed = Mathf.Lerp(currentBobSpeed, idleBobSpeed, Time.deltaTime * 5);
            currentBobAmount = Mathf.Lerp(currentBobAmount, idleBobAmount, Time.deltaTime * 5);
        }
        else if (isRunning)
        {
            currentBobSpeed = Mathf.Lerp(currentBobSpeed, runBobSpeed, Time.deltaTime * 5);
            currentBobAmount = Mathf.Lerp(currentBobAmount, runBobAmount, Time.deltaTime * 5);
        }
        else
        {
            currentBobSpeed = Mathf.Lerp(currentBobSpeed, walkBobSpeed, Time.deltaTime * 5);
            currentBobAmount = Mathf.Lerp(currentBobAmount, walkBobAmount, Time.deltaTime * 5);
        }

        HeadBobEffect(currentBobSpeed, currentBobAmount);
    }

    void HeadBobEffect(float bobSpeed, float bobAmount)
    {
        timer += bobSpeed * Time.deltaTime; // Increment the timer based on the bobbing speed
        float newXPos = defaultXPos + Mathf.Sin(timer) * bobAmount * xMultiplier; // Calculate the new X position using a sine wave
        float newYPos = defaultYPos + Mathf.Sin(timer * 2) * bobAmount;    // Calculate the new Y position using a sine wave
        transform.localPosition = new Vector3(newXPos, newYPos, transform.localPosition.z); // Apply the new X and Y positions
    }
}