using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float sprintSpeed;
    private float currentSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float sprintStaminaConsumptionRate = 20f;
    public float staminaRegenerationRate = 10f;
    [SerializeField] private float currentStamina;
    public StaminaBar staminaBar;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    [Header("Audio Management")] 
    public AudioManager audioManager;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        currentSpeed = moveSpeed;
        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();
        RegenerateStamina();

        // handle drag
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        // Audio Logic
        if (((verticalInput > 0f || horizontalInput > 0f) || (verticalInput < 0f || horizontalInput < 0f)) && !Input.GetKey(sprintKey) && grounded)
        {
            if(!audioManager.isPlayingAudio("walk"))
                audioManager.Play("walk");
        }
        else if ((verticalInput < 0f) && Input.GetKey(sprintKey) && grounded)
            audioManager.Play("walk");
        else if ((verticalInput <= 0f || horizontalInput <= 0f) || !Input.GetKey(sprintKey) || !grounded)
            audioManager.Stop("walk");
        
        if ((verticalInput > 0f || horizontalInput > 0f || horizontalInput < 0f) && Input.GetKey(sprintKey) && currentStamina > 0f && grounded)
        {
            if(!audioManager.isPlayingAudio("run"))
                audioManager.Play("run");
        }
        else
            audioManager.Stop("run");
        
        // Sprinting logic
        if (Input.GetKey(sprintKey) && grounded && verticalInput > 0 && currentStamina > 0f) // Sprint only when grounded and moving forward
        {
            currentSpeed = sprintSpeed; // Set movement speed to sprint speed
            currentStamina -= sprintStaminaConsumptionRate * Time.deltaTime;
            staminaBar.SetStamina(currentStamina);
        }
        else
        {
            currentSpeed = moveSpeed; // Revert to regular movement speed
        }

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();
            audioManager.Play("jump");
            
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > currentSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * currentSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    
    private void RegenerateStamina()
    {
        if (!Input.GetKey(sprintKey) && currentStamina < maxStamina) // Regenerate stamina only when not sprinting and current stamina is less than max
        {
            currentStamina += staminaRegenerationRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina); // Clamp current stamina to ensure it stays within the range [0, maxStamina]
            staminaBar.SetStamina(currentStamina);
        }
    }
}