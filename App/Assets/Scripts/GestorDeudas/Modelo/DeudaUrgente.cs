using GestorUsuarios.Modelo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GestorDeudas.Modelo
{
    public class DeudaUrgente : Deuda
    {
        public DeudaUrgente(Usuario deudor, Usuario acreedor, float adeudado, int idDeuda)
            : base(deudor, acreedor, adeudado, idDeuda)

        {

        }
    }
}
