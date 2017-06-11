namespace UniversalUnitConverterRunning
{
    #region Usings
    using System.IO;
    #endregion
    /// <summary>Temporary Working Class</summary>
    public static class UnitListToEnum
    {
        #region StaticMethods
        /// <summary>Temporary Working Method</summary>
        public static void ListToEnum( )
        {
            string line;
            int i = 0;
            string property = "Length";
            string readFile = @"C:\Users\Amr Al Sayed\Documents\Visual Studio 2013\Projects\Other\A.txt";
            string enumFile = @"C:\Users\Amr Al Sayed\Documents\Visual Studio 2013\Projects\Other\" + property + @"Unit.cs";
            string convFile = @"C:\Users\Amr Al Sayed\Documents\Visual Studio 2013\Projects\Other\" + property + @".cs";
            string enumFileHeader = "namespace UniversalUnitConverter\r\n{\r\n    /// <summary>All available " + property.ToLower( ) + " units.</summary>\r\n    public enum " + property + "Unit\r\n    {\r\n";
            string enumFileFooter = "    }\r\n}";
            using ( StreamReader srr = new StreamReader ( readFile ) )
            {
                //using ( StreamWriter srw = new StreamWriter ( @"C:\Users\Amr Al Sayed\Documents\Visual Studio 2013\Projects\UniversalUnitConverter\UC\Units\Length\LengthUnit.cs" ) )
                using ( StreamWriter srwEnum = new StreamWriter ( enumFile ) )
                {
                    using ( StreamWriter srwConv = new StreamWriter ( convFile ) )
                    {
                        srwEnum.Write ( enumFileHeader );
                        while ( ! srr.EndOfStream )
                        {
                            line = srr.ReadLine( );
                            srwEnum.WriteLine ( "    " + line + " = " + i + " ," );
                            srwConv.WriteLine ( "            " + property + "UnitMap.Add ( " + property + "Unit." + line + " , BigDecimal.Parse ( \"1\" ) );" );
                            i++;
                        }
                        srwEnum.Write ( enumFileFooter );
                    }
                }
            }
        }
        #endregion
    }
}
