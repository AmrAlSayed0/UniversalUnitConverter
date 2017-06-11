namespace UniversalUnitConverterRunning
{
    #region Usings
    using ArbitraryPrecision;
    #endregion
    internal static class Program
    {
        #region StaticMethods
        public static int Main ( string [ ] args )
        {
            //ParseTests.Init(  );
            var a = new BigDecimal ( 5.2 );
            var b = BigDecimal.Log ( a );
            return 0;
        }
        #endregion
    }
}
