# PerlinNoiseHash
Instead of using classic Perlin Noise, generate a gradient permutation table first, then use Xorshift hash function to determine index.

The default frequency is set to 0.03, and the step size is 0.003.

## MonoGame Version
3.8.4

## Platform
Windows (Desktop GL)

## Usage
Press "N" key to generate a new map.\
Press NumPad "+" key to increase the frequency.\
Press NumPad "-" key to decrease the frequency.\
Scroll mouse wheel to rapidly increase or decrease the frequency.
