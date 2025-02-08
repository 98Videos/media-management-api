using MediaManagement.Api.Contracts.Validators;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace MediaManagement.UnitTests.Drivers.Api.Contracts.Validators
{
    public class VideoFormValidatorTests
    {
        [Test]
        public void Validate_WhenFileIsNull_ShouldReturnFalse()
        {
            // Arrange
            IFormFile? formFile = null;

            // Act
            var isValid = VideoFormFileValidator.Validate(formFile);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WhenFileIsEmpty_ShouldReturnFalse()
        {
        }

        [TestCase("virus.exe")]
        [TestCase("not_a_video.txt")]
        [TestCase("no extension")]
        [TestCase("dog.png")]
        public void Validate_WhenFileIsNotAnAcceptedVideoFormat_ShouldReturnFalse(string fileName)
        {
            // Arrange
            var formFile = new FormFile(new MemoryStream(), 0, 10, Path.GetFileNameWithoutExtension(fileName), fileName);

            // Act
            var isValid = VideoFormFileValidator.Validate(formFile);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestCase("video.webm")]
        [TestCase("video.mkv")]
        [TestCase("video.flv")]
        [TestCase("video.vob")]
        [TestCase("video.ogv")]
        [TestCase("video.ogg")]
        [TestCase("video.drc")]
        [TestCase("video.avi")]
        [TestCase("video.MTS")]
        [TestCase("video.M2TS")]
        [TestCase("video.mov")]
        [TestCase("video.qt")]
        [TestCase("video.qt")]
        [TestCase("video.wmv")]
        [TestCase("video.yuv")]
        [TestCase("video.yuv")]
        [TestCase("video.rm")]
        [TestCase("video.rmvb")]
        [TestCase("video.viv")]
        [TestCase("video.amv")]
        [TestCase("video.mp4")]
        [TestCase("video.m4p")]
        [TestCase("video.m4v")]
        [TestCase("video.mpg")]
        [TestCase("video.mp2")]
        [TestCase("video.mpeg")]
        [TestCase("video.mpe")]
        [TestCase("video.mpv")]
        [TestCase("video.m2v")]
        [TestCase("video.m4v")]
        [TestCase("video.3gp")]
        [TestCase("video.3g2")]
        [TestCase("video.roq")]
        [TestCase("video.flv")]
        [TestCase("video.f4v")]
        [TestCase("video.f4p")]
        [TestCase("video.f4a")]
        [TestCase("video.f4b")]
        public void Validate_WhenFileIsAnAcceptedVideoFormat_ShouldReturnTrue(string fileName)
        {
            // Arrange
            var formFile = new FormFile(new MemoryStream(), 0, 10, Path.GetFileNameWithoutExtension(fileName), fileName);

            // Act
            var isValid = VideoFormFileValidator.Validate(formFile);

            // Assert
            Assert.IsTrue(isValid);
        }
    }
}