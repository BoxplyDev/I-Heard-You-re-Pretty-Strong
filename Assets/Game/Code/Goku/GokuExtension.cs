using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GokuExtension : MonoBehaviour
{
    [Header("Ki Blast Settings")] 
    public GameObject kiBlastObj;
    public Transform firePoint;
    public float kiSpeed = 25f;

    private GameObject kiBlast; // currently held blast

    [Header("Audio")] 
    public AudioSource kiBlastAudio;

    public void KiSummon()
    {
        kiBlastAudio.Play();
        
        if (kiBlast != null) return; // prevent double-spawn

        kiBlast = Instantiate(kiBlastObj, firePoint.position, firePoint.rotation);
        kiBlast.transform.SetParent(firePoint); // attach to hand
        Rigidbody rb = kiBlast.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // disable physics while held
        }
    }

    public void KiBlast()
    {
        if (kiBlast == null) return;

        kiBlast.transform.SetParent(null); // detach from hand

        Rigidbody rb = kiBlast.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = firePoint.forward * kiSpeed;
        }

        kiBlast = null; // clear reference so new blasts can be summoned later
    }
}
