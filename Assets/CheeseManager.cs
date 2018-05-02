using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseManager : MonoBehaviour {

    bool eat = false;

    public bool Eat
    {
        get { return eat; }
        set { eat = value; }
    }
}
