using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TechFashion
{
    public interface GreatMoneyDivisionAlgorithm 
    {
        float CalculateDebt(int numberOfParticipats, float moneyAmount);
    }
}