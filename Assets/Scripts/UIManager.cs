using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [Header("WhiteFlash")]
    public Image whiteFlash;

    public GameObject youDiedText;

    [Header("UI Text")]
    public Text waveText;
    public Text pointsText;

    [Header("Privates")]
    int waveNumber;
    int pointsNumber;
    public int pointsToGive;

    public static UIManager instance;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        UpdateText();
    }

    private void Start()
    {
        Flash(true, 0.6f);
    }

    void UpdateText()
    {
        waveNumber = WaveSpawner.instance.currentWave;
        waveText.text =  "Wave: " + waveNumber;
        if(pointsToGive != 0)
        {
            pointsNumber++;
            pointsToGive--;
        }
        pointsText.text = "Points: " + pointsNumber;
    }

    public void YouDiedFlash()
    {
        StartCoroutine(YouDiedFlasher());
    }

    IEnumerator YouDiedFlasher()
    {
        yield return new WaitForSeconds(1);
        youDiedText.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        youDiedText.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        youDiedText.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        youDiedText.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        youDiedText.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        youDiedText.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        youDiedText.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        youDiedText.SetActive(false);
    }
    
    public void Flash(bool fadeOut, float flashTime)
    {
        switch (fadeOut)
        {
            case true:
                whiteFlash.CrossFadeAlpha(0, flashTime, false);
                break;
            case false:
                whiteFlash.CrossFadeAlpha(1, flashTime, false);
                break;
        }
    }
}
