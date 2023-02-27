using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Olympus
{
    public class BossController : SingletonBase<BossController>, IEntityController
    {
        public GameObject snowballPrefab;
        public GameObject snowStormPrefab;
        public GameObject iciclePrefab;
        public GameObject mountedAttackPrefab;

        public Dictionary<int, VFXController> ManagedVFX = new();
        public List<VFXController> VFXWaitList = new();
        public bool IsStatusChangeable {
            get; set;
        }
        public bool ActionLock {
            get; set;
        }
        public bool IsMountable;
        public bool IsGrabbed;
        public bool IsFlying;
        public bool IsReady = false;

        public bool SeekingFlag {
            get; set;
        }
        public bool IsAttacking { get; set; }
        private float targetDistance;
        public float TargetDistance {
            get { return targetDistance; }
        }

        private float directionAlign = 0.0f;
        public float Alignment {
            get { return directionAlign; }
        }
        public float ActionFactor {
            get { return actionFactor; }
        }

        private Vector3 targetDirection;
        public Vector3 TargetDirection {
            get { return targetDirection; }
        }

        public EntityBase bossEntity;
        private CharacterPhysics physicsComponent;
        private CharacterController characterController;

        static public Dictionary<int, List<int>> MoveableTileIndices = new();
        static public Transform[] LandingPoints;
        static public Transform RidingPoint;

        private List<EntityBase> previousSnowballs = new();
        public List<EntityBase> PreviousSnowballs { get { return previousSnowballs; } }
        public class PhaseSet
        {
            public PhaseSet(int hp, int atkDmg, float atkSpd, float movSpd, params SubPatterns[] supportedFeatures)
            {
                health = hp;
                attackDamage = atkDmg;
                attackSpeed = atkSpd;
                moveSpeed = movSpd;
                supportedPatterns = new();
                phaseLock = false;

                for (int i = 0; i < supportedFeatures.Length; i++)
                {
                    supportedPatterns.Add(supportedFeatures[i]);
                }
            }

            public int health;
            public int attackDamage;
            public float attackSpeed;
            public float moveSpeed;
            public bool phaseLock;

            public List<SubPatterns> supportedPatterns;
        };

        public class Pattern
        {
            public delegate void PatternMethod(BossController controller, EntityBase entity);

            public Pattern(float radius, float chance, PatternMethod act)
            {
                radiusCriteria = radius;
                method = act;
                probability = chance;
            }
            public float radiusCriteria;
            public float probability;
            public PatternMethod method;
        };

        public class SubPatterns
        {
            public Pattern[] patterns;
            public SubPatterns(params Pattern[] patternList)
            {
                patterns = new Pattern[patternList.Length];

                for (int i = 0; i < patternList.Length; i++)
                {
                    patterns[i] = patternList[i];
                }
            }
        }

        /// <summary>
        /// Debug Purposes
        /// </summary>
        private DeltaStopwatch chargingStopwatch = new DeltaStopwatch(1);

        private static int fieldSize = 50;

        public static PhaseSet nullPhase = new PhaseSet(0, 0, 0, 0);
        private PhaseSet currentPhase;
        // 405
        // 805
        // 305

        // 0.6, 0.9, 1.0

        public static PhaseSet[] predefinedPhaseSets = {                                //근접 시 기본 공격을 더 많이 하도록 지정, 원거리에서도 추적을 더 많이 시도한다.
            new PhaseSet(1500, 2, 3f, 2.5f,
                new SubPatterns(
                    new Pattern(4.0f, 0.66f, BasicAttackPattern),
                    new Pattern(fieldSize, 1.0f, ChargingAttackPattern)),
                    //new Pattern(fieldSize, 0.0f, BlizzardPattern),

                new SubPatterns(
                    new Pattern(fieldSize, 0.7f, MovingPattern),
                    new Pattern(fieldSize, 1.0f, ChargingAttackPattern))
                    //new Pattern(fieldSize, 0.0f, BlizzardPattern))
            ),

            new PhaseSet(4000, 2, 2.5f, 3.5f,
                new SubPatterns(
                        new Pattern(4.0f, 0.6f, BasicAttackPattern),                    // 눈보라를 발생시키는 패턴이 추가됨. 원거리에서 돌진과 눈보라를 더 많이 시도하도록 변경
                        new Pattern(fieldSize, 0.8f, ChargingAttackPattern),
                        new Pattern(fieldSize, 1.0f, BlizzardPattern)),
                new SubPatterns(
                        new Pattern(fieldSize, 0.3f, MovingPattern),
                        new Pattern(fieldSize, 0.5f, ChargingAttackPattern),
                        new Pattern(fieldSize, 1.0f, BlizzardPattern)
                    )
                ),

            new PhaseSet(3200, 3, 2f, 5f,                                                // 근거리에서 정신 나간 것처럼 돌진을 반복, 원거리에서는 눈보라를 매우 높은 확률로 시전한다.
                new SubPatterns(
                        new Pattern(4.0f, 0.5f, BasicAttackPattern),
                        new Pattern(fieldSize, 0.8f, ChargingAttackPattern),
                        new Pattern(fieldSize, 1.0f, BlizzardPattern)
                    ),
                new SubPatterns(
                        new Pattern(fieldSize, 0.2f, ChargingAttackPattern),
                        new Pattern(fieldSize, 1.0f, BlizzardPattern)
                    )
                )
        };

        static readonly int attackHash = "BasicAttackPattern".GetHashCode();
        static readonly int blizzardHash = "BlizzardPattern".GetHashCode();
        static readonly int chargingHash = "ChargingAttackPattern".GetHashCode();
        static readonly int flyingHash = "FlyingPattern".GetHashCode();
        static readonly int movingHash = "MovingPattern".GetHashCode();

        public static readonly Vector3 INIT_POSITION = new Vector3(183.40094f, 109.847084f, 192.031372f);
        public static readonly Vector3 INIT_ROTATION = new Vector3(-1.52838129e-05f, 102.478035f, 1.54942682e-05f);

        private int phaseLevel = 2;
        public int Phase {
            get { return phaseLevel; }
        }

        private int patternType = 1;
        public int PatternType {
            get { return patternType - 1; }
        }

        public DeltaTimer snowballDurationTimer = new(3.0f);
        public DeltaTimer snowballIntervalTimer = new(1.0f);
        public DeltaTimer snowballDamageTimer = new(2.0f);
        public DeltaTimer flyingIcicleTimer = new(8.0f);
        public DeltaTimer flyingIcicleIntervalTimer = new(0.25f);
        public DeltaTimer flyingIcicleFallingDuration = new(1.0f);

        public Pattern.PatternMethod selectedPatternMethod;
        public Pattern.PatternMethod CurrentPatternMethod {
            get { return selectedPatternMethod; }
        }

        static bool IsAligned(float solution, float threshold = 0.3f)
        {
            return solution >= (1.0f - threshold);
        }

        public void PlayVFX(string name)
        {
            int hash = name.GetHashCode();
            var particle = ManagedVFX.GetValueOrDefault(hash);

            if (particle.target.gameObject.activeInHierarchy == false)
            {
                particle.target.gameObject.SetActive(true);
            }


            if (particle.target.isPlaying == true)
            {
                particle.target.time = 0.0f;
                VFXWaitList.Remove(particle);
            }

            particle.target.Play();

            VFXWaitList.Add(particle);
        }

#if UNITY_EDITOR
        [Button()]
        void Interrupt()
        {
            selectedPatternMethod = BossController.ChargingAttackPattern;
            selectedPatternMethod(this, bossEntity);
            selectedPatternMethod = BossController.BasicAttackPattern;
            selectedPatternMethod(this, bossEntity);
        }
#endif

        static private void BasicAttackPattern(BossController controller, EntityBase entity)
        {
            if (controller.ActionLock == true || entity.ActionType == ActionType.Skill)
            {
                ResetStates();
                controller.IsStatusChangeable = true;
                return;
            }

            PhaseSet currentPhase = BossController.predefinedPhaseSets[controller.Phase];
            float radius = 4.0f;

            if (IsAligned(controller.directionAlign) == false && controller.IsStatusChangeable == true)
            {
                entity.EntityData.moveDirection = controller.TargetDirection;
                entity.EntityRotate();
            }
            else if (IsAligned(controller.directionAlign) == true && controller.IsStatusChangeable == true)
            {
                if (controller.TargetDistance <= radius)
                {
                    entity.SetActionType(ActionType.Attack);
                    controller.ActionLock = true;
                }
            }
            else
            {
                //if (controller.IsAttacking == false)
                //{
                //    controller.IsStatusChangeable = true;
                //}
            }
        }

        static public void MountedFlyingPattern(BossController controller, EntityBase entity)
        {
            return;
        }

        static public void AlertMovingPattern(BossController controller, EntityBase entity)
        {
            return;
        }

        static private void BlizzardPattern(BossController controller, EntityBase entity)
        {
            if (entity.ActionType != ActionType.SecondarySkill)
            {
                entity.SetActionType(ActionType.SecondarySkill);
                controller.snowballDurationTimer.Reset();
                controller.snowballIntervalTimer.Reset();
                controller.ActionLock = true;
            }

            controller.snowballDurationTimer.Tick();
            controller.snowballIntervalTimer.Tick();

            if (controller.snowballIntervalTimer.IsDone == true)
            {
                // throw snowball
                GameObject snowball = GameObjectPoolManager.Instance.CreateGameObject(controller.snowballPrefab, Vector3.zero, Quaternion.identity);
                snowball.transform.localScale = Vector3.zero;
                snowball.transform.position = entity.transform.position + new Vector3(0.0f, 8.0f, 0.0f);
                controller.snowballIntervalTimer.Reset();
            }

            if (controller.snowballDurationTimer.IsDone == true)
            {
                // end pattern
                controller.IsStatusChangeable = true;
                controller.ActionLock = false;
            }

            return;
        }

        const float chargeDistance = 20.0f;
        const float stoppingDistance = 5.0f;
        Vector3 startPosition;
        Vector3 positionBuffer = Vector3.zero;
        float originalSpeed;
        public float endPointDistance;
        public bool IsChargeStarted = false;
        public float movedDistance = 0.0f;
        bool chargeFlag = false;
        LayerMask chargeAttackLayerMask;
        DeltaTimer chargingDeterantTimer = new DeltaTimer(3.0f);
        static private void ChargingAttackPattern(BossController controller, EntityBase entity)
        {
            if (entity.ActionType == ActionType.Attack)
            {
                ResetStates();
                controller.IsStatusChangeable = true;
                return;
            }

            if (entity.ActionType != ActionType.Skill)
            {
                ResetStates();
                Transform target = PlayerController.Instance.PlayerEntity.transform;
                Vector3 targetXZPos = new Vector3(target.position.x, entity.transform.position.y, target.position.z);
                Vector3 targetDir = (targetXZPos - entity.transform.position).normalized;
                float xzAlignment = Vector3.Dot(entity.transform.forward, targetDir);
                controller.SeekingFlag = false;
                controller.chargeFlag = false;
                controller.ActionLock = true;
                controller.IsStatusChangeable = false;

                if (IsAligned(xzAlignment, 0.01f) == false && controller.chargeFlag == false)
                {
                    entity.EntityData.moveDirection = controller.targetDirection;
                    entity.EntityRotate();
                }
                else
                {
                    entity.SetActionType(ActionType.Skill);
                    entity.EntityAnimator.SetTrigger("StartCharging");

                    controller.IsStatusChangeable = false;

                    controller.startPosition = entity.transform.position;
                    controller.movedDistance = 0.0f;
                    controller.positionBuffer = entity.transform.position;
                    entity.EntityData.moveDirection = controller.targetDirection;
                    entity.EntityData.moveSpeed = 640.0F;
                }
            }

            RaycastHit hitInfo;

            if (controller.IsStatusChangeable == false)
            {
                if (controller.IsChargeStarted == true)
                {
                    Ray r = new Ray(entity.transform.position + (-entity.transform.forward * 10.0f), entity.transform.forward);
                    if (Physics.SphereCast(r, 8.0f, out hitInfo, 10.0f, Instance.chargeAttackLayerMask, QueryTriggerInteraction.UseGlobal) == true)
                    {
                        EntityBase targetEntity = PlayerController.Instance.PlayerEntity;
                        if (hitInfo.collider.gameObject == PlayerController.Instance.PlayerEntity.gameObject)
                        {
                            //Physics.IgnoreLayerCollision(1 << LayerMask.NameToLayer("Player"), 1 << LayerMask.NameToLayer("Enemy"));
                            //PlayerController.Instance.IsControlLocked = true;
                            targetEntity.GetDamage(1);
                            targetEntity.EntityData.moveDirection = entity.transform.forward;
                            targetEntity.EntityMove(0.0f, 50.0f * 10.0f, 100.0f, false);
                        }
                    }

                    if (controller.movedDistance < chargeDistance && controller.chargingDeterantTimer.IsDone == false)
                    {
                        controller.chargeFlag = true;
                        controller.chargingStopwatch.Tick();
                        entity.EntityData.moveDirection = entity.transform.forward;
                        entity.EntityMove(1.0f, 120.0f, 500.0f);
                        controller.movedDistance += Vector3.Distance(entity.transform.position, controller.positionBuffer);
                        controller.positionBuffer = entity.transform.position;

                        controller.chargingDeterantTimer.Tick();
                    }
                    else
                    {

                        float lastedTick = controller.chargingStopwatch.GetLatestTick();
                        controller.chargingStopwatch.Reset();

                        entity.EntityData.moveDirection = controller.targetDirection;
                        entity.EntityAnimator.SetTrigger("FinishCharging");
                        controller.PlayVFX("Boss_Rush_Break_VFX");

                        entity.EntityData.moveSpeed = BossController.predefinedPhaseSets[controller.Phase].moveSpeed;
                        controller.IsChargeStarted = false;

                        controller.chargingDeterantTimer.Reset();
                    }
                }

            }

            return;
        }

        static readonly Vector3[] axis = {
                new Vector3(-1.0f, 0.0f, -1.0f),
                new Vector3(1.0f, 0.0f, -1.0f),
                new Vector3(-1.0f, 0.0f, 1.0f),
                new Vector3(1.0f, 0.0f, 1.0f)};

        bool foundSafePoint = false;
        static private void FlyingGroundCharging(BossController controller, EntityBase entity)
        {
            // 임시 코드
            if (controller.foundSafePoint == false)
            {
                // seeking safe point
                for (int i = 0; i < 4; i++)
                {
                    Vector3 direction = axis[i];

                    const float stompDistance = 20.0f; // MAGIC NUMBER!!!
                    if (Physics.Raycast(entity.transform.position, direction, stompDistance) == false)
                    {
                        // found safe point
                        entity.transform.position += direction * stompDistance;
                        controller.foundSafePoint = true;
                        break;
                    }
                }
            }

            Transform target = PlayerController.Instance.PlayerEntity.transform;
            Vector3 targetXZPos = new Vector3(target.position.x, entity.transform.position.y, target.position.z);
            Vector3 targetDir = (targetXZPos - entity.transform.position).normalized;
            float xzAlignment = Vector3.Dot(entity.transform.forward, targetDir);

            if (IsAligned(xzAlignment, 0.3f) == false)
            {
                entity.EntityData.moveDirection = controller.targetDirection;
                entity.EntityRotate();
            }
            else
            {
                if (entity.ActionType != ActionType.GroundCharging)
                {
                    entity.SetActionType(ActionType.GroundCharging);
                }
            }

            return;
        }

        static private void FlyingPattern(BossController controller, EntityBase entity)
        {
            for (int i = 0; i < controller.previousSnowballs.Count; i++)
            {
                var snowball = controller.previousSnowballs[i].GetComponent<SnowballActionIdle>();
                controller.previousSnowballs.RemoveAt(i);
                snowball?.Explode();
            }

            controller.previousSnowballs.Clear();

            if (entity.ActionType != ActionType.Fly)
            {
                entity.SetActionType(ActionType.Fly);
                controller.flyingIcicleTimer.Reset(false);
                controller.flyingIcicleFallingDuration.Reset(false);
                controller.flyingIcicleIntervalTimer.Reset(false);

                controller.IsStatusChangeable = false;
                controller.IsMountable = true;

                entity.UseGravity = false;
                controller.characterController.detectCollisions = true;
            }

            controller.flyingIcicleTimer.Tick();
            controller.flyingIcicleIntervalTimer.Tick();

            if (controller.flyingIcicleTimer.IsDone == true)
            {
                controller.selectedPatternMethod = FlyingGroundCharging;
            }

            if (controller.flyingIcicleIntervalTimer.IsDone == true)
            {
                GameObject newIcicle = GameObject.Instantiate(controller.iciclePrefab);
                newIcicle.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                newIcicle.transform.position = entity.transform.position;

                controller.flyingIcicleIntervalTimer.Reset();
            }

            return;
        }

        static public void MovingPattern(BossController controller, EntityBase entity)
        {
            if (entity.ActionType != ActionType.Move)
            {
                entity.EntityAnimator.SetBool("IsMoving", true);
                entity.SetActionType(ActionType.Move);
            }

            return;
        }

        // 액션 무작위 추첨
        float actionFactor = 0.0f;
        private void ApplyPhase(EntityData data, PhaseSet phase, int phaseIndex)
        {
            if (currentPhase != phase)
            {
                currentPhase = phase;
                data.health = phase.health;
                data.maxHealth = phase.health;
                data.attackSpeed = phase.attackSpeed;
                data.attackDamage = phase.attackDamage;
                data.moveSpeed = phase.moveSpeed;
                phaseLevel = phaseIndex;
            }

            int patternCount = phase.supportedPatterns.Count;

            if (ActionLock == false)
            {
                if (targetDistance < 4.0f)
                {
                    if (IsStatusChangeable == true && ActionLock == false)
                    {
                        int seed = new System.Random().Next();
                        UnityEngine.Random.InitState(seed);
                        LogUtil.Log(seed);
                        actionFactor = UnityEngine.Random.Range(0.0f, 1.0f);
                        ResetStates();

                    }

                    float previousProbability = phase.supportedPatterns[0].patterns[0].probability;

                    if (previousProbability > actionFactor && IsStatusChangeable == true && ActionLock == false)
                    {
                        selectedPatternMethod = phase.supportedPatterns[0].patterns[0].method;
                    }
                    else
                    {
                        for (int i = 1; i < phase.supportedPatterns[0].patterns.Length; i++)
                        {
                            if (IsStatusChangeable == false || ActionLock == true)
                            {
                                break;
                            }

                            float targetProbability = phase.supportedPatterns[0].patterns[i].probability;

                            if (targetProbability == 0.0f)
                            {
                                continue;
                            }

                            if (previousProbability < actionFactor && actionFactor < targetProbability)
                            {
                                patternType = i;
                                selectedPatternMethod = phase.supportedPatterns[0].patterns[i].method;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (IsStatusChangeable == true && ActionLock == false)
                    {
                        int seed = new System.Random().Next();
                        UnityEngine.Random.InitState(seed);
                        LogUtil.Log(seed);
                        actionFactor = UnityEngine.Random.Range(0.0f, 1.0f);
                        ResetStates();
                    }

                    float previousProbability = phase.supportedPatterns[1].patterns[0].probability;

                    if (previousProbability > actionFactor && IsStatusChangeable == true && ActionLock == false)
                    {
                        selectedPatternMethod = phase.supportedPatterns[1].patterns[0].method;
                    }
                    else
                    {
                        for (int i = 1; i < phase.supportedPatterns[1].patterns.Length; i++)
                        {
                            if (IsStatusChangeable == false || ActionLock == true)
                            {
                                break;
                            }

                            float targetProbability = phase.supportedPatterns[1].patterns[i].probability;

                            if (targetProbability == 0.0f)
                            {
                                continue;
                            }

                            if (previousProbability < actionFactor && actionFactor < targetProbability)
                            {
                                patternType = i;
                                selectedPatternMethod = phase.supportedPatterns[1].patterns[i].method;
                                break;
                            }
                        }
                    }
                }
            }

            if (selectedPatternMethod != null)
            {
                selectedPatternMethod(BossController.Instance, bossEntity);
            }
        }
        void IEntityController.ConnectController(EntityBase entity)
        {
            ResetStates();
            phaseLevel = 0;
            currentPhase = null;
            //currentPhase = predefinedPhaseSets[0];
            IsStatusChangeable = true;
            ActionLock = false;
            IsAttacking = false;
            IsMountable = false;
            IsGrabbed = false;
            IsFlying = false;
            IsChargeStarted = false;
            IsReady = false;

            for (int i = 0; i < predefinedPhaseSets.Length; i++)
            {
                predefinedPhaseSets[i].phaseLock = false;
            }

            MoveableTileIndices.Clear();
            ManagedVFX.Clear();

            chargeAttackLayerMask = 1 << LayerMask.NameToLayer("Player");
            for (int i = 0; i < 4; i++)
            {
                if(MoveableTileIndices.ContainsKey(i) == false)
                {
                    MoveableTileIndices.Add(i, new List<int>());
                }
            }

            MoveableTileIndices[0].Add(1);
            MoveableTileIndices[0].Add(2);

            MoveableTileIndices[3].Add(1);
            MoveableTileIndices[3].Add(2);

            MoveableTileIndices[1].Add(0);
            MoveableTileIndices[1].Add(3);

            MoveableTileIndices[2].Add(0);
            MoveableTileIndices[2].Add(3);

            bossEntity = entity;

            var phase = predefinedPhaseSets[0];

            if (currentPhase != phase)
            {
                currentPhase = phase;
                entity.EntityData.health = phase.health;
                entity.EntityData.maxHealth = phase.health;
                entity.EntityData.attackSpeed = phase.attackSpeed;
                entity.EntityData.attackDamage = phase.attackDamage;
                entity.EntityData.moveSpeed = phase.moveSpeed;
                phaseLevel = 0;
            }
            //   ApplyPhase(entity.EntityData, predefinedPhaseSets[1], 1);
            //   ApplyPhase(entity.EntityData, predefinedPhaseSets[0], 0);
            // ApplyPhase(entity.EntityData, predefinedPhaseSets[1], 1);
            // ApplyPhase(entity.EntityData, predefinedPhaseSets[0], 0);

            characterController = entity.GetComponent<CharacterController>();

            IsStatusChangeable = true;
            SeekingFlag = true;

            snowballPrefab = Resources.Load("Temp/Snowball") as GameObject;
            snowStormPrefab = Resources.Load("Temp/TempSnowStormObject") as GameObject;
            iciclePrefab = Resources.Load("Temp/TempIcicleObject") as GameObject;
            mountedAttackPrefab = Resources.Load("Temp/Attack_Mounted_Damager_Effect") as GameObject;
            currentPhase = predefinedPhaseSets[Phase];

            Transform model = entity.transform.Find("EntityModel").Find("Mon_Boss_Flying_idle_v2");
            Transform bip = model.Find("Bip001");
            Transform pelvis = bip.Find("Bip001 Pelvis");
            Transform spine0 = pelvis.Find("Bip001 Spine");
            Transform spine1 = spine0.Find("Bip001 Spine1");
            Transform spine2 = spine1.Find("Bip001 Spine2");
            Transform neck = spine2.Find("RidingPoint");
            RidingPoint = neck;

            Dictionary<int, float> vfxHashList = new();
            KeyValuePair<int, float>[] vfxDurationList =
            {
                new KeyValuePair<int, float>("Boss_Rush".GetHashCode(), 1.72f),
                new KeyValuePair<int, float>("Boss_Rush_Break_VFX".GetHashCode(), 1.3f),
                new KeyValuePair<int, float>("Boss_Rush_Fly_VFX".GetHashCode(), 2.0f),
                new KeyValuePair<int, float>("Boss_Rush_L_WIND_VFX".GetHashCode(), 2.1f),
                new KeyValuePair<int, float>("Boss_Rush_R_WIND_VFX".GetHashCode(), 2.1f),
                new KeyValuePair<int, float>("Blizard_Roar_VFX".GetHashCode(), 4.0f),
                new KeyValuePair<int, float>("Boss_Attack_VFX".GetHashCode(), 1.6f),
                new KeyValuePair<int, float>("Boss_L_Wing_Trail_VFX".GetHashCode(), 1.72f),
                new KeyValuePair<int, float>("Boss_R_Wing_Trail_VFX".GetHashCode(), 1.72f),
                new KeyValuePair<int, float>("Boss_SnowStorm_VFX".GetHashCode(), 7.0f),
                new KeyValuePair<int, float>("Boss_Stomp_Land_VFX".GetHashCode(), 2.0f),
                new KeyValuePair<int, float>("Boss_Stomp_VFX".GetHashCode(), 1.1f),
            };

            foreach (var i in vfxDurationList)
            {
                vfxHashList.Add(i.Key, i.Value);
            }

            ParticleSystem[] vfxRawList;

            vfxRawList = entity.GetComponentsInChildren<ParticleSystem>();
            foreach (var i in vfxRawList)
            {
                if(i.gameObject == null)
                {
                    break;
                }

                int vfxHash = i.gameObject.name.GetHashCode();

                float duration;

                if (vfxHashList.TryGetValue(vfxHash, out duration) == true)
                {
                    VFXController vfxController = new VFXController(i, duration);
                    ManagedVFX.Add(i.gameObject.name.GetHashCode(), vfxController);
                    i.gameObject.SetActive(false);
                }
            }

        }

        private void OnDrawGizmos()
        {
            if (bossEntity == null)
            {
                return;
            }

            if (bossEntity.ActionType == ActionType.Skill)
            {
                Vector3 forward = bossEntity.transform.forward;
                Gizmos.color = new Color(1, 0, 0, 0.5f);

                for (int i = 0; i < 5; i++)
                {
                    Gizmos.DrawSphere((bossEntity.transform.position + (-bossEntity.transform.forward * 5.0f)) + (forward * i), 7.0f);
                }
            }
        }

        void IEntityController.ControlEntity(EntityBase entity)
        {
            if (IsReady == false)
            {
                entity.transform.localPosition = INIT_POSITION;
                entity.transform.localRotation = Quaternion.Euler(INIT_ROTATION);
                entity.EntityData.godModeTimer = float.PositiveInfinity;
                ResetStates();
                return;
            }

            entity.EntityData.godModeTimer = 0;

            EntityData data = entity.EntityData;
            Transform playerTransform = PlayerController.Instance.PlayerEntity.transform;
            Transform bossTransform = entity.transform;

            targetDistance = Vector3.Distance(entity.transform.position, PlayerController.Instance.PlayerEntity.transform.position);
            targetDirection = (playerTransform.position - bossTransform.position).normalized;
            directionAlign = Vector3.Dot(entity.transform.forward, TargetDirection);

            if (phaseLevel == 1 && predefinedPhaseSets[1].phaseLock == false)
            {
                // 2페이즈 공중 발동 조건
                ActionLock = true;
                selectedPatternMethod = FlyingPattern;
                predefinedPhaseSets[1].phaseLock = true;
            }
            else if (phaseLevel == 2 && predefinedPhaseSets[2].phaseLock == false)
            {
                // 3페이즈 공중 발동 조건
                ActionLock = true;
                selectedPatternMethod = FlyingPattern;
                predefinedPhaseSets[2].phaseLock = true;
            }

            // phase 식별 구간
            int phaseCount = predefinedPhaseSets.Length;
            if (data.health <= 5)
            {
                ResetStates();

                if (phaseLevel == 2)
                {
                    data.health = 0;
                    if (entity.ActionType != ActionType.Dead)
                    {
                        entity.SetActionType(ActionType.Dead);
                        selectedPatternMethod = null;
                        LogUtil.Log("You just beat the boss!");
                    }
                    return;
                }
                else
                {
                    ++phaseLevel;

                    data.health = predefinedPhaseSets[phaseLevel].health;
                    data.maxHealth = predefinedPhaseSets[phaseLevel].health;
                    data.attackSpeed = predefinedPhaseSets[phaseLevel].attackSpeed;
                    data.attackDamage = predefinedPhaseSets[phaseLevel].attackDamage;
                    data.moveSpeed = predefinedPhaseSets[phaseLevel].moveSpeed;
                }
            }

            ApplyPhase(data, predefinedPhaseSets[phaseLevel], phaseLevel);

            for (int i = 0; i < VFXWaitList.Count; i++)
            {
                if(VFXWaitList[i].target == null)
                {
                    continue;
                }
                if (VFXWaitList[i].target.time >= VFXWaitList[i].PlaybackTime)
                {
                    VFXWaitList[i].target.time = 0.0f;
                    VFXWaitList[i].target.gameObject.SetActive(false);
                    VFXWaitList.Remove(VFXWaitList[i]);
                }
            }
        }

        public void RemoveTile(int tileIndex)
        {
            MoveableTileIndices.Remove(tileIndex);
            foreach (var t in MoveableTileIndices)
            {
                if (t.Value.Contains(tileIndex) == true)
                {
                    t.Value.Remove(tileIndex);
                }
            }
        }

        static public void ResetStates()
        {
            Instance.ActionLock = false;
            Instance.IsAttacking = false;
            Instance.IsGrabbed = false;
            Instance.IsMountable = false;
            if (Instance.bossEntity == null)
            {
                return;
            }

            Instance.bossEntity.EntityData.moveSpeed = 5.0f;
        }

        void IEntityController.DisconnectController(EntityBase entity)
        {
            MoveableTileIndices.Clear();
            ManagedVFX.Clear();
        }
    }
}