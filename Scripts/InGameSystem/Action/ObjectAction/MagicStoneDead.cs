using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class MagicStoneDead : ActionBase
    {
        public override string ActionName { get; protected set; } = "";
        public GameObject magicStoneRubble;

        public override void ActionUpdate()
        {
        }

        public override void End()
        {
        }

        public override void Excute()
        {
            SoundManager.Instance.PlayInstance("MagicStone_Destory");
            var obj = GameObjectPoolManager.Instance.CreateGameObject(magicStoneRubble, entity.CenterPosition.position, entity.transform.rotation);
            GameObjectPoolManager.Instance.Release(obj, 10.0f);
            //var obj = Instantiate(magicStoneRubble, entity.transform.position, entity.transform.rotation);
            //Destroy(obj, 10.0f);
            GameObjectPoolManager.Instance.Release(entity.gameObject);
            //Destroy(entity.gameObject);
        }
    }
}