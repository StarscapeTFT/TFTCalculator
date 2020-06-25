# TFTCalculator

<img src="/Doc/screenshot.png" align="right" width="379" height="334" />

TFTCalculator is an interactive tool for calculating the probability of finding units in Teamfight Tactics. This program allows you to:

* Search for different combinations of up to four units
* Calculate the exact probabilities of rolling
* Compare rolling at different levels.

### Download

TFTCalculator requires Windows 7 SP1 or later. Sorry Mac players :(.

No installation required. Simply download the executable and run it. The Windows Defender Smartscreen may tell you it prevented an unrecognized app from starting. In that case, click on "More Info" and then "Run anyway".

**[64-bit](https://github.com/StarscapeTFT/TFTCalculator/releases/latest/download/TFTCalculator.exe):** Try this download first. [Requires the .NET Framework v4.7.2 or later.](https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net472-web-installer) Your computer probably already has this if Windows is up to date.

**[64-bit with .NET built in](https://github.com/StarscapeTFT/TFTCalculator/releases/latest/download/TFTCalculatorStandalone.exe):** WARNING: GIANT FILE. This version is self-contained with the entire .NET Core 3.1 runtime. Use this if you are unable (or too lazy) to install the .NET Framework.

**[32-bit](https://github.com/StarscapeTFT/TFTCalculator/releases/latest/download/TFTCalculator32bit.exe):** Note: This version is much slower than the 64-bit version because it does not use optimized instruction sets like SSE, AVX, etc. Please use this only if the 64-bit version doesn't work for you. [Requires the .NET Framework v4.7.2 or later.](https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net472-web-installer)

### Exact vs Approximate

Exact mode calculates the exact probability of finding multiple units, fully accounting for the statistical dependence between the units:

* Each unit is successively less likely to find
* Buying one unit improves the odds of finding other units of the same tier
* No more than 5 of your desired units can appear in a shop

Because the probability of finding units is not independent, the exact calculation time scales exponentially with the number of units. If you are searching for 3 each of 3 different units, then there are 4×4×4 = 64 possible results (3 + 1 for finding none). The probabilities are described by a Markov chain, and the final calculation involves finding the powers of a 64×64 matrix. This pattern gets out of control for larger numbers of units: searching for 9 each of 4 different units involves calculating the powers of a 10000×10000 matrix!

Approximate mode simplifies the calculations by treating each unit as statistically independent. Instead of using a 10000×10000 matrix, approximate mode breaks the calculation into four 10×10 matrices, then combines the results afterwards. Although it is less accurate, the calculations are nearly instant and the results are generally within 1% of the exact mode, so don't be afraid to use it!

### License

The source code for TFTCalculator is released under the MIT License. For convenience, this repository includes binaries for the dependencies OxyPlot and OpenBLAS. OxyPlot is distributed under the MIT license. **OpenBLAS is distributed under the BSD license.**

### Building from source

1. Download the latest version of [Visual Studio 2019](https://visualstudio.microsoft.com/vs/). The free Community Edition will work fine.
2. Download this repository.
3. Open the TFTCalculator.sln solution file with Visual Studio and build! It should work without fiddling with the dependencies.

The first time you open the solution file, Visual Studio will download two dependencies from NuGet:

* Octokit, used to check for updates.
* Fody/Costura, used to produce single-click executables.

The source distribution includes binaries for:

* [OpenBLAS](https://www.openblas.net/), used for high performance multithreaded calculations using the SSE and AVX instruction sets.
* [OxyPlot](https://github.com/oxyplot/oxyplot), used for displaying the final curves. This is included as a binary instead of a NuGet package because, at the time of this release, the latest NuGet package has several bugs related to DPI scaling that are fixed in the most up-to-date source code.

The TFTCalculator solution contains two projects:

* TFTCalculatorMiniBLAS, a thin C++/CLI wrapper around the OpenBLAS library. This project also simplifies the allocation of 64-bit aligned blocks of memory for better performance.
* TFTCalculator, the main application based on the Windows Presentation Foundation.

### Legal stuff

Teamfight Tactics is a registered trademark of Riot Games, Inc. Neither TFTCalculator nor its author are affiliated or endorsed by Riot Games.
