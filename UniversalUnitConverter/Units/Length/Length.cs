#region Usings
using System.Collections.Generic;
using ArbitraryPrecision;
#endregion
namespace UniversalUnitConverter.Units.Length
{
    #region Usings
    #endregion
    /// <summary></summary>
    public static class Length
    {
        #region Constants
        const LengthUnit BaseUnit = LengthUnit.Meter;
        #endregion
        #region StaticFields
        static readonly Dictionary < LengthUnit , BigDecimal > _lengthUnitMap = new Dictionary < LengthUnit , BigDecimal > ();
        #endregion
        #region ConstructorDestructor
        /// <summary>Initializes a new instance of the <see cref = "T:UniversalUnitConverter.Units.Length.Length" /> class.</summary>
        static Length ()
        {
            _lengthUnitMap.Add ( LengthUnit.Meter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.YottaMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.ZettaMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.ExaMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.PetaMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.TeraMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.GigaMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.MegaMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.KiloMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.HectoMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.DecaMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.DeciMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.CentiMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.MilliMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.MicroMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.NanoMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.PicoMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.FemtoMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.AttoMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.ZeptoMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.YoctoMeter , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.MegaParsec , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.KiloParsec , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Parsec , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.LightYear , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.AstronomicalUnit , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Mil , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.NauticalLeagueInternational , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.NauticalLeagueUk , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.LeagueUs , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.NauticalMileInternational , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.NauticalMileUk , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Mile , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.MileUsSurvey , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.MileRoman , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.KiloYard , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Furlong , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.FurlongUsSurvey , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Chain , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.ChainUsSurvey , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.RamsdenChain , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.GunterChain , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Rod , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.RodUsSurvey , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Perch , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Pole , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.PoppySeed , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Stick , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Span , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Fathom , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Cable , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.FathomUsServey , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.FrenchFathom , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Yard , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Foot , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.FootUsSurvey , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.FootIndianSurvey , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.FrenchFoot , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Line , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.FrenchLine , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.PortugueseLine , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.GermanLine , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Link , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.LinkUsSurvey , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Thou , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Toise , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Digit , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Cubit , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Hand , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Ell , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Step , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Rope , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Nail , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Shaftment , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Shackle , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Palm , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Pace , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Grade , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Finger , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Inch , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Spindle , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Skein , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.PyramidInch , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.FrenchInch , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.MilliInch , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.MicroInch , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Angstrom , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Micron , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.XUnit , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Fermi , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Arpent , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Pica , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Point , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.TruchetPoint , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.FournierPoint , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Twip , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Aln , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Famn , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Caliber , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.Ken , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.BarleyCorn , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.RussianArchin , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.MyriaMetre , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.ScandinavianMile , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.RomanActus , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.ClothNail , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.ColthSpan , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.VaraDeTarea , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.VaraConuquera , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.VaraCastellana , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.CubitGreek , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.LongReedBible , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.ReedBible , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.CubitBible , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.LongCubitBible , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.SpanBible , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.HandBreadthBible , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.FingerBreadthBible , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.PlankLength , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.ElectronRadiusClassic , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.BohrRadius , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.EarthEquatorialRadius , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.EarthPolarRadius , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.EarthSunDistance , BigDecimal.Parse ( "1" ) );
            _lengthUnitMap.Add ( LengthUnit.SunRadius , BigDecimal.Parse ( "1" ) );
        }
        #endregion
        #region StaticMethods
        /// <summary>Calculates the ratio of the given unit to base unit.</summary>
        /// <param name = "aUnit" >The given unit.</param>
        /// <returns>The ratio of the given unit to base unit.</returns>
        public static BigDecimal Value ( LengthUnit aUnit )
        {
            if ( _lengthUnitMap != null )
            {
                return _lengthUnitMap [ aUnit ];
            }
            if ( _lengthUnitMap != null )
            {
                return _lengthUnitMap [ BaseUnit ];
            }
            return BigDecimal.Zero;
        }
        #endregion
    }
}
