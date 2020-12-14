using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UserInterface
{
    class ConfirmDisconnect : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Y))
            {
                MapController.INSTANCE.UserFinishVR();
                gameObject.SetActive(false);
            }
            else if (Input.GetKeyUp(KeyCode.N))
            {
                gameObject.SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
            }
        }
    }
}