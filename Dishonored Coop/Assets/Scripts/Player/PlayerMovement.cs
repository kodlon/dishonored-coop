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
    [SerializeField] private float climbDistant = 3f;
    [SerializeField] private float groundDistance = 0.4f;

    private Vector3 velocity;
    private bool isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
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
            Debug.Log("transform: " + transform.position + ", forward: " + transform.TransformDirection(Vector3.forward));
        }
    }
    private void FixedUpdate()
    { 
        Climb();
    }

    private void Climb()
    {
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, 1<<7))
        //{
        //    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        //    if (Input.GetKeyDown("q"))
        //    {
        //        Debug.Log("Did Hit");
        //    }
        //}
        //else
        //{
        //    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.red);
        //    if (Input.GetKeyDown("q"))
        //    {
        //        Debug.Log("Did not Hit");
        //    }
        //}
        MyRaycast(transform.position + new Vector3(0f, 1.36f, 0f), transform.TransformDirection(Vector3.forward), Mathf.Infinity, 1<<7);
        MyRaycast(transform.position, transform.TransformDirection(Vector3.forward), Mathf.Infinity, 1<<7);
        MyRaycast(transform.position + new Vector3(0f, -1.36f, 0f), transform.TransformDirection(Vector3.forward), Mathf.Infinity, 1<<7);
    }

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
            Debug.DrawRay(start, direction * 1000, Color.red);
            if (Input.GetKeyDown("q"))
            {
                Debug.Log("Did not Hit");
            }
        }
    }
}
