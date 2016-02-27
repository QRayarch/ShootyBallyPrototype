using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {

    private bool wasScored = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            wasScored = true;
        }
    }

    public void Reset()
    {
        wasScored = false;
    }

    public bool WasScored
    {
        get { return wasScored; }
    }
}
