namespace BloodHub.Shared.Extentions
{
    #region Enums mapping

    public static class GenderMapping
    {
        public static readonly Dictionary<Gender, string> EnumToStringMap = new Dictionary<Gender, string> 
        {
            { Gender.Male, "Nam" }, 
            { Gender.Female, "Nữ" }, 
            { Gender.Other, "Khác" } 
        }; 
        
        public static readonly Dictionary<string, Gender> StringToEnumMap = new Dictionary<string, Gender> 
        { 
            { "Nam", Gender.Male }, 
            { "Nữ", Gender.Female }, 
            { "Khác", Gender.Other } 
        };
    }

    public static class RhesusMapping
    {
        public static readonly Dictionary<Rhesus, string> EnumToStringMap = new Dictionary<Rhesus, string>
        {
            { Rhesus.Negative, "Âm" },
            { Rhesus.Positive, "Dương" },
        };

        public static readonly Dictionary<string, Rhesus> StringToEnumMap = new Dictionary<string, Rhesus>
        {
            { "Âm", Rhesus.Negative },
            { "Dương", Rhesus.Positive },
        };
    }

    public static class CrossmatchMapping
    {
        public static readonly Dictionary<CrossmatchResult, string> EnumToStringMap = new Dictionary<CrossmatchResult, string>
        {
            { CrossmatchResult.Compatible, "Thuận hợp" },
            { CrossmatchResult.Incompatible, "Không thuận hợp" },
            { CrossmatchResult.Indeterminate, "Không xác định" }
        };

        public static readonly Dictionary<string, CrossmatchResult> StringToEnumMap = new Dictionary<string, CrossmatchResult>
        {
            { "Thuận hợp", CrossmatchResult.Compatible },
            { "Không thuận hợp", CrossmatchResult.Incompatible },
            { "Không xác định", CrossmatchResult.Indeterminate }
        };
    }

    public static class TestResulMapping
    {
        public static readonly Dictionary<TestResult, string> EnumToStringMap = new Dictionary<TestResult, string>
        {
            { TestResult.Negative, "Âm tính" },
            { TestResult.Positive, "Dương tính" },
        };

        public static readonly Dictionary<string, TestResult> StringToEnumMap = new Dictionary<string, TestResult>
        {
            { "Âm tính", TestResult.Negative },
            { "Dương tính", TestResult.Positive },
        };
    }

    #endregion

    #region Color mapping

    public static class BloodGroupColorMapping
    {
        public static Dictionary<BloodGroup, string> BloodGroupColors = new Dictionary<BloodGroup, string>
        {
            { BloodGroup.A, "#DF2D25" }, // Đỏ
            { BloodGroup.B, "#45B9EC" }, // Xanh
            { BloodGroup.AB, "#9B4BA6" }, // Tím
            { BloodGroup.O, "#F9D423" }  // Vàng
        };

        public static string GetColor(BloodGroup bloodGroup)
        {
            return BloodGroupColors.TryGetValue(bloodGroup, out var color) ? color : "Black";
        }
    }

    public static class RhesusColorMapping
    {
        public static Dictionary<Rhesus, string> RhesusColors = new Dictionary<Rhesus, string>
        {
            { Rhesus.Positive, "#62C99E" }, // Rêu
            { Rhesus.Negative, "#F79646" }  // Cam
        };

        public static string GetColor(Rhesus rhesus)
        {
            return RhesusColors.TryGetValue(rhesus, out var color) ? color : "Black";
        }
    }

    #endregion
}
