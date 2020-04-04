#region Usings
using ArbitraryPrecision;
#endregion
namespace UniversalUnitConverterRunning
{
    #region Usings
    #endregion
    static class Program
    {
        #region StaticMethods
        public static int Main ()
        {
            BigDecimal a = new BigDecimal ( 5.2 );
            BigDecimal.Log ( a );
            return 0;
        }
        #endregion
    }
}
