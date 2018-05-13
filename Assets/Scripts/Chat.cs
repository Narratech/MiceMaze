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

	public Color c;
	//public GameObject panelOtros;
	//public GameObject panelResto;
	//public GameObject manager;

	//Interfaz para los ratones, es decir, para los que no son cientificos
	InputField columnaRespuesta;
	InputField filaRespuesta;
	Dropdown respuesta;
	Dropdown ratones;
	Dropdown accionesRatones;

	// Use this for initialization
	void Start () {
		texto = GameObject.Find ("PanelTexto").GetComponent<Text> ();
		asignarColor ();
		if (raton.GetComponent<MouseMovement>().mi_color == Color.magenta) {
			ratonPreguntado = GameObject.Find ("Raton1").GetComponent<Dropdown> ();
			preguntoPor = GameObject.Find ("Raton2").GetComponent<Dropdown> ();
			columna = GameObject.Find ("EntradaColumna").GetComponent<InputField> ();
			fila = GameObject.Find ("EntradaFila").GetComponent<InputField> ();
			accionesPosibles = GameObject.Find ("DropdownAcciones").GetComponent<Dropdown> ();
		} else {
			//input = GameObject.Find ("EntradaTexto").GetComponent<InputField> ();
			columnaRespuesta = GameObject.Find ("EntradaColumnaRespuesta").GetComponent<InputField> ();
			filaRespuesta = GameObject.Find ("EntradaFilaRespuesta").GetComponent<InputField> ();
			respuesta = GameObject.Find ("DropdownDirecta").GetComponent<Dropdown> ();
			ratones = GameObject.Find ("DropdownRaton").GetComponent<Dropdown> ();
			accionesRatones = GameObject.Find ("DropdownRespuestaAcciones").GetComponent<Dropdown> ();
		}
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

	void Update(){
		if (!isLocalPlayer)
			return;

		string ratonInterrogado;//raton que está siendo interrogado
		string preguntaSobre;//raton sobre el que estás preguntando
		string dropdownRespuesta;
		string col;//columna
		string fil;//fila
		string accion;//accion sobre la que quieres preguntar
		string mensaje;//mensaje que se va a producir para hacer la pregunta

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

			if (colorJugador != "Magenta" /*&& input.text != ""*/) {
				/*mensaje = input.text;
				input.text = "";*/

				valorDropdown = respuesta.value;
				dropdownRespuesta = respuesta.options [valorDropdown].text; //Esto es Sí, No o No lo sé

				valorDropdown = ratones.value;
				preguntaSobre = ratones.options [valorDropdown].text;//Esto tiene el valor de los ratones que hay en partida

				valorDropdown = accionesRatones.value;
				accion = accionesRatones.options [valorDropdown].text;//Esto tiene el valor de las acciones posibles en el juego

				col = columnaRespuesta.text;
				fil = filaRespuesta.text;

				columnaRespuesta.text = "";
				filaRespuesta.text = "";

				if (preguntaSobre == "Nadie" && accion == "Ninguna") {
					mensaje = dropdownRespuesta;
				} else if (preguntaSobre == "Nadie") {
					if (dropdownRespuesta == "Sí")
						mensaje = "Si vi a alguien";
					else if (dropdownRespuesta == "No")
						mensaje = "No vi a nadie";
					else
						mensaje = "";

					if (accion == "Comer Queso")
						mensaje += " comerse el queso";
					else if (accion == "Romper Shoji")
						mensaje += " romer un shoji";
					else if (accion == "Jugar")
						mensaje += " jugar";

					if (col == "" || fil == "") {
						mensaje += ".";
					} else {
						mensaje += " en la posicion (" + col + "," + fil + ").";
					}
				} else {
					if (dropdownRespuesta == "Sí")
						mensaje = dropdownRespuesta + ". Vi al raton " + preguntaSobre;
					else if (dropdownRespuesta == "No")
						mensaje = "No vi al raton " + preguntaSobre;
					else
						mensaje = "";

					if (accion == "Comer Queso")
						mensaje += " comerse el queso";
					else if (accion == "Romper Shoji")
						mensaje += " romer un shoji";
					else if (accion == "Jugar")
						mensaje += " jugar";

					if (col == "" || fil == "") {
						mensaje += ".";
					} else {
						mensaje += " en la posicion (" + col + "," + fil + ").";
					}
				}

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

	/*string GenerarAccion(string accion, string mensaje){
		if (accion == "Comer Queso")
			mensaje += " comerse el queso";
		else if (accion == "Romper Shoji")
			mensaje += " romer un shoji";
		else if (accion == "Jugar")
			mensaje += " jugar";

		return mensaje;
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