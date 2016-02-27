using UnityEngine;
using System.Collections;

public class PlayerTurret : MonoBehaviour {

    public enum Player
    {
        ONE = 1,
        TWO = 2
    }

    [Header("Setup")]
    public Rigidbody bulletPrefab;
    public Transform turretBarrel;
    public Transform ring;
    public SpriteRenderer goal;

    [Header("Configure")]
    public Color playerColor;
    public Player player;
    public float bulletSpeed = 1.0f;
    public float fireRate = 1.0f;
    public float fireLead = 0.2f;
    public float turnRate;
    public float turnDrag = 1.0f;

    private float turnSpeed;
    private float fireTimer = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 input = GetInputForPlayer();
        turnSpeed += input.x * turnRate * Time.deltaTime;
        turnSpeed *= turnDrag;

        Quaternion rotation = ring.rotation;
        rotation *= Quaternion.Euler(0, 0, turnSpeed);
        ring.rotation = rotation;


        fireTimer += Time.deltaTime;
        if (input.y >= 0.1f && fireTimer >= fireRate)
        {
            Fire();
            fireTimer = 0;
        }
        if(goal != null)
        {
            goal.color = playerColor;
        }
    }

    private void Fire()
    {
        Rigidbody newBullet = Instantiate(bulletPrefab) as Rigidbody;
        newBullet.AddForce(turretBarrel.up * bulletSpeed, ForceMode.Impulse);
        newBullet.transform.position = turretBarrel.position + turretBarrel.up * fireLead;
        newBullet.transform.rotation = Quaternion.Euler(0, 0, turretBarrel.rotation.eulerAngles.z);
        //Physics.IgnoreCollision()
    }

    public Vector2 GetInputForPlayer()
    {
        string playerString = "P" + ((int)player).ToString();
        return new Vector2(-Input.GetAxis(playerString + "H"), Input.GetAxis(playerString + "V"));
    }

    public void Reset(float angle)
    {
        turnSpeed = 0;
        if(ring != null)
        {
            ring.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
