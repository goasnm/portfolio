using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMove : MonoBehaviour
{

    public Transform Path;
    public float maxSteerAngle = 45f;
    public float turnSpeed = 5f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;
    public float maxMotorTorque = 80f;
    public float maxBrakeTorque = 200;
    public float currentSpeed;
    public float maxSpeed = 150;
    public Vector3 centerOfMass;
    public bool isBraking;

    public CountDown countDown;

    [Header("Sensors")]
    public float sensorLength = 3f;
    public Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0.5f);
    public float frontSideSensorPosition = 0.2f;
    public float frontSensorAngle = 30;

    private List<Transform> nodes;
    private int currectNode = 0;
    private bool avoiding = false;
    private float targetSteerAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;

        Transform[] pathTransfrom = Path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransfrom.Length; i++)
        {
            if (pathTransfrom[i] != Path.transform)
            {
                nodes.Add(pathTransfrom[i]);
            }
        }

    }

    void Update()
    {
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (countDown.countDownTime <= 0)
        {
            Sensors();
            ApplySteer();
            Move();
            CheckWayPint();
            Braking();
            LerpToSteerAngle();
        }
    }

    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
        float avoidMultiplier = 0;
        avoiding = false;

        //front rigt sensor
        sensorStartPos += transform.right * frontSideSensorPosition;

        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier -= 1f;
            }

        }

        //front rigt angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier -= 0.5f;
            }

        }

        //front left sensor
        sensorStartPos -= transform.right * frontSideSensorPosition * 2;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier += 1f;
            }

        }

        //front left angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier += 0.5f;
            }
        }

        //front Center sensor
        if (avoidMultiplier == 0)
        {
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
            {
                if (hit.collider.CompareTag("Terrain"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    avoiding = true;
                    if (hit.normal.x < 0)
                    {
                        avoidMultiplier = -1;
                    }
                    else
                    {
                        avoidMultiplier = 1;
                    }
                }
            }
        }

        if (avoiding)
        {
            targetSteerAngle = maxSteerAngle * avoidMultiplier;
        }
    }

    private void ApplySteer()
    {
        if (avoiding) return;
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currectNode].position);
       
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        targetSteerAngle = newSteer;
    }

    private void Move()
    {
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;

        if (currentSpeed < maxSpeed && !isBraking)
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
    private void CheckWayPint()
    {
        if (Vector3.Distance(transform.position, nodes[currectNode].position) < 5.0f)
        {
            if (currectNode == nodes.Count - 1)
            {
                currectNode = 0;
            }
            else currectNode++;
        }
    }

    private void Braking()
    {
        if (isBraking)
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

    private void LerpToSteerAngle()
    {
        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }
}
