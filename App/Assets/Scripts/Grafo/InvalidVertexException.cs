using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InvalidVertexException : Exception
{
	public InvalidVertexException(string msg)
	{
		Debug.Log(msg);
	}


}
