using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grafo;

public class Resultado<V, E>
{
    List<Vertex<V>> Lv;
    List<Edge<E>> Le;
    bool termine;

    public Resultado()
    {
        Lv = new List<Vertex<V>>();
        Le = new List<Edge<E>>();
        termine = false;
    }

    public void setListaArcos(List<Edge<E>> e)
    {
        Le = e;
    }

    public void setListaVertices(List<Vertex<V>> v)
    {
        Lv = v;
    }

    public List<Vertex<V>> getListaVertices()
    {
        return Lv;
    }

    public List<Edge<E>> getListaArcos()
    {
        return Le;
    }

    public void setTermine(bool b)
    {
        termine = b;
    }

    public bool getTermine()
    {
        return termine;
    }

}