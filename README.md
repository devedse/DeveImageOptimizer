# DeveImageOptimizer
This tool uses FileOptimizer to recompress images: [FileOptimizer](http://nikkhokkho.sourceforge.net/static.php?page=FileOptimizer). After this it does a Pixel by Pixel comparison to verify if the images are still equal. If they are, they can then be used by other projects.

This project is the source for the github package [DeveImageOptimizer](https://www.nuget.org/packages/DeveImageOptimizer/). It's currently being used in my [WebOptimizationProject](https://github.com/devedse/WebOptimizationProject/) and [DeveImageOptimizerWPF](https://github.com/devedse/DeveImageOptimizerWPF/)

## Build status

| GitHubActions Builds |
|:--------------------:|
| [![GitHubActions Builds](https://github.com/devedse/DeveImageOptimizer/workflows/GitHubActionsBuilds/badge.svg)](https://github.com/devedse/DeveImageOptimizer/actions/workflows/githubactionsbuilds.yml) |

## Code Coverage status

| CodeCov |
|:-------:|
| [![codecov](https://codecov.io/gh/devedse/DeveImageOptimizer/branch/master/graph/badge.svg)](https://codecov.io/gh/devedse/DeveImageOptimizer) |

(As of 21-02-2022 image optimization is also done in the unit tests which greatly improved the code coverage)

## Code Quality Status

| SonarQube |
|:---------:|
| [![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=DeveImageOptimizer&metric=alert_status)](https://sonarcloud.io/dashboard?id=DeveImageOptimizer) |

## Package

| NuGet |
|:-----:|
| [![NuGet](https://img.shields.io/nuget/v/DeveImageOptimizer.svg)](https://www.nuget.org/packages/DeveImageOptimizer/) |

## Current Implementation status

| Number | Description | Implementation progress |
| --- | --- | --- |
| 1 | It will remember all processed files, so it won't reprocess them again if they have already been optimized. | 100% |
| 2 | It will do a pixel for pixel comparison between the input/output and only replace the original if it matches 100% (this is just to be sure the image did not get corrupted) | 100% |
| 3 | If you select a folder it will only take PNG's, GIF's, JPEG's and BMP's from that folder to optimize, no other formats will be included | 100% |
| 4 | Automated builds / release | GitHub Actions: 100% |
