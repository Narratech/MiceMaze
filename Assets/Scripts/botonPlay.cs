using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class botonPlay : MonoBehaviour {

	public void CambiarEscena(string nombre){
        // Creo que es mejor usar Debug.Log();
		print ("Cambiando a la escena " + nombre);
		SceneManager.LoadScene (nombre);
	}

	public void Salir(){
		print ("Saliendo");
		Application.Quit ();//en el modo debug no funciona, pero a la hora de exportar el proyecto si que se cierra
	}
}
