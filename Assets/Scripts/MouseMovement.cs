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

    [SyncVar]
    public int m_PlayerNumber = 0;//1;
    public GameObject manager;

    public bool juegoAcabado = false;
	public bool finInterrogatorio = false;

	private bool panelActivo = false;//Esta variable se usa para no estar poniendo a true el setActive() de los paneles del interrogatorio

    [SyncVar]
	public Color mi_color = Color.red;

	public int mis_puntos;

	public int rol;
    
    [SyncVar]
    int mi_turno;
    

    public bool move = false;
    public bool te_has_movido = false;

    public int max_turnos = 10;
	public int numPreguntas = 0;

	float temporizador;

    private void Start()
	{
		Renderer[] rends = GetComponentsInChildren<Renderer> ();
		rends [0].material.color = mi_color;
		mis_puntos = 0;
	
		manager = GameObject.Find ("GameManager");
		if (mi_color != Color.magenta){
			//manager.GetComponent<GameManager> ().panelResto.SetActive (false);
			manager.GetComponent<GameManager> ().IncrementaRatones ();
			manager.GetComponent<GameManager> ().GenerarLista (mi_color);
			m_PlayerNumber = manager.GetComponent<GameManager> ().contadorRatones;
			manager.GetComponent<GameManager> ().m_Mouses [m_PlayerNumber - 1] = this.gameObject;
			this.gameObject.transform.position = manager.GetComponent<MazeBuilder> ().m_SpawnList [m_PlayerNumber - 1].transform.position;

			rol = Random.Range (1, 5);

		}
        else
        {
			rol = 0;
			numPreguntas = manager.GetComponent<GameManager> ().totalPreguntas;
            manager.GetComponent<GameManager>().ratonMorado = this.gameObject;
            this.gameObject.transform.position = new Vector3(0, 20, 0);
            foreach (Renderer render in rends)
            {
                render.enabled = false;
            }
            //gameObject.SetActive(false);
        }

		manager.GetComponent<GameManager> ().panelChat.SetActive (true);

		manager.GetComponent<GameManager> ().panelResto.SetActive (true);

		manager.GetComponent<GameManager> ().panelOtros.SetActive (true);

		if(isLocalPlayer)
			manager.GetComponent<GameManager> ().AsignarInformacionJugador (mi_color, rol);
    }
		
    private void Awake()
    {
		
    }

    public void moverse()
    {
        this.move = true;
    }

	[ClientRpc]
	void RpcTerminarJuego(){
        manager.GetComponent<GameManager>().FinJuego();
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

    [ClientRpc]
    void RpcEatCheese(Vector3 pos)
    {
        manager.GetComponent<GameManager>().CheeseChange(pos);
    }

    [Command]
    void CmdEatCheese(Vector3 pos)
    {
        RpcEatCheese(pos);
    }

    [ClientRpc]
    void RpcBreakShoji(Vector3 pos)
    {

        manager.GetComponent<GameManager>().ShojiChange(pos);
    }

    [Command]
    void CmdBreakShoji(Vector3 pos)
    {
        RpcBreakShoji(pos);
    }

	[Command]
	void CmdPreguntaRealizada(){
		RpcPreguntaRealizada ();
	}

	[ClientRpc]
	void RpcPreguntaRealizada(){
		manager.GetComponent<GameManager> ().totalPreguntas--;
	}

	public void PreguntaHecha(){
		if (isServer)
			RpcPreguntaRealizada();
		else
			CmdPreguntaRealizada();
	}

	[Command]
	void CmdRespuestaRealizada(){
		RpcRespuestaRealizada ();
	}

	[ClientRpc]
	void RpcRespuestaRealizada(){
		manager.GetComponent<GameManager> ().CambiarTurno ();
	}

	public void RespuestaHecha(){
		if (isServer)
			RpcRespuestaRealizada ();
		else
			CmdRespuestaRealizada ();
	}

	[ClientRpc]
	void RpcCambiarTiempo(){
		manager.GetComponent<GameManager> ().segundosTurno = temporizador;
	}

	[Command] 
	void CmdCambiarTiempo(){
		RpcCambiarTiempo ();
	}
		
    // Intentad que no haya tantas cosas en el Update, sino recurrir a eventos
    private void Update()
    {
        juegoAcabado = manager.GetComponent<GameManager>().juegoFinalizado;
		finInterrogatorio = manager.GetComponent<GameManager> ().fin;


		if (finInterrogatorio)
			return;

		if (juegoAcabado) {
			if (!panelActivo) {
				manager.GetComponent<GameManager> ().panelChat.SetActive (true);
				if (isLocalPlayer && mi_color == Color.magenta) {
					manager.GetComponent<GameManager> ().panelResto.SetActive (true);
				} else if (isLocalPlayer) {
					manager.GetComponent<GameManager> ().panelOtros.SetActive (true);
				}
				manager.GetComponent<GameManager> ().tuRol.text = "";
				panelActivo = true;
			}
			if (m_PlayerNumber != manager.GetComponent<GameManager> ().turno && mi_color != Color.magenta)
				GetComponent<Chat> ().enabled = false;
			else
				GetComponent<Chat> ().enabled = true;
			return;
		} else {
			manager.GetComponent<GameManager> ().panelResto.SetActive (false);
			manager.GetComponent<GameManager> ().panelOtros.SetActive (false);
			manager.GetComponent<GameManager> ().panelChat.SetActive (false);
		}


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

		temporizador -= Time.deltaTime;
		if (isServer)
			RpcCambiarTiempo();
		else
		{
			CmdCambiarTiempo();
		}
			

		if (temporizador <= 0) {
			CmdNotificarMovimiento ();
			temporizador = 10;
			return;
		}

		/*temporizador = manager.GetComponent<GameManager> ().segundosTurno;
		if (temporizador == 0) {
			if (isServer)
				RpcNotificarMovimiento();
			else
			{
				CmdNotificarMovimiento();
			}
		}*/
			
			

        /*if (mi_color == Color.magenta && juegoAcabado)
            manager.GetComponent<GameManager>().IniciarInterrogatorio();*/

        //Estoy hay que retocarlo, porque solo con el color no vale. Ya que lo cuenta como un raton
        /*if (mi_color == Color.magenta) {
			return;
		}*/
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
                switch (hit.collider.gameObject.layer)
                {
                    case 8: //Tile
                        if (Move(hit.collider.gameObject, pos))
                        {
                            if (rol == 2)
                                mis_puntos += 4;
                            else
                                mis_puntos++;
                            manager.GetComponent<GameManager>().cambiarPuntos(mis_puntos);
                        }; break;
                    case 9: //Cheese
                        Vector3 posCheese = hit.transform.position;
                        if (Eat(hit.collider.gameObject, posCheese, pos))
                        {

                            if (rol == 1)
                                mis_puntos += 200;
                            else
                                mis_puntos += 100;

                            manager.GetComponent<GameManager>().cambiarPuntos(mis_puntos);

                        }; break;
                    case 10: //Shoji
                        if (Move(hit.collider.gameObject, pos))
                        {
                            if (rol == 2)
                                mis_puntos += 4;
                            else
                                mis_puntos++;
                            manager.GetComponent<GameManager>().cambiarPuntos(mis_puntos);
                            if (rol == 3)
                                mis_puntos -= 3;
                            else if (rol == 4)
                                mis_puntos += 4;

                            manager.GetComponent<GameManager>().cambiarPuntos(mis_puntos);

                            if (isServer)
                            {
                                RpcBreakShoji(hit.collider.transform.position);
                            }
                            else
                            {
                                CmdBreakShoji(hit.collider.transform.position);
                            }
                            manager.GetComponent<GameManager>().BreakShoji(hit.collider.transform.position);
                        }; break; 
                    case 12: //Player
                        if (Move(hit.collider.gameObject, pos))
                        {
                            if (rol == 2)
                                mis_puntos += 4;
                            else
                                mis_puntos++;
                            manager.GetComponent<GameManager>().cambiarPuntos(mis_puntos);
                        }; break; 
                    case 13: //BrokenShoji
                        if (Move(hit.collider.gameObject, pos))
                        {
                            if (rol == 2)
                                mis_puntos += 4;
                            else
                                mis_puntos++;
                            manager.GetComponent<GameManager>().cambiarPuntos(mis_puntos);
                        }; break; 
                }                                           
            }
        }
    }

    // Podría funcionar de esta manera: marcar la casilla de origen como libre, la de llegada como ocupada y cambiar la variables MOVING para que se vaya moviendo... 
    // pero puedes pasar ya el turno y dejar que se muevan otro jugador si quiere (aunque todavía siga moving el ratón, en sus updates) 
    public bool Move(GameObject tile, Vector3 position)
    {
        bool moved = false;
        Vector3 pos;

        pos = tile.gameObject.transform.position;
        pos.x = (int) pos.x / 10;
        pos.z = (int) pos.z / 10;       
        pos.y = 0.75f;

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
        return moved;
    }

    private bool Eat(GameObject cheese, Vector3 positionCheese, Vector3 position)
    {
        bool moved = false;
        Vector3 pos = positionCheese;
        pos.z = (int) pos.z / 10;
        pos.x = (int) pos.x / 10;
        pos.y = 0.75f;

        // Reutilizar el código de Mover (sacarlo a una función auxiliar)... y luego hacéis el Destroy
        if (position.z + 1 == pos.z && position.x == pos.x)
        {

            StartCoroutine(MoveAnimation(0, 1, positionCheese));
            moved = true;
        }
        if (position.z - 1 == pos.z && position.x == pos.x)
        {

            StartCoroutine(MoveAnimation(0, -1, positionCheese));
            moved = true;
        }
        if (position.z == pos.z && position.x + 1 == pos.x)
        {

            StartCoroutine(MoveAnimation(1, 0, positionCheese));
            moved = true;
        }
        if (position.z == pos.z && position.x - 1 == pos.x)
        {

            StartCoroutine(MoveAnimation(-1, 0, positionCheese));
            moved = true;
        }
        if (moved)
        {

            GameManager gm = manager.GetComponent<GameManager>();
            if (isServer)
            {
                RpcEatCheese(positionCheese);
            }
            else
            {
                CmdEatCheese(positionCheese);
            }
            manager.GetComponent<GameManager>().EatCheese(positionCheese);
            
        }

        return moved;
    }

    // Me parece más intuitivo poner el origen primero, y luego el destino...
    // Podrías tener una variable booleana MOVING, y este código tenerlo en el Update, 
    // de manera que si está moviéndose que haga el moverse multiplicando LA VELOCIDAD por el DELTATIME (así el movimiento se hace todo lo suave que se pueda)

    private IEnumerator MoveAnimation(float x, float z, Vector3 position)
    {
        Quaternion rotation = new Quaternion();
        if (x == 0 && z == 1)
        {
            rotation = Quaternion.Euler(0, 0, 0);
        }
        if (x == 0 && z == -1)
        {
            rotation = Quaternion.Euler(0, 180, 0);
        }
        if (x == 1 && z == 0)
        {
            rotation = Quaternion.Euler(0, 90, 0);
        }
        if (x == -1 && z == 0)
        {
            rotation = Quaternion.Euler(0, 270, 0);
        }
        for (int c=0; c<10; c++)
        {
            Vector3 pos = this.gameObject.transform.position;
            pos.Set(pos.x + x, pos.y, pos.z + z);
            this.gameObject.transform.SetPositionAndRotation(pos, this.gameObject.transform.rotation);
            yield return 0.1;
        }
        Vector3 posTile = position;
        posTile.y = 0.75f;
        this.gameObject.transform.SetPositionAndRotation(posTile, rotation);
        if (isServer)
            RpcNotificarMovimiento();
        else
        {
            CmdNotificarMovimiento();
        }
		temporizador = 10;
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

    public void CheckVision()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        RaycastHit hit;
        Vector3 position = this.transform.position;
        bool foundChange = false;
        do
        {
            if (Physics.Raycast(position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                foundChange = CheckHit(hit);
            }
            position = hit.collider.transform.position;
        } while (foundChange);
        position = this.transform.position;
        foundChange = false;
        do
        {
            if (Physics.Raycast(position, transform.TransformDirection(Vector3.back), out hit, Mathf.Infinity))
            {
                foundChange = CheckHit(hit);
            }
            position = hit.collider.transform.position;
        } while (foundChange);
        position = this.transform.position;
        foundChange = false;
        do
        {
            if (Physics.Raycast(position, transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity))
            {
                foundChange = CheckHit(hit);
            }
            position = hit.collider.transform.position;
        } while (foundChange);
        position = this.transform.position;
        foundChange = false;
        do
        {
            if (Physics.Raycast(position, transform.TransformDirection(Vector3.left), out hit, Mathf.Infinity))
            {
                foundChange = CheckHit(hit);
            }
            position = hit.collider.transform.position;
        } while (foundChange);

    }

    private bool CheckHit(RaycastHit hit)
    {
        bool foundChange = false;
        Vector3 pos = new Vector3(hit.collider.gameObject.transform.position.x / 10, hit.collider.gameObject.transform.position.y / 10, hit.collider.gameObject.transform.position.z / 10);
        if (hit.collider.gameObject.layer == 9)
        {
            if (hit.collider.gameObject.GetComponent<CheeseManager>().Eat)
            {
                manager.GetComponent<GameManager>().EatCheese(pos);
                foundChange = true;
            }
        }
        else if (hit.collider.gameObject.layer == 10)
        {
            if (hit.collider.gameObject.GetComponent<ShojiManager>().Broken)
            {
                manager.GetComponent<GameManager>().BreakShoji(pos);
                foundChange = true;
            }
        }
        else if (hit.collider.gameObject.layer == 12)
        {         
            Renderer[] rends = hit.collider.gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer render in rends)
            {
                render.enabled = true;
            }
            foundChange = true;
        }
        return foundChange;
    }

    public void DoInvisible()
    {
        if (!isLocalPlayer)
        {
            Renderer[] rends = this.GetComponentsInChildren<Renderer>();
            foreach (Renderer render in rends)
            {
                render.enabled = false;
            }
        }
    }

}
