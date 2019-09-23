using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//WORK IN PROGRESS
public class CameraController : MonoBehaviour {

    Vector3 originalPos;
    bool cameraShakeActive = false;

    public static CameraController instance;

    void Awake()
    {
        instance = this;
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (cameraShakeActive != true)
        {
            transform.localPosition = originalPos;
        }
    }


    public void ScreenShake(float duration, float strength)
    {
        StartCoroutine(Shaker(duration, strength));
    } 

    public IEnumerator Shaker(float duration, float strength)
    {
        float pendingTime = 0;
        pendingTime = pendingTime + Time.deltaTime;

        while (duration < pendingTime)
        {
            transform.localPosition = new Vector3(Random.Range(-1, 1),Random.Range(-1,1), transform.localPosition.z);
            cameraShakeActive = true; 
        }
        yield return null;
    }
}
