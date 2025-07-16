using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randoms : MonoBehaviour
{
    public string randomInfo;
    public string randomHint;
    
    public void RandomInformation()
    {
        FindObjectOfType<HintText>().ShowHint(randomInfo);
    }
}
