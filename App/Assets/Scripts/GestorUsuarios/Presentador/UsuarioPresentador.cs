using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Colecciones;
using GestorUsuarios.Modelo;
using GestorUsuarios.Vista;
namespace GestorUsuarios.Presentador
{

    public class UsuarioPresentador
    {
        private UsuarioVista vista;
        private UsuarioManager usuarioManager;
        
        public UsuarioPresentador(UsuarioVista vista)
        {
            this.vista = vista;
        }

        public void setearModelo(UsuarioManager usuarioManager)
        {
            this.usuarioManager = usuarioManager;
        }


        public void agregarUsuario(string dni, string nombre, string apellido, string fecha)
        {
            int dniNumber = int.Parse(dni);
            usuarioManager.agregarUsuario(dniNumber, nombre, apellido, fecha);
        }

        public void mostrarMensaje(string mensaje, bool shareButtonEnable)
        {
            vista.mostrarMensaje(mensaje, shareButtonEnable);
        }

        public void obtenerUsuarios()
        {
            usuarioManager.obtenerUsuariosEliminar();
        }

        public void armarDropdownRemoverUsuarios(Coleccion<Usuario> usuarios)
        {
            vista.armarDropdownMenuRemoverUsuario(usuarios);
        }

        public void eliminarUsuario(string dni)
        {
            usuarioManager.eliminarUsuario(int.Parse(dni));

            //Reconstruyo la lista de usuarios a remover, para que no aparezca uno ya eliminado.
            obtenerUsuarios();
        }

        public void getInfoUser(string dniNumber)
        {
            int dni = int.Parse(dniNumber);
            usuarioManager.getInfoUser(dni);
        }

        public void setInfoUser(string userInfo)
        {
            vista.setInfoUser(userInfo);
        }

        public void actualizarHistorial(string msg)
        {
            vista.actualizarHistorial(msg);
        }

    }

}