using UnityEngine;
using UnityEngine.Networking;

public enum ElementMode {
    Any, Water, Fire
}

enum ElementActive
{
    None, Fire, Water
}

public class TorchLogic : MonoBehaviour
{
    public bool isActive;
    public ElementMode elementRequired;

    ElementActive currentElement;
    Material fireMaterial;
    Material waterMaterial;

    // Use this for initialization
    void Start()
    {
        isActive = false;
        currentElement = ElementActive.None;
        fireMaterial = Resources.Load("Materials/Fire", typeof(Material)) as Material;
        waterMaterial = Resources.Load("Materials/Water", typeof(Material)) as Material;

        this.transform.Find("Element").gameObject.SetActive(false);
    }

    // Change element of the torch when it is interacted with
    void OnCollisionEnter(Collision collision)
    {
        /*if (collision.gameObject.tag == "Bullet")
        {
           isActive = true;
        }*/

        if (collision.gameObject.tag == "Fire")
        {
            if (currentElement == ElementActive.None && (elementRequired == ElementMode.Any || elementRequired == ElementMode.Fire))
            {
                currentElement = ElementActive.Fire;
            }
            else if (currentElement == ElementActive.Water)
            {
                currentElement = ElementActive.None;
            }
        }
        else if (collision.gameObject.tag == "Water")
        {
            if (currentElement == ElementActive.None && (elementRequired == ElementMode.Any || elementRequired == ElementMode.Water))
            {
                currentElement = ElementActive.Water;
            }
            else if (currentElement == ElementActive.Fire)
            {
                currentElement = ElementActive.None;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // check logic to determine if torch is active
        if (elementRequired == ElementMode.Any)
        {
            if (currentElement == ElementActive.Fire || currentElement == ElementActive.Water)
            {
                isActive = true;
            }
            else if (currentElement == ElementActive.None)
            {
                isActive = false;
            }
        }
        else if (elementRequired == ElementMode.Fire)
        {
            if (currentElement == ElementActive.Fire)
            {
                isActive = true;
            }
            else if (currentElement == ElementActive.Water || currentElement == ElementActive.None)
            {
                isActive = false;
            }
        }
        else if (elementRequired == ElementMode.Water)
        {
            if (currentElement == ElementActive.Water)
            {
                isActive = true;
            }
            else if (currentElement == ElementActive.Fire || currentElement == ElementActive.None)
            {
                isActive = false;
            }
        }
        
        // activate particle system if active
        if (isActive)
        {
            if (currentElement == ElementActive.Fire)
            {
                this.transform.Find("Element").gameObject.GetComponent<Renderer>().material = fireMaterial;
                this.transform.Find("Element").gameObject.SetActive(true);
            }
            else if (currentElement == ElementActive.Water)
            {
                this.transform.Find("Element").gameObject.GetComponent<Renderer>().material = waterMaterial;
                this.transform.Find("Element").gameObject.SetActive(true);
            }
        }
        else
        {
            this.transform.Find("Element").gameObject.SetActive(false);
        }
    }
}
