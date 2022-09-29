using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechFashion;

namespace AlgoritmosDivision
{
    public class AlgoritmoIVA : DivisionGastos
    {
        GreatMoneyDivisionAlgorithm divisionAlgorithm;
        public AlgoritmoIVA()
        {
            divisionAlgorithm = new IVADivisionAlgorithm();
        }

        public float ejecutarDivision(int cantidadParticipantes, float monto)
        {
            return divisionAlgorithm.CalculateDebt(cantidadParticipantes, monto);
        }
    }
}
