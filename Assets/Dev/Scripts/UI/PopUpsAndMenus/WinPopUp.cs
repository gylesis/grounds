using Fusion;
using TMPro;
using UnityEngine;

namespace Dev.UI.PopUpsAndMenus
{
    public class WinPopUp : PopUp
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _winText;
            

        [Rpc]
        public void RPC_Setup([RpcTarget] PlayerRef playerRef)
        {
            Show();
        }
        
        
    }
}