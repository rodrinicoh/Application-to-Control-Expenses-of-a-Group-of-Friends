using GestorUsuarios.Modelo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Colecciones;
using GestorDeudas.Modelo;
using GestorDeudas.Presentador;
using GestorAlmacenamiento;
using Funcionalidad;

namespace GestorDeudas.Vista
{
    public class DeudasVista : MonoBehaviour
    {
        private Dropdown dropdownDeudores;
        public Dropdown dropdownAcreedores;

        public Dropdown dropdownDeudoresMenu1;
        public Button buttonSeleccionarDeudorMenu1;

        public Dropdown dropdownDeudoresMenu2;
        public Button buttonSeleccionarDeudorMenu2;

        private Text scrollViewText;
        public Text scrollViewTextMenu1;
        public Text scrollViewTextMenu2;

        public InputField inputFieldMonto;

        public Toggle toggleTotal;

        public DeudasPresentador presentador;

        public Button buttonReiniciar;
        public Button buttonSeleccionarDeudor;
        public Button buttonSiguiente;
        public Button buttonLiquidarDeudas;

        public GameObject menuMostrarDeudas;

        public GameObject panelMostrarDeudoresMenu1;
        public GameObject panelMostrarDeudasMenu1;


        public GameObject menuLiquidarDeudas;

        public GameObject panelMostrarDeudoresMenu2;
        public GameObject panelMostrarDeudasMenu2;


        private GameObject panelMostrarDeudores;
        private GameObject panelMostrarDeudas;
        public GameObject panelLiquidarDeudas;

        public GameObject MenuDeudas;


        //private bool isActive = false;
        private UsuarioManager usuarioManager;
        private DeudasManager deudasManager;
        private char userSeparator = '-';

        public BackUp backup;

        public Historial historial;

        public void inicializar()
        {
            presentador = new DeudasPresentador(this);
            deudasManager = new DeudasManager(presentador);
            presentador.setearModelo(deudasManager);

        }


        public DeudasManager obtenerDeudasManager(){
        	return deudasManager;
        }

        private void backUp() {
            backup.hacerBackUp();
        }
        

        public void setearUsuarioManagerYAlmacenamiento(UsuarioManager usuarioManager, Almacenamiento database)
        {
            this.usuarioManager = usuarioManager;
            deudasManager.setearUsuarioManagerYAlmacenamiento(usuarioManager, database);


            backup.setUsuarioManager(usuarioManager);
            backup.setDeudaManager(deudasManager);
            InvokeRepeating("backUp", 0, 300);

        }

        public void reiniciar()
        {
            dropdownDeudores.interactable = true;
            buttonSeleccionarDeudor.interactable = false;
            buttonSiguiente.interactable = false;
            dropdownAcreedores.interactable = false;
            toggleTotal.isOn = false;
            scrollViewText.text = "";
        }

        public void reiniciarMenuVerDeudas()
        {
            dropdownDeudores = dropdownDeudoresMenu1;
            buttonSeleccionarDeudor = buttonSeleccionarDeudorMenu1;
            scrollViewText = scrollViewTextMenu1;

            reiniciar();

            obtenerUsuariosVerDeudas();
            mostrarPanelMostrarDeudores();
        }

        public void reiniciarMenuLiquidarDeudas()
        {
            dropdownDeudores = dropdownDeudoresMenu2;
            buttonSeleccionarDeudor = buttonSeleccionarDeudorMenu2;
            scrollViewText = scrollViewTextMenu2;
            reiniciar();

            obtenerUsuariosDeudores();
            mostrarPanelMostrarDeudores();
        }


        public void obtenerUsuariosDeudores()
        {
            presentador.obtenerUsuariosParticipantes();
        }

        public void obtenerUsuariosVerDeudas()
        {
            presentador.obtenerUsuariosDeudores();
        }

        public void actualizarUsuarios(List<Usuario> usuarios)
        {
            List<string> listDropdown = new List<string>();
            listDropdown.Add("Usuarios");
            foreach(Usuario usuario in usuarios)
            {
                listDropdown.Add(usuario.obtenerDniNombreApellido(userSeparator));
                //listDropdown.Add(usuario.obtenerDni().ToString() + "-" + usuario.getNombre());
            }
            dropdownDeudores.ClearOptions();
            dropdownDeudores.AddOptions(listDropdown);
        }



        public void seleccionarDeudor()
        {
            int optionSelected = dropdownDeudores.value;
            if (optionSelected == 0) //Si la opcion seleccionada es 'Usuarios', no hago nada.
                return;
            string option = dropdownDeudores.options[optionSelected].text;

            string[] text = option.Split(userSeparator);
            string dniSelected = text[0];
            dropdownDeudores.interactable = false;
            buttonSeleccionarDeudor.interactable = false;
            dropdownAcreedores.interactable = true;

            //presentador.obtenerUsuariosAcreedores(dniSelected);
            presentador.obtenerDeudasUsuario(dniSelected);

            mostrarPanelMostrarDeudas();
        }

        public void mostrarMenuDeudas()
        {
            MenuDeudas.SetActive(true);
            menuLiquidarDeudas.SetActive(false);
            menuMostrarDeudas.SetActive(false);

            panelMostrarDeudasMenu1.SetActive(false);
            panelMostrarDeudasMenu2.SetActive(false);
        }

        public void mostrarMenuLiquidarDeudas()
        {
            menuLiquidarDeudas.SetActive(true);
            menuMostrarDeudas.SetActive(false);

            panelMostrarDeudas = panelMostrarDeudasMenu2;
            panelMostrarDeudores = panelMostrarDeudoresMenu2;

            panelMostrarDeudasMenu1.SetActive(false);
            panelMostrarDeudasMenu2.SetActive(false);

            reiniciarMenuLiquidarDeudas();
            mostrarPanelMostrarDeudores();
        }

        public void mostrarMenuVerDeudas()
        {
            menuMostrarDeudas.SetActive(true);
            menuLiquidarDeudas.SetActive(false);

            panelMostrarDeudas = panelMostrarDeudasMenu1;
            panelMostrarDeudores = panelMostrarDeudoresMenu1;

            reiniciarMenuVerDeudas();
            mostrarPanelMostrarDeudores();
        }

        public void mostrarPanelMostrarDeudores()
        {
            panelMostrarDeudores.SetActive(true);
            panelMostrarDeudas.SetActive(false);
            panelLiquidarDeudas.SetActive(false);
        }

        public void mostrarPanelMostrarDeudas()
        {
            panelMostrarDeudores.SetActive(false);
            panelMostrarDeudas.SetActive(true);
            panelLiquidarDeudas.SetActive(false);
        }

        public void mostrarPanelLiquidarDeuda()
        {
            panelMostrarDeudores.SetActive(false);
            panelMostrarDeudas.SetActive(false);
            panelLiquidarDeudas.SetActive(true);
        }
                
        public void actualizarDatosDeudas(string msg)
        {
            scrollViewText.text = msg;
        }


        public void actualizarDeudasDeudor(Coleccion<Deuda> deudas)
        {
            String msg = "";
            List<string> idDeudas = new List<string>();
            foreach (Deuda deuda in deudas.obtenerIterable())
            {
                Usuario deudor = deuda.obtenerDeudor();
                Usuario acreedor = deuda.obtenerAcreedor();

                bool urgente = deudasManager.esDeudaUrgente(deuda);

                int idDeuda = deuda.obtenerIDDeuda();

                msg += "Deuda Nº " + idDeuda + "\n";
                msg += "Deudor: " + deudor.obtenerDniNombreApellido(userSeparator) + "\n";
                msg += "Acreedor: " + acreedor.obtenerDniNombreApellido(userSeparator) + "\n";
                msg += "Monto: " + deuda.getMonto() + "\n";
                msg += "Tipo de deuda: ";
                if (urgente)
                    msg += "Urgente \n \n";
                else
                    msg += "Normal \n \n";
                idDeudas.Add(idDeuda.ToString());
            }

            actualizarDatosDeudas(msg);

            actualizarDropdownDeudas(idDeudas);

        }

        private void actualizarDropdownDeudas(List<string> idDeudas)
        {
            idDeudas.Insert(0, "Numero de deuda");
            dropdownAcreedores.ClearOptions();
            dropdownAcreedores.AddOptions(idDeudas);
        }


        public void liquidarDeuda()
        {
            int dropdownValue = dropdownAcreedores.value;

            
            if (dropdownValue == 0)
            {
                mostrarMensaje("Debe seleccionar una deuda para liquidar\n", false);
                return;
            }

            string inputMonto = inputFieldMonto.text;
            float monto = 0;
            bool total = false;
            if (inputMonto != "")
            {
                if (!Single.TryParse(inputMonto, out monto))
                {
                    mostrarMensaje("Ingrese un monto valido \n", false);
                    return;
                }
            }
            else
            {
                if (toggleTotal.isOn)
                    total = true;
                else
                {
                    mostrarMensaje("Debe ingresar un importe o seleccionar el toggle!\n", false);
                    return;
                }
            }
            string idDeuda = dropdownAcreedores.options[dropdownValue].text;
            string stringDeudor = dropdownDeudores.options[dropdownDeudores.value].text;
            string deudor = stringDeudor.Split(userSeparator)[0];
            presentador.liquidarDeuda(deudor, idDeuda, monto, total);

            reiniciarMenuLiquidarDeudas();
        }


        //Lo utilizo para habilitar o no el boton de buscar deudas segun lo que este seleccionado.
        public void onDropdownDeudoresChange()
        {
            bool interactable = true;
            if (dropdownDeudores.value == 0)
                interactable = false;
            buttonSeleccionarDeudor.interactable = interactable;
        }

        //Lo utilizo para habilitar o no el boton de buscar liquidar segun lo que este seleccionado.
        public void onDropdownDeudasChange()
        {
            bool interactable = true;
            if (dropdownAcreedores.value == 0)
                interactable = false;
            else
                toggleTotal.isOn = false;
            buttonSiguiente.interactable = interactable;
        }


        public void onToggleChange()
        {
            bool interactable = false;
            if (toggleTotal.isOn)
            {
                interactable = true;
                inputFieldMonto.text = "";
            }
            buttonLiquidarDeudas.interactable = interactable;
        }

        public void onInputFieldMontoChange()
        {
            string texto = inputFieldMonto.text;
            bool interactable = false;
            if (texto != "")
            {
                interactable = true;
                toggleTotal.isOn = false;
            }
            buttonLiquidarDeudas.interactable = interactable;
        }


        public void simplificar()
        {
            presentador.simplificar();
        }


        public void mostrarHistorial(string msg)
        {
            historial.mostrarMensaje(msg);
        }

        public void mostrarMensaje(string mensaje, bool shareButtonEnable)
        {
            PanelError.panel.mostrarMensaje(mensaje, shareButtonEnable);
        }

    }
}