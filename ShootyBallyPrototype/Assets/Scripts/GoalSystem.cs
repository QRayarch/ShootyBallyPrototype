using UnityEngine;
using System.Collections;

public class GoalSystem : MonoBehaviour
{

    [Header("Configure")]
    public Vector2 ballStartPos;
    public float turretStartZAngle = 0;
    public bool symetricTurretAnngleStart = true;
    public bool halfwayNonScoreStart = false;

    [Header("Setup")]
    public Transform ball;
    public PlayerInfo playerOne;
    public PlayerInfo playerTwo;

    private bool needsReset = false;
    private float ballZ;

    // Use this for initialization
    void Start()
    {
        ballZ = ball.position.z;
        ResetRound(null);
    }

    // Update is called once per frame
    void Update()
    {
        if (!needsReset)
        {
            if (playerOne.goal.WasScored)
            {
                playerTwo.score += 1;
                needsReset = true;
                ResetRound(playerOne);
            }
            if (playerTwo.goal.WasScored)
            {
                playerOne.score += 1;
                needsReset = true;
                ResetRound(playerTwo);
            }
        }
    }

    private void ResetRound(PlayerInfo scoredOnPlayer)
    {
        needsReset = false;

        //Destroy any left over bullets
        Bullet[] bullets = GameObject.FindObjectsOfType<Bullet>();
        for(int b = 0; b < bullets.Length; b++)
        {
            Destroy(bullets[b].gameObject);
        }

        //Reset ball
        if(ball != null)
        {
            TrailRenderer balltrail = ball.GetComponent<TrailRenderer>();
            float trailWidth = 0;
            if(balltrail != null)
            {
                trailWidth = balltrail.startWidth;
                balltrail.startWidth = 0;
            }
            Vector3 ballPos = new Vector3(ballStartPos.x, ballStartPos.y, ballZ);
            if(halfwayNonScoreStart && scoredOnPlayer != null)
            {
                Vector2 offsetPos = Vector2.Lerp(ballStartPos, scoredOnPlayer.transform.position, 0.5f);
                ballPos.x = offsetPos.x;
                ballPos.y = offsetPos.y;
            }
            ball.transform.position = ballPos;
            if (balltrail != null)
            {
                balltrail.startWidth = trailWidth;
            }
            Rigidbody ballR = ball.GetComponent<Rigidbody>();
            if(ballR)
            {
                ballR.angularVelocity = Vector3.zero;
                ballR.velocity = Vector3.zero;
            }
        }
        //Reset players
        if(playerOne.goal != null)
        {
            playerOne.goal.Reset();
        }
        if (playerTwo.goal != null)
        {
            playerTwo.goal.Reset();
        }
        if (playerOne.turret != null)
        {
            playerOne.turret.Reset(turretStartZAngle);
        }
        if (playerTwo.turret != null)
        {
            if(symetricTurretAnngleStart)
            {
                playerTwo.turret.Reset( 180 + turretStartZAngle);
            } else
            {
                playerTwo.turret.Reset(-turretStartZAngle);
            }

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(ballStartPos.x, ballStartPos.y, ballZ), 0.4f);
        Gizmos.color = Color.magenta;
        if (playerOne != null && playerOne.turret != null)
        {
            Gizmos.DrawRay(playerOne.turret.transform.position, Quaternion.Euler(0, 0, turretStartZAngle) * Vector3.up);
        }
        if (playerTwo != null && playerTwo.turret != null)
        {
            if (symetricTurretAnngleStart)
            {
                Gizmos.DrawRay(playerTwo.turret.transform.position, Quaternion.Euler(0, 0,  180 + turretStartZAngle) * Vector3.up);
            }
            else
            {
                Gizmos.DrawRay(playerTwo.turret.transform.position, Quaternion.Euler(0, 0, -turretStartZAngle) * Vector3.up);
            }

        }
    }
}