using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grafo
{

	public class Vertice<V> : Vertex<V>
	{

		protected V rotulo;


		public Vertice()
		{
			rotulo = default(V);
		}

		public void setRotulo(V r)
		{
			rotulo = r;
		}


		public V element()
		{
			return rotulo;
		}
	}
}

