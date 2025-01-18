using UnityEngine;

namespace Terrain
{
    public class CenterCircle : MonoBehaviour
    {
        public float orbitRadius = 8.0f;
        public float higherRadius = 11.0f;
        public float radius = 5f;
        public Vector3 center;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(new Vector3(center.x, center.y, 0), radius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3(center.x, center.y, 0), orbitRadius);
        }
    }
}