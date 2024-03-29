﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD41
{
    public class InputManager : MonoBehaviour
    {
        // Singleton instance
        private static InputManager mInputMan;

        // Mouse
        public event Delegates.Delegate_TwoVector3Params OnMouseMoveEvent;
        public event Delegates.Delegate_Vector3Param OnLeftMouseButtonDown;
        public event Delegates.Delegate_Vector3Param OnLeftMouseButtonUp;
        public event Delegates.Delegate_Vector3Param OnRightMouseButtonDown;
        public event Delegates.Delegate_Vector3Param OnRightMouseButtonUp;

		// Raw Key input
		private Dictionary<KeyCode, System.Action> KeyDownCallbacks = new Dictionary<KeyCode, System.Action>();
		private Dictionary<KeyCode, System.Action> KeyUpCallbacks = new Dictionary<KeyCode, System.Action>();
		private Dictionary<KeyCode, bool> KeyEventStates = new Dictionary<KeyCode, bool>();

		// Editor bound input
		private Dictionary<string, System.Action> ButtonDownCallbacks = new Dictionary<string, System.Action>();
		private Dictionary<string, System.Action> ButtonUpCallbacks = new Dictionary<string, System.Action>();
		private Dictionary<string, bool> ButtonEventStates = new Dictionary<string, bool>();

		// Axis input
		private Dictionary<string, System.Action<float>> AxisCallbacks = new Dictionary<string, System.Action<float>>();
		private Dictionary<string, float> AxisValues = new Dictionary<string, float>();

		// Mouse variables
		Vector3 mLastMousePosition = new Vector3();
        bool mLeftMouseButtonDown = false;
        bool mRightMouseButtonDown = false;

        #region Property Accessors 
        public static InputManager Singleton
        {
            get { return mInputMan; }
        }

        public Vector3 MousePosition
        {
            get { return mLastMousePosition; }
        }
        #endregion

        void Awake()
        {
            if (!mInputMan)
            {
                mInputMan = this;
            }
            else if (mInputMan != this)
            {
                Destroy(gameObject);
            }
        }

        // Use this for initialization
        void Start()
        {
		}

        void OnDisable()
        {
        }

        void Update()
        {
            // Mouse events
            CheckMouseEvents();
            CheckKeyboardEvents();
			CheckButtonEvents();
			DistributeAxisEvents();
        }

        void CheckMouseEvents()
        {
            // Check and update mouse position
            Vector3 NewMousePosition = Input.mousePosition;
            if (OnMouseMoveEvent != null && NewMousePosition != mLastMousePosition)
            {
                OnMouseMoveEvent(NewMousePosition, NewMousePosition - mLastMousePosition);
                mLastMousePosition = NewMousePosition;
            }

            // Check and update left mouse button
            bool LeftMouseButtonDown = Input.GetMouseButton(0);
            if(OnLeftMouseButtonDown != null && LeftMouseButtonDown && !mLeftMouseButtonDown)
            {
                OnLeftMouseButtonDown(mLastMousePosition);
            }
            else if (OnLeftMouseButtonUp != null && !LeftMouseButtonDown && mLeftMouseButtonDown)
            {
                OnLeftMouseButtonUp(mLastMousePosition);
            }
            mLeftMouseButtonDown = LeftMouseButtonDown;

            // Check and update right mouse button
            bool RightMouseButtonDown = Input.GetMouseButton(1);
            if (OnRightMouseButtonDown != null && RightMouseButtonDown && !mRightMouseButtonDown)
            {
                OnRightMouseButtonDown(mLastMousePosition);
            }
            else if (OnRightMouseButtonUp != null && !RightMouseButtonDown && mRightMouseButtonDown)
            {
                OnRightMouseButtonUp(mLastMousePosition);
            }
            mRightMouseButtonDown = RightMouseButtonDown;
        }

        void CheckKeyboardEvents()
        {
			foreach (var Pair in KeyEventStates)
			{
				bool KeyDown = Input.GetKeyDown(Pair.Key);
				if (KeyEventStates.ContainsKey(Pair.Key) && KeyDown != KeyEventStates[Pair.Key])
				{
					if (KeyDown && KeyDownCallbacks.ContainsKey(Pair.Key))
					{
						KeyDownCallbacks[Pair.Key]();
					}
					else if (!KeyDown && KeyUpCallbacks.ContainsKey(Pair.Key))
					{
						KeyUpCallbacks[Pair.Key]();
					}

					KeyEventStates[Pair.Key] = KeyDown;
				}
			}
        }

		void CheckButtonEvents()
		{
			foreach (var Pair in ButtonEventStates)
			{
				bool ButtonDown = Input.GetKeyDown(Pair.Key);
				if (ButtonEventStates.ContainsKey(Pair.Key) && ButtonDown != ButtonEventStates[Pair.Key])
				{
					if (ButtonDown && ButtonDownCallbacks.ContainsKey(Pair.Key))
					{
						ButtonDownCallbacks[Pair.Key]();
					}
					else if (!ButtonDown && ButtonUpCallbacks.ContainsKey(Pair.Key))
					{
						ButtonUpCallbacks[Pair.Key]();
					}

					ButtonEventStates[Pair.Key] = ButtonDown;
				}
			}
		}

		void DistributeAxisEvents()
		{
			foreach (var Pair in AxisCallbacks)
			{
				AxisValues[Pair.Key] = Input.GetAxis(Pair.Key);
				Pair.Value(AxisValues[Pair.Key]);
			}
		}

		#region Register and Unregister input events
		public void RegisterKeyDownEvent(KeyCode Key, System.Action Callback)
		{
			if (KeyDownCallbacks.ContainsKey(Key))
			{
				KeyDownCallbacks[Key] += Callback;
			}
			else
			{
				KeyDownCallbacks.Add(Key, Callback);
				if (!KeyEventStates.ContainsKey(Key))
				{
					KeyEventStates.Add(Key, false);
				}
			}
		}

		public void RegisterKeyUpEvent(KeyCode Key, System.Action Callback)
		{
			if (KeyUpCallbacks.ContainsKey(Key))
			{
				KeyUpCallbacks[Key] += Callback;
			}
			else
			{
				KeyUpCallbacks.Add(Key, Callback);
				if (!KeyEventStates.ContainsKey(Key))
				{
					KeyEventStates.Add(Key, false);
				}
			}
		}

		public void UnregisterKeyDownEvent(KeyCode Key, System.Action Callback)
		{
			if (KeyDownCallbacks.ContainsKey(Key))
			{
				KeyDownCallbacks[Key] -= Callback;
				if (KeyDownCallbacks[Key] == null)
				{
					KeyDownCallbacks.Remove(Key);
					if (!KeyUpCallbacks.ContainsKey(Key))
					{
						KeyEventStates.Remove(Key);
					}
				}
			}
		}

		public void UnregisterKeyUpEvent(KeyCode Key, System.Action Callback)
		{
			if (KeyUpCallbacks.ContainsKey(Key))
			{
				KeyUpCallbacks[Key] -= Callback;
				if (KeyUpCallbacks[Key] == null)
				{
					KeyUpCallbacks.Remove(Key);
					if (!KeyDownCallbacks.ContainsKey(Key))
					{
						KeyEventStates.Remove(Key);
					}
				}
			}
		}

		public void RegisterButtonDownEvent(string Key, System.Action Callback)
		{
			if (ButtonDownCallbacks.ContainsKey(Key))
			{
				ButtonDownCallbacks[Key] += Callback;
			}
			else
			{
				ButtonDownCallbacks.Add(Key, Callback);
				if (!ButtonEventStates.ContainsKey(Key))
				{
					ButtonEventStates.Add(Key, false);
				}
			}
		}

		public void RegisterButtonUpEvent(string Key, System.Action Callback)
		{
			if (ButtonUpCallbacks.ContainsKey(Key))
			{
				ButtonUpCallbacks[Key] += Callback;
			}
			else
			{
				ButtonUpCallbacks.Add(Key, Callback);
				if (!ButtonEventStates.ContainsKey(Key))
				{
					ButtonEventStates.Add(Key, false);
				}
			}
		}

		public void RegisterAxisEvent(string Key, System.Action<float> Callback)
		{
			if (AxisCallbacks.ContainsKey(Key))
			{
				AxisCallbacks[Key] += Callback;
			}
			else
			{
				AxisCallbacks.Add(Key, Callback);
				AxisValues.Add(Key, 0);
			}
		}

		public void UnregisterAxisEvent(string Key, System.Action<float> Callback)
		{
			if (AxisCallbacks.ContainsKey(Key))
			{
				AxisCallbacks[Key] -= Callback;
				if(AxisCallbacks[Key] == null)
				{
					AxisCallbacks.Remove(Key);
					AxisValues.Remove(Key);
				}
			}
		}
		#endregion
	}
}
