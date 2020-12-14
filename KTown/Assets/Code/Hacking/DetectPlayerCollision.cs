using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hacking
{
    class DetectPlayerCollision : MonoBehaviour
    {
        void OnTriggerEnter(Collider c)
        {
            IPlayerTrigger pt = c.GetComponent<IPlayerTrigger>();
            Debug.Log($"{this} collided with {c}, it has player trigger: {pt != null} of: {pt}");
            pt.OnPlayerTriggered();
        }
    }
}