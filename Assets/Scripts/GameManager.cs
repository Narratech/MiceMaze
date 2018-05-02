﻿using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class GameManager :  NetworkBehaviour
{

    public int m_NumTurnos;
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public GameObject[] m_Mouses = new GameObject[4];
    public GameObject m_MousePrefab;
    public GameObject m_BrokenShoji;

    public Text prueba;// texto para indicar el turno en pantalla
	public Button interrogatorio;
	public Dropdown ratones;

	//Para obtener el valor del Dropdown
	public string mensajeDropdown;
	private int valorDropdown;

	public Text puntuacion;
	public int puntosRaton;

    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private GameObject manager;
    private TileManager[] m_Tiles = new TileManager[100];

    [SyncVar]
    public int turno = 1;

	public int ultimoTurno;
	public Color[] coloresJugadores = new Color[4];
	public int indice = 0;



    public int contadorRatones = 0;

	public Color culpable;//color del jugador que se ha comido el queso
    public bool juegoFinalizado = false;

    public bool tienesRatonMorado = false;
    public GameObject ratonMorado;

    void Start()
    {
        manager = GameObject.Find("GameManager");
        manager.GetComponent<MazeBuilder>().ChargeMaze();
        int c = 0;
        foreach(GameObject tile in manager.GetComponent<MazeBuilder>().m_Maze)
        {
            m_Tiles[c] = manager.GetComponent<MazeBuilder>().m_Maze[c].GetComponent<TileManager>();
            c++;
        }
        
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);
		prueba.color = Color.red;
		prueba.text = "" + turno;
		puntosRaton = 0;
		puntuacion.text = "" + puntosRaton;
		//Este botón debe activarse cuando el juego haya finalizado y el científico crea saber quien es el culpable
		interrogatorio.enabled = !interrogatorio.enabled;
    }

	public void GenerarLista(Color c){
		if (c == Color.red) {
			List<string> nombre = new List<string> (){ "Rojo" };
			ratones.AddOptions (nombre);
		} else if (c == Color.green) {
			List<string> nombre = new List<string> (){ "Verde" };
			ratones.AddOptions (nombre);
		} else if (c == Color.yellow) {
			List<string> nombre = new List<string> (){ "Amarillo" };
			ratones.AddOptions (nombre);
		} else if (c == Color.blue) {
			List<string> nombre = new List<string> (){ "Azul" };
			ratones.AddOptions (nombre);
		} else {

            Destroy(GetComponent<MouseMovement> ().gameObject);
        }

coloresJugadores [indice] = c;
		indice++;
	}

    public void IncrementaRatones()
    {
        contadorRatones++;
    }

	public void cambiarPuntos(int valor){
		puntosRaton = valor;
		puntuacion.text = "" + puntosRaton;
	}
		
    
    public void CambiarTurno()
    {
        turno++;
        if(turno > contadorRatones)
        {
            turno = 1;
        }

		prueba.text = "" + turno;
    }
		

    public void EatCheese(Vector3 pos)
    {
        
        Destroy(manager.GetComponent<MazeBuilder>().GetTile(pos).GetComponent<TileManager>().contains);
    }

    public void CheeseChange(Vector3 pos)
    {
        GameObject cheese = manager.GetComponent<MazeBuilder>().GetTile(pos).GetComponent<TileManager>().contains;
        cheese.GetComponent<CheeseManager>().Eat = true;
    }

    public void BreakShoji(Vector3 pos)
    {
        int x = (int)pos.x;
        int z = (int)pos.z;
        int position = z * 10 + x;
        /*Vector3 position = manager.GetComponent<MazeBuilder>().GetTile(pos).GetComponent<TileManager>().contains.gameObject.transform.position;
        position.y = 0f;*/
        TileManager tile = m_Tiles[position];
        Quaternion rotation = tile.contains.gameObject.transform.rotation;
        pos = new Vector3(pos.x*10, 0, pos.z*10);
        GameObject brokenShoji = Instantiate(m_BrokenShoji, pos, rotation);
        Destroy(manager.GetComponent<MazeBuilder>().GetTile(pos).GetComponent<TileManager>().contains);
        manager.GetComponent<MazeBuilder>().GetTile(pos).GetComponent<TileManager>().SetContains(brokenShoji);
    }

    public void ShojiChange(Vector3 pos)
    {
        Vector3 position = pos;
        manager.GetComponent<MazeBuilder>().GetTile(pos).GetComponent<TileManager>().contains.GetComponent<ShojiManager>().Broken = true;
        float c = pos.x;
    }

    public void FinJuego(GameObject obj)
    {
        juegoFinalizado = true;
        ultimoTurno = turno;
        turno = 0;
        prueba.text = "SE ACABÓ";
        culpable = coloresJugadores[ultimoTurno - 1];
        if (tienesRatonMorado)
            ratonMorado.SetActive(true);
        //interrogatorio.enabled = !interrogatorio.enabled;
    }

    public void IniciarInterrogatorio()
    {
        interrogatorio.enabled = !interrogatorio.enabled;
    }

    public void FaseInterrogatorio()
    {
        valorDropdown = ratones.value;
        mensajeDropdown = ratones.options[valorDropdown].text;
        //SceneManager.LoadScene (nombre);
        if ((mensajeDropdown == "Azul" && culpable == Color.blue) ||
            (mensajeDropdown == "Rojo" && culpable == Color.red) ||
            (mensajeDropdown == "Amarillo" && culpable == Color.yellow) ||
            (mensajeDropdown == "Verde" && culpable == Color.green))
        {

            SceneManager.LoadScene("EscenaWin");
        }
        else
        {
            SceneManager.LoadScene("EscenaLoser");
        }

    }


    // Update is called once per frame
    private IEnumerator GameLoop()
    {
    
        yield return StartCoroutine(RoundStarting());

        yield return StartCoroutine(RoundPlaying());

        yield return StartCoroutine(RoundEnding());
    }

    private IEnumerator RoundStarting()
    {
        
        DisableMouseControl();
       
        yield return m_StartWait;
    }

    private IEnumerator RoundPlaying()
    {
        for (int c = 0; c<m_NumTurnos; c++)
        {
            for(int i = 0; i<contadorRatones; i++)
            {
                EnableMouseControl(i);
                while (!m_Mouses[i].GetComponent<MouseMovement>().move)
                {
                    yield return null;
                }
                DisableMouseControl();
            }
        }
       
    }

    private IEnumerator RoundEnding()
    {
        DisableMouseControl();

        yield return m_EndWait;
    }

    // This function is used to turn all the tanks back on and reset their positions and properties.
 

    private void EnableMousesControl()
    {
        for (int i = 0; i < contadorRatones; i++)
        {
            m_Mouses[i].GetComponent<MouseMovement>().EnableControl();
        }
    }

    private void EnableMouseControl(int i)
    {

         m_Mouses[i].GetComponent<MouseMovement>().EnableControl();

    }

    private void DisableMouseControl()
    {
        for (int i = 0; i < contadorRatones; i++)
        {
           m_Mouses[i].GetComponent<MouseMovement>().DisableControl();
        }
    }

    public void InvisibleMouses(int playerMouse)
    {
       for(int c = 0; c< contadorRatones; c++)
       {
            if(c != playerMouse)
            {
                Renderer[] rends = m_Mouses[c].GetComponentsInChildren<Renderer>();
                foreach(Renderer render in rends)
                {
                    render.enabled = false;
                }
               

            } 
       }
    }

}
