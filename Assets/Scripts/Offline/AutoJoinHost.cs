using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace RizqyNetworking
{
    public class AutoJoinHost : MonoBehaviour
    {
        [SerializeField] NetworkManager networkManager;

        // Building for server
        //void Start()
        //{
        //    if(!Application.isBatchMode)
        //    {
        //        Debug.Log($"<color=green>Run as Client</color>");
        //        networkManager.StartClient();
        //    } else
        //    {
        //        Debug.Log($"<color=yellow>Running as Server</color>");
        //    }
        
        //}

        public void JoinLocal ()
        {
            networkManager.networkAddress = "localhost";
            networkManager.StartClient();
        }
    }
}
