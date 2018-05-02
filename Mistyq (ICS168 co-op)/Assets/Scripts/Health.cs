using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

	private NetworkStartPosition[] spawnPoints;

	public const int maxHealth = 100;
	[SyncVar(hook = "OnChangeHealth")]
	public int currentHealth = maxHealth;
	public RectTransform healthbar;
	public bool destroyOnDeath;

    public float depthUntilRespawn;

	void Start()
	{
		if (isLocalPlayer)
		{ spawnPoints = FindObjectsOfType<NetworkStartPosition>(); }
	}


	void OnChangeHealth(int health)
	{ healthbar.sizeDelta = new Vector2(health, healthbar.sizeDelta.y); }


	public void TakeDamage(int amount)
	{
		if (!isServer)
		{ return; }
		currentHealth -= amount;
		if (currentHealth <= 0)
		{
			if (destroyOnDeath)
			{ Destroy(gameObject); }
			else
			{
				currentHealth = maxHealth;
				RpcRespawn();
			}
		}
	}


	[ClientRpc]
	void RpcRespawn()
	{
		if (isLocalPlayer)
		{
			//transform.position = Vector3.zero;
			
			Vector3 spawnPoint = Vector3.zero;

			if (spawnPoints != null && spawnPoints.Length > 0)
			{
				spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
			}
			
			transform.position = spawnPoint;
		}
	}

    void Update() {

        // Respawn if below a certain depth
        if (this.transform.position.y <= -depthUntilRespawn)
        {
            Vector3 spawnPoint = Vector3.zero;

            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            transform.position = spawnPoint;
        }
    }

}
