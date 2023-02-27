using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor.Validation;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Olympus
{
    /// <summary>
    /// 캐릭터가 공중에 떠있는지&떠있는 이유
    /// </summary>
    public enum ECharacterState
    {
        /// <summary>
        /// 지상에 있는중 - 이동 & 점프 가능
        /// </summary>
        Ground,
        /// <summary>
        /// 미끄러지는중 - 행동 불가
        /// </summary>
        Slide,
        /// <summary>
        /// 공중에서 떨어지는중 - 이동 & 점프 가능
        /// </summary>
        Fall,
        /// <summary>
        /// 점프중 - 이동 & 점프 가능
        /// </summary>
        Jump,
    }

    public class CharacterPhysics_Rigidbody : CharacterPhysics
    {
        #region Type
        /// <summary>
        /// 인접한 바닥에 대한 데이터
        /// </summary>
        private struct SGroundData
        {
            public Vector3 position;
            public Vector3 normal;

            public SGroundData(Collision _col)
            {
                position = Vector3.zero;
                normal = Vector3.zero;
                foreach (var v in _col.contacts)
                {
                    position += v.point;
                    normal += v.normal;
                }
                position /= _col.contactCount;
                normal.Normalize();
            }
        }
        #endregion

        #region Inspector
        [SerializeField, TabGroup("Component"), LabelText("Rigidbody")] private Rigidbody m_Rigidbody;
        [SerializeField, TabGroup("Component"), LabelText("Collider")] private CapsuleCollider m_Collider;
        [SerializeField, TabGroup("Component"), LabelText("발 기준점")] private Transform m_FootTransform;

        [SerializeField, TabGroup("Physics"), LabelText("기본 머테리얼")] private PhysicMaterial m_DefaultPhysicsMaterial;
        [SerializeField, TabGroup("Physics"), LabelText("이동시 머테리얼")] private PhysicMaterial m_MovePhysicsMaterial;

        [SerializeField, TabGroup("Option"), LabelText("자동 초기화")] private bool m_IsAutoInit = true;
        [SerializeField, TabGroup("Option"), LabelText("최대 이동속도")] private float m_MoveSpeed;
        [SerializeField, TabGroup("Option"), LabelText("0->최대이속 시간")] private float m_MoveMaxSec;
        [SerializeField, TabGroup("Option"), LabelText("최대이속->0 시간")] private float m_MoveMinSec;
        [SerializeField, TabGroup("Option"), LabelText("공중에서 이속 변경시간")] private float m_FlyMoveChangeSec;
        [SerializeField, TabGroup("Option"), LabelText("미끄러지는 각도")] private float m_SlideAngle;
        [SerializeField, TabGroup("Option"), LabelText("넉백 기절 시간")] private float m_KnockbackStunTime;
        [SerializeField, TabGroup("Option"), LabelText("넉백 커브 시간")] private float m_KnockbackCurveTime;
        [SerializeField, TabGroup("Option"), LabelText("넉백 지상속도 커브")] private AnimationCurve m_KnockbackCurve;
        #endregion
        #region Get,Set
        //Component
        /// <summary>
        /// 캐릭터 Rigidbody, 왠만하면 접근하지 말것!!!
        /// </summary>
        public Rigidbody TargetRigidbody { get => m_Rigidbody; }
        /// <summary>
        /// 캐릭터 Collider, 왠만하면 접근하지 말것!!!
        /// </summary>
        public CapsuleCollider TargetCollider { get => m_Collider; }
        //State
        /// <summary>
        /// 캐릭터 상태
        /// </summary>
        public ECharacterState State { get => m_State; }
        /// <summary>
        /// 캐릭터가 현재 땅위에 있는지
        /// </summary>
        public override bool IsGround { get => 0 < m_GroundData.Count; protected set { } }
        /// <summary>
        /// 캐릭터가 어떤 물체 위에 있는지
        /// </summary>
        public override GameObject GroundObject { get => m_GroundData.First().Key.gameObject; protected set => throw new System.NotImplementedException(); }
        /// <summary>
        /// 캐릭터가 이동이 가능한 상태인지
        /// </summary>
        public bool IsMoveEnable { get => m_State == ECharacterState.Ground || m_State == ECharacterState.Fall || m_State == ECharacterState.Jump; }
        /// <summary>
        /// 캐릭터가 점프가 가능한 상태인지
        /// </summary>
        public bool IsJumpEnable { get => m_State == ECharacterState.Ground || m_State == ECharacterState.Fall; }
        public override Vector3 Velocity { get => new Vector3(m_Velocity.x, m_Rigidbody.velocity.y, m_Velocity.z); set { LateralVelocity = value; VerticalVelocity = value; } }
        public override Vector3 LateralVelocity { get { return new Vector3(m_Velocity.x, 0, m_Velocity.z); } set { m_Velocity = new Vector3(value.x, 0, value.z); } }
        public override Vector3 VerticalVelocity { get { return new Vector3(0, m_Rigidbody.velocity.y, 0); } set { m_Rigidbody.velocity = new Vector3(m_Velocity.x, value.y, m_Velocity.z); } }
        //Option
        /// <summary>
        /// 이동속도
        /// </summary>
        public float MoveSpeed { get => m_MoveSpeed; set => m_MoveSpeed = value; }
        public override float Radius { get => m_Collider.radius; }
        public override float Height { get => m_Collider.height; }
        public override Vector3 Center { get => m_Collider.center; }
        public Vector3 CenterPosition { get { return transform.position + TargetCollider.center; } }
        #endregion
        #region Value
        private ECharacterState m_State = ECharacterState.Fall;
        private Dictionary<Collider, SGroundData> m_GroundData = new Dictionary<Collider, SGroundData>();   //현재 인접한 바닥 콜라이더 및 관련 정보
        [SerializeField, ReadOnly] protected bool m_IsMoved;                                   //이번 프레임에 이동을 했는지 여부
        [SerializeField, ReadOnly] protected Vector2 m_MoveVelocity;                           //목표 이동방향
        protected float m_MoveEndTimer;                             //이 시간 이상 지나면 강제로 멈추도록 함
        protected float m_KnockbackTimer;                           //넉백 최소 시간 (바닥에서 넉백시)
        protected Vector3 m_KnockbackPower;                         //넉백 세기 (바닥에서 넉백시)
        private Vector3 m_Velocity;
        #endregion

        #region Event
        public void Initialize()
        {
        }

        //Unity Event
        private void Awake()
        {
            if (m_IsAutoInit)
                Initialize();
        }
        private void FixedUpdate()
        {
            //Debug.Log(Velocity);
            m_Rigidbody.velocity = Velocity;
        }
        private void OnCollisionEnter(Collision _col)
        {
            SGroundData data = new SGroundData(_col);
            if (data.position.y < m_FootTransform.position.y)
                SetGround(_col.collider, data);
        }
        private void OnCollisionStay(Collision _col)
        {
            bool isNotJumping = m_State != ECharacterState.Jump || TargetRigidbody.velocity.y <= 0;
            if (isNotJumping)
            {
                SGroundData data = new SGroundData(_col);
                if (data.position.y < m_FootTransform.position.y)
                    SetGround(_col.collider, data);
                else
                    RemoveGround(_col.collider);
            }
        }
        private void OnCollisionExit(Collision collision)
        {
            RemoveGround(collision.collider);
        }
        #endregion
        #region Function - Public
        //Public
        public override void UpdatePhysics(float snap)
        {
            if (IsGround && VerticalVelocity.y <= 0.0f)
                VerticalVelocity = new Vector3(0, snap, 0) * Time.deltaTime;

            //State 업데이트
            if (ECharacterState.Fall <= m_State)
                m_State = (m_GroundData.Count == 0) ? m_State : ECharacterState.Ground;
            if (m_State <= ECharacterState.Slide)
            {
                if (m_GroundData.Count == 0)
                    m_State = ECharacterState.Fall;
                else
                {
                    var normal = Vector3.zero;
                    foreach (var v in m_GroundData.Values)
                        normal += v.normal;
                    m_State = (Vector3.Angle(Vector3.up, normal.normalized) < m_SlideAngle) ? ECharacterState.Ground : ECharacterState.Slide;
                    var velocity = m_Rigidbody.velocity;
                    velocity.y = Mathf.Min(0, m_Rigidbody.velocity.y);
                    m_Rigidbody.velocity = velocity;
                }
            }
            m_IsMoved = false;
        }
        public override void Warp(Vector3 position)
        {
            TargetRigidbody.MovePosition(position);
        }
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
                m_IsMoved = true;                  //이번 프레임에 이동했다고 한다.
                m_MoveEndTimer = m_MoveMinSec;              //이동 타이머를 초기화한다.
                m_Collider.sharedMaterial = m_MovePhysicsMaterial;                                 //이동 재질로 변경한다.
            }
            //Debug.Log("acc" + Velocity);
        }
        public override void Decelerate(float deceleration)
        {
            float delta = deceleration * Time.deltaTime;
            LateralVelocity = Vector3.MoveTowards(LateralVelocity, Vector3.zero, delta);
            if (LateralVelocity.sqrMagnitude <= 0.01)
                m_Collider.sharedMaterial = m_MovePhysicsMaterial;
            else
                m_Collider.sharedMaterial = m_DefaultPhysicsMaterial;
            //d Debug.Log("dec");
        }
        public override void Gravity(float gravity)
        {
            if (!IsGround || 0 < VerticalVelocity.y)
                VerticalVelocity += Vector3.up * (gravity - Physics.gravity.y) * Time.deltaTime;
        }
        public override void Jump(float height)
        {
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, height, m_Rigidbody.velocity.z);    //velocity를 설정한다.
            m_State = ECharacterState.Jump;
            ClearGround();
        }
        public override bool SphereCast(Vector3 direction, float distance, out RaycastHit hit, int layer = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            float castDist = distance - Radius;
            return Physics.SphereCast(CenterPosition, Radius, direction, out hit, castDist, layer, queryTriggerInteraction);
        }
        #endregion
        #region Function - Private
        /// <summary>
        /// 밟고있는 바닥을 추가합니다.
        /// </summary>
        /// <param name="col">콜라이더</param>
        private void SetGround(Collider _col, SGroundData _data)
        {
            if (!m_GroundData.ContainsKey(_col))
                m_GroundData.Add(_col, _data);
            else
                m_GroundData[_col] = _data;
        }
        /// <summary>
        /// 밟고 있었던 바닥을 제거합니다.
        /// </summary>
        /// <param name="col"></param>
        private void RemoveGround(Collider col)
        {
            m_GroundData.Remove(col);
        }
        /// <summary>
        /// 현재 밟고있는 것으로 등록된 바닥을 전부 제거합니다.
        /// </summary>
        private void ClearGround()
        {
            m_GroundData.Clear();
        }
        #endregion
    }
}