using libebur128_net.libebur128;
using System.Runtime.InteropServices;
using System.Security;

namespace libebur128_net
{
    using Ebur128State = libebur128Native.ebur128_state;
    using Mode = libebur128Native.mode;
    using Channel = libebur128Native.channel;
    using Error = libebur128Native.error;

    public class Ebur128: IDisposable
    {

        private Ebur128State _state;
        private libebur128Native.mode _mode;
        private uint _channels;
        private uint _sampleRate;


        /// <summary>
        /// Internal state.
        /// </summary>
        public Ebur128State State { get => this._state; internal set => this._state = value;  }


        /// <summary>
        /// The current mode.
        /// </summary>
        public libebur128Native.mode Mode { get => this._mode; internal set => this._mode = value;  }

        /// <summary>
        /// The number of channels.
        /// </summary>
        public uint Channels { get => this._channels; internal set => this._channels = value; }

        /// <summary>
        /// The sample rate.
        /// </summary>
        public uint SampleRate { get => this._sampleRate; internal set => this._sampleRate = value; }



        public static Ebur128Varsion Version
        {
            get
            {
                (int major, int minor, int patch) = LibEbur128Wrapper.Ebur128_GetVersion();
                return new Ebur128Varsion() 
                { 
                    Major = major,
                    Minor = minor,  
                    Patch = patch
                };
            }
        }

        public Ebur128()
        {

        }

        static public Ebur128 Init(uint channels, uint sampleRate, Mode mode)
        {
            Ebur128State? ebur128State = LibEbur128Wrapper.Ebur128_Init(channels, sampleRate, mode);
            if (ebur128State.HasValue)
            {
                return new Ebur128()
                {
                    State = ebur128State.Value,
                    Channels = channels,
                    SampleRate = sampleRate,
                    Mode = mode,

                };
            }
            else
                throw new Exception("Initialized Ebur128 library error");
        }

        public void Dispose() =>
            LibEbur128Wrapper.Ebur128_Destroy(ref this._state);

        public void SetChannelType(uint channelNumber, Channel value) =>
           Ebur128Exception.ThrowIfError(
               LibEbur128Wrapper.Ebur128_SetChannel(this._state, channelNumber, value));


        public void ChangeParameters(uint channels, System.UInt32 sampleRate)
        {
            Ebur128Exception.ThrowIfError(
                            LibEbur128Wrapper.Ebur128_ChangeParameters(this._state, channels, sampleRate));
            this.Channels = channels;
            this.SampleRate = sampleRate;
        }
            
        public void SetMaxWindow(uint channels, System.UInt32 windows) =>
             Ebur128Exception.ThrowIfError(
                 LibEbur128Wrapper.Ebur128_SetMaxWindow(this._state, windows));


        public double LoudnessGlobal()
        {
            Ebur128Exception.ThrowIfError(
                LibEbur128Wrapper.Ebur128_LoudnessGlobal(this._state, out double result));
            return result;
        }
                 

        public double RelativeThreshold()
        { 
            Ebur128Exception.ThrowIfError(
                LibEbur128Wrapper.Ebur128_RelativeThreshold(this._state, out double result));
            return result;
        }

        public double LoudnessRange() 
        { 
            Ebur128Exception.ThrowIfError(
                LibEbur128Wrapper.Ebur128_LoudnessRange(this._state, out double result));
            return result;
        }

        public double LoudnessMomentary() 
        { 
            Ebur128Exception.ThrowIfError(
                LibEbur128Wrapper.Ebur128_LoudnessMomentary(this._state, out double result));
            return result;
        }

        public double LoudnessShortterm()
        {
            Ebur128Exception.ThrowIfError(
                LibEbur128Wrapper.Ebur128_LoudnessShortterm(this._state, out double result));
            return result;
        }
           

        public double TruePeak(uint channelNumber)
        {
            Ebur128Exception.ThrowIfError(
                LibEbur128Wrapper.Ebur128_TruePeak(this._state, channelNumber, out double result));
            return result;
        }
           

        public double SamplePeak(uint channelNumber)
        {
            Ebur128Exception.ThrowIfError(
                 LibEbur128Wrapper.Ebur128_SamplePeak(this._state, channelNumber, out double result));
            return result;
        }
           

        public double AbsoluteTruePeak()
        {
            var absolutePeak = 0.0;
            for (uint channel = 0; channel < this.Channels; channel++)
            {
                absolutePeak = Math.Max(this.TruePeak(channel), absolutePeak);
            }
            return absolutePeak;
        }

        public double AbsoluteSamplePeak()
        {
            var absolutePeak = 0.0;
            for (uint channel = 0; channel < this.Channels; channel++)
            {
                absolutePeak = Math.Max(this.SamplePeak(channel), absolutePeak);
            }
            return absolutePeak;
        }

        public Error AddFramesFloat(Span<float> samples, uint frames) =>
            LibEbur128Wrapper.Ebur128_AddFramesFloat(this._state, samples, frames);

        public Error AddFramesShort(Span<short> samples, uint frames) =>
            LibEbur128Wrapper.Ebur128_AddFramesShort(this._state, samples, frames);

        public Error AddFramesDouble(Span<double> samples, uint frames) =>
            LibEbur128Wrapper.Ebur128_AddFramesDouble(this._state, samples, frames);

        static public double LoudnessGlobalMultiple(Ebur128[] handles)
        {
            Ebur128Exception.ThrowIfError(
                LibEbur128Wrapper.Ebur128_LoudnessGlobalMultiple(handles.Select(handle => handle.State).ToArray(), out double result));
            return result;
        }

        static public double LoudnessRangeMultiple(Ebur128[] handles)
        {
            Ebur128Exception.ThrowIfError(
                LibEbur128Wrapper.Ebur128_LoudnessRangeMultiple(handles.Select(handle => handle.State).ToArray(), out double result));
            return result;
        }

    }
}