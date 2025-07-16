using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bell : MonoBehaviour
{
    public bool bellRang;
    [SerializeField] private float bellCooldown;
    
    public void RingBell()
    {
        if (!bellRang)
        {
            Debug.Log("You rang the bell!");
            bellRang = true;
            StartCoroutine(ResetBell());
        }
    }

    IEnumerator ResetBell()
    {
        yield return new WaitForSeconds(bellCooldown);
        bellRang = false;
    }
}
