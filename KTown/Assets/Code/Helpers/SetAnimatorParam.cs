using UnityEngine;

namespace Helpers
{
    class SetAnimatorParam : MonoBehaviour
    {
        public string ParamName = "Param";
        public Animator mAnimator;

        int paramHash;

        void Awake()
        {
            if (!mAnimator) Debug.LogError($"{this} doesn't have dependencies assigned!");
            paramHash = Animator.StringToHash(ParamName);
        }
        public void SetBool(bool b)
        {
            if (mAnimator) mAnimator.SetBool(paramHash, b);
        }
        public void SetInt(int i)
        {
            if (mAnimator) mAnimator.SetInteger(paramHash, i);
        }
        public void SetFloat(float f)
        {
            if (mAnimator) mAnimator.SetFloat(paramHash, f);
        }
    }
}
