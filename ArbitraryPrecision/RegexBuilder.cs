#region Usings
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
#endregion
namespace ArbitraryPrecision
{
    #region Usings
    #endregion
    static class RegexBuilder
    {
        #region Constants
        const string BackSlash = @"\";
        const string LeftSquareBracket = @"[";
        const string RightSquareBracket = @"]";
        const string Caret = @"^";
        const string DollarSign = @"$";
        const string Dot = @".";
        const string QuestionMark = @"\";
        const string Asterisk = @"*";
        const string PlusSign = @"+";
        const string LeftParentheses = @"(";
        const string RightParentheses = @")";
        const string LeftCurlyBraces = @"{";
        const string RightCurlyBraces = @"{";
        const string Pipe = @"|";
        const string Colon = @":";
        const string LessThan = @"<";
        const string GreaterThan = @">";
        #endregion
        #region StaticMethods
        internal static string RegexSanitize ( string input ) =>
        input.Replace ( BackSlash , BackSlash + BackSlash )
             .Replace ( LeftSquareBracket , BackSlash + LeftSquareBracket )
             .Replace ( Caret , BackSlash + Caret )
             .Replace ( DollarSign , BackSlash + DollarSign )
             .Replace ( Dot , BackSlash + Dot )
             .Replace ( Pipe , BackSlash + Pipe )
             .Replace ( QuestionMark , BackSlash + QuestionMark )
             .Replace ( Asterisk , BackSlash + Asterisk )
             .Replace ( PlusSign , BackSlash + PlusSign )
             .Replace ( LeftParentheses , BackSlash + LeftParentheses )
             .Replace ( RightParentheses , BackSlash + RightParentheses )
             .Replace ( LeftCurlyBraces , BackSlash + LeftCurlyBraces )
             .Replace ( RightCurlyBraces , BackSlash + RightCurlyBraces );
        internal static string [] RegexSanitize ( string [] input )
        {
            int arrLen = input.Length;
            for ( int i = 0; i < arrLen; i++ )
            {
                input [ i ] = RegexSanitize ( input [ i ] );
            }
            return input;
        }
        internal static string CapturingGroup ( string item , string suffix = "" ) => LeftParentheses + item + RightParentheses + suffix;
        internal static string NonCapturingGroup ( string item , string suffix = "" ) => LeftParentheses + QuestionMark + Colon + item + RightParentheses + suffix;
        internal static string NamedCapturingGroup ( string item , string suffix = "" , string name = "" )
        {
            if ( name == string.Empty )
            {
                item = CapturingGroup ( item , suffix );
            }
            else
            {
                item = LeftParentheses + QuestionMark + LessThan + name + GreaterThan + item + RightParentheses + suffix;
            }
            return item;
        }
        internal static string Set ( string item , string suffix = "" ) => LeftSquareBracket + item + RightSquareBracket + suffix;
        //internal static string Alternate ( string [ ] alternations )
        //{
        //    StringBuilder result = new StringBuilder ();
        //    int arrayLen = alternations.Length;
        //    for ( int i = 0;
        //          i < arrayLen;
        //          i++ )
        //    {
        //        result.Append ( alternations [ i ] );
        //        if ( i != arrayLen - 1 )
        //        {
        //            result.Append ( Pipe );
        //        }
        //    }
        //    return result.ToString ();
        //}
        internal static string Alternate ( params string [] alternations )
        {
            StringBuilder result = new StringBuilder ();
            int arrayLen = alternations.Length;
            for ( int i = 0; i < arrayLen; i++ )
            {
                result.Append ( alternations [ i ] );
                if ( i != ( arrayLen - 1 ) )
                {
                    result.Append ( Pipe );
                }
            }
            return result.ToString ();
        }
        static Regex SpecificParsePattern ( NumberStyles numStyles , IFormatProvider numberOrCultureFormatInfo )
        {
            NumberFormatInfo format = NumberFormatInfo.GetInstance ( numberOrCultureFormatInfo );
            string [] saintDigits = RegexSanitize ( format.NativeDigits );
            string saintPositiveSign = RegexSanitize ( format.PositiveSign );
            string saintNegativeSign = RegexSanitize ( format.NegativeSign );
            string saintNaNSymbol = RegexSanitize ( format.NaNSymbol );
            string saintPositiveInfinitySymbol = RegexSanitize ( format.PositiveInfinitySymbol );
            string saintNegativeInfinitySymbol = RegexSanitize ( format.NegativeInfinitySymbol );
            string saintCurrencyDecimalSeparator = RegexSanitize ( format.CurrencyDecimalSeparator );
            string saintCurrencyGroupSeparator = RegexSanitize ( format.CurrencyGroupSeparator );
            string saintCurrencySymbol = RegexSanitize ( format.CurrencySymbol );
            string saintNumberDecimalSeparator = RegexSanitize ( format.NumberDecimalSeparator );
            string saintNumberGroupSeparator = RegexSanitize ( format.NumberGroupSeparator );
            string saintPercentDecimalSeparator = RegexSanitize ( format.PercentDecimalSeparator );
            string saintPercentGroupSeparator = RegexSanitize ( format.PercentGroupSeparator );
            string saintPercentSymbol = RegexSanitize ( format.PercentSymbol );
            string saintPerMilleSymbol = RegexSanitize ( format.PerMilleSymbol );
            StringBuilder saintDigitsString = new StringBuilder ();
            return new Regex ( string.Empty );
        }
        static string GetIntegralDigitsRegex ( string [] saintDigits )
        {
            string s = Alternate ( saintDigits );
            s = NonCapturingGroup ( s , PlusSign );
            s = NamedCapturingGroup ( s , string.Empty , "IntegralDigits" );
            return s;
        }
        #endregion
    }
}
