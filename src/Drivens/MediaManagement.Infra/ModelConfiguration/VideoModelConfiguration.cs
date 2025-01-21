using MediaManagementApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaManagement.Database.ModelConfiguration;

internal class VideoModelConfiguration: IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Filename).IsRequired();
        builder.Property(x => x.EmailUser).IsRequired();
    }
}