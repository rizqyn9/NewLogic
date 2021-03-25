using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace RizqyNetworking
{
    public class UI_MainMenu : MonoBehaviour
    {
        [Header("Main Menu UI")]
        [SerializeField] List<Selectable>   MainMenuSelectables = new List<Selectable>();
        [SerializeField] InputField         InputMatch;
        [SerializeField] Canvas             CanvasMainMenu;

        [Header("Game Settings")]
        [SerializeField] public byte CustomMaxPlayers = 40;


        /// <summary>
        /// Cross-reference of client that created the corresponding match in openMatches below
        /// </summary>
        internal static readonly Dictionary<NetworkConnection, Guid> playerMatches = new Dictionary<NetworkConnection, Guid>();


        /// <summary>
        /// Network Connections of all players in a match
        /// </summary>
        internal static readonly Dictionary<Guid, HashSet<NetworkConnection>> matchConnections = new Dictionary<Guid, HashSet<NetworkConnection>>();

        /// <summary>
        /// Open matches that are available for joining
        /// </summary>
        internal static readonly Dictionary<Guid, MatchInfo> openMatches = new Dictionary<Guid, MatchInfo>();


        /// <summary>
        /// Player informations by Network Connection
        /// </summary>
        internal static readonly Dictionary<NetworkConnection, PlayerInfo> playerInfos = new Dictionary<NetworkConnection, PlayerInfo>();

        /// <summary>
        /// Network Connections that have neither started nor joined a match yet
        /// </summary>
        internal static readonly List<NetworkConnection> waitingConnections = new List<NetworkConnection>();


        #region UI Functions

        // Called from several places to ensure a clean reset
        //  - MatchNetworkManager.Awake
        //  - OnStartServer
        //  - OnStartClient
        //  - OnClientDisconnect
        //  - ResetCanvas
        internal void InitializeData()
        {
            playerMatches.Clear();
            openMatches.Clear();
            matchConnections.Clear();
        }

        #endregion



        #region Button Services
        public void ButtonStart ()
        {

        }

        public void ButtonHostPublic ()
        {
            //if (!NetworkClient.active) return;

            Debug.Log($"Host Public");

            // Create Room
            NetworkClient.connection.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Create });
        }

        public void ButtonHostPrivate()
        {

        }

        public void ButtonSearch()
        {

        }

        #endregion



        #region Server & Client Callbacks

        // Methods in this section are called from MatchNetworkManager's corresponding methods

        internal void OnStartServer()
        {
            if (!NetworkServer.active) return;

            Debug.Log("On Start Server");
            InitializeData();
            NetworkServer.RegisterHandler<ServerMatchMessage>(OnServerMatchMessage);
        }

        internal void OnServerReady(NetworkConnection conn)
        {
            if (!NetworkServer.active) return;

            waitingConnections.Add(conn);
            playerInfos.Add(conn, new PlayerInfo { ready = false });
        }

        #endregion



        void OnServerMatchMessage(NetworkConnection conn, ServerMatchMessage msg)
        {
            if (!NetworkServer.active) return;

            switch (msg.serverMatchOperation)
            {
                case ServerMatchOperation.None:
                    {
                        Debug.LogWarning("Missing ServerMatchOperation");
                        break;
                    }
                case ServerMatchOperation.Create:
                    {
                        Debug.Log("Create Room");
                        OnServerCreateMatch(conn);
                        break;
                    }
                //case ServerMatchOperation.Cancel:
                //    {
                //        OnServerCancelMatch(conn);
                //        break;
                //    }
                //case ServerMatchOperation.Start:
                //    {
                //        OnServerStartMatch(conn);
                //        break;
                //    }
                //case ServerMatchOperation.Join:
                //    {
                //        OnServerJoinMatch(conn, msg.matchId);
                //        break;
                //    }
                //case ServerMatchOperation.Leave:
                //    {
                //        OnServerLeaveMatch(conn, msg.matchId);
                //        break;
                //    }
                //case ServerMatchOperation.Ready:
                //    {
                //        OnServerPlayerReady(conn, msg.matchId);
                //        break;
                //    }
            }
        }

        void OnServerCreateMatch(NetworkConnection conn)
        {
            if (!NetworkServer.active || playerMatches.ContainsKey(conn)) return;

            Guid newMatchId = Guid.NewGuid();
            matchConnections.Add(newMatchId, new HashSet<NetworkConnection>());
            matchConnections[newMatchId].Add(conn);
            playerMatches.Add(conn, newMatchId);
            openMatches.Add(newMatchId, new MatchInfo { matchId = newMatchId, maxPlayers = CustomMaxPlayers, players = 1 });

            PlayerInfo playerInfo = playerInfos[conn];
            playerInfo.ready = false;
            playerInfo.matchId = newMatchId;
            playerInfos[conn] = playerInfo;

            //PlayerInfo[] infos = matchConnections[newMatchId].Select(playerConn => playerInfos[playerConn]).ToArray();

            conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Created, matchId = newMatchId, });
            Debug.Log(playerInfo);
            print(playerInfo);

            
        }
    }
}
