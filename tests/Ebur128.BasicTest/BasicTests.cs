using NAudio.Wave;
using libebur128_net;
using libebur128_net.libebur128;
using System.Buffers.Binary;
using Mode = libebur128_net.libebur128.libebur128Native.mode;
using System.Reflection;
using Ebur128.BasicTest.Attributes;
using System.IO.Compression;

namespace Ebur128.BasicTest
{
    [TestCaseOrderer("Ebur128.BasicTest.Orderers.PriorityOrderer", "Ebur128.BasicTest")]
    public class BasicTests
    {
        private readonly ITestOutputHelper _output;

        private readonly string _url = "https://tech.ebu.ch/files/live/sites/tech/files/shared/testmaterial/ebu-loudness-test-setv05.zip";

        public string _tempararyPath;

        public BasicTests(ITestOutputHelper output)
        {
            this._output = output;

            this._tempararyPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(this._url));
        }

        /// <summary>
        /// Download and extract the test WAV files.
        /// 
        /// The latest data can be found here:
        /// https://tech.ebu.ch/publications/ebu_loudness_test_set
        /// I used to point to the official dataset hosted by the EBU, but they
        /// either throttled me or are getting hammered by other requests. For now, I
        /// am hosting a copy to get around the issue. If anyone has suggestions on
        /// dataset hosting for integration tests, I'm all ears.
        /// </summary>
        [Fact, TestPriority(10000)]
        [Trait("Category", "System")]
        public async Task DownloadLoudnessTestFiles()
        {
            await DownloadAndExtractZIP(this._url, this._tempararyPath);
            this._output.WriteLine($"Temparay files path is {this._tempararyPath}");
        }

        [Fact, TestPriority(-1)]
        [Trait("Category", "System")]
        public void CleanLoudnessTestFiles()
        {
            if (!Directory.Exists(this._tempararyPath))
                Directory.Delete(this._tempararyPath, true);
        }


        [Fact]
        [Trait("Category", "libEbur128")]
        public void VersionTest()
        {
            Ebur128Varsion ebur128Varsion = libebur128_net.Ebur128.Version;
            this._output.WriteLine($"libebur128 v.{ebur128Varsion}");
            Assert.NotNull(ebur128Varsion);
        }


        /// <summary>
        /// Test for the global loudness value of a single file.
        /// </summary>
        [Theory]
        [InlineData("seq-3341-1-16bit.wav", -23.0)]
        [InlineData("seq-3341-2-16bit.wav", -33.0)]
        [InlineData("seq-3341-3-16bit-v02.wav", -23.0)]
        [InlineData("seq-3341-4-16bit-v02.wav", -23.0)]
        [InlineData("seq-3341-5-16bit-v02.wav", -23.0)]
        [InlineData("seq-3341-6-5channels-16bit.wav", -23.0)]
        [InlineData("seq-3341-6-6channels-WAVEEX-16bit.wav", -23.0)]
        [InlineData("seq-3341-7_seq-3342-5-24bit.wav", -23.0)]
        [InlineData("seq-3341-2011-8_seq-3342-6-24bit-v02.wav", -23.0)]

        [Trait("Category", "libEbur128")]
        public void LoudnessGlobalTest(string filename, double value)
        {
            this._output.WriteLine($"FileName = {filename}");
            this._output.WriteLine($"libebur128 v.{libebur128_net.Ebur128.Version}");
            using (WaveFileReader reader = new WaveFileReader(Path.Combine(this._tempararyPath, filename)))
            {        
                using (libebur128_net.Ebur128 ebur128 = libebur128_net.Ebur128.Init((uint)reader.WaveFormat.Channels, (uint)reader.WaveFormat.SampleRate, Mode.EBUR128_MODE_I))
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

                    double actual = ebur128.LoudnessGlobal();
                    this._output.WriteLine($"LoudnessGlobal = {actual}");
                    Assert.Equal(value, actual, 1);
                }
            }
        }

        /// <summary>
        /// Test for the loudness range value of a single file.
        /// </summary>
        [Theory]
        [InlineData("seq-3342-1-16bit.wav", 10.0)]
        [InlineData("seq-3342-2-16bit.wav", 5.0)]
        [InlineData("seq-3342-3-16bit.wav", 20.0)]
        [InlineData("seq-3342-4-16bit.wav", 15.0)]
        [InlineData("seq-3341-7_seq-3342-5-24bit.wav", 5.0)]
        [InlineData("seq-3341-2011-8_seq-3342-6-24bit-v02.wav", 15.0)]
        [Trait("Category", "libEbur128")]
        public void LoudnessRangeTest(string filename, double value)
        {
            this._output.WriteLine($"FileName = {filename}");
            this._output.WriteLine($"libebur128 v.{libebur128_net.Ebur128.Version}");
            using (WaveFileReader reader = new WaveFileReader(Path.Combine(this._tempararyPath, filename)))
            {
                using (libebur128_net.Ebur128 ebur128 = libebur128_net.Ebur128.Init((uint)reader.WaveFormat.Channels, (uint)reader.WaveFormat.SampleRate, Mode.EBUR128_MODE_LRA))
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

                    double actual = ebur128.LoudnessRange();
                    this._output.WriteLine($"LoudnessRange = {actual}");
                    Assert.Equal(value, actual, 1);
                }
            }
        }

        /// <summary>
        /// Test for the maximum true loudness peak value of a single file.
        /// </summary>
        [Theory]
        [InlineData("seq-3341-15-24bit.wav.wav", -6.0)]
        [InlineData("seq-3341-16-24bit.wav.wav", -6.0)]
        [InlineData("seq-3341-17-24bit.wav.wav", -6.0)]
        [InlineData("seq-3341-18-24bit.wav.wav", -6.0)]
        [InlineData("seq-3341-19-24bit.wav.wav", 3.0)]
        [InlineData("seq-3341-20-24bit.wav.wav", -0.1)]
        [InlineData("seq-3341-21-24bit.wav.wav", -0.1)]
        [InlineData("seq-3341-22-24bit.wav.wav", -0.2)]
        [InlineData("seq-3341-23-24bit.wav.wav", -0.1)]
        [Trait("Category", "libEbur128")]
        public void AbsoluteTruePeakTest(string filename, double value)
        {
            this._output.WriteLine($"FileName = {filename}");
            this._output.WriteLine($"libebur128 v.{libebur128_net.Ebur128.Version}");
            using (WaveFileReader reader = new WaveFileReader(Path.Combine(this._tempararyPath, filename)))
            {
                using (libebur128_net.Ebur128 ebur128 = libebur128_net.Ebur128.Init((uint)reader.WaveFormat.Channels, (uint)reader.WaveFormat.SampleRate, Mode.EBUR128_MODE_TRUE_PEAK))
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

                    double actual = 20 * Math.Log10(ebur128.AbsoluteTruePeak());
                    this._output.WriteLine($"AbsoluteTruePeak = {actual}");
                    Assert.Equal(value, actual, 1);
                }
            }
        }

        /// <summary>
        /// Test for the loudness value of a single file in momentary (400ms) chunks.
        /// </summary>
        [Theory]
        [InlineData("seq-3341-13-1-24bit.wav", -23.0)]
        [InlineData("seq-3341-13-2-24bit.wav", -23.0)]
        [InlineData("seq-3341-13-3-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-4-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-5-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-6-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-7-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-8-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-9-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-10-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-11-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-12-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-13-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-14-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-15-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-16-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-17-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-18-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-19-24bit.wav.wav", -23.0)]
        [InlineData("seq-3341-13-20-24bit.wav.wav", -23.0)]
        [Trait("Category", "libEbur128")]
        public void LoudnessMomentaryTest(string filename, double value)
        {
            this._output.WriteLine($"FileName = {filename}");
            this._output.WriteLine($"libebur128 v.{libebur128_net.Ebur128.Version}");
            using (WaveFileReader reader = new WaveFileReader(Path.Combine(this._tempararyPath, filename)))
            {
                using (libebur128_net.Ebur128 ebur128 = libebur128_net.Ebur128.Init((uint)reader.WaveFormat.Channels, (uint)reader.WaveFormat.SampleRate, Mode.EBUR128_MODE_M))
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

                    double maxMomentary = float.MinValue;
                    for (int i = 0, refreshRate = 0; i < reader.SampleCount ; i++, refreshRate++)
                    {
                        Span<float> sampleFrame = reader.ReadNextSampleFrame();
                        ebur128.AddFramesFloat(sampleFrame, (uint)1);

                        //  10 ms buffer / 10 Hz refresh rate.
                        if (refreshRate >= reader.WaveFormat.SampleRate / 100)
                        {
                            // # Invalid results before the first 400 miliseconds.
                            if (i >= (4 * reader.WaveFormat.SampleRate / 10))
                            {
                                maxMomentary = Math.Max(ebur128.LoudnessMomentary(), maxMomentary);
                            }
                            refreshRate = 0;
                        }
                    }

                    this._output.WriteLine($"LoudnessMomentary = {maxMomentary}");
                    Assert.Equal(value, maxMomentary, 1);
                }
            }
        }

        /// <summary>
        /// Test for the loudness value of a single file in short-term (3s) chunks.
        /// </summary>
        [Theory]
        [InlineData("seq-3341-10-1-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-2-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-3-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-4-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-5-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-6-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-7-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-8-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-9-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-10-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-11-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-12-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-13-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-14-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-15-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-16-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-17-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-18-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-19-24bit.wav", -23.0)]
        [InlineData("seq-3341-10-20-24bit.wav", -23.0)]
        [Trait("Category", "libEbur128")]
        public void LoudnessShorttermTest(string filename, double value)
        {
            this._output.WriteLine($"FileName = {filename}");
            this._output.WriteLine($"libebur128 v.{libebur128_net.Ebur128.Version}");
            using (WaveFileReader reader = new WaveFileReader(Path.Combine(this._tempararyPath, filename)))
            {
                using (libebur128_net.Ebur128 ebur128 = libebur128_net.Ebur128.Init((uint)reader.WaveFormat.Channels, (uint)reader.WaveFormat.SampleRate, Mode.EBUR128_MODE_S))
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

                    double maxShortterm = float.MinValue;
                    for (int i = 0, refreshRate = 0; i < reader.SampleCount; i++, refreshRate++)
                    {
                        Span<float> sampleFrame = reader.ReadNextSampleFrame();
                        ebur128.AddFramesFloat(sampleFrame, (uint)1);

                        //  10 ms buffer / 10 Hz refresh rate.
                        if (refreshRate >= reader.WaveFormat.SampleRate / 100)
                        {
                            // # Invalid results before the first 3 seconds.
                            if (i >= (3 * reader.WaveFormat.SampleRate))
                            {
                                maxShortterm = Math.Max(ebur128.LoudnessShortterm(), maxShortterm);
                            }
                            refreshRate = 0;
                        }
                    }

                    this._output.WriteLine($"LoudnessShortterm = {maxShortterm}");
                    Assert.Equal(value, maxShortterm, 1);
                }
            }
        }

        public static async Task DownloadAndExtractZIP(string url, string path)
        {
            using (var client = new HttpClient())
            {

                using (var result = await client.GetAsync(url))
                {
                    if (result.IsSuccessStatusCode)
                    {

                        using (ZipArchive archive = new ZipArchive(await result.Content.ReadAsStreamAsync()))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                using (Stream stream = entry.Open())
                                {
                                    string destination = Path.GetFullPath(Path.Combine(path, entry.FullName));

                                    var directory = Path.GetDirectoryName(destination);
                                    if (!Directory.Exists(directory))
                                        Directory.CreateDirectory(directory!);

                                    using (FileStream file = new FileStream(destination, FileMode.Create, FileAccess.Write))
                                    {
                                        await stream.CopyToAsync(file);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}