using GestorDeudas;
using GestorReunion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Colecciones;
using GestorUsuarios.Modelo;
using GestorUsuarios.Presentador;
using GestorDeudas.Vista;
using Funcionalidad;
using GestorAlmacenamiento;

namespace GestorUsuarios.Vista
{

    public class UsuarioVista : MonoBehaviour {
        public GameObject botonCrearUsuario;
        public InputField ingresarDNI;
        public InputField ingresarNombre;
        public InputField ingresarApellido;

        public UsuarioPresentador presentador;

        public Text textAgregarUsuario;

        public Text textMostrarUsuarios;

        public Dropdown dropdownDia;
        public Dropdown dropdownMes;
        public Dropdown dropdownAño;

        public Dropdown dropdownUsuariosRemover;
        public Text textInfoUser;

        private Coleccion<Usuario> usuariosRemover;

        public GameObject menuUsuarios;
        public GameObject menuEliminarUsuarios;

        public Button buttonEliminarUsuario;

        //private static string separador = "%";

        public ReunionVista reunionVista;
        public DeudasVista deudasVista;

        public PanelError panel;


        private UsuarioManager usuarioManager;
        private string fechaSeparador = "/";
        private char userSeparator = '-';


        public Historial historial;


        private void Start() { 
            presentador = new UsuarioPresentador(this);
            usuarioManager = new UsuarioManager(presentador);

            presentador.setearModelo(usuarioManager);

            //Debo inicializar el panel desde aca, ya que si ni bien se crea esta clase se realiza un metodo
            //Y ese metodo llama al panel, aun no se creo el panel y da error.

            panel.inicializar();
            reunionVista.inicializar();
            deudasVista.inicializar();

            Almacenamiento database = usuarioManager.obtenerDatabase();
            reunionVista.setearUsuarioManagerYAlmacenamiento(usuarioManager, database);
            deudasVista.setearUsuarioManagerYAlmacenamiento(usuarioManager, database);

            reunionVista.setearDeudasManager(deudasVista.obtenerDeudasManager());
            //BackUp nuevoBackUp = new BackUp(usuarioManager);

            cargarDropdownFechas();

            historial.setDatabase(usuarioManager.getDatabase());
        }

        public void setInfoUser(string userInfo)
        {
            showInfoUser(userInfo);
        }

        public void actualizarHistorial(string msg)
        {
            historial.mostrarMensaje(msg);
        }

        private void cargarDropdownFechas()
        {
            dropdownDia.ClearOptions();
            dropdownMes.ClearOptions();
            dropdownAño.ClearOptions();
            dropdownDia.AddOptions(listaDropdown("Dia", 1, 31));
            dropdownMes.AddOptions(listaDropdown("Mes", 1, 12));
            dropdownAño.AddOptions(listaDropdown("Año", 1950, DateTime.Now.Year));

        }

        private List<String> listaDropdown(string titulo, int inicio, int limite)
        {
            List<string> options = new List<string>();
            options.Add(titulo);
            for (int i = inicio; i <= limite; i++)
                options.Add(i.ToString());
            return options;
        }

        public UsuarioManager obtenerUsuarioManager()
        {
            return usuarioManager;
        }

        public void agregarUsuario()
        {
            
            string dni = ingresarDNI.text;
            string nombre = ingresarNombre.text;
            string apellido = ingresarApellido.text;

            string errorActual = "";
            bool errores = false;
            if ((nombre == "") || (apellido == "") || (dni == ""))
            {
                errorActual = errorActual + "Debe completar todos los campos" + "\n";
                errores = true;
            }

            if (nombre.Contains("%"))
            {
                errorActual = errorActual + "El nombre no puede contener simbolos" + "\n";
                errores = true;
            }

            if (apellido.Contains("%"))
            {
                errorActual = errorActual + "El apellido no puede contener simbolos" + "\n";
                errores = true;
            }

            int dniNumber;
            if (int.TryParse(dni, out dniNumber) == false)
            {
                errorActual = errorActual + "El DNI ingresado debe ser un número y no debe tener más de 9 dígitos" + "\n";
                errores = true;
            }

            else
            {
                if (dniNumber < 0 || dniNumber > 55555555)
                {
                    errorActual = errorActual + "El número de DNI debe tener un máximo de 8 dígitos y un minimo de 1 digito" + "\n";
                    errores = true;
                }
            }

            
            int dropdownValueDia = dropdownDia.value;
            int dropdownValueMes = dropdownMes.value;
            int dropdownValueAño = dropdownAño.value;

            string dia = dropdownDia.options[dropdownValueDia].text;
            string mes = dropdownMes.options[dropdownValueMes].text;
            string anio = dropdownAño.options[dropdownValueAño].text;
            
            if (dropdownValueDia == 0)
            {
                errorActual += "Debe ingresar un dia \n";
                errores = true;
            }
            if (dropdownValueMes == 0)
            {
                errorActual += "Debe ingresar un mes \n";
                errores = true;
            }
            if (dropdownValueAño == 0)
            {
                errorActual += "Debe ingresar un año \n";
                errores = true;
            }



            string fecha = "";
            if (dia.Length < 2)
                fecha += "0";
            fecha += dia + fechaSeparador;
            if (mes.Length < 2)
                fecha += "0";
            fecha += mes + fechaSeparador + anio;

            string format = "dd/MM/yyyy";
            DateTime fechaDeCumpleaños;
            Debug.Log("fecha:" + fecha);
            Debug.Log("format:" + format);

            if (DateTime.TryParseExact(Convert.ToString(fecha), format, null, DateTimeStyles.None, out fechaDeCumpleaños) == false)
            {
                errorActual = errorActual + "La fecha ingresada no posee el formato especificado" + "\n";
                errores = true;
            }

            if (errores)
            {
                mostrarMensaje(errorActual, false);
                return;
            }
            presentador.agregarUsuario(dni, nombre, apellido, fecha);
            
        }

        public void mostrarMensaje(string mensaje, bool shareButtonEnable)
        {
            PanelError.panel.mostrarMensaje(mensaje, shareButtonEnable);
        }

        public void obtenerUsuariosParaEliminar()
        {
            presentador.obtenerUsuarios();
        }

        public void armarDropdownMenuRemoverUsuario(Coleccion<Usuario> usuarios)
        {
            List<string> m_DropOptions = new List<string>();
            dropdownUsuariosRemover.ClearOptions();
            m_DropOptions.Add("Usuarios");

            foreach (Usuario usuario in usuarios.obtenerIterable())
            {
                string text = usuario.obtenerDniNombreApellido(userSeparator);
                m_DropOptions.Add(text);
            }

            dropdownUsuariosRemover.AddOptions(m_DropOptions);

            //No hay usuarios para eliminar
            if (m_DropOptions.Count == 1)
                buttonEliminarUsuario.interactable = false;
            else
                buttonEliminarUsuario.interactable = true;
            
        }

        private void mostrarUsuariosAEliminar(Dropdown listaDeUsuariosAMostrar, List<string> listaDeUsuariosString)
        {
            List<string> m_DropOptions = new List<string>();
            listaDeUsuariosAMostrar.ClearOptions();
            m_DropOptions.Add("Usuarios");

            foreach (string option in listaDeUsuariosString) {         
                string text = option;
                m_DropOptions.Add(text);
            }

            listaDeUsuariosAMostrar.AddOptions(m_DropOptions);

            //No hay usuarios para eliminar
            if (m_DropOptions.Count == 1)
                buttonEliminarUsuario.interactable = false;
            else
                buttonEliminarUsuario.interactable = true;
        }

        // function to enable 
        private void enableObject(GameObject onButton)
        {
            onButton.SetActive(true);
        }

        // function to disable
        private void disableObject(GameObject offButton)
        {
            offButton.SetActive(false);
        }

        private string getDniFromDropdownOption(string option)
        {
            string[] strSplit = option.Split(userSeparator);
            string dni = strSplit[0];
            return dni;
        }

        public void removerUsuario()
        {
            if (dropdownUsuariosRemover.value != 0)
            {
                string option = dropdownUsuariosRemover.options[dropdownUsuariosRemover.value].text;
                string dni = getDniFromDropdownOption(option);
                presentador.eliminarUsuario(dni);
            }
            //Reseteo la informacion
            showInfoUser("");
        }


        /**métodos sin utilizar*/

        public void volverMenuUsuarios()
        {
            disableObject(menuEliminarUsuarios);
            enableObject(menuUsuarios);
        }

        public void onValueChangedDropdownRemoverUsuarios()
        {
            bool interactable = true;
            if (dropdownUsuariosRemover.value != 0)
            {
                string option = dropdownUsuariosRemover.options[dropdownUsuariosRemover.value].text;
                string dni = getDniFromDropdownOption(option);

                presentador.getInfoUser(dni);
            }
            else
            {
                interactable = false;
                showInfoUser("");
            }

            buttonEliminarUsuario.interactable = interactable;
        }


        public void showInfoUser(string informacion)
        {
            textInfoUser.text = informacion;
        }
        
    }

}
