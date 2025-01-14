using BloodHub.Shared.Entities;
using BloodHub.Shared.Extentions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BloodHub.Data.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            // Cấu hình ValueConverter cho thuộc tính Gender
            var genderConverter = EnumConverter.CreateEnumToStringConverter(
                GenderMapping.EnumToStringMap, GenderMapping.StringToEnumMap);
            // Áp dụng ValueConverter cho thuộc tính Gender
            builder.Property(p => p.Gender).HasConversion(genderConverter);

            // Cấu hình ValueConverter cho thuộc tính BloodGroup
            var bloodGroupConverter = new ValueConverter<BloodGroup, string>(
                v => v.ToString(),
                v => (BloodGroup)Enum.Parse(typeof(BloodGroup), v)
                );
            // Áp dụng ValueConverter cho thuộc tính BloodGroup
            builder.Property(p => p.BloodGroup).HasConversion(bloodGroupConverter);

            // Cấu hình ValueConverter cho thuộc tính Rhesus
            var rhesusConverter = EnumConverter.CreateEnumToStringConverter(
                RhesusMapping.EnumToStringMap, RhesusMapping.StringToEnumMap);
            // Áp dụng ValueConverter cho thuộc tính Rhesus
            builder.Property(p => p.Rhesus).HasConversion(rhesusConverter);
        }
    }
}
