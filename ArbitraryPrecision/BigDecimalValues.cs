#region Usings
using System;
#endregion
namespace ArbitraryPrecision
{
    #region Usings
    #endregion
    /// <summary>Represents the special values a number could take.</summary>
    [ Serializable ]
    enum BigDecimalValues : byte
    {
        /// <summary>Represents a normal number.</summary>
        Normal = 0 ,
        /// <summary>Represents a positively infinite value. ( +∞ )</summary>
        PositiveInfinity = 1 ,
        /// <summary>Represents a negatively infinite value. ( -∞ )</summary>
        NegativeInfinity = 2 ,
        ///// <summary>Represents a positive zero value. ( +0 )</summary>
        //PositiveZero = 3 ,
        /// <summary>Represents a negative zero value. ( -0 )</summary>
        NegativeZero = 4 ,
        /// <summary>Represents a value that is not a number. ( Not-a-Number )</summary>
        NaN = 5
    }
}
