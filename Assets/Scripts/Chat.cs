using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Chat : NetworkBehaviour {

	Text texto;
	InputField input;
	public MouseMovement raton;
	public string colorJugador;

	Dropdown ratonPreguntado;
	Dropdown preguntoPor;
	Dropdown accionesPosibles;

	InputField columna;
	InputField fila;


	public string mensajeDropdown;
	private int valorDropdown;

	public Color c;
	public GameObject panelOtros;
	public GameObject panelResto;
	public GameObject manager;

	//Button preguntado;

	// Use this for initialization
	void Start () {
		//texto = GameObject.Find ("PanelTexto").GetComponent<Text> ();
		asignarColor ();
		//if (colorJugador == "Magenta") {
			texto = GameObject.Find ("PanelTexto").GetComponent<Text> ();
			ratonPreguntado = GameObject.Find ("Raton1").GetComponent<Dropdown> ();
			preguntoPor = GameObject.Find ("Raton2").GetComponent<Dropdown> ();
			columna = GameObject.Find ("EntradaColumna").GetComponent<InputField> ();
			fila = GameObject.Find ("EntradaFila").GetComponent<InputField> ();
			accionesPosibles = GameObject.Find ("DropdownAcciones").GetComponent<Dropdown> ();
		//} else {
			input = GameObject.Find ("EntradaTexto").GetComponent<InputField> ();
		//}
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
		} else if (raton.mi_color == Color.magenta) {
			colorJugador = "Magenta";
		}
	}

	/*bool comprobarColor(Color colorRaton){
		string ratonInterrogado;

		valorDropdown = ratonPreguntado.value;
		ratonInterrogado = ratonPreguntado.options [valorDropdown].text;

		if (colorRaton == Color.red && ratonInterrogado == "Rojo")
			return true;
		else if (colorRaton == Color.blue && ratonInterrogado == "Azul")
			return true;
		else if (colorRaton == Color.green && ratonInterrogado == "Verde")
			return true;
		else if (colorRaton == Color.yellow && ratonInterrogado == "Amarillo")
			return true;

		return false;
	}*/
		
	void Update(){
		if (!isLocalPlayer)
			return;

		string ratonInterrogado;//raton que está siendo interrogado
		string preguntaSobre;//raton sobre el que estás preguntando
		string col;//columna
		string fil;//fila
		string accion;//accion sobre la que quieres preguntar
		string mensaje;//mensaje que se va a producir para hacer la pregunta

		//panelOtros.SetActive (false);

		/*if (colorJugador == "Magenta") {
			valorDropdown = ratonPreguntado.value;
			ratonInterrogado = ratonPreguntado.options [valorDropdown].text;

			valorDropdown = preguntoPor.value;
			preguntaSobre = preguntoPor.options [valorDropdown].text;

			valorDropdown = accionesPosibles.value;
			accion = accionesPosibles.options [valorDropdown].text;

			col = columna.text;
			fil = fila.text;
		}*/

		if (Input.GetKeyDown (KeyCode.Return)) {

			if (colorJugador != "Magenta" && input.text != "") {
				mensaje = input.text;
				input.text = "";
				CmdEnviar (mensaje);
			} else if(colorJugador == "Magenta"){
				valorDropdown = ratonPreguntado.value;
				ratonInterrogado = ratonPreguntado.options [valorDropdown].text;

				valorDropdown = preguntoPor.value;
				preguntaSobre = preguntoPor.options [valorDropdown].text;

				valorDropdown = accionesPosibles.value;
				accion = accionesPosibles.options [valorDropdown].text;

				col = columna.text;
				fil = fila.text;

				//Codigo que habia antes. Lo de arriba de este ELSE es lo añadido para probar
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
