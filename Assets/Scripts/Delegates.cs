using System;
using System.Collections.Generic;
using UnityEngine;

namespace LD41
{
    namespace Delegates
    {
        public delegate void Delegate_Vector3Param(Vector3 Position);
        public delegate void Delegate_TwoVector3Params(Vector3 Position, Vector3 Delta);
		public delegate void Delegate_Float(float Value);
        public delegate void Delegate_Empty();
    }

    namespace Enums
    {
        public enum FoodTokenType
        {
            Null,
            Tomato,
            Cheese,
            Bread,
            Turnip,
            Meat,
            Ham,
            CookedPerfect,
            CookedImperfect
        }
    }
}
