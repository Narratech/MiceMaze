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
	Dropdown accionesPosibles;

	InputField columna;
	InputField fila;


	public string mensajeDropdown;
	private int valorDropdown;

	//Button preguntado;

	// Use this for initialization
	void Start () {
		/*texto = GameObject.Find ("PanelTexto").GetComponent<Text> ();
		input = GameObject.Find ("EntradaTexto").GetComponent<InputField> ();*/
		asignarColor ();
		texto = GameObject.Find ("PanelTexto").GetComponent<Text> ();
		ratonPreguntado = GameObject.Find ("Raton1").GetComponent<Dropdown> ();
		preguntoPor = GameObject.Find ("Raton2").GetComponent<Dropdown> ();
		columna = GameObject.Find ("EntradaColumna").GetComponent<InputField> ();
		fila = GameObject.Find ("EntradaFila").GetComponent<InputField> ();
		accionesPosibles = GameObject.Find ("DropdownAcciones").GetComponent<Dropdown> ();
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

		/*if (Input.GetKeyDown (KeyCode.Return)) {
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
		}*/

		string ratonInterrogado;//raton que está siendo interrogado
		string preguntaSobre;//raton sobre el que estás preguntando
		string col;//columna
		string fil;//fila
		string accion;//accion sobre la que quieres preguntar
		string mensaje;//mensaje que se va a producir para hacer la pregunta

		valorDropdown = ratonPreguntado.value;
		ratonInterrogado = ratonPreguntado.options [valorDropdown].text;

		valorDropdown = preguntoPor.value;
		preguntaSobre = preguntoPor.options [valorDropdown].text;

		valorDropdown = accionesPosibles.value;
		accion = accionesPosibles.options [valorDropdown].text;

		col = columna.text;
		fil = fila.text;

		if (Input.GetKeyDown (KeyCode.Return)) {
			columna.text = "";
			fila.text = "";

			mensaje = "Raton " + ratonInterrogado;

			if (preguntaSobre == "Nadie") {
				mensaje += " ¿Has visto a alguien";
			} else {
				mensaje += " ¿Has visto al raton " + preguntaSobre;
			}

			if (accion == "Comer Queso")
				mensaje += " comerse el queso";
			else if (accion == "Romper Shoji")
				mensaje += " romer un shoji";
			else if (accion == "Jugar")
				mensaje += " jugar";

			if (col == "" || fil == "") {
				mensaje += "?";
			} else {
				mensaje += " en la posicion (" + col + "," + fil + ")?";
			}
			
			CmdEnviar (mensaje);
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
