using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour {

    public int maxNumberBounce = 0;
    public float ballAddForce = 10;

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
            Rigidbody ballBody = collision.gameObject.GetComponent<Rigidbody>();
            if(ballBody != null)
            {
                ballBody.AddForce(transform.up * ballAddForce, ForceMode.Impulse);
            }
        }
        if(numBounces >= maxNumberBounce)
        {
            Destroy(gameObject);
        }
        numBounces++;
    }
}
