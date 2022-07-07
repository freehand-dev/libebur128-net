// See https://aka.ms/new-console-template for more information
using libebur128_net;
using libebur128_net.libebur128;
using NAudio.Wave;
using System.Runtime.InteropServices;

Console.WriteLine("Current directory: " + Environment.CurrentDirectory);
Console.WriteLine("Runnung in {0}-bit mode.", Environment.Is64BitProcess ? "64" : "32");

string filename = args[0];
Console.WriteLine($"FileName = { filename }");
Console.WriteLine($"libebur128 v.{libebur128_net.Ebur128.Version}");

if (!File.Exists(filename))
{
    throw new ApplicationException($"The file does not exist.");
}

using (WaveFileReader reader = new WaveFileReader(Path.Combine(filename)))
{
    using (libebur128_net.Ebur128 ebur128 = libebur128_net.Ebur128.Init(
        (uint)reader.WaveFormat.Channels, 
        (uint)reader.WaveFormat.SampleRate,
        libebur128Native.mode.EBUR128_MODE_I |
        libebur128Native.mode.EBUR128_MODE_TRUE_PEAK |
        libebur128Native.mode.EBUR128_MODE_HISTOGRAM |
        libebur128Native.mode.EBUR128_MODE_LRA))
    {
        /* example: set channel map (note: see ebur128.h for the default map) */
        if (reader.WaveFormat.Channels == 5)
        {
            ebur128.SetChannelType(0, libebur128Native.channel.EBUR128_LEFT);
            ebur128.SetChannelType(1, libebur128Native.channel.EBUR128_RIGHT);
            ebur128.SetChannelType(2, libebur128Native.channel.EBUR128_CENTER);
            ebur128.SetChannelType(3, libebur128Native.channel.EBUR128_LEFT_SURROUND);
            ebur128.SetChannelType(4, libebur128Native.channel.EBUR128_RIGHT_SURROUND);
        }

        for (int i = 0; i < reader.SampleCount; i++)
        {
            Span<float> sampleFrame = reader.ReadNextSampleFrame();
            ebur128.AddFramesFloat(sampleFrame, (uint)1);
        }

        Console.WriteLine("");
        Console.WriteLine("Summary:");

        Console.WriteLine("");
        Console.WriteLine("  Integrated loudness:");
        Console.WriteLine(String.Format("    {0,-10} {1:0.#} LUFS", "I:", ebur128.LoudnessGlobal()));
        Console.WriteLine(String.Format("    {0,-10} {1:0.#} LUFS", "Threshold:", ebur128.RelativeThreshold()));

        Console.WriteLine("");
        Console.WriteLine("  Loudness range:");
        Console.WriteLine(String.Format("    {0,-10} {1:0.#} LU", "LRA:", ebur128.LoudnessRange()));


        Console.WriteLine("");
        Console.WriteLine("  True peak:");
        var maxTruePeak = 20 * Math.Log10(ebur128.AbsoluteTruePeak());
        Console.WriteLine(String.Format("    {0,-10} {1:0.#} dBTP", "Peak:", maxTruePeak));

        Console.WriteLine("");
        Console.WriteLine("");
    }
}

Console.WriteLine("Press enter to exit.");
Console.ReadLine();











