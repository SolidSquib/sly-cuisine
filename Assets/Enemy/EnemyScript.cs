using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD41
{
	[RequireComponent(typeof(ITargeter))]
	[RequireComponent(typeof(AIConeDetection))]
	public class EnemyScript : CharacterScript
	{
		[SerializeField] float PatrolSpeed = 10f;
		[SerializeField] bool StopChasingWhenSightImpaired = false;
		[SerializeField] bool bUsePhysics = false;

		AIConeDetection ConeDetection = null;
		ITargeter Targeter = null;
		List<Transform> CurrentTargets = new List<Transform>();

		// Use this for initialization
		protected override void Start()
		{
			base.Start();
            
			ConeDetection = GetComponent<AIConeDetection>();
			ConeDetection.RegisterOnObjectDetectedCallback(OnObjectDetected);
			ConeDetection.RegisterOnObjectLostCallback(OnObjectLost);

			Targeter = GetComponent<ITargeter>();
		}
		protected virtual void Disable()
		{
			ConeDetection.UnregisterOnObjectDetectedCallback(OnObjectDetected);
			ConeDetection.UnregisterOnObjectLostCallback(OnObjectLost);
		}

		void OnObjectDetected(GameObject Object)
		{
			if (Object && Object.CompareTag("Player"))
			{
				Targeter.SetTarget(Object.transform);
			}
		}

		void OnObjectLost(GameObject Object)
		{
			if (StopChasingWhenSightImpaired && Object && Object.CompareTag("Player"))
			{
				Targeter.SetTarget(null);
			}
		}

		public virtual void MoveAI(Vector2 Target, bool bPatrolling)
		{
			Vector2 Direction = (Target - CharacterBody.position).normalized;
			if (bUsePhysics)
				CharacterBody.AddForce(Direction * (bPatrolling ? PatrolSpeed : MaxCharacterSpeed));
			else
				CharacterBody.velocity = Direction * (bPatrolling ? PatrolSpeed : MaxCharacterSpeed);
		}

		public override void MoveUp(float Value)
		{
			// Do nothing
		}

		public override void MoveRight(float Value)
		{
			// Do nothing
		}

		protected override void OnCollision(Transform other)
		{
			
		}

		protected override void Update()
		{
			if (CurrentTargets.Count == 0 && CharacterBody.velocity.normalized.magnitude > 0.1f)
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, CharacterBody.velocity.normalized), 0.5f);
				UpdateAnimatorVariable(CharacterBody.velocity.magnitude);
			}
			else
			{
				CharacterBody.angularVelocity = 0;
			}
		}

		public Transform GetFirstTarget()
		{
			while (CurrentTargets.Count > 0 && CurrentTargets[0] == null)
			{
				CurrentTargets.RemoveAt(0);
			}

			if (CurrentTargets.Count > 0)
			{
				return CurrentTargets[0];
			}

			return null;
		}
	}
}
