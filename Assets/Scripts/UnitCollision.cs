using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LD41
{
    public class UnitCollision : MonoBehaviour
    {
        public Action<Transform> CollisionEvent = delegate { };

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log("OnCollisionEnter2D : " + collision.transform);
            CollisionHandler(collision.transform);
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log("OnTriggerEnter2D : " + collision.transform);
            CollisionHandler(collision.transform);
        }

        protected virtual void CollisionHandler(Transform other)
        {
            CollisionEvent(other);
        }        
    }
}