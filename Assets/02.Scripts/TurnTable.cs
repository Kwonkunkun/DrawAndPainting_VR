using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTable : MonoBehaviour
{
    public void Right60Rotate()
    {
        transform.Rotate(new Vector3(0, 60, 0));
    }
    public void Left60Rotate()
    {
        transform.Rotate(new Vector3(0, -60, 0));
    }
}
