using System.Runtime.InteropServices;
using System.Security;

namespace libebur128_net.libebur128
{
    using Ebur128State = libebur128Native.ebur128_state;
    using Mode = libebur128Native.mode;
    using Channel = libebur128Native.channel;
    using Error = libebur128Native.error;

    public class LibEbur128Wrapper
    {


        /// <summary>
        /// Destroy library state.
        /// </summary>
        /// <param name="st">st pointer to a library state.</param>
        public static (int major, int minor, int patch) Ebur128_GetVersion()
        {
            libebur128Native.ebur128_get_version(out int major, out int minor, out int patch);
            return (major, minor, patch);
        }

        /// <summary>
        /// Initialize library state.
        /// </summary>
        /// <param name="channels">Channels the number of channels.</param>
        /// <param name="sampleRate">samplerate the sample rate.</param>
        /// <param name="mode">Mode see the mode enum for possible values.</param>
        /// <returns>Initialized library state, or NULL on error</returns>
        public static Ebur128State? Ebur128_Init(uint channels, uint sampleRate, Mode mode)
        {
            IntPtr ptr = libebur128Native.ebur128_init(channels, sampleRate, (int)mode);
            return ptr == IntPtr.Zero
                ? null
                : Marshal.PtrToStructure<Ebur128State>(ptr);
        }

        /// <summary>
        /// Destroy library state.
        /// </summary>
        /// <param name="st">st pointer to a library state.</param>
        public static void Ebur128_Destroy(ref Ebur128State st)
        {

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                libebur128Native.ebur128_destroy(ref ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }


        /// <summary>
        /// Set channel type.
        /// The default is:
        ///   - 0 -> EBUR128_LEFT
        ///   - 1 -> EBUR128_RIGHT
        ///   - 2 -> EBUR128_CENTER
        ///   - 3 -> EBUR128_UNUSED
        ///   - 4 -> EBUR128_LEFT_SURROUND
        ///   - 5 -> EBUR128_RIGHT_SURROUND
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="channel_number">zero based channel index.</param>
        /// <param name="value">type from the "channel" enum.</param>
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_INVALID_CHANNEL_INDEX if invalid channel index.
        /// </returns>
        public static Error Ebur128_SetChannel(Ebur128State st,
                        uint channelNumber,
                        Channel value)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_set_channel(ptr, channelNumber, (int)value);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// Change library parameters.
        /// 
        ///   Note that the channel map will be reset when setting a different number of
        ///   channels. The current unfinished block will be lost.
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="channels">new number of channels</param>
        /// <param name="sampleRate">new sample rate</param>
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_NOMEM on memory allocation error. The state will be invalid and must be destroyed.
        /// EBUR128_ERROR_NO_CHANGE if channels and sample rate were not changed.
        /// </returns>
        public static Error Ebur128_ChangeParameters(Ebur128State st,
            uint channels,
            System.UInt32 sampleRate)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_change_parameters(ptr, channels, sampleRate);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// Set the maximum window duration.
        ///  
        ///    Set the maximum duration that will be used for ebur128_loudness_window().
        ///    Note that this destroys the current content of the audio buffer.
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="window">duration of the window in ms.</param>
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_NOMEM on memory allocation error. The state will be invalid and must be destroyed.
        /// EBUR128_ERROR_NO_CHANGE if window duration not changed.
        /// </returns>
        public static Error Ebur128_SetMaxWindow(Ebur128State st, uint window)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_set_max_window(ptr, window);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }


        /// <summary>
        /// Get global integrated loudness in LUFS.
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="result">out integrated loudness in LUFS. -HUGE_VAL if result is negative infinity.</param>       
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_INVALID_MODE if mode "EBUR128_MODE_I" has not been set.
        /// </returns>
        public static Error Ebur128_LoudnessGlobal(Ebur128State st, out double result)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_loudness_global(ptr, out result);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// Get global integrated loudness in LUFS across multiple instances.
        /// </summary>
        /// <param name="sts">array of library states.</param>
        /// <param name="size"> length of sts</param>
        /// <param name="result"> integrated loudness in LUFS. -HUGE_VAL if result is negative infinity.</param>
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_INVALID_MODE if mode "EBUR128_MODE_I" has not been set.
        /// </returns>
        public static Error Ebur128_LoudnessGlobalMultiple(Ebur128State[] sts, out double result)
        {
            List<IntPtr> states = new List<IntPtr>();
            try
            {
                foreach (var st in sts)
                {

                    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
                    Marshal.StructureToPtr(st, ptr, false);
                    states.Add(ptr);
                }
                return (Error)libebur128Native.ebur128_loudness_global_multiple(states.ToArray(), new((uint)states.Count), out result);
            }
            finally
            {
                foreach (var ptr in states)
                {
                    Marshal.FreeHGlobal(ptr);
                }              
            }
        }

        /// <summary>
        /// Get loudness range (LRA) in LU across multiple instances.
        /// Calculates loudness range according to EBU 3342.
        /// </summary>
        /// <param name="sts">library state</param>
        /// <param name="size"> length of sts</param>
        /// <param name="result">loudness range (LRA) in LU. Will not be changed in case of error. EBUR128_ERROR_NOMEM or EBUR128_ERROR_INVALID_MODE will be  returned in this case.</param>       
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_NOMEM in case of memory allocation error.
        /// EBUR128_ERROR_INVALID_MODE if mode "EBUR128_MODE_LRA" has not been set.
        /// </returns>
        public static Error Ebur128_LoudnessRangeMultiple(Ebur128State[] sts, out double result)
        {
            List<IntPtr> states = new List<IntPtr>();
            try
            {
                foreach (var st in sts)
                {

                    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
                    Marshal.StructureToPtr(st, ptr, false);
                    states.Add(ptr);
                }
                return (Error) libebur128Native.ebur128_loudness_range_multiple(states.ToArray(), new ((uint) states.Count), out result);
            }
            finally
            {
                foreach (var ptr in states)
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }


        /// <summary>
        /// Get loudness range (LRA) of programme in LU.
        /// Calculates loudness range according to EBU 3342.
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="result">loudness range (LRA) in LU. Will not be changed in case of error. EBUR128_ERROR_NOMEM or EBUR128_ERROR_INVALID_MODE will be  returned in this case.</param>       
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_NOMEM in case of memory allocation error.
        /// EBUR128_ERROR_INVALID_MODE if mode "EBUR128_MODE_LRA" has not been set.
        /// </returns>
        public static Error Ebur128_LoudnessRange(Ebur128State st, out double result)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_loudness_range(ptr, out result);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// Get relative threshold in LUFS.
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="result">relative threshold in LUFS</param>
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_INVALID_MODE if mode "EBUR128_MODE_I" has not been set.
        /// </returns>
        public static Error Ebur128_RelativeThreshold(Ebur128State st, out double result)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_relative_threshold(ptr, out result);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }


        /// <summary>
        ///  Get momentary loudness (last 400ms) in LUFS.
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="result">out momentary loudness in LUFS. -HUGE_VAL if result is negative infinity.</param>       
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// </returns>
        public static Error Ebur128_LoudnessMomentary(Ebur128State st, out double result)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_loudness_momentary(ptr, out result);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        ///   Get short-term loudness (last 3s) in LUFS.
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="result">out short-term loudness in LUFS. -HUGE_VAL if result is negative infinity.</param>       
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_INVALID_MODE if mode "EBUR128_MODE_S" has not been set.
        /// </returns>
        public static Error Ebur128_LoudnessShortterm(Ebur128State st, out double result)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_loudness_shortterm(ptr, out result);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// Get loudness of the specified window in LUFS.
        ///   window must not be larger than the current window set in st.
        ///   The current window can be changed by calling ebur128_set_max_window().
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="window">window in ms to calculate loudness.</param>
        /// <param name="result">out loudness in LUFS. -HUGE_VAL if result is negative infinity.</param>       
        /// <returns>
        /// EBUR12
        public static Error Ebur128_LoudnessWindows(Ebur128State st, uint window, out double result)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_loudness_window(ptr, window, out result);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// Get maximum true peak from all frames that have been processed.
        /// 
        ///   Uses an implementation defined algorithm to calculate the true peak. Do not
        ///   try to compare resulting values across different versions of the library,
        ///   as the algorithm may change.
        /// 
        ///   The current implementation uses a custom polyphase FIR interpolator to
        ///   calculate true peak. Will oversample 4x for sample rates < 96000 Hz, 2x for
        ///   sample rates < 192000 Hz and leave the signal unchanged for 192000 Hz.
        /// 
        ///   The equation to convert to dBTP is: 20 * log10(out)
        /// <param name="st">library state</param>
        /// <param name="channel_number">channel to analyse</param>
        /// <param name="result">out maximum true peak in float format (1.0 is 0 dBTP)</param>
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_INVALID_MODE if mode "EBUR128_MODE_TRUE_PEAK" has not been set.
        /// EBUR128_ERROR_INVALID_CHANNEL_INDEX if invalid channel index.
        /// </returns>
        public static Error Ebur128_TruePeak(Ebur128State st, uint channelNumber, out double result)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_true_peak(ptr, channelNumber, out result);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }


        /// <summary>
        /// Get maximum true peak from the last call to add_frames().
        /// 
        ///   Uses an implementation defined algorithm to calculate the true peak. Do not
        ///   try to compare resulting values across different versions of the library,
        ///   as the algorithm may change.
        /// 
        ///   The current implementation uses a custom polyphase FIR interpolator to
        ///   calculate true peak. Will oversample 4x for sample rates < 96000 Hz, 2x for
        ///   sample rates < 192000 Hz and leave the signal unchanged for 192000 Hz.
        /// 
        ///   The equation to convert to dBTP is: 20 * log10(out)
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="channel_number">channel to analyse</param>
        /// <param name="result">out maximum true peak in float format (1.0 is 0 dBTP)</param>
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_INVALID_MODE if mode "EBUR128_MODE_TRUE_PEAK" has not been set.
        /// EBUR128_ERROR_INVALID_CHANNEL_INDEX if invalid channel index.
        /// </returns>
        public static Error Ebur128_PrevTruePeak(Ebur128State st, uint channelNumber, out double result)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_prev_true_peak(ptr, channelNumber, out result);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// Get maximum sample peak from all frames that have been processed.
        /// The equation to convert to dBFS is: 20 * log10(out)
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="channel_number">channel to analyse</param>       
        /// <param name="result">out maximum sample peak in float format (1.0 is 0 dBFS)</param>       
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_INVALID_MODE if mode "EBUR128_MODE_SAMPLE_PEAK" has not been set.
        /// EBUR128_ERROR_INVALID_CHANNEL_INDEX if invalid channel index.
        /// </returns>
        public static Error Ebur128_SamplePeak(Ebur128State st, uint channelNumber, out double result)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_sample_peak(ptr, channelNumber, out result);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// Get maximum sample peak from the last call to add_frames().
        /// The equation to convert to dBFS is: 20 * log10(out)
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="channel_number">channel to analyse</param>       
        /// <param name="result">out maximum sample peak in float format (1.0 is 0 dBFS)</param>       
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_INVALID_MODE if mode "EBUR128_MODE_SAMPLE_PEAK" has not been set.
        /// EBUR128_ERROR_INVALID_CHANNEL_INDEX if invalid channel index.
        /// </returns>
        public static Error Ebur128_PrevSamplePeak(Ebur128State st, uint channelNumber, out double result)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_prev_sample_peak(ptr, channelNumber, out result);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// Add frames to be processed.
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="src">array of source frames. Channels must be interleaved.</param>       
        /// <param name="frames">number of frames. Not number of samples!</param>
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_NOMEM on memory allocation error.
        /// </returns>
        public static Error Ebur128_AddFramesShort(Ebur128State st, Span<short> samples, uint frames)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_add_frames_short(ptr, MemoryMarshal.GetReference(samples), new(frames));
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        ///See \ref ebur128_add_frames_short 
        /// </summary>
        public static Error Ebur128_AddFramesInt(Ebur128State st, Span<int> samples, uint frames)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_add_frames_int(ptr, MemoryMarshal.GetReference(samples), new(frames));
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        ///See \ref ebur128_add_frames_short 
        /// </summary>
        public static Error Ebur128_AddFramesFloat(Ebur128State st, Span<float> samples, uint frames)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_add_frames_float(ptr, MemoryMarshal.GetReference(samples), new(frames));
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        ///See \ref ebur128_add_frames_short 
        /// </summary>
        public static Error Ebur128_AddFramesDouble(Ebur128State st, Span<double> samples, uint frames)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Ebur128State>());
            Marshal.StructureToPtr(st, ptr, false);
            try
            {
                return (Error)libebur128Native.ebur128_add_frames_double(ptr, MemoryMarshal.GetReference(samples), new(frames));
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

    }
}