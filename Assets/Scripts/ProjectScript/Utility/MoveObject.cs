using UnityEngine;
using System.Collections;

namespace ProjectScript
{
    public class MoveObject : MonoBehaviour
    {
        public float speed = 250;

        void Update()
        {
            transform.position = transform.position + transform.forward * Time.deltaTime * speed;
        }
    }
}
