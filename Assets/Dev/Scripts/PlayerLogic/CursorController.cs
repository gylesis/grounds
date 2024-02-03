using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public static class CursorController
    {
        public static void SetActiveState(bool isActive)
        {
            Cursor.visible = isActive;
            //Cursor.lockState = CursorLockMode.None;
        }
    }
}