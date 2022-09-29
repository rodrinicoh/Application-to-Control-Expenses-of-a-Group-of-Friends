using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using GestorDeudas;
using Colecciones;
using GestorUsuarios.Modelo;
using GestorDeudas.Modelo;
namespace GestorAlmacenamiento
{
    public class BackUp : MonoBehaviour
    {
        [SerializeField]
        private string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSco-UstgLa9bFfGP6-oiZxuf2911anhQZ_lNHhXbVmNDde5QA/formResponse";

        private UsuarioManager managerUsuarios;
        private DeudasManager managerDeudas;



        public void setUsuarioManager(UsuarioManager managerusuario)
        {
            managerUsuarios = managerusuario;
        }

        public void setDeudaManager(DeudasManager managerdeuda)
        {
            managerDeudas = managerdeuda;
        }

        public void hacerBackUp()
        {
            string stringDeuda = BuckUpDeudas(managerDeudas.obtenerDeudas());
            string stringUsuario = BuckUpUsers(managerUsuarios.obtenerUsuariosVerDeudas());
            PostUsuarioYDeuda(stringUsuario, stringDeuda);
        }

        private void PostUsuarioYDeuda(string usuarioEnJson, string deudaoEnJson)
        {
            Debug.Log("entre");
            WWWForm form = new WWWForm();
            form.AddField("entry.255701270", usuarioEnJson);
            form.AddField("entry.1859449973", deudaoEnJson);
            byte[] rawData = form.data;
            WWW www = new WWW(BASE_URL, rawData);
            Debug.Log("salí");
        }



        private string armarStringUsuarioSinDeudasJson(Usuario usuario)
        {
            String stringDelUsuario2 = "";
            try
            {
                string stringDeudasDelUsuario = "";
                string nombre;
                string apellido;
                string dni;
                string fechaCumpleaños;
                //Debug.Log(stringDelUsuario2 + "*********************===");
                stringDelUsuario2 = "{ \n                ";

                nombre = usuario.getNombre();
                apellido = usuario.getApellido();
                dni = Convert.ToString(usuario.obtenerDni());
                fechaCumpleaños = Convert.ToString(usuario.obtenerFechaNacimiento());


                //aplico el formato json
                stringDelUsuario2 = stringDelUsuario2 + "                 \"nombre\":\"" + nombre + "\", \n  ";
                stringDelUsuario2 = stringDelUsuario2 + "                                 \"apellido\":\"" + apellido + "\", \n  ";
                stringDelUsuario2 = stringDelUsuario2 + "                                 \"dni\":" + dni + ", \n  ";
                stringDelUsuario2 = stringDelUsuario2 + "                                 \"fechaCumpleaños\":" + fechaCumpleaños + ", \n  ";
                stringDelUsuario2 = stringDelUsuario2 + "} \n                ";

            }
            catch(Exception e)
            {
                Debug.Log("Backup error");
            }
            return stringDelUsuario2;
        }

        private string armarStringJson(Usuario usuario)
        {
            string stringDelUsuario = "";
            try
            {
                string stringDeudasDelUsuario = "";
                string nombre;
                string apellido;
                string dni;
                string fechaCumpleaños;
                List<string> deudasUsuarioString = new List<string>();
                Coleccion<Deuda> deudasUsuario = new ColeccionLista<Deuda>();
                Deuda deudaUsuario;
                Usuario deudorAux;
                Usuario acreedorAux;
                string deudor;
                string acreedor;
                string adeudado;
                string idDeuda;
                string deudaLiquidada;

                stringDelUsuario = "{ \n";

                nombre = usuario.getNombre();
                apellido = usuario.getApellido();
                dni = Convert.ToString(usuario.obtenerDni());
                fechaCumpleaños = Convert.ToString(usuario.obtenerFechaNacimiento());
                deudasUsuario = usuario.obtenerDeudas();


                //aplico el formato json
                stringDelUsuario = stringDelUsuario + "                 \"nombre\":\"" + nombre + "\", \n  ";
                stringDelUsuario = stringDelUsuario + "                 \"apellido\":\"" + apellido + "\", \n  ";
                stringDelUsuario = stringDelUsuario + "                 \"dni\":" + dni + ", \n  ";
                stringDelUsuario = stringDelUsuario + "                 \"fechaCumpleaños\":" + fechaCumpleaños + ", \n  ";
                stringDelUsuario = stringDelUsuario + "                 \"deudas\": [";


                //Debug.Log("longitud deudas usuario:  " + deudasUsuario.longitud());
                for (int j = 0; j < deudasUsuario.longitud(); j++)
                {
                    //Debug.Log("valor de j: " + j);
                    //stringDelUsuario = stringDelUsuario + "   \n  { \n                ";
                    deudaUsuario = deudasUsuario.get(j);
                    deudorAux = deudaUsuario.obtenerDeudor();
                    deudor = armarStringUsuarioSinDeudasJson(deudorAux);
                    acreedorAux = deudaUsuario.obtenerAcreedor();
                    acreedor = armarStringJson(acreedorAux);
                    adeudado = Convert.ToString(deudaUsuario.getMonto());
                    idDeuda = Convert.ToString(deudaUsuario.obtenerIDDeuda());
                    deudaLiquidada = Convert.ToString(deudaUsuario.verificarSiDeudaEstaLiquidada());

                    //aplico el formato json
                    if (j > 0)
                    {
                        stringDeudasDelUsuario = stringDeudasDelUsuario + ",";
                    }
                    stringDeudasDelUsuario = stringDeudasDelUsuario + "\n { \n";
                    stringDeudasDelUsuario = stringDeudasDelUsuario + "                                 \"deudor\":" + deudor + ", \n  ";
                    stringDeudasDelUsuario = stringDeudasDelUsuario + "                                 \"acreedor\":" + acreedor + ", \n  ";
                    stringDeudasDelUsuario = stringDeudasDelUsuario + "                                 \"adeudado\":" + adeudado + ", \n  ";
                    stringDeudasDelUsuario = stringDeudasDelUsuario + "                                 \"idDeuda\":" + idDeuda + ", \n  ";
                    stringDeudasDelUsuario = stringDeudasDelUsuario + "                                 \"deudaLiquidada\":" + deudaLiquidada + ", \n  ";
                    stringDeudasDelUsuario = stringDeudasDelUsuario + "                                }";
                }
                stringDelUsuario = stringDelUsuario + stringDeudasDelUsuario;
                //stringDelUsuario = stringDelUsuario + "\n    ";
                stringDelUsuario = stringDelUsuario + "]\n    ";
                stringDelUsuario = stringDelUsuario + "}\n    ";

            }
            catch (Exception e)
            {
                Debug.Log("backup exception");
            }
            return stringDelUsuario;

        }

        public string BuckUpUsers(Coleccion<Usuario> usuarios)
        {
            Usuario usuarioAux;
            string stringUsuario = "";
            for (int i = 0; i < usuarios.longitud(); i++)
            {
                Debug.Log("valor de i: " + i);
                usuarioAux = usuarios.get(i);
                /*
                stringUsuario = stringUsuario + JsonUtility.ToJson(usuarioAux);
                Debug.Log("json un usuario: "+JsonUtility.ToJson(usuarioAux));*/

                stringUsuario = stringUsuario + armarStringJson(usuarioAux);
                Debug.Log("json un usuario: " + armarStringJson(usuarioAux));
            }
            Debug.Log("string con todos los usuarios:" + stringUsuario);

            Debug.Log("epa");

            return stringUsuario;

        }


        private string armarStringDeudaJson(Deuda deuda)
        {
            string stringDeuda = "";
            Usuario deudorAux;
            Usuario acreedorAux;
            string deudor;
            string acreedor;
            string adeudado;
            string idDeuda;
            string deudaLiquidada;


            deudorAux = deuda.obtenerDeudor();
            deudor = armarStringUsuarioSinDeudasJson(deudorAux);
            acreedorAux = deuda.obtenerAcreedor();
            acreedor = armarStringUsuarioSinDeudasJson(acreedorAux);
            adeudado = Convert.ToString(deuda.getMonto());
            idDeuda = Convert.ToString(deuda.obtenerIDDeuda());
            deudaLiquidada = Convert.ToString(deuda.verificarSiDeudaEstaLiquidada());

            stringDeuda = stringDeuda + "\n { \n                ";
            stringDeuda = stringDeuda + " \"deudor\":" + deudor + ", \n  ";
            stringDeuda = stringDeuda + " \"acreedor\":" + acreedor + ", \n  ";
            stringDeuda = stringDeuda + " \"adeudado\":" + adeudado + ", \n  ";
            stringDeuda = stringDeuda + " \"idDeuda\":" + idDeuda + ", \n  ";
            stringDeuda = stringDeuda + " \"deudaLiquidada\":" + deudaLiquidada + ", \n  ";
            stringDeuda = stringDeuda + "}";

            return stringDeuda;
        }
        public string BuckUpDeudas(Coleccion<Deuda> deudas)
        {
            Deuda deudaAux;
            string stringDeuda = "";
            //Debug.Log("Cantidad de deudas: " + deudas.longitud()+ "**************************************************************************************************");
            for (int i = 0; i < deudas.longitud(); i++)
            {
                deudaAux = deudas.get(i);

                stringDeuda = stringDeuda + armarStringDeudaJson(deudaAux);
            }
            //Debug.Log("string completo " + stringDeuda);

            //Debug.Log("epa");

            return stringDeuda;

        }
    }
}
