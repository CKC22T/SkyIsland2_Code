using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

namespace Olympus
{
    public class CharacterPhysics_CharacterController : CharacterPhysics
    {
        #region Inspector
        [SerializeField, TabGroup("Component")] private CharacterController controller;
        [SerializeField, TabGroup("Option")] private float groundOffset = 0.1f;
        #endregion
        #region Property
        public CharacterController Controller { get { return controller; } }
        public override Vector3 Velocity { get => velocity; set => velocity = value; }
        public override Vector3 LateralVelocity { get { return new Vector3(Velocity.x, 0, Velocity.z); } set { Velocity = new Vector3(value.x, Velocity.y, value.z); } }
        public override Vector3 VerticalVelocity { get { return new Vector3(0, Velocity.y, 0); } set { Velocity = new Vector3(Velocity.x, value.y, Velocity.z); } }
        public Vector3 CenterPosition { get { return transform.position + controller.center; } }
        public Vector3 StepPosition { get { return CenterPosition - transform.up * (controller.height * 0.5f - controller.stepOffset); } }
        public float LastGroundTime { get => lastGroundTime; private set => lastGroundTime = value; }
        public override bool IsGround { get => isGround; protected set => isGround = value; }
        public override GameObject GroundObject { get => groundObject; protected set => groundObject = value; }
        public override float Radius { get => controller.radius; }
        public override float Height { get => controller.height; }
        public override Vector3 Center { get => controller.center; }
        
        #endregion
        #region Value
        [SerializeField, ReadOnly, TabGroup("Debug")] private bool isGround;
        [SerializeField, ReadOnly, TabGroup("Debug")] private GameObject groundObject;
        [SerializeField, ReadOnly, TabGroup("Debug")] private float lastGroundTime;
        [SerializeField, ReadOnly, TabGroup("Debug")] private Vector3 velocity;
        #endregion

        #region Event
        /// <summary>
        /// 초기화합니다.
        /// </summary>
        internal void Init()
        {
            OnInit();
        }

        protected void OnInit() { }
        protected bool OnLanding(RaycastHit hit) { return true; }
        protected void OnContact(Collider other) { }
        #endregion
        #region Function
        /// <summary>
        /// 현재 프레임의 물리를 업데이트합니다.
        /// </summary>
        public override void UpdatePhysics(float snap)
        {
            controller.Move(Velocity * Time.deltaTime);
            if (isGround && VerticalVelocity.y <= 0.0f && Physics.Raycast(StepPosition, Vector3.down, controller.stepOffset * 2))
                controller.Move(new Vector3(0, snap, 0) * Time.deltaTime);
            UpdateGround();
            UpdateCeiling();
        }
        /** 순간이동합니다. */
        public override void Warp(Vector3 position)
        {
            controller.enabled = false;
            transform.position = position;
            controller.enabled = true;
        }
        /** 가속합니다. */
        public override void Accelerate(Vector3 direction, float turningDrag, float acceleration, float topSpeed)
        {
            if (0 < direction.sqrMagnitude)
            {
                float speed = Vector3.Dot(direction, LateralVelocity);
                Vector3 velocity = direction * speed;
                Vector3 turningVelocity = LateralVelocity - velocity;
                float turningDelta = turningDrag * Time.deltaTime;
                speed += acceleration * Time.deltaTime;
                velocity = direction * speed;
                turningVelocity = Vector3.MoveTowards(turningVelocity, Vector3.zero, turningDelta);
                LateralVelocity = Vector3.ClampMagnitude(velocity + turningVelocity, topSpeed);
            }
        }
        public override void Decelerate(float deceleration)
        {
            float delta = deceleration * Time.deltaTime;
            LateralVelocity = Vector3.MoveTowards(LateralVelocity, Vector3.zero, delta);
        }
        public override void Gravity(float gravity)
        {
            if (!IsGround || 0 < VerticalVelocity.y)
            {
                VerticalVelocity += Vector3.up * gravity * Time.deltaTime;
            }
        }
        public override void Jump(float height)
        {
            VerticalVelocity = Vector3.up * height;
        }

        public void SetHeight(float height)
        {
            controller.height = height;
            controller.center += Vector3.up * (height - controller.height) * 0.5f;
        }
        public bool CapsuleCast(Vector3 direction, float distance, out RaycastHit hit, int layer = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            Vector3 origin = CenterPosition - direction * controller.radius + controller.center;
            Vector3 offset = transform.up * (controller.height * 0.5f - controller.radius);
            Vector3 top = origin + offset;
            Vector3 bottom = origin - offset;
            return Physics.CapsuleCast(top, bottom, controller.radius, direction, out hit, distance + controller.radius, layer, queryTriggerInteraction);
        }
        public override bool SphereCast(Vector3 direction, float distance, out RaycastHit hit, int layer = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            float castDist = distance - controller.radius;
            return Physics.SphereCast(CenterPosition, controller.radius, direction, out hit, castDist, layer, queryTriggerInteraction);
        }
        public int Overlap(Collider[] result, float skinOffset = 0)
        {
            float contactOffset = skinOffset + Controller.skinWidth + Physics.defaultContactOffset;
            float overlapsRadius = controller.radius + contactOffset;
            float offset = (controller.height + contactOffset) * 0.5f - overlapsRadius;
            Vector3 top = CenterPosition + Vector3.up * offset;
            Vector3 bottom = CenterPosition + Vector3.down * offset;
            return Physics.OverlapCapsuleNonAlloc(top, bottom, overlapsRadius, result);
        }
        //Function - Private
        private void UpdateGround()
        {
            float distance = (controller.height * 0.5f) + groundOffset;
            if (SphereCast(Vector3.down, distance, out var hit))
            {
                GroundObject = hit.collider.gameObject;
                if (!IsGround && OnLanding(hit) && VerticalVelocity.y <= 0)
                {
                    IsGround = true;
                    //GroundObject = hit.collider.gameObject;
                    VerticalVelocity = new Vector3(0, Mathf.Max(0, VerticalVelocity.y), 0);
                }
                if (IsGround)
                {
                    if (hit.point.y < StepPosition.y)
                    {
                        if (Physics.Raycast(CenterPosition, transform.up * -1, out var hit2, controller.height * 10, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                        {
                            if (Controller.slopeLimit <= Vector3.Angle(hit2.normal, Vector3.up))
                            {   //경사로에 닿음
                                Vector3 slopeDirection = Vector3.Cross(hit2.normal, Vector3.Cross(hit2.normal, Vector3.up)).normalized;
                                controller.Move(slopeDirection * -1 * Physics.gravity.y * Time.deltaTime);    //TODO 커스텀 가능하게 변경, 근데 어떤식으로??
                            }
                            else
                            {  //일반 바닥에 닿음
                            }
                        }
                    }
                    else
                    {  //지나치게 높은 경사에 닿음
                        Vector3 edgeNormal = hit.point - CenterPosition;
                        Vector3 edgePushDirection = Vector3.Cross(edgeNormal, Vector3.Cross(edgeNormal, Vector3.up));
                        controller.Move(edgePushDirection * -1 * Physics.gravity.y * Time.deltaTime);    //TODO 커스텀 가능하게 변경, 근데 어떤식으로??
                    }
                }
            }
            else if (IsGround)
            {
                IsGround = false;
                LastGroundTime = Time.time;
            }
        }
        private void UpdateCeiling()
        {
            float distance = (controller.height * 0.5f) - Controller.radius + 0.1f;
            if (Physics.SphereCast(CenterPosition, controller.radius, transform.up, out var hit, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (0 < VerticalVelocity.y)
                {  //천장에 부딛힘
                    VerticalVelocity = Vector3.zero;
                }
            }
        }
        #endregion
        #region Editor
#if UNITY_EDITOR
        protected virtual void Reset()
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorUtility.SetDirty(this);

            if (!controller)
                controller = GetComponent<CharacterController>();
        }
#endif
        #endregion
    }
}