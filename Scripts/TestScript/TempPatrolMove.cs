using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class TempPatrolMove : MonoBehaviour
    {
        public Vector3 position1;
        public Vector3 position2;

        public bool IsMovePosition1;
        public float moveSpeed;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (IsMovePosition1)
            {
                transform.position = Vector3.MoveTowards(transform.position, position1, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, position1) < 0.1f)
                {
                    IsMovePosition1 = false;
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, position2, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, position2) < 0.1f)
                {
                    IsMovePosition1 = true;
                }
            }
        }
    }
}