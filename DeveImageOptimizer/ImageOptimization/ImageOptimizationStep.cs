﻿using DeveCoolLib.ProcessAsTask;
using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DeveImageOptimizer.ImageOptimization
{
    public class ImageOptimizationStep
    {
        private readonly string _toolExePath;
        private readonly bool _replaceToWindowsSlashOnNonWindows;

        public string Arguments { get; }

        public string ToolExeFileName => Path.GetFileName(_toolExePath);

        public const string InputFileToken = "%INPUTFILETOKEN%";
        public const string OutputFileToken = "%OUTPUTFILETOKEN%";

        /// <summary>
        /// A step to execute in the image optimization process
        /// </summary>
        /// <param name="toolExePath">The path to the .exe file of the optimizer</param>
        /// <param name="arguments">The arguments to pass to the tool, any temp file arguments should be tokenized as follows: ImageOptimizationStep.TempFileToken</param>
        /// <param name="replaceToWindowsSlashOnNonWindows">For some tools when running in Linux you want to replace slashes</param>
        public ImageOptimizationStep(string toolExePath, string arguments, bool replaceToWindowsSlashOnNonWindows)
        {
            _toolExePath = toolExePath;
            Arguments = arguments;
            _replaceToWindowsSlashOnNonWindows = replaceToWindowsSlashOnNonWindows;
        }

        private string ReplaceSlashesIfneeded(string input)
        {
            if (_replaceToWindowsSlashOnNonWindows && !OperatingSystem.IsWindows())
            {
                return "z:" + input.Replace('/', '\\');
            }
            return input;
        }

        public async Task<ImageOptimizationStepResult> Run(DeveImageOptimizerConfiguration configuration, string imagePath, List<string> tempFiles)
        {
            var extension = Path.GetExtension(imagePath);
            var inputFile = Path.Combine(configuration.TempDirectory, RandomFileNameHelper.RandomFileNameShort(Path.GetFileNameWithoutExtension(ToolExeFileName), extension));
            tempFiles.Add(inputFile);

            File.Copy(imagePath, inputFile, true);
            string args = Arguments;

            args = args.Replace(InputFileToken, ReplaceSlashesIfneeded(inputFile));

            bool usesTempFile = Arguments.Contains(OutputFileToken);
            string tempFilePath = "";
            if (usesTempFile)
            {
                tempFilePath = Path.Combine(configuration.TempDirectory, RandomFileNameHelper.RandomFileNameShort(Path.GetFileNameWithoutExtension($"{ToolExeFileName}_out"), extension));
                tempFiles.Add(tempFilePath);
                args = args.Replace(OutputFileToken, ReplaceSlashesIfneeded(tempFilePath));
            }

            ProcessStartInfo psi;

            if (OperatingSystem.IsWindows())
            {
                psi = new ProcessStartInfo(_toolExePath, args)
                {
                    //WorkingDirectory = Path.GetDirectoryName(_toolExePath)
                };
            }
            else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm || RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
            {
                psi = new ProcessStartInfo("/root/hangover/build/wine-host/loader/wine", $"/root/hangover/build/qemu/x86_64-windows-user/qemu-x86_64.exe.so \"{_toolExePath}\" {args}")
                {
                    //If you use a working directory, paths in Linux with wine don't work anymore
                    //WorkingDirectory = Path.GetDirectoryName(_toolExePath)
                };
            }
            else
            {
                psi = new ProcessStartInfo("wine", $"\"{_toolExePath}\" {args}")
                {
                    //If you use a working directory, paths in Linux with wine don't work anymore
                    //WorkingDirectory = Path.GetDirectoryName(_toolExePath)
                };
            }

            Console.WriteLine($"Executing: {psi.FileName} {psi.Arguments}");

            var processResult = await ProcessRunner.RunAsync(psi, configuration.ForwardOptimizerToolLogsToConsole, configuration.HideOptimizerWindow);

            return new ImageOptimizationStepResult(processResult, usesTempFile ? tempFilePath : inputFile);
        }

        public override string ToString()
        {
            return $"{ToolExeFileName} {Arguments}";
        }
    }
}
