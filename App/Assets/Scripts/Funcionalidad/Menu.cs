using GestorDeudas;
using GestorReunion;
using GestorUsuarios;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Funcionalidad
{
    public class Menu : MonoBehaviour
    {
        public GameObject menuAgregarUsuario;
        public GameObject menuRemoverUsuario;
        public GameObject menuCrearReunion;
        public GameObject menuDeudas;

        public GameObject menuPrincipal;

        public GameObject menuHistorial;

        public GameObject panelMostrarDeudoresVerDeudas;

        public GameObject panelMostrarDeudasVerDeudas;

        public GameObject menuVerDeudas;


        const string palabra_clave_historial = "%historial";
        //private bool setUsuarioDeudas = true;
        //private bool setUsuarioReunion = true;


        // Start is called before the first frame update
        void Start()
        {
            menuAgregarUsuario.SetActive(true);
            menuAgregarUsuario.SetActive(false);

            menuRemoverUsuario.SetActive(true);
            menuRemoverUsuario.SetActive(false);

            menuCrearReunion.SetActive(false);
            menuCrearReunion.SetActive(true);

            menuDeudas.SetActive(true);
            menuDeudas.SetActive(false);


            menuPrincipal.SetActive(true);
            menuPrincipal.SetActive(false);

            menuHistorial.SetActive(true);
            menuHistorial.SetActive(false);

            mostrarMenu();


            int modo = 0;
            if (modo == 1)
                PlayerPrefs.DeleteAll();
        }


        public void mostrarMenu()
        {
            menuAgregarUsuario.SetActive(true);
            menuAgregarUsuario.SetActive(false);

            menuRemoverUsuario.SetActive(true);
            menuRemoverUsuario.SetActive(false);

            menuCrearReunion.SetActive(true);
            menuCrearReunion.SetActive(false);

            menuPrincipal.SetActive(false);
            menuPrincipal.SetActive(true);

            menuDeudas.SetActive(false);

            menuHistorial.SetActive(false);

            menuVerDeudas.SetActive(false);

            panelMostrarDeudoresVerDeudas.SetActive(false);
            panelMostrarDeudasVerDeudas.SetActive(false);
        }

        public void mostrarMenuAgregarUsuario()
        {

            menuAgregarUsuario.SetActive(true);

            menuRemoverUsuario.SetActive(false);

            menuCrearReunion.SetActive(false);

            menuPrincipal.SetActive(false);

            menuDeudas.SetActive(false);

            menuHistorial.SetActive(false);


            menuVerDeudas.SetActive(false);

            panelMostrarDeudoresVerDeudas.SetActive(false);

            panelMostrarDeudasVerDeudas.SetActive(false);
        }

        public void mostrarMenuReunion()
        {

            menuAgregarUsuario.SetActive(false);

            menuRemoverUsuario.SetActive(false);

            menuCrearReunion.SetActive(true);

            menuPrincipal.SetActive(false);

            menuDeudas.SetActive(false);

            menuHistorial.SetActive(false);

            menuVerDeudas.SetActive(false);

            panelMostrarDeudoresVerDeudas.SetActive(false);
            panelMostrarDeudasVerDeudas.SetActive(false);
        }

        public void mostrarMenuBorrarUsuario()
        {

            menuAgregarUsuario.SetActive(false);

            menuRemoverUsuario.SetActive(true);

            menuCrearReunion.SetActive(false);

            menuPrincipal.SetActive(false);

            menuDeudas.SetActive(false);

            menuVerDeudas.SetActive(false);

            menuHistorial.SetActive(false);

            panelMostrarDeudoresVerDeudas.SetActive(false);
            panelMostrarDeudasVerDeudas.SetActive(false);
        }

        public void mostrarMenuDeudas()
        {
            menuAgregarUsuario.SetActive(false);

            menuRemoverUsuario.SetActive(false);

            menuCrearReunion.SetActive(false);

            menuPrincipal.SetActive(false);

            menuDeudas.SetActive(true);

            menuHistorial.SetActive(false);
        }

        public void mostrarHistorial()
        {
            menuAgregarUsuario.SetActive(false);

            menuRemoverUsuario.SetActive(false);

            menuCrearReunion.SetActive(false);

            menuPrincipal.SetActive(false);

            menuDeudas.SetActive(false);

            menuHistorial.SetActive(true);

            panelMostrarDeudoresVerDeudas.SetActive(false);
            panelMostrarDeudasVerDeudas.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private IEnumerator ScreenshotAndShare(string msg)
        {
            yield return new WaitForEndOfFrame();
            //new NativeShare().SetSubject("my titleardo").SetText("i am el textoooo").AddTarget("com.whatsapp").Share();
            Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            ss.Apply();

            string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
            File.WriteAllBytes(filePath, ss.EncodeToPNG());

            // To avoid memory leaks
            Destroy(ss);

            new NativeShare().AddFile(filePath)
                .SetSubject("Subject goes here").SetText(msg).SetUrl("https://github.com/yasirkula/UnityNativeShare")
                .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
                .Share();

            //new NativeShare().SetSubject("my titleardo").SetText("i am el textoooo").Share();
        }

        public void Share()
        {
            StartCoroutine(ScreenshotAndShare(""));
        }

        public void Share(Text msg)
        {
            StartCoroutine(ScreenshotAndShare(msg.text));
        }

    }
}
