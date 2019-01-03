using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2.0f;
    [Range(1,10)]
    public float jumpVelocity = 5.0f;
    public int maxJumps = 1;
    public float aw_x_error = 1.0f;
    public float aw_z_error = 1.0f;

    private int jumpsLeft;
    private bool autoWalk = false;
    private Vector3 autoWalkPoint;

    Rigidbody rb;

    public new Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        //Constructor
        rb = GetComponent<Rigidbody>();
        jumpsLeft = maxJumps;
    }

    // Update is called once per frame
    void Update()
    {
        //Tick Logic
        GetMovement();
        GetAction();

        //Smooth Fall
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else if (rb.velocity.y > 0 && !Input.GetButton("Jump")){
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void Run()
    {

    }

    void GetMovement()
    {
        // Get the horizontal and vertical axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

        // Make it move 10 meters per second instead of 10 meters per frame...
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        // Move translation along the object's z-axis
        transform.Translate(0, 0, Input.GetButton("Fire3") ? translation * 2 : translation);

        // Rotate around our y-axis
        transform.Rotate(0, rotation, 0);

        if(Input.GetButtonDown("Jump") && jumpsLeft > 0)
        {
            jumpsLeft -= 1;
            rb.velocity = Vector3.up * jumpVelocity;
        }
    }

    void OnCollisionEnter(Collision Col)
    {
        if (Col.gameObject.tag == "Ground")
        {
            jumpsLeft = maxJumps;
        }
    }

    void GetAction()
    {
        //Click interaction
        if(Input.GetButtonDown("Fire1"))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var forward = transform.TransformDirection(Vector3.forward) * 10;
                Debug.DrawRay(transform.position, Vector3.forward * 10, Color.green);
                Transform objectHit = hit.transform;
                Debug.DrawRay(Camera.main.ScreenToWorldPoint(Input.mousePosition), ray.direction * 10000, Color.green, 10, false);
                // Do something with the object that was hit by the raycast.
                autoWalkPoint = hit.point;
                autoWalk = true;
            }
        }
        if(autoWalk)
        {
            //Determing proper rotation
            Vector3 imaginaryPoint = new Vector3(autoWalkPoint.z, 0, transform.position.x);
            float a = Mathf.Abs(imaginaryPoint.z - transform.position.z);
            float b = Mathf.Abs(autoWalkPoint.x - imaginaryPoint.x);
            float c = Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
            float rotationDegree = Mathf.Acos((Mathf.Pow(c, 2) + Mathf.Pow(a, 2) - Mathf.Pow(b, 2)) / (2 * c * a));
            if (rotationDegree > aw_x_error)
            {
                float rotation = rotationDegree * rotationSpeed;

                // Make it move 10 meters per second instead of 10 meters per frame...
                rotation *= Time.deltaTime;

                // Move translation along the object's x-axis
                transform.Rotate(0, rotationDegree, 0);
            }
            if(Mathf.Abs(autoWalkPoint.z - transform.position.z) > aw_z_error)
            {
                float direcitonOfMovementX = ((autoWalkPoint.z - transform.position.z) / Mathf.Abs(autoWalkPoint.z - transform.position.z));

                float translation = direcitonOfMovementX * speed;

                // Make it move 10 meters per second instead of 10 meters per frame...
                translation *= Time.deltaTime;

                // Move translation along the object's z-axis
                transform.Translate(0, 0, Input.GetButton("Fire3") ? translation * 2 : translation);
            }
            if(Mathf.Abs(autoWalkPoint.x - transform.position.x) < aw_x_error && Mathf.Abs(autoWalkPoint.z - transform.position.z) < aw_z_error)
            {
                autoWalk = false;
            }
        }
    }
}
