using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Behavior : MonoBehaviour
{
    public GameObject settingsMenu;
    public bool active = false;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!active)
            {
                settingsMenu.SetActive(true);
                active = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                settingsMenu.SetActive(false);
                active = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
