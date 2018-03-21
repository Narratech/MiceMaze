using UnityEngine;
using Prototype.NetworkLobby;
using System.Collections;
using UnityEngine.Networking;

public class ProbandoHook : LobbyHook 
{
	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
	{
		LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
		MouseMovement localPlayer = gamePlayer.GetComponent<MouseMovement> ();

		localPlayer.mi_color = lobby.playerColor;
	}
}
