using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShaker : MonoBehaviour
{
    public static ScreenShaker Instance;
    CinemachineVirtualCamera virtualCam;
    CinemachineBasicMultiChannelPerlin machine;
    float shakeAmount;
    public float shakeTime = 0.5f;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        virtualCam = GetComponent<CinemachineVirtualCamera>();
        machine = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        shakeAmount -= Time.deltaTime / shakeTime;
        shakeAmount = Mathf.Clamp01(shakeAmount);

        machine.m_AmplitudeGain = Curves.QuadEaseOut(0,1,shakeAmount);
    }

    public void SmallShake()
    {
        shakeAmount = 0.4f;
    }

    public void MediumShake()
    {
        shakeAmount = 0.7f;
    }

    public void LargeShake()
    {
        shakeAmount = 1f;
    }
}
