using System.Collections;
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

 	//PARA EL INTERROGATORIO
	public Dropdown ratonPreguntado;
	public Dropdown preguntoPor;
	public Dropdown dropdownratones;
	//FIN

	public GameObject panelOtros;
	public GameObject panelResto;
	public GameObject panelChat;

	public int totalTurnos = 20;

    void Start()
    {
        var configuration = ConfigurationXml.Load();
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
		/*prueba.color = Color.red;
		prueba.text = "" + turno;*/
		puntosRaton = 0;
		puntuacion.text = "" + puntosRaton;
		//Este botón debe activarse cuando el juego haya finalizado y el científico crea saber quien es el culpable
		interrogatorio.enabled = !interrogatorio.enabled;
    }

	void Update(){
		if (totalTurnos == 0)
			FinJuego ();
	}

	public void GenerarLista(Color c){
		if (c == Color.red) {
			List<string> nombre = new List<string> (){ "Rojo" };
			ratones.AddOptions (nombre);
			ratonPreguntado.AddOptions(nombre);
			preguntoPor.AddOptions(nombre);
			dropdownratones.AddOptions (nombre);
		} else if (c == Color.green) {
			List<string> nombre = new List<string> (){ "Verde" };
			ratones.AddOptions (nombre);
			ratonPreguntado.AddOptions(nombre);
			preguntoPor.AddOptions(nombre);
			dropdownratones.AddOptions (nombre);
		} else if (c == Color.yellow) {
			List<string> nombre = new List<string> (){ "Amarillo" };
			ratones.AddOptions (nombre);
			ratonPreguntado.AddOptions(nombre);
			preguntoPor.AddOptions(nombre);
			dropdownratones.AddOptions (nombre);
		} else if (c == Color.blue) {
			List<string> nombre = new List<string> (){ "Azul" };
			ratones.AddOptions (nombre);
			ratonPreguntado.AddOptions(nombre);
			preguntoPor.AddOptions(nombre);
			dropdownratones.AddOptions (nombre);
		} else {

            Destroy(GetComponent<MouseMovement> ().gameObject);
        }

        coloresJugadores [indice] = c;
		indice++;

		prueba.color = coloresJugadores [turno - 1];
		ponerColor (coloresJugadores [turno - 1]);
	}

    public void IncrementaRatones()
    {
        contadorRatones++;
    }

	public void cambiarPuntos(int valor){
		puntosRaton = valor;
		puntuacion.text = "" + puntosRaton;
	}
		
	void ponerColor(Color c){
		if (c == Color.red)
			prueba.text = "Rojo";
		else if (c == Color.green)
			prueba.text = "Verde";
		else if (c == Color.blue)
			prueba.text = "Azul";
		else if (c == Color.yellow)
			prueba.text = "Amarillo";
	}
    
    public void CambiarTurno()
    {
        InvisibleMouses();
        CheckVision();
        turno++;
        if(turno > contadorRatones)
        {
            turno = 1;
        }
		totalTurnos--;
		//prueba.text = "" + turno;
		prueba.color = coloresJugadores[turno - 1];
		ponerColor (coloresJugadores [turno - 1]);
    }
		

    public void EatCheese(Vector3 pos)
    {
        
        manager.GetComponent<MazeBuilder>().GetTile(pos).GetComponent<TileManager>().contains.SetActive(false);
    }

    public void CheeseChange(Vector3 pos)
    {
        GameObject cheese = manager.GetComponent<MazeBuilder>().GetTile(pos).GetComponent<TileManager>().contains;
        cheese.GetComponent<CheeseManager>().Eat = true;

		//Aquí asignamos el culpable
		ultimoTurno = turno;
		culpable = coloresJugadores [ultimoTurno - 1];
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

    public void FinJuego()
    {
        juegoFinalizado = true;
        //ultimoTurno = turno;
        turno = 0;
        prueba.text = "SE ACABÓ";
        //culpable = coloresJugadores[ultimoTurno - 1];
    }

    public void IniciarInterrogatorio()
    {
        interrogatorio.enabled = !interrogatorio.enabled;
    }

    public void FaseInterrogatorio()
    {
        valorDropdown = ratones.value;
        mensajeDropdown = ratones.options[valorDropdown].text;
		string escena;

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

    private void InvisibleMouses()
    {
       for(int c = 0; c< contadorRatones; c++)
       {
            m_Mouses[c].GetComponent<MouseMovement>().DoInvisible();
            
       }
    }

    private void CheckVision()
    {
        for (int c = 0; c < contadorRatones; c++)
        {
            m_Mouses[c].GetComponent<MouseMovement>().CheckVision();

        }
    }

}
