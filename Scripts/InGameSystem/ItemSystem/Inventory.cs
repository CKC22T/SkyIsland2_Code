using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class Inventory : MonoBehaviour
    {
        [field: Sirenix.OdinInspector.Title("Inventory Info")]
        [field: SerializeField] public List<ItemBase> Items { get; private set; } = new();


    }
}