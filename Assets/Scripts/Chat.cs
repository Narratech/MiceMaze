using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Chat : NetworkBehaviour {

	Text texto;
	//InputField input;
	public MouseMovement raton;
	public string colorJugador;

	Dropdown ratonPreguntado;
	Dropdown preguntoPor;

	InputField columna;
	InputField fila;


	public string mensajeDropdown;
	private int valorDropdown;

	//Button preguntado;

	// Use this for initialization
	void Start () {
		/*texto = GameObject.Find ("PanelTexto").GetComponent<Text> ();
		input = GameObject.Find ("EntradaTexto").GetComponent<InputField> ();
		asignarColor ();*/
		texto = GameObject.Find ("PanelTexto").GetComponent<Text> ();
		ratonPreguntado = GameObject.Find ("Raton1").GetComponent<Dropdown> ();
		preguntoPor = GameObject.Find ("Raton2").GetComponent<Dropdown> ();
		columna = GameObject.Find ("EntradaColumna").GetComponent<InputField> ();
		fila = GameObject.Find ("EntradaFila").GetComponent<InputField> ();
		//preguntado = GameObject.Find ("BotonPreguntado").GetComponent<Button> ();
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

	void Update(){
		if (!isLocalPlayer)
			return;

		if (Input.GetKeyDown (KeyCode.Return)) {
			valorDropdown = ratonPreguntado.value;
			mensajeDropdown = ratonPreguntado.options [valorDropdown].text;
			string col = columna.text;
			string fil = fila.text;
			string mensaje = "Raton " + mensajeDropdown; //+ " has visto al raton ";

			valorDropdown = preguntoPor.value;
			mensajeDropdown = preguntoPor.options [valorDropdown].text;
			if (mensajeDropdown == "Nadie")
				mensaje += " en la posicion (" + col + "," + fil + ")";
			else {
				mensaje += " has visto al raton " + mensajeDropdown + " en la posicion (" + col + "," + fil + ")";
			}
			columna.text = "";
			fila.text = "";

			CmdEnviar (mensaje);
		}

	}


	/*void Update () {

		if (!isLocalPlayer)
			return;

		if (Input.GetKeyDown (KeyCode.Return)) {
			if (input.text != "") {
				string mensaje = input.text;
				input.text = "";

				CmdEnviar (mensaje);
			}
		}
	}*/

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
