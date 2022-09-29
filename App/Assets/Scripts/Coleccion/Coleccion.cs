using GestorUsuarios;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Colecciones
{

    public interface Coleccion<T>
    {

        public void agregar(T item);

        public void eliminar(T item);

        public T get(int index);

        public int longitud();

        IEnumerable<T> obtenerIterable();
        //public bool existItem(int dni);
    }
}
