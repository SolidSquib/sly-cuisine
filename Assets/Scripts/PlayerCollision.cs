using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LD41
{
    public class PlayerCollision : UnitCollision
    {
        protected override void CollisionHandler(Transform other)
        {
            CollisionEvent(other);
        }        
    }
}