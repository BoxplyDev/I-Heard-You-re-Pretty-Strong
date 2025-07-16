using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamenBowl : MonoBehaviour
{
    public bool haveCookedRamen;

    public void PickRamen()
    {
        haveCookedRamen = true;
        gameObject.SetActive(false);
        
        // Show that you got ramen
        Debug.Log("You picked up the bowl of ramen!");
        FindObjectOfType<HintText>().ShowHint("You put the ramen on the table!");
    }
}
