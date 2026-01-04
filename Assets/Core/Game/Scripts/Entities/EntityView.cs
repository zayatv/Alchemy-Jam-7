using UnityEngine;

namespace Core.Game.Entities
{
    public class EntityView : MonoBehaviour
    {
        #region Serialized Fields
        
        [Header("Animation")]
        [SerializeField] private Animator animator;
        
        #endregion
        
        #region Properties
        
        public Animator Animator => animator;
        
        public Vector3 AccumulatedRootMotion { get; set; }
        public Quaternion AccumulatedRootRotation { get; set; }
        
        #endregion

        private void OnAnimatorMove()
        {
            if (animator == null)
                return;
            
            if (animator.applyRootMotion)
            {
                AccumulatedRootMotion += animator.deltaPosition;
                AccumulatedRootRotation *= animator.deltaRotation;
            }
        }
    }
}