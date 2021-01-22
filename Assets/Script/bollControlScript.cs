using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Rigidbody))]
public class bollControlScript : MonoBehaviour
{
    //this is the force of the throw
    public float m_Throwforce = 100f;

    //X and Y axis damping factors for the throw  direction 
    public float m_ThrowDirectionX = 0.17f;
    public float m_ThrowDirectionY = 0.67f;

    //offset of the ball position in relation to camera's position 
    public Vector3 m_BallCameraOffset = new Vector3(0f, -0.4f, 1f);

    //The following variables contain the state of the current throw 
    private Vector3 StartPosition;
    private Vector3 direction;
    private float StartTime;
    private float endTime;
    private float duration;
    private bool directionChosen = false;
    private bool throwStarted = false;

    [SerializeField]
    GameObject ARCam;

    [SerializeField]
    ARSessionOrigin m_SessionOrigin;

    Rigidbody rb;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        m_SessionOrigin = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
        ARCam = m_SessionOrigin.transform.Find("AR Camera").gameObject;
        transform.parent = ARCam.transform;
        ResetBall();
    }

    public void Update()
    {
        //we will start the touch of the screen,which will start the ball throw
        if (Input.GetMouseButtonDown(0)) // run for mouse click as well as mobile touch 
        {
            StartPosition = Input.mousePosition;
            StartTime = Time.time;
            throwStarted = true;
            directionChosen = false;
        }
        // we have ended the touch of the screen, which will throw the ball 
        else if (Input.GetMouseButtonUp(0)) 
        {
            endTime = Time.time;
            duration = endTime - StartTime;
            direction = Input.mousePosition - StartPosition;
            directionChosen = true;
        }

        if(directionChosen)
        {
            rb.mass = 1;
            rb.useGravity = true;

            rb.AddForce(
                ARCam.transform.forward * m_Throwforce / duration +
                ARCam.transform.up * direction.y * m_ThrowDirectionY +
                ARCam.transform.right * direction.x * m_ThrowDirectionX);

            StartTime = 0.0f;
            duration = 0.0f;

            StartPosition = new Vector3(0, 0, 0);
            direction = new Vector3(0, 0, 0);

            throwStarted = false;
            directionChosen = false;
        }

        //5 sec after throwing the ball we reset its location 
        if (Time.time - endTime >= 5 && Time.time - endTime<=6)
            ResetBall();
    }
    public void ResetBall()
    {
        rb.mass = 0;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        endTime = 0.0f;

        Vector3 ballPos = ARCam.transform.position + ARCam.transform.forward * m_BallCameraOffset.z
            + ARCam.transform.up * m_BallCameraOffset.y;
        transform.position = ballPos;
    }
}




