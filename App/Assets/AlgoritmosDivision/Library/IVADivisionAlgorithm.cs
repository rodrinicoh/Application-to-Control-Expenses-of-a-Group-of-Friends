using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TechFashion
{
    /**
     * Clase que calcula la deuda de cada participante del gasto, 
     * más un recargo del 21% correspondiente al IVA.
     * */
    public class IVADivisionAlgorithm : GreatMoneyDivisionAlgorithm
    {
        private const float IVA = 0.25f;

        public float CalculateDebt(int numberOfParticipats, float moneyAmount)
        {
            float individualDebt = (float)(moneyAmount / (float)numberOfParticipats);
            individualDebt += individualDebt * IVA;
            return individualDebt;
        }
    }
}