#region Usings
using System;
using System.Numerics;
#endregion
namespace ArbitraryPrecision
{
    #region Usings
    #endregion
    /// <summary>Defines the context (settings) for the <see cref = "BigDecimal" /> struct.</summary>
    /// <seealso cref = "T:IEquatable" />
    /// <remarks></remarks>
    [ Serializable ]
    public struct BigDecimalContext : IEquatable < BigDecimalContext >
    {
        #region Constants
        /// <summary>Specifies the default type of the value of the <see cref = "BigDecimal" /> when no type is specified.</summary>
        internal const BigDecimalValues DefaultValueType = BigDecimalValues.Normal;
        /// <summary>Specifies the default rounding method to be used in the <see cref = "BigDecimal" /> when no other methods are specified.</summary>
        public const BigDecimalRoundingMethod DefaultRoundingMethod = BigDecimalRoundingMethod.RoundHalfUp;
        /// <summary>Specifies whether rounding should occur by default or not.</summary>
        public const bool DefaultRoundingState = true;
        /// <summary>Specifies whether signaling should occur by default or not.</summary>
        public const bool DefaultNaNSignaling = false;
        #endregion
        #region StaticFields
        /// <summary>Specifies the minimum precision the <see cref = "BigDecimal" /> number is allowed to be.</summary>
        static readonly BigInteger _minPrecision = BigInteger.One;
        /// <summary>Specifies the default precision used in the <see cref = "BigDecimal" /> when no other precision is specified.</summary>
        static readonly BigInteger _defaultPrecision = new BigInteger ( 128 );
        /// <summary>The default context for a normal number value.</summary>
        static readonly BigDecimalContext _defaultContext = new BigDecimalContext
                                                            {
                                                                _precision = _defaultPrecision ,
                                                                _alwaysRound = DefaultRoundingState ,
                                                                _roundingMethod = DefaultRoundingMethod ,
                                                                _isSpecialValue = false ,
                                                                _valueType = DefaultValueType ,
                                                                _isPositiveInfinity = false ,
                                                                _isNegativeInfinity = false ,
                                                                //_isPositiveZero = false ,
                                                                _isNegativeZero = false ,
                                                                _isNaN = false ,
                                                                _isSignalingNaN = DefaultNaNSignaling
                                                            };
        /// <summary>Represents the context (settings) of the Positive Infinity value.</summary>
        static readonly BigDecimalContext _positiveInfinityContext = new BigDecimalContext
                                                                     {
                                                                         _precision = BigInteger.Zero ,
                                                                         _alwaysRound = false ,
                                                                         _roundingMethod = BigDecimalRoundingMethod.None ,
                                                                         _isSpecialValue = true ,
                                                                         _valueType = BigDecimalValues.PositiveInfinity ,
                                                                         _isPositiveInfinity = true ,
                                                                         _isNegativeInfinity = false ,
                                                                         //_isPositiveZero = false ,
                                                                         _isNegativeZero = false ,
                                                                         _isNaN = false ,
                                                                         _isSignalingNaN = DefaultNaNSignaling
                                                                     };
        /// <summary>Represents the context (settings) of the Negative Infinity value.</summary>
        static readonly BigDecimalContext _negativeInfinityContext = new BigDecimalContext
                                                                     {
                                                                         _precision = BigInteger.Zero ,
                                                                         _alwaysRound = false ,
                                                                         _roundingMethod = BigDecimalRoundingMethod.None ,
                                                                         _isSpecialValue = true ,
                                                                         _valueType = BigDecimalValues.NegativeInfinity ,
                                                                         _isPositiveInfinity = false ,
                                                                         _isNegativeInfinity = true ,
                                                                         //_isPositiveZero = false ,
                                                                         _isNegativeZero = false ,
                                                                         _isNaN = false ,
                                                                         _isSignalingNaN = DefaultNaNSignaling
                                                                     };
        // <summary>Represents the context (settings) of the Positive Zero value.</summary>
        // private static readonly BigDecimalContext _positiveZeroContext = new BigDecimalContext
        //                                                                  {
        //                                                                  _precision = BigInteger.Zero ,
        //                                                                  _alwaysRound = false ,
        //                                                                  _roundingMethod = BigDecimalRoundingMethod.None ,
        //                                                                  _isSpecialValue = true ,
        //                                                                  _valueType = BigDecimalValues.PositiveZero ,
        //                                                                  _isPositiveInfinity = false ,
        //                                                                  _isNegativeInfinity = false ,
        //                                                                  _isPositiveZero = true ,
        //                                                                  _isNegativeZero = false ,
        //                                                                  _isNaN = false
        /// <summary>Represents the context (settings) of the Negative Zero value.</summary>
        static readonly BigDecimalContext _negativeZeroContext = new BigDecimalContext
                                                                 {
                                                                     _precision = BigInteger.Zero ,
                                                                     _alwaysRound = false ,
                                                                     _roundingMethod = BigDecimalRoundingMethod.None ,
                                                                     _isSpecialValue = true ,
                                                                     _valueType = BigDecimalValues.NegativeZero ,
                                                                     _isPositiveInfinity = false ,
                                                                     _isNegativeInfinity = false ,
                                                                     //_isPositiveZero = false ,
                                                                     _isNegativeZero = true ,
                                                                     _isNaN = false ,
                                                                     _isSignalingNaN = DefaultNaNSignaling
                                                                 };
        /// <summary>Represents the context (settings) of a value that is Not a Number.</summary>
        static readonly BigDecimalContext _nanContext = new BigDecimalContext
                                                        {
                                                            _precision = BigInteger.Zero ,
                                                            _alwaysRound = false ,
                                                            _roundingMethod = BigDecimalRoundingMethod.None ,
                                                            _isSpecialValue = true ,
                                                            _valueType = BigDecimalValues.NaN ,
                                                            _isPositiveInfinity = false ,
                                                            _isNegativeInfinity = false ,
                                                            //_isPositiveZero = false ,
                                                            _isNegativeZero = false ,
                                                            _isNaN = true ,
                                                            _isSignalingNaN = DefaultNaNSignaling
                                                        };
        #endregion
        #region Fields
        /// <summary>Specifies the maximum precision this number could be. If <see cref = "_alwaysRound" /> is set to <c>true</c> all operations are affected.</summary>
        [ NonSerialized ]
        BigInteger _precision;
        /// <summary>Specifies whether the significant digits should be rounded to the given precision after each operation. The rounding method used is specified by the <see cref = "_roundingMethod" /> field.</summary>
        bool _alwaysRound;
        /// <summary>Specifies the rounding method to be used when the result's precision exceeds the specified precision.</summary>
        BigDecimalRoundingMethod _roundingMethod;
        /// <summary>Represents what type of value the <see cref = "BigDecimal" /> is.</summary>
        BigDecimalValues _valueType;
        /// <summary>Specifies whether the number has a special value. ( eg. +∞ , -∞ , +0 , -0 , NaN )</summary>
        bool _isSpecialValue;
        /// <summary>Specifies whether this context represents a Positive Infinity ( +∞ ).</summary>
        bool _isPositiveInfinity;
        /// <summary>Specifies whether this context represents a Negative Infinity ( -∞ ).</summary>
        bool _isNegativeInfinity;
        // <summary>Specifies whether this context represents a Positive Zero ( +0 ).</summary>
        // private bool _isPositiveZero;
        /// <summary>Specifies whether this context represents a Negative Zero ( -0 ).</summary>
        bool _isNegativeZero;
        /// <summary>Specifies whether this context represents a Non-Numeric value ( NaN ).</summary>
        bool _isNaN;
        /// <summary>Specifies whether an Exception is thrown if a NaN is encountered or not.</summary>
        bool _isSignalingNaN;
        #endregion
        #region StaticProperties
        /// <summary>Gets the minimum precision the <see cref = "BigDecimal" /> number is allowed to be.</summary>
        /// <value>The minimum precision the <see cref = "BigDecimal" /> number is allowed to be.</value>
        /// <returns>The minimum precision the <see cref = "BigDecimal" /> number is allowed to be.</returns>
        public static BigInteger MinPrecision => _minPrecision;
        /// <summary>Gets the default precision used in the <see cref = "BigDecimal" /> when no other precision is specified.</summary>
        /// <value>The default precision used in the <see cref = "BigDecimal" /> when no other precision is specified.</value>
        /// <returns>The default precision used in the <see cref = "BigDecimal" /> when no other precision is specified.</returns>
        public static BigInteger DefaultPrecision => _defaultPrecision;
        /// <summary>Get the default context (settings) for a normal number value.</summary>
        /// <value>The default context (settings) for a normal number value.</value>
        /// <returns>The default context (settings) for a normal number value.</returns>
        public static BigDecimalContext DefaultContext => _defaultContext;
        /// <summary>Get the context (settings) of the Positive Infinity value.</summary>
        /// <value>The context (settings) of the Positive Infinity value.</value>
        /// <returns>The context (settings) of the Positive Infinity value.</returns>
        public static BigDecimalContext PositiveInfinityContext => _positiveInfinityContext;
        /// <summary>Gets the context (settings) of the Negative Infinity value.</summary>
        /// <value>The context (settings) of the Negative Infinity value.</value>
        /// <returns>The context (settings) of the Negative Infinity value.</returns>
        public static BigDecimalContext NegativeInfinityContext => _negativeInfinityContext;
        /// <summary>Gets the context (settings) of the Negative Zero value.</summary>
        /// <value>The context (settings) of the Negative Zero value.</value>
        /// <returns>The context (settings) of the Negative Zero value.</returns>
        public static BigDecimalContext NegativeZeroContext => _negativeZeroContext;
        /// <summary>Gets the context (settings) of a value that is Not a Number.</summary>
        /// <value>The context (settings) of a value that is Not a Number.</value>
        /// <returns>The context (settings) of a value that is Not a Number.</returns>
        public static BigDecimalContext NanContext => _nanContext;
        #endregion
        #region Properties
        /// <summary>Gets or sets what type of value the <see cref = "BigDecimal" /> is.</summary>
        /// <value>The type of the value.</value>
        /// <exception cref = "System.ArgumentOutOfRangeException" >The value assigned to the enum <see cref = "BigDecimalValues" /> is out of range of the allowed values.</exception>
        /// <returns>The type of the value.</returns>
        internal BigDecimalValues ValueType
        {
            get => this._valueType;
            set
            {
                switch ( value )
                {
                    case BigDecimalValues.NaN :
                        this = NanContext;
                        break;
                    case BigDecimalValues.NegativeZero :
                        this = NegativeZeroContext;
                        break;
                    //case BigDecimalValues.PositiveZero :
                    //this = PositiveZero;
                    //break;
                    case BigDecimalValues.NegativeInfinity :
                        this = NegativeInfinityContext;
                        break;
                    case BigDecimalValues.PositiveInfinity :
                        this = PositiveInfinityContext;
                        break;
                    case BigDecimalValues.Normal :
                        if ( this._valueType == BigDecimalValues.Normal )
                        {
                            this._isSpecialValue = false;
                            this._isPositiveInfinity = false;
                            this._isNegativeInfinity = false;
                            //this._isPositiveZero = false;
                            this._isNegativeZero = false;
                            this._isNaN = false;
                            this._isSignalingNaN = false;
                        }
                        else
                        {
                            this = DefaultContext;
                        }
                        break;
                    default : throw new ArgumentOutOfRangeException ( nameof ( value ) , value , "The value assigned to the enum \"" + nameof ( BigDecimalValues ) + "\" is out of range of the allowed values." );
                }
            }
        }
        /// <summary>Gets or sets the maximum precision the number could be. If AlwaysRound is set to <c>true</c> all operations are affected.</summary>
        /// <value>The maximum precision the number could be.</value>
        /// <returns>The maximum precision the number could be.</returns>
        public BigInteger Precision
        {
            get => this._precision;
            set
            {
                if ( this.IsSpecialValue )
                {
                    return;
                }
                this._precision = value < MinPrecision ? MinPrecision : value;
            }
        }
        /// <summary>Gets or sets whether the significant digits should be truncated to the given precision after each operation. The rounding method used is specified by the <see cref = "RoundingMethod" /> enum.</summary>
        /// <value><c>true</c> if the significant digits should be rounded to the given precision after each operation; otherwise, <c>false</c></value>
        /// <returns><c>true</c> if the significant digits should be rounded to the given precision after each operation; otherwise, <c>false</c></returns>
        public bool AlwaysRound
        {
            get => this._alwaysRound;
            set
            {
                if ( this.IsSpecialValue )
                {
                    return;
                }
                if ( ! value )
                {
                    this._roundingMethod = BigDecimalRoundingMethod.None;
                }
                this._alwaysRound = value;
            }
        }
        /// <summary>Gets or sets the rounding method to be used when the result's precision exceeds the specified precision.</summary>
        /// <value>The rounding method to be used when the result's precision exceeds the specified precision.</value>
        /// <returns>The rounding method to be used when the result's precision exceeds the specified precision.</returns>
        public BigDecimalRoundingMethod RoundingMethod
        {
            get => this._roundingMethod;
            set
            {
                if ( this.IsSpecialValue )
                {
                    return;
                }
                if ( value == BigDecimalRoundingMethod.None )
                {
                    this._alwaysRound = false;
                }
                this._roundingMethod = value;
            }
        }
        /// <summary>Gets or sets whether the number has a special value. ( eg. +∞ , -∞ , +0 , -0 , NaN )</summary>
        /// <value><c>true</c> if this instance has a special value, <c>false</c> if not.</value>
        /// <returns><c>true</c> if this instance has a special value, <c>false</c> if not.</returns>
        public bool IsSpecialValue
        {
            get
            {
                this._isSpecialValue = this.IsPositiveInfinity ||
                                       this.IsNegativeInfinity || /*this.IsPositiveZero ||*/
                                       this.IsNegativeZero ||
                                       this.IsNaN;
                return this._isSpecialValue;
            }
            set
            {
                if ( value )
                {
                    if ( ! this.IsSpecialValue )
                    {
                        this = NanContext;
                    }
                }
                else
                {
                    if ( this.IsSpecialValue )
                    {
                        this = DefaultContext;
                    }
                }
            }
        }
        /// <summary>Gets whether this context represents a Positive Infinity ( +∞ ).</summary>
        /// <value><c>true</c> if this instance is Positive Infinity ( +∞ ), <c>false</c> if not.</value>
        /// <returns><c>true</c> if this instance is Positive Infinity ( +∞ ), <c>false</c> if not.</returns>
        public bool IsPositiveInfinity
        {
            get
            {
                if ( ( this._valueType == BigDecimalValues.PositiveInfinity ) &&
                     ( this._precision == BigInteger.Zero ) &&
                     ! this._alwaysRound &&
                     ( this._roundingMethod == BigDecimalRoundingMethod.None ) &&
                     this._isSpecialValue )
                {
                    this = PositiveInfinityContext;
                }
                else
                {
                    this._isPositiveInfinity = false;
                }
                return this._isPositiveInfinity;
            }
            internal set
            {
                if ( value )
                {
                    this = PositiveInfinityContext;
                }
                else
                {
                    if ( this.IsPositiveInfinity )
                    {
                        this = DefaultContext;
                    }
                }
                this._isPositiveInfinity = value;
            }
        }
        /// <summary>Gets whether this context represents a Negative Infinity ( -∞ ).</summary>
        /// <value><c>true</c> if this instance is a Negative Infinity ( -∞ ), <c>false</c> if not.</value>
        /// <returns><c>true</c> if this instance is a Negative Infinity ( -∞ ), <c>false</c> if not.</returns>
        public bool IsNegativeInfinity
        {
            get
            {
                if ( ( this._valueType == BigDecimalValues.NegativeInfinity ) &&
                     ( this._precision == BigInteger.Zero ) &&
                     ! this._alwaysRound &&
                     ( this._roundingMethod == BigDecimalRoundingMethod.None ) &&
                     this._isSpecialValue )
                {
                    this = NegativeInfinityContext;
                }
                else
                {
                    this._isNegativeInfinity = false;
                }
                return this._isNegativeInfinity;
            }
            internal set
            {
                if ( value )
                {
                    this = NegativeInfinityContext;
                }
                if ( ! value )
                {
                    if ( this.IsNegativeInfinity )
                    {
                        this = DefaultContext;
                    }
                }
                this._isNegativeInfinity = value;
            }
        }
        // <summary>Gets or sets whether this context represents a Positive Zero ( +0 ).</summary>
        // public bool IsPositiveZero
        // {
        //     get
        //     {
        //         if ( this._valueType == BigDecimalValues.PositiveZero &&
        //              this._precision == BigInteger.Zero &&
        //              !this._alwaysRound &&
        //              this._roundingMethod == BigDecimalRoundingMethod.None &&
        //              this._isSpecialValue )
        //         {
        //             this = PositiveZeroContext;
        //         }
        //         else
        //         {
        //             this._isPositiveZero = false;
        //         }
        //         return this._isPositiveZero;
        //     }
        //     internal set
        //     {
        //         if ( value )
        //         {
        //             this = PositiveZeroContext;
        //         }
        //         if ( !value )
        //         {
        //             if ( this.IsPositiveZero )
        //             {
        //                 this = DefaultContext;
        //             }
        //         }
        //         this._isPositiveZero = value;
        //     }
        // }
        /// <summary>Gets or sets whether this context represents a Negative Zero ( -0 ).</summary>
        /// <value><c>true</c> if this instance is a Negative Zero ( -0 )., <c>false</c> if not.</value>
        /// <returns><c>true</c> if this instance is a Negative Zero ( -0 )., <c>false</c> if not.</returns>
        public bool IsNegativeZero
        {
            get
            {
                if ( ( this._valueType == BigDecimalValues.NegativeZero ) &&
                     ( this._precision == BigInteger.Zero ) &&
                     ! this._alwaysRound &&
                     ( this._roundingMethod == BigDecimalRoundingMethod.None ) &&
                     this._isSpecialValue )
                {
                    this = NegativeZeroContext;
                }
                else
                {
                    this._isNegativeZero = false;
                }
                return this._isNegativeZero;
            }
            internal set
            {
                if ( value )
                {
                    this = NegativeZeroContext;
                }
                if ( ! value )
                {
                    if ( this.IsNegativeZero )
                    {
                        this = DefaultContext;
                    }
                }
                this._isNegativeZero = value;
            }
        }
        /// <summary>Gets whether this context represents a Non-Numeric value ( NaN ).</summary>
        /// <value><c>true</c> if this instance is a Non-Numeric value ( NaN )., <c>false</c> if not.</value>
        /// <returns><c>true</c> if this instance is a Non-Numeric value ( NaN )., <c>false</c> if not.</returns>
        public bool IsNaN
        {
            get
            {
                if ( ( this._valueType == BigDecimalValues.NaN ) &&
                     ( this._precision == BigInteger.Zero ) &&
                     ! this._alwaysRound &&
                     ( this._roundingMethod == BigDecimalRoundingMethod.None ) &&
                     this._isSpecialValue )
                {
                    this = NanContext;
                }
                else
                {
                    this._isNaN = false;
                }
                return this._isNaN;
            }
            internal set
            {
                if ( value )
                {
                    this = NanContext;
                }
                if ( ! value )
                {
                    if ( this.IsNaN )
                    {
                        this = DefaultContext;
                    }
                }
                this._isNaN = value;
            }
        }
        /// <summary>Specifies whether an Exception is thrown if a NaN is encountered or not.</summary>
        /// <value><c>true</c> if an exception should be thrown when NaN is encountered, <c>false</c> if not.</value>
        /// <returns><c>true</c> if an exception should be thrown when NaN is encountered, <c>false</c> if not.</returns>
        public bool IsSignalingNaN
        {
            get => this._isSignalingNaN;
            set
            {
                if ( this.IsNaN )
                {
                    this._isSignalingNaN = value;
                }
            }
        }
        #endregion
        #region ConstructorDestructor
        /// <summary>Initializes a new instance of the <see cref = "BigDecimalContext" /> class.</summary>
        /// <param name = "precision" >The precision to be used in the <see cref = "BigDecimal" /> struct.</param>
        /// <param name = "alwaysRound" >Whether to round the number when the digits exceed the precision or not.</param>
        /// <param name = "roundingMethod" >The rounding method to be used.</param>
        /// <param name = "valueType" >The type of value this <see cref = "BigDecimalContext" /> represents.</param>
        internal BigDecimalContext ( BigInteger precision , bool alwaysRound , BigDecimalRoundingMethod roundingMethod , BigDecimalValues valueType )
        : this ()
        {
            this.ValueType = valueType;
            this.Precision = precision;
            this.RoundingMethod = roundingMethod;
            this.AlwaysRound = alwaysRound;
            this.IsSignalingNaN = DefaultNaNSignaling;
        }
        // ///<summary>Initializes a new instance of the<see cref = "BigDecimalContext" /> class.</summary>
        // public BigDecimalContext()
        // : this(DefaultPrecision,
        //          DefaultRoundingState,
        //          DefaultRoundingMethod,
        //          DefaultValueType)
        //{ }
        /// <summary>Initializes a new instance of the <see cref = "BigDecimalContext" /> class.</summary>
        /// <param name = "precision" >The precision to be used in the <see cref = "BigDecimal" /> struct.</param>
        public BigDecimalContext ( BigInteger precision )
        : this ( precision , DefaultRoundingState , DefaultRoundingMethod , DefaultValueType )
        {}
        /// <summary>Initializes a new instance of the <see cref = "BigDecimalContext" /> class.</summary>
        /// <param name = "precision" >The precision to be used in the <see cref = "BigDecimal" /> struct.</param>
        /// <param name = "alwaysRound" >Whether to round the number when the digits exceed the precision or not.</param>
        /// <param name = "roundingMethod" >The rounding method to be used.</param>
        public BigDecimalContext ( BigInteger precision , bool alwaysRound , BigDecimalRoundingMethod roundingMethod )
        : this ( precision , alwaysRound , roundingMethod , DefaultValueType )
        {}
        #endregion
        #region Overrides
        /// <summary>Returns a string that represents the current <see cref = "BigDecimalContext" /> object.</summary>
        /// <returns>A string that represents the current <see cref = "BigDecimalContext" /> object.</returns>
        public override string ToString ()
        {
            if ( ! this.IsSpecialValue )
            {
                string roundingMethodString;
                switch ( this.RoundingMethod )
                {
                    case BigDecimalRoundingMethod.RoundDown :
                        roundingMethodString = "Down";
                        break;
                    case BigDecimalRoundingMethod.RoundUp :
                        roundingMethodString = "Up";
                        break;
                    case BigDecimalRoundingMethod.RoundHalfDown :
                        roundingMethodString = "Half-Down";
                        break;
                    case BigDecimalRoundingMethod.RoundHalfUp :
                        roundingMethodString = "Half-Up";
                        break;
                    case BigDecimalRoundingMethod.RoundFloor :
                        roundingMethodString = "Floor";
                        break;
                    case BigDecimalRoundingMethod.RoundCeiling :
                        roundingMethodString = "Ceiling";
                        break;
                    case BigDecimalRoundingMethod.RoundHalfEven :
                        roundingMethodString = "Half-Even";
                        break;
                    case BigDecimalRoundingMethod.Round05Up :
                        roundingMethodString = "0-5-Up";
                        break;
                    case BigDecimalRoundingMethod.None :
                        roundingMethodString = "None";
                        break;
                    default :
                        roundingMethodString = "None";
                        break;
                }
                return $"Precision = {this.Precision:D} , Always Round = {this.AlwaysRound} , Rounding Method = {roundingMethodString}.";
            }
            string valueTypeString;
            switch ( this.ValueType )
            {
                case BigDecimalValues.PositiveInfinity :
                    valueTypeString = "Positive Infinity ( + ∞ )";
                    break;
                case BigDecimalValues.NegativeInfinity :
                    valueTypeString = "Negative Infinity ( - ∞ )";
                    break;
                //case BigDecimalValues.PositiveZero :
                //    valueTypeString = "Positive Infinity ( + 0 )";
                //    break;
                case BigDecimalValues.NegativeZero :
                    valueTypeString = "Positive Infinity ( - 0 )";
                    break;
                case BigDecimalValues.NaN :
                    valueTypeString = this.IsSignalingNaN ? "Signaling Not-a-Number ( sNaN )" : "Not-a-Number ( NaN )";
                    break;
                default :
                    valueTypeString = "Not-a-Number ( NaN )";
                    break;
            }
            return $"Is Special Value = {this.IsSpecialValue} , Value Type = {valueTypeString}.";
        }
        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name = "obj" >.</param>
        /// <returns><c>true</c> if <paramref name = "obj" /> and this instance are the same type and represent the same value; otherwise, <c>false</c>.</returns>
        public override bool Equals ( object obj )
        {
            if ( ReferenceEquals ( null , obj ) )
            {
                return false;
            }
            return obj is BigDecimalContext bigDecimalContext && this.Equals ( bigDecimalContext );
        }
        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>A hash code for the current <see cref = "BigDecimalContext" />.</returns>
        public override int GetHashCode () =>
        HashCode.Combine ( this._roundingMethod ,
                           this._alwaysRound ,
                           this._precision ,
                           this._isSpecialValue ,
                           this._valueType ,
                           this._isPositiveInfinity ,
                           this._isNegativeInfinity ,
                           this._isNegativeZero ) ^
        HashCode.Combine ( this._isNaN , this._isSignalingNaN );
        #endregion
        #region OperatorOverloads
        /// <summary>Indicates whether the left <see cref = "BigDecimalContext" /> object is equal to the right <see cref = "BigDecimalContext" /> object.</summary>
        /// <param name = "left" >The <c>left</c> <see cref = "BigDecimalContext" /> object.</param>
        /// <param name = "right" >The <c>right</c> <see cref = "BigDecimalContext" /> object.</param>
        /// <returns><c>true</c> if the <paramref name = "left" /> <see cref = "BigDecimalContext" /> object is equal to the <paramref name = "right" /> <see cref = "BigDecimalContext" /> object; otherwise, <c>false</c></returns>
        public static bool operator == ( BigDecimalContext left , BigDecimalContext right ) => left.Equals ( right );
        /// <summary>Indicates whether the left <see cref = "BigDecimalContext" /> object is  not equal to the right <see cref = "BigDecimalContext" /> object.</summary>
        /// <param name = "left" >The <c>left</c> <see cref = "BigDecimalContext" /> object.</param>
        /// <param name = "right" >The <c>right</c> <see cref = "BigDecimalContext" /> object.</param>
        /// <returns><c>true</c> if the <paramref name = "left" /> <see cref = "BigDecimalContext" /> object is not equal to the <paramref name = "right" /> <see cref = "BigDecimalContext" /> object; otherwise, <c>false</c></returns>
        public static bool operator != ( BigDecimalContext left , BigDecimalContext right ) => ! left.Equals ( right );
        #endregion
        #region IEquatable<BigDecimalContext> Members
        /// <summary>Indicates whether the current <see cref = "BigDecimalContext" /> object is equal to another <see cref = "BigDecimalContext" /> object of the same type.</summary>
        /// <param name = "other" >.</param>
        /// <returns><c>true</c> if the current object is equal to the <paramref name = "other" /> parameter; otherwise, <c>false</c>.</returns>
        public bool Equals ( BigDecimalContext other ) =>
        this._roundingMethod.Equals ( other._roundingMethod ) &&
        this._alwaysRound.Equals ( other._alwaysRound ) &&
        this._precision.Equals ( other._precision ) &&
        this._isSpecialValue.Equals ( other._isSpecialValue ) &&
        this._valueType.Equals ( other._valueType ) &&
        this._isPositiveInfinity.Equals ( other._isPositiveInfinity ) &&
        this._isNegativeInfinity.Equals ( other._isNegativeInfinity ) &&
        this._isNegativeZero.Equals ( other._isNegativeZero ) &&
        this._isNaN.Equals ( other._isNaN ) &&
        this._isSignalingNaN.Equals ( other._isSignalingNaN );
        #endregion
    }
}
