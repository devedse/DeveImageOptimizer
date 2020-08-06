using DeveCoolLib.ProcessAsTask;
using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DeveImageOptimizer.ImageOptimization
{
    public class ImageOptimizationStep
    {
        private readonly string _toolExePath;
        public string Arguments { get; }

        public string ToolExeFileName => Path.GetFileName(_toolExePath);

        public const string InputFileToken = "%INPUTFILETOKEN%";
        public const string OutputFileToken = "%OUTPUTFILETOKEN%";

        /// <summary>
        /// A step to execute in the image optimization process
        /// </summary>
        /// <param name="toolExePath">The path to the .exe file of the optimizer</param>
        /// <param name="arguments">The arguments to pass to the tool, any temp file arguments should be tokenized as follows: ImageOptimizationStep.TempFileToken</param>
        public ImageOptimizationStep(string toolExePath, string arguments)
        {
            _toolExePath = toolExePath;
            Arguments = arguments;
        }

        public async Task<ImageOptimizationStepResult> Run(DeveImageOptimizerConfiguration configuration, string imagePath, List<string> tempFiles)
        {
            var extension = Path.GetExtension(imagePath);
            var inputFile = Path.Combine(configuration.TempDirectory, RandomFileNameHelper.RandomFileNameShort(Path.GetFileNameWithoutExtension(ToolExeFileName), extension));
            tempFiles.Add(inputFile);

            File.Copy(imagePath, inputFile, true);
            string args = Arguments;

            args = args.Replace(InputFileToken, inputFile);

            bool usesTempFile = Arguments.Contains(OutputFileToken);
            string tempFilePath = "";
            if (usesTempFile)
            {
                tempFilePath = Path.Combine(configuration.TempDirectory, RandomFileNameHelper.RandomFileNameShort(Path.GetFileNameWithoutExtension($"{ToolExeFileName}_out"), extension));
                tempFiles.Add(tempFilePath);
                args = args.Replace(OutputFileToken, tempFilePath);
            }

            var processResult = await ProcessRunner.RunAsyncAndLogToConsole(_toolExePath, args);

            return new ImageOptimizationStepResult(processResult, usesTempFile ? tempFilePath : inputFile);
        }
    }
}
