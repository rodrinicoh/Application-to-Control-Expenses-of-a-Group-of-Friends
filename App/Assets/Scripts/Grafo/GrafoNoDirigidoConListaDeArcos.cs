using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Grafo
{
	public class GrafoNoDirigidoConListaDeArcos<V, E> : Graph<V, E>
	{


		protected List<Vertice<V>> listaVertices;
		protected List<Arco<V, E>> listaArcos;

		public GrafoNoDirigidoConListaDeArcos()
		{
			listaVertices = new List<Vertice<V>>();
			listaArcos = new List<Arco<V, E>>();
		}

		public List<Vertex<V>> vertices()
		{
			List<Vertex<V>> aux = new List<Vertex<V>>();
			List<Vertice<V>> IT = listaVertices;
			foreach (Vertice<V> cursor in IT)
			{
				aux.Add(cursor);
			}
			return aux;
		}

		public List<Edge<E>> edges()
		{
			List<Edge<E>> aux = new List<Edge<E>>();
			List<Arco<V, E>> IT = listaArcos;
			foreach (Arco<V, E> cursor in IT)
			{
				aux.Add(cursor);
			}
			return aux;
		}

		public List<Edge<E>> incidentEdges(Vertex<V> v)
		{
			List<Edge<E>> aux = new List<Edge<E>>();
			Vertice<V> vertice = checkVertex(v);
			List<Arco<V, E>> IT = listaArcos;
			foreach (Arco<V, E> cursor in IT)
			{
				if ((cursor.getSucesor() == vertice) || (cursor.getPredecesor() == vertice))
					aux.Add(cursor);
			}
			return aux;
		}

		public Vertex<V> opposite(Vertex<V> v, Edge<E> e)
		{

			//se asume que el vertice pasado por parametro pertenece a uno de los 2 extremos del arco
			Vertice<V> vertice = checkVertex(v);
			Arco<V, E> arco = checkEdge(e);
			Vertex<V> resultado = null;
			if (arco.getPredecesor() == vertice)
				resultado = arco.getSucesor();
			if (arco.getSucesor() == vertice)
				resultado = arco.getPredecesor();
			if (resultado == null)
				throw new InvalidEdgeException("El arco no incide en vertice");
			return resultado;

		}

		public List<Vertex<V>> endvertices(Edge<E> e)
		{
			List<Vertex<V>> aux = new List<Vertex<V>>(2);
			Arco<V, E> arco = checkEdge(e);
			aux.Add(arco.getPredecesor());
			aux.Add(arco.getSucesor());
			return aux;
		}

		public bool areAdjacent(Vertex<V> v, Vertex<V> w)
		{
			Vertice<V> verticev = checkVertex(v);
			Vertice<V> verticew = checkVertex(w);
			bool encontre = false;
			List<Arco<V, E>> IT = listaArcos;
			foreach (Arco<V, E> cursor in IT)
			{
				if ((encontre == false) && ((cursor.getPredecesor() == verticev) || (cursor.getSucesor() == verticev)))
				{
					if ((cursor.getPredecesor() == verticew) || (cursor.getSucesor() == verticew))
						encontre = true;
				}
			}
			return encontre;
		}


		public V replace(Vertex<V> v, V x)
		{
			Vertice<V> vertice = checkVertex(v);
			V to_ret = vertice.element();
			vertice.setRotulo(x);
			return to_ret;
		}

		public Vertex<V> insertVertex(V x)
		{
			Vertice<V> v = new Vertice<V>();

			v.setRotulo(x);
			listaVertices.Add(v);

			return v;
		}

		public Edge<E> insertEdge(Vertex<V> v, Vertex<V> w, E e)
		{
			Vertice<V> verticev = checkVertex(v);
			Vertice<V> verticew = checkVertex(w);
			Arco<V, E> arco = new Arco<V, E>();

			arco.setRotulo(e);
			arco.setPredecesor(verticev);
			arco.setSucesor(verticew);
			listaArcos.Add(arco);
			return arco;
		}

		public E removeEdge(Edge<E> e)
		{
			Arco<V, E> arco = checkEdge(e);
			E to_ret = arco.element();


			listaArcos.Remove(arco);
			arco.setRotulo(default(E));
			arco.setSucesor(null);
			arco.setPredecesor(null);

			return to_ret;
		}


		public V removeVertex(Vertex<V> v)
		{
			Vertice<V> vertice = checkVertex(v);
			V to_ret = vertice.element();
			List<Arco<V, E>> IT = listaArcos;
			Stack<Arco<V, E>> pila = new Stack<Arco<V, E>>();
			try
			{
				foreach (Arco<V, E> cursor in IT)
				{
					if ((cursor.getPredecesor() == vertice) || (cursor.getSucesor() == vertice))
					{
						pila.Push(cursor);
					}
				}

				while (pila.Count > 0)
				{
					removeEdge(pila.Pop());
				}
				listaVertices.Remove(vertice);
				vertice.setRotulo(default(V));
			}
			catch (InvalidEdgeException q)
			{
				Debug.Log(q.ToString());
			}

			return to_ret;
		}

















		private Arco<V, E> checkEdge(Edge<E> e)
		{
			if (e == null)
				throw new InvalidEdgeException("Vertice nulo");
			Arco<V, E> resultado = null;
			try
			{
				resultado = (Arco<V, E>)e;
			}
			catch (InvalidCastException)
			{
				throw new InvalidEdgeException("El parametro no es un vertice");
			}
			return resultado;
		}



		private Vertice<V> checkVertex(Vertex<V> v)
		{
			if (v == null)
				throw new InvalidVertexException("Vertice nulo");
			Vertice<V> resultado = null;
			try
			{
				resultado = (Vertice<V>)v;
			}
			catch (InvalidCastException)
			{
				throw new InvalidVertexException("El parametro no es un vertice");
			}
			return resultado;
		}

	}
	
}

