using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestorUsuarios.Modelo;
using Colecciones;
using GestorReunion.Modelo;
using Exceptions;

namespace GestorReunion.Presentador
{
    public class ReunionPresentador
    {
        public ReunionVista vista;
        public ReunionManager reunionManager;
        private Coleccion<Usuario> usuarios;

        public ReunionPresentador(ReunionVista vista)
        {
            this.vista = vista;

        }

        public void actualizarHistorial(string msg)
        {
            Debug.Log(msg + "************************ANTES***************************");
            vista.actualizarHistorial(msg);
        }

        public void setearModelo(ReunionManager reunionManager)
        {
            this.reunionManager = reunionManager;
        }

        /**
         * Funcion invocada por la vista que le solicita los usuarios y luego llama a otra funcion que se los envia.
         * 
        */
        public void obtenerUsuariosParticipantes()
        {
            /*
            usuarios = reunionManager.obtenerUsuarios();
            enviarUsuariosVista();
            */
            reunionManager.obtenerUsuarios();
        }

        public void enviarUsuariosParticipantes(Coleccion<Usuario> usuarios)
        {
            vista.armarDropdownQuienPaga(usuarios);
        }



        public void crearReunion(int dniAcreedor, List<int> participantes, float monto, string algoritmo, bool esUrgente, DateTime fecha)
        {
            try
            {
            reunionManager.crearReunion(dniAcreedor, participantes, monto, algoritmo, esUrgente, fecha);

            }
            catch (ReunionException e)
            {
                mostrarMensaje(e.Message, false);
            }
            //throw new NotImplementedException();
        }


        public void mostrarMensaje(string mensaje, bool shareButtonEnable)
        {
            vista.mostrarMensaje(mensaje, shareButtonEnable);
        }
    }
}
