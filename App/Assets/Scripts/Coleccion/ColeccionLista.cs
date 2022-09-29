using GestorUsuarios;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Colecciones
{

    public class ColeccionLista<T> : Coleccion<T>{

         private List<T> list = new List<T>();

        public void agregar(T item){
            list.Add(item);
        }

        public void eliminar(T item){
            list.Remove(item);
        }
        
        public T get(int index){
            if (index < list.Count)
                return list[index];
            //Equivale a retornar null
            return default(T);
        }

        public int longitud(){
            return list.Count;
        }


        public IEnumerable<T> obtenerIterable()
        {
            return list;
        }
    }

}