using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface for defining the basics of noise filters
public interface INoiseFilter
{
    float Evaluate(Vector3 point);
}
