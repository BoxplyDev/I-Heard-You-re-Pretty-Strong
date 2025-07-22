using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GokuAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask Ground, Player;
    private Interact interact;
    private PlayerLife playerLife;

    [Header("Patrolling")]
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;
    public float minIdleTime = 2f;
    public float maxIdleTime = 5f;
    private float idleTime;
    
    private float stuckTimer = 0f;
    public float stuckThreshold = 2f;

    [Header("States")] 
    public float sightRange;
    public float catchRange;
    public bool playerInSightRange;
    public bool playerInCatchRange;
    private bool chasing;
    
    [Header("Distraction")]
    public Transform searchRamenLocation;
    public GameObject bell;
    public Transform sitPoint;
    public CupboardRamen ramenSystem;
    private bool eatingRamen;
    private bool searchingForRamen;
    private float ramenEatTime;
    private Bell bellComponent;
    public float minRamenEatTime;
    public float maxRamenEatTime;

    [Header("Stats")] 
    public float patrolSpeed;
    public float chaseSpeed;

    [Header("Animations")] public Animator animator;

    [Header("Audio Configuration")] 
    public AudioSource[] gokuAudio;
    private bool isPlaying;
    private bool played;
    private bool playingWalk;
    private bool playingRun;
    private bool playingEat;

    private Vector3 velocity = Vector3.zero;
    
    [Header("Turning")]
    public float turnSpeed = 50f;  // try values between 5 and 12
    
    [Header("Teleport Settings")]
    public float teleportCooldown = 10f;  // Time in seconds between allowed teleports
    public float teleportChance = 0.1f;
    public float teleportDistance = 4f;
    private float lastTeleportTime = -Mathf.Infinity;
    private Rigidbody playerRigidbody;
    private Camera playerCam;
    

    
    public enum GokuState { Patrol, Chase, Eat, KiKill }
    public GokuState currentState = GokuState.Patrol;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        agent.updatePosition = false;
    }

    private void Start()
    {
        interact = player.gameObject.GetComponentInChildren<Interact>();
        playerLife = player.gameObject.GetComponent<PlayerLife>();
        
        agent.updateRotation = false;
        playerRigidbody = player.GetComponent<Rigidbody>();

        bellComponent = bell.GetComponent<Bell>();
        playerCam = player.GetComponentInChildren<Camera>();
    }

    void Update()
    {
        // Update player detection each frame
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, Player);
        playerInCatchRange = Physics.CheckSphere(transform.position, catchRange, Player);

        // Determine state
        HandleStateTransitions();

        // Act based on current state
        switch (currentState)
        {
            case GokuState.Patrol:
                Patroling();
                break;
            case GokuState.Chase:
                ChasePlayer();
                TryTeleportInFrontOfPlayer();
                break;
            case GokuState.KiKill:
                KiDeath();
                break;
            case GokuState.Eat:
                FindRamen();
                break;
        }
    }
    
    void HandleStateTransitions()
    {
        // Handle ramen distraction
        if (bellComponent.bellRang && !eatingRamen && !searchingForRamen)
        {
            searchingForRamen = true;
            currentState = GokuState.Eat;
            return;
        }
        else if (playerInSightRange && interact.hiding && !playerInCatchRange && !searchingForRamen && !eatingRamen) currentState = GokuState.Patrol;

        // Handle catching while hiding
        if (playerInCatchRange && interact.hiding && chasing)
        {
            currentState = GokuState.KiKill;
            return;
        }

        // Handle chase
        if (playerInSightRange && !interact.hiding && !eatingRamen)
        {
            currentState = GokuState.Chase;
            return;
        }

        // Default to patrol
        if (!playerInSightRange && !eatingRamen && !searchingForRamen)
        {
            currentState = GokuState.Patrol;
        }
    }

    private void LateUpdate()
    {
        transform.position = agent.nextPosition;
        RotateTowardsMovement();
    }

    private void Patroling()
    {
        chasing = false;
        played = false;
        gokuAudio[3].Stop();
        playingRun = false;

        if (!playingWalk)
        {
            gokuAudio[2].Play();
            playingWalk = true;
        }

        agent.speed = patrolSpeed;

        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
            animator.SetBool("isKi", false);

            // 🔍 Check if Goku is not really moving
            if (agent.remainingDistance < 0.5f && !agent.pathPending && agent.velocity.magnitude < 0.05f)
            {
                stuckTimer += Time.deltaTime;

                if (stuckTimer >= stuckThreshold)
                {
                    walkPointSet = false;
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f; // Reset timer if he's moving
            }
        }

        Vector3 distToWalkPoint = transform.position - walkPoint;

        if (walkPointSet && distToWalkPoint.magnitude < 0.5f)
        {
            gokuAudio[2].Stop();
            playingWalk = false;
            idleTime -= Time.deltaTime;

            if (idleTime <= 0)
                walkPointSet = false;

            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isKi", false);
        }
    }


    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, Ground))
            walkPointSet = true;
        
        idleTime = Random.Range(minIdleTime, maxIdleTime);
    }

    private void ChasePlayer()
    {
        gokuAudio[2].Stop();
        playingWalk = false;
        if (!playingRun)
        {
            gokuAudio[3].Play();
            playingRun = true;
        }
        
        chasing = true;
        if (!isPlaying && !played && chasing)
        {
            played = true;
            StartCoroutine(PlaySound(0, 1.7f));
        }
        searchingForRamen = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);
        animator.SetBool("isKi", false);
    }

    private void KiDeath()
    {
        gokuAudio[3].Stop();
        playingRun = false;

        if (!playingWalk)
        {
            gokuAudio[2].Play();
            playingWalk = true;
        }
        // Player is effectively dead
        playerLife.dead = true;
        
        Transform caughtTransform = interact.hidingObj.GetComponent<HideObject>().caughtObj;
        Vector3 distToWalkPoint = transform.position - caughtTransform.position;

        if (distToWalkPoint.magnitude <= 0.5f)
        {
            gokuAudio[2].Stop();
            playingWalk = false;
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isKi", true);
            transform.LookAt(new Vector3(player.position.x, transform.position.y,player.position.z));
        }
        else
        {
            agent.SetDestination(caughtTransform.position);
        }
    }

    private void FindRamen()
    {
        if (!playingRun)
        {
            gokuAudio[3].Play();
            playingRun = true;
        }
        gokuAudio[2].Stop();
        playingWalk = false;
        
        Vector3 distToWalkPoint = transform.position - searchRamenLocation.position;
        
        chasing = false;
        agent.speed = chaseSpeed;

        if (distToWalkPoint.magnitude <= 1f)
        {
            gokuAudio[3].Stop();
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isEating", true);
            
            // Check For Ramen
            if (interact.ramenReady)
            {
                EatRamen();
            }
            else
            {
                searchingForRamen = false;
            }
        }
        else
        {
            agent.SetDestination(searchRamenLocation.position);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", true);
        }
    }

    private void EatRamen()
    {
        searchingForRamen = false;
        eatingRamen = true;
        transform.position = sitPoint.position;
        agent.SetDestination(transform.position);
        StartCoroutine(FinishRamen());
    }

    IEnumerator FinishRamen()
    {
        ramenEatTime = Random.Range(minRamenEatTime, maxRamenEatTime);
        if (!playingEat && eatingRamen)
        {
            playingEat = true;
            StartCoroutine(PlaySound(1, ramenEatTime));
        }
        yield return new WaitForSeconds(ramenEatTime);
        eatingRamen = false;
        ramenSystem.Reset();
        interact.ramenReady = false;
        gameObject.transform.position = searchRamenLocation.position;
        animator.SetBool("isEating", false);
        playingEat = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gokuAudio[2].Stop();
            gokuAudio[3].Stop();
            playerLife.Jumpscare();
            gameObject.SetActive(false);
        }
    }

    IEnumerator PlaySound(int audio, float duration)
    {
        isPlaying = true;
        gokuAudio[audio].Play();
        yield return new WaitForSeconds(duration);
        isPlaying = false;
        gokuAudio[audio].Stop();
    }
    
    private void RotateTowardsMovement()
    {
        Vector3 direction = agent.velocity;

        if (direction.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            // Use a high multiplier on Time.deltaTime to make lerp very fast but smooth
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }

    
    public float teleportCheckInterval = 0.5f;  // Only check every 0.5 seconds
    private float nextTeleportCheckTime = 0f;
    
    void TryTeleportInFrontOfPlayer()
    {
        // Time-based check to avoid checking every frame
        if (Time.time < nextTeleportCheckTime)
            return;
        nextTeleportCheckTime = Time.time + teleportCheckInterval;

        float checkWallDistance = teleportDistance + 1f;
        float navMeshCheckRadius = 1.0f;

        Camera playerCamera = playerCam;
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 teleportPos = player.position + forward * teleportDistance;

        // Cooldown check
        if (Time.time - lastTeleportTime < teleportCooldown)
            return;

        // Check if player is running
        Vector3 horizontalVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0, playerRigidbody.linearVelocity.z);
        bool isRunning = horizontalVelocity.magnitude > 6f && Input.GetKey(KeyCode.LeftShift);

        if (isRunning && Random.value < teleportChance)
        {
            if (!Physics.Raycast(player.position, forward, checkWallDistance, ~0))
            {
                if (NavMesh.SamplePosition(teleportPos, out NavMeshHit hit, navMeshCheckRadius, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);
                    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                    gokuAudio[4].Play();
                    animator.SetTrigger("Teleport");
                    lastTeleportTime = Time.time;
                }
            }
        }
    }


}