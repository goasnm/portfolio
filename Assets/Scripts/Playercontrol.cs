using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playercontrol : MonoBehaviour
{
    public Rigidbody PlayerRig;
    public float maxSteerAngle = 0f;
    public float SteerAngle = 0f;
    public float maxMotorTorque = 0f;
    public float maxBrakeTorque = 0f;
    public float currentSpeed;
    public float maxSpeed = 0f;
    public float maxBackSpeed = 0f;
    public float moveSpeed;
    public float isBraking;
    public Vector3 centerOfMass;
    public Vector3 PlayerPos;
    public bool isbrak;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;
    public CountDown countDown;
    public CountDown LapTime;

    [Header("MiniMap")]
    public GameObject minimap;
    public Transform minimapCam;

    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Rigidbody>().centerOfMass = centerOfMass;

        PlayerRig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //PlayerControl();
        //Brak();

        if (countDown.countDownTime <= 0)
        {
            PlayerMove();
        }

        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }

    private void PlayerControl()
    {
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;

        if (currentSpeed < maxSpeed && !isbrak)
        {
            wheelFL.motorTorque = maxMotorTorque;
            wheelFR.motorTorque = maxMotorTorque;
        }
        else
        {
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
        }
    }

    private void Brak()
    {
        if (isbrak)
        {
            wheelRL.brakeTorque = maxBrakeTorque;
            wheelRR.brakeTorque = maxBrakeTorque;
        }
        else
        {
            wheelRL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
        }
    }

    private void PlayerMove()
    {
        minimap.SetActive(true);

        countDown.curSpeedText.text = string.Format("{0:000.00}", currentSpeed);

        //bool inputkey = false;
        PlayerPos = this.transform.position;

        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;

        if (currentSpeed < maxSpeed && !isbrak)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                //inputkey = true;
                wheelFL.motorTorque = maxMotorTorque;
                wheelFR.motorTorque = maxMotorTorque;
            }

            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
            {
                //inputkey = false;
                //wheelFL.motorTorque = maxMotorTorque - (maxMotorTorque + 10);
                //wheelFR.motorTorque = maxMotorTorque - (maxMotorTorque + 10);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                //inputkey = true;
                wheelFL.motorTorque = -maxMotorTorque * 2;
                wheelFR.motorTorque = -maxMotorTorque * 2;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                wheelFL.steerAngle = -SteerAngle;
                wheelFR.steerAngle = -SteerAngle;
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                wheelFL.steerAngle = 0;
                wheelFR.steerAngle = 0;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                wheelFL.steerAngle = SteerAngle;
                wheelFR.steerAngle = SteerAngle;
            }

            if(Input.GetKeyUp(KeyCode.D))
            {
                wheelFL.steerAngle = 0;
                wheelFR.steerAngle = 0;
            }

            minimapCam.position = PlayerPos + new Vector3(0, 30, 0);
        }


        // 현재속도가 0 보다 크면 감소 0 보다 작으면 증가?
        //if(!inputkey)
        //{
        //    if (currentSpeed > 0)
        //    {
        //        wheelFL.motorTorque --;
        //        wheelFR.motorTorque --;
        //    }
        //    else if (currentSpeed < 0)
        //    {
        //        wheelFL.motorTorque++;
        //        wheelFR.motorTorque++;
        //    }
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Finish")
        {
            if (countDown.check)
            {
                countDown.check = false;
                if (countDown.lap > 0)
                {
                    countDown.LapTime();
                }

                countDown.lap += 1;
            }
        }
        if (other.gameObject.tag == "GoalCheck")
        {
            countDown.check = true;
        }
    }
}
