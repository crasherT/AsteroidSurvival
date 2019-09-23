using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFXStates
{
    Damage,
    WaveCleared,
    teleport,
    SubExplosion,
    MainExplosion,
    Laser,
    Asteroid,
    PickUp,
    PowerDown,
}

public class SFXPlayer : MonoBehaviour
{
    [Header("AsteroidExplosions")]
    public AudioSource[] explosionSFX;

    [Header("SFX for non player Entities")]
    public AudioSource damage;
    public AudioSource waveCleared;
    public AudioSource teleport;
    public AudioSource subExplosion;
    public AudioSource mainExplosion;
    public AudioSource laser;
    public AudioSource pickUp;
    public AudioSource powerDown;

    public void PlaySFX(SFXStates whatToPlay)
    {
        switch (whatToPlay)
        {
            case SFXStates.Damage:
                damage.Play();
                break;
            case SFXStates.WaveCleared:
                waveCleared.Play();
                break;
            case SFXStates.teleport:
                teleport.Play();
                break;
            case SFXStates.SubExplosion:
                subExplosion.Play();
                break;
            case SFXStates.MainExplosion:
                mainExplosion.Play();
                break;
            case SFXStates.Laser:
                laser.Play();
                break;
            case SFXStates.PickUp:
                pickUp.Play();
                break;
            case SFXStates.PowerDown:
                powerDown.Play();
                break;
            case SFXStates.Asteroid:
                int randomNumber = Random.Range(0, explosionSFX.Length);
                explosionSFX[randomNumber].Play();
                break;

            default:
                Debug.LogError("This isn't supposed to happen! Code:SFX1");
                break;
        }
        StartCoroutine(DeleteGameObject());
    }

    IEnumerator DeleteGameObject()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
