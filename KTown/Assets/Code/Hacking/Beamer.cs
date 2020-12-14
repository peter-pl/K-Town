using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hacking
{
    class Beamer : Interactable
    {
        public Vector3 BeamDir = new Vector3(0, 3, 0);
        public float BeamTime = 1;

        protected override void Fire()
        {
            var p = MapController.INSTANCE.ActionCharCtrl;
            p.enabled = false;
            p.gameObject.LeanMove(p.transform.position + BeamDir, BeamTime).setOnComplete(() => p.enabled = true);
        }
    }
}