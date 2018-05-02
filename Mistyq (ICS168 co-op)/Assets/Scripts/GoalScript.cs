using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum LevelNumber {
    _1, _2, _3, _4
}

public class GoalScript : MonoBehaviour {

    public LevelNumber goalForLevel;
    public Vector3 teleportFirstArrivingPlayerTo;
    public Vector3 teleportSecondArrivingPlayerTo;

    bool levelCompleted;
    int playersNeeded;
    List<GameObject> players = new List<GameObject>();

    // Use this for initialization
    void Start() {
        levelCompleted = false;
        playersNeeded = 2;
    }


	void SetSpawn(int which)
	{
		//Debug.Log("Setting spawn to... position " + which + "!");
		if (which == 1)			// Tutorial level
		{
			GameObject.FindGameObjectWithTag("Spawn1").transform.position = new Vector3(-6.0f, 5.0f, 2.0f);
			GameObject.FindGameObjectWithTag("Spawn2").transform.position = new Vector3(6.0f, 5.0f, 2.0f);
		}
		else if (which == 2)	// Level 1
		{
			GameObject.FindGameObjectWithTag("Spawn1").transform.position = new Vector3(-242.0f, 16.0f, 32.0f);
			GameObject.FindGameObjectWithTag("Spawn2").transform.position = new Vector3(246.0f, 16.0f, 32.0f);
		}
		else if (which == 3)	// Level 2
		{
			GameObject.FindGameObjectWithTag("Spawn1").transform.position = new Vector3(402.0f, 16.0f, 12.0f);
			GameObject.FindGameObjectWithTag("Spawn2").transform.position = new Vector3(408.0f, 16.0f, 12.0f);
		}
		else if (which == 4)	// Level 3
		{
            GameObject.FindGameObjectWithTag("Spawn1").transform.position = new Vector3(502.0f, 12.2f, 12.0f);
            GameObject.FindGameObjectWithTag("Spawn2").transform.position = new Vector3(502.0f, 12.2f, 16.0f);

            GameObject.FindGameObjectWithTag("Win Text").gameObject.GetComponent<Text>().text = "Congratulations! You win!";
		}

	}


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
			players.Add(collision.gameObject);
			playersNeeded -= 1;

            if (playersNeeded == 0)
            {
				SetSpawn((int)goalForLevel + 2);

                players[0].transform.position = teleportFirstArrivingPlayerTo;
                players[1].transform.position = teleportSecondArrivingPlayerTo;

                players.RemoveAt(0);
                players.RemoveAt(1);
            }
        }
        
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playersNeeded += 1;

            if (playersNeeded == 2)
            { players.RemoveAt(0); }
        }
    }
	
	// Update is called once per frame
	void Update () {

    }
}
