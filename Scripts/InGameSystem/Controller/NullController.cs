using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class NullController : SingletonBase<NullController>, IEntityController
    {

        public void ConnectController(EntityBase entity)
        {
        }

        public void ControlEntity(EntityBase entity)
        {
        }

        public void DisconnectController(EntityBase entity)
        {
        }
    }
}