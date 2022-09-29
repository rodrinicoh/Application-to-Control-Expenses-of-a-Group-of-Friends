using GestorUsuarios.Modelo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Colecciones;
using GestorDeudas.Modelo;
using GestorDeudas.Vista;
namespace GestorDeudas.Presentador
{
    public class DeudasPresentador{
        private DeudasVista vista;
        private DeudasManager deudasManager;

        public DeudasPresentador(DeudasVista vista)
        {
            this.vista = vista;

        }

        public void mostrarHistorial(string msg)
        {
            vista.mostrarHistorial(msg);
        }
        public void setearModelo(DeudasManager deudasManager)
        {
            this.deudasManager = deudasManager;
        }

        /*
         * Funcion que se invoca desde el modelo, luego de que se llamo a obtenerusuariosdeudores
         */
        public void obtenerUsuariosParticipantes()
        {
            deudasManager.obtenerUsuariosParticipantes();
        }

        public void obtenerUsuariosDeudores()
        {
            deudasManager.obtenerUsuariosDeudores();
        }

        public void actualizarUsuarios(List<Usuario> usuarios)
        {
            vista.actualizarUsuarios(usuarios);
        }



        public void obtenerDeudasUsuario(string dniSelected)
        {
            deudasManager.obtenerDeudasUsuario(int.Parse(dniSelected));
        }

        public void actualizarDeudasDeudor(Coleccion<Deuda> deudas)
        {
            vista.actualizarDeudasDeudor(deudas);
        }


        public void simplificar()
        {
            deudasManager.simplificarDeuda();
        }
        public void liquidarDeuda(string deudor, string idDeuda, float monto, bool total)
        {
            int id = int.Parse(idDeuda);
            int dniDeudor = int.Parse(deudor);
            deudasManager.liquidarDeuda(dniDeudor, id, monto, total);
        }


        public void mostrarMensaje(string msg, bool shareButtonEnable)
        {
            vista.mostrarMensaje(msg, shareButtonEnable);
        }





    }
}

