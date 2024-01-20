using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using SimpleInputNamespace;

public class VehicleMovement : MonoBehaviour
{
    public Transform fL_Mesh;
    public Transform fR_Mesh;
    public Transform rL_Mesh;
    public Transform rR_Mesh;

    public WheelCollider fL_coll;
    public WheelCollider fR_coll;
    public WheelCollider rL_coll;
    public WheelCollider rR_coll;

    public float acc;
    public float brake;
    public float turnAngle;

    float curr_acc, curr_brake, curr_turnAngle;

    public Rigidbody rb;
    public Transform cog;

    public GameObject gate_Col;
    public GameObject start_Gate;
    public GameObject finish_Gate;
    public GameObject restrict;
    public GameObject goText;
    public AudioSource vehicleAccell;
    public AudioSource brakeAudio;

    public float racetime;
    public TMP_Text timer_Text;
    bool raceStart = false;
    public GameObject looseScreen;
    public GameObject wonScreen;
    public int currLevel;

    float gas;
    float cBrake;


    void Start()
    {
        rb.centerOfMass = cog.localPosition;
        rb.isKinematic = false;

        Time.timeScale = 1;
    }

    private void FixedUpdate()
    {
        curr_acc = gas * acc;
        rL_coll.motorTorque = curr_acc;
        rR_coll.motorTorque = curr_acc;

        if (cBrake == 1)
        {
            curr_brake = brake * cBrake;
        }
        else
        {
            curr_brake = 0;
        }


        fL_coll.brakeTorque = curr_brake;
        fR_coll.brakeTorque = curr_brake;
        rL_coll.brakeTorque = curr_brake;
        rR_coll.brakeTorque = curr_brake;

        curr_turnAngle = SimpleInput.GetAxis("Horizontal") * turnAngle;
        fL_coll.steerAngle = curr_turnAngle;
        fR_coll.steerAngle = curr_turnAngle;

        WheelMeshUpdate(fL_coll, fL_Mesh);
        WheelMeshUpdate(fR_coll, fR_Mesh);
        WheelMeshUpdate(rL_coll, rL_Mesh);
        WheelMeshUpdate(rR_coll, rR_Mesh);

    }

    private void Update()
    {
        if(raceStart == true)
        {
            racetime -= Time.deltaTime;
            int Timer = (int)racetime;
            timer_Text.text = "Timer : " + Timer.ToString();
            if (racetime <= 0)
            {
                Time.timeScale = 0;
            }
        }
        /*if (Input.GetKey(KeyCode.W))
        {
            tempoAccell.Play();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            brakeAudio.Play();
        }*/

        if (racetime < 1)
        {
            Destroy(timer_Text);
            Destroy(goText);
            looseScreen.gameObject.SetActive(true);
            Time.timeScale = 0;
        }

    }

    public void WheelMeshUpdate(WheelCollider coll, Transform trans)
    {
        Vector3 pos;
        Quaternion rot;

        coll.GetWorldPose(out pos, out rot);
        trans.position = pos;
        trans.rotation = rot;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Gate Collider")
        {
            raceStart = true;
            StartCoroutine(Go_Text());
            start_Gate.gameObject.SetActive(false);
            finish_Gate.gameObject.SetActive(false);
        }
        
        if(other.gameObject.tag == "FinishCol")
        {
            finish_Gate.gameObject.SetActive(true);
            restrict.gameObject.SetActive(false);
        }
        
        if(other.gameObject.tag == "Gate Collider" && racetime<30)
        {
            Destroy(timer_Text);
            racetime = 60;
            Time.timeScale = 0;
            Destroy(goText);
            if (currLevel == 3)
            {
                SceneManager.LoadScene(currLevel + 1);
            }
            else
            {
                wonScreen.gameObject.SetActive(true);
            }
        }
    }

    IEnumerator Go_Text()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            goText.SetActive(true);
            yield return new WaitForSeconds(1f);
            goText.SetActive(false);
        }
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(currLevel + 1);
        Time.timeScale = 1;
    }

    public void AccPress()
    {
        gas = 1;
        vehicleAccell.Play();
    }
    
    public void RevPress()
    {
        gas = -1;
        vehicleAccell.Play();
    }
    
    public void AccRelease()
    {
        gas = 0;
    }

    public void brakePressed()
    {
        cBrake = 1;
        brakeAudio.Play();
    }
    
    public void brakeReleased()
    {
        cBrake = 0;
    }

}
