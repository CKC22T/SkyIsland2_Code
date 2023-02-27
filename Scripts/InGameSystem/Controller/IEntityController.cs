using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public interface IEntityController
    {
        public void ControlEntity(EntityBase entity);
        public void DisconnectController(EntityBase entity);
        public void ConnectController(EntityBase entity);
    }
}