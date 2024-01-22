using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dev.Infrastructure
{
    public class DedicatedServerAutoHost : MonoBehaviour
    {
        [SerializeField] private bool _simulateServer;
        
        private async void Awake()
        {
            if (Application.platform == RuntimePlatform.WindowsServer || _simulateServer)
            {
                Debug.Log($"Hello. I am dedicated server, I am going to host a server!");
                Debug.Log($"___________________________________________________________________________");

                DontDestroyOnLoad(gameObject);

                if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    await SceneManager.LoadSceneAsync(1).AsAsyncOperationObservable();
                }
                
                FindObjectOfType<ConnectionManager>().StartServer();
            }
        }
    }
}