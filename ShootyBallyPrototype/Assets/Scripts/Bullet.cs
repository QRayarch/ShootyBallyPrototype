using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour {

    public int maxNumberBounce = 0;
    public float ballAddForce = 10;

    private Rigidbody body;
    private Vector3 forward;
    private int numBounces = 0;

	// Use this for initialization
	void Awake () {
        body = GetComponent<Rigidbody>();
        forward = transform.up;

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
        if(collision.gameObject.GetComponent<Bullet>() != null)
        {
            numBounces = maxNumberBounce;
        }
        if(numBounces >= maxNumberBounce)
        {
            Destroy(gameObject);
        } else
        {
            body.angularVelocity = Vector3.zero;
            float angle = -Vector3.Angle(Vector3.up, Vector3.Reflect(forward, collision.contacts[0].normal));
            transform.rotation = Quaternion.Euler(0, 0, angle);
            forward = transform.up;
        }
        numBounces++;
    }
}
