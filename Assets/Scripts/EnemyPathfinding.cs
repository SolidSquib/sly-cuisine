using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD41
{
    public class EnemyPathfinding : MonoBehaviour
    {
        [SerializeField] private Transform patrolParent;
        [SerializeField] private List<Transform> patrolPoints = new List<Transform>();
        [SerializeField] private Transform currentTargetLocation;
        [SerializeField] private bool resetToZero = false;
        private Rigidbody2D rb2D;

        public float toVel = 2.5f;      // Converts the distance remaining to the desired velocity (if too low, the rb2D slows down early and takes a long time to stop)
        public float maxVel = 15.0f;    // The max speed the rb2D will reach when moving
        public float maxForce = 40.0f;  // Limits the force applied to the rb2D to avoid excessive acceleration
        public float gain = 5f;

        private void Awake()
        {
            rb2D = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            //transform.position = patrolPoints[0].position;

            if (patrolParent != null)
            {
                foreach (Transform v in patrolParent)
                    patrolPoints.Add(v);
            }

            SetPatrolPoint(0);

            if (GetComponent<UnitCollision>() != null)
            {
                GetComponent<UnitCollision>().CollisionEvent += NextPatrolPoint;
            }
        }

        private void OnDisable()
        {
            if (GetComponent<UnitCollision>() != null)
            {
                GetComponent<UnitCollision>().CollisionEvent -= NextPatrolPoint;
            }
        }

        private void NextPatrolPoint(Transform other)
        {
            if (other.CompareTag("PatrolPoint") && patrolPoints.Contains(other))
            {
                int newIndex = patrolPoints.IndexOf(other);

                if (newIndex == (patrolPoints.Count - 1))
                {
                    newIndex = 0;

                    if (!resetToZero)
                        patrolPoints.Reverse();
                }

                newIndex++;

                SetPatrolPoint(newIndex);
            }
        }

        private void SetPatrolPoint(int index)
        {
            if (index < patrolPoints.Count)
            {
                currentTargetLocation = patrolPoints[index];
            }
        }

        private void FixedUpdate()
        {
            if (currentTargetLocation != null)
            {
                RotateToTarget();
                TestMovement();
            }
        }

        private void SetValues()
        {
            //targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            ////testDistance = targetPosition - transform.position;
            ////testDistance.z = 0;
            //Vector3 playerPos = new Vector3(transform.position.x, transform.position.y, 0);
            //Vector3 targetPos = new Vector3(targetPosition.x, targetPosition.y, 0);
            //currentDistance = Vector3.Distance(targetPos, playerPos);
            ////Debug.Log("currentDistance " + currentDistance);
        }

        private void RotateToTarget()
        {
            Vector3 diff = currentTargetLocation.position - transform.position;
            diff.Normalize();

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        }

        private void MoveToTarget()
        {
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(currentTargetLocation.position.x, currentTargetLocation.position.y), 3 * Time.deltaTime);
        }

        private void TestMovement()
        {
            Vector3 dist = currentTargetLocation.position - transform.position;
            dist.z = 0; // ignore height differences
                        // calc a target vel proportional to distance (clamped to maxVel)
                        //Debug.Log("dist " + dist);
            Vector3 tgtVel = Vector3.ClampMagnitude(toVel * dist, maxVel);
            // calculate the velocity error
            Vector3 error = (tgtVel - new Vector3(rb2D.velocity.x, rb2D.velocity.y, 0));
            // calc a force proportional to the error (clamped to maxForce)
            Vector3 force = Vector3.ClampMagnitude(gain * error, maxForce);
            rb2D.AddForce(force);
        }
    }
}