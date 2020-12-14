using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hacking
{
    class Transpol : MonoBehaviour, IRefreshable
    {
        public List<GameObjToPass> Containers;
        public TextMesh mTextMesh;

        bool available;

        public void Init()
        {
            Debug.LogWarning("Bezpieczny terminal Trans-Pol, wprowadź numer konteneru");
            UserInterface.PassCode.INSTANCE.Init(TryGuess);
        }

        public void Refesh(MapController.VRMode mode)
        {
            available = true;
            //foreach (var v in Containers) v.mObj.SetActive(false);
            mTextMesh.text = "";
        }

        public void TryGuess(string s)
        {
            available = true; //not necessary, let them do more if they want to
            if (!available)
            {
                Debug.LogWarning("Error! Security breach! Init Shutdown");
                GameManager.INSTANCE.AddHeatEvent.Invoke(1, "Detected!");
                return;
            }
            foreach (var v in Containers)
            {
                if (v.mPass == s)
                {
                    //v.mObj.SetActive(true);
                    mTextMesh.text = v.mValue;
                    Debug.LogWarning(v.mValue);
                    available = false;
                    return;
                }
            }
            Debug.LogWarning("Nie znaleziono kontenera o tym numerze");
        }

        [Serializable]
        public class GameObjToPass
        {
            //public GameObject mObj;
            public string mPass;
            public string mValue;
        }
    }
}