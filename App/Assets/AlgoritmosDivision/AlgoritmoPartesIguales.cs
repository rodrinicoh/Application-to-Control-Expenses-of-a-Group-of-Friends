using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmosDivision
{
    public class AlgoritmoPartesIguales : DivisionGastos
    {
        public float ejecutarDivision(int cantidadParticipantes, float monto)
        {
            return monto / cantidadParticipantes;
        }
        
    }
}
