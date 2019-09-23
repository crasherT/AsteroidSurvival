using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointTag : MonoBehaviour {

    public Text pointText;
    public float timer;
    public float moveSpeed;

    void Start()
    {
        StartCoroutine(DisapearTimer());
    }

    //moves the text up slightly
    private void FixedUpdate()
    {
        transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);
    }

    //assigns points gets called from asteroid script
    public void PointAssigner(int points)
    {
        pointText.text = "+"  + points;
    }
    //makes the text disapear after a certain amount of time
    IEnumerator DisapearTimer()
    {
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }
}
