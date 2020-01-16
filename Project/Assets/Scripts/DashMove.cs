using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashMove : MonoBehaviour
{
    private Rigidbody2D rb;
    public float dashSpeed;
    private float dashTime;
    public float startDashTime;
    private int direction;

    public GameObject dashEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dashTime = startDashTime;



    }
    void Update()
    {
        if (direction == 0)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKey(KeyCode.LeftArrow) && Input.GetKeyDown(KeyCode.LeftShift))
            {
                //Instantiate(dashEffect, transform.position, Quaternion.identity);
                direction = 1;
            }
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKey(KeyCode.RightArrow) && Input.GetKeyDown(KeyCode.LeftShift))
            {
                //Instantiate(dashEffect, transform.position, Quaternion.identity);
                direction = 2;
            }
        }
        else
        {
            if (dashTime <= 0)
            {
                direction = 0;
                dashTime = startDashTime;
                rb.velocity = Vector2.zero;
            }
            else
            {
                dashTime -= Time.deltaTime;

                if (direction == 1)
                {
                    rb.velocity = Vector2.left * dashSpeed;
                }
                else if (direction == 2)
                {
                    rb.velocity = Vector2.right * dashSpeed;
                }
            }

        }

    }

}
