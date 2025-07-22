using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI sens;
    public UI_Behavior uiScript;
    // Start is called before the first frame update
    void Start()
    {
        slider.value = FindObjectOfType<PlayerCam>().senseX;
    }

    public void ApplyChanges()
    {
        FindObjectOfType<PlayerCam>().senseX = slider.value;
        FindObjectOfType<PlayerCam>().senseY = slider.value;
    }

    public void UpdateSens()
    {
        sens.text = slider.value.ToString();
    }

    public void CloseMenu()
    {
        uiScript.active = false;
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
