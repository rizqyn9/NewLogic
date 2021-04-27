using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Linq;
using UnityEngine.SceneManagement;

namespace RizqyNetworking
{
    //Monobehavior
    public class UI_MainMenu : MonoBehaviour
    {
        [Header("Main Menu UI")]
        [SerializeField] List<Selectable>   MainMenuSelectables = new List<Selectable>();
        [SerializeField] InputField         InputRoomID;
        [SerializeField] Canvas             CanvasMainMenu;

        [Header("Game Settings")]
        [SerializeField] public byte CustomMaxPlayers = 40;

        [Header("Room")]
        [SerializeField] GameObject roomView;
        [SerializeField] Text roomCode;

        [Header("RoomGUI")]
        public GameObject matchControllerPrefab;
        public GameObject lobbyPrefab;


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

        /// <summary>
        /// List for Room ID
        /// </summary>
        internal static readonly Dictionary<string, MatchInfo> roomOnline = new Dictionary<string, MatchInfo>(); 

        /// <summary>
        /// GUID of a match the local player has created
        /// </summary>
        internal Guid localPlayerMatch = Guid.Empty;

        /// <summary>
        /// GUID of a match the local player has joined
        /// </summary>
        internal Guid localJoinedMatch = Guid.Empty;

        #region PlayerIndex

        // Used in UI for "Player #"
        int playerIndex = 1;

        #endregion


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
            waitingConnections.Clear();
            localPlayerMatch = Guid.Empty;
            localJoinedMatch = Guid.Empty;
        }

        #endregion



        #region Button Services
        public void ButtonStart ()
        {
            if (!NetworkClient.active || localPlayerMatch == Guid.Empty) return;

            Debug.Log($"-> Start Request");
            NetworkClient.connection.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Start });

        }

        public void ButtonHostPublic ()
        {
            if (!NetworkClient.active) return;

            Debug.Log($"-> Host Public");

            // Create Room
            NetworkClient.connection.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Create });

        }

        public void ButtonJoin()
        {
            if (!NetworkClient.active) return;

            Debug.Log($"-> {InputRoomID.text}");
            Debug.Log("-> Join Room Request");
            NetworkClient.connection.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Join, roomID = InputRoomID.text.ToUpper()});

        }

        public void ButtonHostPrivate()
        {

        }

        public void ButtonSearch()
        {

        }

        /// <summary>
        /// Sends updated match list to all waiting connections or just one if specified
        /// </summary>
        /// <param name="conn"></param>
        internal void SendMatchList(NetworkConnection conn = null)
        {
            Debug.Log("-> Send Match List");
            if (!NetworkServer.active) return;

            if (conn != null)
            {
                conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.List, matchInfos = openMatches.Values.ToArray() });
            }
            else
            {
                foreach (var waiter in waitingConnections)
                {
                    waiter.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.List, matchInfos = openMatches.Values.ToArray() });
                }
            }
        }


        #endregion



        #region Server & Client Callbacks

        // Methods in this section are called from MatchNetworkManager's corresponding methods

        internal void OnStartServer()
        {
            if (!NetworkServer.active) return;

            Debug.Log("-> On Start Server");
            InitializeData();
            NetworkServer.RegisterHandler<ServerMatchMessage>(OnServerMatchMessage);
        }

        internal void OnServerReady(NetworkConnection conn)
        {
            if (!NetworkServer.active) return;

            waitingConnections.Add(conn);
            Debug.Log("-> On Server Ready");
            playerInfos.Add(conn, new PlayerInfo { ready = false ,playerIndex = this.playerIndex});
            playerIndex++;

            SendMatchList();
        }

        internal void OnStartClient()
        {
            Debug.Log("-> On Start Client");
            if (!NetworkClient.active) return;

            InitializeData();
            //ShowLobbyView();
            //createButton.gameObject.SetActive(true);
            //joinButton.gameObject.SetActive(true);
            NetworkClient.RegisterHandler<ClientMatchMessage>(OnClientMatchMessage);
        }

        internal void OnClientConnect(NetworkConnection conn)
        {
            Debug.Log("-> On Client Connect");
            playerInfos.Add(conn, new PlayerInfo { playerIndex = this.playerIndex, ready = false });
        }




        #endregion


        #region Server Message Hadler
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
                        Debug.Log("<color=green>->Server Create Room</color>");
                        OnServerCreateMatch(conn);
                        break;
                    }
                case ServerMatchOperation.Join:
                    {
                        Debug.Log("<color=green>->Server Join Room</color>");
                        OnServerJoinMatch(conn, msg.roomID);
                        break;
                    }
                case ServerMatchOperation.Start:
                    {
                        Debug.Log($"<color=green>->Server Start Room {conn.connectionId}</color>");
                        OnServerStartMatch(conn);
                        break;
                    }
                    //case ServerMatchOperation.Cancel:
                    //    {
                    //        OnServerCancelMatch(conn);
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
            string _GetRoomID = GetRoomID();
            matchConnections.Add(newMatchId, new HashSet<NetworkConnection>());
            matchConnections[newMatchId].Add(conn);
            playerMatches.Add(conn, newMatchId);
            MatchInfo newMatchInfo = new MatchInfo { matchId = newMatchId, maxPlayers = CustomMaxPlayers, players = 1, roomID = _GetRoomID };
            roomOnline.Add(_GetRoomID, newMatchInfo);
            openMatches.Add(newMatchId, newMatchInfo);

            PlayerInfo playerInfo = playerInfos[conn];
            playerInfo.ready = false;
            playerInfo.matchId = newMatchId;
            playerInfo.roomID = _GetRoomID;
            playerInfos[conn] = playerInfo;

            PlayerInfo[] infos = matchConnections[newMatchId].Select(playerConn => playerInfos[playerConn]).ToArray();

            conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Created, matchId = newMatchId, roomID = _GetRoomID });
            Debug.Log(playerInfo);
            print(playerInfo);

            
        }

        void OnServerJoinMatch(NetworkConnection conn, String roomID)
        {
            Debug.Log("-> OnServerJoinMatch");
            if (!NetworkServer.active || !roomOnline.ContainsKey(roomID)
                //|| !openMatches.ContainsKey(matchId)
                )
            {
                Debug.Log("Gagal Join");
                return;
            }
            Debug.Log("<color=green>->Success Join</color>");
            MatchInfo matchInfo = roomOnline[roomID];
            matchInfo.players++;
            roomOnline[roomID] = matchInfo;
            matchConnections[matchInfo.matchId].Add(conn);

            PlayerInfo playerInfo = playerInfos[conn];
            playerInfo.roomID = roomID;
            //playerInfo.ready = false;
            //playerInfo.matchId = matchId;
            playerInfo.matchId = matchInfo.matchId;
            playerInfos[conn] = playerInfo;

            PlayerInfo[] infos = matchConnections[matchInfo.matchId].Select(playerConn => playerInfos[playerConn]).ToArray();
            //SendMatchList();

            conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Joined, matchId = matchInfo.matchId, playerInfos = infos, roomID = roomID });

            foreach (NetworkConnection playerConn in matchConnections[matchInfo.matchId])
            {
                playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos });
            }
        }

        void OnServerStartMatch(NetworkConnection conn)
        {
            if (!NetworkServer.active || !playerMatches.ContainsKey(conn)) return;

            Debug.Log($"-> On ServerStartMatch");
            Guid matchId;
            if (playerMatches.TryGetValue(conn, out matchId))
            {
                GameObject matchControllerObject = Instantiate(matchControllerPrefab);
#pragma warning disable 618
                matchControllerObject.GetComponent<NetworkMatchChecker>().matchId = matchId;
#pragma warning restore 618
                NetworkServer.Spawn(matchControllerObject);


                foreach (NetworkConnection playerConn in matchConnections[matchId])
                {
                    Debug.Log("-> Getting Player in Room");
                    playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Started });

                    GameObject player = Instantiate(NetworkManager.singleton.playerPrefab);

#pragma warning disable 618
                        player.GetComponent<NetworkMatchChecker>().matchId = matchId;
#pragma warning restore 618

                    NetworkServer.AddPlayerForConnection(playerConn, player);
                    int indeks = 0;
                    Debug.Log($"Joining Player {indeks++ }");
                }


                //                    if (matchController.player1 == null)
                //                    {
                //                        matchController.player1 = playerConn.identity;
                //                    }
                //                    else
                //                    {
                //                        matchController.player2 = playerConn.identity;
                //                    }

                //                    /* Reset ready state for after the match. */
                //                    PlayerInfo playerInfo = playerInfos[playerConn];
                //                    //playerInfo.ready = false;
                //                    playerInfos[playerConn] = playerInfo;
                //                }

                //                matchController.startingPlayer = matchController.player1;
                //                matchController.currentPlayer = matchController.player1;

                //                playerMatches.Remove(conn);
                //                openMatches.Remove(matchId);
                //                matchConnections.Remove(matchId);
                //                SendMatchList();

                //OnPlayerDisconnected += matchController.OnPlayerDisconnected;
            }
        }


        #endregion

        #region Client Message Handler

        void OnClientMatchMessage(NetworkConnection conn, ClientMatchMessage msg)
        {
            if (!NetworkClient.active) return;
            Debug.Log("-> OnClientMatchMessage");
            switch (msg.clientMatchOperation)
            {
                case ClientMatchOperation.None:
                    {
                        Debug.LogWarning("Missing ClientMatchOperation");
                        break;
                    }
                case ClientMatchOperation.List:
                    {
                        openMatches.Clear();
                        foreach (MatchInfo matchInfo in msg.matchInfos)
                        {
                            openMatches.Add(matchInfo.matchId, matchInfo);
                        }
                        //RefreshMatchList();
                        break;
                    }
                case ClientMatchOperation.Created:
                    {
                        Debug.Log($"-> ClientMatchOperation.Created {conn}");
                        localPlayerMatch = msg.matchId;
                        roomView.SetActive(true);
                        roomCode.text = msg.roomID;
                        Instantiate(lobbyPrefab);
                        lobbyPrefab.gameObject.GetComponent<LobbyController>().RoomCode(msg.roomID);


                        //SceneManager.LoadScene(1, LoadSceneMode.Single);
                        //NetworkManager. 

                        //ShowRoomView();
                        //roomGUI.RefreshRoomPlayers(msg.playerInfos);
                        //roomGUI.SetOwner(true);
                        break;
                    }
                case ClientMatchOperation.Joined:
                    {
                        localJoinedMatch = msg.matchId;
                        Instantiate(lobbyPrefab);
                        lobbyPrefab.gameObject.GetComponent<LobbyController>().RoomCode(msg.roomID);

                        //ShowRoomView();
                        //roomGUI.RefreshRoomPlayers(msg.playerInfos);
                        //roomGUI.SetOwner(false);
                        //SceneManager.LoadScene(1, LoadSceneMode.Single);
                        break;
                    }
                case ClientMatchOperation.Started:
                    {
                        //lobbyView.SetActive(false);
                        //roomView.SetActive(false);
                        SceneManager.LoadScene(2, LoadSceneMode.Single);
                        break;
                    }
                //case ClientMatchOperation.Cancelled:
                //    {
                //        localPlayerMatch = Guid.Empty;
                //        ShowLobbyView();
                //        break;
                //    }
                    //case ClientMatchOperation.Departed:
                    //    {
                    //        localJoinedMatch = Guid.Empty;
                    //        ShowLobbyView();
                    //        break;
                    //    }
                    //case ClientMatchOperation.UpdateRoom:
                    //    {
                    //        roomGUI.RefreshRoomPlayers(msg.playerInfos);
                    //        break;
                    //    }
            }
        }

        //void ShowRoomView()
        //{
        //    lobbyView.SetActive(false);
        //    roomView.SetActive(true);
        //}

        public static string GetRoomID()
        {
            string _id = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                int random = UnityEngine.Random.Range(0, 36);
                if (random < 26)
                {
                    _id += (char)(random + 65);
                }
                else
                {
                    _id += (random - 26).ToString();
                }
            }
            Debug.Log($"Random Match ID: {_id}");
            return _id.ToUpper();
        }


        #endregion


// Checking Debug Console
        public void Refresh()
        {
            foreach(var roomData in roomOnline)
            {
                Debug.Log($"<color=red>{roomData.Value.players}</color>");
            }
        }
    }
}
