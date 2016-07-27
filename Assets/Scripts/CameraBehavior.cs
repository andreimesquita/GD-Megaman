using UnityEngine;
using Megaman.Util;

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

        [Header("Camera")]
        [SerializeField]
        [Range(1, 30)]
        private float minOrthographicZoom = 7;

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

            float newOrtographicSize = FindRequiredSize();

            camera.orthographicSize = newOrtographicSize;
        }

        private float FindRequiredSize()
        {
            // Find the position the camera rig is moving towards in its local space.
            Vector3 desiredLocalPos = transform.InverseTransformPoint(camera.transform.position);

            // Start the camera's size calculation at zero.
            float size = 0f;

            //Player1 - Normaliza a posição da câmera com base no player1
            Vector3 targetLocalPos = transform.InverseTransformPoint(player1.position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / (camera.aspect * .8f));

            ////Player2 - Normaliza a posição da câmera com base no player2
            targetLocalPos = transform.InverseTransformPoint(player1.position);

            desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / (camera.aspect * .8f));

            size = Mathf.Max(size, minOrthographicZoom);

            return size;
        }
    }
}