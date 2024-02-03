using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public static class CursorController
    {
        public static void SetActiveState(bool isActive)
        {
            Cursor.visible = isActive;

            return;
#if UNITY_EDITOR
            if (isActive)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
#endif

        }
        
    }
}