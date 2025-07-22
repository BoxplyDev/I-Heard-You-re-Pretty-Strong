using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObject : MonoBehaviour
{
    public bool isHiding = false;
    private GameObject player;
    private Rigidbody playerRigidbody;
    public Transform escapePos;
    public Transform hidingPos;
    public Transform caughtObj;
    
    private PlayerMovement playerMov;
    private Interact _interact;
    private PlayerLife life;
    private Camera cam;
    private HintText hintCode;
    private Quaternion startRot;
    private Quaternion startPlayerRot;
    public Transform targetObjectTransform;
    private PlayerCam playerCam;
    
        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerMov = player.GetComponent<PlayerMovement>();
            _interact = player.GetComponentInChildren<Interact>();
            life = player.GetComponent<PlayerLife>();
            playerRigidbody = player.GetComponent<Rigidbody>();
            cam = player.GetComponentInChildren<Camera>();
            hintCode = FindObjectOfType<HintText>();
            playerCam = cam.gameObject.GetComponent<PlayerCam>();
        }
    
        public void Hide()
        {
            isHiding = true;
            LockPlayer();
        }
    
        private void Update() {
            // Player exited hiding area
            if (Input.GetKeyDown(KeyCode.Q) && isHiding && !life.dead) {
               isHiding = false;
               UnlockPlayer();   
            }
        }
    
        // Method to check if the player is currently hiding
        public bool IsHiding()
        {
            return isHiding;
        }
    
        private void LockPlayer()
        {
            // Disable player movement
            playerRigidbody.isKinematic = true;

            //collider.enabled = false;
            playerMov.enabled = false;
            
            // Hide Player
            player.transform.position = hidingPos.position;
            
            //Lock Camera
            CameraManagerOff();
            
            //Hint
            hintCode.PermanentHint("Press 'Q' to exit");
        }

        private void CameraManagerOff()
        {
            playerCam.enabled = false;
            startRot = cam.gameObject.transform.rotation;
            cam.gameObject.transform.rotation = new Quaternion(0,0,0,0);
            startPlayerRot = player.gameObject.transform.rotation;
            player.gameObject.transform.rotation = targetObjectTransform.rotation;
        }

        private void CameraManagerOn()
        {
            playerCam.enabled = true;
            cam.gameObject.transform.rotation = startRot;
            player.gameObject.transform.rotation = startPlayerRot;
        }
    
        private void UnlockPlayer()
        {
            hintCode.DestroyHint();
            StartCoroutine(UnHide());
        }

        IEnumerator UnHide()
        {
            // Restore the player's position
            player.transform.position = escapePos.position;
            
            //collider.enabled = true;
            yield return new WaitForFixedUpdate();
            
            if (!Physics.Raycast(player.transform.position, Vector3.down, out RaycastHit hit, 2f, LayerMask.GetMask("Ground")))
            {
                // If not grounded, adjust the position slightly above the ground
                player.transform.position = escapePos.position;
            }
            
            playerMov.enabled = true;
            playerRigidbody.isKinematic = false;
            _interact.hiding = false;
            
            // Regive Camera Control
            CameraManagerOn();
        }
}
