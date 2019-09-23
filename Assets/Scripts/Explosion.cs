using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script is used to destroy instantiated particle system so the hierarchy wont clutter up.
public class Explosion : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}

