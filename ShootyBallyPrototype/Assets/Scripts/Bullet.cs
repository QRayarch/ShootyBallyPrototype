using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour {

    public int maxNumberBounce = 0;

    private Rigidbody body;
    private int numBounces = 0;

	// Use this for initialization
	void Awake () {
        body = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.isTrigger) return;
        if (collision.gameObject.CompareTag("Ball"))
        {
            numBounces = maxNumberBounce;
        }
        if(numBounces >= maxNumberBounce)
        {
            Destroy(gameObject);
        }
        numBounces++;
    }
}
