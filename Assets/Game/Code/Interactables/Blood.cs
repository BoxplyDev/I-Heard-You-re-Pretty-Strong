using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    public string bloodInfo;
    public void BloodInformation()
    {
        FindObjectOfType<HintText>().ShowHint(bloodInfo);
    }
}
