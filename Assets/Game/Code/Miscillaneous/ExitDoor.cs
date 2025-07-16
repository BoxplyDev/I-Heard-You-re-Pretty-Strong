using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public LockSystem lockSystem;
    public GameObject escapeUI;
    public PlayerLife playerLife;
    public GameObject goku;

    public void Win()
    {
        if (lockSystem.locks == 0 && !lockSystem.securityLock)
        {
            Debug.Log("You Escaped!");
            playerLife.DisableMovement();
            Destroy(goku);
            escapeUI.SetActive(true);
        }
        else if (lockSystem.locks == 0 && lockSystem.securityLock)
            FindObjectOfType<HintText>().ShowHint("You haven't disabled the security lock...");
        else
        {
             Debug.Log("Door is locked");
             FindObjectOfType<HintText>().ShowHint("Door is Locked!");
        }
    }
}
