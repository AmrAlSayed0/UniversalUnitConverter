namespace UniversalUnitConverterScratchPad
{
    #region Usings
    using ArbitraryPrecision;
    #endregion
    public static class Program
    {
        #region StaticMethods
        public static void Main ( string [ ] args )
        {
            //BigInteger a = new BigInteger ( 255 );
            //byte [ ] byteArr = a.ToByteArray( );
            //foreach ( byte b in byteArr )
            //{
            //Console.WriteLine ( "{0} = {1:X2} " , b , b );
            //}
            //Console.WriteLine( );
            //Console.WriteLine( );
            //byte [ ] byteArrRev = a.ToByteArray( ).Reverse( ).ToArray( );
            //byte [ ] truncByteArr =
            //{
            //byteArrRev [ 1 ]
            //};
            //Console.WriteLine ( new BigInteger ( truncByteArr ) );
            BigDecimal a = ( decimal ) 5013.567892;
        }
        #endregion
    }
    //public static class BigIntegerExtensions
    //{
    //    #region StaticMethods
    //    public static string ToBase64String ( this BigInteger source )
    //    {
    //        byte [ ] bigIntBytes = source.ToByteArray( ).Reverse( ).ToArray( );
    //        return Convert.ToBase64String ( bigIntBytes );
    //    }
    //    #endregion
    //}
}
