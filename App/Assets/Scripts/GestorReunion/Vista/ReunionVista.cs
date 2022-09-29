using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GestorUsuarios.Modelo;
using GestorDeudas;
using System;
using Colecciones;
using GestorReunion.Modelo;
using GestorReunion.Presentador;
using GestorDeudas.Modelo;
using Exceptions;
using Funcionalidad;
using GestorAlmacenamiento;

namespace GestorReunion
{
    public class ReunionVista : MonoBehaviour
    {
        public Dropdown dropdownQuienPaga;
        public Dropdown dropdownDeudores;

        public Dropdown listaAlgoritmos;

        public Text textScrollViewAcreedor;
        public Text textScrollViewParticipantes;
        public Text textScrollViewCrearReunion;
        public Text textScrollViewActual;

        public Button buttonSeleccionarAcreedor;
        public Button buttonSeleccionarDeudor;
        public Button buttonCrearReunion;

        // Start is called before the first frame update
        public ReunionPresentador presentador;

        public Coleccion<Usuario> usuarios;
        public List<int> usuariosDnis;

        public List<int> participantes;

        public Button buttonReiniciar;
        public Button buttonReiniciarMenuAcreedor;
        public Button buttonReiniciarMenuParticipantes;
        public Button buttonReiniciarMenuCrearReunion;

        public Button buttonMenuReunion;

        public InputField ingresarMonto;
        public Toggle toggleUrgente;

        private UsuarioManager usuarioManager;

        public GameObject menuSeleccionarDeudor;
        public GameObject menuSeleccionarParticipantes;
        public GameObject menuCrearReunion;



        private ReunionManager reunionManager;

        private Coleccion<Usuario> usuariosParticipantes;
        private char userSeparator = '-';
        private string userQuienPaga = "";
        public Button buttonIrMenuSeleccionarParticipantes;

        public Historial historial;

        public void actualizarHistorial(string msg)
        {
            Debug.Log(msg + "***************************************************");
            historial.mostrarMensaje(msg);
        }

        void Awake()
        {

            irMenuSeleccionarAcreedor();

        }

        public void inicializar()
        {
            //Esta lista antes la inciializaba ni bien la declaraba xke sino se me reiniciaba, ver q onda.
            //usuariosDnis = new List<int>();
            presentador = new ReunionPresentador(this);
            reunionManager = new ReunionManager(presentador);

            presentador.setearModelo(reunionManager);


        }

        public void setearDeudasManager(DeudasManager deudasManager){
            reunionManager.setearDeudasManager(deudasManager);
        }


        /**
         * Metodo que se invoca para setear el usuarioManager 
        */
        public void setearUsuarioManagerYAlmacenamiento(UsuarioManager usuarioManager, Almacenamiento database)
        {
            this.usuarioManager = usuarioManager;
            reunionManager.setearUsuarioManagerYAlmacenamiento(usuarioManager, database);

            reiniciar();
        }

        public void reiniciar()
        {
            habilitarButton(buttonSeleccionarDeudor, false);
            habilitarButton(buttonCrearReunion, false);
            habilitarButton(buttonSeleccionarAcreedor, true);

            dropdownQuienPaga.interactable = true;
            dropdownDeudores.interactable = false;

            textScrollViewActual.text = "";

            irMenuSeleccionarAcreedor();
            buttonIrMenuSeleccionarParticipantes.interactable = false;

            //Le solicito los datos al presentador para mostrar.
            presentador.obtenerUsuariosParticipantes();
        }

        private void habilitarButton(Button btn, bool option)
        {
            btn.interactable = option;
        }

        private void crearListaUsuarios(Dropdown lista, List<int> L)
        {
            List<string> m_DropOptions = new List<string>();
            m_DropOptions.Add("Usuarios");
            lista.ClearOptions();
            foreach (int dni in L)
            {
                m_DropOptions.Add(Convert.ToString(dni));
            }
            lista.AddOptions(m_DropOptions);
        }

        /**
         * Arma el dropdown de la lista de acreedores.
        */
        public void armarDropdownQuienPaga(Coleccion<Usuario> usuarios)
        {
            //Debug.Log("armando dropdown quien paga");
            usuariosParticipantes = usuarios;

            List<string> listDropdown = new List<string>();
            listDropdown.Add("Usuarios");
            foreach (Usuario usuario in usuarios.obtenerIterable())
            {
                listDropdown.Add(usuario.obtenerDniNombreApellido(userSeparator));
            }
            dropdownQuienPaga.ClearOptions();
            dropdownQuienPaga.AddOptions(listDropdown);           
        }

        /**
         * 
         * Metodo que se invoca cuando se presiona el boton agregar deudor
         * 
        */
        public void agregarDeudor()
        {
            String errorActual = "";
            int indexDropdown = dropdownDeudores.value;
            if (indexDropdown == 0)
            {
                errorActual = errorActual + "Debe seleccionar un usuario para pagar el gasto" + "\n";
                mostrarMensaje(errorActual, false);
                return;
            }

            string text = dropdownDeudores.options[indexDropdown].text;
            string[] option = text.Split(userSeparator);
            string deudorDNI = option[0];
            string deudorNombre = option[1];
            string deudorApellido = option[2];

            participantes.Add(int.Parse(deudorDNI));

            //Remuevo la opcion del dropdown
            dropdownDeudores.options.RemoveAt(indexDropdown);
            //Seteo la opcion 0 por default.
            dropdownDeudores.value = 0;

            agregarTextoScroll("Nº " + participantes.Count + ": " + deudorDNI + userSeparator + deudorNombre + userSeparator + deudorApellido + "\n");

            habilitarButton(buttonCrearReunion, true);

            //Si hay solo una opcion en el dropdown (la opcion default 'Usuarios'), desactivo dropdown y boton.
            if (dropdownDeudores.options.Count < 2)
            {
                dropdownDeudores.interactable = false;
                buttonSeleccionarDeudor.interactable = false;
            }
        }

        /**
         * Se invoca cuando el usuario hace click en seleccionar acreedor.
         *  
        */
        public void seleccionarAcreedor()
        {
            string errorActual = "";

            if (dropdownQuienPaga.value == 0)
            {
                errorActual = errorActual + "Debe seleccionar un usuario para pagar el gasto" + "\n";
                mostrarMensaje(errorActual, false);
                return;
            }
            buttonIrMenuSeleccionarParticipantes.interactable = true;
            //Sino cargar la lista de los que pueden participar y habilitar botones etc..

            //Desparseo la opcion seleccionada
            string text = dropdownQuienPaga.options[dropdownQuienPaga.value].text;
            string[] split = text.Split(userSeparator);

            //El primer elemento es el dni del usuario.
            string quienPagaDni = split[0];
            string quienPagaNombre = split[1];
            string quienPagaApellido = split[2];

            agregarTextoScroll("Acreedor: " + "\n" + quienPagaDni + userSeparator + quienPagaNombre + userSeparator + quienPagaApellido + "\n");
            agregarTextoScroll("Participantes: \n");


            armarDropdownDeudores(quienPagaDni);

            userQuienPaga = quienPagaDni;

            habilitarButton(buttonSeleccionarDeudor, true);
            habilitarButton(buttonSeleccionarAcreedor, false);

            dropdownQuienPaga.interactable = false;
        }



        private void agregarTextoScroll(string v)
        {
            textScrollViewActual.text += v;
        }

        private void armarDropdownDeudores(string quienPaga)
        {

            List<string> listDropdown = new List<string>();
            listDropdown.Add("Usuarios");

            //Se agregan a los posibles participantse todos los usuarios excepto el que ya pago.
            foreach (Usuario usuario in usuariosParticipantes.obtenerIterable())
            {
                if (usuario.obtenerDni() != int.Parse(quienPaga))
                    listDropdown.Add(usuario.obtenerDniNombreApellido(userSeparator));
            }

            dropdownDeudores.ClearOptions();
            dropdownDeudores.AddOptions(listDropdown);

            dropdownDeudores.interactable = true;
            
            participantes = new List<int>();

        }

        public void mostrarMensaje(string mensaje, bool shareButtonEnable)
        {
            PanelError.panel.mostrarMensaje(mensaje, shareButtonEnable);
        }

        public void crearReunion()
        {

            String cadena = ingresarMonto.text;
            float monto = 0;

            if (!float.TryParse(cadena, out monto))
            {
                mostrarMensaje("El monto ingresado no es un numero valido", false);
                return;
            }

            int dniAcreedor = int.Parse(userQuienPaga);

            bool esUrgente = toggleUrgente.isOn;

            string algoritmo = listaAlgoritmos.options[listaAlgoritmos.value].text;

            //mostrarMensaje("Crear reunion con" + dniAcreedor + "-monto:" + monto + "-algoritmo:" + algoritmo + "-urgente:" + esUrgente);
            try
            {

            presentador.crearReunion(dniAcreedor, participantes , monto, algoritmo, esUrgente, DateTime.Now);
            }
            catch(ReunionException e)
            {
                mostrarMensaje(e.Message, false);
            }
            /*
            if (seCreo)
            {
                agregarEvento("se creó un gasto con sus respectivas deudas");
                mostrarMenuPrincipal();
            }

            */
        }

        public void irMenuSeleccionarAcreedor()
        {
            menuSeleccionarDeudor.SetActive(true);
            menuSeleccionarParticipantes.SetActive(false);
            menuCrearReunion.SetActive(false);

            buttonReiniciar = buttonReiniciarMenuAcreedor;

            string text = textScrollViewActual.text;
            textScrollViewActual = textScrollViewAcreedor;
            textScrollViewActual.text = text;
        }
        public void irMenuSeleccionarParticipantes()
        {
            menuSeleccionarDeudor.SetActive(false);
            menuSeleccionarParticipantes.SetActive(true);
            menuCrearReunion.SetActive(false);

            buttonReiniciar = buttonReiniciarMenuParticipantes;

            string text = textScrollViewActual.text;
            textScrollViewActual = textScrollViewParticipantes;
            textScrollViewActual.text = text;
        }

        public void irMenuCrearReunion()
        {
            menuSeleccionarDeudor.SetActive(false);
            menuSeleccionarParticipantes.SetActive(false);
            menuCrearReunion.SetActive(true);

            buttonReiniciar = buttonReiniciarMenuCrearReunion;

            string text = textScrollViewActual.text;
            textScrollViewActual = textScrollViewCrearReunion;
            textScrollViewActual.text = text;
        }
    }
}
