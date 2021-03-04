using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody theRB;

    public float maxSpeed;
    
    public float forwardAccel = 8f, reverseAccel = 4f;
    private float speedInput;

    public float turnStrength = 180f;
    private float turnInput;

    private bool grounded;

    public Transform groundRayPoint, groundRayPoint2;
    public LayerMask whatIsGround;
    public float groundRayLength = 0.75f;

    private float dragOnGround;
    public float gravityMod = 10f;

    public Transform leftFrontWheel, rightFrontWheel;
    public float maxWheelTurn = 25f;

    public ParticleSystem[] dustTrail;
    public float maxEmission = 25f, emissionFadeSpeed = 20f;
    private float emissionRate;

    public AudioSource engineSound, tireSqueal;
    public float skidFadeSpeed;


    // Start is called before the first frame update
    void Start()
    {
        // makes sphere not child of car so that movement will be smooth
        theRB.transform.parent = null;

        dragOnGround = theRB.drag;
    }

    // Update is called once per frame
    void Update()
    {
        speedInput = 0f;

        //controls forward and backward movement
        if (Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * forwardAccel;
        }
        else if(Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverseAccel;
        }

        //controls sideways movement
        turnInput = Input.GetAxis("Horizontal");
        //if(grounded && Input.GetAxis("Vertical") != 0) -- moved to FixedUpdate
        //{
        //    //sets rotation based on turnInput, turnStrength, frame rate, reverse direction (Mathf.Sign makes reverse turn opposite, which is correct), turnSpeed (makes it so turns are bigger the faster the car is going)
        //    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude / maxSpeed), 0f));
        //}

        //turning wheels - localRotation takes the rotation of that specific object(the wheel), the left already has a rotation of 180
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);

        //transform.position = theRB.position;  --moved to FixedUpdate()

        //control particle emission (dust from tires)
            //fades out dust
        emissionRate = Mathf.MoveTowards(emissionRate, 0f, emissionFadeSpeed * Time.deltaTime);

        // car is grounded, wheels are turning, car is no sitting still and is taking off
        if (grounded && (Mathf.Abs(turnInput) > .5f || (theRB.velocity.magnitude < maxSpeed * .5f && theRB.velocity.magnitude != 0)))
        {
            emissionRate = maxEmission;
        }

        // stops dust when car stops
        if(theRB.velocity.magnitude <= .5f)
        {
            emissionRate = 0;
        }

        // sets rateOverTime of dust to emission rate for all four wheels
        for(int i = 0; i < dustTrail.Length; i++)
        {
            var emissionModule = dustTrail[i].emission;
            emissionModule.rateOverTime = emissionRate;
        }

        //control engine sound to increase based on speed
        if(engineSound != null)
        {
            engineSound.pitch = 1f + ((theRB.velocity.magnitude / maxSpeed) * 2f);
        }

        if(tireSqueal != null)
        {
            if(Mathf.Abs(turnInput) > 0.5f && grounded)
            {
                tireSqueal.volume = 1f;
            }
            else
            {
                tireSqueal.volume = Mathf.MoveTowards(tireSqueal.volume, 0f, skidFadeSpeed * Time.deltaTime);
            }
        }
    }

    private void FixedUpdate()
    {
        grounded = false;

        RaycastHit hit;
        Vector3 normalTarget = Vector3.zero;


        //shots ray from groundRayPoint down groundRayLength to the ground and returns hit
        if(Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            normalTarget = hit.normal;
        }

        //handles "snapping" on ramp
        if (Physics.Raycast(groundRayPoint2.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            normalTarget = (normalTarget + hit.normal) / 2;
        }

        //when on ground rotate to match the normal (this is used to help keep car horizontally in line when on ramps and such)
        if (grounded)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation;
        }

        //accelerates the car
        if (grounded)
        {
            theRB.drag = dragOnGround;
            theRB.AddForce(transform.forward * speedInput * 1000f);
        }
        else
        {//if not on ground, less drag and more gravity to come down more quickly
            theRB.drag = .1f;
            theRB.AddForce(-Vector3.up * gravityMod * 100f);
        }
        

        //makes sure car doesnt go aboove maxSpeed
        if(theRB.velocity.magnitude > maxSpeed)
        {
            theRB.velocity = theRB.velocity.normalized;
        }


        transform.position = theRB.position;

        if (grounded && Input.GetAxis("Vertical") != 0)
        {
            //sets rotation based on turnInput, turnStrength, frame rate, reverse direction (Mathf.Sign makes reverse turn opposite, which is correct), turnSpeed (makes it so turns are bigger the faster the car is going)
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude / maxSpeed), 0f));
            

        }
    }
}
