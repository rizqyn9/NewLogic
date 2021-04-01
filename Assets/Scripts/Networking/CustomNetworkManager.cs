using UnityEngine;
using Mirror;

namespace RizqyNetworking
{
    [AddComponentMenu("")]
    public class CustomNetworkManager : NetworkManager
    {
        [Header("Custom")]
        public GameObject canvas;
        public UI_MainMenu ui_mainMenuController;

        #region Unity Callbacks

        /// <summary>
        /// Runs on both Server and Client
        /// Networking is NOT initialized when this fires
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            ui_mainMenuController.InitializeData();
        }

        #endregion


        #region Server System Callbacks

        /// <summary>
        /// Called on the server when a client is ready.
        /// <para>The default implementation of this function calls NetworkServer.SetClientReady() to continue the network setup process.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerReady(NetworkConnection conn)
        {
            Debug.Log("--- On Server Ready");
            base.OnServerReady(conn);
            ui_mainMenuController.OnServerReady(conn);
        }

        #endregion

        #region Start & Stop Callbacks

        /// <summary>
        /// This is invoked when a server is started - including when a host is started.
        /// <para>StartServer has multiple signatures, but they all cause this hook to be called.</para>
        /// </summary>
        public override void OnStartServer()
        {
            Debug.Log("---On start stop callback");
            if (mode == NetworkManagerMode.ServerOnly)
                canvas.SetActive(true);

            ui_mainMenuController.OnStartServer();

        }

        /// <summary>
        /// This is invoked when the client is started.
        /// </summary>
        public override void OnStartClient()
        {
            canvas.SetActive(true);
            ui_mainMenuController.OnStartClient();
        }

        #endregion
    }
}
