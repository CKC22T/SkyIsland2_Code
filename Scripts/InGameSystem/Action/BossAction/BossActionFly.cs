using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Olympus
{
    public class BossActionFly : ActionBase
    {
        public override string ActionName { get; protected set; } = "DoFly";

        bool IsMovingUpwards;
        float flySpeed = 70.0f;
        public DeltaTimer flyingMountTimer = new(1.0f);
        public DeltaTimer cameraFocusSwitchTimer = new(2.0f);
        private Vector3 initialPoint;
        public Transform endPoint;
        public Transform initialTransform;
        public AnimationCurve FlyingUpwardCurvature;
        public float FlyingUpwardDuration = 10.0f;
        public DeltaTimer FlyingUpwardDurationTimer;
        public float decellerator = 10.0f;
        [SerializeField] LevelObject interactionUILevelObject;
        [SerializeField] Transform levelCenter;

        bool shakeFlag = false;

        int sequenceIndex = 0;

        int[] previousWeaponDamages = new int[4];

        BossController controller;
        public override void ActionUpdate()
        {
            PlayerCamera cameraInstance = PlayerCamera.Instance;

            if (sequenceIndex == 1)
            {
                flyingMountTimer.Tick();

                if (flyingMountTimer.IsDone == true)
                {
                    controller.IsMountable = false;
                }
            }

            if (sequenceIndex == 2)
            {
                if (IsMovingUpwards == true)
                {
                    //controllerRigid.Move(Vector3.up * flySpeed * Time.deltaTime);
                    Vector3 dir = (endPoint.position - transform.position).normalized;

                    // transform.Translate(dir * flySpeed * Time.deltaTime);
                    float factor = FlyingUpwardDurationTimer.Tick() / FlyingUpwardDuration;
                    float evaluation = FlyingUpwardCurvature.Evaluate(factor);
                    transform.position = Vector3.Lerp(initialPoint, endPoint.position, evaluation);
                    float distance = Vector3.Distance(transform.position, endPoint.position);

                    if (distance <= 1.0f)
                    {
                        var weaponList = PlayerController.Instance.PlayerEntity.WeaponList;
                        for (int i = 0; i < weaponList.Count; i++)
                        {
                            weaponList[i].WeaponData.attackDamage = previousWeaponDamages[i];
                        }

                        IsMovingUpwards = false;
                        controller.flyingIcicleTimer.GoodToGo = true;
                        controller.flyingIcicleIntervalTimer.GoodToGo = true;
                        controller.flyingIcicleFallingDuration.GoodToGo = true;

                        if (controller.IsGrabbed == true)
                        {
                            SoundManager.Instance.StopSound("Homeros_Flight");
                            cameraInstance.trackingTarget = PlayerController.Instance.PlayerEntity.transform;
                            controller.selectedPatternMethod = BossController.MountedFlyingPattern;
                            entity.SetActionType(ActionType.MountedFly);
                            PlayerCamera.Instance.VelocityTracking = false;
                            return;
                        }
                    }
                }
            }

            if (controller.IsGrabbed == true)
            {
                cameraFocusSwitchTimer.Tick();
                cameraInstance.trackingTarget = initialTransform;
            }
        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            controller = BossController.Instance;
            PlayerCamera cameraInstance = PlayerCamera.Instance;

            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationEffectStart:

                    if(sequenceIndex == 0)
                    {
                        SoundManager.Instance.PlaySound("Homeros_Fly");
                    }
                    break;

                case AnimationEventTriggerType.AnimationStart:
                    break;
                case AnimationEventTriggerType.AnimationLock:
                    StartCoroutine(spawnStorm(3, 0.0f));
                    //StartCoroutine(spawnStorm(8, 5.0f));
                    SoundManager.Instance.PlaySound("Homeros_Fly");
                    SoundManager.Instance.PlaySound("Homeros_SkyDrop", true);

                    IsMovingUpwards = true;

                    controller.PlayVFX("Boss_Rush_Fly_VFX");
                    if (shakeFlag == false)
                    {
                        PlayerCamera.Instance.Shake("BossMountedFly");
                        shakeFlag = true;
                    }

                    initialPoint = transform.position;
                    cameraInstance.cameraResponse = 0.05f;

                    interactionUILevelObject.IsActive = false;
                    interactionUILevelObject.gameObject.SetActive(false);

                    break;

                case AnimationEventTriggerType.AnimationEnd:

                    if (sequenceIndex == 1)
                    {
                        if (flyingMountTimer.IsDone == true || controller.IsGrabbed == true)
                        {
                            sequenceIndex++;
                        }
                    }
                    else
                    {
                        sequenceIndex++;
                    }

                    if (sequenceIndex == 2)
                    {
                        if (IsMovingUpwards == false)
                        {
                            sequenceIndex = 2;
                            entity.EntityAnimator.SetTrigger("EndFlyLoop");
                        }
                    }
                    if (sequenceIndex == 3)
                    {
                        entity.EntityAnimator.ResetTrigger("EndFlyLoop");
                        sequenceIndex = 2;
                    }
                    break;
            }
        }

        private IEnumerator spawnStorm(int count, float delay)
        {
            yield return new WaitForSeconds(delay);
            float length = 1.0f;
            int deterant = 0;
            float time = 0;
            for (int i = 0; i < count; i++)
            {
                float x = Random.Range(-20, 20);
                float z = Random.Range(-20, 20);

                Vector3 randomPoint = new Vector3(levelCenter.position.x + x, levelCenter.position.y + length, levelCenter.position.z + z);
                Vector3 castDir = new Vector3(0, -length, 0);
                GameObject newObject = GameObject.Instantiate(controller.snowStormPrefab);
                SnowStorm storm = newObject.GetComponent<SnowStorm>();
                storm.Center = levelCenter;
                newObject.transform.position = randomPoint;

                Debug.DrawLine(entity.transform.position + randomPoint, entity.transform.position + castDir, Color.red, 10.0f);

                if (deterant >= 1000)
                {
                    break;
                }

                if (time < 2.0f)
                {
                    time += Time.deltaTime;
                }
                else
                {
                    time = 0.0f;
                }
                deterant++;
                // get random 4 points

            }
        }

        public override void End()
        {
            sequenceIndex = 0;
            shakeFlag = false;
            SoundManager.Instance.StopSound("Homeros_Fly");


        }

        public override void Excute()
        {
            entity.EntityAnimator.ResetTrigger("DoFly");
            IsMovingUpwards = false;
            controller = BossController.Instance;
            initialTransform.position = transform.position;
            FlyingUpwardDurationTimer = new DeltaTimer(FlyingUpwardDuration);

            flyingMountTimer.Reset();
            FlyingUpwardDurationTimer.Reset();
            cameraFocusSwitchTimer.Reset();

            var weaponList = PlayerController.Instance.PlayerEntity.WeaponList;
            for (int i = 0; i < weaponList.Count; i++)
            {
                previousWeaponDamages[i] = weaponList[i].WeaponData.attackDamage;
                weaponList[i].WeaponData.attackDamage /= 2;
            }

            interactionUILevelObject.IsActive = true;
            interactionUILevelObject.gameObject.SetActive(true);



        }
    }
}