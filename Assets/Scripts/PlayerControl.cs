#pragma warning disable 0168 //variable declared but not used
#pragma warning disable 0219 //variable assigned but not used
#pragma warning disable 0414 //private field assigned but not used

using UnityEngine;

namespace Megaman.Player
{
    /// <summary>
    /// Esta classe é responsável por captar o Input de um player particular (servindo tanto para
    /// o player 1, quanto para o player 2) e fazendo o seu personagem mover e saltar.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerControl : MonoBehaviour
    {
        private bool isLookingLeft = false;

        [Header("Shoot")]
        [SerializeField]
        private float shootVelocity;
        [SerializeField]
        private GameObject prefebShoot;

        //Definição dos layers
        private int LAYER_MASK_SCENERY;
        private int LAYER_MASK_PLAYER;

        [Header("Input")]
        [SerializeField]
        private PlayerKeysSO playerKeysSO;

        private Transform shootRight;
        private Transform shootLeft;

        [Header("Movement")]
        [SerializeField]
        [Range(1, 400)]
        private float moveForce = 200f;
        [SerializeField]
        [Range(1, 20)]
        private float maxMovSpeed = 6f;
        [SerializeField]
        [Range(-20, -1)]
        private float minFallingVelocity = -8;

        [Header("Jump")]
        [SerializeField]
        [Range(1, 30)]
        private float jumpForce = 15f;
        [SerializeField]
        [Range(.1f, 1)]
        private float wallJumpForce = 1f;
        [SerializeField]
        [Range(0f, 1f)]
        private float wallJumpDelay = .22f;

        private float wallJumpCounter = 0;

        //Collision detection Transforms
        private Transform groundCheck, leftWallCheck, rightWallCheck;

        /// <summary>
        /// Raio do circulo utilizado para verificar colisão com o cenário.
        /// O ideal é manter este valor em .3f.
        /// </summary>
        private const float groundCheckCircleRadius = .3f;

        //Internal logic variables
        private bool IsGrounded, canMove = true;
        private bool IsLeaningAWall, IsLeaningTheRightWall, IsLeaningTheLeftWall;

        private new Rigidbody2D rigidbody2D;

        /// <summary>
        /// Determina posição em que o personagem quer se mover (left == -1 e Right == 1)
        /// </summary>
        float hAxis;

        /* Os seguintes atributos servem apenas para a correta apresentação 
           dos Labels no método OnGUI() */
        private static byte counter = 0;
        private byte labelYPos;

        void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();

            labelYPos = counter++;

            groundCheck = transform.FindChild("groundCheck");

            if (groundCheck == null)
                throw new MissingComponentException("groundCheck não encontrado!");

            leftWallCheck = transform.FindChild("leftWallCheck");

            if (leftWallCheck == null)
                throw new MissingComponentException("leftWallCheck não encontrado!");

            rightWallCheck = transform.FindChild("rightWallCheck");

            if (rightWallCheck == null)
                throw new MissingComponentException("rightWallCheck não encontrado!");

            shootRight = transform.FindChild("shootRight");

            if (shootRight == null)
                throw new MissingComponentException("shootRight não encontrado!");

            shootLeft = transform.FindChild("shootLeft");

            if (shootLeft == null)
                throw new MissingComponentException("shootLeft não encontrado!");
        }

        void Start()
        {
            LAYER_MASK_SCENERY = LayerMask.NameToLayer("Scenery");
            LAYER_MASK_PLAYER = LayerMask.NameToLayer(playerKeysSO.otherPlayerLayer);
        }

        void Update()
        {
            CheckCollisionTriggers();

            ReadInput();
             //test
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
        }

        void FixedUpdate()
        {
            if (canMove)
            {
                // If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
                if (hAxis * rigidbody2D.velocity.x < maxMovSpeed)
                {
                    // ... Adiciona força no moveimtno do personagem
                    rigidbody2D.AddForce(Vector2.right * hAxis * moveForce);
                }
            } else if (Mathf.Abs(rigidbody2D.velocity.x) > maxMovSpeed)
            {
                // ... define a velocidade do player como sendo sua velocidade máxima, no eixo X.
                rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxMovSpeed,
                                                   rigidbody2D.velocity.y);
            }

            //Se a velocidade vertical, quando o player está caindo, é menor do que sua velocidade mínima...
            if (rigidbody2D.velocity.y < minFallingVelocity)
            {
                // ... define a velocidade Y do player como sendo sua velocidade mínima, no eixo X.
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, minFallingVelocity);
            }
        }

        private void CheckCollisionTriggers()
        {
            //Verifica se o personagem está no chão
            IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckCircleRadius,
                                                 1 << LAYER_MASK_SCENERY)
                                                 
                         || Physics2D.OverlapCircle(groundCheck.position, groundCheckCircleRadius,
                                                 1 << LAYER_MASK_PLAYER);

            //Verifica se o personagem está encostado em alguma parede
            IsLeaningTheLeftWall =
                Physics2D.OverlapPoint(leftWallCheck.position, 1 << LAYER_MASK_SCENERY);

            IsLeaningTheRightWall =
                Physics2D.OverlapPoint(rightWallCheck.position, 1 << LAYER_MASK_SCENERY);

            IsLeaningAWall = IsLeaningTheLeftWall || IsLeaningTheRightWall;
        }

        private void ReadInput()
        {
            hAxis = 0;

            if (canMove)
            {
                if (Input.GetKey(playerKeysSO.leftKey))
                {
                    hAxis = -1;
                    isLookingLeft = true;
                }
                else if (Input.GetKey(playerKeysSO.rightKey))
                {
                    hAxis = 1;
                    isLookingLeft = false;
                }
            }

            //Permite que o personagem salte apenas quando está no chão
            if (Input.GetKeyDown(playerKeysSO.jumpKey) && (IsGrounded || IsLeaningAWall))
            {
                Vector2 jumpVelocity = new Vector2(0f, jumpForce);

                //Permite ao personagem saltar quando encostado na parede
                if (IsLeaningAWall && !IsGrounded)
                {
                    jumpVelocity.x = (IsLeaningTheRightWall ? -jumpForce : jumpForce) * wallJumpForce;

                    //Anula a velocidade atual do rigidbody antes de forçar o salto
                    rigidbody2D.velocity = Vector2.zero;

                    canMove = false;
                }

                // Add a vertical force to the player.
                rigidbody2D.AddForce(jumpVelocity, ForceMode2D.Impulse);
            }

            if (Input.GetKeyDown(playerKeysSO.fire))
            {
                GameObject gameObject = Instantiate(prefebShoot);

                gameObject.transform.position = isLookingLeft ? shootLeft.position : shootRight.position;  

                gameObject.GetComponent<ShootController>().AddVelocity(isLookingLeft ? -shootVelocity : shootVelocity);
            }
        }

        void OnGUI()
        {
            GUI.color = Color.white;

            GUI.Label(new Rect(8, labelYPos * 16, Screen.width, Screen.height),
              string.Format("{0} ({1}, {2}, {3}) - IsGrounded:{4} | IsLeaningTheWall:{5}",
              name, playerKeysSO.jumpKey, playerKeysSO.leftKey, playerKeysSO.rightKey,
              IsGrounded, IsLeaningAWall));
        }

        void OnDrawGismoz()
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawSphere(groundCheck.position, groundCheckCircleRadius);
        }

    }
}