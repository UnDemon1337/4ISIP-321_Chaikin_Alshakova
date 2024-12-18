using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessPartners
{
    public static class MaterialCalculator
    {
        public static int CalculateMaterialRequirement(int productTypeId, int materialTypeId, int quantity, double param1, double param2, double coefficient, double defectRate)
        {
            if (productTypeId <= 0 || materialTypeId <= 0 || quantity <= 0 || param1 <= 0 || param2 <= 0 || coefficient <= 0 || defectRate < 0)
                return -1;
            double materialPerUnit = param1 * param2 * coefficient;
            double totalMaterial = materialPerUnit * quantity;
            totalMaterial += totalMaterial * defectRate;
            return (int)Math.Ceiling(totalMaterial);
        }
    }
}
