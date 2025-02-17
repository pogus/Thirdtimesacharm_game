using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skyboxmanager : MonoBehaviour
{
    public float skyspeed;
    // Start is called before the first frame update
   
    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skyspeed);
    }
}
