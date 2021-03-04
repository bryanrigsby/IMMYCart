using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{

    public GameObject greeter;
    public GameObject nurse;
    public GameObject checkInSuccessText;
    public GameObject swapSuccessText;


    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Checkin")
        {
            greeter.transform.position = new Vector3(greeter.transform.position.x + 4f, greeter.transform.position.y, greeter.transform.position.z);


        }

        if (other.tag == "CheckInSuccess")
        {
            checkInSuccessText.SetActive(true);
        }

        if (other.tag == "swab")
        {
            nurse.transform.position = new Vector3(nurse.transform.position.x - 4f, nurse.transform.position.y, nurse.transform.position.z);
        }

        if (other.tag == "SwabSuccess")
        {
            swapSuccessText.SetActive(true);
        }
    }


}
