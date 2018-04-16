/**    
    Copyright (C) 2018 Narratech Laboratories
    http://www.narratech.com

    This file is part of MiceMaze project, a test-bed game for intelligent agents.

    Contact: info@narratech.com
*/

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// Renombrad ahora como el MouseManager
public class MouseMovement : NetworkBehaviour{
	//SE LE ASIGNA AL CIENTIFICO EL COLOR MAGENTA, ES DECIR, Color.magenta
	//EL RESTO DE COLORES CORRESPONDEN A UN JUGADOR DE TIPO RATON

    [SyncVar]
    public int m_PlayerNumber = 0;//1;
    public GameObject manager;

	[SyncVar]
	public Color mi_color = Color.red;

	public int puntos = 0;
	public Text mis_puntos;

	[SyncVar]
	public bool quesoComido = false;
    

    [SyncVar]
    int mi_turno;
    

    public bool move = false;
    public bool te_has_movido = false;

    
    private void Start()
    {
		Renderer[] rends = GetComponentsInChildren<Renderer> ();
        rends[0].material.color = mi_color; 
		//mis_puntos.text = "" + puntos;
		
        manager = GameObject.Find("GameManager");
        manager.GetComponent<GameManager>().IncrementaRatones();
        m_PlayerNumber = manager.GetComponent<GameManager>().contadorRatones;
        manager.GetComponent<GameManager>().m_Mouses[m_PlayerNumber - 1] = this.gameObject;
        this.gameObject.transform.position = manager.GetComponent<MazeBuilder>().m_SpawnList[m_PlayerNumber - 1].transform.position;
		
    }

    private void Awake()
    {
		mis_puntos.text = "" + puntos;
    }


    public void moverse()
    {
        this.move = true;
    }

	[ClientRpc]
	void RpcTerminarJuego(){
		manager.GetComponent<GameManager> ().FinJuego ();
	}

	[Command]
	void CmdTerminarJuego()
	{
		RpcTerminarJuego();
	}

    [ClientRpc]
    void RpcNotificarMovimiento()
    {
        manager.GetComponent<GameManager>().CambiarTurno();
    }

    [Command]
    void CmdNotificarMovimiento()
    {
        RpcNotificarMovimiento();
    }

    // Intentad que no haya tantas cosas en el Update, sino recurrir a eventos
    private void Update()
    {
		
        RaycastHit hit;
        Ray ray;
        int layerMask = 1 << 8;
        Vector3 pos = new Vector3();

        if (!isLocalPlayer)
        {
            return;
        }

        if(m_PlayerNumber != manager.GetComponent<GameManager>().turno)
        {
            return;
        }
       
		if (mi_color == Color.magenta) {
			return;
		}
        // Se me hace raro verlo aquí... a lo mejor se podría recibir el clic EN GENERAL (de hecho se puede recibir el de la casilla... que
        // esta vez si es el jugador local el que la ha clicado, para coger el color y 
        // luego vosotros tener un EVENTO PROPIO que recibe este MouseManager, y que las casillas cuando son clicadas, notifiquen en general "ESTE JUGADOR -COLOR TAL- ha clicado la casilla la X, Y"), 
        if (Input.GetMouseButtonDown(0))
        {

            
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, layerMask))
            {
                pos.x = (int)this.gameObject.transform.position.x / 10;
                pos.y = 0.75f;
                pos.z = (int)this.gameObject.transform.position.z / 10;
                if(hit.collider.gameObject.layer == 8)
                {
                    if(Move(hit.collider.gameObject, pos))
                    {
                        if (isServer)
                            RpcNotificarMovimiento();
                        else
                            CmdNotificarMovimiento();
                        
                    }
                }if(hit.collider.gameObject.layer == 9)
                {

                    Vector3 posCheese = hit.transform.position;



                    if (Eat(hit.collider.gameObject, posCheese, pos))
                    {
						if (isServer)
							RpcTerminarJuego ();
						else
							CmdTerminarJuego ();
						/*quesoComido = true;
                        if (isServer)
                            RpcNotificarMovimiento();
                        else
                            CmdNotificarMovimiento();*/
                    }
                }
               
            }
        }
    }

    // Podría funcionar de esta manera: marcar la casilla de origen como libre, la de llegada como ocupada y cambiar la variables MOVING para que se vaya moviendo... 
    // pero puedes pasar ya el turno y dejar que se muevan otro jugador si quiere (aunque todavía siga moving el ratón, en sus updates) 
    public bool Move(GameObject tile, Vector3 position)
    {
        bool moved = false;
        GameObject contains = tile.GetComponent<TileManager>().contains;
        Vector3 pos = tile.GetComponent<TileManager>().GetPosition();
        pos.y = 0.75f;
     
        if (contains == null)
        {
            // Faltaría añadir la rotación
            if (position.z + 1 == pos.z && position.x == pos.x)
            {
                StartCoroutine(MoveAnimation(0, 1, tile.transform.position));
                moved = true;
            }
            if (position.z - 1 == pos.z && position.x == pos.x)
            {
                StartCoroutine(MoveAnimation(0, -1, tile.transform.position));
                moved = true;
            }
            if (position.z == pos.z && position.x + 1 == pos.x)
            {
                StartCoroutine(MoveAnimation(1, 0, tile.transform.position));
                moved = true;
            }
            if (position.z == pos.z && position.x - 1 == pos.x)
            {
                StartCoroutine(MoveAnimation(-1, 0, tile.transform.position));
                moved = true;
            }
           

        }
        
        return moved;
    }

    private bool Eat(GameObject cheese, Vector3 positionCheese, Vector3 position)
    {
        bool moved = false;
        Vector3 pos = positionCheese;
        pos.z = pos.z / 10;
        pos.x = pos.x / 10;
        pos.y = 0.75f;

        // Reutilizar el código de Mover (sacarlo a una función auxiliar)... y luego hacéis el Destroy
        if (position.z + 1 == pos.z && position.x == pos.x)
        {
            Destroy(cheese);
            StartCoroutine(MoveAnimation(0, 1, positionCheese));
            moved = true;
        }
        if (position.z - 1 == pos.z && position.x == pos.x)
        {
            Destroy(cheese);
            StartCoroutine(MoveAnimation(0, -1, positionCheese));
            moved = true;
        }
        if (position.z == pos.z && position.x + 1 == pos.x)
        {
            Destroy(cheese);
            StartCoroutine(MoveAnimation(1, 0, positionCheese));
            moved = true;
        }
        if (position.z == pos.z && position.x - 1 == pos.x)
        {
            Destroy(cheese);
            StartCoroutine(MoveAnimation(-1, 0, positionCheese));
            moved = true;
        }

        return moved;
    }

    // Me parece más intuitivo poner el origen primero, y luego el destino...
    // Podrías tener una variable booleana MOVING, y este código tenerlo en el Update, 
    // de manera que si está moviéndose que haga el moverse multiplicando LA VELOCIDAD por el DELTATIME (así el movimiento se hace todo lo suave que se pueda)

    private IEnumerator MoveAnimation(float x, float z, Vector3 position)
    {
        for(int c=0; c<10; c++)
        {
            Vector3 pos = this.gameObject.transform.position;
            pos.Set(pos.x + x, pos.y, pos.z + z);
            this.gameObject.transform.SetPositionAndRotation(pos, this.gameObject.transform.rotation);
            yield return 0.1;
        }
        Vector3 posTile = position;
        posTile.y = 0.75f;
        Quaternion rotation = new Quaternion();
        this.gameObject.transform.SetPositionAndRotation(posTile, rotation);
    }

    public void DisableControl()
    {
        this.enabled = false;
        move = false;
    }

    public void EnableControl()
    {
        this.enabled = true;

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cheese"))
        {
            other.gameObject.SetActive(false);
        }
    }


}
