using UnityEngine;
using Mirror;

namespace RizqyNetworking
{
    [AddComponentMenu("")]
    public class CustomNetworkManager : NetworkRoomManager
    {
        //[Header("Custom")]
        //public UI_MainMenu ui_mainMenu;

        #region Server System Callbacks

        /// <summary>
        /// Called on the server when a client is ready.
        /// <para>The default implementation of this function calls NetworkServer.SetClientReady() to continue the network setup process.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);
            Debug.Log($"Server Ready");
            //ui_mainMenu.OnServerReady(conn);
        }

        #endregion

        //#region Start & Stop Callbacks

        /// <summary>
        /// This is invoked when a server is started - including when a host is started.
        /// <para>StartServer has multiple signatures, but they all cause this hook to be called.</para>
        /// </summary>
        //public override void OnStartServer()
        //{
        //    if (mode == NetworkManagerMode.ServerOnly)
        //        canvas.SetActive(true);

        //    canvasController.OnStartServer();

        //}
        //#endregion
    }
}
