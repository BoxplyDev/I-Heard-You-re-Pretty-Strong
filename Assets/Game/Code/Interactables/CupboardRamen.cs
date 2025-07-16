using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupboardRamen : MonoBehaviour
{
    public GameObject edibleRamen;
    public bool ramenExtracted;
    public GameObject ramenCup;
    public Pot pot;
    public RamenBowl ramenBowl;

    private void Start()
    {
        ramenCup.SetActive(false);
        edibleRamen.SetActive(false);
    }

    public void Reset()
    {
        ramenCup.SetActive(false);
        edibleRamen.SetActive(false);
        ramenExtracted = false;

        pot.cooking = false;
        ramenBowl.haveCookedRamen = false;
    }

    public void ExtractRamen()
    {
        ramenExtracted = true;
        ramenCup.SetActive(true);
        
        //Show some text saying you have ramen or some visual representation later
        Debug.Log("Ramen Extracted!");
        FindObjectOfType<HintText>().ShowHint("You Extracted Ramen");
    }
}
