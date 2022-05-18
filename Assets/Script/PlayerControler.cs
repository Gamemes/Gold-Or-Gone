using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    // Start is called before the first frame update]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2.0f;
    [Range(1, 100)]
    public float jumpSpeed = 5f;
    public float walkSpeed = 5f;
    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 speed = new Vector2();
        speed.x = Input.GetAxis("Horizontal") * walkSpeed;
        speed.y = rb.velocity.y;
        if (Input.GetButtonDown("Jump"))
        {
            speed.y = jumpSpeed;
        }
        rb.velocity = speed;
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
