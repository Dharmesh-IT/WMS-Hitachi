using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WMS.Data.Mapping.Process
{
    public partial class ItemSerialDetailsMap : WMSEntityTypeConfiguration<ItemSerialDetailsDb>
    {
        #region Methods
        public override void Configure(EntityTypeBuilder<ItemSerialDetailsDb> builder)
        {
            builder.ToTable("ItemSerialDetails", "WMS");
            builder.HasKey(x => x.Id);

            base.Configure(builder);
        }
        #endregion
    }
}
