using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GestorUsuarios;

namespace GestorDeudas
{
    public class DeudaDNI
    {
        protected int deudor;
        protected int acreedor;
        protected float adeudado;
        protected bool deudaLiquidada;

        public DeudaDNI(int deudor, int acreedor, float adeudado)
        {
            this.deudor = deudor;
            this.acreedor = acreedor;
            this.adeudado = adeudado;
            this.deudaLiquidada = false;
        }

        public float getMonto()
        {
            return adeudado;
        }

        public void setMonto(float m)
        {
            adeudado = m;
        }

        public int obtenerDeudor()
        {
            return deudor;
        }

        public int obtenerAcreedor()
        {
            return acreedor;
        }


    }

}