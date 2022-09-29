using Exceptions;
using GestorDeudas;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Colecciones;
using GestorUsuarios.Modelo;
using GestorDeudas.Modelo;

namespace GestorAlmacenamiento
{
    public class DatabaseAndroid : Almacenamiento
    {
        const string key_last_deuda = "last_deuda";
        const string key_last_id = "last_id";

        const string key_last_deuda_not_exists = "No existe la clave last_deuda, comunicarse con el gestor";

        const string palabra_clave_acreedor = "acreedor";
        const string palabra_clave_monto = "monto";
        const string palabra_clave_urgente = "urgente";
        const string palabra_clave_deudor = "deudor";

        const string palabra_clave_deuda = "$deuda:";

        const char separador_deuda = '&';

        const string palabra_clave_dni = "%dni";
        const string palabra_clave_nombre = "%nombre";
        const string palabra_clave_apellido = "%apellido";
        const string palabra_clave_fecha = "%fecha";
        const string palabra_clave_cantDeudas = "%cant_deudas";

        const string palabra_clave_deuda_del_usuario = "%deuda:";
        const string textoEncabezadoException = "Hay un error en el metodo ";
        const string palabra_clave_historial = "%historial";

        public DatabaseAndroid()
        {
            crearClavesIniciales();
        }

        private void crearClavesIniciales()
        {
            if (!existeClave(key_last_id))
            {
                guardarEnteroEnBaseDeDatos(key_last_id, 0);
                guardarBaseDeDatos();
            }

            if (!existeClave(key_last_deuda))
            {
                guardarEnteroEnBaseDeDatos(key_last_deuda, 0);
                guardarBaseDeDatos();
            }

            if (!existeClave(palabra_clave_historial))
            {
                guardarStringEnBaseDeDatos(palabra_clave_historial, "");
                guardarBaseDeDatos();
            }
        }

        private bool existeClave(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        private void guardarDeuda(string key, string urgente, string deudor, string acreedor, string monto)
        {
            string value = palabra_clave_urgente + "=" + urgente + separador_deuda +
                           palabra_clave_deudor + "=" + deudor + separador_deuda +
                           palabra_clave_acreedor + "=" + acreedor + separador_deuda +
                           palabra_clave_monto + "=" + monto;

            //Guardo la deuda actualizada en playerpfres
            guardarStringEnBaseDeDatos(key, value);

            guardarBaseDeDatos();
        }

        public bool actualizarDeuda(Deuda deuda)
        {
            try
            {
                if (!estaEnLaBaseDeDatos(key_last_deuda))
                    throw new DatabaseException("No existe la clave de la ultima deuda, comunicarse con el administrador \n");

                int idDeuda = deuda.obtenerIDDeuda();
                int lastDeuda = obtenerEnteroDeLaBaseDeDatos(key_last_deuda);

                string key = palabra_clave_deuda + idDeuda;
                string valueAnterior = obtenerStringDeLaBaseDeDatos(key);
                string[] option = valueAnterior.Split(separador_deuda);
                string urgente = "";
                string acreedor = "";
                string deudor = "";

                foreach (string str in option)
                {
                    string[] valor = str.Split('=');
                    if (str.Contains(palabra_clave_urgente))
                        urgente = valor[1];
                    if (str.Contains(palabra_clave_deudor))
                        deudor = valor[1];
                    if (str.Contains(palabra_clave_acreedor))
                        acreedor = valor[1];
                }
                if (urgente == "" || acreedor == "" || deudor == "")
                {
                    throw new DatabaseException("Error al actualizar la deuda\n");
                }
                float monto = deuda.getMonto();
                guardarDeuda(key, urgente, deudor, acreedor, monto.ToString());
                //guardarBaseDeDatos();
            }

            catch (DatabaseException e)
            {
                lanzarDatabaseExcepcion("actualizarDeuda()", e.Message);
            }
            return true;
        }




        private void eliminarClave(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
        public bool eliminarDeuda(int idDeuda, int deudor)
        {
            try
            {

                string keyDeuda = palabra_clave_deuda + idDeuda;

                if (!estaEnLaBaseDeDatos(keyDeuda))
                    throw new DatabaseException("No existe la clave en el sistema, comunicarse con el administrador\n");


                //Primero elimino la deuda
                eliminarClave(keyDeuda);

                //Debo eliminar el idDeuda del usuario deudor
                int id = obtenerIDUsuario(deudor);
                string keyDeudaDeudor = id + palabra_clave_deuda_del_usuario + idDeuda;
                eliminarClave(keyDeudaDeudor);

                //Disminuyo en 1 la cantidad de deudas del suuario
                string keyCantDeudasDeudor = id + palabra_clave_cantDeudas;
                int cantDeudas = obtenerEnteroDeLaBaseDeDatos(keyCantDeudasDeudor);
                cantDeudas--;

                if (cantDeudas >= 0) //No deberia ser menor a cero, por las dudas.
                {
                    guardarEnteroEnBaseDeDatos(keyCantDeudasDeudor, cantDeudas);
                }
                guardarBaseDeDatos();
            }

            catch (DatabaseException e)
            {
                lanzarDatabaseExcepcion("eliminarDeuda()", e.Message);
            }
            return true;
        }

        private void controlarSiExisteClaveEnBaseDeDatos()
        {
            //Si no se encuentra la clave last_id creada, debo lanazr un error
            if (!estaEnLaBaseDeDatos(key_last_id))
                throw new DatabaseException("Error de claves");//throw new DatabaseException("No existe last_id");

        }

        private int obtenerEnteroDeLaBaseDeDatos(String clave)
        {
            return PlayerPrefs.GetInt(clave);
        }

        private String obtenerStringDeLaBaseDeDatos(String clave)
        {
            return PlayerPrefs.GetString(clave);
        }

        private bool estaEnLaBaseDeDatos(String clave)
        {
            return PlayerPrefs.HasKey(clave);
        }

        private void borrarDeLaBaseDeDatos(String clave)
        {
            PlayerPrefs.DeleteKey(clave);
        }

        private void guardarStringEnBaseDeDatos(String clave, String valor)
        {
            PlayerPrefs.SetString(clave, valor);
        }

        private void guardarEnteroEnBaseDeDatos(String clave, int valor)
        {
            PlayerPrefs.SetInt(clave, valor);
        }

        private void guardarBaseDeDatos()
        {
            PlayerPrefs.Save();
        }

        private void controlarSiElUsuarioExiste(int idUsuario)
        {
            if (idUsuario == -1)
                throw new DatabaseException("No existe el usuario en el sistema");
        }


        private bool userPoseeDeudas(int dni)
        {
            int idUser = obtenerIDUsuario(dni);
            string key = idUser + palabra_clave_cantDeudas;
            int cant_deudas = obtenerEnteroDeLaBaseDeDatos(key);
            if (cant_deudas > 0)
                return true;
            return false;
        }

        public bool eliminarUsuario(int dni)
        {
            try
            {
                int idUsuario = obtenerIDUsuario(dni);
                controlarSiElUsuarioExiste(idUsuario);
                if (userPoseeDeudas(dni))
                    return false;
                borrarDeLaBaseDeDatos(idUsuario + palabra_clave_dni);
                borrarDeLaBaseDeDatos(idUsuario + palabra_clave_nombre);
                borrarDeLaBaseDeDatos(idUsuario + palabra_clave_apellido);
                borrarDeLaBaseDeDatos(idUsuario + palabra_clave_fecha);
                borrarDeLaBaseDeDatos(idUsuario + palabra_clave_cantDeudas);
                guardarBaseDeDatos();
            }

            catch (DatabaseException e)
            {
                lanzarDatabaseExcepcion("EliminarUsuario()", e.Message);
            }
            return true;
        }

        private int obtenerIDUsuario(int dni)
        {

            int last_id = 0;
            try
            {
                controlarSiExisteClaveEnBaseDeDatos();

                last_id = obtenerEnteroDeLaBaseDeDatos(key_last_id);

                for (int id = 0; id < last_id; id++)
                {
                    if (estaEnLaBaseDeDatos(id + palabra_clave_dni))
                    {
                        int obtenerDni = obtenerEnteroDeLaBaseDeDatos(id + palabra_clave_dni);
                        if (obtenerDni == dni)
                        {
                            return id;
                        }
                    }
                }
            }
            catch(DatabaseException e)
            {
                throw new DatabaseException(e.Message);
            }

            return -1;
        }

        public bool existeUsuario(int dni)
        {
            try
            { 
                if (obtenerIDUsuario(dni) == -1)
                    return false;
            }

            catch (DatabaseException e)
            {
                lanzarDatabaseExcepcion("existeUsuario()", e.Message);
            }
            return true;
        }

        public bool guardarUsuario(int dni, string nombre, string apellido, string fecha)
        {
            try
            {
                int last_id = 0;

                controlarSiExisteClaveEnBaseDeDatos();

                last_id = obtenerEnteroDeLaBaseDeDatos(key_last_id);

                //string fechaNacimiento = usuario.obtenerFechaNacimiento().Date.ToString("dd/MM/yyyy");

                guardarEnteroEnBaseDeDatos(last_id + palabra_clave_dni, dni);
                guardarStringEnBaseDeDatos(last_id + palabra_clave_nombre, nombre);
                guardarStringEnBaseDeDatos(last_id + palabra_clave_apellido, apellido);
                guardarStringEnBaseDeDatos(last_id + palabra_clave_fecha, fecha);
                guardarEnteroEnBaseDeDatos(last_id + palabra_clave_cantDeudas, 0);

                //Aumento el last_id, para cuando agregue el proximo usuario
                last_id++;
                guardarEnteroEnBaseDeDatos(key_last_id, last_id);

                //Guardo los cambios
                guardarBaseDeDatos();
            }
            catch (DatabaseException e)
            {
                lanzarDatabaseExcepcion("guardarUsuario()", e.Message);
            }
            return true;
        }

        private DateTime parsearFechaYControlarError(string fecha)
        {
            DateTime fechaDeCumpleaños;
            if (DateTime.TryParseExact(Convert.ToString(fecha), "dd/MM/yyyy", null, DateTimeStyles.None, out fechaDeCumpleaños) == false)
                throw new DatabaseException("Error al parsear fecha");
            //Las excepciones de metodos privados capaz deberian ser un itpo de excepcion: InternalDatabaseException
            return fechaDeCumpleaños;
        }

        public void obtenerUsuarios(Coleccion<Usuario> usuarios)
        {
            try
            {
                int last_id = obtenerEnteroDeLaBaseDeDatos(key_last_id);

                for (int id = 0; id < last_id; id++)
                {
                    if (estaEnLaBaseDeDatos(id + palabra_clave_dni))
                    {
                        int dni = obtenerEnteroDeLaBaseDeDatos(id + palabra_clave_dni);
                        string nombre = obtenerStringDeLaBaseDeDatos(id + palabra_clave_nombre);
                        string apellido = obtenerStringDeLaBaseDeDatos(id + palabra_clave_apellido);
                        string fecha = obtenerStringDeLaBaseDeDatos(id + palabra_clave_fecha);
                        DateTime fechaDeCumpleaños = parsearFechaYControlarError(fecha);

                        Usuario nuevoUsuario = new Usuario(nombre, apellido, dni, fechaDeCumpleaños);
                        usuarios.agregar(nuevoUsuario);
                    }
                }
            }
            catch (DatabaseException e)
            {
                lanzarDatabaseExcepcion("obtenerUsuarios()", e.Message);
            }
        }

        public bool obtenerYCargarDeudas(Coleccion<Deuda> deudas, Coleccion<Usuario> usuarios)
        {
            throw new NotImplementedException();
        }

        public void startDatabase(int modo)
        {
            throw new NotImplementedException();
        }

        private void controlarSiExisteUsuario(int dni)
        {
            if (!existeUsuario(dni))
                throw new DatabaseException("No existe el usuario");
        }


        public bool poseeDeudas(int dni)
        {
            try
            {
                int last_id = 0;

                controlarSiExisteClaveEnBaseDeDatos();
                last_id = obtenerEnteroDeLaBaseDeDatos(key_last_id);

                if (!existeUsuario(dni))
                    throw new DatabaseException("No existe el usuario");

                //Si el usuario no tiene deudas, retorno falso.
                if (!userPoseeDeudas(dni))
                    return false;
            }
            catch(DatabaseException e)
            {
                lanzarDatabaseExcepcion("poseeDeudas()", e.Message);
            }

            return true;
        }

        private void controlarSiLaDeudaExiste()
        {
            //Obtengo el ultimo id de deuda insertado, para insertar a partir del mismo.
            if (!estaEnLaBaseDeDatos(key_last_deuda))
                throw new DatabaseException(key_last_deuda_not_exists);
        }

        private string formarValorDeLaDeuda(Deuda deuda)
        {
            int deudor = deuda.obtenerDeudor().obtenerDni();
            int acreedor = deuda.obtenerAcreedor().obtenerDni();
            float monto = deuda.getMonto();

            //Determino si la deuda era urgente, ver si se pasa la deuda por parametro o directamente se pasan los dnis, monto, urgente, etc.
            int urgente = 0;
            if (deuda.GetType() == typeof(DeudaUrgente))
                urgente = 1;
            
            string value = palabra_clave_urgente + "=" + urgente + separador_deuda +
                           palabra_clave_deudor + "=" + deudor + separador_deuda +
                           palabra_clave_acreedor + "=" + acreedor + separador_deuda +
                           palabra_clave_monto + "=" + monto;
            return value;
        }

        public bool guardarDeuda(Deuda deuda)
        {
            try
            {
                int deudor = deuda.obtenerDeudor().obtenerDni();
                int acreedor = deuda.obtenerAcreedor().obtenerDni();
                float monto = deuda.getMonto();

                controlarSiLaDeudaExiste();
                int last_id_deuda = obtenerEnteroDeLaBaseDeDatos(key_last_deuda);

                //Determino si la deuda era urgente, ver si se pasa la deuda por parametro o directamente se pasan los dnis, monto, urgente, etc.
                int urgente = 0;
                if (deuda.GetType() == typeof(DeudaUrgente))
                    urgente = 1;

                string key = palabra_clave_deuda + last_id_deuda;
                string value = palabra_clave_urgente + "=" + urgente + separador_deuda +
                               palabra_clave_deudor + "=" + deudor + separador_deuda +
                               palabra_clave_acreedor + "=" + acreedor + separador_deuda +
                               palabra_clave_monto + "=" + monto;

                //Guardo la deuda en playerpfres
                guardarStringEnBaseDeDatos(key, value);

                //Aumento en 1 la cantidad de deudas en el sistema
                int id_deuda = last_id_deuda;
                deuda.establecerIDDeuda(id_deuda);
                last_id_deuda++;
                guardarEnteroEnBaseDeDatos(key_last_deuda, last_id_deuda);

                //Ademas debo aumentar en 1 la deuda del usuario y asignarle el id de deuda:
                int id_deudor = obtenerIDUsuario(deudor);

                //Aumento en 1 las deudas del usuario.
                int cant_deudas = obtenerEnteroDeLaBaseDeDatos(id_deudor + palabra_clave_cantDeudas);
                cant_deudas++;
                guardarEnteroEnBaseDeDatos(id_deudor + palabra_clave_cantDeudas, cant_deudas);

                //Asigno el id de la deuda al suuario.
                string key_deuda = id_deudor + "%deuda:" + id_deuda; //'13%deuda:4', el usuario con id = 13 tiene una deuda con id de deuda = 4.
                int value_deuda = id_deuda;
                guardarEnteroEnBaseDeDatos(key_deuda, value_deuda);


                guardarBaseDeDatos();
            }
            catch(DatabaseException e)
            {
                lanzarDatabaseExcepcion("guardarDeuda()", e.Message);
            }
            return true;
        }

        private void controlarSiLaDeudaEstaEnElUsuarioYEnLaBaseDeDatos(string key)
        {
            if (!estaEnLaBaseDeDatos(key))
                throw new DatabaseException("Existia la deuda para el usuario pero no existe la misma en el sistema");
        }

        private void lanzarDatabaseExcepcion(string metodo, string msg)
        {
            //throw new DatabaseException(textoEncabezadoException + metodo + ":\n" + msg);
            throw new DatabaseException(msg);
        }
        
        public void cargarTodasLasDeudas(Coleccion<Deuda> deudas, Coleccion<Usuario> usuarios)
        {
            try
            {
                foreach (Usuario user in usuarios.obtenerIterable())
                {

                    if (this.poseeDeudas(user.obtenerDni()))
                    {
                        int id_user = this.obtenerIDUsuario(user.obtenerDni());
                        int cant_deudas_user = obtenerEnteroDeLaBaseDeDatos(id_user + palabra_clave_cantDeudas);
                        int last_id_deuda = obtenerEnteroDeLaBaseDeDatos(key_last_deuda);

                        for (int id_deuda = 0; id_deuda < last_id_deuda; id_deuda++)
                        {
                            string key_deuda = id_user + "%deuda:" + id_deuda;
                            if (estaEnLaBaseDeDatos(key_deuda)) //Si existe un id de deuda 'id_deuda' asociada al usuario 'id_user'
                            {
                                string key = palabra_clave_deuda + id_deuda;
                                controlarSiLaDeudaEstaEnElUsuarioYEnLaBaseDeDatos(key);

                                String value_deuda = obtenerStringDeLaBaseDeDatos(key);
                                String[] data = value_deuda.Split(separador_deuda);

                                int deudor = 0;
                                int acreedor = 0;
                                int urgente = 0;
                                float monto = 0;
                                //Descompongo el string devuelvo por la base de datos, para formar la deuda.
                                foreach (string str in data)
                                {
                                    string[] column = str.Split('=');

                                    string parametro = column[0];
                                    string value = column[1];
                                    if (parametro.Equals(palabra_clave_deudor))
                                        deudor = int.Parse(value);
                                    if (parametro.Equals(palabra_clave_acreedor))
                                        acreedor = int.Parse(value);
                                    if (parametro.Equals(palabra_clave_monto))
                                        monto = Single.Parse(value);
                                    if (parametro.Equals(palabra_clave_urgente))
                                        urgente = int.Parse(value);
                                }
                                Usuario usuarioDeudor = user;
                                Usuario usuarioAcreedor = null;
                                foreach (Usuario u in usuarios.obtenerIterable())
                                {
                                    if (u.obtenerDni() == acreedor)
                                        usuarioAcreedor = u;
                                }

                                Deuda deuda;
                                if (urgente == 0)
                                    deuda = new Deuda(usuarioDeudor, usuarioAcreedor, monto, id_deuda);
                                else
                                    deuda = new DeudaUrgente(usuarioDeudor, usuarioAcreedor, monto, id_deuda);
                                deudas.agregar(deuda);
                                user.agregarDeuda(deuda);
                            }
                        }
                    }

                }
            }
            catch(DatabaseException e)
            {
                lanzarDatabaseExcepcion("cargarTodasLasDeudas()", e.Message);
            }
        }

        public string obtenerHistorial()
        {
            string value = "";
            if (!estaEnLaBaseDeDatos(palabra_clave_historial))
                throw new DatabaseException("Clave no encontrada en la base de datos");

            value = obtenerStringDeLaBaseDeDatos(palabra_clave_historial);
            return value;
        }

        public void guardarHistorial(string msg)
        {
            guardarStringEnBaseDeDatos(palabra_clave_historial, msg);
            guardarBaseDeDatos();
        }
    }

}
