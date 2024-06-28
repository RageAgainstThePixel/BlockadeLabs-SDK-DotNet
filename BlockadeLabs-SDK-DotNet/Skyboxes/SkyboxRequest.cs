using System;
using System.IO;

namespace BlockadeLabsSDK
{
    public sealed class SkyboxRequest : IDisposable
    {
        /// <summary>
        /// Creates a new Skybox Request.
        /// </summary>
        /// <param name="style">
        /// The predefined style that influences the overall aesthetic of your skybox generation.
        /// </param>
        /// <param name="prompt">
        /// Text prompt describing the skybox world you wish to create.
        /// Maximum number of characters: 550.
        /// If you are using <see cref="SkyboxStyle"/> then the maximum number of characters is defined
        /// in the max-char response parameter defined for each style.
        /// </param>
        /// <param name="negativeText">
        /// Describe things to avoid in the skybox world you wish to create.
        /// Maximum number of characters: 200.
        /// If you are using <see cref="SkyboxStyle"/> then the maximum number of characters is defined
        /// in the negative-text-max-char response parameter defined for each style.
        /// </param>
        /// <param name="enhancePrompt">
        /// Have an AI automatically improve your prompt to generate pro-level results every time (default: false)
        /// </param>
        /// <param name="seed">
        /// Send 0 for a random seed generation.
        /// Any other number (1-2147483647) set will be used to "freeze" the image generator and
        /// create similar images when run again with the same seed and settings.
        /// </param>
        /// <param name="remixImagineId">
        /// ID of a previously generated skybox.
        /// </param>
        /// <param name="webhookUrl">
        /// Optional, specify a webhook url to specify the destination for progress updates.
        /// </param>
        /// <param name="hqDepth">
        /// Request for high quality depth map. It will be returned in the depth_map_url parameter. (default: false)
        /// </param>
        public SkyboxRequest(
            SkyboxStyle style,
            string prompt,
            string negativeText = null,
            bool? enhancePrompt = null,
            int? seed = null,
            int? remixImagineId = null,
            string webhookUrl = null,
            bool? hqDepth = null)
        {
            Prompt = prompt;
            NegativeText = negativeText;
            EnhancePrompt = enhancePrompt;
            Seed = seed;
            SkyboxStyleId = style;
            RemixImagineId = remixImagineId;
            WebhookUrl = webhookUrl;
            HqDepth = hqDepth;
        }

        /// <summary>
        /// Creates a new Skybox Request.
        /// </summary>
        /// <param name="style">
        /// The predefined style that influences the overall aesthetic of your skybox generation.
        /// </param>
        /// <param name="prompt">
        /// Text prompt describing the skybox world you wish to create.
        /// Maximum number of characters: 550.
        /// If you are using <see cref="SkyboxStyle"/> then the maximum number of characters is defined
        /// in the max-char response parameter defined for each style.
        /// </param>
        /// <param name="controlImagePath">
        /// File path to the control image for the request.
        /// </param>
        /// <param name="negativeText">
        /// Describe things to avoid in the skybox world you wish to create.
        /// Maximum number of characters: 200.
        /// If you are using <see cref="SkyboxStyle"/> then the maximum number of characters is defined
        /// in the negative-text-max-char response parameter defined for each style.
        /// </param>
        /// <param name="enhancePrompt">
        /// Have an AI automatically improve your prompt to generate pro-level results every time (default: false)
        /// </param>
        /// <param name="seed">
        /// Send 0 for a random seed generation.
        /// Any other number (1-2147483647) set will be used to "freeze" the image generator and
        /// create similar images when run again with the same seed and settings.
        /// </param>
        /// <param name="remixImagineId">
        /// ID of a previously generated skybox.
        /// </param>
        /// <param name="webhookUrl">
        /// Optional, specify a webhook url to specify the destination for progress updates.
        /// </param>
        /// <param name="hqDepth">
        /// Request for high quality depth map. It will be returned in the depth_map_url parameter. (default: false)
        /// </param>
        public SkyboxRequest(
            SkyboxStyle style,
            string prompt,
            string controlImagePath,
            string negativeText = null,
            bool? enhancePrompt = null,
            int? seed = null,
            int? remixImagineId = null,
            string webhookUrl = null,
            bool? hqDepth = null)
            : this(
                style,
                prompt,
                File.OpenRead(controlImagePath),
                Path.GetFileName(controlImagePath),
                negativeText,
                enhancePrompt,
                seed,
                remixImagineId,
                webhookUrl,
                hqDepth)
        {
        }

        /// <summary>
        /// Creates a new Skybox Request.
        /// </summary>
        /// <param name="style">
        /// The id of predefined style that influences the overall aesthetic of your skybox generation.
        /// </param>
        /// <param name="prompt">
        /// Text prompt describing the skybox world you wish to create.
        /// Maximum number of characters: 550.
        /// If you are using <see cref="SkyboxStyle"/> then the maximum number of characters is defined
        /// in the max-char response parameter defined for each style.
        /// </param>
        /// <param name="controlImage">
        /// <see cref="Stream"/> data of control image for request.
        /// </param>
        /// <param name="controlImageFileName">
        /// File name of <see cref="controlImage"/>.
        /// </param>
        /// <param name="negativeText">
        /// Describe things to avoid in the skybox world you wish to create.
        /// Maximum number of characters: 200.
        /// If you are using <see cref="SkyboxStyle"/> then the maximum number of characters is defined
        /// in the negative-text-max-char response parameter defined for each style.
        /// </param>
        /// <param name="enhancePrompt">
        /// Have an AI automatically improve your prompt to generate pro-level results every time (default: false)
        /// </param>
        /// <param name="seed">
        /// Send 0 for a random seed generation.
        /// Any other number (1-2147483647) set will be used to "freeze" the image generator and
        /// create similar images when run again with the same seed and settings.
        /// </param>
        /// <param name="remixImagineId">
        /// ID of a previously generated skybox.
        /// </param>
        /// <param name="webhookUrl">
        /// Optional, specify a webhook url to specify the destination for progress updates.
        /// </param>
        /// <param name="hqDepth">
        /// Request for high quality depth map. It will be returned in the depth_map_url parameter. (default: false)
        /// </param>
        public SkyboxRequest(
            SkyboxStyle style,
            string prompt,
            Stream controlImage,
            string controlImageFileName,
            string negativeText = null,
            bool? enhancePrompt = null,
            int? seed = null,
            int? remixImagineId = null,
            string webhookUrl = null,
            bool? hqDepth = null)
            : this(style, prompt, negativeText, enhancePrompt, seed, remixImagineId, webhookUrl, hqDepth)
        {
            ControlImage = controlImage;

            if (string.IsNullOrWhiteSpace(controlImageFileName))
            {
                const string defaultImageName = "control_image.png";
                controlImageFileName = defaultImageName;
            }

            ControlImageFileName = controlImageFileName;
            const string scribble = nameof(scribble);
            const string remix = nameof(remix);
            ControlModel = style.Model == SkyboxModel.Model2 ? scribble : remix;
        }

        ~SkyboxRequest() => Dispose(false);

        /// <summary>
        /// Text prompt describing the skybox world you wish to create.
        /// Maximum number of characters: 550.
        /// If you are using <see cref="SkyboxStyle"/> then the maximum number of characters is defined
        /// in the max-char response parameter defined for each style.
        /// </summary>
        public string Prompt { get; }

        /// <summary>
        /// Describe things to avoid in the skybox world you wish to create.
        /// Maximum number of characters: 200.
        /// If you are using <see cref="SkyboxStyle"/> then the maximum number of characters is defined
        /// in the negative-text-max-char response parameter defined for each style.
        /// </summary>
        public string NegativeText { get; }

        /// <summary>
        /// Have an AI automatically improve your prompt to generate pro-level results every time (default: false)
        /// </summary>
        public bool? EnhancePrompt { get; }

        /// <summary>
        /// Send 0 for a random seed generation.
        /// Any other number (1-2147483647) set will be used to "freeze" the image generator and
        /// create similar images when run again with the same seed and settings.
        /// </summary>
        public int? Seed { get; }

        /// <summary>
        /// The id of predefined style that influences the overall aesthetic of your skybox generation.
        /// </summary>
        public int? SkyboxStyleId { get; }

        /// <summary>
        /// ID of a previously generated skybox.
        /// </summary>
        public int? RemixImagineId { get; }

        /// <summary>
        /// Optional webhook url to specify the destination for progress updates
        /// </summary>
        public string WebhookUrl { get; }

        /// <summary>
        /// Request for high quality depth map. It will be returned in the depth_map_url parameter. (default: false)
        /// </summary>
        public bool? HqDepth { get; }

        /// <summary>
        /// Control image used to influence the generation.
        /// The image needs to be exactly 1024 pixels wide and 512 pixels tall PNG equirectangular projection image
        /// of a scribble with black background and white brush strokes.
        /// </summary>
        public Stream ControlImage { get; }

        /// <summary>
        /// File name of <see cref="ControlImage"/>.
        /// </summary>
        public string ControlImageFileName { get; }

        /// <summary>
        /// Model used for the <see cref="ControlImage"/>.
        /// Currently, the only options are: "scribble" for model 2 and "remix" for model 3.
        /// </summary>
        internal string ControlModel { get; set; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                ControlImage?.Close();
                ControlImage?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
