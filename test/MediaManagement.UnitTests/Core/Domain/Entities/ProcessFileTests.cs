using MediaManagementApi.Domain.Entities;
using Moq;
using NUnit.Framework;

namespace MediaManagement.UnitTests.Core.Domain.Entities;

[TestFixture]
public class ProcessFileTests
{
    [Test]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        var identifier = "test-file";
        var mockStream = new Mock<Stream>().Object;

        var processFile = new ProcessFile(identifier, mockStream);

        Assert.AreEqual(identifier, processFile.Identifier);
        Assert.AreEqual(mockStream, processFile.FileStreamReference);
    }

    [Test]
    public void Constructor_ShouldSetIdentifierCorrectly()
    {
        var identifier = "file-123";
        var mockStream = new Mock<Stream>().Object;

        var processFile = new ProcessFile(identifier, mockStream);
        
        Assert.AreEqual("file-123", processFile.Identifier);
    }

    [Test]
    public void Constructor_ShouldSetFileStreamReferenceCorrectly()
    {
        var mockStream = new Mock<Stream>().Object;

        var processFile = new ProcessFile("file-123", mockStream);

        Assert.NotNull(processFile.FileStreamReference);
        Assert.AreEqual(mockStream, processFile.FileStreamReference);
    }
}