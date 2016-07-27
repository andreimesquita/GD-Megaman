using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class ShootController : MonoBehaviour
{
    private new Rigidbody2D rigidbody2D;

    private void Awake()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
    }

    public void AddVelocity(float vel)
    {
        rigidbody2D.velocity = new Vector2(vel, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.transform.CompareTag("enemy"))
        {
            Destroy(this);
        }

        //if (collision2D.transform.CompareTag("shoot"))
        //{
        //    rigidbody2D.velocity = collision2D.transform.up * Math.Abs(rigidbody2D.velocity.x);
        //}

        //Debug.Log("OnCollisionEnter: " + rigidbody2D.velocity);
    }
}
