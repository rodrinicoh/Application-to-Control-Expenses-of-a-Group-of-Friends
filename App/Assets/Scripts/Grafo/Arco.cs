using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grafo
{

	public class Arco<V, E> : Edge<E>
	{


		protected E rotulo;
		protected Vertice<V> sucesor;
		protected Vertice<V> predecesor;


		public Arco()
		{
			rotulo = default(E);
			sucesor = null;
			predecesor = null;
		}

		public void setRotulo(E r)
		{
			rotulo = r;
		}


		public void setPredecesor(Vertice<V> v1)
		{
			predecesor = v1;
		}

		public void setSucesor(Vertice<V> v1)
		{
			sucesor = v1;
		}

		public Vertice<V> getSucesor()
		{
			return sucesor;
		}

		public Vertice<V> getPredecesor()
		{
			return predecesor;
		}

		public E element()
		{
			return rotulo;
		}
	}
}