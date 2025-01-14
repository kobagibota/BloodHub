using BloodHub.Shared.Entities;
using BloodHub.Shared.Extentions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodHub.Data.Configurations
{
    public class CrossmatchConfiguration : IEntityTypeConfiguration<Crossmatch>
    {
        public void Configure(EntityTypeBuilder<Crossmatch> builder)
        {
            // Cấu hình ValueConverter cho thuộc tính Result
            var resultConverter = EnumConverter.CreateEnumToStringConverter(
                CrossmatchMapping.EnumToStringMap, CrossmatchMapping.StringToEnumMap);
            // Áp dụng ValueConverter cho thuộc tính Result
            builder.Property(p => p.Result).HasConversion(resultConverter);

            // Cấu hình ValueConverter cho thuộc tính các ống
            var testReulstConverter = EnumConverter.CreateEnumToStringConverter(
                TestResulMapping.EnumToStringMap, TestResulMapping.StringToEnumMap);

            // Áp dụng ValueConverter cho thuộc tính ResultSaline1
            builder.Property(p => p.ResultSaline1).HasConversion(testReulstConverter);
            builder.Property(p => p.ResultSaline2).HasConversion(testReulstConverter);
            builder.Property(p => p.ResultAHG1).HasConversion(testReulstConverter);
            builder.Property(p => p.ResultAHG2).HasConversion(testReulstConverter);

        }
    }
}
