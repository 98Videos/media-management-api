namespace MediaManagement.Api.Contracts.Validators
{
    public static class VideoFormFileValidator
    {
        private static readonly string[] acceptedVideoExtensions = [
            ".webm",
            ".mkv",
            ".flv",
            ".vob",
            ".ogv",
            ".ogg",
            ".drc",
            ".avi",
            ".MTS",
            ".M2TS",
            ".mov",
            ".qt",
            ".qt",
            ".wmv",
            ".yuv",
            ".yuv",
            ".rm",
            ".rmvb",
            ".viv",
            ".amv",
            ".mp4",
            ".m4p",
            ".m4v",
            ".mpg",
            ".mp2",
            ".mpeg",
            ".mpe",
            ".mpv",
            ".m2v",
            ".m4v",
            ".3gp",
            ".3g2",
            ".roq",
            ".flv",
            ".f4v",
            ".f4p",
            ".f4a",
            ".f4b"
        ];


        public static bool Validate(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            return IsAcceptedVideoFormat(file);
        }

        private static bool IsAcceptedVideoFormat(IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            return acceptedVideoExtensions.Contains(fileExtension);
        }
    }
}