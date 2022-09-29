using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InvalidEdgeException : Exception
{
	public InvalidEdgeException(string msg)
	{
		Debug.Log(msg);
	}


}

