using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Locations
{
    [Serializable]
    public class Lootable
    {
        //TODO: Loot conditions for max security locations
        public int LootEnergyCost = 30;
        public int ScoutEnergyCost = 5;
        public Transform TeleportTo;
        public InfoUnit mInfoLoot;
    }
}