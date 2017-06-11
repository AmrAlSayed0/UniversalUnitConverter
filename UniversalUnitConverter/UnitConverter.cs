namespace UniversalUnitConverter
{
    #region Usings
    using ArbitraryPrecision;
    using Units.Length;
    #endregion
    /// <summary>The main Converter Class of this Library</summary>
    public static class UnitConverter
    {
        #region StaticMethods
        /// <summary>Converts a Length from one unit to another.</summary>
        /// <param name = "value" >The value of the property to be converted.</param>
        /// <param name = "fromUnit" >The unit of the property to be converted.</param>
        /// <param name = "toUnit" >The unit to convert the property's value to.</param>
        /// <returns>The value of the property in the target unit.</returns>
        public static BigDecimal Convert ( decimal value , LengthUnit fromUnit , LengthUnit toUnit )
        {
        return value * ( Length.Value ( fromUnit ) / Length.Value ( toUnit ) );
        }
        #endregion
    }
}
