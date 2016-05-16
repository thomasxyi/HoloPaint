// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// GestureManager creates a gesture recognizer and signs up for a tap gesture.
    /// When a tap gesture is detected, GestureManager uses GazeManager to find the game object.
    /// GestureManager then sends a message to that game object.
    /// </summary>
    [RequireComponent(typeof(GazeManager))]
    public class GestureManager : Singleton<GestureManager>
    {
        /// <summary>
        /// To select even when a hologram is not being gazed at,
        /// set the override focused object.
        /// If its null, then the gazed at object will be selected.
        /// </summary>
        public GameObject OverrideFocusedObject
        {
            get; set;
        }

        private GameObject focusedObject;

        // The universal tap gesture recogniszer
        public GestureRecognizer gestureRecognizer { get; private set; }

        public bool IsNavigating { get; private set; }

        public Vector3 ManipulationPosition { get; private set; }


        void Start()
        {
            // Create a new GestureRecognizer. Sign up for tapped events.
            gestureRecognizer = new GestureRecognizer();
            gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.ManipulationTranslate);

            gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;

            // Register for the ManipulationStartedEvent with the ManipulationRecognizer_ManipulationStartedEvent function.
            gestureRecognizer.ManipulationStartedEvent += GestureRecognizer_ManipulationStartedEvent;
            // Register for the ManipulationUpdatedEvent with the ManipulationRecognizer_ManipulationUpdatedEvent function.
            gestureRecognizer.ManipulationUpdatedEvent += GestureRecognizer_ManipulationUpdatedEvent;
            // Register for the ManipulationCompletedEvent with the ManipulationRecognizer_ManipulationCompletedEvent function. 
            gestureRecognizer.ManipulationCompletedEvent += GestureRecognizer_ManipulationCompletedEvent;
            // Register for the ManipulationCanceledEvent with the ManipulationRecognizer_ManipulationCanceledEvent function. 
            gestureRecognizer.ManipulationCanceledEvent += GestureRecognizer_ManipulationCanceledEvent;

            // Start looking for gestures.
            gestureRecognizer.StartCapturingGestures();
        }

        private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            if (focusedObject != null)
            {
                focusedObject.SendMessage("OnSelect");
            }
        }

        void LateUpdate()
        {
            GameObject oldFocusedObject = focusedObject;
            
            if (GazeManager.Instance.Hit && 
                OverrideFocusedObject == null &&
                GazeManager.Instance.HitInfo.collider != null)
            {
                // If gaze hits a hologram, set the focused object to that game object.
                // Also if the caller has not decided to override the focused object.
                focusedObject = GazeManager.Instance.HitInfo.collider.gameObject;
            }
            else
            {
                // If our gaze doesn't hit a hologram, set the focused object to null or override focused object.
                focusedObject = OverrideFocusedObject;
            }

            if (focusedObject != oldFocusedObject)
            {
                // If the currently focused object doesn't match the old focused object, cancel the current gesture.
                // Start looking for new gestures.  This is to prevent applying gestures from one hologram to another.
                gestureRecognizer.CancelGestures();
                gestureRecognizer.StartCapturingGestures();
            }
        }

        void OnDestroy()
        {
            gestureRecognizer.StopCapturingGestures();
            gestureRecognizer.TappedEvent -= GestureRecognizer_TappedEvent;


            gestureRecognizer.ManipulationStartedEvent -= GestureRecognizer_ManipulationStartedEvent;
            gestureRecognizer.ManipulationUpdatedEvent -= GestureRecognizer_ManipulationUpdatedEvent;
            gestureRecognizer.ManipulationCompletedEvent -= GestureRecognizer_ManipulationCompletedEvent;
            gestureRecognizer.ManipulationCanceledEvent -= GestureRecognizer_ManipulationCanceledEvent;
        }

        private void GestureRecognizer_ManipulationStartedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            Debug.Log("Manipulation Started");
            // 2.b: Set IsNavigating to be true.
            IsNavigating = true;

            if (focusedObject != null)
            {
                focusedObject.SendMessage("OnSelect");
            }

            // 2.b: Set ManipulationPosition to be relativePosition.
            ManipulationPosition = relativePosition;
        }

        private void GestureRecognizer_ManipulationUpdatedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            // 2.b: Set IsNavigating to be true.
            IsNavigating = true;

            if (focusedObject != null)
            {
                focusedObject.SendMessage("OnSelect");
            }

            // 2.b: Set ManipulationPosition to be relativePosition.
            ManipulationPosition = relativePosition;
        }

        private void GestureRecognizer_ManipulationCompletedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            Debug.Log("Manipulation Completed");
            // 2.b: Set IsNavigating to be false.
            IsNavigating = false;
        }

        private void GestureRecognizer_ManipulationCanceledEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            Debug.Log("Manipulation Cancelled");
            // 2.b: Set IsNavigating to be false.
            IsNavigating = false;
        }
    }
}