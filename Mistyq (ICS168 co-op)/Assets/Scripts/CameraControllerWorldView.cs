using UnityEngine;

public class CameraControllerWorldView : MonoBehaviour {
    
    // Set the height and distance of the camera from the origin
    public float cameraDistanceFromOrigin;
    public float cameraHeightFromOrigin;

    // Set world rotation speed
    public float worldRotationSpeed;


	public void LookAtOrigin()
	{
        this.transform.LookAt(Vector3.zero);
	}

	public void LookUp()
	{
		this.transform.rotation = Quaternion.Euler(new Vector3(-70.0f, 0.0f, 0.0f));
	}


    void Start()
    {
        // Position the camera accordingly
        this.transform.position = new Vector3(0.0f, cameraHeightFromOrigin, -cameraDistanceFromOrigin);
        
        // Point the camera towards the origin
    }

}
