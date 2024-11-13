using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraRecentre : MonoBehaviour
{
    private CinemachineFreeLook Camera;

    // Start is called before the first frame update
    void Start()
    {
        Camera = GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("CameraRecentre") == 1)
        {
            Camera.m_RecenterToTargetHeading.m_enabled = true;
        }
        else
        {
            Camera.m_RecenterToTargetHeading.m_enabled = false;
        }
    }
}
