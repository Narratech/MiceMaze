using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class GameManager :  NetworkBehaviour
{

    public int m_NumTurnos;
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public GameObject[] m_Mouses = new GameObject[5];
    public GameObject m_MousePrefab;

	public Text prueba;

    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;

    [SyncVar]
    public int turno = 1;

    public int contadorRatones = 0;
    // Use this for initialization

    

    void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);
		prueba.color = Color.red;
		prueba.text = "" + turno;
        //SpawnAllMouses();

        //StartCoroutine(GameLoop());
    }

    public void IncrementaRatones()
    {
        contadorRatones++;
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
      
}
