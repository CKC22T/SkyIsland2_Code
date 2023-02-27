using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public enum LockType
    {
        None,
        FromGUI,
        FromTimeline,
        EventOccur,
        ForceMove,
    }

    public class PlayerController : SingletonBase<PlayerController>, IEntityController
    {
        public EntityBase PlayerEntity { get; private set; }
        BossController bossInstance;
        private PlayerCamera playerCam;
        public bool IsStunned { get; set; }
        public bool IsNoclip { get; set; }
        public DeltaTimer stunnedTimer = new DeltaTimer(1.0F);
        public DeltaTimer noClipTimer = new DeltaTimer(1.0F);
        public DeltaTimer mountedAttackIntervalTimer = new DeltaTimer(1.0f);
        public float originalSpeed;
        public bool allowStandingOnOther = false;
        public bool IsControlLocked => InputLockSet.Count > 0;

        private readonly HashSet<LockType> InputLockSet = new();

        public void InputLock(LockType lockType)
        {
            if (!InputLockSet.Contains(lockType))
            {
                InputLockSet.Add(lockType);
            }
            PlayerEntity.physics.Velocity = Vector3.zero;
        }

        public void InputUnLock(LockType lockType)
        {
            if (InputLockSet.Contains(lockType))
            {
                InputLockSet.Remove(lockType);
            }
        }

        public void InputUnLock()
        {
            InputLockSet.Clear();
        }

        CharacterController characterController;
        public void ConnectController(EntityBase entity)
        {
            PlayerEntity = entity;
            playerCam = Camera.main.GetComponent<PlayerCamera>();
            IsStunned = false;
            originalSpeed = entity.EntityData.moveSpeed;
            characterController = entity.GetComponent<CharacterController>();
            allowStandingOnOther = true;
            bossInstance = BossController.Instance;
        }

        void NoclipCheck()
        {
            if (IsNoclip == true)
            {
                noClipTimer.Tick();

                if (noClipTimer.IsDone == true)
                {
                    noClipTimer.Reset();
                    IsNoclip = false;
                    characterController.detectCollisions = true;
                }

                return;
            }
        }

        void DontStandOnOtherEntity()
        {
            RaycastHit hitInfo;
            Transform playerTransform = PlayerEntity.transform;
            Debug.DrawRay(playerTransform.position + new Vector3(0, 1, 0), Vector3.down, Color.magenta);
            Ray castingRay = new Ray(playerTransform.position + new Vector3(0.0f, 2.0f, 0.0f), Vector3.down);
            if (Physics.SphereCast(castingRay, 1.0f, out hitInfo, 1.5f) == true)
            {
                if (hitInfo.distance <= 1.5f)
                {
                    if (hitInfo.collider.gameObject.GetComponent<EntityBase>() != null)
                    {
                        Vector3 outerDirection = -(hitInfo.transform.position - playerTransform.position + new Vector3(0, 1, 0));
                        outerDirection.y = 0.0f;
                        outerDirection.Normalize();
                        Debug.DrawRay(hitInfo.transform.position, outerDirection, Color.magenta, 3.0f);

                        PlayerEntity.EntityData.moveDirection = outerDirection * 5;
                        PlayerEntity.EntityMove(0.0f, 5.0f * 10, 10.0f);
                        PlayerEntity.EntityJump(10.5f);
                        IsStunned = true;
                    }
                }
            }
        }

        void StunnedCheck()
        {
            if (IsStunned == true)
            {
                SoundManager.Instance.PlaySound("Player_Stun", false);
                stunnedTimer.Tick();

                if (stunnedTimer.IsDone == true)
                {
                    stunnedTimer.Reset();
                    IsStunned = false;
                    SoundManager.Instance.StopSound("Player_Stun");
                }
                return;
            }
        }

        void PopupShift()
        {
            if (playerCam.IsCameraShifted == false)
            {
                playerCam.SetCameraShift(true, 0);

            }
            else
            {
                playerCam.SetCameraShift(false);
            }
        }

        public void PopupShift(bool isShift)
        {
            playerCam.SetCameraShift(isShift);
        }

        public void ControlEntity(EntityBase entity)
        {
            NoclipCheck();
            StunnedCheck();

            if (allowStandingOnOther == false)
            {
                DontStandOnOtherEntity();
            }

            //UI Key
            //if (Input.GetKeyDown(KeyCode.Escape))
            //{
            //    UIManager uiManager = UIManager.Instance;

            //    bool isPause = true;
            //    for (int i = 0; i < uiManager.ActiveElements.Count; i++)
            //    {
            //        if (uiManager.ActiveElements[i].HaveFlag(UIBase.UIFlag.IMMUTABLE) == false)
            //        {
            //            uiManager.ActiveElements[i].Hide();
            //            //uiManager.ActiveElements.RemoveAt(i);
            //            isPause = false;
            //            break;
            //        }
            //    }

            //    if (isPause)
            //    {
            //        UIManager.Show(UIList.PauseUI);
            //    }
            //}

            if (bossInstance.IsGrabbed == true)
            {
                mountedAttackIntervalTimer.Tick();
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (bossInstance.IsGrabbed == true && bossInstance.IsFlying == true)
                {
                    // do damage to boss
                    if (mountedAttackIntervalTimer.IsDone == true)
                    {
                        entity.EntityAnimator.SetTrigger("MountAttack");
                        mountedAttackIntervalTimer.Reset(false);
                    }
                }
            }

            if (IsControlLocked)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                UIManager.Show(UIList.PopupUI);
                SoundManager.Instance.PlaySound("UI_Information");
            }


            if (Input.GetKeyDown(KeyCode.F1))
            {
                UIManager.Show(UIList.WeaponInfoUI);
            }

            //Scene Change Key
            //if (Input.GetKeyDown(KeyCode.F4))
            //{
            //    Main.Instance.ChangeScene(SceneType.InGameBossScene);
            //}

            //if (Input.GetKeyDown(KeyCode.F5))
            //{
            //    Main.Instance.ChangeScene(SceneType.InGameSpringScene);
            //}

            //if (Input.GetKeyDown(KeyCode.F6))
            //{
            //    Main.Instance.ChangeScene(SceneType.InGameSummerScene);
            //}

            //if (Input.GetKeyDown(KeyCode.F7))
            //{
            //    Main.Instance.ChangeScene(SceneType.InGameFallScene);
            //}

            //if (Input.GetKeyDown(KeyCode.F8))
            //{
            //    Main.Instance.ChangeScene(SceneType.InGameWinterScene);
            //}

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Z))
            {
                entity.SetActionType(ActionType.Dead);
                return;
            }

            //if (Input.GetKeyDown(KeyCode.F9))
            //{
            //    entity.Weapon.UpgradeWeapon(0);
            //}
            //if (Input.GetKeyDown(KeyCode.F10))
            //{
            //    entity.Weapon.UpgradeWeapon(1);
            //}
            //if (Input.GetKeyDown(KeyCode.F11))
            //{
            //    entity.Weapon.UpgradeWeapon(2);
            //}
            //if (Input.GetKeyDown(KeyCode.F12))
            //{
            //    entity.Weapon.UpgradeWeapon(3);
            //}
            //

            //if (Input.GetKeyDown(KeyCode.F))
            //{
            //    if (bossInstance.IsMountable == true)
            //    {
            //        bossInstance.IsGrabbed = true;

            //        Transform neck = BossController.NeckPoint;

            //        Vector3 attachPoint = bossInstance.bossEntity.transform.position + new Vector3(0, 1.5f, 0.0f);
            //        entity.transform.position = attachPoint;
            //        entity.transform.rotation = bossInstance.bossEntity.transform.rotation;
            //        entity.transform.parent = neck;
            //        entity.UseGravity = false;
            //        InputLock(LockType.None);
            //        entity.EntityAnimator.SetTrigger("Mount");

            //        bossInstance.IsMountable = false;
            //    }

            //    if (bossInstance.IsGrabbed == true && bossInstance.IsFlying == true)
            //    {
            //        // do damage to boss

            //        if (mountedAttackIntervalTimer.IsDone == true)
            //        {
            //            bossInstance.bossEntity.GetDamage(5);
            //            GameObject trigger = GameObjectPoolManager.Instance.CreateGameObject(bossInstance.mountedAttackPrefab, BossController.NeckPoint.position, Quaternion.identity);
            //            trigger.GetComponent<DamageEffectLevelEvent>().owner = PlayerEntity;
            //            trigger.SetActive(true);

            //            mountedAttackIntervalTimer.Reset();
            //        }
            //    }
            //}

            //Weapon Swap Key
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                entity.SetActionType(ActionType.Idle);
            }

            int setWeaponNumber = -1;
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                setWeaponNumber = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                setWeaponNumber = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                setWeaponNumber = 2;
            }
            if (entity.Weapon && setWeaponNumber >= 0 && entity.SetWeaponChange(setWeaponNumber))
            {
                SoundManager.Instance.PlaySound("Player_ChangeWeapon");
                var upgradeUI = UIManager.Instance.GetUI(UIList.UpgradeUI) as UpgradeUI;
                upgradeUI.SetInfo(PlayerEntity.Weapon);

                if (!upgradeUI.isActiveAndEnabled)
                {
                    //WeaponChangeUI element = UIManager.Show(UIList.WeaponChangeUI) as WeaponChangeUI;
                    //element.SelectWeapon(setWeaponNumber);
                    SkillUI element2 = UIManager.Instance.GetUI(UIList.SkillUI) as SkillUI;
                    element2.ChangeWeapon(setWeaponNumber);
                }

                //if (entity.Weapon.WeaponData.upgradeCount > 0)
                //{
                //    SkillUI ui = UIManager.Instance.GetUI(UIList.SkillUI) as SkillUI;
                //    ui.ChangeWeapon(setWeaponNumber);
                //}

                entity.Weapon.CheckBuffAnimation();
            }

            if (entity.Weapon)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (entity.PossibleChangeAction(ActionType.Attack))
                    {
                        RotateToLookDirection(entity);
                        entity.TryChangeActionType(ActionType.Attack);
                        return;
                    }
                }
                if (Input.GetMouseButton(1))
                {
                    if (entity.PossibleChangeAction(ActionType.SecondaryAttack))
                    {
                        RotateToLookDirection(entity);
                        entity.TryChangeActionType(ActionType.SecondaryAttack);
                        return;
                    }
                    if (entity.ActionType == ActionType.SecondaryAttack)
                    {
                        RotateToLookDirection(entity);
                    }
                }
            }

            Vector3 moveDir = Vector3.zero;
            if (Input.GetKey(KeyCode.A))
            {
                moveDir += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveDir += Vector3.right;
            }
            if (Input.GetKey(KeyCode.W))
            {
                moveDir += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveDir += Vector3.back;
            }

            if (moveDir != Vector3.zero)
            {
                var camDir = Camera.main.transform.forward;
                camDir.y = 0;
                camDir.Normalize();
                var quaternion = Quaternion.LookRotation(moveDir) * Quaternion.FromToRotation(Vector3.forward, camDir);
                moveDir = quaternion * Vector3.forward;
            }
            entity.EntityData.moveDirection = moveDir;


            if (Input.GetKey(KeyCode.Space))
            {
                if (entity.IsGround)
                {
                    if (entity.ActionType == ActionType.Ultimate)
                    {
                        entity.EntityJump();
                    }
                    else if (entity.PossibleChangeAction(ActionType.Jump))
                    {
                        entity.EntityJump();
                        entity.SetActionType(ActionType.Jump);
                        SoundManager.Instance.PlaySound("Player_Jump", false);
                    }
                }
            }

            entity.EntityData.dashTimer -= Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.LeftShift) &&
                entity.ActionType != ActionType.Dash &&
                entity.ActionType != ActionType.Dead &&
                entity.EntityData.dashTimer < 0.0f)
            {
                if (0.01f < entity.EntityData.moveDirection.sqrMagnitude)
                {
                    entity.EntityRotate(54000);
                }
                entity.SetActionType(ActionType.Dash);
                SoundManager.Instance.PlaySound("Player_Tumble");
                return;
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (entity.Weapon)
                {
                    entity.TryChangeActionType(ActionType.Ultimate);

                    return;
                }
            }

            //if (Input.GetKeyDown(KeyCode.E))
            //{
            //    if (entity.Weapon.WeaponData.upgradeCount > 0 && entity.PossibleChangeAction(ActionType.Skill))
            //    {
            //        if (!entity.Weapon.WeaponData.IsBuffCoolDown)
            //        {
            //            RotateToLookDirection(entity);
            //            entity.TryChangeActionType(ActionType.Skill);
            //            entity.Weapon.OnBuff();
            //            return;
            //        }
            //    }
            //}


            if (0.01f < entity.EntityData.moveDirection.sqrMagnitude)
            {
                entity.TryChangeActionType(ActionType.Move);
                return;
            }
        }

        public void RotateToLookDirection(EntityBase entity)
        {
            var camDir = Camera.main.transform.forward;
            camDir.y = 0;
            camDir.Normalize();

            Vector3 entityPosition = Camera.main.WorldToScreenPoint(entity.CenterPosition.position);
            Vector3 dir = Input.mousePosition - entityPosition;
            dir.z = dir.y;
            dir.y = 0;
            dir.Normalize();

            var quaternion = Quaternion.LookRotation(dir) * Quaternion.FromToRotation(Vector3.forward, camDir);
            entity.EntityData.moveDirection = quaternion * Vector3.forward;
            entity.EntityRotate(36000);
        }

        public void DisconnectController(EntityBase entity)
        {
            // PlayerEntity = null;
        }

    }
}