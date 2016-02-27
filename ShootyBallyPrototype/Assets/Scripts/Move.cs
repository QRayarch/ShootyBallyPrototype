using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour
{
    [Header("Configure")]
    public float timeForTransition;
    public Vector2 endPos;

    private Vector2 startPos = Vector2.zero;
    private Vector3 goingToPos;
    private Vector3 goingFromPos;
    private Vector3 absPos;

    private float time = 0;

    // Use this for initialization
    void Start()
    {
        absPos = transform.position;
        startPos = Vector2.zero;
        goingToPos = endPos;
        goingFromPos = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime / timeForTransition;
        Vector3 newPos = absPos + (Vector3)Vector2.Lerp(goingFromPos, goingToPos, time);
        transform.position = newPos;
        if(time >= 1)
        {
            Vector2 temp = goingFromPos;
            goingFromPos = goingToPos;
            goingToPos = temp;
            time = 0;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        if(absPos == Vector3.zero)
        {
            absPos = transform.position;
        }
        Gizmos.DrawWireCube(absPos + (Vector3)startPos, transform.localScale);
        Gizmos.DrawWireCube(absPos + (Vector3)endPos, transform.localScale);
    }
}
