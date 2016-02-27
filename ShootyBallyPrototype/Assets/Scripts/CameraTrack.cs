using UnityEngine;
using System.Collections;

public class CameraTrack : MonoBehaviour {

    public Transform target;
    public float scale = 1.0f;
    public float maxDegrees = 10.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void LateUpdate()
    {
        if (target == null) return;
        Quaternion lookRotaion = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position, Vector3.up), Time.deltaTime * scale);
        transform.rotation = Quaternion.RotateTowards(Quaternion.identity, lookRotaion, maxDegrees);
    }
}
