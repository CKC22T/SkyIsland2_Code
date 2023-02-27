using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using System.Net.Http;

namespace Olympus
{
    public class EntityBase : MonoBehaviour
    {
        [field: Title("Debug Param")]
        [field: ShowInInspector]
        public IEntityController EntityController { get; private set; } = null;
        [field: ShowInInspector] public bool IsGround => physics == null || physics.IsGround;
        [field: ShowInInspector] public bool PhysicsApplication { get; set; }
        [field: ShowInInspector] public bool UseGravity { get; set; }

        [field: Title("Entity Param")]
        [field: SerializeField] public int EntityNumber { get; private set; } = 0;
        [field: SerializeField] public EntityType EntityType { get; private set; } = EntityType.None;
        [field: SerializeField] public ActionType ActionType { get; private set; } = ActionType.None;
        [field: SerializeField] public EntityData EntityData { get; private set; }
        [field: SerializeField] public List<WeaponBase> WeaponList { get; private set; }
        [field: SerializeField] public WeaponBase Weapon { get; private set; }
        [field: SerializeField] public WeaponBase SecondaryWeapon { get; private set; }
        [field: SerializeField] public Transform AttackPosition { get; private set; }
        [field: SerializeField] public Transform CenterPosition { get; private set; }
        [field: SerializeField] public Transform EntityModel { get; private set; }
        [field: SerializeField] public Transform MoveCheckPosition { get; private set; }
        [field: Title("Animation Param")]
        [field: SerializeField] public Animator EntityAnimator { get; private set; }
        [field: SerializeField] public AnimationEventListener AnimationEventListener { get; private set; }
        [field: SerializeField] public SkinnedMeshRenderer[] EntityMeshRenderers { get; private set; }
        private List<Material> entityMaterials = new();

        [field: Title("Entity Action Setting")]
        [field: SerializeField] private SerializableDictionary<ActionType, ActionBase> actions = new();

        [field: SerializeField] public CharacterPhysics physics = null;
        [field: SerializeField] public Vector3 Velocity => physics.Velocity;
        [field: SerializeField] public float Radius => physics.Radius;
        [field: SerializeField] public float Height => physics.Center.y + physics.Height * 0.5f;
        [field: SerializeField] public Vector3 Offset => physics.Center + Vector3.up * physics.Height * 0.5f;

        [field: SerializeField] private bool requestGroundCheck = false;
        [field: SerializeField] private bool abortJumpCoroutine = false;

        [field: SerializeField, ReadOnly, TabGroup("Debug")] private bool isMove = false;

        public System.Action<EntityBase> entityDeadCallback = null;

        [Button]
        public void ChangeEntityController(IEntityController changeController)
        {
            EntityController?.DisconnectController(this);
            EntityController = changeController;
            EntityController?.ConnectController(this);
        }

        private void Awake()
        {
            TryGetComponent<CharacterPhysics>(out physics);
            if (Weapon)
            {
                if (Weapon.weaponModel)
                {
                    Weapon.weaponModel.SetActive(true);
                }
                actions[ActionType.Attack] = Weapon.AttackAction;
                actions[ActionType.Skill] = Weapon.SkillAction;
            }

            if (SecondaryWeapon)
            {
                actions[ActionType.SecondaryAttack] = SecondaryWeapon.AttackAction;
                actions[ActionType.SecondarySkill] = SecondaryWeapon.SkillAction;
            }
        }

        private void Start()
        {
            if (EntityMeshRenderers.Length > 0)
            {
                if (EntityType == EntityType.Player)
                {
                    entityMaterials.Add(EntityMeshRenderers[0].material);
                }
                else
                {
                    foreach (var meshRenderer in EntityMeshRenderers)
                    {
                        entityMaterials.Add(meshRenderer.materials[meshRenderer.materials.Length - 1]);
                    }
                }
            }
        }

        private void OnEnable()
        {
            GameManager.Instance.SceneEntityContainer.Add(this);

            PhysicsApplication = true;
            UseGravity = true;

            if (EntityController == null)
            {
                ChangeEntityController(NullController.Instance);

                if (EntityType == EntityType.Puri)
                {
                    ChangeEntityController(PuriController.Instance);
                }
                if (EntityType == EntityType.MagicBoar)
                {
                    ChangeEntityController(MagicBoarController.Instance);
                }
                if (EntityType == EntityType.Wisp)
                {
                    ChangeEntityController(WispController.Instance);
                }
                if (EntityType == EntityType.Spirit)
                {
                    ChangeEntityController(SpiritController.Instance);
                    //ActionType = ActionType.Dead;
                }
                if (EntityType == EntityType.FlowerTrap)
                {
                    ChangeEntityController(FlowerTrapController.Instance);
                }
            }

            if (!actions.ContainsKey(ActionType))
            {
                gameObject.SetActive(false);
                return;
            }

            SetActionType(ActionType);
            //actions[ActionType].Excute();

            physics.Velocity = Vector3.one;
            EntityData.health = EntityData.maxHealth;
        }

        private void OnDisable()
        {
            if (EntityType != EntityType.Spirit)
            {
                ActionType = ActionType.Idle;
            }
            if (EntityType == EntityType.MagicBoar ||
                EntityType == EntityType.Wisp ||
                EntityType == EntityType.BoxPuri)
            {
                ActionType = ActionType.Spawn;
            }

            EnemyHUDManagerUI ui = UIManager.Instance.GetUI(UIList.EnemyHUDManagerUI) as EnemyHUDManagerUI;
            if (ui)
            {
                ui.UnRegisterEnemyHud(this);
            }

            if (EntityType == EntityType.Player)
            {
                entityMaterials[0].SetFloat("_DamageController", 0.0f);
            }
            else
            {
                foreach (var material in entityMaterials)
                {
                    Color color = material.GetColor("_BaseColor");
                    color.a = 0.0f;
                    material.SetColor("_BaseColor", color);
                }
            }

            entityDeadCallback = null;
            GameManager.Instance.SceneEntityContainer.Remove(this);
        }

        //���� �׼� ��ȯ
        public bool SetActionType(ActionType setActionType)
        {
            if (!actions.ContainsKey(setActionType))
            {
                //LogUtil.LogError($"EntityType[{EntityType}] EntityNumber[{EntityNumber}] Not Found ActionType[{setActionType}]");
                return false;
            }

            actions[ActionType]?.End();
            if (AnimationEventListener)
            {
                //AnimationEventListener.OnAnimationEventAction -= actions[ActionType].AnimationEventAction;
                EntityAnimator.ResetTrigger(actions[ActionType].ActionName);
            }
            ActionType = setActionType;
            actions[ActionType]?.Excute();
            if (AnimationEventListener)
            {
                AnimationEventListener.OnAnimationEventAction = actions[ActionType].AnimationEventAction;
                EntityAnimator.SetTrigger(actions[ActionType].ActionName);
            }
            return true;
        }

        //���ǿ� ���� �׼� ��ȯ
        public bool PossibleChangeAction(ActionType actionType)
        {
            if (!actions.ContainsKey(actionType))
            {
                return false;
            }
            return actions[ActionType].TryChangeActionType(actionType);
        }

        public bool TryChangeActionType(ActionType setActionType)
        {
            //���ǵ�
            //return false;
            if (!actions[ActionType].TryChangeActionType(setActionType))
            {
                return false;
            }

            return SetActionType(setActionType);
        }

        public bool SetWeaponChange(int weaponNumber)
        {
            //if(ActionType == ActionType.Attack || ActionType == ActionType.Skill)
            //{
            //    return false;
            //}
            //if(!PossibleChangeAction(ActionType.Idle))
            if (ActionType == ActionType.Dead)
            {
                return false;
            }
            if(ActionType == ActionType.SecondaryAttack)
            {
                return false;
            }

            if (weaponNumber < 0)
            {
                LogUtil.LogError($"EntityType[{EntityType}] EntityNumber[{EntityNumber}] weaponNumber < 0");
                return false;
            }

            if (weaponNumber >= WeaponList.Count)
            {
                LogUtil.LogError($"EntityType[{EntityType}] EntityNumber[{EntityNumber}] weaponNumber >= weaponList.Count");
                return false;
            }

            if(WeaponList[weaponNumber] == Weapon)
            {
                return false;
            }

            if (Weapon)
            {
                if (Weapon.WeaponData.buffTimer > 4.0f)
                {
                    return false;
                }
            }

            //if (Weapon)
            //{
            //    if (Weapon.WeaponData.IsOnBuff)
            //    {
            //        return false;
            //    }
            //}

            if (Weapon)
            {
                Weapon.weaponModel?.SetActive(false);
            }
            Weapon = WeaponList[weaponNumber];
            Weapon.weaponModel?.SetActive(true);

            if (Weapon.WeaponData.upgradeCount > 0)
            {
                Weapon.WeaponData.buffTimer = 5.0f;
            }

            if (AnimationEventListener)
            {
                AnimationEventListener.OnAnimationEventAction -= actions[ActionType].AnimationEventAction;
            }

            actions[ActionType.Attack] = Weapon.AttackAction;
            actions[ActionType.Skill] = Weapon.SkillAction;

            if (AnimationEventListener)
            {
                AnimationEventListener.OnAnimationEventAction += actions[ActionType].AnimationEventAction;
            }

            if (ActionType == ActionType.Move)
            {
                SetActionType(ActionType.Move);
            }

            else if (ActionType != ActionType.Jump ||
                ActionType != ActionType.Dash)
            {
                TryChangeActionType(ActionType.Idle);
            }

            return true;
        }

        public void SetWeaponAttackAction(WeaponActionBase weaponAction)
        {
            actions[ActionType.Attack] = weaponAction;
        }

        private void Update()
        {
            if (GameManager.Instance.IsGameStop)
            {
                return;
            }

            if (EntityData.godModeTimer > 0.0f)
            {
                EntityData.godModeTimer -= Time.deltaTime;
            }

            if (EntityData.ultimateGauge < EntityData.ultimateMaxGauge)
            {
                EntityData.ultimateGauge += Time.deltaTime * EntityData.ultimateChargeSpeed;
            }

            EntityController?.ControlEntity(this);
            actions[ActionType]?.ActionUpdate();

            if (transform.position.y < -30.0f)
            {
                //// �˼��մϴ� ���� ���� �߸ŷ� ������ ..^^ ����ϰ� ����ڽ��ϴ�.,,^^
                //if (EntityType == EntityType.Enemy)
                //{
                //    Vector3 randomPosition = MagicBoarController.Instance.followTarget.transform.position;
                //    randomPosition.x = Random.Range(randomPosition.x - 3.0f, randomPosition.x + 3.0f);
                //    randomPosition.z = Random.Range(randomPosition.z - 3.0f, randomPosition.z + 3.0f);
                //    EntityWarp(randomPosition);

                //    MagicBoarController.Instance.ChaseStop(this, 1.0f);
                //    return;
                //}
                EntityData.health = 0;
                TryChangeActionType(ActionType.Dead);
                return;
            }
        }
        private void FixedUpdate()
        {
            //if (ActionType != ActionType.Move && ActionType != ActionType.Jump &&
            //    ActionType != ActionType.Walk && ActionType != ActionType.Run &&
            //    ActionType != ActionType.Dash)
            //{
            //    EntityIdle();
            //}

            if (GameManager.Instance.IsGameStop)
            {
                return;
            }

            if(requestGroundCheck == true)
            {
                RaycastHit hitInfo;
                Debug.DrawRay(transform.position, Vector3.down * 1.0f);
                if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 1.0f, LayerData.LAYER_MASK_PLAYER_COLLISION, QueryTriggerInteraction.Ignore))
                {
                    if(hitInfo.collider.tag.Equals("Environment"))
                    {
                        transform.position = hitInfo.point;
                        abortJumpCoroutine = true;
                    }
                }
            }

            actions[ActionType]?.ActionFixedUpdate();

            if (!isMove)
            {
                EntityIdle();
            }

            if (PhysicsApplication == true)
            {
                if (UseGravity == true)
                {
                    physics?.UpdatePhysics(Physics.gravity.y * EntityData.gravityScale);
                    physics?.Gravity(Physics.gravity.y * EntityData.gravityScale);
                }
                if (!IsGround)
                {
                    if (isMove)
                    {
                        if (physics.Velocity.y < -9.8f * 2f)
                        {
                            TryChangeActionType(ActionType.Jump);
                        }
                    }
                    else if (ActionType != ActionType.Jump)
                    {
                        TryChangeActionType(ActionType.Jump);
                    }
                }
            }
            isMove = false;
        }

        public bool GetDamage(int damage)
        {
            if(!isActiveAndEnabled)
            {
                return false;
            }

            if (EntityData.godModeTimer > 0.0f)
            {
                return false;
            }
            if (ActionType == ActionType.Dead)
            {
                return false;
            }

            if (entityMaterials.Count > 0)
            {
                StartCoroutine(DamageEffectRoutine());
            }

            if (EntityType == EntityType.None)
            {
                SetActionType(ActionType.Hit);
                return true;
            }

            if (gameObject.layer == LayerData.LAYER_ENEMY)
            {
                EnemyHUDManagerUI ui = UIManager.Instance.GetUI(UIList.EnemyHUDManagerUI) as EnemyHUDManagerUI;
                ui.RegisterEnemyHud(this);
            }

            EntityData.health -= damage;
            if (EntityData.health <= 0)
            {
                EntityData.health = 0;
                if (gameObject.layer == LayerData.LAYER_ENEMY)
                {
                    EnemyHUDManagerUI ui = UIManager.Instance.GetUI(UIList.EnemyHUDManagerUI) as EnemyHUDManagerUI;
                    ui.UnRegisterEnemyHud(this);
                    if (GameDataManager.Instance.enemyKillCounts.ContainsKey(EntityType))
                    {
                        GameDataManager.Instance.enemyKillCounts[EntityType]++;
                    }
                }
                else if (EntityType == EntityType.Player)
                {
                    GameDataManager.Instance.deathCount++;
                }
                SetActionType(ActionType.Dead);
                return true;
            }

            if (EntityType == EntityType.Player)
            {
                EntityData.godModeTimer = 0.5f;
                UIManager.Show(UIList.DamageUI);
            }
            else
            {
                StartCoroutine(modelShake(0.1f));

                IEnumerator modelShake(float time)
                {
                    float timer = 0.0f;
                    while (timer < time)
                    {
                        timer += Time.deltaTime;
                        EntityModel.localPosition = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)) * Time.deltaTime * 4.0f + Vector3.up;
                        yield return null;
                    }
                    EntityModel.localPosition = Vector3.up;
                }
            }

            if (ActionType == ActionType.Ultimate)
            {
                return true;
            }

            SetActionType(ActionType.Hit);
            return true;
        }

        private IEnumerator DamageEffectRoutine()
        {
            if (EntityType == EntityType.Player)
            {
                entityMaterials[0].SetFloat("_DamageController", 1.0f);
                yield return new WaitForSeconds(0.05f);
                entityMaterials[0].SetFloat("_DamageController", 0.0f);
            }
            else
            {
                foreach (var material in entityMaterials)
                {
                    Color color = material.GetColor("_BaseColor");
                    color.a = 0.5882353f;
                    material.SetColor("_BaseColor", color);
                }
                yield return new WaitForSeconds(0.05f);
                foreach (var material in entityMaterials)
                {
                    Color color = material.GetColor("_BaseColor");
                    color.a = 0.0f;
                    material.SetColor("_BaseColor", color);
                }
            }
        }

        public bool GetHeal(int heal)
        {
            EntityData.health += heal;
            if (EntityData.health > EntityData.maxHealth)
            {
                EntityData.health = EntityData.maxHealth;
            }

            if(EntityType == EntityType.Player)
            {
                (UIManager.Instance.GetUI(UIList.CharacterHPUI) as CharacterHPUI).PlayRecoveryAnimation();
            }

            return true;
        }

        public bool GetUltimateGauge(float gauge)
        {
            EntityData.ultimateGauge += gauge;
            if (EntityData.ultimateGauge > EntityData.ultimateMaxGauge)
            {
                EntityData.ultimateGauge = EntityData.ultimateMaxGauge;
            }
            return true;
        }

        public IEnumerator ThrowEntity(Vector3 from, Vector3 to, float duration, AnimationCurve curve, float threshold = 0.1f)
        {
            DeltaTimer timer = new DeltaTimer(duration);
            PhysicsApplication = false;
            UseGravity = false;
            PlayerCamera.Instance.VelocityTracking = false;
            while (true)
            {
                float t = timer.Tick() / duration;
                float evaluation = curve.Evaluate(t);
                transform.position = Vector3.Lerp(from, to, evaluation);

                if (Vector3.Distance(transform.position, to) <= threshold)
                {
                    break;
                }

                yield return null;
            }

            PhysicsApplication = true;
            UseGravity = true;

            if (this == PlayerController.Instance.PlayerEntity)
            {
                PlayerController.Instance.InputUnLock(LockType.None);
                PlayerCamera.Instance.VelocityTracking = true;
            }

            yield return null;
        }

        public IEnumerator JumpToDestination(Vector3 from, Vector3 to, float duration, float maximumAltitude, AnimationCurve[] axisCurves, float threshold = 0.1f)
        {
            DeltaTimer timer = new DeltaTimer(duration);
            PhysicsApplication = false;
            UseGravity = false;
            PlayerController.Instance.InputLock(LockType.None);
            PlayerCamera.Instance.VelocityTracking = false;
            Vector3 dir = to - from;
            while (timer.IsDone == false)
            {
                physics.Velocity = Vector3.zero;
                float t = timer.Tick() / duration;

                if(t >= 0.5f)
                {
                    requestGroundCheck = true;
                }

                float xEvaluation = axisCurves[0].Evaluate(t);
                float yEvaluation = axisCurves[1].Evaluate(t);
                float zEvaluation = axisCurves[2].Evaluate(t);

                //float totalEvaluation = xEvaluation * yEvaluation;

                float x = Mathf.Lerp(from.x, to.x, xEvaluation);
                float z = Mathf.Lerp(from.z, to.z, zEvaluation);

                //float y = Mathf.LerpUnclamped(from.y, to.y + maximumAltitude, yEvaluation);
                float ty = from.y + (dir * xEvaluation).y;
                float y = Mathf.LerpUnclamped(ty, ty + maximumAltitude, yEvaluation);

                if(abortJumpCoroutine == true)
                {
                    break;
                }

                transform.position = new Vector3(x, y, z); //Vector3.Lerp(from, to, evaluation);
                if (Vector3.Distance(transform.position, to) <= threshold)
                {
                    break;
                }

                yield return null;
            }

            requestGroundCheck = false;
            PhysicsApplication = true;
            abortJumpCoroutine = false;
            UseGravity = true;
            PlayerCamera.Instance.VelocityTracking = true;
            PlayerController.Instance.InputUnLock(LockType.None);

            yield return null;
        }

        //Entity Movement
        #region Entity Movement
        public void EntityWarp(Vector3 position)
        {
            physics?.Warp(position);
            physics.Velocity = Vector3.zero;
        }

        public void EntityIdle(float deceleration)
        {
            physics?.Decelerate(deceleration);
        }

        public void EntityIdle()
        {
            physics?.Decelerate(EntityData.deceleration);
        }

        public void EntityMove(float turningDrag, float acceleration, float topSpeed, bool isRotate = true)
        {
            physics?.Accelerate(EntityData.moveDirection, turningDrag, acceleration, topSpeed);
            if (isRotate)
            {
                EntityRotate();
            }
            isMove = true;
        }

        public void EntityMove()
        {
            physics?.Accelerate(EntityData.moveDirection, EntityData.turningDrag, EntityData.acceleration, EntityData.moveSpeed);
            EntityRotate();
            isMove = true;
        }

        public void EntityJump(float height)
        {
            physics?.Jump(height);
        }

        public void EntityJump()
        {
            physics?.Jump(EntityData.jumpHeight);
        }

        public bool EntitySphereCast(Vector3 direction, float distance, out RaycastHit hit, int layer = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            return physics.SphereCast(direction, distance, out hit, layer, queryTriggerInteraction);
        }

        public void EntityRotate()
        {
            var dir = EntityData.moveDirection;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(dir),
                EntityData.rotateSpeed * Time.deltaTime
                );
            }
        }

        public void EntityRotate(float rotationSpeed)
        {
            var dir = EntityData.moveDirection;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(dir),
                rotationSpeed * Time.deltaTime
                );
            }
        }
        #endregion
    }
}