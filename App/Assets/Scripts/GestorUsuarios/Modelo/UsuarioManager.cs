using GestorAlmacenamiento;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Colecciones;
using GestorUsuarios.Presentador;
using Exceptions;

namespace GestorUsuarios.Modelo
{
    public class UsuarioManager
    {
        private UsuarioPresentador presentador;

        private Almacenamiento database;

        private Coleccion<Usuario> usuarios;

        const string textErrorAdministrador = "\n Por favor, comuniquese con el administrador\n";

        const int dniTech = 99999999;
        const string nombreTech = "Tech";
        const string apellidoTech = "Fashion";
        const string dateTech = "09/11/2001";

        public UsuarioManager(UsuarioPresentador presentador)
        {
            this.presentador = presentador;
            database = new DatabaseAndroid();

            usuarios = new ColeccionLista<Usuario>();
            
            crearUsuarioTechFashion();

            database.obtenerUsuarios(usuarios);
        }

        public Almacenamiento getDatabase()
        {
            return database;
        }

        private void crearUsuarioTechFashion()
        {
            //Solo se creara la primera vez que se ejecute, cuando el usuario no este en la base de datos.
            if (!existeUsuario(dniTech))
            {
                try
                {
                    parsearFechaYControlarError(dateTech);
                    database.guardarUsuario(dniTech, nombreTech, apellidoTech, dateTech);
                }
                catch (DatabaseException e)
                {
                    mostrarMensajeError("Error al crear el sistema");
                }
                catch(Exception e)
                {
                    mostrarMensajeError(e.Message);
                }
            }
        }

        public void getInfoUser(int dni)
        {
            try
            {
                Usuario usuario = obtenerUsuario(dni);
                string userInfo = informacionUsuario(usuario);
                presentador.setInfoUser(userInfo);
            }
            catch(Exception e)
            {
                mostrarMensajeError("Error al obtener informacion del usuario");
            }
        }

        private DateTime parsearFechaYControlarError(string fecha)
        {
            DateTime fechaDeCumpleaños;
            if (DateTime.TryParseExact(Convert.ToString(fecha), "dd/MM/yyyy", null, DateTimeStyles.None, out fechaDeCumpleaños) == false)
                throw new Exception("Se ha computado una fecha invalida");

            return fechaDeCumpleaños;
        }


        public void agregarUsuario(int dni, string nombre, string apellido, string fecha)
        {
            string mensaje = "";
            try
            {
                bool error = true;
                if (existeUsuario(dni))
                    mensaje = "Ya existe el usuario en el sistema";
                else
                    if (database.guardarUsuario(dni, nombre, apellido, fecha))
                    {
                        DateTime fechaDeCumpleaños = parsearFechaYControlarError(fecha);
                        Usuario user = new Usuario(nombre, apellido, dni, fechaDeCumpleaños);
                        usuarios.agregar(user);
                        mensaje = "Se agrego el usuario: \n";
                        mensaje += "DNI: " + user.obtenerDni() + "\n";
                        mensaje += "Nombre: " + user.getNombre() + "\n";
                        mensaje += "Apellido: " + user.getApellido() + "\n";
                        mensaje += "Fecha de Nacimiento: " + user.obtenerFechaNacimiento().ToShortDateString() + "\n";
                        presentador.actualizarHistorial(mensaje);
                        error = false;
                    }
                    else
                        mensaje = "Error al agregar usuario";

                if (error)
                    mostrarMensajeError(mensaje);
                else
                    mostrarMensajeCorrecto(mensaje);
            }
            catch (Exception e)  
            {
                mostrarMensajeError(mensaje);
            }
        }

        public Almacenamiento obtenerDatabase()
        {
            return database;
        }

        public bool existeUsuario(int dni)
        {
            try
            {
                if (database.existeUsuario(dni))
                    return true;
            }
            catch(DatabaseException e)
            {
                mostrarMensajeError("Error de consistencia de la base de datos");
            }
            return false;
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

        public bool esUsuarioTech(int dni)
        {
            return dni == dniTech;
        }

        public Usuario obtenerUsuarioTechFashion()
        {
            foreach(Usuario usuario in usuarios.obtenerIterable())
            {
                if (usuario.obtenerDni() == dniTech)
                    return usuario;
            }
            throw new Exception("No se encontro el usuario administrador, comunicarse con el administrador\n");
        }

        private void lanzarDatabaseExcepcion(string metodo, string msg)
        {
            string texto = "Hay un error en el metodo " + metodo + ":\n\n" + msg + "\n Por favor, comuniquese con el administrador\n";
            mostrarMensajeError(texto);
        }

        public string informacionUsuario(Usuario user)
        {
            string mensaje = "";
            mensaje += "DNI: " + user.obtenerDni() + "\n";
            mensaje += "Nombre: " + user.getNombre() + "\n";
            mensaje += "Apellido: " + user.getApellido() + "\n";
            mensaje += "Fecha de Nacimiento: " + user.obtenerFechaNacimiento().ToShortDateString() + "\n";
            return mensaje;
        }

        public void eliminarUsuario(int dni)
        {
            try
            {
                Usuario user = obtenerUsuario(dni);
                String error = "";
                bool eliminarUsuario = database.eliminarUsuario(dni);
                string infoUser = informacionUsuario(user);
                if (eliminarUsuario)
                {
                    usuarios.eliminar(user);
                    error = "Se elimino: el usuario: \n" + infoUser + "\n";
                    mostrarMensajeCorrecto(error);
                    presentador.actualizarHistorial(error);
                }
                else
                {
                    error = "No se pudo eliminar el usuario: \n" + infoUser + ", ya que posee al menos una deuda";
                    mostrarMensajeError(error);
                }
            }
            catch (DatabaseException e)
            {
                lanzarDatabaseExcepcion("Error en el sistema al intentar eliminar el usuario con dni: " + dni, e.Message);
            }
            catch (Exception e)
            {
                lanzarDatabaseExcepcion("Error en el sistema al intentar eliminar el usuario con dni: " + dni, e.Message);
            }
            
        }

        public Usuario obtenerUsuario(int dni)
        {
            for(int i = 0; i < usuarios.longitud(); i++)
            {
                Usuario usuarioActual = usuarios.get(i);
                if (usuarioActual.obtenerDni() == dni)
                    return usuarioActual;
            }
            throw new Exception("No se encontro el usuario en el sistema, hay una inconsistencia");
        }

        public Coleccion<Usuario> exobtenerUsuarios()
        {
            Coleccion<Usuario> usuarios = new ColeccionLista<Usuario>();
            database.obtenerUsuarios(usuarios);
            return usuarios;
        }

        public Coleccion<Usuario> obtenerUsuarios()
        {
            Coleccion<Usuario> coleccionUsuarios = new ColeccionLista<Usuario>();
            foreach(Usuario user in usuarios.obtenerIterable())
            {
                if (user.obtenerDni() != dniTech)
                    coleccionUsuarios.agregar(user);
            }
            return coleccionUsuarios;
        }

        public Coleccion<Usuario> obtenerUsuariosVerDeudas()
        {
            return usuarios;
        }

        public void obtenerUsuariosEliminar()
        {
            presentador.armarDropdownRemoverUsuarios(obtenerUsuarios());
        }


    }

}

