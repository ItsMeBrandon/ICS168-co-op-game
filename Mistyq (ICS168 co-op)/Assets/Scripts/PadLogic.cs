using UnityEngine;
using UnityEngine.Networking;

public class PadLogic : MonoBehaviour {

    public bool isActive;
    public bool remainsActive;

    GameObject innerRing;
    Color colorWhenInactive;

	// Use this for initialization
	void Start () {
        isActive = false;
        innerRing = this.transform.Find("Inner Ring").gameObject;
        colorWhenInactive = innerRing.GetComponent<Renderer>().material.color;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isActive = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (!remainsActive && collision.gameObject.tag == "Player")
        {
            isActive = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
        /*Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up), Color.blue);

        var layerMask = 1 << 8;
        RaycastHit hit;

        // check if button is triggered
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, 1.0f, layerMask))
        {
            Debug.Log("Hit");
            isActive = true;
        }
        else if (!remainsActive && !Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, 1.0f, layerMask))
        {
            Debug.Log("Not hit");
            isActive = false;
        }*/

        // color the button appropriately
        if (isActive)
        {
            innerRing.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            innerRing.GetComponent<Renderer>().material.color = colorWhenInactive;
        }
	}
}
