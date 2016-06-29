using UnityEngine;
using System.Collections;

public class ParallaxLayer : MonoBehaviour 
{
    public float SpeedCoef;
    
    public void UpdateLayer(Vector2 deltaMovement)
    {
        Vector2 movement = deltaMovement * SpeedCoef;

        transform.position += (Vector3)movement;
    }
}
