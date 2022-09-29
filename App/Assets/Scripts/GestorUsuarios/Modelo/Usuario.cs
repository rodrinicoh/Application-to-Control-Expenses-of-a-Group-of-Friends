using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GestorDeudas;
using Colecciones;
using GestorDeudas.Modelo;

namespace  GestorUsuarios.Modelo
{
    public class Usuario{
        private String nombre;
        private String apellido;
        private int DNI;
        private DateTime fechaNacimiento;
        private Coleccion<Deuda> deudas;

        public Usuario(String nombre, String apellido, int DNI, DateTime fechaNacimiento){
            this.nombre = nombre;
            this.apellido = apellido;
            this.DNI = DNI;
            this.fechaNacimiento = fechaNacimiento;
            deudas = new ColeccionLista<Deuda>();
        }

        public void agregarDeuda(Deuda deuda){
            deudas.agregar(deuda);
        }

        public void liquidarDeuda(Deuda deuda){
            deudas.eliminar(deuda);
        }


        public int obtenerDni(){
            return this.DNI;
        }

        public DateTime obtenerFechaNacimiento(){
            return fechaNacimiento;
        }

        public bool poseeDeudas(){
            return deudas.longitud() != 0;
        }

        public bool poseeDeudores(){
            //return deudas.longitud() != 0;
            return true;
        }

        public bool poseeDeudasUrgentes(){
            for(int i = 0; i < deudas.longitud(); i++){
                Deuda deudaActual = deudas.get(i);
                bool esDeudaUrgente = deudaActual.GetType() == typeof(DeudaUrgente);
                if (esDeudaUrgente)
                    return true;
            }
        
            return false;
        }

        public Coleccion<Deuda> obtenerDeudas(){
            return deudas;
        }

        public List<Deuda> obtenerListaDeudas(){
            List<Deuda> lista = new List<Deuda>();
            for (int i = 0; i < deudas.longitud(); i++){
                lista.Add(deudas.get(i));
            }
            return lista;
        }

        public string obtenerDniNombreApellido(char separator)
        {
            return DNI + separator.ToString() + nombre + separator.ToString() + apellido;
        }

        public string getNombre()
        {
            return nombre;
        }

        public string getApellido()
        {
            return apellido;
        }




    }
}
