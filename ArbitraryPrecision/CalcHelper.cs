#region Usings
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
#endregion
namespace ArbitraryPrecision
{
    #region Usings
    #endregion
    /// <summary>A class used to aid in various calculations used in the <see cref = "BigDecimal" /> struct</summary>
    /// <remarks></remarks>
    public static class CalcHelper
    {
        #region StaticMethods
        /// <summary>Calculates the factorial of the specified value.</summary>
        /// <param name = "value" >The value whose factorial is to be calculated.</param>
        /// <returns>The factorial of the specified value.</returns>
        /// <exception cref = "System.ArgumentOutOfRangeException" >The value who's factorial is to be calculated cannot be a negative number.</exception>
        public static BigInteger Factorial ( BigInteger value )
        {
            if ( value.Sign < 0 )
            {
                throw new ArgumentOutOfRangeException ( nameof ( value ) , value , "The value who's factorial is to be calculated cannot be a negative number." );
            }
            if ( value.IsZero ||
                 value.IsOne )
            {
                return BigInteger.One;
            }
            BigInteger result = BigInteger.One;
            for ( BigInteger i = new BigInteger ( 2 ); i <= value; i++ )
            {
                result *= i;
            }
            return result;
        }
        /// <summary>Calculates the power of a <see cref = "BigInteger" /> to another <see cref = "BigInteger" /></summary>
        /// <param name = "value" >The <paramref name = "value" /> to be raised.</param>
        /// <param name = "power" >The <paramref name = "power" /> to raise the <paramref name = "value" /> to.</param>
        /// <returns>The result of <paramref name = "value" /> raised to the <paramref name = "power" />.</returns>
        /// <exception cref = "System.ArgumentOutOfRangeException" >The power cannot be a negative number.</exception>
        public static BigInteger Pow ( BigInteger value , BigInteger power )
        {
            if ( power < 0 )
            {
                throw new ArgumentOutOfRangeException ( nameof ( power ) , power , "The power cannot be a negative number." );
            }
            if ( power == BigInteger.Zero )
            {
                return BigInteger.One;
            }
            if ( power == BigInteger.One )
            {
                return value;
            }
            BigInteger result = value;
            BigInteger compareValue = power - BigInteger.One;
            for ( BigInteger i = BigInteger.Zero; i < compareValue; i++ )
            {
                result *= value;
            }
            return result;
        }
        /// <summary>Multiplies corresponding elements of the matrices nd returns an array result.</summary>
        /// <param name = "arrays" >The arrays to multiply together.</param>
        /// <returns>The element of each array multiplied together.</returns>
        /// <exception cref = "System.ArgumentException" >The passed arrays must be of the same Length.</exception>
        public static BigDecimal [] LineMatrixMult ( params BigDecimal [] [] arrays )
        {
            int arraysLen = arrays [ 0 ].Length;
            if ( arraysLen == 1 )
            {
                return arrays [ 0 ];
            }
            for ( int i = 1; i < arrays.Length; i++ )
            {
                if ( arrays [ i ].Length == arraysLen )
                {
                    continue;
                }
                throw new ArgumentException ( "The passed arrays must be of the same Length." , nameof ( arrays ) );
            }
            BigDecimal [] result = new BigDecimal[ arraysLen ];
            for ( int i = 0; i < arraysLen; i++ )
            {
                result [ i ] = BigDecimal.One;
                for ( int j = 0; j < arrays.Length; j++ )
                {
                    result [ i ] *= arrays [ j ] [ i ];
                }
            }
            return result;
        }
        /// <summary>Divides each element of the first array with the corresponding element in the second array.</summary>
        /// <param name = "arr1" >The array to divide.</param>
        /// <param name = "arr2" >The array to divide by.</param>
        /// <returns>An array containing each element of the first array divide by the corresponding element in the second array.</returns>
        /// <exception cref = "System.ArgumentException" >The passed arrays must be of the same size.</exception>
        public static BigDecimal [] LineMatrixDiv ( BigDecimal [] arr1 , BigInteger [] arr2 )
        {
            int arr1Len = arr1.Length;
            int arr2Len = arr2.Length;
            if ( arr1Len != arr2Len )
            {
                throw new ArgumentException ( "The passed arrays must be of the same size." );
            }
            BigDecimal [] result = new BigDecimal[ arr1Len ];
            for ( int i = 0; i < arr1Len; i++ )
            {
                result [ i ] = arr1 [ i ] / new BigDecimal ( arr2 [ i ] , 0 );
            }
            return result;
        }
        /// <summary>Calculates the sum of all the elements of the array together.</summary>
        /// <param name = "array" >The array whose elements is to be added.</param>
        /// <returns>The sum of all the elements of the array.</returns>
        public static BigDecimal LineMatrixElementSum ( BigDecimal [] array )
        {
            BigDecimal result = BigDecimal.Zero;
            for ( int i = 0; i < array.Length; i++ )
            {
                result += array [ i ];
            }
            return result;
        }
        /// <summary>Calculates the values of X! from X = 0 to X = ∞.</summary>
        /// <param name = "numOfFactorials" >Number of values to calculate.</param>
        /// <returns>The calculated values.</returns>
        /// <exception cref = "System.ArgumentOutOfRangeException" >The number of values to calculate must not be a negative number.</exception>
        /// <exception cref = "System.OverflowException" >The number of values to calculate cannot exceed 18446744073709551615.</exception>
        [ MethodImpl ( MethodImplOptions.AggressiveInlining ) ]
        public static BigInteger [] ZeroToInfFactorialArray ( BigInteger numOfFactorials )
        {
            if ( numOfFactorials <= 0 )
            {
                throw new ArgumentOutOfRangeException ( nameof ( numOfFactorials ) , numOfFactorials , "The number of values to calculate must not be a negative number." );
            }
            if ( numOfFactorials.IsOne )
            {
                return new []
                       {
                           BigInteger.One
                       };
            }
            if ( numOfFactorials > new BigInteger ( ulong.MaxValue ) )
            {
                throw new OverflowException ( "The number of values to calculate cannot exceed " + ulong.MaxValue + "." );
            }
            ulong numOfFactorialsUl = ( ulong ) numOfFactorials;
            BigInteger [] result = new BigInteger[ numOfFactorialsUl ];
            result [ 0UL ] = BigInteger.One;
            for ( ulong i = 1UL; i < numOfFactorialsUl; i++ )
            {
                result [ i ] = result [ i - 1UL ] * i;
            }
            return result;
        }
        /// <summary>Calculates the values of X^n from n = 0 to n = ∞.</summary>
        /// <param name = "value" >The value whose power is to be calculated.</param>
        /// <param name = "numOfPowers" >Number of values to calculate.</param>
        /// <returns>The calculated values.</returns>
        /// <exception cref = "System.ArgumentOutOfRangeException" >Powers must not be a negative number or zero.</exception>
        /// <exception cref = "System.OverflowException" >The number of values to calculate cannot exceed 18446744073709551615.</exception>
        [ MethodImpl ( MethodImplOptions.AggressiveInlining ) ]
        public static BigDecimal [] ZeroToInfXPowerNArr ( BigDecimal value , BigInteger numOfPowers )
        {
            if ( numOfPowers <= 0 )
            {
                throw new ArgumentOutOfRangeException ( nameof ( numOfPowers ) , numOfPowers , "Powers must not be a negative number or zero." );
            }
            if ( numOfPowers.IsOne )
            {
                return new []
                       {
                           BigDecimal.One
                       };
            }
            if ( numOfPowers > new BigInteger ( ulong.MaxValue ) )
            {
                throw new OverflowException ( "The number of values to calculate cannot exceed " + ulong.MaxValue + "." );
            }
            ulong numOfPowersUl = ( ulong ) numOfPowers;
            BigDecimal [] result = new BigDecimal[ numOfPowersUl ];
            result [ 0UL ] = BigDecimal.One;
            for ( ulong i = 1UL; i < numOfPowersUl; i++ )
            {
                result [ i ] = result [ i - 1UL ] * value;
            }
            return result;
        }
        /// <summary>Calculates the values of ( ( X - 1 )/( X + 1 ) ) ^ ( 2 n + 1 ) from n = 0 to n = ∞.</summary>
        /// <param name = "value" >The value of X in the formula.</param>
        /// <param name = "numOfPowers" >Number of values to calculate.</param>
        /// <returns>The calculated values.</returns>
        /// <exception cref = "System.ArgumentOutOfRangeException" >Powers must not be a negative number or zero.</exception>
        /// <exception cref = "System.OverflowException" >The number of values to calculate cannot exceed 18446744073709551615.</exception>
        [ MethodImpl ( MethodImplOptions.AggressiveInlining ) ]
        public static BigDecimal [] OneToInfOddXMinusOneDivXPlusOnePowerN ( BigDecimal value , BigInteger numOfPowers )
        {
            if ( numOfPowers <= 0 )
            {
                throw new ArgumentOutOfRangeException ( nameof ( numOfPowers ) , numOfPowers , "Powers must not be a negative number or zero." );
            }
            if ( numOfPowers.IsOne )
            {
                return new []
                       {
                           ( value - BigDecimal.One ) / ( value + BigDecimal.One )
                       };
            }
            if ( numOfPowers > new BigInteger ( ulong.MaxValue ) )
            {
                throw new OverflowException ( "The number of values to calculate cannot exceed " + ulong.MaxValue + "." );
            }
            ulong numOfPowersUl = ( ulong ) numOfPowers;
            BigDecimal [] result = new BigDecimal[ numOfPowersUl ];
            result [ 0UL ] = ( value - BigDecimal.One ) / ( value + BigDecimal.One );
            BigDecimal tempValueSquared = result [ 0UL ] * result [ 0UL ];
            for ( ulong i = 1UL; i < numOfPowersUl; i++ )
            {
                result [ i ] = result [ i - 1UL ] * tempValueSquared;
            }
            return result;
        }
        /// <summary>Calculates the values of 1 / ( 2 n + 1 ) from n = 0 to n = ∞.</summary>
        /// <param name = "numOfNums" >Number of values to calculate.</param>
        /// <returns>The calculated values.</returns>
        /// <exception cref = "System.ArgumentOutOfRangeException" >Powers must not be a negative number or zero.</exception>
        /// <exception cref = "System.OverflowException" >The number of values to calculate cannot exceed 18446744073709551615.</exception>
        [ MethodImpl ( MethodImplOptions.AggressiveInlining ) ]
        public static BigDecimal [] OneToInfOddOneDivN ( BigInteger numOfNums )
        {
            if ( numOfNums <= 0 )
            {
                throw new ArgumentOutOfRangeException ( nameof ( numOfNums ) , numOfNums , "Powers must not be a negative number or zero." );
            }
            if ( numOfNums.IsOne )
            {
                return new []
                       {
                           BigDecimal.One
                       };
            }
            if ( numOfNums > new BigInteger ( ulong.MaxValue ) )
            {
                throw new OverflowException ( "The number of values to calculate cannot exceed " + ulong.MaxValue + "." );
            }
            ulong numOfNumsUl = ( ulong ) numOfNums;
            BigDecimal [] result = new BigDecimal[ numOfNumsUl ];
            for ( ulong i = 0UL; i < numOfNumsUl; i++ )
            {
                result [ i ] = BigDecimal.One / ( ( 2UL * i ) + 1UL );
            }
            return result;
        }
        /// <summary>Gets the last digit of the <see cref = "BigInteger" />.</summary>
        /// <param name = "bigInteger" >The <see cref = "BigInteger" />.</param>
        /// <returns>The last digit.</returns>
        public static BigInteger LastDigit ( this BigInteger bigInteger ) => bigInteger % new BigInteger ( 10 );
        /// <summary>Counts the number of digits in a BigInteger.</summary>
        /// <param name = "value" >The BigInteger to be counted.</param>
        /// <returns>The number of Digits in the BigInteger</returns>
        internal static int NumberOfDigits ( BigInteger value ) => BigInteger.Abs ( value ).ToString ().Length;
        #endregion
    }
}
