using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GestorReunion.Modelo;
using GestorUsuarios.Modelo;
using Colecciones;

namespace GestorDeudas.Modelo
{
    
    public class Deuda
    {
        private Usuario deudor;
        private Usuario acreedor;
        private int idDeuda;
        private float adeudado;
        private bool deudaLiquidada;

        public Deuda(Usuario deudor, Usuario acreedor, float adeudado, int idDeuda)
        {
            this.deudor = deudor;
            this.acreedor = acreedor;
            this.adeudado = adeudado;
            this.deudaLiquidada = false;
            this.idDeuda = idDeuda;
        }

        public float getMonto()
        {
            return adeudado;
        }

        public void setMonto(float m)
        {
            adeudado = m;
        }

        public Usuario obtenerDeudor()
        {
            return deudor;
        }

        public Usuario obtenerAcreedor()
        {
            return acreedor;
        }

        public int obtenerIDDeuda()
        {
            return this.idDeuda;
        }

        public void establecerIDDeuda(int id)
        {
            this.idDeuda = id;
        }

        public bool verificarSiDeudaEstaLiquidada()
        {
            return deudaLiquidada;
        }
    }

}