using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Networking;

public enum PlayerRole {
    Fire, Water
}

public class PlayerController : NetworkBehaviour {

	public GameObject fireballPrefab;
    public GameObject waterballPrefab;
	public Transform bulletSpawn;

    // for world rotation
    float currentWorldAngle = 0.0f;
    float targetWorldAngle = 0.0f;
    bool rotationIsAnimating = false;

	private int spellType;

	// Audio
	private AudioSource audioS;
	public AudioClip fireSpell;
	public AudioClip waterSpell;
	public AudioClip fall1;
	public AudioClip fall2;
	public AudioClip getHit1;
	public AudioClip getHit2;
	private bool falling = false;

    // for picking up co-op partner
    bool isHoldingObject = false;
    GameObject carrying;
    bool isBeingHeld = false;
    GameObject carrier;

    // for jumping
    bool isGrounded = false;
    Vector3 jump = new Vector3(0.0f, 1.0f, 0.0f);

    // role assignment
    public PlayerRole playerElement;

    // levels
    public Vector3 level1FireSpawn;
    public Vector3 level1WaterSpawn;
    public Vector3 level2FireSpawn;
    public Vector3 level2WaterSpawn;
    public Vector3 level3FireSpawn;
    public Vector3 level3WaterSpawn;



	public override void OnStartLocalPlayer()
	{
		audioS = GetComponent<AudioSource>();

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControllerWorldView>().LookAtOrigin();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioPlayer>().SetSound(4, true, true);
		GameObject.FindGameObjectWithTag("Title").GetComponent<Title>().Disappear();
		
		GameObject.FindGameObjectWithTag("Main Camera").gameObject.GetComponent<AudioPlayer>().SetSound(4, true, true);
	}


    void Start()
    {
        var fireAttire = Resources.Load("Materials/Fire Attire", typeof(Material)) as Material;
        var waterAttire = Resources.Load("Materials/Water Attire", typeof(Material)) as Material;

        var hat = this.transform.Find("Hat");

        if (!isLocalPlayer)
        {
            // Turn off native gravity from the RigidBody
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }

        if ((isServer && isLocalPlayer) || (!isServer && !isLocalPlayer))
        { playerElement = PlayerRole.Fire; }
        else
        { playerElement = PlayerRole.Water; }

        if (playerElement == PlayerRole.Fire)
        {
            hat.Find("Hat brim").GetComponent<Renderer>().material = fireAttire;
            hat.Find("Hat body 1").GetComponent<Renderer>().material = fireAttire;
            hat.Find("Hat body 2").GetComponent<Renderer>().material = fireAttire;
            hat.Find("Hat body 3").GetComponent<Renderer>().material = fireAttire;
            hat.Find("Hat tip").GetComponent<Renderer>().material = fireAttire;
			spellType = 2;
        }
        else if (playerElement == PlayerRole.Water)
        {
            hat.Find("Hat brim").GetComponent<Renderer>().material = waterAttire;
            hat.Find("Hat body 1").GetComponent<Renderer>().material = waterAttire;
            hat.Find("Hat body 2").GetComponent<Renderer>().material = waterAttire;
            hat.Find("Hat body 3").GetComponent<Renderer>().material = waterAttire;
            hat.Find("Hat tip").GetComponent<Renderer>().material = waterAttire;
			spellType = 1;
        }
    }


	public void OnDestroy()
	{
		if (!isLocalPlayer)
		{ return; }

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControllerWorldView>().LookUp();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioPlayer>().SetSound(1, true, true);
		GameObject.FindGameObjectWithTag("Win Text").gameObject.GetComponent<Text>().text = "";
		GameObject.FindGameObjectWithTag("Title").GetComponent<Title>().Appear();

		GameObject.FindGameObjectWithTag("Spawn1").transform.position = new Vector3(-6.0f, 5.0f, 2.0f);
		GameObject.FindGameObjectWithTag("Spawn2").transform.position = new Vector3(6.0f, 5.0f, 2.0f);
	}


	[Command]
	void CmdFire(GameObject shooter, Vector3 spawnPosition, Quaternion spawnRotation)
	{
        //Debug.Log("Spawning Bullet");
		// Create the Bullet from the Bullet Prefab
    	//var bullet = (GameObject)Instantiate(bulletPrefab, spawnPosition, spawnRotation);

	    // Add velocity to the bullet
    	//bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

		// Spawn the bullet on the Clients
		//NetworkServer.Spawn(bullet);

	    // Destroy the bullet after 2 seconds
	    //Destroy(bullet, 2.0f);

        RpcFire(shooter, spawnPosition, spawnRotation);
	}

    [ClientRpc]
    void RpcFire(GameObject shooter, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        if (shooter.gameObject.GetComponent<PlayerController>().playerElement == PlayerRole.Fire)
        {
            // Create the Fire Projectile from the Fireball Prefab
            var ball = (GameObject)Instantiate(fireballPrefab, spawnPosition, spawnRotation);

            // Add velocity to the ball
            ball.GetComponent<Rigidbody>().velocity = ball.transform.forward * 12;

            // Destroy the bullet after 2 seconds
            Destroy(ball, 1.0f);
        }
        else if (shooter.gameObject.GetComponent<PlayerController>().playerElement == PlayerRole.Water)
        {
            // Create the Water Projectile from the Waterball Prefab
            var ball = (GameObject)Instantiate(waterballPrefab, spawnPosition, spawnRotation);

            // Add velocity to the ball
            ball.GetComponent<Rigidbody>().velocity = ball.transform.forward * 12;

            // Destroy the bullet after 2 seconds
            Destroy(ball, 2.0f);
        }
    }
    
    /*void ClientFire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }*/

    [ClientRpc]
    void RpcTeleport(GameObject obj, Vector3 newPosition)
    {
        obj.transform.position = newPosition;
    }

    [Command]
    void CmdTeleport(GameObject obj, Vector3 newPosition)
    {
        RpcTeleport(obj, newPosition);
    }

    [ClientRpc]
    void RpcPickUp(GameObject objCarrier, GameObject objCarried)
    {
        objCarrier.GetComponent<PlayerController>().isHoldingObject = true;
        objCarried.GetComponent<PlayerController>().isBeingHeld = true;

        objCarried.transform.position = objCarrier.transform.position + new Vector3(0.0f, 2.5f, 0.0f);
        objCarrier.GetComponent<PlayerController>().carrying = objCarried;
        objCarried.GetComponent<PlayerController>().carrier = objCarrier;
    }

    [Command]
    void CmdPickUp(GameObject objCarrier, GameObject objCarried)
    {
        RpcPickUp(objCarrier, objCarried);
    }

    [ClientRpc]
    void RpcPutDown(GameObject objCarrier, GameObject objCarried)
    {
        objCarrier.GetComponent<PlayerController>().isHoldingObject = false;
        objCarried.GetComponent<PlayerController>().isBeingHeld = false;

        objCarried.transform.position = objCarrier.transform.position + objCarrier.transform.TransformDirection(Vector3.forward * 1.5f);
        objCarrier.GetComponent<PlayerController>().carrying = null;
        objCarried.GetComponent<PlayerController>().carrier = null;
    }

    [Command]
    void CmdPutDown(GameObject objCarrier, GameObject objCarried)
    {
        RpcPutDown(objCarrier, objCarried);
    }

    [ClientRpc]
    void RpcSetFree(GameObject objCarrier, GameObject objCarried)
    {
        objCarrier.GetComponent<PlayerController>().isHoldingObject = false;
        objCarried.GetComponent<PlayerController>().isBeingHeld = false;

        objCarrier.GetComponent<PlayerController>().carrying = null;
        objCarried.GetComponent<PlayerController>().carrier = null;
    }

    [Command]
    void CmdSetFree(GameObject objCarrier, GameObject objCarried)
    {
        RpcSetFree(objCarrier, objCarried);
    }

    public void TPme (Vector3 newPosition)
    {
        if (isLocalPlayer)
            this.gameObject.transform.position = newPosition;
    }



	// Friendly fire
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Fire" || collision.gameObject.tag == "Water")
		{
			if (spellType == 1)
			{ audioS.clip = getHit1; }
			else
			{ audioS.clip = getHit2; }

			audioS.loop = false;		
			audioS.Play();
		}
	}



	void Update()
	{
        float cameraDistance = GameObject.Find("Main Camera").GetComponent<CameraControllerWorldView>().cameraDistanceFromOrigin;
        float cameraHeight   = GameObject.Find("Main Camera").GetComponent<CameraControllerWorldView>().cameraHeightFromOrigin;

        float worldRotationSpeed = GameObject.Find("Main Camera").GetComponent<CameraControllerWorldView>().worldRotationSpeed;

		// Play falling sound if about to die
		if (transform.position.y <= (GetComponent<Health>().depthUntilRespawn * -1) + 15.0f && falling == false)
		{
			falling = true;
			if (spellType == 1)
			{ audioS.clip = fall1; }
			else
			{ audioS.clip = fall2; }

			audioS.loop = false;			
			audioS.Play();
		}
        
        if (!isLocalPlayer)
		{ return; }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 180.0f;
        transform.Rotate(0, x, 0);

        if (!isBeingHeld)
        {
            if (isLocalPlayer)
            {
                GetComponent<Rigidbody>().useGravity = true;
            }
            var z = Input.GetAxis("Vertical") * Time.deltaTime * 5.0f;
            transform.Translate(0, 0, z);
        }
        else
        {
            if (isLocalPlayer)
            {
                GetComponent<Rigidbody>().useGravity = false;
            }
            transform.position = carrier.transform.position + new Vector3(0.0f, 2.5f, 0.0f);
        }

        // check if player is grounded
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1.4f, Color.green);
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), 1.4f))
        {
            isGrounded = true;
			falling = false;
        }
        else
        {
            isGrounded = false;
        }

        // Firing
        //Debug.Log("Checking for K");
        if (Input.GetKeyDown(KeyCode.K))
		{
            //Debug.Log("K pressed");
            if (isServer)
            {
                RpcFire(this.gameObject, bulletSpawn.position, bulletSpawn.rotation);
            }
            else
            {
                CmdFire(this.gameObject, bulletSpawn.position, bulletSpawn.rotation);
            }
            
        }
        
        // Picking up a player
        var layerMask = 1 << 8;
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1.5f, Color.yellow);
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isHoldingObject)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1.5f, layerMask))
                {
                    if (isServer)
                    {
                        RpcPickUp(this.gameObject, hit.transform.gameObject);
                    }
                    else
                    {
                        CmdPickUp(this.gameObject, hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if (isServer)
                {
                    RpcPutDown(this.gameObject, carrying);
                }
                else
                {
                    CmdPutDown(this.gameObject, carrying);
                }
            }
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isHoldingObject)
        {
            //Debug.Log("Jump");
            // allow jumping to free oneself from a grab
            if (isBeingHeld)
            {
                if (isServer)
                {
                    RpcSetFree(carrier, this.gameObject);
                    isGrounded = false;
                    GetComponent<Rigidbody>().velocity = transform.forward * 1.0f + Vector3.up * 9.0f;
                }
                else
                {
                    CmdSetFree(carrier, this.gameObject);
                    isGrounded = false;
                    GetComponent<Rigidbody>().velocity = transform.forward * 0.0f + Vector3.up * 9.0f;
                }
            }
            else
            {
                isGrounded = false;     // for good measure
                GetComponent<Rigidbody>().AddForce(jump * 8.0f, ForceMode.Impulse);
            }
            
        }

        // Rotate the world to the right
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!rotationIsAnimating)
            {
                rotationIsAnimating = true;
                targetWorldAngle += 90;
            }
        }

        // Rotate the world to the left
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!rotationIsAnimating)
            {
                rotationIsAnimating = true;
                targetWorldAngle -= 90;
            }
        }

        // Animate camera rotation
        if (rotationIsAnimating)
        {
            if (targetWorldAngle > currentWorldAngle)
            {
                currentWorldAngle += worldRotationSpeed;

                // Change camera position
                GameObject.Find("Main Camera").transform.position = new Vector3(
                     transform.position.x + cameraDistance * Mathf.Sin(Mathf.Deg2Rad * currentWorldAngle),
                     transform.position.y + cameraHeight,
                     transform.position.z + -cameraDistance * Mathf.Cos(Mathf.Deg2Rad * currentWorldAngle));

                // Point the camera towards the player
                GameObject.Find("Main Camera").transform.LookAt(this.transform);
            }
            else if (targetWorldAngle < currentWorldAngle)
            {
                currentWorldAngle -= worldRotationSpeed;

                // Change camera position
                GameObject.Find("Main Camera").transform.position = new Vector3(
                     transform.position.x + cameraDistance * Mathf.Sin(Mathf.Deg2Rad * currentWorldAngle),
                     transform.position.y + cameraHeight,
                     transform.position.z + -cameraDistance * Mathf.Cos(Mathf.Deg2Rad * currentWorldAngle));

                // Point the camera towards the player
                GameObject.Find("Main Camera").transform.LookAt(this.transform);
            }
            else
            {
                rotationIsAnimating = false;
            }
        }
        else
        {
            // Point the camera towards the player
            GameObject.Find("Main Camera").transform.position = new Vector3(
                     transform.position.x + cameraDistance * Mathf.Sin(Mathf.Deg2Rad * currentWorldAngle),
                     transform.position.y + cameraHeight,
                     transform.position.z + -cameraDistance * Mathf.Cos(Mathf.Deg2Rad * currentWorldAngle));
            GameObject.Find("Main Camera").transform.LookAt(this.transform);
        }

        /*// Update camera position
        GameObject.Find("Main Camera").transform.position = new Vector3(
            this.transform.position.x + -cameraDistance * Mathf.Sin(Mathf.Deg2Rad * this.transform.rotation.eulerAngles.y),
            this.transform.position.y +  cameraHeight,
            this.transform.position.z + -cameraDistance * Mathf.Cos(Mathf.Deg2Rad * this.transform.rotation.eulerAngles.y));
           
        // Make camera look at the player after transform
        GameObject.Find("Main Camera").transform.LookAt(this.transform);
        */
    }

}
