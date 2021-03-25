using System;
using Mirror;

namespace RizqyNetworking
{

    #region ServerState
    // Server State
    public struct ServerMatchMessage : NetworkMessage
    {
        public ServerMatchOperation serverMatchOperation;
        public Guid matchID;
    }

    public enum ServerMatchOperation : byte
    {
        None,
        Create,
        Cancel,
        Start,
        Join,
        Leave,
        Ready
    }

    #endregion

    #region ClientState

    /// <summary>
    /// Match message to be sent to the client
    /// </summary>
    public struct ClientMatchMessage : NetworkMessage
    {
        public ClientMatchOperation clientMatchOperation;
        public Guid matchId;
        public MatchInfo[] matchInfos;
        public PlayerInfo[] playerInfos;
    }


    /// <summary>
    /// Match operation to execute on the client
    /// </summary>
    public enum ClientMatchOperation : byte
    {
        None,
        List,
        Created,
        Cancelled,
        Joined,
        Departed,
        UpdateRoom,
        Started
    }


    #endregion


    #region Match Info Models

    /// <summary>
    /// Information about a match
    /// </summary>
    [Serializable]
    public struct MatchInfo
    {
        public Guid matchId;
        public byte players;
        public byte maxPlayers;
    }

    #endregion

    #region Player Info Struct
    /// <summary>
    /// Information about a player
    /// </summary>
    [Serializable]
    public struct PlayerInfo
    {
        public int playerIndex;
        public bool ready;
        public Guid matchId;
    }

    #endregion
}