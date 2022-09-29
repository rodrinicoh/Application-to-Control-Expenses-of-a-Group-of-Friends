using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Funcionalidad
{
    public class PanelError : MonoBehaviour
    {

        public GameObject panelError;
        public Text textoError;
        public Button botonError;
        private string errorActual;
        public GameObject panelError2;

        public Text scrollViewTextError;

        //Voy a utilizar esta variable estatica para llamarlo desde otros scripts.
        public static PanelError panel;
        public Button buttonShare;

        // Start is called before the first frame update
        void Start()
        {
        }

        public void inicializar()
        {

            panel = gameObject.GetComponent<PanelError>();

            disableObject(panelError);
            disableObject(panelError2);
        }

        public void mostrarMensaje(string mensaje, bool shareButtonEnable)
        {
            buttonShare.interactable = shareButtonEnable;
            errorActual = mensaje;
            mostrarPanelError();
        }

        private void mostrarPanelError()
        {
            scrollViewTextError.text = errorActual;
            enableObject(panelError);
            enableObject(panelError2);
        }

        public void ocultarPanelError()
        {
            disableObject(panelError);
            disableObject(panelError2);
        }

        public void enableObject(GameObject onButton)
        {
            onButton.SetActive(true);
        }

        public void disableObject(GameObject offButton)
        {
            offButton.SetActive(false);
        }

    }
}
