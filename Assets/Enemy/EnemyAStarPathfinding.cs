using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LD41
{
	[RequireComponent(typeof(EnemyScript))]
	public class EnemyAStarPathfinding : Pathfinding2D, ITargeter
	{
		public uint SearchPerSecond = 5;
		public float TimeToGiveUp = 3f;
		public List<Transform> Waypoints;

		private int CurrentWaypointIndex = 0;
		private Transform TargetObject = null;		
		private bool search = true;
		private float tempDistance = 0F;

		public Transform CurrentTarget
		{
			get { return TargetObject; }
			private set { TargetObject = value; }
		}

		public void SetTarget(Transform NewTarget)
		{
			TargetObject = NewTarget;
		}

		void Start()
		{
			//Make sure that we dont dividde by 0 in our search timer coroutine
			if (SearchPerSecond == 0)
				SearchPerSecond = 1;
		}

		void Update()
		{
			if(TargetObject == null && Waypoints.Count > 0)
			{
				TargetObject = Waypoints[CurrentWaypointIndex++];
				if (CurrentWaypointIndex >= Waypoints.Count)
				{
					CurrentWaypointIndex = 0;
				}
				search = true;
			}

			//Make sure we set a player in the inspector!
			if (TargetObject != null)
			{
				//save distance so we do not have to call it multiple times
				tempDistance = Vector2.Distance(transform.position, TargetObject.position);

				//Check if we are able to search
				if (search == true)
				{
					//Start the time
					StartCoroutine(SearchTimer());
					FindPath(transform.position, TargetObject.position);
				}

				//Make sure that we actually got a path! then call the new movement method
				if (Path.Count > 0)
				{
					MoveAI();
				}
			}
		}

		IEnumerator SearchTimer()
		{
			//Set search to false for an amount of time, and then true again.
			search = false;
			yield return new WaitForSeconds(1 / SearchPerSecond);
			search = true;
		}

		private void MoveAI()
		{
			//if we get close enough or we are closer then the indexed position, then remove the position from our path list, 
			if (Vector2.Distance(transform.position, Path[0]) < 0.4F || tempDistance < Vector2.Distance(Path[0], TargetObject.position))
			{
				Path.RemoveAt(0);
			}

			if (Path.Count < 1 || tempDistance < 0.4f)
			{
				TargetObject = null;
				return;
			}

			//First we will create a new vector ignoreing the depth (z-axiz).
			Vector3 ignoreZ = new Vector3(Path[0].x, Path[0].y, transform.position.z);

			//now move towards the newly created position
			//transform.position = Vector3.MoveTowards(transform.position, ignoreZ, Time.deltaTime * Speed);  
			GetComponent<EnemyScript>().MoveAI(ignoreZ, !TargetObject.CompareTag("Player"));
		}
	}
}
