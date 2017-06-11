namespace UniversalUnitConverterRunning
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using ArbitraryPrecision;
    #endregion
    internal class ParseTests
    {
        #region StaticFields
        private static List < string > _methodList = new List < string >( );
        private static StringBuilder _strBld = new StringBuilder( );
        private static int _currentPrecision;
        private static BigDecimalRoundingMethod _currentRoundingMethod;
        #endregion
        #region StaticMethods
        public static void Init( )
        {
            Regex methodRegex = new Regex ( @"^(?<Method>addx\d{3,4}) add +'?(?<Operand1>.+?)'? +'?(?<Operand2>.+?)'? +-> +'?(?<Result>.+?)'?(?: (?<ResultCondition>[A-Za-z ]+))?$" );
            Regex precisionRegex = new Regex ( @"^precision *: *(?<Precision>\d+)$" );
            Regex roundingMethodRegex = new Regex ( @"rounding *: *(?<RoundingMethod>\w+)" );
            using ( StreamReader sr = new StreamReader ( @"D:\Users\Amr\Downloads\Compressed\dectest\add.decTest" ) )
            {
                while ( sr.Peek( ) >= 0 )
                {
                    string line = sr.ReadLine( );
                    if ( line != null && methodRegex.IsMatch ( line ) )
                    {
                        TextInfo ti = new CultureInfo ( "en-GB" ).TextInfo;
                        Match methodMatch = methodRegex.Match ( line );
                        string methodName = ti.ToTitleCase ( methodMatch.Groups [ "Method" ].Value );
                        string operand1 = methodMatch.Groups [ "Operand1" ].Value;
                        string operand2 = methodMatch.Groups [ "Operand2" ].Value;
                        string result = methodMatch.Groups [ "Result" ].Value;
                        _strBld.Append ( "[TestMethod]public void " + methodName + "(){BigDecimal num1=BigDecimal.Parse(\"" + operand1 + "\",new BigDecimalContext(" + _currentPrecision + ",true,BigDecimalRoundingMethod." + _currentRoundingMethod + "));BigDecimal num2=BigDecimal.Parse(\"" + operand2 + "\",new BigDecimalContext(" + _currentPrecision + ",true,BigDecimalRoundingMethod." + _currentRoundingMethod + "));BigDecimal acNum3=num1+num2;BigDecimal exNum3=BigDecimal.Parse(\"" + result + "\",new BigDecimalContext(" + _currentPrecision + ",true,BigDecimalRoundingMethod." + _currentRoundingMethod + "));Assert.AreEqual(exNum3,acNum3,\"" + methodName + " Works.\");}" );
                        _strBld.Append ( "\r\n" );
                        continue;
                    }
                    if ( line != null && precisionRegex.IsMatch ( line ) )
                    {
                        Match precisionMatch = precisionRegex.Match ( line );
                        _currentPrecision = int.Parse ( precisionMatch.Groups [ "Precision" ].Value );
                        continue;
                    }
                    if ( line != null && roundingMethodRegex.IsMatch ( line ) )
                    {
                        Match roundingMethodMatch = roundingMethodRegex.Match ( line );
                        switch ( roundingMethodMatch.Groups [ "RoundingMethod" ].Value )
                        {
                            case "down" :
                                _currentRoundingMethod = BigDecimalRoundingMethod.RoundDown;
                                break;
                            case "half_up" :
                                _currentRoundingMethod = BigDecimalRoundingMethod.RoundHalfUp;
                                break;
                            case "half_even" :
                                _currentRoundingMethod = BigDecimalRoundingMethod.RoundHalfEven;
                                break;
                            case "ceiling" :
                                _currentRoundingMethod = BigDecimalRoundingMethod.RoundCeiling;
                                break;
                            case "floor" :
                                _currentRoundingMethod = BigDecimalRoundingMethod.RoundFloor;
                                break;
                            case "half_down" :
                                _currentRoundingMethod = BigDecimalRoundingMethod.RoundHalfDown;
                                break;
                            case "up" :
                                _currentRoundingMethod = BigDecimalRoundingMethod.RoundUp;
                                break;
                            case "05up" :
                                _currentRoundingMethod = BigDecimalRoundingMethod.Round05Up;
                                break;
                            default :
                                _currentRoundingMethod = BigDecimalRoundingMethod.RoundHalfUp;
                                Console.WriteLine ( "Default rounding method used." );
                                break;
                        }
                    }
                }
                using ( StreamWriter srWriter = new StreamWriter ( @"D:\Users\Amr\Downloads\add.csharp" ) )
                {
                srWriter.Write ( _strBld );
                }
            }
        }
        #endregion
    }
}
