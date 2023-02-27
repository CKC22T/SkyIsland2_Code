using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

namespace Olympus
{
    public class BossActionMountedFly : ActionBase
    {
        BossController controller;
        public override string ActionName { get; protected set; } = "IsMountedFlying";
        public Transform initialPoint;
        public Transform[] landingPoints;
        public int goalDamage;
        public float targetHealth;
        static int tryCount = 0;
        public AnimationCurve LandingCurvature;
        public AnimationCurve ThorwingCurvature;
        public AnimationCurve TurnCurvature;
        public AnimationCurve attenuationCurve;

        public float LandingDuration = 10.0f;
        public DeltaTimer LandingDurationTimer;
        private Vector3 landingStartPoint;
        [SerializeField] private Transform landingPoint;

        private bool saveLandingPoint;
        private bool lockAction = false;
        private bool IsAlignedToLandingSite = false;

        private static float remainingPitch = 0.0f;
        private static float totalAngle = 0.0f;
        private bool rollRight = false;
        private int previousPattern;

        public GameObject flyingLVFX;
        public GameObject flyingRVFX;

        [SerializeField] AnimationCurve throwingXZCurve;
        [SerializeField] AnimationCurve throwingYCurve;

        private int sequenceIndex = 0;

        class RotatePath
        {
            public delegate bool CirclingMethod(BossActionMountedFly instance, EntityBase entity, ref RotatePath path);

            private RotatePath() { }
            public RotatePath(float r, float angSpd, float movSpd, int circleCount, CirclingMethod func, Vector3 tilt)
            {
                radius = r;
                angularSpeed = angSpd;
                moveSpeed = movSpd;
                method = func;
                tiltedAxis = tilt;
                angularGoal = circleCount * 360.0f * Mathf.Deg2Rad;
                angularIntegral = 0.0f;
            }

            public bool Integrate(float deltaRadian)
            {
                if (angularIntegral < angularGoal)
                {
                    angularIntegral += Mathf.Abs(deltaRadian) * Mathf.Deg2Rad;
                }
                else
                {
                    angularIntegral = 0.0f;
                    return true;
                }
                return false;
            }

            public float radius;
            public float angularSpeed;
            public float moveSpeed;
            public float angularIntegral;
            public float angularGoal;
            public Vector3 tiltedAxis;
            public CirclingMethod method;

        }

        static RotatePath[] predefinedRotatePath =
        {
            new (25.0f, 60.0f, 30.0f,1 , fullCircularPath, new Vector3(0,0,0)),
            new (12.5f, 60.0f, 30.0f,1 , tiltedCircularPath, new Vector3(0,1,1).normalized),
            new (50.0f, 60.0f, 30.0f, 1, lemniscateCircularPath, new Vector3(0,0,0).normalized),
        };

        RotatePath selectedMethod = predefinedRotatePath[0];

        public float thresholdDistance = 0.1f;
        public float currentSpeed = 0.0f;
        public bool flapFlag = false;
        public bool flapAnimFlag = false;
        public float decelerationFactor = 0.4f;
        static bool fullCircularPath(BossActionMountedFly instance, EntityBase entity, ref RotatePath path)
        {
            float normalizedAngle = path.angularIntegral / path.angularGoal;
            float curveEvaluation = instance.TurnCurvature.Evaluate(normalizedAngle);

            //float attenuation = instance.attenuationCurve.Evaluate(normalizedAngle);

            float angle = -Time.deltaTime * path.angularSpeed;
            float arc = 2 * Mathf.PI * path.radius * (angle / 360.0f);

            bool IsDone = path.Integrate(angle);

            instance.currentSpeed -= Time.deltaTime;
            float normalizedSpeed = instance.currentSpeed / path.moveSpeed;

            if (normalizedSpeed <= instance.decelerationFactor && instance.flapFlag == false)
            {
                entity.EntityAnimator.ResetTrigger("IsMountedFlying");
                entity.EntityAnimator.SetTrigger("Flap");
                instance.flapFlag = true;
            }

            entity.transform.Rotate(Vector3.up * angle);
            entity.transform.Translate(Vector3.forward * instance.currentSpeed * Time.deltaTime);

            return IsDone;
        }
        static bool tiltedCircularPath(BossActionMountedFly instance, EntityBase entity, ref RotatePath path)
        {
            float angle = Time.deltaTime * path.angularSpeed;
            float arc = 2 * Mathf.PI * path.radius * (angle / 360.0f);

            bool IsDone = path.Integrate(angle);

            entity.transform.Rotate(Vector3.up * angle);

            if (Mathf.Abs(remainingPitch) >= 0.0f)
            {
                entity.transform.Rotate(Vector3.right, angle);

                if (remainingPitch > 0.0f)
                {
                    remainingPitch -= angle;
                }
                else
                {
                    remainingPitch += angle;
                }
            }

            instance.currentSpeed -= Time.deltaTime;
            float normalizedSpeed = instance.currentSpeed / path.moveSpeed;

            if (normalizedSpeed <= instance.decelerationFactor && instance.flapFlag == false)
            {
                entity.EntityAnimator.ResetTrigger("IsMountedFlying");
                entity.EntityAnimator.SetTrigger("Flap");
                instance.flapFlag = true;
            }

            entity.transform.Translate(Vector3.forward * Time.deltaTime * instance.currentSpeed);

            return IsDone;
        }

        static float flatSlopeRadians = Mathf.Deg2Rad * 360.0f;
        static bool reversedRotation = false;
        static bool lemniscateCircularPath(BossActionMountedFly instance, EntityBase entity, ref RotatePath path)
        {
            float angle = path.angularSpeed * Time.deltaTime;

            Vector3 localForward = entity.transform.forward;
            localForward = entity.transform.TransformVector(localForward);

            instance.currentSpeed -= Time.deltaTime;
            float normalizedSpeed = instance.currentSpeed / path.moveSpeed;

            if (normalizedSpeed <= instance.decelerationFactor && instance.flapFlag == false)
            {
                entity.EntityAnimator.ResetTrigger("IsMountedFlying");
                entity.EntityAnimator.SetTrigger("Flap");
                instance.flapFlag = true;
            }

            entity.transform.Translate(Vector3.forward * (instance.currentSpeed * Time.deltaTime));

            bool IsDone = path.Integrate(angle);



            if (path.angularIntegral >= flatSlopeRadians)
            {
                reversedRotation = !reversedRotation;
            }

            if (reversedRotation == true)
            {
                entity.transform.Rotate(Vector3.up * -angle);
            }
            else
            {
                entity.transform.Rotate(Vector3.up * angle);
            }

            return IsDone;
        }

        bool stompingLock = false;
        bool stompingAnimDetect = false;
        bool stompingEnd = false;
        bool isStomping = false;
        public bool isHit = false;

        public override void ActionUpdate()
        {
            PlayerCamera cameraInstance = PlayerCamera.Instance;
            //    cameraInstance.cameraResponse = Mathf.Lerp(cameraInstance.cameraResponse, 1.0f, 0.2f);

            Vector3 center = new Vector3(0.0f, entity.transform.position.y, 0.0f);
            Vector3 centerDirection = (center - entity.transform.position).normalized;
            Vector3 up = new Vector3(0.0f, entity.transform.position.y + 1.0f, 0.0f).normalized;
            Vector3 nextDir = Vector3.Cross(centerDirection, up).normalized;

            if (lockAction == true)
            {
                return;
            }

            entity.EntityData.moveDirection = nextDir;
            bool record = true;
            float threshold = 0.0f;
            // start landing
            if (entity.EntityData.health <= targetHealth)
            {
                if (record == true)
                {
                    threshold = Vector3.Distance(transform.position, landingPoints[tryCount].position) / 181.0f;
                    record = false;
                }

                if (IsAlignedToLandingSite == true)
                {
                    if (stompingAnimDetect == true)
                    {
                        float tick = LandingDurationTimer.Tick();
                        float factor = tick / LandingDuration;
                        float t = LandingCurvature.Evaluate(factor);

                        //// end this action
                        if (saveLandingPoint == false)
                        {
                            landingStartPoint = transform.position;
                            saveLandingPoint = true;
                        }

                        transform.position = Vector3.Lerp(landingStartPoint, landingPoint.position, t);
                        float dist = Vector3.Distance(transform.position, landingPoint.position);
                        Vector3 fallDir = (-landingStartPoint).normalized;

                        //controller.IsFlying = false;

                        if (dist <= 1.0f)
                        {
                            SoundManager.Instance.PlayInstance("Homeros_SkyDrop");
                            stompingEnd = true;
                            lockAction = true;
                            entity.PhysicsApplication = true;
                            entity.UseGravity = true;
                            entity.EntityAnimator.ResetTrigger("MountedStompStarted");
                            entity.EntityAnimator.SetTrigger("MountedStompEnd");
                            SoundManager.Instance.PlaySound("Homeros_Landing");
                            EntityBase target = PlayerController.Instance.PlayerEntity;

                            target.transform.SetParent(null);
                          //  controller.RemoveTile(tryCount);
                            var firstElement = BossController.MoveableTileIndices.First();
                            int tileIndex = firstElement.Key;

                            float distance = Random.Range(18.0f, 22.0f);
                            Vector3 dir = new Vector3(Random.Range(0.0f, 1.0f), 0.0f, Random.Range(0.0f, 1.0f)).normalized;

                            Vector3 point = distance * dir;
                            Vector3 landingPoint = point + target.transform.position;
                            landingPoint.y = landingPoints[0].position.y;

                            Debug.DrawLine(target.transform.position, landingPoint, Color.cyan);

                            AnimationCurve[] curves = { throwingXZCurve, throwingYCurve, throwingXZCurve };

                            //StartCoroutine(target.ThrowEntity(target.transform.position, landingPoints[tileIndex].position, 2.0f, ThorwingCurvature));
                            StartCoroutine(target.JumpToDestination(target.transform.position, landingPoint, 1.0f, 6.0f, curves, 0.05f));
                            target.TryChangeActionType(ActionType.Idle);
                            target.EntityAnimator.ResetTrigger("Mount");
                            target.EntityAnimator.SetTrigger("Eject");
                            PlayerCamera.Instance.Shake("BossMountedStomp");


                            //entity.EntityAnimator.SetTrigger("IsFallen");
                            Debug.DrawLine(target.transform.position, landingPoints[tileIndex].position, Color.red, float.PositiveInfinity);
                        }
                    }

                }
                else
                {
                    Vector3 midAirCenter = new Vector3(landingPoint.position.x, transform.position.y, landingPoint.position.z);
                    Vector3 centerDir = (midAirCenter - transform.position).normalized;
                    currentSpeed -= Time.deltaTime * 2;

                    float normalizedSpeed = currentSpeed / selectedMethod.moveSpeed;

                    Quaternion targetRot = Quaternion.LookRotation(centerDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.1f);

                    if (stompingLock == false)
                    {
                        entity.EntityAnimator.SetTrigger("MountedStompStarted");
                        stompingLock = true;
                    }

                    float alignment = Vector3.Dot(transform.forward, centerDir);
                    controller.IsFlying = false;

                    Debug.DrawRay(transform.position, centerDir * Vector3.Distance(midAirCenter, transform.position), Color.magenta);
                    Debug.DrawRay(transform.position, transform.forward * 5.0f, Color.magenta);

                    if (Mathf.Approximately(Mathf.Abs(alignment), 1.0f) == true)
                    {
                        IsAlignedToLandingSite = true;
                    }
                }
            }
            else
            {
                Debug.DrawRay(entity.transform.position, transform.forward, Color.blue, float.PositiveInfinity, false);

                if (selectedMethod.method(this, entity, ref selectedMethod) == true)
                {
                    int pattern = Random.Range(0, predefinedRotatePath.Length);
                    while (pattern == previousPattern)
                    {
                        pattern = Random.Range(0, predefinedRotatePath.Length);
                    }
                    //pattern = 2;
                    //currentSpeed = selectedMethod.moveSpeed;

                    //if (pattern == 1)
                    //{
                    //    float pitch = transform.rotation.eulerAngles.z;

                    //    if(pitch >= 40.0f)
                    //    {
                    //        rollRight = false;
                    //    }
                    //    if(pitch <= -40.0f)
                    //    {
                    //        rollRight = true;
                    //    }

                    //    if(rollRight == true)
                    //    {
                    //        remainingPitch = 45.0f;
                    //    }
                    //    else
                    //    {
                    //        remainingPitch = -45.0f;
                    //    }
                    //}
                    //else
                    //{

                    //}
                    selectedMethod.angularIntegral = 0.0f;
                    selectedMethod = predefinedRotatePath[pattern];
                    previousPattern = pattern;
                }
                else
                {
                    SoundManager.Instance.PlaySound("Homeros_Flight", false);
                }
            }
        }

        public override void End()
        {
            PlayerCamera.Instance.cameraResponse = 0.25f;
            PlayerCamera.Instance.trackingTarget = PlayerController.Instance.PlayerEntity.transform;
            //BossController.Instance.PlayVFX("Boss_Rush_L_WIND_VFX");
            //BossController.Instance.PlayVFX("Boss_Rush_R_WIND_VFX");
        }

        public override void Excute()
        {
            lockAction = false;
            saveLandingPoint = false;
            IsAlignedToLandingSite = false;
            stompingLock = false;
            stompingAnimDetect = false;
            stompingEnd = false;
            isStomping = false;
            isHit = false;

            controller = BossController.Instance;
            targetHealth = Mathf.Clamp(entity.EntityData.health - goalDamage, 0, entity.EntityData.maxHealth);
            //entity.EntityData.health += (int)goalDamage;
            LandingDurationTimer = new DeltaTimer(LandingDuration);
            LandingDurationTimer.Reset();

            PlayerCamera cameraInstance = PlayerCamera.Instance;
            SoundManager.Instance.PlaySound("Homeros_Flight");

            // cameraInstance.trackingTarget = BossController.Instance.bossEntity.transform;
            transform.position = initialPoint.position;
            controller.IsFlying = true;

            BossController.LandingPoints = landingPoints;

            currentSpeed = selectedMethod.moveSpeed;

            //    GameObject lfly= GameObjectPoolManager.Instance.CreateGameObject(flyingLVFX, transform.position, Quaternion.identity);
            //    GameObject rfly = GameObjectPoolManager.Instance.CreateGameObject(flyingRVFX, transform.position, Quaternion.identity);
            //    lfly.transform.SetParent(transform);
            //    rfly.transform.SetParent(transform);
        }
        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            BossController controller = BossController.Instance;
            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationStart:
                    if (sequenceIndex == 1)
                    {
                    }
                    break;
                case AnimationEventTriggerType.AnimationEnd:
                    if (sequenceIndex == 0)
                    {
                        sequenceIndex++;
                    }

                    if (flapFlag == true)
                    {
                        entity.EntityAnimator.ResetTrigger("flap");
                        entity.EntityAnimator.SetTrigger("IsMountedFlying");
                        flapFlag = false;
                    }

                    if (stompingLock == true)
                    {
                        if (stompingEnd == true)
                        {
                            entity.EntityAnimator.ResetTrigger("MountedStompEnd");
                            entity.SetActionType(ActionType.Move);
                            controller.selectedPatternMethod = BossController.MovingPattern;
                        }
                    }

                    if (isHit == true && stompingLock == false)
                    {
                        entity.EntityAnimator.ResetTrigger("MountedHit");
                        entity.EntityAnimator.SetTrigger("IsMountedFlying");
                        isHit = false;
                    }

                    break;
                case AnimationEventTriggerType.AnimationAttack:
                    currentSpeed = selectedMethod.moveSpeed;

                    if (stompingLock == true)
                    {
                        stompingAnimDetect = true;
                    }

                    break;
                case AnimationEventTriggerType.AnimationUnLock:
                    controller.IsGrabbed = false;
                    entity.EntityAnimator.ResetTrigger("IsFallen");
                    entity.SetActionType(ActionType.Alert);
                    PlayerCamera.Instance.VelocityTracking = true;

                    controller.selectedPatternMethod = BossController.AlertMovingPattern;
                    break;
                case AnimationEventTriggerType.AnimationLock:
                    entity.EntityAnimator.ResetTrigger("MountedHit");
                    break;
            }
        }
    }
}