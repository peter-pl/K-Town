using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Helpers
{
    class DEV_Shortcuts : MonoBehaviour
    {
        public void RevealAllInfos()
        {
            foreach (var v in InfoUnit.ALL_INFOS)
            {
                v.Value.DEV_Reveal();
            }
        }
    }
}