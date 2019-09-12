using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD41
{
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioManager))]
    public class CharacterScript : MonoBehaviour
	{
        private AudioManager audioManager;
        [SerializeField] private List<AudioClip> goodInteractionAudioClips = new List<AudioClip>();
        [SerializeField] private List<AudioClip> badInteractionAudioClips = new List<AudioClip>();

        [SerializeField] bool StartActive = true;
		[SerializeField] protected float MaxCharacterSpeed = 20f;
		[SerializeField] protected float LinearAcceleration = 5f;
		[SerializeField] LayerMask GroundLayers = 0xFFFFFF;
        private Animator animator;
      
        System.Action<CharacterScript> OnDiedDelegate = delegate { };
		protected Rigidbody2D CharacterBody = null;
		Vector2 ControlDirection = new Vector2();
        private FoodToken HeldFoodToken = null;
        private Enums.FoodTokenType HeldFoodType = Enums.FoodTokenType.Null; // For Debugging
		protected float TurnSpeed = 0;
		float AccelerationModifier = 1;
		float MaxVelocityModifier = 1;

        public Rigidbody2D PhysBody
		{
			get { return CharacterBody; }
			private set { CharacterBody = value; }
		}

		public bool PossessOnStartup
		{
			get { return StartActive; }
		}

		// Use this for initialization
		protected virtual void Start()
		{
			CharacterBody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            audioManager = GetComponent<AudioManager>();
            GetComponent<PlayerCollision>().CollisionEvent = OnCollision;
        }

        private void OnEnable()
        {
        }

		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			CapsuleCollider2D Capsule = GetComponent<CapsuleCollider2D>();
			Vector2 Offset = new Vector2(transform.position.x, transform.position.y) + new Vector2(transform.up.x * Capsule.offset.x, transform.up.y * Capsule.offset.y);
			Vector3 Offset3 = new Vector3(Offset.x, Offset.y, 0);
			Gizmos.DrawSphere(Offset3, (Capsule.size.x * 0.7f) / 2);
		}

		protected virtual void Update()
		{
			if (Mathf.Abs(ControlDirection.magnitude) > 0.1f)
			{
				Quaternion TargetRotation = Quaternion.LookRotation(Vector3.forward, ControlDirection);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, Time.deltaTime * 2000);
				UpdateAnimatorVariable(ControlDirection.magnitude);
			}

			CharacterBody.angularVelocity = 0;

			// Check the ground and adjust our modifiers accordingly
			CapsuleCollider2D Capsule = GetComponent<CapsuleCollider2D>();
			Vector2 Position2D = new Vector2(transform.position.x, transform.position.y);
			Collider2D Ground = Physics2D.OverlapCircle(Position2D + Capsule.offset, (Capsule.size.x * 0.7f) / 2, GroundLayers);
			Ground = Physics2D.OverlapCapsule(Position2D /*+ Capsule.offset*/, Capsule.size / 2, CapsuleDirection2D.Vertical, Vector3.SignedAngle(Vector3.up, transform.up, Vector3.forward), GroundLayers);
			if (Ground)
			{
				GroundAnomaly GroundMod = Ground.GetComponent<GroundAnomaly>();
				if (GroundMod)
				{
					GroundMod.SetGroundModifier(ref AccelerationModifier, ref MaxVelocityModifier);
				}
			}
			else
			{
				AccelerationModifier = 1;
				MaxVelocityModifier = 1;
			}
		}
		
        public virtual void MoveRight(float Value)
		{
			ControlDirection.x = Value;

			float newX = Mathf.MoveTowards(CharacterBody.velocity.x, Value * Mathf.Max(MaxVelocityModifier, 0.1f) * MaxCharacterSpeed, Mathf.Max(AccelerationModifier, 0.1f) * LinearAcceleration);

			// Kinematic
			Vector2 MoveSpeed = new Vector2(newX, CharacterBody.velocity.y);
			MoveSpeed = Vector2.ClampMagnitude(MoveSpeed, Mathf.Max(MaxVelocityModifier, 0.1f) * MaxCharacterSpeed);
			CharacterBody.velocity = MoveSpeed;
		}

		public virtual void MoveUp(float Value)
		{
			ControlDirection.y = Value;

			float newY = Mathf.MoveTowards(CharacterBody.velocity.y, Value * Mathf.Max(MaxVelocityModifier, 0.1f) * MaxCharacterSpeed, Mathf.Max(AccelerationModifier, 0.1f) * LinearAcceleration);

			// Kinematic
			Vector2 MoveSpeed = new Vector2(CharacterBody.velocity.x, newY);
			MoveSpeed = Vector2.ClampMagnitude(MoveSpeed, Mathf.Max(MaxVelocityModifier, 0.1f) * MaxCharacterSpeed);
			CharacterBody.velocity = MoveSpeed;
		}

        protected virtual void OnCollision(Transform other)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Kill Volume"))
            {
                LevelManager.Singleton.UnitDied(transform);                

                if (HeldFoodToken)
                {
                    LevelManager.Singleton.FoodTokenDropped(HeldFoodToken);
                    HeldFoodToken.FoodDropped();
                    HeldFoodToken.transform.SetParent(null);
                    HeldFoodToken = null;
                }

                OnDiedDelegate(this);
                OnDiedDelegate = null;

                // Do other death stuff here.
                Destroy(gameObject);
            }
            else if (other.CompareTag("ExitDoor"))
            {
                LevelManager.Singleton.UnitReachedExit(transform);

                if (HeldFoodToken != null)
                    LevelManager.Singleton.FoodTokenCollected(HeldFoodToken);

                OnDiedDelegate(this);
                OnDiedDelegate = null;

                // Do other Despawn stuff
                Destroy(gameObject);
            }
            else if (other.CompareTag("FoodToken"))
            {
                if (HeldFoodToken == null && other.GetComponent<FoodToken>())
                {
                    HeldFoodToken = other.GetComponent<FoodToken>();

                    if (!HeldFoodToken.IsPickedUp && HeldFoodToken.FoodType != Enums.FoodTokenType.Null)
                    {
                        audioManager.PickRandomClip(goodInteractionAudioClips);
                        audioManager.PlayClip();

                        HeldFoodToken.FoodPickedUp();
                        HeldFoodType = HeldFoodToken.FoodType;
                        LevelManager.Singleton.FoodTokenPickedUpEvent(HeldFoodToken.FoodType);
                        other.SetParent(transform);
                        other.localPosition = new Vector3(0, 1.2f, 0);
                    }
                }
            }
            else if (other.CompareTag("PrepTable"))
            {
                if (other.GetComponent<FoodManager>() != null && HeldFoodToken != null)
                {
                    if(other.GetComponent<FoodManager>().IsActive && !HeldFoodToken.IsCooked)
                    {
                        audioManager.PickRandomClip(goodInteractionAudioClips);
                        audioManager.PlayClip();

                        LevelManager.Singleton.FoodTokenCooked(HeldFoodToken);
                        //HeldFoodToken.FoodDropped();
                        HeldFoodToken.transform.SetParent(null);
                        other.GetComponent<FoodManager>().IngredientAdded(HeldFoodToken);
                        HeldFoodToken = null;
                        HeldFoodType = Enums.FoodTokenType.Null;
                    }
                }
            }

            if (HeldFoodToken != null && transform.childCount == 0)
            {
                HeldFoodToken = null;
                HeldFoodType = Enums.FoodTokenType.Null;
            }
        }

        protected void UpdateAnimatorVariable(float value)
        {
            if (animator != null)
                animator.SetFloat("Velocity", value);
        }

		public void RegisterOnDeathCallback(System.Action<CharacterScript> Callback)
		{
			OnDiedDelegate += Callback;
		}

		public void ApplyModifiers(float ModAccel, float ModMaxSpeed)
		{
			AccelerationModifier += ModAccel;
			MaxVelocityModifier += ModMaxSpeed;
		}

		public void RemoveModifiers(float ModAccel, float ModMaxSpeed)
		{
			AccelerationModifier -= ModAccel;
			MaxVelocityModifier -= ModMaxSpeed;
		}
	}
}
