using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using TMPro;
using Unity.Burst.CompilerServices;

public class HintText : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    [SerializeField] private float hintDuration;

    private bool isCoroutineRunning = false;
    
    public void ShowHint(string hint)
    {
        hintText.text = hint;
        if (!isCoroutineRunning)
        {
            StartCoroutine(DestroyText());
        }
    }

    IEnumerator DestroyText()
    {
        isCoroutineRunning = true;
        yield return new WaitForSeconds(hintDuration);
        hintText.text = "";
        isCoroutineRunning = false;
    }

    public void PermanentHint(string hint)
    {
        hintText.text = hint;
    }

    public void DestroyHint()
    {
        hintText.text = "";
    }
}
