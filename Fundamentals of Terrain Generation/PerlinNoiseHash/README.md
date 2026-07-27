# PerlinNoiseHash
Instead of using classic Perlin Noise, generate a gradient permutation table first, then use Xorshift hash function to determine index.

The default frequency is set to 0.03, and the step size is 0.01.

## MonoGame Version
3.8.5

## Platform
Windows (Desktop GL)

## Usage
Press "N" key to generate a new map.\
Press "D" key to toggle whether domain warping is applied.\
Press "F" key to toggle whether fractal brownian motion is applied.\
Press NumPad "+" key to increase the frequency.\
Press NumPad "-" key to decrease the frequency.
