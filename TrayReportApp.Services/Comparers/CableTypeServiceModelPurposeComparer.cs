using System.Diagnostics.CodeAnalysis;
using TrayReportApp.Services.Models;

namespace TrayReportApp.Services.Comparers
{
    public class CableTypeServiceModelPurposeComparer : IEqualityComparer<CableTypeServiceModel>
    {
        public bool Equals(CableTypeServiceModel? x, CableTypeServiceModel? y)
        {
            if (x == null || y == null)
                return false;

            return string.Equals(x.Purpose, y.Purpose, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode([DisallowNull] CableTypeServiceModel obj)
        {
            return obj.Purpose?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0;
        }
    }
}
