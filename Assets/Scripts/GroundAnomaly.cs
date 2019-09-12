using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD41
{
	[RequireComponent(typeof(Collider2D))]
	public class GroundAnomaly : MonoBehaviour
	{
		[SerializeField] float MaxSpeedModifier = 0;
		[SerializeField] float AccelerationModifier = 0;

		public float SpeedModifier
		{
			get { return MaxSpeedModifier; }
		}

		public float AccelModifier
		{
			get { return AccelerationModifier; }
		}

		private void Start()
		{
			GetComponent<Collider2D>().isTrigger = true;
		}

		protected virtual void OnCollisionEnter2D(Collision2D collision)
		{
			//Debug.Log("OnCollisionEnter2D : " + collision.transform);
			CollisionHandlerEnter(collision.transform);
		}

		protected virtual void OnTriggerEnter2D(Collider2D collision)
		{
			//Debug.Log("OnTriggerEnter2D : " + collision.transform);
			CollisionHandlerEnter(collision.transform);
		}

		protected virtual void OnTriggerExit2D(Collider2D collision)
		{
			CollisionHandlerExit(collision.transform);
		}

		protected virtual void CollisionHandlerEnter(Transform other)
		{
// 			CharacterScript Character = other.GetComponent<CharacterScript>();
// 			if (Character)
// 			{
// 				Character.ApplyModifiers(AccelerationModifier, MaxSpeedModifier);
// 			}
		}

		protected virtual void CollisionHandlerExit(Transform other)
		{
// 			CharacterScript Character = other.GetComponent<CharacterScript>();
// 			if (Character)
// 			{
// 				Character.RemoveModifiers(AccelerationModifier, MaxSpeedModifier);
// 			}
		}

		public void SetGroundModifier(ref float AccelMod, ref float MaxSpeedMod)
		{
			AccelMod = AccelerationModifier;
			MaxSpeedMod = MaxSpeedModifier;
		}
	}
}
