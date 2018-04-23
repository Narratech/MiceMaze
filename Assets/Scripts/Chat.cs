using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Chat : NetworkBehaviour {

	Text texto;
	InputField input;
	public MouseMovement raton;
	//public Color probando;
	public string colorJugador;

	// Use this for initialization
	void Start () {
		texto = GameObject.Find ("PanelTexto").GetComponent<Text> ();
		input = GameObject.Find ("EntradaTexto").GetComponent<InputField> ();
		//probando = raton.mi_color;
		asignarColor ();
	}

	void asignarColor(){
		if (raton.mi_color == Color.red) {
			colorJugador = "Rojo";
		} else if (raton.mi_color == Color.green) {
			colorJugador = "Verde";
		} else if (raton.mi_color == Color.yellow) {
			colorJugador = "Amarillo";
		} else if (raton.mi_color == Color.blue) {
			colorJugador = "Azul";
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (!isLocalPlayer)
			return;

		if (Input.GetKeyDown (KeyCode.Return)) {
			if (input.text != "") {
				string mensaje = input.text;
				input.text = "";

				CmdEnviar (mensaje);
			}
		}
	}

	[Command]
	void CmdEnviar(string mensaje){
		
		RpcRecivir (mensaje);
	}

	[ClientRpc]
	public void RpcRecivir(string mensaje){

		//texto.text += ">>" + mensaje + "\n";
		texto.text += colorJugador + " >> " + mensaje + "\n";
	}

}
