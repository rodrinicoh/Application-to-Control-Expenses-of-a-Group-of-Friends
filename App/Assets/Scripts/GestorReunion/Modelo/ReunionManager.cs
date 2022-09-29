using GestorAlmacenamiento;
using System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestorUsuarios.Modelo;
using Exceptions;
using AlgoritmosDivision;
using Colecciones;
using GestorReunion.Presentador;
using GestorDeudas.Modelo;

namespace GestorReunion.Modelo
{
    public class ReunionManager
    {
        private GestorAlmacenamiento.Almacenamiento database;
        public ReunionPresentador presentador;
        private UsuarioManager usuarioManager;
        private Coleccion<Usuario> usuarios;
        private DeudasManager deudasManager;
        private string algoritmoGastoEmpresarial = "Gasto Empresarial";
        const string algoritmoPartesIguales = "Partes Iguales";
        const string algoritmoIVA = "IVA";
        const char userSeparator = '-';

        private DivisionGastos divisionGastosAlgoritmoIVA;
        private DivisionGastos divisionGastosAlgoritmoPartesIguales;
        const string textErrorAdministrador = "\n Por favor, comuniquese con el administrador\n";


        public ReunionManager(ReunionPresentador presentador)
        {
            this.presentador = presentador;
            //database = new DatabaseAndroid();

            crearAlgoritmosDivision();
        }

        private void crearAlgoritmosDivision()
        {
            divisionGastosAlgoritmoIVA = new AlgoritmoIVA();
            divisionGastosAlgoritmoPartesIguales = new AlgoritmoPartesIguales();
        }


        /**
         *  Setea el usuario manager.
        */
        public void setearUsuarioManagerYAlmacenamiento(UsuarioManager usuarioManager, Almacenamiento database)
        {
            this.usuarioManager = usuarioManager;
            this.database = database;
        }

        public void setearDeudasManager(DeudasManager deudasManager){
            this.deudasManager = deudasManager;
        }

        public void obtenerUsuarios()
        {

            usuarios = usuarioManager.obtenerUsuarios();
            presentador.enviarUsuariosParticipantes(usuarios);
        }


        private float ejecutarDivisionPartesIguales(int cantidadParticipantes, float monto)
        {
            //LAMAR A CLASE DIVISION PARTE SIGUALSE.
            return monto / cantidadParticipantes;
        }

        private float ejecutarDivisionGastoEmpresarial(int cantidadParticipantes, float monto)
        {
            //LLAMAR A CLASE DIVISIONGASTOEMPRESARIAL
            return (monto / 2) / cantidadParticipantes;
        }

        private float ejecutarDivisionIVA(int cantidadParticipantes, float monto)
        {
            //LLAMAR A CLASE Q NOS DIERON
            const float IVA = 0.25f;
            float individualDebt = (float)(monto / (float)cantidadParticipantes);
            individualDebt += individualDebt * IVA;
            return individualDebt;
        }

        private void lanzarExcepcionTieneDeudasUrgentes(Usuario user)
        {
            string mensaje = "El usuario: " + user.obtenerDniNombreApellido(userSeparator) + " posee al menos una deuda urgente, se cancela la reunion" + "\n";
            throw new ReunionException(mensaje);
        }

        private int crearReunionPartesIguales(Usuario acreedor, List<int> participantes, float monto, string algoritmo, bool esUrgente, DateTime fechaReunion, Coleccion<Usuario> participantesDeuda)
        {
            foreach (int dni in participantes)
            {
                //Buscamos el usuario con el siguiente número de dni
                Usuario user = encontrarUsuario(dni);

                bool poseeDeudasUrgentes = user.poseeDeudasUrgentes();
                //Comparamos las fechas del cumpleaños y de la reunion para ver si son iguales
                bool cumpleaños = cumpleañosIguales(user.obtenerFechaNacimiento(), fechaReunion);
                if (!cumpleaños)
                {
                    if (poseeDeudasUrgentes)
                        lanzarExcepcionTieneDeudasUrgentes(user);
                    else
                        participantesDeuda.agregar(user);
                }
            }

            int cantidadParticipantes = participantesDeuda.longitud();
            //Comparamos las fechas del cumpleaños y de la reunion para ver si el acreedor cumple a;os
            if (!cumpleañosIguales(acreedor.obtenerFechaNacimiento(), fechaReunion))
                if (acreedor.poseeDeudasUrgentes()) //El acreedor no cumple anios y tiene deudas urgentes, se cancela la reunion
                    lanzarExcepcionTieneDeudasUrgentes(acreedor);
                else
                    cantidadParticipantes++;

            if (cantidadParticipantes == 0)
            {
                string msg = "Todos los participantes de la reunion (incluido el acreedor) cumplen años. ";
                msg += "Al menos un participante no debe cumplir años el dia de la reunion";
                throw new ReunionException(msg);
            }
            return cantidadParticipantes;
        }


        private int crearReunionGastoEmpresarial(Usuario acreedor, List<int> participantes, float monto, string algoritmo, bool esUrgente, DateTime fechaReunion, Coleccion<Usuario> participantesDeuda)
        {
            foreach (int dni in participantes)
            {
                //Buscamos el usuario con el siguiente número de dni
                Usuario user = encontrarUsuario(dni);

                bool poseeDeudasUrgentes = user.poseeDeudasUrgentes();
                if (poseeDeudasUrgentes)
                    lanzarExcepcionTieneDeudasUrgentes(user);
                else
                    participantesDeuda.agregar(user);
            }

            //La cantidad de participantes sera los deudores mas el acreedor.
            int cantidadParticipantes = participantesDeuda.longitud() + 1;

            return cantidadParticipantes;
        }
        
        private int crearReunionIVA(Usuario acreedor, List<int> participantes, float monto, string algoritmo, bool esUrgente, DateTime fechaReunion, Coleccion<Usuario> participantesDeuda)
        {
            foreach (int dni in participantes)
            {
                //Buscamos el usuario con el siguiente número de dni
                Usuario user = encontrarUsuario(dni);

                bool poseeDeudasUrgentes = user.poseeDeudasUrgentes();
                if (poseeDeudasUrgentes)
                    lanzarExcepcionTieneDeudasUrgentes(user);
                else
                    participantesDeuda.agregar(user);
            }

            //La cantidad de participantes sera los deudores mas el acreedor.
            int cantidadParticipantes = participantesDeuda.longitud() + 1;
            return cantidadParticipantes;
        }



        public void crearReunion(int dniAcreedor, List<int> participantes, float monto, string algoritmo, bool esUrgente, DateTime fechaReunion)
        {
            //Buscamos el usuario acreedor con el siguiente número de dni
            Usuario acreedor = encontrarUsuario(dniAcreedor);
            DivisionGastos divisionGastos = null;
            Coleccion<Usuario> participantesDeuda = new ColeccionLista<Usuario>();
            int cantidadParticipantes = 0;
            int cantDeudas = 0;
            string msg = "";
            try
            {
                if (algoritmo.Equals(algoritmoGastoEmpresarial))
                {
                    divisionGastos = divisionGastosAlgoritmoPartesIguales;
                    cantidadParticipantes = crearReunionGastoEmpresarial(acreedor, participantes, monto, algoritmo, esUrgente, fechaReunion, participantesDeuda);

                    //Cuando se selecciona gasto empresarial, el usuario tech fashion paga el 50% del gasto y el resto se dividira en partes iguales.
                    Usuario usuarioTechFashion = usuarioManager.obtenerUsuarioTechFashion();
                    float montoTech = monto / 2;
                    monto = monto - montoTech;

                    //Creo la deuda del usuario tech fashion.
                    msg += crearDeuda(usuarioTechFashion, acreedor, montoTech, esUrgente);
                    cantDeudas++;
                }

                if (algoritmo.Equals(algoritmoIVA))
                {
                    divisionGastos = divisionGastosAlgoritmoIVA;
                    cantidadParticipantes = crearReunionIVA(acreedor, participantes, monto, algoritmo, esUrgente, fechaReunion, participantesDeuda);
                }

                if (algoritmo.Equals(algoritmoPartesIguales))
                {
                    divisionGastos = divisionGastosAlgoritmoPartesIguales;
                    cantidadParticipantes = crearReunionPartesIguales(acreedor, participantes, monto, algoritmo, esUrgente, fechaReunion, participantesDeuda);
                }
                
            }
            catch(ReunionException e)
            {
                mostrarMensajeError(e.Message);
                return;
            }
            catch(Exception e)
            {
                mostrarMensajeError("Error al crear el gasto");
            }

            float montoPersona = ejecutarDivision(divisionGastos, cantidadParticipantes, monto);

            //Se crean las deudas pertinentes

            msg += crearDeudas(acreedor, participantesDeuda, montoPersona, esUrgente, algoritmo);
            cantDeudas += participantesDeuda.longitud();

            string encabezadoMsg = "Se creo un gasto el dia " + fechaReunion + " con los siguientes datos:" + "\n";
            string mensajeDeudas = "Se crearon " + cantDeudas + " deudas \n";
            msg = encabezadoMsg + mensajeDeudas + msg;

            presentador.actualizarHistorial(msg);
            mostrarMensajeCorrecto(msg);
        }

        private float ejecutarDivision(DivisionGastos divisionGastos, int cantidadParticipantes, float monto)
        {
            return divisionGastos.ejecutarDivision(cantidadParticipantes, monto);
        }

        private string crearDeuda(Usuario deudor, Usuario acreedor, float montoPersona, bool urgente)
        {
            
            deudasManager.crearDeuda(deudor, acreedor, montoPersona, urgente);
            const char separador = '-';

            string msg = "Nueva Deuda \nDeudor: " + deudor.obtenerDniNombreApellido(separador) + "\n";
            msg += "Acreedor: " + acreedor.obtenerDniNombreApellido(separador) + "\n";
            msg += "Monto: $" + montoPersona + "\n\n";

            return msg;
        }

        private string crearDeudas(Usuario acreedor, Coleccion<Usuario> participantesDeuda, float montoPersona, bool urgente, string algoritmo)
        {           
            string msg = "";
            for (int i = 0; i < participantesDeuda.longitud(); i++)
            {
                Usuario deudor = participantesDeuda.get(i);
                msg += crearDeuda(deudor, acreedor, montoPersona, urgente);
            }
            
            return msg;
        }

        private bool cumpleañosIguales(DateTime dt1, DateTime dt2)
        {
            int day1 = dt1.Day;
            int day2 = dt2.Day;

            int month1 = dt1.Month;
            int month2 = dt2.Month;

            bool days = day1 == day2;
            bool month = month1 == month2;
            return days && month;
        }

        private Usuario encontrarUsuario(int dni)
        {
            for (int i = 0; i < usuarios.longitud(); i++)
            {
                Usuario user = usuarios.get(i);
                if (user.obtenerDni() == dni)
                    return user;
            }
            throw new Exception("No se encontro el usuario: "+dni+" solicitado en el sistema al crear una reunion");
        }



        private void mostrarMensaje(string mensaje, bool shareButtonEnable)
        {
            presentador.mostrarMensaje(mensaje, shareButtonEnable);
        }

        private void mostrarMensajeError(string mensaje)
        {
            mostrarMensaje(mensaje + textErrorAdministrador, false);
        }

        private void mostrarMensajeCorrecto(string mensaje)
        {
            mostrarMensaje(mensaje, true);
        }
    }
}
