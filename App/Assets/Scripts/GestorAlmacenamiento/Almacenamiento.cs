using GestorDeudas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Colecciones;
using GestorUsuarios.Modelo;
using GestorDeudas.Modelo;

namespace GestorAlmacenamiento
{

    public interface Almacenamiento{
        public void startDatabase(int modo);

        public void obtenerUsuarios(Coleccion<Usuario> usuarios);

        public bool obtenerYCargarDeudas(Coleccion<Deuda> deudas, Coleccion<Usuario> usuarios);

        public bool guardarUsuario(int dni, string nombre, string apellido, string fecha);
    
        public bool eliminarUsuario(int dni);


        public bool guardarDeuda(Deuda deuda);

        //public bool guardarReunionYGasto(Reunion reunion, Gasto gasto);

        public bool eliminarDeuda(int idDeuda,int deudor);

        public bool actualizarDeuda(Deuda deuda);
        public bool existeUsuario(int dni);

        public bool poseeDeudas(int dni);


        public void cargarTodasLasDeudas(Coleccion<Deuda> deudas, Coleccion<Usuario> usuarios);
        public string obtenerHistorial();

        public void guardarHistorial(string msg);
    }

}