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

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.transform.CompareTag("enemy"))
        {
            Destroy(this.gameObject);
        } else if (collider2D.transform.CompareTag("shoot") && 
                   rigidbody2D.velocity.x != collider2D.GetComponent<Rigidbody2D>().velocity.x)
        {
            // Inverte a velocidade em X e move para cima
            rigidbody2D.velocity = new Vector3(-rigidbody2D.velocity.x, Mathf.Abs(rigidbody2D.velocity.x));

            // Destroi este GameObject após 4 segundos
            Destroy(this.gameObject, 4f);
        }
    }
}
