using UnityEngine;

namespace Megaman.Util.Extensions
{
    /// <summary>
    /// Métodos de extensão para a classe Vector3.
    /// </summary>
    public static class Vector3Extensions
    {
        /// <summary>
        /// Transforma um Vector3 em um Vector2, retirando o eixo Z.
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }

    }
}