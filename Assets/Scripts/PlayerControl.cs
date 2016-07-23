using UnityEngine;

/// <summary>
/// Esta classe é responsável por captar o Input de um player particular (servindo tanto para
/// o player 1, quanto para o player 2) e fazendo o seu personagem mover e saltar.
/// </summary>
public class PlayerControl : MonoBehaviour
{
    //Definição dos layers
    private int LAYER_MASK_SCENERY;
    
    [Header("Input")]
    [SerializeField]
    private PlayerKeysSO playerKeysSO;

    [Header("Movement & Jump")]
    [SerializeField]
    private float moveForce = 200f;
    [SerializeField]
    private float maxSpeed = 6f;
    [SerializeField]
    private float jumpForce = 15f;
    [SerializeField]
    [Range(.1f, 1)]
    private float wallJumpForce = 1f;
    [SerializeField]
    [Range(0f, 1f)]
    private float wallJumpDelay = .3f;

    ///O ideal é manter este valor em .3f.
    private const float groundCheckCircleRadius = .3f;
    
    private float wallJumpCounter = 0;

    //Collision detection Transforms
    private Transform groundCheck, wallCheck;

    //Internal logic variables
    private bool IsGrounded, IsFacingRight = true, WannaJump, IsLeaningTheWall, canMove = true;

    private static byte counter = 0;
    private byte labelYPos;

    private new Rigidbody2D rigidbody2D;

    float hAxis = 0;

    void Awake()
	{
        labelYPos = counter++;

		groundCheck = transform.Find("groundCheck");
        wallCheck = transform.Find("wallCheck");

        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        LAYER_MASK_SCENERY = LayerMask.NameToLayer("Scenery");
    }

    void Update()
	{
        CheckCollisionTriggers();

        if (!canMove)
        {
            //Impossibilita que o player se mova durante um pequeno intervalo de tempo, logo após o salto
            wallJumpCounter += Time.deltaTime;

            if (wallJumpCounter < wallJumpDelay)
                return;

            //Reseta o contador
            wallJumpCounter = 0;

            canMove = true;
        }

        //Permite que o personagem salte apenas quando está no chão
        if (Input.GetKeyDown(playerKeysSO.jumpKey) && (IsGrounded || IsLeaningTheWall))
			WannaJump = true;
	}

	void FixedUpdate()
	{
        hAxis = 0;

        if (canMove)
        {
            if (Input.GetKey(playerKeysSO.leftKey))
            {
                hAxis = -1;
            }
            else if (Input.GetKey(playerKeysSO.rightKey))
            {
                hAxis = 1;
            }
        }

        // If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
        if (hAxis * rigidbody2D.velocity.x < maxSpeed)
        {
            // ... add a force to the player.
            rigidbody2D.AddForce(Vector2.right * hAxis * moveForce);
        }

        // If the player's horizontal velocity is greater than the maxSpeed...
        if (Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed)
        {
            // ... set the player's velocity to the maxSpeed in the x axis.
            rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed,
                                               rigidbody2D.velocity.y);
        }

        // If the input is moving the player right and the player is facing left...
        if (hAxis > 0 && !IsFacingRight)
        {
            // ... flip the player.
            FlipHorizontally();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (hAxis < 0 && IsFacingRight)
        {
            // ... flip the player.
            FlipHorizontally();
        }

		// If the player should jump...
		if(WannaJump)
		{
            Vector2 jumpVelocity = new Vector2(0f, jumpForce);

            if (IsLeaningTheWall && !IsGrounded)
            {
                jumpVelocity.x = (IsFacingRight ? -jumpForce : jumpForce) * wallJumpForce;

                canMove = false;

                rigidbody2D.velocity = Vector2.zero;
            }

            // Add a vertical force to the player.
            rigidbody2D.AddForce(jumpVelocity, ForceMode2D.Impulse);

            // Make sure the player can't jump again until the jump conditions from Update are satisfied.
            WannaJump = false;
		}
	}
	
    void OnGUI()
    {
        GUI.color = Color.white;

        GUI.Label(new Rect(8, labelYPos * 16, Screen.width, Screen.height), 
          string.Format("{0} ({1}, {2}, {3}) - IsGrounded:{4} | IsLeaningTheWall:{5}", 
          name, playerKeysSO.jumpKey, playerKeysSO.leftKey, playerKeysSO.rightKey, 
          IsGrounded, IsLeaningTheWall));
    }

    private void CheckCollisionTriggers()
    {
        //Verifica se o personagem está no chão
        //IsGrounded = Physics2D.Linecast(transform.position, groundCheck.position,
        //                              1 << LAYER_MASK_SCENERY);

        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckCircleRadius, 
                                             1 << LAYER_MASK_SCENERY);

        IsLeaningTheWall = Physics2D.OverlapPoint(wallCheck.position, 1 << LAYER_MASK_SCENERY);
    }

    private void FlipHorizontally ()
	{
		IsFacingRight = !IsFacingRight;

		/* Multiplica a escala local do player por -1, fazendo com que o sprite 
        rotacione horizontalmente, junto com os checkers que são filhos deste objeto */
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
