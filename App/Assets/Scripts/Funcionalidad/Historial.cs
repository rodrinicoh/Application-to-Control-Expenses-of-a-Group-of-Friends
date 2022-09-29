using GestorAlmacenamiento;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Funcionalidad
{

    public class Historial : MonoBehaviour
    {
        private string textoHistorial;

        public Text scrollViewTextHistorial;

        private Almacenamiento database;
        

        private void actualizarMensaje(string msg)
        {
            string horaActual = DateTime.Now.ToString("dd/MM/yyy HH:mm:ss");
            textoHistorial = "Evento -" + horaActual + ":\n" + msg + "\n" + "------------------------------------\n" + textoHistorial;
            database.guardarHistorial(textoHistorial);
        }

        public void mostrarMensaje(string mensaje)
        {
            actualizarMensaje(mensaje);
            scrollViewTextHistorial.text = textoHistorial;
        }

        public void setDatabase(Almacenamiento almacenamiento)
        {
            this.database = almacenamiento;
            textoHistorial = database.obtenerHistorial();
            scrollViewTextHistorial.text = textoHistorial;
        }

    }
}
