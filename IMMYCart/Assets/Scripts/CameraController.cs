using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public CarController target;
    private Vector3 offsetDir;

    public float minDistance, maxDistance;

    private float activeDistance;

    // Start is called before the first frame update
    void Start()
    {
        //set camera to car
        offsetDir = transform.position - target.transform.position;

        //sets initial zoom of camera
        activeDistance = minDistance;

        offsetDir.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        //sets distance of camera from car based on if car is accelarating 
        activeDistance = minDistance + ((maxDistance - minDistance) * (target.theRB.velocity.magnitude / target.maxSpeed));

        //set camera to car
        transform.position = target.transform.position + (offsetDir * activeDistance);
    }
}
