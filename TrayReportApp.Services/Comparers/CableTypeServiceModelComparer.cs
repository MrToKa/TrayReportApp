using System;
using System.Collections.Generic;
using TrayReportApp.Services.Models;

namespace TrayReportApp.Services.Comparers
{
    public class CableTypeServiceModelComparer : IEqualityComparer<CableTypeServiceModel>
    {
        public bool Equals(CableTypeServiceModel x, CableTypeServiceModel y)
        {
            if (x == null || y == null)
                return false;

            return string.Equals(x.Type, y.Type, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(CableTypeServiceModel obj)
        {
            return obj.Type?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0;
        }
    }
}
