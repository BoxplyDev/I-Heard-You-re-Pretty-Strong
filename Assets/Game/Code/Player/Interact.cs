using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Interact : MonoBehaviour
{
    public float interactRange = 5f;
    public KeyCode interactKey = KeyCode.E;

    public Image cursor;
    public TextMeshProUGUI identifierText;

    public bool hiding;
    [HideInInspector] public GameObject hidingObj;

    private GameObject edibleRamen;
    private Inventory playerInventory;
    [SerializeField] private LockSystem lockSys;
    public bool ramenReady;

    private AudioManager audioManager;

    private void Start()
    {
        playerInventory = gameObject.GetComponentInParent<Inventory>();
        identifierText.text = "";
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        CheckUI();
        // Check for player input to interact
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    void CheckUI()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
        {
            if (!hit.collider.gameObject.GetComponent<Interactable>())
            {
                cursor.color = Color.white;
                identifierText.text = "";
            }
        }
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange))
        {
            if (hit.collider.gameObject.GetComponent<Interactable>())
            {
                cursor.color = Color.red;
                identifierText.text = "Interact";

                if (hit.collider.gameObject.CompareTag("Key"))
                {
                    identifierText.text = identifierText.text + " | Key";
                }
                else if (hit.collider.gameObject.CompareTag("Cupboard"))
                {
                    identifierText.text = identifierText.text + " | Get Ramen";
                }
                else if (hit.collider.gameObject.CompareTag("Pot"))
                {
                    identifierText.text = identifierText.text + " | Pot";
                }
                else if (hit.collider.gameObject.CompareTag("Bowl"))
                {
                    identifierText.text = identifierText.text + " | Bowl Of Ramen";
                }
                else if (hit.collider.gameObject.CompareTag("Bell"))
                {
                    identifierText.text = identifierText.text + " | Bell";
                }
                else if (hit.collider.gameObject.CompareTag("Lock"))
                {
                    identifierText.text = identifierText.text + " | Lock";
                }
                else if (hit.collider.gameObject.CompareTag("Exit Door"))
                {
                    identifierText.text = identifierText.text + " | Exit";
                }
                else if (hit.collider.gameObject.CompareTag("Button"))
                {
                    identifierText.text = identifierText.text + " | Security Deactivation Button";
                }
                else if (hit.collider.gameObject.CompareTag("Blood"))
                {
                    identifierText.text = identifierText.text + " | Puddle Of Blood";
                }
                else if (hit.collider.gameObject.CompareTag("Randoms"))
                {
                    Debug.Log("its a random!");
                    identifierText.text = identifierText.text + " | " + hit.collider.gameObject.GetComponent<Randoms>().randomHint;
                }
            }
        }
    }
    
    void TryInteract()
    {
        // Cast a ray from the player's position forward
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange))
        {
            //Check if you can interact
            if (hit.collider.gameObject.GetComponent<Interactable>())
            {
                // Hide if it's hide interact
                if (hit.collider.gameObject.CompareTag("Hide"))
                {
                    hidingObj = hit.collider.gameObject;
                    Debug.Log("Found Obj");
                    HideObject hide = hit.collider.gameObject.GetComponent<HideObject>();
                    hide.Hide();
                    hiding = true;
                    audioManager.Play("hide");
                    audioManager.Stop("walk");
                    audioManager.Stop("run");
                }
                
                // Cupboard Mechanic
                if (hit.collider.gameObject.CompareTag("Cupboard"))
                {
                    CupboardRamen ramen = hit.collider.gameObject.GetComponent<CupboardRamen>();
                    edibleRamen = ramen.edibleRamen;
                    if (!ramen.ramenExtracted)
                    {
                        ramen.ExtractRamen();
                        audioManager.Play("cabinet");
                    }
                }
                
                // Cook Ramen
                if (hit.collider.gameObject.CompareTag("Pot"))
                {
                    Pot pot = hit.collider.gameObject.GetComponent<Pot>();
                    pot.CookRamen();
                }
                
                // Transfer Ramen
                if (hit.collider.gameObject.CompareTag("Bowl"))
                {
                    RamenBowl bowlOfRamen = hit.collider.gameObject.GetComponent<RamenBowl>();
                    if (!bowlOfRamen.haveCookedRamen)
                    {
                        bowlOfRamen.PickRamen();
                        edibleRamen.SetActive(true);
                        ramenReady = true;
                        audioManager.Play("cloth_1");
                    }
                }
                
                // Bell Ring
                if (hit.collider.gameObject.CompareTag("Bell"))
                {
                    Bell bell = hit.collider.gameObject.GetComponent<Bell>();
                    bell.RingBell();
                    audioManager.Play("bell");
                }
                
                // Key Grab
                if (hit.collider.gameObject.CompareTag("Key") && !playerInventory.hasKey)
                {
                    playerInventory.hasKey = true;
                    Destroy(hit.collider.gameObject);
                    audioManager.Play("cloth_1");
                    FindObjectOfType<HintText>().ShowHint("You Got a Key | Unlock a lock before picking any other keys");
                }
                
                // Unlock Locks
                if (hit.collider.gameObject.CompareTag("Lock") && playerInventory.hasKey)
                {
                    GameObject doorLock = hit.collider.gameObject;
                    Rigidbody rb = doorLock.GetComponent<Rigidbody>();
                    rb.isKinematic = false;
                    playerInventory.hasKey = false;
                    lockSys.locks -= 1;
                    audioManager.Play("lock");
                }
                
                //Security Button Control
                if (hit.collider.gameObject.CompareTag("Button") && lockSys.securityLock)
                {
                    lockSys.securityLock = false;
                    audioManager.Play("button");
                    Debug.Log("The Security Lock is deactivated");
                    FindObjectOfType<HintText>().ShowHint("The Security Lock is deactivated");
                }
                
                //Escape
                if (hit.collider.gameObject.CompareTag("Exit Door"))
                {
                    hit.collider.gameObject.GetComponent<ExitDoor>().Win();
                }
                
                //Blood Interaction
                if (hit.collider.gameObject.CompareTag("Blood"))
                {
                    hit.collider.gameObject.GetComponent<Blood>().BloodInformation();
                }
                
                //Random Interactions
                if (hit.collider.gameObject.CompareTag("Randoms"))
                {
                    hit.collider.gameObject.GetComponent<Randoms>().RandomInformation();
                }
            }
        }
    }

    public bool isHiding()
    {
        return hiding;
    }
}
