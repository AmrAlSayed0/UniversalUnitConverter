namespace ArbitraryPrecision
{
    #region Usings
    using System;
    #endregion
    /// <summary>Determines the rounding method to be used on the <see cref = "BigDecimal" /> number.</summary>
    /// <remarks></remarks>
    [ Serializable ]
    public enum BigDecimalRoundingMethod : byte
    {
        /// <summary>No rounding to occur.</summary>
        None = 0 ,
        /// <summary>The discarded digits are ignored; the result is unchanged. (Round toward 0; truncate.)</summary>
        RoundDown = 1 ,
        /// <summary>If the discarded digits represent greater than or equal to half (0.5) of the value of a one in the next left position then the result coefficient should be incremented by 1 (rounded up). Otherwise the discarded digits are ignored.</summary>
        RoundHalfUp = 2 ,
        /// <summary>If the discarded digits represent greater than half (0.5) the value of a one in the next left position then the result coefficient should be incremented by 1 (rounded up). If they represent less than half, then the result coefficient is not adjusted (that is, the discarded digits are ignored). Otherwise (they represent exactly half) the result coefficient is unaltered if its rightmost digit is even, or incremented by 1 (rounded up) if its rightmost digit is odd (to make an even digit).</summary>
        RoundHalfEven = 3 ,
        /// <summary>If all of the discarded digits are zero or if the sign is -1 the result is unchanged. Otherwise, the result coefficient should be incremented by 1 (rounded up). (Round toward +∞.)</summary>
        RoundCeiling = 4 ,
        /// <summary>If all of the discarded digits are zero or if the sign is 1 the result is unchanged. Otherwise, the sign is -1 and the result coefficient should be incremented by 1. (Round toward -∞.)</summary>
        RoundFloor = 5 ,
        /// <summary>If the discarded digits represent greater than half (0.5) of the value of a one in the next left position then the result coefficient should be incremented by 1 (rounded up). Otherwise (the discarded digits are 0.5 or less) the discarded digits are ignored.</summary>
        RoundHalfDown = 6 ,
        /// <summary>If all of the discarded digits are zero the result is unchanged. Otherwise, the result coefficient should be incremented by 1 (rounded up). (Round away from 0.)</summary>
        RoundUp = 7 ,
        /// <summary>The same as round-up, except that rounding up only occurs if the digit to be rounded up is 0 or 5, and after overflow the result is the same as for round-down. (Round zero or five away from 0.)</summary>
        Round05Up = 8
    }
}
