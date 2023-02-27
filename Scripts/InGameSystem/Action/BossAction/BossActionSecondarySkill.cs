using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class BossActionSecondarySkill : ActionBase
    {
        public Transform LevelCenter;
        public override string ActionName { get; protected set; } = "Roar";
        public override void ActionUpdate()
        {
            LogUtil.Log("Blizzard Attack");

        }

        public override void AnimationEventAction(AnimationEventTriggerType eventId)
        {
            var controller = BossController.Instance;
            switch (eventId)
            {
                case AnimationEventTriggerType.AnimationStart:

                    controller.IsStatusChangeable = false;
                    controller.PlayVFX("Blizard_Roar_VFX");
                    SoundManager.Instance.PlaySound("Homeros_Roar");
                    break;
                case AnimationEventTriggerType.AnimationEffectStart:
                    for (int i = 0; i < controller.PreviousSnowballs.Count; i++)
                    {
                        if (controller.PreviousSnowballs[i] != null)
                        {
                            var snowballInstance = controller.PreviousSnowballs[i].GetComponent<SnowballActionIdle>();
                            snowballInstance.Explode();
                        }
                    }

                    controller.PreviousSnowballs.Clear();

                    const float length = 1.0f;
                    int deterant = 0;
                    float time = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        float x = Random.Range(-20, 20);
                        float z = Random.Range(-20, 20);

                        Vector3 randomPoint = new Vector3(LevelCenter.position.x + x, transform.position.y + length, LevelCenter.position.z + z);
                        Vector3 castDir = new Vector3(0, -length, 0);
                        //if (Physics.Raycast(randomPoint, castDir, length) == true)
                        //{
                        GameObject newObject = GameObject.Instantiate(controller.snowStormPrefab);
                        SnowStorm storm = newObject.GetComponent<SnowStorm>();
                        storm.Center = LevelCenter;
                        newObject.transform.position = randomPoint;

                        Debug.DrawLine(entity.transform.position + randomPoint, entity.transform.position + castDir, Color.red, 10.0f);
                        //}
                        //else
                        //{
                        //    i--;
                        //}

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
                    break;
                case AnimationEventTriggerType.AnimationEnd:

                    break;
            }
        }

        public override void End()
        {
            BossController.ResetStates();

        }

        public override void Excute()
        {

        }
    }
}