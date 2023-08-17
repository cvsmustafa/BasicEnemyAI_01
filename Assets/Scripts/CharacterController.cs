using UnityEngine;



public class CharacterController : MonoBehaviour
{

    public float speed = 10.0f;

    public float jumpForce = 250.0f;

    private Rigidbody rb;



    void Start()
    {

        rb = GetComponent<Rigidbody>();

    }



    void FixedUpdate()
    {

        float moveHorizontal = Input.GetAxis("Horizontal");

        float moveVertical = Input.GetAxis("Vertical");



        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);



        rb.AddForce(movement * speed);



        if (Input.GetKeyDown(KeyCode.Space))
        {

            rb.AddForce(Vector3.up * jumpForce);

        }

    }

}