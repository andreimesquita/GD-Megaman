using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Megaman
{
    [RequireComponent(typeof(Camera))]
    public class CameraBehavior : MonoBehaviour
    {

        private new Camera camera;

        [Header("Players")]
        /// <summary>
        /// Referência ao Transform do player1.
        /// </summary>
        [SerializeField]
        public Transform player1;

        /// <summary>
        /// Referência ao Transform do player2.
        /// </summary>
        [SerializeField]
        private Transform player2;

        private float defaultZ;

        void Awake()
        {
            if (player1 == null || player2 == null)
            {
                throw new MissingReferenceException("player1 or player2 is not set!");
            }

            camera = GetComponent<Camera>();
        }

        void Start()
        {
            defaultZ = camera.transform.position.z;
        }

        void Update()
        {
            //Define a posição atual da câmera como sendo o centro entre os players
            //posicao = (pos1 + pos2) / 2;
            Vector3 newCameraPos = (player1.position + player2.position) * .5f;

            //Corrige a posição Z da câmera
            newCameraPos.z = defaultZ;

            camera.transform.position = newCameraPos;
        }

    }
}