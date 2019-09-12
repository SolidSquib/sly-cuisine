using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD41
{
    public class FoodToken : MonoBehaviour
    {
        private Animator animator;
        private CircleCollider2D circleCollider2D;

        [SerializeField] private bool m_IsCooked = false;
        [SerializeField] private Enums.FoodTokenType m_FoodType;
        public Enums.FoodTokenType FoodType { get { return m_FoodType; } }
        public bool IsCooked { get { return m_IsCooked; } set { m_IsCooked = value; } }
        private bool m_IsPickedUp = false;
        public bool IsPickedUp { get { return m_IsPickedUp; } }

        private void Awake()
        {
            if (GetComponent<CircleCollider2D>() != null)
            {
                circleCollider2D = GetComponent<CircleCollider2D>();
                circleCollider2D.enabled = true;
            }

            if (GetComponentInChildren<Animator>() != null)
                animator = GetComponentInChildren<Animator>();

            if (m_FoodType == Enums.FoodTokenType.CookedPerfect || m_FoodType == Enums.FoodTokenType.CookedImperfect)
            {
                if (m_IsCooked == false)
                    IsCooked = true;
            }
        }

        private void OnEnable()
        {
            if (circleCollider2D != null)
                circleCollider2D.enabled = true;
        }

        public void FoodPickedUp()
        {
            m_IsPickedUp = true;

                if(circleCollider2D != null)
                circleCollider2D.enabled = false;

            if (animator != null)
                animator.SetTrigger("PickedUp");
        }

        public void FoodDropped()
        {
            m_IsPickedUp = false;

            if (circleCollider2D != null)
                circleCollider2D.enabled = true;

            if (animator != null)
                animator.SetTrigger("Dropped");
        }
    }
}
   
