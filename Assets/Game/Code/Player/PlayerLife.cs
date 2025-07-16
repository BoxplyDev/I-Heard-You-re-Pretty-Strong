using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerLife : MonoBehaviour
{
    public bool dead;

    [Header("Main")]
    private PlayerCam playerCam;
    private HeadBob headBob;
    private PlayerMovement pMovement;
    private Rigidbody rb;

    [Header("Jumpscare")] 
    public GameObject jumpscareGokuPrefab;
    public Transform playerCamera;
    public float jumpscareDuration = 2f;
    [SerializeField] private float yOffset;
    [SerializeField] private float cameraDist;

    private bool isJumpscaring = false;
    private GameObject jumpscareGokuInstance;

    [Header("UI")] 
    public GameObject deathUI;
    
    [Header("Post Processing")]
    public Volume globalVolume; // Drag your Global Volume here in Inspector
    private Vignette vignette;

    private void Start()
    {
        playerCam = GetComponentInChildren<PlayerCam>();
        headBob = GetComponentInChildren<HeadBob>();
        pMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        
        if (globalVolume != null && globalVolume.profile.TryGet(out vignette))
        {
            vignette.intensity.Override(0.3f); // Ensure starts from 0.3
        }
    }

    public void Die()
    {
        DisableMovement();
        deathUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Player is dead");
    }

    public void Jumpscare()
    {
        TriggerJumpscare();
        
        Debug.Log("Caught By Goku");
    }

    public void DisableMovement()
    {
        playerCam.enabled = false;
        headBob.enabled = false;
        pMovement.enabled = false;
        rb.isKinematic = true;
    }
    
    public void TriggerJumpscare()
    {
        if (!isJumpscaring)
        {
            CameraShake.Instance.Shake(0.5f, 0.15f);
            StartCoroutine(JumpscareSequence());
            FindObjectOfType<AudioManager>().Play("IT");
            FindObjectOfType<AudioManager>().Play("Jumpscare");
        }
    }
    
    private IEnumerator JumpscareSequence()
    {
        isJumpscaring = true;
        
        DisableMovement();
        
        StartCoroutine(AnimateVignette(0.5f, jumpscareDuration)); // or 0.8f for full drama

        // Instantiate the jumpscare Goku model in front of the player camera
        Vector3 jumpscarePosition = playerCamera.position + playerCamera.forward * cameraDist; // Adjust the distance as needed
        jumpscareGokuInstance = Instantiate(jumpscareGokuPrefab, new Vector3(jumpscarePosition.x, jumpscarePosition.y - yOffset, jumpscarePosition.z), Quaternion.identity);

        // Make the player camera look at the jumpscare Goku model
        jumpscareGokuInstance.transform.LookAt(playerCamera);
        jumpscareGokuInstance.transform.rotation = Quaternion.Euler(0, jumpscareGokuInstance.transform.rotation.eulerAngles.y, 0);

        // Wait for the duration of the jumpscare
        yield return new WaitForSeconds(jumpscareDuration);

        // Clean up the jumpscare Goku model
        if (jumpscareGokuInstance != null)
        {
            Destroy(jumpscareGokuInstance);
        }

        isJumpscaring = false;
        Die();
    }
    
    IEnumerator AnimateVignette(float targetIntensity, float duration)
    {
        float elapsed = 0f;
        float startIntensity = vignette.intensity.value;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, t);
            yield return null;
        }

        vignette.intensity.value = targetIntensity;
    }
}
