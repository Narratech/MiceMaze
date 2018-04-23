using System;
using UnityEngine;

// No se usa... revisadlo y BORRADLO si no hace falta

[Serializable]
public class MouseManager
{


    public Color m_PlayerColor = Color.red;
    public Transform m_SpawnPoint;

    [HideInInspector] public int m_PlayerNumber;
    [HideInInspector] public GameObject m_Instance;
    public Vector3 m_Position;

    private MouseMovement m_Movement;

    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<MouseMovement>();

        m_Movement.m_PlayerNumber = m_PlayerNumber;




        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            // ... set their material color to the color specific to this tank.
            renderers[i].material.color = m_PlayerColor;
        }
    }

    public void SetPosition(Vector3 position)
    {
        m_Position = position;
    }

    public bool Move(GameObject tile, Vector3 position, out Vector3 newPosition)
    {
        bool moved = false;

        newPosition = position;

        if (m_Movement.Move(tile, position, true))
        {

            newPosition = tile.GetComponent<TileManager>().GetPosition();
            newPosition.y = 2.5f;
            moved = true;
        }


        return moved;

    }

    public void DisableControl()
    {
        m_Movement.enabled = false;

    }

    public void EnableControl()
    {
        m_Movement.enabled = true;

    }

    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}

