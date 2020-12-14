using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    /// <summary>
    /// Have this cover up whole screen at start
    /// </summary>
    class LoginUI : MonoBehaviour
    {
        [Header("Pick User Prefab has to include Text and Button Component!")]
        public GameObject PickUserPrefab;
        public Transform PickUsersParent;
        public GameObject LoginUIWindowParent;
        public GameObject EnterPasswordWindowParent;
        public Button BtConfirmPassword;
        public InputField IFPassword;
        public GameObject RaycasterObj, PlayerObj;

        private int pickedUserId;

        void Start()
        {
            Display();
            BtConfirmPassword.onClick.AddListener(TryLogin);
            RaycasterObj.SetActive(false);
            PlayerObj.SetActive(false);
        }
        void Display()
        {
            int i = 0;
            
            StartCoroutine(RefreshGrid());
        }
        IEnumerator RefreshGrid()
        {
            PickUsersParent.gameObject.SetActive(false);
            yield return null;
            PickUsersParent.gameObject.SetActive(true);
        }
        void Pick(int i)
        {
            pickedUserId = i;
            EnterPasswordWindowParent.SetActive(true);
        }
        void TryLogin()
        {
            EnterPasswordWindowParent.SetActive(false);
            if (GameManager.INSTANCE.LoginData.TryLogin(pickedUserId, IFPassword.text))
            {
                gameObject.SetActive(false);
                LoginUIWindowParent.SetActive(false);
                RaycasterObj.SetActive(true);
                PlayerObj.SetActive(true);
            }
        }
    }
}