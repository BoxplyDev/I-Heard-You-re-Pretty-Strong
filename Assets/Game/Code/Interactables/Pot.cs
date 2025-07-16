using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    public bool cooking;
    public GameObject cookParticle;
    public Transform cookLocation;
    public GameObject bowlOfRamen;
    public GameObject cupOfRamen;
    public CupboardRamen ramenStatus;
    public float cookTime;

    private GameObject cookParticleFX;
    private AudioSource audioSource;

    private void Start()
    {
        bowlOfRamen.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    public void CookRamen()
    {
        if (ramenStatus.ramenExtracted && !cooking)
        {
            cooking = true;
            cupOfRamen.SetActive(false);
            cookParticleFX = Instantiate(cookParticle, cookLocation.position, cookLocation.transform.rotation);
            StartCoroutine(Cook());
            
            audioSource.Play();
            FindObjectOfType<HintText>().ShowHint("The Ramen is Cooking");
        
            // Show Some UI saying you are now cooking
            Debug.Log("Ramen is Cooking");
        }
    }

    IEnumerator Cook()
    {
        yield return new WaitForSeconds(cookTime);
        Debug.Log("Ramen is done cooking!");
        Destroy(cookParticleFX);
        bowlOfRamen.SetActive(true);
        audioSource.Stop();
        FindObjectOfType<HintText>().ShowHint("The Ramen is Done Cooking");
    }
}
