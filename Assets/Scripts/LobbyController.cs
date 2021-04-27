using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    [SerializeField] Text roomCode;
    [SerializeField] Text playerRoom;

    public void RoomCode (string _roomID)
    {
        Debug.Log($"--- {_roomID}");
        roomCode.text = "ahsgdhjadh";
    }
}
