// See https://aka.ms/new-console-template for more information
using libebur128_net;
using libebur128_net.libebur128;
using NAudio.Wave;
using System.Runtime.InteropServices;
using Mode = libebur128_net.libebur128.libebur128Native.mode;



Console.WriteLine("Current directory: " + Environment.CurrentDirectory);
Console.WriteLine("Runnung in {0}-bit mode.", Environment.Is64BitProcess ? "64" : "32");

Console.WriteLine("Press enter to continue.");
Console.ReadLine();


string filename = args[0];

Console.WriteLine($"FileName = { filename }");
Console.WriteLine($"libebur128 v.{libebur128_net.Ebur128.Version}");


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


        Console.WriteLine(String.Format("Integrated: {0,10:0.##} LUFS",
            ebur128.LoudnessGlobal()));

        Console.WriteLine(String.Format("Threshold: {0,10:0.##} LUFS",
            ebur128.RelativeThreshold()));

        Console.WriteLine(String.Format("LRA: {0,10:0.##} LU",
            ebur128.LoudnessRange()));

        Console.WriteLine(String.Format("Momentary max: {0,10:0.##} LUFS",
            ebur128.LoudnessMomentary()));

        Console.WriteLine(String.Format("Short-term max: {0,10:0.##} LUFS",
            ebur128.LoudnessShortterm()));

        var maxTruePeak = 20 * Math.Log10(ebur128.AbsoluteTruePeak());
        Console.WriteLine(String.Format("True Peak: {0,10:0.##} dBTP", maxTruePeak));

        var maxSamplePeak = 20 * Math.Log10(ebur128.AbsoluteSamplePeak());
        Console.WriteLine(String.Format("Sample Peak: {0,10:0.##} dBFS", maxSamplePeak));
    }
}

Console.WriteLine("Press enter to exit.");
Console.ReadLine();











