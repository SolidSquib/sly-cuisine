using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD41
{
	[RequireComponent(typeof(EnemyScript))]
	public class LazySwipe : MonoBehaviour, ITargeter
	{
		//[SerializeField] float SwipeDelay = 0.7f;
        //[SerializeField] float SwipeCooldown = 0.7f;
        [SerializeField] float SwipeRange = 3f;
		[SerializeField] float PatrolAngle = 90f;
		[SerializeField] float PatrolSpeed = 5f;
		[SerializeField] bool DrawPatrolAngle = false;
		[SerializeField] float WaitTime = 1f;

        private Animator animator;
		Transform CurrentTarget = null;
		EnemyScript Character = null;
		Vector3 RestingDirection = Vector3.zero;
		float MinRadians = 0;
		float MaxRadians = 0;
		float MinDegrees = 0;
		float MaxDegrees = 0;
		float CurrentPatrolAngle = 0;
		bool bPatrolReversed = false;
		bool bWaiting = false;
        //bool bSwipeWindingUp = false;
        //bool bSwipeOnCooldown = false;

        bool bAttackActive = false;

        void Start()
		{
            if(GetComponent<Animator>())
                animator = GetComponent<Animator>();

            Character = GetComponent<EnemyScript>();

			// Init patrol angle
			RestingDirection = transform.up;
			MinRadians = (360f - (PatrolAngle * 0.5f)) * Mathf.Deg2Rad;
			MaxRadians = (PatrolAngle * 0.5f) * Mathf.Deg2Rad;
			MinDegrees = (360f - (PatrolAngle * 0.5f));
			MaxDegrees = (PatrolAngle * 0.5f);
			CurrentPatrolAngle = PatrolAngle * 0.5f; // Start half way (facing forward);
		}

		public void SetTarget(Transform NewTarget)
		{
			CurrentTarget = NewTarget;
        }

        private void OnEnable()
        {
            bAttackActive = false;
        }

        private void Update()
		{
			if (DrawPatrolAngle)
			{
				float minX = RestingDirection.x * Mathf.Cos(MinRadians) - RestingDirection.y * Mathf.Sin(MinRadians);
				float minY = RestingDirection.x * Mathf.Sin(MinRadians) + RestingDirection.y * Mathf.Cos(MinRadians);
				float maxX = RestingDirection.x * Mathf.Cos(MaxRadians) - RestingDirection.y * Mathf.Sin(MaxRadians);
				float maxY = RestingDirection.x * Mathf.Sin(MaxRadians) + RestingDirection.y * Mathf.Cos(MaxRadians);

				Debug.DrawLine(transform.position, new Vector3(minX, minY, 0) * 100f, Color.red);
				Debug.DrawLine(transform.position, new Vector3(maxX, maxY, 0) * 100f, Color.red);
			}

			if (CurrentTarget)
            {
                float Distance = Vector2.Distance(CurrentTarget.position, Character.transform.position);
                Vector3 Direction = (CurrentTarget.position - Character.transform.position).normalized;

                Character.transform.up = Vector3.Lerp(Character.transform.up, Direction, 0.5f);


                if (Distance <= SwipeRange && !bAttackActive)
                {
                    bAttackActive = true;

                    if (GetComponent<Animator>())
                        animator.SetTrigger("Attack");

                    //StartCoroutine(StartSwipeDelay(SwipeDelay));
                }
                else if (Distance > SwipeRange && bAttackActive)
                {
                    bAttackActive = false;

                    if (GetComponent<Animator>())
                        animator.SetTrigger("StopAttack");
                }
            }
            else if (bAttackActive)
            {
                bAttackActive = false;

                if (GetComponent<Animator>())
                    animator.SetTrigger("StopAttack");
            }
			else if (!bWaiting)// Idle behaviour, as a security cam.
			{
				float NewPatrolAngle = Mathf.Clamp(CurrentPatrolAngle + (bPatrolReversed ? -PatrolSpeed : PatrolSpeed) * Time.deltaTime, 0, PatrolAngle);
				float Diff = NewPatrolAngle - CurrentPatrolAngle;
				Vector3 LookDirection = Quaternion.Euler(0, 0, Diff) * transform.up;
				transform.rotation = Quaternion.LookRotation(Vector3.forward, LookDirection);
				CurrentPatrolAngle = NewPatrolAngle;

				if(CurrentPatrolAngle >= PatrolAngle || CurrentPatrolAngle <= 0)
				{
					StartCoroutine(StartWait(WaitTime));
					bPatrolReversed = !bPatrolReversed;
				}
			}
        } 

        IEnumerator StartSwipeDelay(float delay)
		{            
            yield return new WaitForSeconds(delay);

            if (GetComponent<Animator>())
                animator.SetTrigger("Attack");

            //StartCoroutine(StartSwipeCooldown(SwipeCooldown));
        }

		IEnumerator StartWait(float delay)
		{
			bWaiting = true;
			yield return new WaitForSeconds(delay);
			bWaiting = false;
		}

        IEnumerator StartSwipeCooldown(float delay)
        {
            yield return new WaitForSeconds(delay);
           // bSwipeOnCooldown = false;
        }
    }
}
