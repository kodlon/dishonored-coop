using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    public LayerMask groundMask;

    [SerializeField] private float speed = 12f;
    [SerializeField] private float sprintSpeed = 20f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float climbDistant = 3f; // Висота на яку може влізти гг
    [SerializeField] private float groundDistance = 1f;

    private Vector3 velocity;
    private bool isGrounded;
    private enum states { lowSquats, squats, stand};

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Debug.Log("Size: " + GetComponent<Collider>().bounds.size);
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position + new Vector3(0f, -1.829f, 0f), groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //if (Input.GetKeyDown("e"))
        //{
        //    Debug.Log(transform.forward);
        //}

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(Input.GetButton("Sprint") && isGrounded ? move * sprintSpeed * Time.deltaTime : move * speed * Time.deltaTime);

        // if (Input.GetButton("Sprint")) { controller.Move(move * sprintSpeed * Time.deltaTime); }
        // else { controller.Move(move * speed * Time.deltaTime); }
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else if (Input.GetButtonDown("Jump"))
        {
            //Climb();
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown("e"))
        {
            //Debug.Log("transform: " + transform.position + ", forward: " + transform.TransformDirection(Vector3.forward));
        }
    }
    private void FixedUpdate()
    { 
        Climb();
    }

    private void Climb()
    {
        MyRaycast(transform.position + new Vector3(0f, 1.36f, 0f), transform.TransformDirection(Vector3.forward), groundDistance, 1<<7);
        MyRaycast(transform.position, transform.TransformDirection(Vector3.forward), groundDistance, 1<<7);
        MyRaycast(transform.position + new Vector3(0f, -1.36f, 0f), transform.TransformDirection(Vector3.forward), groundDistance, 1<<7);
        RaycastHit hitHead;
        RaycastHit hitTorso;
        RaycastHit hitFeet;
        //Physics.Raycast(transform.position + new Vector3(0f, 1.36f, 0f), transform.TransformDirection(Vector3.forward), out hitHead, groundDistance, 1 << 7);
        //Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitTorso, groundDistance, 1 << 7);
        //Physics.Raycast(transform.position + new Vector3(0f, -1.36f, 0f), transform.TransformDirection(Vector3.forward), out hitFeet, groundDistance, 1 << 7);

        if (Physics.Raycast(transform.position + new Vector3(0f, 1.36f, 0f), transform.TransformDirection(Vector3.forward), out hitHead, groundDistance, 1 << 7))
        {
            float state;
            Vector3 climbPoint = StartClimb2(hitHead.point, out state);
            //Debug.Log(hitHead.collider.bounds.size.y);
            Debug.Log("State head: " + state);
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitTorso, groundDistance, 1 << 7))
        {
            float state;
            Vector3 climbPoint = StartClimb2(hitTorso.point, out state);
            //Debug.Log(hitTorso.collider.bounds.size.y);
            Debug.Log("State torso: " + state);
        }
        else if (Physics.Raycast(transform.position + new Vector3(0f, -1.36f, 0f), transform.TransformDirection(Vector3.forward), out hitFeet, groundDistance, 1 << 7))
        {
            float state;
            Vector3 climbPoint = StartClimb2(hitFeet.point, out state);
            //Debug.Log(hitFeet.collider.bounds.size.y);
            Debug.Log("State feet: " + state);
        }
    }

    private Vector3 StartClimb2(Vector3 startPoint, out float state)
    {
        Vector3 newStartPoint = startPoint;
        float numberOfCheckRays = 15.0f;
        float checkRayLength = climbDistant / numberOfCheckRays;
        state = -1;
        for (int i = 0; i < (int)numberOfCheckRays; i++)
        {
            Debug.Log("Check Ray Length: " + checkRayLength);
            Debug.DrawRay(newStartPoint, new Vector3(0f, checkRayLength, 0f), Color.yellow, Mathf.Infinity);
            if (!Physics.Raycast(newStartPoint, new Vector3(0f, checkRayLength, 0f), checkRayLength, 1 << 7))
            {
                for (int j = 0; j < 3; j++)
                {
                    state = 0;
                    if (!Physics.Raycast(newStartPoint, transform.TransformDirection(Vector3.up), 3.8f / 3.0f * (float)j, 1 << 7))
                    {
                        state = j;
                    }
                }
                if (state != -1) { return newStartPoint; }
            }
            newStartPoint += new Vector3(0f, checkRayLength, 0f);
            Debug.Log("New Start Point: " + newStartPoint);
            Debug.Log("i: " + i);
        }
        return new Vector3(-1f, -1f, -1f);
    }
    
    // Малює промені з перевіркою на вдаряння в об'єкт (тільки малює)
    private void MyRaycast(Vector3 start, Vector3 direction, float distance, int layerMask)
    {
        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, distance, layerMask))
        {
            Debug.DrawRay(start, direction * hit.distance, Color.yellow);
            if (Input.GetKeyDown("q"))
            {
                Debug.Log("Did Hit");
            }
        }
        else
        {
            Debug.DrawRay(start, direction * distance, Color.red);
            if (Input.GetKeyDown("q"))
            {
                Debug.Log("Did not Hit");
            }
        }
    }

    //private void StartClimb(float height, RaycastHit hit, float climbHeight)
    //{
    //    /* // Ця логіа застаріла, пробую нову
    //     Тута логіка така:
    //     Береться висота точки з якої путили промінь (float height),
    //     висота предмета в який та точка вдарилася (обчислюється з hit)
    //     і різниця цих висот (тобто скільки до вершини залишилося), порівнюється
    //     з висотою на яку може залізти перс (climbHeight). Якщо різниця менша за 
    //     максимально допустиму висоту перс залазить, якщо ж ні - то не залазить.
    //     */
    //    //if (height)
    //    float hitHeight = hit.transform.position.y - height;
    //}
}
