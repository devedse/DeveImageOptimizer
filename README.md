# DeveImageOptimizer
This tool uses FileOptimizer to recompress images: http://nikkhokkho.sourceforge.net/static.php?page=FileOptimizer

## Build status

| AppVeyor (Windows build) |
|:------------------------:|
| [![Build status](https://ci.appveyor.com/api/projects/status/qo0hx7i9j2hrlmpr?svg=true)](https://ci.appveyor.com/project/devedse/deveimageoptimizer) |

## Code Coverage status

| CodeCov |
|:-------:|
| [![codecov](https://codecov.io/gh/devedse/DeveImageOptimizer/branch/master/graph/badge.svg)](https://codecov.io/gh/devedse/DeveImageOptimizer) |

(Reason why Coverage is quite low is because a lot of tests can't run on the Build Server)

## More information

This project is the source for the github package [DeveImageOptimizer](https://www.nuget.org/packages/DeveImageOptimizer/). It's currently being used in my [WebOptimizationProject](devedse/WebOptimizationProject) and [DeveImageOptimizerWPF](devedse/DeveImageOptimizerWPF)

| Number | Description | Implementation progress |
| --- | --- | --- |
| 1 | It will remember all processed files, so it won't reprocess them again if they have already been optimized. | 100% |
| 2 | It will do a pixel for pixel comparison between the input/output and only replace the original if it matches 100% (this is just to be sure the image did not get corrupted) | 100% |
| 3 | If you select a folder it will only take PNG's, GIF's, JPEG's and BMP's from that folder to optimize, no other formats will be included | 100% |
| 4 | Automated builds / release | AppVeyor: 100% |