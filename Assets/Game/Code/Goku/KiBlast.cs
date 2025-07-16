using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KiBlast : MonoBehaviour
{
    public GameObject explodeParticle;

    private void OnCollisionEnter(Collision other)
    {
        Instantiate(explodeParticle, transform.position, Quaternion.identity);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>().Die();
        Destroy(gameObject);
    }
}
