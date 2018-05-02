using UnityEngine;
using UnityEngine.Networking;

public enum LogicMode {
    AND, OR
}

public class DoorLogic : MonoBehaviour {

    public GameObject activator1;
    public GameObject activator2;
    public LogicMode logicMode;
    public bool canDeactivate;
    public float yDisplacementWhenActive;

    bool isActive;
    bool activator1_isNull;
    bool activator2_isNull;
    bool activator1_isActive;
    bool activator2_isActive;

    Vector3 inactivePosition;
    Vector3 activePosition;

    // Use this for initialization
    void Start () {
        isActive = false;
        activator1_isNull = (activator1 == null);
        activator2_isNull = (activator2 == null);
        activator1_isActive = false;
        activator2_isActive = false;

        inactivePosition = this.transform.position;
        activePosition = inactivePosition + new Vector3(0.0f, yDisplacementWhenActive, 0.0f);
    }
	
	// Update is called once per frame
	void Update () {
        // check if Activator 1 is activated
        if (!activator1_isNull)
        {
            if (activator1.gameObject.tag == "Pad")
                activator1_isActive = activator1.GetComponent<PadLogic>().isActive;
            else if (activator1.gameObject.tag == "Torch")
                activator1_isActive = activator1.GetComponent<TorchLogic>().isActive;
        }
        else
        {
            activator1_isActive = true;
        }

        // check if Activator 2 is activated
        if (!activator2_isNull)
        {
            if (activator2.gameObject.tag == "Pad")
                activator2_isActive = activator2.GetComponent<PadLogic>().isActive;
            else if (activator2.gameObject.tag == "Torch")
                activator2_isActive = activator2.GetComponent<TorchLogic>().isActive;
        }
        else
        {
            activator2_isActive = true;
        }

        // address cases where Activators have/do not have GameObjects associated
        if (activator1_isNull && activator2_isNull)
        {
            isActive = true;
        }
        else if (activator1_isNull)
        {
            if (activator2_isActive)
            {
                isActive = true;
            }
            else if (canDeactivate && !activator2_isActive)
            {
                isActive = false;
            }
        }
        else if (activator2_isNull)
        {
            if (activator1_isActive)
            {
                isActive = true;
            }
            else if (canDeactivate && !activator1_isActive)
            {
                isActive = false;
            }
        }
        else
        {
            if (logicMode == LogicMode.AND)
            {
                if (activator1_isActive && activator2_isActive)
                {
                    isActive = true;
                }
                else if (canDeactivate && !(activator1_isActive && activator2_isActive))
                {
                    isActive = false;
                }
            }
            else if (logicMode == LogicMode.OR)
            {
                if (activator2_isActive || activator2_isActive)
                {
                    isActive = true;
                }
                else if (canDeactivate && !(activator2_isActive || activator2_isActive))
                {
                    isActive = false;
                }
            }
        }

        // animate displacement of wall
        if (isActive)
        {
            if (this.transform.position != activePosition)
            {
                float deltaY;

                deltaY = activePosition.y - this.transform.position.y;
                deltaY = (deltaY / Mathf.Abs(deltaY)) * 0.5f;

                if (deltaY > 0)             // if deltaY is positive
                {
                    if (activePosition.y < this.transform.position.y + deltaY)
                    {
                        this.transform.position = activePosition;
                    }
                    else
                    {
                        this.transform.position += new Vector3(0.0f, deltaY, 0.0f);
                    }
                }
                else if (deltaY < 0)        // if deltaY is negative
                {
                    if (activePosition.y > this.transform.position.y + deltaY)
                    {
                        this.transform.position = activePosition;
                    }
                    else
                    {
                        this.transform.position += new Vector3(0.0f, deltaY, 0.0f);
                    }
                }
            }
        }
        else
        {
            if (this.transform.position != inactivePosition)
            {
                float deltaY;

                deltaY = inactivePosition.y - this.transform.position.y;
                deltaY = (deltaY / Mathf.Abs(deltaY)) * 0.5f;

                if (deltaY > 0)             // if deltaY is positive
                {
                    if (inactivePosition.y < this.transform.position.y + deltaY)
                    {
                        this.transform.position = inactivePosition;
                    }
                    else
                    {
                        this.transform.position += new Vector3(0.0f, deltaY, 0.0f);
                    }
                }
                else if (deltaY < 0)        // if deltaY is negative
                {
                    if (inactivePosition.y > this.transform.position.y + deltaY)
                    {
                        this.transform.position = inactivePosition;
                    }
                    else
                    {
                        this.transform.position += new Vector3(0.0f, deltaY, 0.0f);
                    }
                }
            }
        }
	}
}
