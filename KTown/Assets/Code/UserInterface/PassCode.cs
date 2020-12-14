using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class PassCode : MonoBehaviour
    {
        public static PassCode INSTANCE;

        public GameObject ParentObj;
        public Button BtConfirm;
        public InputField IfPassword;

        Action<string> Callback;

        void Awake()
        {
            BtConfirm.onClick.AddListener(OnConfirm);
            INSTANCE = this;
        }
        public void Init(Action<string> callback)
        {
            Callback = callback;
            ParentObj.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            IfPassword.Select();
            IfPassword.ActivateInputField();
        }
        void OnConfirm()
        {
            ParentObj.SetActive(false);
            if (Callback != null) Callback.Invoke(IfPassword.text);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}