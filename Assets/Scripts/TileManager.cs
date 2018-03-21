using UnityEngine;

public class TileManager : MonoBehaviour {

	public Vector3 position;
	public GameObject contains;

 
    public void SetPosition(int x, int z)
	{
		position.x = x;
		position.y = 0;
        position.z = z;
	}

	public Vector3 GetPosition()
	{
		return position;
	}

	public void SetContains(GameObject gameObject)
	{
		contains = gameObject;
	}

	public GameObject GetContains()
	{
		return contains;
	}
}
