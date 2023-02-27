using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public abstract class CharacterPhysics : MonoBehaviour
    {
        #region Property
        public abstract Vector3 Velocity { get; set; }
        public abstract Vector3 LateralVelocity { get; set; }
        public abstract Vector3 VerticalVelocity { get; set; }
        public abstract bool IsGround { get; protected set; }
        public abstract GameObject GroundObject { get; protected set; }
        public abstract float Radius { get; }
        public abstract float Height { get; }
        public abstract Vector3 Center { get; }
        #endregion

        #region Function
        public abstract void UpdatePhysics(float snap);
        public abstract void Warp(Vector3 position);
        public abstract void Accelerate(Vector3 direction, float turningDrag, float acceleration, float topSpeed);
        public abstract void Decelerate(float deceleration);
        public abstract void Gravity(float gravity);
        public abstract void Jump(float height);
        public abstract bool SphereCast(Vector3 direction, float distance, out RaycastHit hit, int layer = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore);
        #endregion
    }
}