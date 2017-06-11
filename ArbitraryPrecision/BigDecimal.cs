namespace ArbitraryPrecision
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Text.RegularExpressions;
    #endregion
    /// <summary>Represents an arbitrarily large decimal number.</summary>
    /// <seealso cref = "IFormattable" />
    /// <seealso cref = "IComparable" />
    /// <seealso cref = "IComparable{BigDecimal}" />
    /// <seealso cref = "IEquatable{BigDecimal}" />
    [ Serializable ]
    public struct BigDecimal : IFormattable , IComparable , IComparable < BigDecimal > , IEquatable < BigDecimal >
    {
        #region StaticFields
        /// <summary>Specifies the number of summations performed during the calculation of the Taylor series used for calculating exponentials an logarithms.</summary>
        private static int MaxN = 1000;
        //TODO: Deprecate and implement the IFormatProvider and the NumberStyles format.
        /// <summary>Regex pattern used to match a number string. (Relaxed Culture Invariant)</summary>
        private static readonly Regex _relaxedParsePattern = new Regex ( @"(?imsx:
                                                                          ^   (?#Has to be the Begining of the String.)
                                                                          (?<LeadingSpaces>\s*)                   (?#Spaces at the Beginig of the Number, Captured But Ignored.)
                                                                          (?<NumberSign>[-+\s]*[-+])?             (?#Multiple Number Signs with Spaces Inbetween, Captured For Filtering.)
                                                                          (?<SignSpaces>\s*)                      (?#Spaces After the Last Sign and Before the First Digit, Captured But Ignored.)
                                                                          (?<LeadingRedundantZeros>[0,]*)         (?#Leading Redundant Zeros Before the Begining of the First Significant Digit [If Any] With Digit Separators Inbetween, Captured But Ignored.)
                                                                          (?<BeforeDecimal>[\d,]*)                (?#Digits Before the Decimal Point [If Any] with Digit Separators Inbetween, Captured For Filtering.)
                                                                          (?#Optional Fraction Part)
                                                                          (?:\.(?<AfterDecimal>\d*?)              (?#Digits After the Decimal Point, Captured for Use)
                                                                          (?<TrailingRedundantZeros>(?<!0)0*))?   (?#Trailing Redundant Zeros after the Fraction Part, Captured But Ignored.)
                                                                          (?<NumberSpaces>\s*)                    (?#Spaces After the Fraction Part, Captured But Ignored.)
                                                                          (?:[Ee](?<ESpaces>\s*)                  (?#Spaces After the Exponent Notifier, Captured But Ignored.)
                                                                          (?<ExponentSign>[-+\s]*[-+])?           (?#Multiple Exponent Signs with Spaces Inbetween, Captured For Filtering.)
                                                                          (?<EponentSignSpaces>\s*)               (?#Spaces After the Exponent Sign, Captured But Ignored.)
                                                                          (?<ExponentRedundantZeros>[0,]*)        (?#Leading Redundant Zeros Befor the Begining of the First Significant Digit [If Any] of the Exponent With Digit Separators Inbetween, Captured But Ignored)
                                                                          (?<ExponentNumber>[\d,]*))?             (?#Exponent Digits With Digit Separators Inbetween, Captured For Filtering.)
                                                                          (?<TrailingSpaces>\s*)                  (?#Trailing Spaces after the End of the Number, Captured But Ignored.)
                                                                          $   (?#Has to be the End of the String.)
                                                                          )" );
        /// <summary>Regex used to match the Standard Numeric Format.</summary>
        private static readonly Regex _standardNumericFormatPattern = new Regex ( @"^(?i:(?<FormatSpecifier>[CcDdEeFfGgNnPpRrXx]))(?<PrecisionSpecifier>[0-9]*)$" );
        //TODO: Create the Regex for the Custom Numeric Format. (Possibly multiple Regexes)
        /// <summary>Regex used to match the Custom Numeric Format.</summary>
        private static readonly Regex _customNumericFormatPattern = new Regex ( "^$" );
        /// <summary>A decimal whose value is zero (0).</summary>
        private static readonly BigDecimal _zero = new BigDecimal
                                                   {
                                                       _currentContext = BigDecimalContext.DefaultContext ,
                                                       _sign = 0 ,
                                                       _mantissa = 0 ,
                                                       _exponent = 0 ,
                                                       _isEven = true ,
                                                       _isOdd = false ,
                                                       _isInteger = true ,
                                                       _isSpecialValue = false
                                                   };
        /// <summary>A decimal whose value is one (1).</summary>
        private static readonly BigDecimal _one = new BigDecimal
                                                  {
                                                      _currentContext = BigDecimalContext.DefaultContext ,
                                                      _sign = 1 ,
                                                      _mantissa = 1 ,
                                                      _exponent = 0 ,
                                                      _isEven = false ,
                                                      _isOdd = true ,
                                                      _isInteger = true ,
                                                      _isSpecialValue = false
                                                  };
        /// <summary>A decimal whose value is negative one (-1).</summary>
        private static readonly BigDecimal _minusOne = new BigDecimal
                                                       {
                                                           _currentContext = BigDecimalContext.DefaultContext ,
                                                           _sign = - 1 ,
                                                           _mantissa = - 1 ,
                                                           _exponent = 0 ,
                                                           _isEven = false ,
                                                           _isOdd = true ,
                                                           _isInteger = true ,
                                                           _isSpecialValue = false
                                                       };
        /// <summary>A number whose value is positively infinite. (+∞)</summary>
        private static readonly BigDecimal _positiveInfinity = new BigDecimal
                                                               {
                                                                   _currentContext = BigDecimalContext.PositiveInfinityContext ,
                                                                   _sign = 1 ,
                                                                   _mantissa = 0 ,
                                                                   _exponent = 0 ,
                                                                   _isEven = false ,
                                                                   _isOdd = false ,
                                                                   _isInteger = false ,
                                                                   _isSpecialValue = true
                                                               };
        /// <summary>A number whose value is negatively infinite. (-∞)</summary>
        private static readonly BigDecimal _negativeInfinity = new BigDecimal
                                                               {
                                                                   _currentContext = BigDecimalContext.NegativeInfinityContext ,
                                                                   _sign = - 1 ,
                                                                   _mantissa = 0 ,
                                                                   _exponent = 0 ,
                                                                   _isEven = false ,
                                                                   _isOdd = false ,
                                                                   _isInteger = false ,
                                                                   _isSpecialValue = true
                                                               };
        /// <summary>A number whose value is negative zero. (-0)</summary>
        private static readonly BigDecimal _negativeZero = new BigDecimal
                                                           {
                                                               _currentContext = BigDecimalContext.NegativeZeroContext ,
                                                               _sign = - 1 ,
                                                               _mantissa = 0 ,
                                                               _exponent = 0 ,
                                                               _isEven = true ,
                                                               _isOdd = false ,
                                                               _isInteger = true ,
                                                               _isSpecialValue = true
                                                           };
        /// <summary>A value that is not a number. (Not-a-Number)</summary>
        private static readonly BigDecimal _nan = new BigDecimal
                                                  {
                                                      _currentContext = BigDecimalContext.NanContext ,
                                                      _sign = 0 ,
                                                      _mantissa = 0 ,
                                                      _exponent = 0 ,
                                                      _isEven = false ,
                                                      _isOdd = false ,
                                                      _isInteger = false ,
                                                      _isSpecialValue = true
                                                  };
        #endregion
        #region Fields
        /// <summary>The integer part of the number.</summary>
        private BigInteger _mantissa;
        /// <summary>The exponent part of the number.</summary>
        private BigInteger _exponent;
        /// <summary>A number that indicates the sign of the <see cref = "BigDecimal" />, as shown in the following table:
        ///     <list type = "table" >
        ///         <listheader>
        ///             <term>Number</term>
        ///             <term>Description</term>
        ///         </listheader>
        ///         <item>
        ///             <term>-1</term>
        ///             <term>The value of this <see cref = "BigDecimal" /> is negative.</term>
        ///         </item>
        ///         <item>
        ///             <term>0</term>
        ///             <term>The value of this <see cref = "BigDecimal" /> is 0 (zero) or NaN (Not-a-Number).</term>
        ///         </item>
        ///         <item>
        ///             <term>1</term>
        ///             <term>The value of this <see cref = "BigDecimal" /> is positive.</term>
        ///         </item>
        ///     </list>
        /// </summary>
        private int _sign;
        /// <summary>Specifies the context (settings) for the current <see cref = "BigDecimal" /> struct.</summary>
        private BigDecimalContext _currentContext;
        /// <summary>Specifies whether the current <see cref = "BigDecimal" /> is a special value.</summary>
        private bool _isSpecialValue;
        /// <summary>Specifies whether the current <see cref = "BigDecimal" /> is an integer.</summary>
        private bool _isInteger;
        /// <summary>Specifies whether the current <see cref = "BigDecimal" /> is even.</summary>
        private bool _isEven;
        /// <summary>Specifies whether the current <see cref = "BigDecimal" /> is odd.</summary>
        private bool _isOdd;
        #endregion
        #region StaticProperties
        /// <summary>Gets the regex used to match the Standard Numeric Format.</summary>
        /// <value>The regex pattern used to match the Standard Numeric Format.</value>
        private static Regex StandardNumericFormatPattern
        {
            get
            {
            return _standardNumericFormatPattern;
            }
        }
        /// <summary>Gets the regex used to match the Custom Numeric Format.</summary>
        /// <value>The regex pattern used to match the Custom Numeric Format.</value>
        private static Regex CustomNumericFormatPattern
        {
            get
            {
            return _customNumericFormatPattern;
            }
        }
        //TODO: Deprecate and implement the IFormatProvider and the NumberStyles format.
        /// <summary>Gets the regex pattern used to match a number string. This pattern is very relaxed and is culture invariant.</summary>
        /// <value>The regex pattern used to match a number string.</value>
        private static Regex RelaxedParsePattern
        {
            get
            {
            return _relaxedParsePattern;
            }
        }
        /// <summary>Gets a value that represents the number 0 (zero).</summary>
        /// <value>A decimal whose value is zero (0).</value>
        public static BigDecimal Zero
        {
            get
            {
            return _zero;
            }
        }
        /// <summary>Gets a value that represents the number one (1).</summary>
        /// <value>A decimal whose value is one (1).</value>
        public static BigDecimal One
        {
            get
            {
            return _one;
            }
        }
        /// <summary>Gets a value that represents the number negative one (-1).</summary>
        /// <value>A decimal whose value is negative one (-1).</value>
        public static BigDecimal MinusOne
        {
            get
            {
            return _minusOne;
            }
        }
        /// <summary>Gets a number whose value is positively infinite. (+∞)</summary>
        /// <value>A number whose value is positively infinite. (+∞)</value>
        public static BigDecimal PositiveInfinity
        {
            get
            {
            return _positiveInfinity;
            }
        }
        /// <summary>Gets a number whose value is negatively infinite. (-∞)</summary>
        /// <value>A number whose value is negatively infinite. (-∞)</value>
        public static BigDecimal NegativeInfinity
        {
            get
            {
            return _negativeInfinity;
            }
        }
        /// <summary>Gets a number whose value is negative zero. (-0)</summary>
        /// <value>A number whose value is negative zero. (-0)</value>
        public static BigDecimal NegativeZero
        {
            get
            {
            return _negativeZero;
            }
        }
        /// <summary>Gets a value that is not a number. (Not-a-Number)</summary>
        /// <value>A value that is not a number. (Not-a-Number)</value>
        public static BigDecimal NaN
        {
            get
            {
            return _nan;
            }
        }
        #endregion
        #region Properties
        /// <summary>Gets or sets the integer part of the number.</summary>
        /// <value>The integer part of the number.</value>
        public BigInteger Mantissa
        {
            get
            {
            return _mantissa;
            }
            set
            {
                if ( ! IsSpecialValue )
                {
                    _mantissa = value;
                    _sign = _mantissa.Sign;
                }
                else
                {
                _mantissa = BigInteger.Zero;
                }
            }
        }
        /// <summary>Gets or sets the exponent part of the number.</summary>
        /// <value>The exponent part of the number.</value>
        public BigInteger Exponent
        {
            get
            {
            return _exponent;
            }
            set
            {
                if ( ! IsSpecialValue )
                {
                _exponent = value;
                }
                else
                {
                _exponent = BigInteger.Zero;
                }
            }
        }
        /// <summary>Gets or sets a number that indicates the sign (negative, positive, or zero) of the current <see cref = "BigDecimal" />.</summary>
        /// <exception cref = "InvalidOperationException" >Thrown when a sign is set to a value that is not a number (NaN).</exception>
        /// <value>A number that indicates the sign of the <see cref = "BigDecimal" /> object, as shown in the following table:
        ///     <list type = "table" >
        ///         <listheader>
        ///             <term>Number</term>
        ///             <term>Description</term>
        ///         </listheader>
        ///         <item>
        ///             <term>-1</term>
        ///             <term>The value of this <see cref = "BigDecimal" /> is negative.</term>
        ///         </item>
        ///         <item>
        ///             <term>0</term>
        ///             <term>The value of this <see cref = "BigDecimal" /> is 0 (zero) or NaN (Not-a-Number).</term>
        ///         </item>
        ///         <item>
        ///             <term>1</term>
        ///             <term>The value of this <see cref = "BigDecimal" /> is positive.</term>
        ///         </item>
        ///     </list></value>
        /// <remarks>Assigning a zero to the Sign property will automatically make this <see cref = "BigDecimal" /> a <see cref = "BigDecimal.Zero" /> or a <see cref = "BigDecimal.NaN" /> if the object was already a <see cref = "BigDecimal.NaN" /> before assigning zero to the property.</remarks>
        public int Sign
        {
            get
            {
            return _sign;
            }
            set
            {
                if ( ! IsSpecialValue )
                {
                    if ( value < 0 )
                    {
                        if ( IsZero )
                        {
                        this = NegativeZero;
                        }
                        else
                        {
                            if ( Sign > 0 )
                            {
                                _sign = - 1;
                                _mantissa = - BigInteger.Abs ( Mantissa );
                            }
                        }
                    }
                    else if ( value > 0 )
                    {
                        if ( Sign < 0 )
                        {
                            _sign = 1;
                            _mantissa = BigInteger.Abs ( Mantissa );
                        }
                    }
                    else if ( value == 0 )
                    {
                        _sign = 0;
                        _mantissa = 0;
                    }
                }
                else
                {
                    if ( value < 0 )
                    {
                        if ( IsNaN )
                        {
                        throw new InvalidOperationException ( "Cannot assign a sign to a value that is not a number (NaN)." );
                        }
                        if ( IsPositiveInfinity )
                        {
                        this = NegativeInfinity;
                        }
                        //if ( this.IsPositiveZero )
                        //{
                        //    this = NegativeZero;
                        //}
                    }
                    if ( value > 0 )
                    {
                        if ( IsNaN )
                        {
                        throw new InvalidOperationException ( "Cannot assign a sign to a value that is not a number (NaN)." );
                        }
                        if ( IsNegativeInfinity )
                        {
                        this = PositiveInfinity;
                        }
                        if ( IsNegativeZero )
                        {
                        this = Zero;
                        }
                    }
                    if ( value == 0 )
                    {
                        if ( ! IsNaN )
                        {
                        this = Zero;
                        }
                    }
                }
            }
        }
        /// <summary>Gets or sets the context (settings) for the current <see cref = "BigDecimal" /> struct.</summary>
        /// <value>The context (settings) for the current <see cref = "BigDecimal" /> struct.</value>
        /// <remarks>Changing the context from a normal context to a special context will also change the <see cref = "BigDecimal" /> to the correct value. Changing from a normal context to another will result in immediate application of the new context.</remarks>
        public BigDecimalContext CurrentContext
        {
            get
            {
            return _currentContext;
            }
            set
            {
                if ( ! value.Equals ( _currentContext ) )
                {
                    if ( ! value.IsSpecialValue )
                    {
                        _currentContext = value;
                        Round( );
                    }
                    else if ( value.IsPositiveInfinity )
                    {
                    this = PositiveInfinity;
                    }
                    else if ( value.IsNegativeInfinity )
                    {
                    this = NegativeInfinity;
                    }
                    else if ( value.IsNegativeZero )
                    {
                    this = NegativeZero;
                    }
                    else if ( value.IsNaN )
                    {
                    this = NaN;
                    }
                }
            }
        }
        /// <summary>Indicates whether the value of the current <see cref = "BigDecimal" /> object is <see cref = "One" />.</summary>
        /// <value><c>true</c> if the value of the <see cref = "BigDecimal" /> object is <see cref = "One" />; otherwise, <c>false</c>.</value>
        public bool IsOne
        {
            get
            {
            return Equals ( One );
            }
        }
        /// <summary>Indicates whether the value of the current <see cref = "BigDecimal" /> object is <see cref = "BigDecimal.Zero" />.</summary>
        /// <value><c>true</c> if the value of the <see cref = "BigDecimal" /> object is <see cref = "Zero" />; otherwise, <c>false</c>.</value>
        public bool IsZero
        {
            get
            {
            return Equals ( Zero );
            }
        }
        /// <summary>Gets whether the current <see cref = "BigDecimal" /> is not a normal value.</summary>
        /// <value><c>true</c> if the value of the <see cref = "BigDecimal" /> object is either Positive Infinity or Negative Infinity or Positive Zero or Negative Zero or Not-a-Number, otherwise <c>false</c>.</value>
        public bool IsSpecialValue
        {
            get
            {
                _isSpecialValue = IsPositiveInfinity || IsNegativeInfinity || /*this.IsPositiveZero ||*/ IsNegativeZero || IsNaN;
                return _isSpecialValue;
            }
        }
        /// <summary>Gets whether the current <see cref = "BigDecimal" /> is an integer.</summary>
        /// <value><c>true</c> if the value of the current <see cref = "BigDecimal" /> is an integer, otherwise, <c>false</c>.</value>
        public bool IsInteger
        {
            get
            {
                if ( IsSpecialValue )
                {
                _isInteger = /*this.IsPositiveZero ||*/ IsNegativeZero;
                }
                else
                {
                _isInteger = Exponent >= 0;
                }
                return _isInteger;
            }
        }
        /// <summary>Gets whether the current <see cref = "BigDecimal" /> is even.</summary>
        /// <value><c>true</c> if the value of the current <see cref = "BigDecimal" /> is even, otherwise, <c>false</c>.</value>
        public bool IsEven
        {
            get
            {
                if ( IsSpecialValue )
                {
                _isEven = /*this.IsPositiveZero ||*/ IsNegativeZero;
                }
                else
                {
                    if ( ! IsInteger )
                    {
                    _isEven = false;
                    }
                    else
                    {
                        if ( Exponent == 0 )
                        {
                        _isEven = Mantissa.LastDigit( ) % 2 == 0;
                        }
                        else
                        {
                        _isEven = true;
                        }
                    }
                }
                return _isEven;
            }
        }
        /// <summary>Gets whether the current <see cref = "BigDecimal" /> is odd.</summary>
        /// <value><c>true</c> if the value of the current <see cref = "BigDecimal" /> is odd, otherwise, <c>false</c>.</value>
        public bool IsOdd
        {
            get
            {
                if ( IsSpecialValue )
                {
                _isOdd = false;
                }
                else
                {
                    if ( ! IsInteger )
                    {
                    _isOdd = false;
                    }
                    else
                    {
                        if ( Exponent == 0 )
                        {
                        _isOdd = Mantissa.LastDigit( ) % 2 != 0;
                        }
                        else
                        {
                        _isOdd = false;
                        }
                    }
                }
                return _isOdd;
            }
        }
        /// <summary>Indicates whether the value of the current <see cref = "BigDecimal" /> object is <see cref = "BigDecimal.PositiveInfinity" />.</summary>
        /// <value><c>true</c> if the value of the <see cref = "BigDecimal" /> object is <see cref = "PositiveInfinity" />; otherwise, <c>false</c>.</value>
        public bool IsPositiveInfinity
        {
            get
            {
            return Mantissa == BigInteger.Zero && Exponent == BigInteger.Zero & Sign > 0 && CurrentContext.Equals ( PositiveInfinity.CurrentContext );
            }
        }
        /// <summary>Indicates whether the value of the current <see cref = "BigDecimal" /> object is <see cref = "NegativeInfinity" />.</summary>
        /// <value><c>true</c> if the value of the <see cref = "BigDecimal" /> object is <see cref = "NegativeInfinity" />; otherwise, <c>false</c>.</value>
        public bool IsNegativeInfinity
        {
            get
            {
            return Mantissa == BigInteger.Zero && Exponent == BigInteger.Zero & Sign < 0 && CurrentContext.Equals ( NegativeInfinity.CurrentContext );
            }
        }
        /// <summary>Indicates whether the value of the current <see cref = "BigDecimal" /> object is <see cref = "NegativeZero" />.</summary>
        /// <value><c>true</c> if the value of the <see cref = "BigDecimal" /> object is <see cref = "NegativeZero" />; otherwise, <c>false</c>.</value>
        public bool IsNegativeZero
        {
            get
            {
            return Mantissa == BigInteger.Zero && Exponent == BigInteger.Zero & Sign < 0 && CurrentContext.Equals ( NegativeZero.CurrentContext );
            }
        }
        /// <summary>Indicates whether the value of the current <see cref = "BigDecimal" /> object is <see cref = "NaN" />.</summary>
        /// <value><c>true</c> if the value of the <see cref = "BigDecimal" /> object is <see cref = "NaN" />; otherwise, <c>false</c>.</value>
        public bool IsNaN
        {
            get
            {
            return Mantissa == BigInteger.Zero && Exponent == BigInteger.Zero & Sign == 0 && CurrentContext.Equals ( NaN.CurrentContext );
            }
        }
        #endregion
        #region ConstructorDestructor
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure using a <paramref name = "mantissa" />, an <paramref name = "exponent" /> and a <paramref name = "context" />.</summary>
        /// <exception cref = "ArgumentException" >Thrown when <paramref name = "context" /> is of unknown value.</exception>
        /// <param name = "mantissa" >The mantissa of the number.</param>
        /// <param name = "exponent" >The exponent of the number.</param>
        /// <param name = "context" >The context of the number.</param>
        public BigDecimal ( BigInteger mantissa , BigInteger exponent , BigDecimalContext context )
            : this( )
        {
            CurrentContext = context;
            Mantissa = mantissa;
            Exponent = exponent;
            if ( ! CurrentContext.IsSpecialValue )
            {
                Normalize( );
                Round( );
            }
            else
            {
                Mantissa = BigInteger.Zero;
                Exponent = BigInteger.Zero;
                switch ( context.ValueType )
                {
                    case BigDecimalValues.PositiveInfinity :
                        this = PositiveInfinity;
                        break;
                    case BigDecimalValues.NegativeInfinity :
                        this = NegativeInfinity;
                        break;
                    //case BigDecimalValues.PositiveZero :
                    //    this = PositiveZero;
                    //    break;
                    case BigDecimalValues.NegativeZero :
                        this = NegativeZero;
                        break;
                    case BigDecimalValues.NaN :
                        this = NaN;
                        break;
                    default :
                        throw new ArgumentException ( "Argument is of unknown value" , nameof ( context ) );
                }
            }
        }
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure using a <paramref name = "mantissa" />, an <paramref name = "exponent" /> and a <paramref name = "precision" />.</summary>
        /// <param name = "mantissa" >The mantissa of the number.</param>
        /// <param name = "exponent" >The exponent of the number.</param>
        /// <param name = "precision" >The required precision of the number.</param>
        public BigDecimal ( BigInteger mantissa , BigInteger exponent , BigInteger precision )
            : this ( mantissa , exponent , new BigDecimalContext ( precision ) )
        {}
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure using a <paramref name = "mantissa" /> and an <paramref name = "exponent" />.</summary>
        /// <param name = "mantissa" >The mantissa of the number.</param>
        /// <param name = "exponent" >The exponent of the number.</param>
        public BigDecimal ( BigInteger mantissa , BigInteger exponent )
            : this ( mantissa , exponent , BigDecimalContext.DefaultContext )
        {}
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure using an 8-bit signed integer value.</summary>
        /// <param name = "value" >An 8-bit signed integer value.</param>
        public BigDecimal ( sbyte value )
            : this ( value , 0 )
        {}
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure using an 8-bit unsigned integer value.</summary>
        /// <param name = "value" >An 8-bit unsigned integer value.</param>
        public BigDecimal ( byte value )
            : this ( value , 0 )
        {}
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure using a 16-bit unicode character.</summary>
        /// <param name = "value" >A 16-bit unicode character.</param>
        [ Obsolete ( "This constructor should not be used since the char represents a textual and not a numeric value." , false ) ]
        public BigDecimal ( char value )
            : this ( value , 0 )
        {}
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure using a 16-bit signed integer value.</summary>
        /// <param name = "value" >A 16-bit signed integer value.</param>
        public BigDecimal ( short value )
            : this ( value , 0 )
        {}
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure using a 16-bit unsigned integer value.</summary>
        /// <param name = "value" >A 16-bit unsigned integer value.</param>
        public BigDecimal ( ushort value )
            : this ( value , 0 )
        {}
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure using a 32-bit signed integer value.</summary>
        /// <param name = "value" >A 32-bit signed integer value.</param>
        public BigDecimal ( int value )
            : this ( value , 0 )
        {}
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure using an unsigned 32-bit integer value.</summary>
        /// <param name = "value" >An unsigned 32-bit integer value.</param>
        public BigDecimal ( uint value )
            : this ( value , 0 )
        {}
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure using a 64-bit signed integer value.</summary>
        /// <param name = "value" >A 64-bit signed integer value.</param>
        public BigDecimal ( long value )
            : this ( value , 0 )
        {}
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure with a 64-bit unsigned integer value.</summary>
        /// <param name = "value" >A 64-bit unsigned integer value.</param>
        public BigDecimal ( ulong value )
            : this ( value , 0 )
        {}
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure using a single-precision floating-point value.</summary>
        /// <param name = "value" >A single-precision floating-point value.</param>
        public BigDecimal ( float value )
            : this( )
        {
        this = value;
        }
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure using a double-precision floating-point value.</summary>
        /// <param name = "value" >A double-precision floating-point value.</param>
        public BigDecimal ( double value )
            : this( )
        {
        this = value;
        }
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure using a <see cref = "System.Decimal" /> value.</summary>
        /// <param name = "value" >A <see cref = "System.Decimal" /> value.</param>
        public BigDecimal ( decimal value )
            : this( )
        {
        this = value;
        }
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure using a <see cref = "BigInteger" /> value.</summary>
        /// <param name = "value" >A <see cref = "BigInteger" /> value.</param>
        public BigDecimal ( BigInteger value )
            : this ( value , 0 )
        {}
        /// <summary>Initializes a new instance of the <see cref = "BigDecimal" /> structure using a <see cref = "System.String" /> value.</summary>
        /// <param name = "value" >A <see cref = "System.String" /> value representing a number.</param>
        public BigDecimal ( string value )
            : this( )
        {
        TryParse ( value , out this );
        }
        #endregion
        #region IComparable Members
        /// <summary>Compares this instance to a specified object and returns an integer that indicates whether the value of this instance is less than, equal to, or greater than the value of the specified object.</summary>
        /// <exception cref = "ArgumentNullException" >Thrown when <paramref name = "obj" /> is null.</exception>
        /// <exception cref = "ArgumentException" >Thrown when <paramref name = "obj" /> is not of the correct type.</exception>
        /// <param name = "obj" >The object to compare.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings:
        ///     <list type = "table" >
        ///         <listheader>
        ///             <term>Value</term>
        ///             <term>Meaning</term>
        ///         </listheader>
        ///         <item>
        ///             <term>Less than zero</term>
        ///             <term>This instance precedes <paramref name = "obj" /> in the sort order.</term>
        ///         </item>
        ///         <item>
        ///             <term>Zero</term>
        ///             <term>This instance occurs in the same position in the sort order as <paramref name = "obj" />.</term>
        ///         </item>
        ///         <item>
        ///             <term>Greater than zero</term>
        ///             <term>This instance follows <paramref name = "obj" /> in the sort order.</term>
        ///         </item>
        ///     </list></returns>
        /// <seealso cref = "IComparable.CompareTo(object)" />
        int IComparable.CompareTo ( object obj )
        {
            if ( obj == null )
            {
            throw new ArgumentNullException ( nameof ( obj ) , "Argument is null." );
            }
            if ( ! ( obj is BigDecimal ) )
            {
            throw new ArgumentException ( "Argument is not of type BigDecimal." , nameof ( obj ) );
            }
            return CompareTo ( ( BigDecimal ) obj );
        }
        #endregion
        #region IComparable<BigDecimal> Members
        /// <summary>Compares this instance to a second <see cref = "BigDecimal" /> and returns an integer that indicates whether the value of this instance is less than, equal to, or greater than the value of the specified <see cref = "BigDecimal" />.</summary>
        /// <param name = "other" cref = "BigDecimal" >The <see cref = "BigDecimal" /> to compare.</param>
        /// <returns>A value that indicates the relative order of the
        ///     <see cref = "BigDecimal" />s being compared. The return value has these meanings:
        ///     <list type = "table" >
        ///         <listheader>
        ///             <term>Value</term>
        ///             <term>Meaning</term>
        ///         </listheader>
        ///         <item>
        ///             <term>Less than zero</term>
        ///             <term>This <see cref = "BigDecimal" /> precedes <paramref name = "other" /> in the sort order.</term>
        ///         </item>
        ///         <item>
        ///             <term>Zero</term>
        ///             <term>This <see cref = "BigDecimal" /> occurs in the same position in the sort order as <paramref name = "other" />.</term>
        ///         </item>
        ///         <item>
        ///             <term>Greater than zero</term>
        ///             <term>This <see cref = "BigDecimal" /> follows <paramref name = "other" /> in the sort order.</term>
        ///         </item>
        ///     </list></returns>
        /// <seealso cref = "BigDecimal.CompareTo(BigDecimal)" />
        int IComparable < BigDecimal >.CompareTo ( BigDecimal other )
        {
        return CompareTo ( other );
        }
        #endregion
        #region IEquatable<BigDecimal> Members
        /// <summary>Returns a value that indicates whether the current <see cref = "BigDecimal" /> and a specified <see cref = "BigDecimal" /> object have the same value.</summary>
        /// <param name = "other" cref = "BigDecimal" >The <see cref = "BigDecimal" /> to compare.</param>
        /// <returns><c>true</c> if the current <see cref = "BigDecimal" /> is equal to the <paramref name = "other" /> parameter; otherwise, <c>false</c>.</returns>
        /// <seealso cref = "BigDecimal.Equals(BigDecimal)" />
        bool IEquatable < BigDecimal >.Equals ( BigDecimal other )
        {
        return Equals ( other );
        }
        #endregion
        #region IFormattable Members
        /// <summary>Formats the value of the current instance using the specified format.</summary>
        /// <param name = "format" >The format to use.</param>
        /// <param name = "formatProvider" >The provider to use to format the value.</param>
        /// <returns>The value of the current instance in the specified format.</returns>
        /// <seealso cref = "IFormattable.ToString(string,IFormatProvider)" />
        string IFormattable.ToString ( string format , IFormatProvider formatProvider )
        {
            //TODO: Support "Standard Numeric Format" and "Custom Numeric Format" and the "IFormatProvider".
            //TODO: Support special values.
            return ToString ( format , formatProvider );
        }
        #endregion
        #region StaticMethods
        /// <summary>Compares two <see cref = "BigDecimal" /> values and returns an integer that indicates whether the first value is less than, equal to, or greater than the second value.</summary>
        /// <param name = "left" >Left <see cref = "BigDecimal" />The left value.</param>
        /// <param name = "right" >Right <see cref = "BigDecimal" />The right value.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name = "left" /> and <paramref name = "right" />.</returns>
        public static int Compare ( BigDecimal left , BigDecimal right )
        {
        return left.CompareTo ( right );
        }
        /// <summary>Adds two <see cref = "BigDecimal" /> values and returns the result.</summary>
        /// <param name = "left" >Left <see cref = "BigDecimal" />The left value to add.</param>
        /// <param name = "right" >Right <see cref = "BigDecimal" />The right value to add.</param>
        /// <returns>The sum of <paramref name = "left" /> and <paramref name = "right" />.</returns>
        public static BigDecimal Add ( BigDecimal left , BigDecimal right )
        {
        return left + right;
        }
        /// <summary>Subtracts one <see cref = "BigDecimal" /> value from another and returns the result.</summary>
        /// <param name = "left" >Left <see cref = "BigDecimal" />The left value to subtract from.</param>
        /// <param name = "right" >Right <see cref = "BigDecimal" />The right value to subtract.</param>
        /// <returns>The result of subtracting <paramref name = "right" /> from <paramref name = "left" />.</returns>
        public static BigDecimal Subtract ( BigDecimal left , BigDecimal right )
        {
        return left - right;
        }
        /// <summary>Returns the product of two <see cref = "BigDecimal" /> values.</summary>
        /// <param name = "left" >Left <see cref = "BigDecimal" />The left value to multiply.</param>
        /// <param name = "right" >Right <see cref = "BigDecimal" />The right value to multiply.</param>
        /// <returns>The product of the <paramref name = "left" /> and <paramref name = "right" /> parameters.</returns>
        public static BigDecimal Multiply ( BigDecimal left , BigDecimal right )
        {
        return left * right;
        }
        /// <summary>Divides one <see cref = "BigDecimal" /> value by another and returns the result.</summary>
        /// <param name = "dividend" >The <see cref = "BigDecimal" /> to divide.</param>
        /// <param name = "divisor" >The <see cref = "BigDecimal" /> to divide by.</param>
        /// <returns>The value of the division.</returns>
        public static BigDecimal Divide ( BigDecimal dividend , BigDecimal divisor )
        {
        return dividend / divisor;
        }
        /// <summary>Negates a specified <see cref = "BigDecimal" /> value.</summary>
        /// <param name = "value" >The <see cref = "BigDecimal" /> to negate.</param>
        /// <returns>The result of the <paramref name = "value" /> parameter multiplied by negative one (-1).</returns>
        public static BigDecimal Negate ( BigDecimal value )
        {
        return - value;
        }
        /// <summary>Returns e raised to the specified <paramref name = "power" />.</summary>
        /// <param name = "power" >The <paramref name = "power" /> to raise the <see cref = "BigDecimal" /> to.</param>
        /// <returns>The number e raised to the <paramref name = "power" />.</returns>
        public static BigDecimal Exp ( BigDecimal power )
        {
            if ( ! power.IsSpecialValue )
            {
                if ( power.IsZero )
                {
                return One;
                }
                if ( power < 0 )
                {
                return 1 / Exp ( Abs ( power ) );
                }
                BigDecimal endResult = 0;
                BigInteger i = 1;
                //BigDecimal [ ] powerArr = CalcHelper.ZeroToInfXPowerNArr ( power , MaxN );
                if ( MaxN <= 0 )
                {
                throw new ArgumentOutOfRangeException ( nameof ( MaxN ) , MaxN , "Powers must not be a negative number or zero." );
                }
                if ( MaxN == 1 )
                {
                endResult = One;
                }
                BigInteger denomenator = 0;
                BigDecimal neumerator = 0;
                while ( i <= MaxN )
                {
                    if ( i == 1 )
                    {
                        neumerator = 1;
                        denomenator = 1;
                    }
                    else
                    {
                        neumerator *= power;
                        denomenator *= i - 1;
                    }
                    endResult += neumerator / denomenator;
                    i++;
                }
                //BigInteger [ ] factorialArr = CalcHelper.ZeroToInfFactorialArray ( MaxN );
                //BigDecimal [ ] result = CalcHelper.LineMatrixDiv ( powerArr , factorialArr );
                //return CalcHelper.LineMatrixElementSum ( result );
                return endResult;
            }
            if ( power.IsNaN )
            {
                if ( power.CurrentContext.IsSignalingNaN )
                {
                throw new ArithmeticException ( "Cannot perform arithmatic operations on or with a value that is NaN." );
                }
                return NaN;
            }
            if ( /*power.IsPositiveZero ||*/
            power.IsNegativeZero )
            {
            return One;
            }
            if ( power.IsPositiveInfinity )
            {
            return PositiveInfinity;
            }
            if ( power.IsNegativeInfinity )
            {
            return Zero;
            }
            throw new ArgumentException ( "Argument is of unrecognized value." , nameof ( power ) );
        }
        /// <summary>Raises a BigDecimal value to the power of a specified value.</summary>
        /// <param name = "value" >The value to be raised to a power.</param>
        /// <param name = "exponent" >The power to be raised to.</param>
        /// <returns>The result of raising <paramref name = "value" /> to the <paramref name = "exponent" /> power.</returns>
        public static BigDecimal Pow ( BigDecimal value , BigInteger exponent )
        {
            //TODO: Update to support special values.
            if ( value.IsZero )
            {
                if ( exponent.IsZero )
                {
                    if ( value.CurrentContext.IsSignalingNaN )
                    {
                    throw new ArithmeticException ( "Cannot perform arithmatic operations on or with a value that is NaN." );
                    }
                    return NaN;
                }
                if ( exponent < 0 )
                {
                return PositiveInfinity;
                }
                if ( exponent > 0 )
                {
                return Zero;
                }
            }
            if ( value.IsOne )
            {
            return One;
            }
            if ( value.IsPositiveInfinity )
            {
                if ( exponent < 0 )
                {
                return Zero;
                }
                if ( exponent.IsZero )
                {
                    if ( value.CurrentContext.IsSignalingNaN )
                    {
                    throw new ArithmeticException ( "Cannot perform arithmatic operations on or with a value that is NaN." );
                    }
                    return NaN;
                }
                if ( exponent > 0 )
                {
                return PositiveInfinity;
                }
            }
            if ( value.IsNegativeInfinity )
            {
                if ( exponent.IsEven && exponent.Sign < 1 )
                {
                return Zero;
                }
                if ( ! exponent.IsEven && exponent.Sign < 1 )
                {
                return NegativeZero;
                }
                if ( exponent.IsEven && exponent.Sign > 1 )
                {
                return PositiveInfinity;
                }
                if ( ! exponent.IsEven && exponent.Sign > 1 )
                {
                return NegativeInfinity;
                }
                if ( exponent.IsZero )
                {
                    if ( value.CurrentContext.IsSignalingNaN )
                    {
                    throw new ArithmeticException ( "Cannot perform arithmatic operations on or with a value that is NaN." );
                    }
                    return NaN;
                }
            }
            if ( exponent.IsZero )
            {
            return One;
            }
            if ( exponent > 0 )
            {
                if ( value > 0 || value < 0 && exponent.IsEven )
                {
                    BigDecimal result = value;
                    for ( BigInteger i = 0; i < exponent - 1; i++ )
                    {
                    result *= value;
                    }
                    return result;
                }
                if ( value < 0 && ! exponent.IsEven )
                {
                return - Pow ( Abs ( value ) , exponent );
                }
            }
            if ( exponent < 0 )
            {
            return 1 / Pow ( value , exponent );
            }
            if ( value.CurrentContext.IsSignalingNaN )
            {
            throw new ArithmeticException ( "Cannot perform arithmatic operations on or with a value that is NaN." );
            }
            return NaN;
        }
        /// <summary>Raises a BigDecimal value to the power of a specified value.</summary>
        /// <param name = "value" >The value to be raised to a power.</param>
        /// <param name = "exponent" >The power to be raised to.</param>
        /// <returns>The result of raising value to the exponent power.</returns>
        public static BigDecimal Pow ( BigDecimal value , BigDecimal exponent )
        {
        return Exp ( exponent * Log ( value ) );
        }
        /// <summary>Returns the natural (base e) logarithm of a specified number.</summary>
        /// <exception cref = "ArgumentException" >Thrown when one or more arguments have unsupported or illegal values.</exception>
        /// <exception cref = "System.ArgumentException" >.</exception>
        /// <param name = "value" >The value to calculate the logarithm of.</param>
        /// <returns>The natural (base e) logarithm of value, as shown in the table in the Remarks section.</returns>
        public static BigDecimal Log ( BigDecimal value )
        {
            //Not updated to support Special Values.
            if ( ! value.IsSpecialValue )
            {
                if ( value < 0 )
                {
                return NaN;
                }
                if ( value.IsZero )
                {
                return NegativeInfinity;
                }
                if ( value > 0 && value < One )
                {
                return - Log ( One / value );
                }
                if ( value.IsOne )
                {
                return Zero;
                }
                BigDecimal endResult = 0;
                BigInteger i = 1;
                BigDecimal innerFraction = ( value - One ) / ( value + One );
                BigDecimal currentPowerFraction = 0;
                BigDecimal maxNDouble = 2 * MaxN;
                if ( MaxN <= 0 )
                {
                throw new ArgumentOutOfRangeException ( nameof ( MaxN ) , MaxN , "Powers must not be a negative number or zero." );
                }
                if ( MaxN == 1 )
                {
                return ( value - One ) / ( value + One ) * 2;
                }
                while ( i <= maxNDouble )
                {
                    if ( i == 1 )
                    {
                    currentPowerFraction = innerFraction;
                    }
                    else
                    {
                    currentPowerFraction *= innerFraction * innerFraction;
                    }
                    endResult += One / i * currentPowerFraction;
                    i += 2;
                }
                return endResult * 2;
                //BigDecimal [ ] result;
                //result = CalcHelper.LineMatrixMult ( CalcHelper.OneToInfOddXMinusOneDivXPlusOnePowerN ( value , MaxN ) , CalcHelper.OneToInfOddOneDivN ( MaxN ) );
                //for ( int i = 0; i < result.Length; i++ )
                //{
                //result [ i ] *= 2;
                //}
                //return CalcHelper.LineMatrixElementSum ( result );
            }
            if ( value.IsNaN )
            {
            return NaN;
            }
            //if ( value.IsPositiveZero )
            //{
            //    return NegativeInfinity;
            //}
            if ( value.IsNegativeZero )
            {
            return NaN;
            }
            if ( value.IsPositiveInfinity )
            {
            return PositiveInfinity;
            }
            if ( value.IsNegativeInfinity )
            {
                //or NaN
                return PositiveInfinity;
            }
            throw new ArgumentException ( "Argument is of unrecognized value." , nameof ( value ) );
        }
        /// <summary>Returns the logarithm of a specified number in a specified base.</summary>
        /// <exception cref = "System.ArgumentOutOfRangeException" >.</exception>
        /// <param name = "value" >The value to calculate the logarithm of.</param>
        /// <param name = "baseValue" >The base to calculate the logarithm to.</param>
        /// <returns>The base <paramref name = "baseValue" /> logarithm of <pramref name = "value" />.</returns>
        public static BigDecimal Log ( BigDecimal value , BigDecimal baseValue )
        {
        return Log ( value ) / Log ( baseValue );
        }
        /// <summary>Returns the base 10 logarithm of a specified number.</summary>
        /// <exception cref = "System.ArgumentOutOfRangeException" >.</exception>
        /// <exception cref = "System.ArgumentException" >.</exception>
        /// <param name = "value" >The value to calculate the logarithm of.</param>
        /// <returns>The base 10 logarithm of <paramref name = "value" ></paramref>.</returns>
        public static BigDecimal Log10 ( BigDecimal value )
        {
        return Log ( value ) / Log ( 10 );
        }
        /// <summary>Returns the larger of two <see cref = "BigInteger" ></see> values.</summary>
        /// <param name = "left" >Left <see cref = "BigDecimal" />.</param>
        /// <param name = "right" >Right <see cref = "BigDecimal" />.</param>
        /// <returns>The <paramref name = "left" ></paramref> or <paramref name = "right" ></paramref> parameter, whichever is larger.</returns>
        public static BigDecimal Max ( BigDecimal left , BigDecimal right )
        {
            if ( left > right )
            {
            return left;
            }
            return right;
        }
        /// <summary>Returns the smaller of two <see cref = "BigInteger" ></see> values.</summary>
        /// <param name = "left" >Left <see cref = "BigDecimal" />.</param>
        /// <param name = "right" >Right <see cref = "BigDecimal" />.</param>
        /// <returns>The <paramref name = "left" /> or <paramref name = "right" /> parameter, whichever is smaller.</returns>
        public static BigDecimal Min ( BigDecimal left , BigDecimal right )
        {
            if ( left < right )
            {
            return left;
            }
            return right;
        }
        /// <summary>Gets the absolute value of a <see cref = "BigDecimal" /> object.</summary>
        /// <param name = "value" >A <see cref = "BigDecimal" />.</param>
        /// <returns>The absolute value of the <see cref = "BigDecimal" />.</returns>
        public static BigDecimal Abs ( BigDecimal value )
        {
            if ( value.Sign < 0 )
            {
                BigDecimal temp = value;
                temp.Sign = 1;
                return temp;
            }
            return value;
        }
        //TODO: Deprecate and implement the IFormatProvider and the NumberStyles format.
        /// <summary>Specifics the parse pattern.</summary>
        /// <param name = "numStyles" >The styles permitted in numeric string arguments of the integral and floating-point numeric types.</param>
        /// <param name = "numberOrCultureFormatInfo" >Culture-specific information to parse a number.</param>
        /// <returns>A Regex the follows the <paramref name = "numStyles" /> and the <paramref name = "numberOrCultureFormatInfo" />.</returns>
        private static Regex SpecificParsePattern ( NumberStyles numStyles , IFormatProvider numberOrCultureFormatInfo )
        {
            NumberFormatInfo format = NumberFormatInfo.GetInstance ( numberOrCultureFormatInfo );
            string saintNegativeSign = RegexBuilder.RegexSanitize ( format.NegativeSign );
            string saintPositiveSign = RegexBuilder.RegexSanitize ( format.PositiveSign );
            string saintNumberGroupSeparator = RegexBuilder.RegexSanitize ( format.NumberGroupSeparator );
            string saintNumberDecimalSeparator = RegexBuilder.RegexSanitize ( format.NumberDecimalSeparator );
            string [ ] saintDigits = RegexBuilder.RegexSanitize ( format.NativeDigits );
            StringBuilder saintDigitsString = new StringBuilder( );
            saintDigitsString.Append ( "(?:" );
            for ( int i = 0; i < saintDigits.Length; i++ )
            {
                if ( i == saintDigits.Length - 1 )
                {
                saintDigitsString.Append ( saintDigits [ i ] );
                }
                else
                {
                saintDigitsString.Append ( saintDigits [ i ] + "|" );
                }
            }
            saintDigitsString.Append ( ")" );
            StringBuilder sb = new StringBuilder( );
            sb.Append ( @"(?ims:^(?<LeadingSpaces>\s*)(?<NumberSign>(?:" );
            sb.Append ( saintNegativeSign + @"|" + saintPositiveSign );
            sb.Append ( @"|\s)*(?:" );
            sb.Append ( saintNegativeSign + @"|" + saintPositiveSign );
            sb.Append ( @"))?(?<SignSpaces>\s*)(?<LeadingRedundantZeros>(?:" + saintDigits [ 0 ] + @"|" );
            sb.Append ( saintNumberGroupSeparator );
            sb.Append ( @")*)(?<BeforeDecimal>(?:" + saintDigitsString + @"|" );
            sb.Append ( saintNumberGroupSeparator );
            sb.Append ( @")*)(?:" );
            sb.Append ( saintNumberDecimalSeparator );
            sb.Append ( @"(?<AfterDecimal>" + saintDigitsString + @"*?)(?<TrailingRedundantZeros>(?<!" + saintDigits [ 0 ] + @")(?:" + saintDigits [ 0 ] + @")*))?(?<NumberSpaces>\s*)(?:[Ee](?<ESpaces>\s*)(?<ExponentSign>(?:" );
            sb.Append ( saintNegativeSign + @"|" + saintPositiveSign );
            sb.Append ( @"|\s)*(?:" );
            sb.Append ( saintNegativeSign + @"|" + saintPositiveSign );
            sb.Append ( @"))?(?<EponentSignSpaces>\s*)(?<ExponentRedundantZeros>(?:" + saintDigits [ 0 ] + @"|" );
            sb.Append ( saintNumberGroupSeparator );
            sb.Append ( @")*)(?<ExponentNumber>(?:" + saintDigitsString + @"|" );
            sb.Append ( saintNumberGroupSeparator );
            sb.Append ( @")*))?(?<TrailingSpaces>\s*)$)" );
            return new Regex ( sb.ToString( ) );
        }
        /// <summary>Tries to convert the string representation of a number to its BigDecimal equivalent, and returns a value that indicates whether the conversion succeeded.</summary>
        /// <exception cref = "ArgumentException" >.</exception>
        /// <param name = "number" >The string to attempt to parse.</param>
        /// <param name = "parsedBigDecimal" >The parsed <see cref = "BigDecimal" />.</param>
        /// <returns><c>true</c> if value was converted successfully; otherwise, <c>false</c>.</returns>
        public static bool TryParse ( string number , out BigDecimal parsedBigDecimal )
        {
            if ( RelaxedParsePattern.IsMatch ( number ) )
            {
                parsedBigDecimal = Parse ( number );
                return true;
            }
            parsedBigDecimal = Zero;
            return false;
        }
        /// <summary>Converts the string representation of a number to its BigDecimal equivalent.</summary>
        /// <exception cref = "ArgumentException" >.</exception>
        /// <param name = "number" >The string to parse.</param>
        /// <returns>A value that is equivalent to the number specified in the <paramref name = "number" /> parameter.</returns>
        public static BigDecimal Parse ( string number )
        {
        return Parse ( number , BigDecimalContext.DefaultContext );
        }
        /// <summary>Converts the string representation of a number to its BigDecimal equivalent.</summary>
        /// <exception cref = "ArgumentException" >.</exception>
        /// <param name = "number" >The string to parse.</param>
        /// <param name = "context" cref = "BigDecimal" >The context to parse the number into.</param>
        /// <returns>A value that is equivalent to the number specified in the <paramref name = "number" /> parameter.</returns>
        public static BigDecimal Parse ( string number , BigDecimalContext context )
        {
            if ( RelaxedParsePattern.IsMatch ( number ) )
            {
                Match numberMatch = RelaxedParsePattern.Match ( number );
                string sign = FilterSign ( numberMatch.Groups [ "NumberSign" ].Value );
                string beforeDecimal = FilterInteger ( numberMatch.Groups [ "BeforeDecimal" ].Value );
                string afterDecimal = FilterInteger ( numberMatch.Groups [ "AfterDecimal" ].Value );
                string exponentSign = FilterSign ( numberMatch.Groups [ "ExponentSign" ].Value );
                string exponentNum = FilterInteger ( numberMatch.Groups [ "ExponentNumber" ].Value );
                return RadixNormalize ( beforeDecimal , afterDecimal , sign , exponentSign , exponentNum , context );
            }
            //throw new ArgumentException ( "Argument is not in the correct format" , "number" );
            return Zero;
        }
        /// <summary>Converts the string representation of a number in a specified style to its <see cref = "BigInteger" /> equivalent.</summary>
        /// <exception cref = "System.NotImplementedException" >.</exception>
        /// <param name = "value" >The string to parse.</param>
        /// <param name = "style" >The style by which the number is formated.</param>
        /// <returns>A value that is equivalent to the number specified in the value parameter.</returns>
        public static BigDecimal Parse ( string value , NumberStyles style )
        {
        throw new NotImplementedException( );
        }
        /// <summary>Converts the string representation of a number in a specified culture-specific format to its BigInteger equivalent.</summary>
        /// <exception cref = "System.NotImplementedException" >.</exception>
        /// <param name = "value" >The string to parse.</param>
        /// <param name = "provider" >The format by which the number is formated.</param>
        /// <returns>A value that is equivalent to the number specified in the <pram name = "value" /> parameter.</returns>
        public static BigDecimal Parse ( string value , IFormatProvider provider )
        {
        throw new NotImplementedException( );
        }
        /// <summary>Converts the string representation of a number in a specified style and culture-specific format to its BigInteger equivalent.</summary>
        /// <exception cref = "System.NotImplementedException" >.</exception>
        /// <param name = "value" >The string to parse.</param>
        /// <param name = "style" >The style by which the number is formated.</param>
        /// <param name = "provider" >The format by which the number is formated.</param>
        /// <returns>A value that is equivalent to the number specified in the value parameter.</returns>
        public static BigDecimal Parse ( string value , NumberStyles style , IFormatProvider provider )
        {
            NumberFormatInfo format = NumberFormatInfo.GetInstance ( provider );
            throw new NotImplementedException( );
        }
        /// <summary>Radix normalize.</summary>
        /// <param name = "beforeDecimal" >The before decimal.</param>
        /// <param name = "afterDecimal" >The after decimal.</param>
        /// <param name = "sign" >A number that indicates the sign of the <see cref = "BigDecimal" />
        ///     object, as shown in the following table:
        ///     <list type = "table" >
        ///         <listheader>
        ///             <term>Number</term>
        ///             <term>Description</term>
        ///         </listheader>
        ///         <item>
        ///             <term>-1</term>
        ///             <term>The value of this <see cref = "BigDecimal" />
        ///                 object is negative.</term>
        ///         </item>
        ///         <item>
        ///             <term>0</term>
        ///             <term>The value of this <see cref = "BigDecimal" />
        ///                 object is 0 (zero) or NaN (Not-a-Number).</term>
        ///         </item>
        ///         <item>
        ///             <term>1</term>
        ///             <term>The value of this <see cref = "BigDecimal" />
        ///                 object is positive.</term>
        ///         </item>
        ///     </list></param>
        /// <param name = "exponentSign" >The exponent sign.</param>
        /// <param name = "exponentNum" >The exponent number.</param>
        /// <param name = "context" cref = "BigDecimal" >.</param>
        /// <returns>A BigDecimal.</returns>
        private static BigDecimal RadixNormalize ( string beforeDecimal , string afterDecimal , string sign , string exponentSign , string exponentNum , BigDecimalContext context )
        {
            BigInteger mantissa;
            BigInteger exponent;
            if ( beforeDecimal != string.Empty && afterDecimal != string.Empty )
            {
                mantissa = BigInteger.Parse ( sign + beforeDecimal + afterDecimal );
                if ( exponentNum == string.Empty )
                {
                exponent = - afterDecimal.Length;
                }
                else
                {
                exponent = BigInteger.Parse ( exponentSign + exponentNum ) - afterDecimal.Length;
                }
                return new BigDecimal ( mantissa , exponent , context );
            }
            if ( beforeDecimal == string.Empty && afterDecimal == string.Empty )
            {
            return Zero;
            }
            if ( beforeDecimal != string.Empty && afterDecimal == string.Empty )
            {
                string zerosRemoved = beforeDecimal.TrimEnd ( '0' );
                mantissa = BigInteger.Parse ( sign + zerosRemoved );
                if ( exponentNum == string.Empty )
                {
                exponent = beforeDecimal.Length - zerosRemoved.Length;
                }
                else
                {
                exponent = BigInteger.Parse ( exponentSign + exponentNum ) + ( beforeDecimal.Length - zerosRemoved.Length );
                }
                return new BigDecimal ( mantissa , exponent , context );
            }
            if ( beforeDecimal == string.Empty && afterDecimal != string.Empty )
            {
                string zerosRemoved = afterDecimal.TrimStart ( '0' );
                mantissa = BigInteger.Parse ( sign + zerosRemoved );
                if ( exponentNum == string.Empty )
                {
                exponent = - afterDecimal.Length;
                }
                else
                {
                exponent = BigInteger.Parse ( exponentSign + exponentNum ) - afterDecimal.Length;
                }
                return new BigDecimal ( mantissa , exponent , context );
            }
            return Zero;
        }
        /// <summary>Filter integer.</summary>
        /// <param name = "toBeFiltered" >to be filtered.</param>
        /// <returns>A string.</returns>
        private static string FilterInteger ( string toBeFiltered )
        {
            string finalString = toBeFiltered;
            if ( finalString == string.Empty )
            {
            return string.Empty;
            }
            if ( toBeFiltered.Contains ( "," ) )
            {
                finalString = finalString.Replace ( "," , string.Empty );
                if ( finalString == string.Empty )
                {
                return string.Empty;
                }
            }
            return finalString;
        }
        /// <summary>Filter sign.</summary>
        /// <param name = "toBeFiltered" >to be filtered.</param>
        /// <returns>A string.</returns>
        private static string FilterSign ( string toBeFiltered )
        {
            string finalString = toBeFiltered;
            if ( finalString == string.Empty )
            {
            return string.Empty;
            }
            if ( finalString.Contains ( " " ) || finalString.Contains ( "\t" ) )
            {
                finalString = finalString.Replace ( " " , "" ).Replace ( "\t" , "" );
                if ( finalString == string.Empty )
                {
                return string.Empty;
                }
            }
            if ( finalString.Contains ( "-" ) )
            {
                int minusCount = finalString.Count ( ch => ch == '-' );
                if ( minusCount % 2 == 0 )
                {
                return string.Empty;
                }
                if ( minusCount % 2 != 0 )
                {
                return "-";
                }
            }
            return string.Empty;
        }
        /// <summary>Align the exponent of the <paramref name = "value" /> to the exponent of <paramref name = "reference" />.</summary>
        /// <exception cref = "ArgumentOutOfRangeException" >Thrown when one or more arguments are outside the required range.</exception>
        /// <param name = "value" >The <see cref = "BigDecimal" /> to align.</param>
        /// <param name = "reference" >The <see cref = "BigDecimal" /> to align to.</param>
        /// <returns>A BigInteger.</returns>
        private static BigInteger AlignExponent ( BigDecimal value , BigDecimal reference )
        {
            if ( value.Exponent - reference.Exponent > 0 )
            {
            return value.Mantissa * CalcHelper.Pow ( 10 , value.Exponent - reference.Exponent );
            }
            throw new ArgumentOutOfRangeException ( nameof ( reference ) , "reference's exponent is higher than value's exponent." );
        }
        /// <summary>Gets the higher presision of two <see cref = "BigDecimal" />.</summary>
        /// <param name = "first" >The first <see cref = "BigDecimal" />.</param>
        /// <param name = "second" >The second <see cref = "BigDecimal" />.</param>
        /// <returns>The higher presision of the two <see cref = "BigDecimal" />.</returns>
        private static BigInteger MaxPrecision ( BigDecimal first , BigDecimal second )
        {
        return second.CurrentContext.Precision > first.CurrentContext.Precision ? second.CurrentContext.Precision : first.CurrentContext.Precision;
        }
        /// <summary>Implicit conversion from <see cref = "System.SByte" /> type to <see cref = "BigDecimal" />.</summary>
        /// <param name = "value" cref = "System.SByte" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static implicit operator BigDecimal ( sbyte value )
        {
        return new BigDecimal ( value , 0 );
        }
        /// <summary>Implicit conversion from <see cref = "System.Byte" /> type to <see cref = "BigDecimal" />.</summary>
        /// <param name = "value" cref = "System.Byte" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static implicit operator BigDecimal ( byte value )
        {
        return new BigDecimal ( value , 0 );
        }
        /// <summary>Implicit conversion from <see cref = "System.Int16" /> type to <see cref = "BigDecimal" />.</summary>
        /// <param name = "value" cref = "System.Int16" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static implicit operator BigDecimal ( short value )
        {
        return new BigDecimal ( value , 0 );
        }
        /// <summary>Implicit conversion from <see cref = "System.UInt16" /> type to <see cref = "BigDecimal" />.</summary>
        /// <param name = "value" cref = "System.UInt16" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static implicit operator BigDecimal ( ushort value )
        {
        return new BigDecimal ( value , 0 );
        }
        /// <summary>Implicit conversion from <see cref = "System.Int32" /> type to <see cref = "BigDecimal" />.</summary>
        /// <param name = "value" cref = "System.Int32" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static implicit operator BigDecimal ( int value )
        {
        return new BigDecimal ( value , 0 );
        }
        /// <summary>Implicit conversion from <see cref = "System.UInt32" /> type to <see cref = "BigDecimal" />.</summary>
        /// <param name = "value" cref = "System.UInt32" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static implicit operator BigDecimal ( uint value )
        {
        return new BigDecimal ( value , 0 );
        }
        /// <summary>Implicit conversion from <see cref = "System.Int64" /> type to <see cref = "BigDecimal" />.</summary>
        /// <param name = "value" cref = "System.Int64" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static implicit operator BigDecimal ( long value )
        {
        return new BigDecimal ( value , 0 );
        }
        /// <summary>Implicit conversion from <see cref = "System.UInt64" /> type to <see cref = "BigDecimal" />.</summary>
        /// <param name = "value" cref = "System.UInt64" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static implicit operator BigDecimal ( ulong value )
        {
        return new BigDecimal ( value , 0 );
        }
        /// <summary>Implicit conversion from <see cref = "System.Single" /> type to <see cref = "BigDecimal" />.</summary>
        /// <param name = "value" cref = "System.Single" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static implicit operator BigDecimal ( float value )
        {
            if ( float.IsInfinity ( value ) )
            {
                if ( float.IsPositiveInfinity ( value ) )
                {
                return PositiveInfinity;
                }
                if ( float.IsNegativeInfinity ( value ) )
                {
                return NegativeInfinity;
                }
            }
            BigInteger mantissa = ( BigInteger ) value;
            int exponent = 0;
            float scaleFactor = 1;
            while ( Math.Abs ( value * scaleFactor - ( float ) mantissa ) > 0 )
            {
                exponent--;
                scaleFactor *= 10;
                mantissa = ( BigInteger ) ( value * scaleFactor );
            }
            return new BigDecimal ( mantissa , exponent );
        }
        /// <summary>Implicit conversion from <see cref = "System.Double" /> type to <see cref = "BigDecimal" />.</summary>
        /// <param name = "value" cref = "System.Double" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static implicit operator BigDecimal ( double value )
        {
            if ( double.IsInfinity ( value ) )
            {
                if ( double.IsPositiveInfinity ( value ) )
                {
                return PositiveInfinity;
                }
                if ( double.IsNegativeInfinity ( value ) )
                {
                return NegativeInfinity;
                }
            }
            BigInteger mantissa = ( BigInteger ) value;
            int exponent = 0;
            double scaleFactor = 1;
            while ( Math.Abs ( value * scaleFactor - ( double ) mantissa ) > 0 )
            {
                exponent--;
                scaleFactor *= 10;
                mantissa = ( BigInteger ) ( value * scaleFactor );
            }
            return new BigDecimal ( mantissa , exponent );
        }
        /// <summary>Implicit conversion from <see cref = "System.Decimal" /> type to <see cref = "BigDecimal" />.</summary>
        /// <param name = "value" cref = "System.Decimal" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static implicit operator BigDecimal ( decimal value )
        {
            BigInteger mantissa = ( BigInteger ) value;
            int exponent = 0;
            decimal scaleFactor = 1;
            //while ( ( decimal ) mantissa != value * scaleFactor )
            while ( Math.Abs ( value * scaleFactor - ( decimal ) mantissa ) > 0 )
            {
                exponent--;
                scaleFactor *= 10;
                mantissa = ( BigInteger ) ( value * scaleFactor );
            }
            return new BigDecimal ( mantissa , exponent );
        }
        /// <summary>Implicit conversion from <see cref = "BigInteger" /> type to <see cref = "BigDecimal" />.</summary>
        /// <param name = "value" cref = "BigInteger" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static implicit operator BigDecimal ( BigInteger value )
        {
        return new BigDecimal ( value , 0 );
        }
        /// <summary>Explicit conversion from <see cref = "System.SByte" /> type to <see cref = "BigDecimal" />.</summary>
        /// <exception cref = "OverflowException" >Thrown when an arithmetic overflow occurs.</exception>
        /// <exception cref = "System.OverflowException" >.</exception>
        /// <param name = "value" cref = "System.SByte" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static explicit operator sbyte ( BigDecimal value )
        {
            if ( value > sbyte.MaxValue || value < sbyte.MinValue )
            {
            throw new OverflowException ( "This value cannot be converted to a SByte type." );
            }
            return ( sbyte ) ( value.Mantissa * CalcHelper.Pow ( 10 , value.Exponent ) );
        }
        /// <summary>Explicit conversion from <see cref = "System.Byte" /> type to <see cref = "BigDecimal" />.</summary>
        /// <exception cref = "OverflowException" >Thrown when an arithmetic overflow occurs.</exception>
        /// <exception cref = "System.OverflowException" >.</exception>
        /// <param name = "value" cref = "System.Byte" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static explicit operator byte ( BigDecimal value )
        {
            if ( value > byte.MaxValue || value < byte.MinValue )
            {
            throw new OverflowException ( "This value cannot be converted to a Byte type." );
            }
            return ( byte ) ( value.Mantissa * CalcHelper.Pow ( 10 , value.Exponent ) );
        }
        /// <summary>Explicit conversion from <see cref = "System.Int16" /> type to <see cref = "BigDecimal" />.</summary>
        /// <exception cref = "OverflowException" >Thrown when an arithmetic overflow occurs.</exception>
        /// <exception cref = "System.OverflowException" >.</exception>
        /// <param name = "value" cref = "System.Int16" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static explicit operator short ( BigDecimal value )
        {
            if ( value > short.MaxValue || value < short.MinValue )
            {
            throw new OverflowException ( "This value cannot be converted to a Int16 type." );
            }
            return ( short ) ( value.Mantissa * CalcHelper.Pow ( 10 , value.Exponent ) );
        }
        /// <summary>Explicit conversion from <see cref = "System.UInt16" /> type to <see cref = "BigDecimal" />.</summary>
        /// <exception cref = "OverflowException" >Thrown when an arithmetic overflow occurs.</exception>
        /// <exception cref = "System.OverflowException" >.</exception>
        /// <param name = "value" cref = "System.UInt16" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static explicit operator ushort ( BigDecimal value )
        {
            if ( value > ushort.MaxValue || value < ushort.MinValue )
            {
            throw new OverflowException ( "This value cannot be converted to a UInt16 type." );
            }
            return ( ushort ) ( value.Mantissa * CalcHelper.Pow ( 10 , value.Exponent ) );
        }
        /// <summary>Explicit conversion from <see cref = "System.Int32" /> type to <see cref = "BigDecimal" />.</summary>
        /// <exception cref = "OverflowException" >Thrown when an arithmetic overflow occurs.</exception>
        /// <exception cref = "System.OverflowException" >.</exception>
        /// <param name = "value" cref = "System.Int32" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static explicit operator int ( BigDecimal value )
        {
            if ( value > int.MaxValue || value < int.MinValue )
            {
            throw new OverflowException ( "This value cannot be converted to a Int32 type." );
            }
            return ( int ) ( value.Mantissa * CalcHelper.Pow ( 10 , value.Exponent ) );
        }
        /// <summary>Explicit conversion from <see cref = "System.UInt32" /> type to <see cref = "BigDecimal" />.</summary>
        /// <exception cref = "OverflowException" >Thrown when an arithmetic overflow occurs.</exception>
        /// <exception cref = "System.OverflowException" >.</exception>
        /// <param name = "value" cref = "System.UInt32" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static explicit operator uint ( BigDecimal value )
        {
            if ( value > uint.MaxValue || value < uint.MinValue )
            {
            throw new OverflowException ( "This value cannot be converted to a UInt32 type." );
            }
            return ( uint ) ( value.Mantissa * CalcHelper.Pow ( 10 , value.Exponent ) );
        }
        /// <summary>Explicit conversion from <see cref = "System.Int64" /> type to <see cref = "BigDecimal" />.</summary>
        /// <exception cref = "OverflowException" >Thrown when an arithmetic overflow occurs.</exception>
        /// <exception cref = "System.OverflowException" >.</exception>
        /// <param name = "value" cref = "System.Int64" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static explicit operator long ( BigDecimal value )
        {
            if ( value > long.MaxValue || value < long.MinValue )
            {
            throw new OverflowException ( "This value cannot be converted to a Int64 type." );
            }
            return ( long ) ( value.Mantissa * CalcHelper.Pow ( 10 , value.Exponent ) );
        }
        /// <summary>Explicit conversion from <see cref = "System.UInt64" /> type to <see cref = "BigDecimal" />.</summary>
        /// <exception cref = "OverflowException" >Thrown when an arithmetic overflow occurs.</exception>
        /// <exception cref = "System.OverflowException" >.</exception>
        /// <param name = "value" cref = "System.UInt64" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static explicit operator ulong ( BigDecimal value )
        {
            if ( value > ulong.MaxValue || value < ulong.MinValue )
            {
            throw new OverflowException ( "This value cannot be converted to a UInt64 type." );
            }
            return ( ulong ) ( value.Mantissa * CalcHelper.Pow ( 10 , value.Exponent ) );
        }
        /// <summary>Explicit conversion from <see cref = "System.Single" /> type to <see cref = "BigDecimal" />.</summary>
        /// <exception cref = "OverflowException" >Thrown when an arithmetic overflow occurs.</exception>
        /// <exception cref = "System.OverflowException" >.</exception>
        /// <param name = "value" cref = "System.Single" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static explicit operator float ( BigDecimal value )
        {
            if ( value > float.MaxValue || value < float.MinValue )
            {
            throw new OverflowException ( "This value cannot be converted to a Single type." );
            }
            return Convert.ToSingle ( ( double ) value );
        }
        /// <summary>Explicit conversion from <see cref = "System.Double" /> type to <see cref = "BigDecimal" />.</summary>
        /// <exception cref = "OverflowException" >Thrown when an arithmetic overflow occurs.</exception>
        /// <exception cref = "System.OverflowException" >.</exception>
        /// <param name = "value" cref = "System.Double" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static explicit operator double ( BigDecimal value )
        {
            if ( value > double.MaxValue || value < double.MinValue )
            {
            throw new OverflowException ( "This value cannot be converted to a Double type." );
            }
            return ( double ) ( value.Mantissa * CalcHelper.Pow ( 10 , value.Exponent ) );
        }
        /// <summary>Explicit conversion from <see cref = "System.Decimal" /> type to <see cref = "BigDecimal" />.</summary>
        /// <exception cref = "OverflowException" >Thrown when an arithmetic overflow occurs.</exception>
        /// <exception cref = "System.OverflowException" >.</exception>
        /// <param name = "value" cref = "System.Decimal" >value to be converted.</param>
        /// <returns>The converted <see cref = "BigDecimal" />.</returns>
        public static explicit operator decimal ( BigDecimal value )
        {
            if ( value > decimal.MaxValue || value < decimal.MinValue )
            {
            throw new OverflowException ( "This value cannot be converted to a Decimal type." );
            }
            return ( decimal ) ( value.Mantissa * CalcHelper.Pow ( 10 , value.Exponent ) );
        }
        /// <summary>Returns the value of the <see cref = "BigDecimal" /> operand. (The sign of the operand is unchanged.)</summary>
        /// <param name = "value" >.</param>
        /// <returns>The value of the <paramref name = "value" /> operand.</returns>
        public static BigDecimal operator + ( BigDecimal value )
        {
        return value;
        }
        /// <summary>Negates a specified <see cref = "BigDecimal" /> value.</summary>
        /// <param name = "value" >to negate.</param>
        /// <returns>The result of the <paramref name = "value" /> parameter multiplied by negative one (-1).</returns>
        public static BigDecimal operator - ( BigDecimal value )
        {
            // Updated to support special values.
            // Copying not referencing. 
            BigDecimal temp = value;
            if ( ! temp.IsSpecialValue )
            {
                if ( temp.Mantissa.Sign > 0 || temp.Mantissa.Sign < 0 )
                {
                temp.Mantissa = BigInteger.Negate ( temp.Mantissa );
                }
                if ( temp.Mantissa.Sign == 0 )
                {
                return NegativeZero;
                }
                return temp;
            }
            if ( temp.Sign < 0 )
            {
                temp.Sign = 1;
                return temp;
            }
            temp.Sign = - 1;
            return temp;
        }
        /// <summary>Increments a <see cref = "BigDecimal" /> value by 1.</summary>
        /// <param name = "value" >to increment.</param>
        /// <returns>The value of the <paramref name = "value" /> parameter incremented by 1.</returns>
        public static BigDecimal operator ++ ( BigDecimal value )
        {
        return value + One;
        }
        /// <summary>Decrements a <see cref = "BigDecimal" /> value by 1.</summary>
        /// <param name = "value" >to decrement.</param>
        /// <returns>The value of the <paramref name = "value" /> parameter decremented by 1.</returns>
        public static BigDecimal operator -- ( BigDecimal value )
        {
        return value - One;
        }
        /// <summary>Adds the values of two specified <see cref = "BigDecimal" /> objects.</summary>
        /// <exception cref = "ArgumentException" >Thrown when one or more arguments have unsupported or illegal values.</exception>
        /// <exception cref = "System.InvalidOperationException" >.</exception>
        /// <exception cref = "System.ArgumentException" >.</exception>
        /// <param name = "left" >Left <see cref = "BigDecimal" />.</param>
        /// <param name = "right" >Right <see cref = "BigDecimal" />.</param>
        /// <returns>The sum of <paramref name = "left" /> and <paramref name = "right" />.</returns>
        public static BigDecimal operator + ( BigDecimal left , BigDecimal right )
        {
            //Updated to support special values
            if ( ! left.IsSpecialValue && ! right.IsSpecialValue )
            {
                if ( left.Exponent > right.Exponent )
                {
                return new BigDecimal ( AlignExponent ( left , right ) + right.Mantissa , right.Exponent , MaxPrecision ( left , right ) );
                }
                if ( right.Exponent > left.Exponent )
                {
                return new BigDecimal ( AlignExponent ( right , left ) + left.Mantissa , left.Exponent , MaxPrecision ( left , right ) );
                }
                return new BigDecimal ( left.Mantissa + right.Mantissa , right.Exponent , MaxPrecision ( left , right ) );
            }
            if ( left.IsSpecialValue && ! right.IsSpecialValue )
            {
                if ( left.IsNaN )
                {
                return NaN;
                }
                if ( left.IsPositiveInfinity )
                {
                return PositiveInfinity;
                }
                if ( left.IsNegativeInfinity )
                {
                return NegativeInfinity;
                }
                if ( /*left.IsPositiveZero ||*/
                left.IsNegativeZero )
                {
                return right;
                }
            }
            if ( ! left.IsSpecialValue && right.IsSpecialValue )
            {
                if ( right.IsNaN )
                {
                return NaN;
                }
                if ( right.IsPositiveInfinity )
                {
                return PositiveInfinity;
                }
                if ( right.IsNegativeInfinity )
                {
                return NegativeInfinity;
                }
                if ( /*right.IsPositiveZero ||*/
                right.IsNegativeZero )
                {
                return left;
                }
            }
            if ( left.IsSpecialValue && right.IsSpecialValue )
            {
                if ( left.IsNaN || right.IsNaN )
                {
                return NaN;
                }
                if ( left.IsPositiveInfinity && right.IsNegativeInfinity || left.IsNegativeInfinity && right.IsPositiveInfinity )
                {
                    return NaN;
                    //throw new InvalidOperationException ( "Cannot add +∞ and -∞." );
                }
                if ( left.IsPositiveInfinity || right.IsPositiveInfinity )
                {
                return PositiveInfinity;
                }
                if ( left.IsNegativeInfinity || right.IsNegativeInfinity )
                {
                return NegativeInfinity;
                }
            }
            throw new ArgumentException( );
        }
        /// <summary>Subtracts a BigDecimal value from another <see cref = "BigDecimal" /> value.</summary>
        /// <param name = "left" >Left <see cref = "BigDecimal" />.</param>
        /// <param name = "right" >Right <see cref = "BigDecimal" />.</param>
        /// <returns>The result of subtracting <paramref name = "right" /> from <paramref name = "left" />.</returns>
        public static BigDecimal operator - ( BigDecimal left , BigDecimal right )
        {
        return left + - right;
        }
        /// <summary>Multiplies two specified <see cref = "BigDecimal" /> values.</summary>
        /// <exception cref = "ArgumentException" >Thrown when one or more arguments have unsupported or illegal values.</exception>
        /// <exception cref = "System.ArgumentException" >.</exception>
        /// <param name = "left" >Left <see cref = "BigDecimal" />.</param>
        /// <param name = "right" >Right <see cref = "BigDecimal" />.</param>
        /// <returns>The product of <paramref name = "left" /> and <paramref name = "right" />.</returns>
        public static BigDecimal operator * ( BigDecimal left , BigDecimal right )
        {
            if ( ! left.IsSpecialValue && ! right.IsSpecialValue )
            {
            return new BigDecimal ( left.Mantissa * right.Mantissa , left.Exponent + right.Exponent , MaxPrecision ( left , right ) );
            }
            if ( ! left.IsSpecialValue && right.IsSpecialValue )
            {
                if ( right.IsPositiveInfinity )
                {
                    if ( left.Sign < 0 )
                    {
                    return NegativeInfinity;
                    }
                    if ( left.Sign > 0 )
                    {
                    return PositiveInfinity;
                    }
                    if ( left.IsZero )
                    {
                        return NaN;
                        //throw new InvalidOperationException ( "Cannot multiply 0 by Infinity." );
                    }
                }
                if ( right.IsNegativeInfinity )
                {
                    if ( left.Sign < 0 )
                    {
                    return PositiveInfinity;
                    }
                    if ( left.Sign > 0 )
                    {
                    return NegativeInfinity;
                    }
                    if ( left.IsZero )
                    {
                        //throw new InvalidOperationException ( "Cannot multiply 0 by Infinity." );
                        return NaN;
                    }
                }
                if ( /*right.IsPositiveZero ||*/
                right.IsNegativeZero )
                {
                return Zero;
                }
            }
            if ( left.IsSpecialValue && ! right.IsSpecialValue )
            {
                if ( left.IsPositiveInfinity )
                {
                    if ( right.Sign < 0 )
                    {
                    return NegativeInfinity;
                    }
                    if ( right.Sign > 0 )
                    {
                    return PositiveInfinity;
                    }
                    if ( right.IsZero )
                    {
                        //throw new InvalidOperationException ( "Cannot multiply 0 by Infinity." );
                        return NaN;
                    }
                }
                if ( left.IsNegativeInfinity )
                {
                    if ( right.Sign < 0 )
                    {
                    return PositiveInfinity;
                    }
                    if ( right.Sign > 0 )
                    {
                    return NegativeInfinity;
                    }
                    if ( right.IsZero )
                    {
                        //throw new InvalidOperationException ( "Cannot multiply 0 by Infinity." );
                        return NaN;
                    }
                }
                if ( /*left.IsPositiveZero ||*/
                left.IsNegativeZero )
                {
                return Zero;
                }
            }
            if ( left.IsSpecialValue && right.IsSpecialValue )
            {
                if ( left.IsPositiveInfinity && right.IsPositiveInfinity || left.IsNegativeInfinity && right.IsNegativeInfinity )
                {
                return PositiveInfinity;
                }
                if ( left.IsPositiveInfinity && right.IsNegativeInfinity || left.IsNegativeInfinity && right.IsPositiveInfinity )
                {
                return NegativeInfinity;
                }
                if ( left.IsNegativeZero /*)*/&& ( right.IsPositiveInfinity || right.IsNegativeInfinity ) || right.IsNegativeZero /*)*/&& ( left.IsPositiveInfinity || left.IsNegativeInfinity ) )
                {
                    //throw new InvalidOperationException ( "Cannot multiply 0 by Infinity." );
                    return NaN;
                }
            }
            throw new ArgumentException( );
        }
        /// <summary>Divides a specified <see cref = "BigDecimal" /> value by another specified <see cref = "BigDecimal" /> value.</summary>
        /// <exception cref = "ArgumentException" >Thrown when one or more arguments have unsupported or illegal values.</exception>
        /// <exception cref = "System.InvalidOperationException" >.</exception>
        /// <exception cref = "System.ArgumentException" >.</exception>
        /// <exception cref = "System.DivideByZeroException" name = "divisor" >is 0 (zero).</exception>
        /// <param name = "dividend" >.</param>
        /// <param name = "divisor" >.</param>
        /// <returns>The integral result of the division.</returns>
        public static BigDecimal operator / ( BigDecimal dividend , BigDecimal divisor )
        {
            if ( ! dividend.IsSpecialValue && ! divisor.IsSpecialValue )
            {
                if ( divisor.IsZero )
                {
                    if ( dividend.Sign < 0 )
                    {
                    return NegativeInfinity;
                    }
                    if ( dividend.Sign > 0 )
                    {
                    return PositiveInfinity;
                    }
                    if ( dividend.IsZero )
                    {
                        //throw new InvalidOperationException ( "Cannot divide 0 by 0." );
                        return NaN;
                    }
                }
                BigInteger prec = MaxPrecision ( dividend , divisor );
                BigInteger exponentChange = prec - ( CalcHelper.NumberOfDigits ( dividend.Mantissa ) - CalcHelper.NumberOfDigits ( divisor.Mantissa ) );
                if ( exponentChange < 0 )
                {
                exponentChange = 0;
                }
                dividend.Mantissa *= CalcHelper.Pow ( 10 , exponentChange );
                return new BigDecimal ( dividend.Mantissa / divisor.Mantissa , dividend.Exponent - divisor.Exponent - exponentChange , prec );
            }
            if ( ! dividend.IsSpecialValue && divisor.IsSpecialValue )
            {
                if ( divisor.IsNaN )
                {
                return NaN;
                }
                if ( divisor.IsPositiveInfinity )
                {
                    if ( dividend.Sign < 0 )
                    {
                    return NegativeZero;
                    }
                    if ( dividend.Sign >= 0 )
                    {
                    return Zero;
                    }
                }
                if ( divisor.IsNegativeInfinity )
                {
                    if ( dividend.Sign < 0 )
                    {
                    return Zero;
                    }
                    if ( dividend.Sign >= 0 )
                    {
                    return NegativeZero;
                    }
                }
                //if ( divisor.IsPositiveZero )
                //{
                //    if ( dividend.Sign < 0 )
                //    {
                //        return NegativeInfinity;
                //    }
                //    if ( dividend.Sign >= 0 )
                //    {
                //        return PositiveInfinity;
                //    }
                //}
                if ( divisor.IsNegativeZero )
                {
                    if ( dividend.Sign < 0 )
                    {
                    return PositiveInfinity;
                    }
                    if ( dividend.Sign >= 0 )
                    {
                    return NegativeInfinity;
                    }
                }
            }
            if ( dividend.IsSpecialValue && ! divisor.IsSpecialValue )
            {
                if ( dividend.IsNaN )
                {
                return NaN;
                }
                if ( dividend.IsPositiveInfinity )
                {
                    if ( divisor.Sign < 0 )
                    {
                    return NegativeInfinity;
                    }
                    if ( divisor.Sign >= 0 )
                    {
                    return PositiveInfinity;
                    }
                }
                if ( dividend.IsNegativeInfinity )
                {
                    if ( divisor.Sign < 0 )
                    {
                    return PositiveInfinity;
                    }
                    if ( divisor.Sign >= 0 )
                    {
                    return NegativeInfinity;
                    }
                }
                //if ( dividend.IsPositiveZero )
                //{
                //    if ( divisor.Sign < 0 )
                //    {
                //        return NegativeZero;
                //    }
                //    if ( divisor.Sign > 0 )
                //    {
                //        return PositiveZero;
                //    }
                //    if ( divisor.IsZero )
                //    {
                //        throw new InvalidOperationException ( "Cannot divide +0 by 0." );
                //    }
                //}
                if ( dividend.IsNegativeZero )
                {
                    if ( divisor.Sign < 0 )
                    {
                    return Zero;
                    }
                    if ( divisor.Sign > 0 )
                    {
                    return NegativeZero;
                    }
                    if ( divisor.IsZero )
                    {
                        //throw new InvalidOperationException ( "Cannot divide +0 by 0." );
                        return NaN;
                    }
                }
            }
            if ( dividend.IsSpecialValue && divisor.IsSpecialValue )
            {
                if ( dividend.IsNaN || divisor.IsNaN )
                {
                return NaN;
                }
                if ( dividend.IsPositiveInfinity )
                {
                    if ( divisor.IsPositiveInfinity || divisor.IsNegativeInfinity )
                    {
                        //throw new InvalidOperationException ( "Cannot divide an infinite value by another infinite value." );
                        return NaN;
                    }
                    //if ( divisor.IsPositiveZero )
                    //{
                    //    return PositiveInfinity;
                    //}
                    if ( divisor.IsNegativeZero )
                    {
                    return NegativeInfinity;
                    }
                }
                if ( dividend.IsNegativeInfinity )
                {
                    if ( divisor.IsPositiveInfinity || divisor.IsNegativeInfinity )
                    {
                        //throw new InvalidOperationException ( "Cannot divide an infinite value by another infinite value." );
                        return NaN;
                    }
                    //if ( divisor.IsPositiveZero )
                    //{
                    //    return NegativeInfinity;
                    //}
                    if ( divisor.IsNegativeZero )
                    {
                    return PositiveInfinity;
                    }
                }
                //if ( dividend.IsPositiveZero )
                //{
                //    if ( divisor.IsPositiveInfinity )
                //    {
                //        return PositiveInfinity;
                //    }
                //    if ( divisor.IsNegativeInfinity )
                //    {
                //        return NegativeInfinity;
                //    }
                //    if ( divisor.IsPositiveZero ||
                //         divisor.IsNegativeZero )
                //    {
                //        throw new InvalidOperationException ( "Cannot divide 0 by 0." );
                //    }
                //}
                if ( dividend.IsNegativeZero )
                {
                    if ( divisor.IsPositiveInfinity )
                    {
                    return NegativeInfinity;
                    }
                    if ( divisor.IsNegativeInfinity )
                    {
                    return PositiveInfinity;
                    }
                    if ( /*divisor.IsPositiveZero ||*/
                    divisor.IsNegativeZero )
                    {
                        //throw new InvalidOperationException ( "Cannot divide 0 by 0." );
                        return NaN;
                    }
                }
            }
            throw new ArgumentException( );
        }
        /// <summary>Returns the remainder that results from division with two specified <see cref = "BigDecimal" /> values.</summary>
        /// <exception cref = "InvalidOperationException" >Thrown when the requested operation is invalid.</exception>
        /// <param name = "dividend" >.</param>
        /// <param name = "divisor" >.</param>
        /// <returns>The remainder that results from the division.</returns>
        public static BigInteger operator % ( BigDecimal dividend , BigDecimal divisor )
        {
            if ( dividend.IsInteger && divisor.IsInteger )
            {
            return dividend.Mantissa * CalcHelper.Pow ( 10 , dividend.Exponent ) % ( divisor.Mantissa * CalcHelper.Pow ( 10 , divisor.Exponent ) );
            }
            throw new InvalidOperationException ( "Cannot calculate modulus for non-integer values." );
        }
        /// <summary>Returns a value that indicates whether the values of two BigDecimal objects are equal.</summary>
        /// <param name = "left" >.</param>
        /// <param name = "right" >.</param>
        /// <returns><c>true</c> if the left and right parameters have the same value; otherwise, <c>false</c>.</returns>
        public static bool operator == ( BigDecimal left , BigDecimal right )
        {
            //Updated to support special values
            if ( ! left.IsSpecialValue && ! right.IsSpecialValue )
            {
                right.Normalize( );
                left.Normalize( );
                return right.Mantissa.Equals ( left.Mantissa ) && right.Exponent.Equals ( left.Exponent );
            }
            if ( left.IsSpecialValue && ! right.IsSpecialValue )
            {
            return /*( left.IsPositiveZero ||*/ left.IsNegativeZero /*)*/&& right.IsZero;
            }
            if ( ! left.IsSpecialValue && right.IsSpecialValue )
            {
            return /*( right.IsPositiveZero ||*/ right.IsNegativeZero /*)*/&& left.IsZero;
            }
            if ( left.IsSpecialValue && right.IsSpecialValue )
            {
                //return left.IsNegativeZero /*)*/&& /*( right.IsPositiveZero ||*/ right.IsNegativeZero || left.IsPositiveInfinity && right.IsPositiveInfinity || left.IsNegativeInfinity && right.IsNegativeInfinity || left.IsNaN && right.IsNaN;
                return left.IsNegativeZero && right.IsNegativeZero || left.IsPositiveInfinity && right.IsPositiveInfinity || left.IsNegativeInfinity && right.IsNegativeInfinity || left.IsNaN && right.IsNaN;
            }
            return false;
        }
        /// <summary>Returns a value that indicates whether two BigDecimal objects have different values.</summary>
        /// <param name = "left" >.</param>
        /// <param name = "right" >.</param>
        /// <returns><c>true</c> if the left and right are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator != ( BigDecimal left , BigDecimal right )
        {
        return ! ( left == right );
        }
        /// <summary>Returns a value that indicates whether a BigDecimal value is less than another BigDecimal value.</summary>
        /// <exception cref = "InvalidOperationException" >Thrown when the requested operation is invalid.</exception>
        /// <exception cref = "System.InvalidOperationException" >.</exception>
        /// <param name = "left" >.</param>
        /// <param name = "right" >.</param>
        /// <returns><c>true</c> if left is less than right; otherwise, <c>false</c>.</returns>
        public static bool operator < ( BigDecimal left , BigDecimal right )
        {
            //Updated to support special values.
            if ( ! left.IsSpecialValue && ! right.IsSpecialValue )
            {
                if ( left.Exponent > right.Exponent )
                {
                return AlignExponent ( left , right ) < right.Mantissa;
                }
                if ( right.Exponent > left.Exponent )
                {
                return left.Mantissa < AlignExponent ( right , left );
                }
                return left.Mantissa < right.Mantissa;
            }
            if ( left.IsSpecialValue && ! right.IsSpecialValue )
            {
                if ( left.IsNaN )
                {
                throw new InvalidOperationException ( "Cannot compare a value that is not a number." );
                }
                if ( left.IsPositiveInfinity )
                {
                return false;
                }
                if ( left.IsNegativeInfinity )
                {
                return true;
                }
                if ( /*left.IsPositiveZero ||*/
                left.IsNegativeZero )
                {
                    if ( right > 0 )
                    {
                    return true;
                    }
                    if ( right <= 0 )
                    {
                    return false;
                    }
                }
            }
            if ( ! left.IsSpecialValue && right.IsSpecialValue )
            {
                if ( right.IsNaN )
                {
                throw new InvalidOperationException ( "Cannot compare a value that is not a number." );
                }
                if ( right.IsPositiveInfinity )
                {
                return true;
                }
                if ( right.IsNegativeInfinity )
                {
                return false;
                }
                if ( /*right.IsPositiveZero ||*/
                right.IsNegativeZero )
                {
                    if ( left >= 0 )
                    {
                    return false;
                    }
                    if ( left < 0 )
                    {
                    return true;
                    }
                }
            }
            if ( left.IsSpecialValue && right.IsSpecialValue )
            {
                if ( left.IsNaN || right.IsNaN )
                {
                throw new InvalidOperationException ( "Cannot compare a value that is not a number." );
                }
                if ( left.IsPositiveInfinity )
                {
                return false;
                }
                if ( left.IsNegativeInfinity )
                {
                    if ( right.IsNegativeInfinity )
                    {
                    return false;
                    }
                    return true;
                }
                if ( /*left.IsPositiveZero ||*/
                left.IsNegativeZero )
                {
                    if ( right.IsPositiveInfinity )
                    {
                    return true;
                    }
                    if ( right.IsNegativeZero || /*right.IsPositiveZero ||*/ right.IsNegativeInfinity )
                    {
                    return false;
                    }
                }
                return false;
            }
            return false;
        }
        /// <summary>Returns a value that indicates whether a BigDecimal value is greater than another BigDecimal value.</summary>
        /// <exception cref = "InvalidOperationException" >Thrown when the requested operation is invalid.</exception>
        /// <exception cref = "System.InvalidOperationException" >.</exception>
        /// <param name = "left" >.</param>
        /// <param name = "right" >.</param>
        /// <returns><c>true</c> if left is greater than right; otherwise, <c>false</c>.</returns>
        public static bool operator > ( BigDecimal left , BigDecimal right )
        {
            //Updated to support special values.
            if ( ! left.IsSpecialValue && ! right.IsSpecialValue )
            {
                if ( left.Exponent > right.Exponent )
                {
                return AlignExponent ( left , right ) > right.Mantissa;
                }
                if ( right.Exponent > left.Exponent )
                {
                return left.Mantissa > AlignExponent ( right , left );
                }
                return left.Mantissa > right.Mantissa;
            }
            if ( left.IsSpecialValue && ! right.IsSpecialValue )
            {
                if ( left.IsNaN )
                {
                throw new InvalidOperationException ( "Cannot compare a value that is not a number." );
                }
                if ( left.IsPositiveInfinity )
                {
                return true;
                }
                if ( left.IsNegativeInfinity )
                {
                return false;
                }
                if ( /*left.IsPositiveZero ||*/
                left.IsNegativeZero )
                {
                    if ( right >= 0 )
                    {
                    return false;
                    }
                    if ( right < 0 )
                    {
                    return true;
                    }
                }
            }
            if ( ! left.IsSpecialValue && right.IsSpecialValue )
            {
                if ( right.IsNaN )
                {
                throw new InvalidOperationException ( "Cannot compare a value that is not a number." );
                }
                if ( right.IsPositiveInfinity )
                {
                return false;
                }
                if ( right.IsNegativeInfinity )
                {
                return true;
                }
                if ( /*right.IsPositiveZero ||*/
                right.IsNegativeZero )
                {
                    if ( left > 0 )
                    {
                    return true;
                    }
                    if ( left <= 0 )
                    {
                    return false;
                    }
                }
            }
            if ( left.IsSpecialValue && right.IsSpecialValue )
            {
                if ( left.IsNaN || right.IsNaN )
                {
                throw new InvalidOperationException ( "Cannot compare a value that is not a number." );
                }
                if ( left.IsPositiveInfinity )
                {
                    if ( right.IsPositiveInfinity )
                    {
                    return false;
                    }
                    return true;
                }
                if ( left.IsNegativeInfinity )
                {
                return false;
                }
                if ( /*left.IsPositiveZero ||*/
                left.IsNegativeZero )
                {
                    if ( /*right.IsPositiveZero ||*/
                    right.IsNegativeZero || right.IsPositiveInfinity )
                    {
                    return false;
                    }
                    if ( right.IsNegativeInfinity )
                    {
                    return true;
                    }
                }
                return false;
            }
            return false;
        }
        /// <summary>Returns a value that indicates whether a BigDecimal value is less than or equal to another BigDecimal value.</summary>
        /// <exception cref = "InvalidOperationException" >Thrown when the requested operation is invalid.</exception>
        /// <exception cref = "System.InvalidOperationException" >.</exception>
        /// <param name = "left" >.</param>
        /// <param name = "right" >.</param>
        /// <returns><c>true</c> if left is less than or equal to right; otherwise, <c>false</c>.</returns>
        public static bool operator <= ( BigDecimal left , BigDecimal right )
        {
            if ( ! left.IsSpecialValue && ! right.IsSpecialValue )
            {
                if ( left.Exponent > right.Exponent )
                {
                return AlignExponent ( left , right ) <= right.Mantissa;
                }
                if ( right.Exponent > left.Exponent )
                {
                return left.Mantissa <= AlignExponent ( right , left );
                }
                return left.Mantissa <= right.Mantissa;
            }
            if ( left.IsSpecialValue && ! right.IsSpecialValue )
            {
                if ( left.IsNaN )
                {
                throw new InvalidOperationException ( "Cannot compare a value that is not a number." );
                }
                if ( left.IsPositiveInfinity )
                {
                return false;
                }
                if ( left.IsNegativeInfinity )
                {
                return true;
                }
                if ( /*left.IsPositiveZero ||*/
                left.IsNegativeZero )
                {
                    if ( right >= 0 )
                    {
                    return true;
                    }
                    if ( right < 0 )
                    {
                    return false;
                    }
                }
            }
            if ( ! left.IsSpecialValue && right.IsSpecialValue )
            {
                if ( right.IsNaN )
                {
                throw new InvalidOperationException ( "Cannot compare a value that is not a number." );
                }
                if ( right.IsPositiveInfinity )
                {
                return true;
                }
                if ( right.IsNegativeInfinity )
                {
                return false;
                }
                if ( /*right.IsPositiveZero ||*/
                right.IsNegativeZero )
                {
                    if ( left > 0 )
                    {
                    return false;
                    }
                    if ( left <= 0 )
                    {
                    return true;
                    }
                }
            }
            if ( left.IsSpecialValue && right.IsSpecialValue )
            {
                if ( left.IsNaN || right.IsNaN )
                {
                throw new InvalidOperationException ( "Cannot compare a value that is not a number." );
                }
                if ( left.IsPositiveInfinity )
                {
                    if ( right.IsPositiveInfinity )
                    {
                    return true;
                    }
                    return false;
                }
                if ( left.IsNegativeInfinity )
                {
                return true;
                }
                if ( /*left.IsPositiveZero ||*/
                left.IsNegativeZero )
                {
                    if ( /*right.IsPositiveZero ||*/
                    right.IsNegativeZero || right.IsPositiveInfinity )
                    {
                    return true;
                    }
                    if ( right.IsNegativeInfinity )
                    {
                    return false;
                    }
                }
                return false;
            }
            return false;
        }
        /// <summary>Returns a value that indicates whether a BigDecimal value is greater than or equal to another BigDecimal value.</summary>
        /// <exception cref = "InvalidOperationException" >Thrown when the requested operation is invalid.</exception>
        /// <exception cref = "System.InvalidOperationException" >.</exception>
        /// <param name = "left" >.</param>
        /// <param name = "right" >.</param>
        /// <returns><c>true</c> if left is greater than right; otherwise, <c>false</c>.</returns>
        public static bool operator >= ( BigDecimal left , BigDecimal right )
        {
            if ( ! left.IsSpecialValue && ! right.IsSpecialValue )
            {
                if ( left.Exponent > right.Exponent )
                {
                return AlignExponent ( left , right ) >= right.Mantissa;
                }
                if ( right.Exponent > left.Exponent )
                {
                return left.Mantissa >= AlignExponent ( right , left );
                }
                return left.Mantissa >= right.Mantissa;
            }
            if ( left.IsSpecialValue && ! right.IsSpecialValue )
            {
                if ( left.IsNaN )
                {
                throw new InvalidOperationException ( "Cannot compare a value that is not a number." );
                }
                if ( left.IsPositiveInfinity )
                {
                return true;
                }
                if ( left.IsNegativeInfinity )
                {
                return false;
                }
                if ( /*left.IsPositiveZero ||*/
                left.IsNegativeZero )
                {
                    if ( right > 0 )
                    {
                    return false;
                    }
                    if ( right <= 0 )
                    {
                    return true;
                    }
                }
            }
            if ( ! left.IsSpecialValue && right.IsSpecialValue )
            {
                if ( right.IsNaN )
                {
                throw new InvalidOperationException ( "Cannot compare a value that is not a number." );
                }
                if ( right.IsPositiveInfinity )
                {
                return false;
                }
                if ( right.IsNegativeInfinity )
                {
                return true;
                }
                if ( /*right.IsPositiveZero ||*/
                right.IsNegativeZero )
                {
                    if ( left >= 0 )
                    {
                    return true;
                    }
                    if ( left < 0 )
                    {
                    return false;
                    }
                }
            }
            if ( left.IsSpecialValue && right.IsSpecialValue )
            {
                if ( left.IsNaN || right.IsNaN )
                {
                throw new InvalidOperationException ( "Cannot compare a value that is not a number." );
                }
                if ( left.IsPositiveInfinity )
                {
                return true;
                }
                if ( left.IsNegativeInfinity )
                {
                    if ( right.IsNegativeInfinity )
                    {
                    return true;
                    }
                    return false;
                }
                if ( /*left.IsPositiveZero ||*/
                left.IsNegativeZero )
                {
                    if ( right.IsPositiveInfinity )
                    {
                    return false;
                    }
                    if ( /*right.IsPositiveZero ||*/
                    right.IsNegativeZero || right.IsNegativeInfinity )
                    {
                    return true;
                    }
                }
                return false;
            }
            return false;
        }
        #endregion
        #region NormalMethods
        /// <summary>Removes trailing zeros on the mantissa.</summary>
        private void Normalize( )
        {
            if ( Mantissa.IsZero )
            {
            Exponent = 0;
            }
            else
            {
                BigInteger remainder;
                do
                {
                    BigInteger shortened = BigInteger.DivRem ( Mantissa , 10 , out remainder );
                    if ( remainder == BigInteger.Zero )
                    {
                        Mantissa = shortened;
                        Exponent++;
                    }
                }
                while ( remainder == BigInteger.Zero );
                //string shortenedMantissaSting = Mantissa.ToString( ).TrimEnd ( '0' );
                //Exponent += Mantissa.ToString( ).Length - shortenedMantissaSting.Length;
                //Mantissa = BigInteger.Parse ( shortenedMantissaSting );
            }
        }
        /// <summary>Rounds this <see cref = "BigDecimal" /> number with the current rounding method set or does nothing if <see cref = "BigDecimalContext.AlwaysRound" /> is set to <c>false</c>.</summary>
        public void Round( )
        {
            if ( CurrentContext.AlwaysRound )
            {
                switch ( CurrentContext.RoundingMethod )
                {
                    case BigDecimalRoundingMethod.RoundDown :
                        RoundDown( );
                        break;
                    case BigDecimalRoundingMethod.RoundUp :
                        RoundUp( );
                        break;
                    case BigDecimalRoundingMethod.RoundHalfDown :
                        RoundHalfDown( );
                        break;
                    case BigDecimalRoundingMethod.RoundHalfUp :
                        RoundHalfUp( );
                        break;
                    case BigDecimalRoundingMethod.RoundHalfEven :
                        RoundHalfEven( );
                        break;
                    case BigDecimalRoundingMethod.RoundCeiling :
                        RoundCeiling( );
                        break;
                    case BigDecimalRoundingMethod.RoundFloor :
                        RoundFloor( );
                        break;
                    case BigDecimalRoundingMethod.Round05Up :
                        Round05Up( );
                        break;
                    default :
                        goto case BigDecimalRoundingMethod.RoundDown;
                }
            }
        }
        /// <summary>Round Down the <see cref = "BigDecimal" /> to the given precision by discarding the extra digits. (Truncate)</summary>
        /// <returns>The rounded <see cref = "BigDecimal" /></returns>
        /// <param name = "precision" >.</param>
        public void RoundDown ( BigInteger precision )
        {
            BigInteger initialLength = CalcHelper.NumberOfDigits ( Mantissa );
            BigInteger diff = initialLength - precision;
            if ( diff > 0 )
            {
            DiscardDigits ( diff );
            }
        }
        /// <summary>Round Down the <see cref = "BigDecimal" /> to the set precision by discarding the extra digits. (Truncate)</summary>
        /// <returns>The rounded <see cref = "BigDecimal" /></returns>
        public void RoundDown( )
        {
        RoundDown ( CurrentContext.Precision );
        }
        /// <summary>Round half up.</summary>
        /// <param name = "precision" >.</param>
        public void RoundHalfUp ( BigInteger precision )
        {
            Normalize( );
            BigInteger initialLength = CalcHelper.NumberOfDigits ( Mantissa );
            BigInteger diff = initialLength - precision;
            if ( diff > 0 )
            {
                BigInteger lastDigit = 0;
                DiscardDigits ( diff , ref lastDigit );
                if ( lastDigit >= 5 )
                {
                    if ( Mantissa.Sign < 0 )
                    {
                    Mantissa--;
                    }
                    else
                    {
                    Mantissa++;
                    }
                }
                if ( CalcHelper.NumberOfDigits ( Mantissa ) > precision )
                {
                RoundHalfUp ( precision );
                }
            }
        }
        /// <summary>Round half up.</summary>
        public void RoundHalfUp( )
        {
        RoundHalfUp ( CurrentContext.Precision );
        }
        /// <summary>Round half even.</summary>
        /// <param name = "precision" >.</param>
        public void RoundHalfEven ( BigInteger precision )
        {
            Normalize( );
            BigInteger initialLength = CalcHelper.NumberOfDigits ( Mantissa );
            BigInteger diff = initialLength - precision;
            List < BigInteger > discardedDigits = new List < BigInteger >( );
            if ( diff > 0 )
            {
                BigInteger lastDigit = 0;
                BigDecimal discardedDigitsAsDecimal;
                DiscardDigits ( diff , out discardedDigitsAsDecimal , out lastDigit );
                if ( discardedDigitsAsDecimal > 0.5 )
                {
                    if ( Mantissa.Sign < 0 )
                    {
                    Mantissa--;
                    }
                    else
                    {
                    Mantissa++;
                    }
                }
                if ( discardedDigitsAsDecimal == 0.5 )
                {
                    if ( ! Mantissa.LastDigit( ).IsEven )
                    {
                        if ( Mantissa.Sign < 0 )
                        {
                        Mantissa--;
                        }
                        else
                        {
                        Mantissa++;
                        }
                    }
                }
                if ( CalcHelper.NumberOfDigits ( Mantissa ) > precision )
                {
                RoundHalfEven ( precision );
                }
            }
        }
        /// <summary>Round half even.</summary>
        public void RoundHalfEven( )
        {
        RoundHalfEven ( CurrentContext.Precision );
        }
        /// <summary>Rounds the ceiling.</summary>
        /// <param name = "precision" >.</param>
        public void RoundCeiling ( BigInteger precision )
        {
            Normalize( );
            BigInteger initialLength = CalcHelper.NumberOfDigits ( Mantissa );
            BigInteger diff = initialLength - precision;
            if ( diff > 0 )
            {
                DiscardDigits ( diff );
                if ( Mantissa.Sign >= 0 )
                {
                Mantissa++;
                }
                if ( CalcHelper.NumberOfDigits ( Mantissa ) > precision )
                {
                RoundHalfEven ( precision );
                }
            }
        }
        /// <summary>Rounds the ceiling.</summary>
        public void RoundCeiling( )
        {
        RoundCeiling ( CurrentContext.Precision );
        }
        /// <summary>Rounds the floor.</summary>
        /// <exception cref = "System.NotImplementedException" >.</exception>
        /// <exception cref = "NotImplementedException" >.</exception>
        /// <param name = "precision" >.</param>
        public void RoundFloor ( BigInteger precision )
        {
            Normalize( );
            BigInteger initialLength = CalcHelper.NumberOfDigits ( Mantissa );
            BigInteger diff = initialLength - precision;
            if ( diff > 0 )
            {
                DiscardDigits ( diff );
                if ( Mantissa.Sign < 0 )
                {
                Mantissa--;
                }
                if ( CalcHelper.NumberOfDigits ( Mantissa ) > precision )
                {
                RoundHalfEven ( precision );
                }
            }
        }
        /// <summary>Rounds the floor.</summary>
        /// <exception cref = "System.NotImplementedException" >.</exception>
        /// <exception cref = "NotImplementedException" >.</exception>
        public void RoundFloor( )
        {
        RoundFloor ( CurrentContext.Precision );
        }
        /// <summary>Rounds the half down.</summary>
        /// <exception cref = "System.NotImplementedException" >.</exception>
        /// <exception cref = "NotImplementedException" >.</exception>
        /// <param name = "precision" >.</param>
        public void RoundHalfDown ( BigInteger precision )
        {
            Normalize( );
            BigInteger initialLength = CalcHelper.NumberOfDigits ( Mantissa );
            BigInteger diff = initialLength - precision;
            if ( diff > 0 )
            {
                BigInteger lastDigit = 0;
                DiscardDigits ( diff , ref lastDigit );
                if ( lastDigit > 5 )
                {
                    if ( Mantissa.Sign < 0 )
                    {
                    Mantissa--;
                    }
                    else
                    {
                    Mantissa++;
                    }
                }
                if ( CalcHelper.NumberOfDigits ( Mantissa ) > precision )
                {
                RoundHalfUp ( precision );
                }
            }
        }
        /// <summary>Rounds the half down.</summary>
        /// <exception cref = "System.NotImplementedException" >.</exception>
        /// <exception cref = "NotImplementedException" >.</exception>
        public void RoundHalfDown( )
        {
        RoundHalfDown ( CurrentContext.Precision );
        }
        /// <summary>Rounds up.</summary>
        /// <param name = "precision" >.</param>
        public void RoundUp ( BigInteger precision )
        {
            Normalize( );
            BigInteger initialLength = CalcHelper.NumberOfDigits ( Mantissa );
            BigInteger diff = initialLength - precision;
            if ( diff > 0 )
            {
                BigInteger lastDigit = 0;
                DiscardDigits ( diff , ref lastDigit );
                if ( Mantissa.Sign < 0 )
                {
                Mantissa--;
                }
                else
                {
                Mantissa++;
                }
                if ( CalcHelper.NumberOfDigits ( Mantissa ) > precision )
                {
                RoundHalfUp ( precision );
                }
            }
        }
        /// <summary>Rounds up.</summary>
        public void RoundUp( )
        {
        RoundUp ( CurrentContext.Precision );
        }
        /// <summary>Round away from 0.</summary>
        /// <param name = "precision" >.</param>
        public void Round05Up ( BigInteger precision )
        {
            Normalize( );
            BigInteger initialLength = CalcHelper.NumberOfDigits ( Mantissa );
            BigInteger diff = initialLength - precision;
            if ( diff > 0 )
            {
                BigInteger lastDigit = 0;
                DiscardDigits ( diff , ref lastDigit );
                if ( lastDigit == 0 || lastDigit == 5 )
                {
                    if ( Mantissa.Sign < 0 )
                    {
                    Mantissa--;
                    }
                    else
                    {
                    Mantissa++;
                    }
                }
                if ( CalcHelper.NumberOfDigits ( Mantissa ) > precision )
                {
                RoundHalfUp ( precision );
                }
            }
        }
        /// <summary>Round zero or five away from 0.</summary>
        /// <exception cref = "System.NotImplementedException" >.</exception>
        /// <exception cref = "NotImplementedException" >.</exception>
        public void Round05Up( )
        {
        Round05Up ( CurrentContext.Precision );
        }
        /// <summary>Discard digits.</summary>
        /// <param name = "diff" >The difference.</param>
        private void DiscardDigits ( BigInteger diff )
        {
            for ( int i = 0; i < diff; i++ )
            {
                Mantissa = BigInteger.Divide ( Mantissa , 10 );
                Exponent++;
            }
        }
        /// <summary>Discard digits.</summary>
        /// <param name = "diff" >The difference.</param>
        /// <param name = "lastDigit" >[out] The last digit.</param>
        private void DiscardDigits ( BigInteger diff , ref BigInteger lastDigit )
        {
            for ( int i = 0; i < diff; i++ )
            {
                if ( i == diff - 1 )
                {
                Mantissa = BigInteger.DivRem ( Mantissa , 10 , out lastDigit );
                }
                else
                {
                Mantissa = BigInteger.Divide ( Mantissa , 10 );
                }
                Exponent++;
            }
        }
        /// <summary>Discard digits.</summary>
        /// <param name = "diff" >The difference.</param>
        /// <param name = "discardedDigitsAsDecimal" >[out] The discarded digits as decimal.</param>
        /// <param name = "lastDigit" >[out] The last digit.</param>
        private void DiscardDigits ( BigInteger diff , out BigDecimal discardedDigitsAsDecimal , out BigInteger lastDigit )
        {
            List < BigInteger > discardedDigits = new List < BigInteger >( );
            lastDigit = 0;
            for ( int i = 0; i < diff; i++ )
            {
                Mantissa = BigInteger.DivRem ( Mantissa , 10 , out lastDigit );
                discardedDigits.Insert ( 0 , lastDigit );
                Exponent++;
            }
            discardedDigitsAsDecimal = new BigDecimal ( "0." + string.Join ( "" , discardedDigits ) );
        }
        /// <summary>Compares this instance to a specified object and returns an integer that indicates whether the value of this instance is less than, equal to, or greater than the value of the specified object.</summary>
        /// <exception cref = "ArgumentNullException" >Thrown when one or more required arguments are null.</exception>
        /// <exception cref = "ArgumentException" >Thrown when one or more arguments have unsupported or illegal values.</exception>
        /// <param name = "obj" >is not the same type as this instance.</param>
        /// <returns>A value that indicates the relative order of the
        ///     <see cref = "BigDecimal" />s being compared. The return value has the following meanings:
        ///     <list type = "table" >
        ///         <listheader>
        ///             <term>Value</term>
        ///             <term>Meaning</term>
        ///         </listheader>
        ///         <item>
        ///             <term>Less than zero</term>
        ///             <term>This <see cref = "BigDecimal" /> is less than the <paramref name = "obj" /> parameter.</term>
        ///         </item>
        ///         <item>
        ///             <term>Zero</term>
        ///             <term>This <see cref = "BigDecimal" /> is equal to <paramref name = "obj" />.</term>
        ///         </item>
        ///         <item>
        ///             <term>Greater than zero</term>
        ///             <term>This <see cref = "BigDecimal" /> is greater than <paramref name = "obj" />.</term>
        ///         </item>
        ///     </list></returns>
        /// <seealso cref = "IComparable.CompareTo(object)" />
        public int CompareTo ( object obj )
        {
            if ( ReferenceEquals ( obj , null ) )
            {
            throw new ArgumentNullException ( nameof ( obj ) , "Passed argument is null" );
            }
            if ( ! ( obj is BigDecimal ) )
            {
            throw new ArgumentException ( "Passed argument is not of type System.Numerics.BigDecimal." , nameof ( obj ) );
            }
            return CompareTo ( ( BigDecimal ) obj );
        }
        /// <summary>Compares this instance to a second <see cref = "BigDecimal" /> and returns an integer that indicates whether the value of this instance is less than, equal to, or greater than the value of the specified <see cref = "BigDecimal" />.</summary>
        /// <param name = "other" cref = "BigDecimal" >The <see cref = "BigDecimal" /> to compare.</param>
        /// <returns>A value that indicates the relative order of the
        ///     <see cref = "BigDecimal" />s being compared. The return value has these meanings:
        ///     <list type = "table" >
        ///         <listheader>
        ///             <term>Value</term>
        ///             <term>Meaning</term>
        ///         </listheader>
        ///         <item>
        ///             <term>Less than zero</term>
        ///             <term>This <see cref = "BigDecimal" /> precedes <paramref name = "other" /> in the sort order.</term>
        ///         </item>
        ///         <item>
        ///             <term>Zero</term>
        ///             <term>This <see cref = "BigDecimal" /> occurs in the same position in the sort order as <paramref name = "other" />.</term>
        ///         </item>
        ///         <item>
        ///             <term>Greater than zero</term>
        ///             <term>This <see cref = "BigDecimal" /> follows <paramref name = "other" /> in the sort order.</term>
        ///         </item>
        ///     </list></returns>
        /// <seealso cref = "BigDecimal.CompareTo(BigDecimal)" />
        public int CompareTo ( BigDecimal other )
        {
            if ( this < other )
            {
            return - 1;
            }
            if ( this > other )
            {
            return 1;
            }
            return 0;
        }
        /// <summary>Returns a value that indicates whether the current instance and a specified <see cref = "BigDecimal" /> object have the same value.</summary>
        /// <param name = "other" cref = "BigDecimal" >to compare with this <see cref = "BigDecimal" />.</param>
        /// <returns><c>true</c> if the current <see cref = "BigDecimal" /> is equal to the <paramref name = "other" /> parameter; otherwise, <c>false</c>.</returns>
        /// <seealso cref = "BigDecimal.Equals(BigDecimal)" />
        public bool Equals ( BigDecimal other )
        {
        return this == other;
        }
        /// <summary>Converts the numeric value of the current <see cref = "BigDecimal" /> object to its equivalent string representation.</summary>
        /// <returns>The string representation of the current <see cref = "BigDecimal" /> value.</returns>
        /// <seealso cref = "ValueType.ToString()" />
        public override string ToString( )
        {
        return ToString ( string.Empty , null );
        }
        /// <summary>Converts the numeric value of the current <see cref = "BigDecimal" /> object to its equivalent string representation by using the specified culture-specific formatting information.</summary>
        /// <param name = "formatProvider" >.</param>
        /// <returns>The string representation of the current <see cref = "BigDecimal" /> value in the format specified by the provider parameter.</returns>
        public string ToString ( IFormatProvider formatProvider )
        {
            //TODO: Support "Standard Numeric Format" and "Custom Numeric Format" and the "IFormatProvider".
            //TODO: Support special values.
            return string.Empty;
        }
        /// <summary>Converts the numeric value of the current BigInteger object to its equivalent string representation by using the specified format.</summary>
        /// <param name = "format" >.</param>
        /// <returns>The string representation of the current BigInteger value in the format specified by the format parameter.</returns>
        public string ToString ( string format )
        {
            //TODO: Support "Standard Numeric Format" and "Custom Numeric Format" and the "IFormatProvider".
            //TODO: Support special values.
            return string.Concat ( Mantissa.ToString( ) , "E" , Exponent );
        }
        /// <summary>Converts the numeric value of the current <see cref = "BigDecimal" /> object to its equivalent string representation by using the specified format and culture-specific format information.</summary>
        /// <param name = "format" >.</param>
        /// <param name = "formatProvider" >.</param>
        /// <returns>The string representation of the current <see cref = "BigDecimal" /> value as specified by the format and provider parameters.</returns>
        /// <seealso cref = "IFormattable.ToString(string,IFormatProvider)" />
        public string ToString ( string format , IFormatProvider formatProvider )
        {
            //TODO: Support "Standard Numeric Format" and "Custom Numeric Format" and the "IFormatProvider".
            //TODO: Support special values.
            //if ( StandardNumericFormatPattern.IsMatch ( format ) )
            //{
            //    Match formatMatch = StandardNumericFormatPattern.Match ( format );
            //    string decimalDigits = formatMatch.Groups [ "PrecisionSpecifier" ].Value;
            //    switch ( formatMatch.Groups [ "FormatSpecifier" ].Value.ToUpper () )
            //    {
            //        case "C" :
            //            if ( !string.IsNullOrEmpty ( decimalDigits ) )
            //            {}
            //            break;
            //        case "D" :
            //            break;
            //        case "E" :
            //            break;
            //        case "F" :
            //            break;
            //        case "N" :
            //            break;
            //        case "P" :
            //            break;
            //        case "R" :
            //            break;
            //        case "X" :
            //            break;
            //        default :
            //            throw new FormatException ( "Number format was not in the correct format." );
            //    }
            //}
            //else
            //{
            //    throw new NotImplementedException ();
            //}
            if ( Exponent > 0 )
            {
            return string.Concat ( Mantissa.ToString( ) , new string ( '0' , ( int ) Exponent ) );
            }
            if ( Exponent == 0 )
            {
            return Mantissa.ToString( );
            }
            if ( Exponent < 0 )
            {
                int numOfDigits = CalcHelper.NumberOfDigits ( Mantissa );
                BigInteger absExponent = BigInteger.Abs ( Exponent );
                if ( absExponent < numOfDigits )
                {
                    if ( Mantissa.Sign > 0 )
                    {
                    return Mantissa.ToString( ).Insert ( ( int ) ( numOfDigits - absExponent ) , "." );
                    }
                    if ( Mantissa.Sign < 0 )
                    {
                    return Mantissa.ToString( ).Insert ( ( int ) ( numOfDigits - absExponent + 1 ) , "." );
                    }
                }
                else if ( absExponent >= numOfDigits )
                {
                    if ( Mantissa.Sign > 0 )
                    {
                    return string.Concat ( "0." , new string ( '0' , ( int ) ( absExponent - numOfDigits ) ) , Mantissa );
                    }
                    if ( Mantissa.Sign < 0 )
                    {
                    return string.Concat ( "-0." , new string ( '0' , ( int ) ( absExponent - numOfDigits ) ) , Mantissa );
                    }
                }
            }
            return string.Concat ( Mantissa.ToString( ) , "E" , Exponent );
        }
        /// <summary>Returns a value that indicates whether the current instance and a specified object have the same value.</summary>
        /// <param name = "obj" >.</param>
        /// <returns><c>true</c> if the <paramref name = "obj" /> parameter is a <see cref = "BigDecimal" /> object or a type capable of implicit conversion to a <see cref = "BigDecimal" /> value, and its value is equal to the value of the current <see cref = "BigDecimal" /> object; otherwise, <c>false</c>.</returns>
        /// <seealso cref = "ValueType.Equals(object)" />
        public override bool Equals ( object obj )
        {
            if ( ReferenceEquals ( null , obj ) )
            {
            return false;
            }
            if ( obj is BigDecimal )
            {
            return Equals ( ( BigDecimal ) obj );
            }
            return false;
        }
        /// <summary>Returns the hash code for the current <see cref = "BigDecimal" /> object.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        /// <seealso cref = "ValueType.GetHashCode()" />
        public override int GetHashCode( )
        {
            unchecked
            {
            return ( int ) ( Mantissa.GetHashCode( ) * 13 ^ Exponent );
            }
        }
        #endregion
    }
}
