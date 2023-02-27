using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class ItemData : MonoBehaviour
    {
        [field: Sirenix.OdinInspector.Title("Item Info")]
        [field: SerializeField] public string ItemName { get; private set; } = "";
        [field: SerializeField] public string ItemInfo { get; private set; } = "";
        [field: SerializeField] public ItemType ItemType { get; private set; } = ItemType.None;


    }
}