using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;

namespace libebur128_net.libebur128
{

    /// <summary>
    /// libebur128 - a library for loudness measurement according to the EBU R128 standard.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public static class libebur128Native
    {
        public const int EBUR128_VERSION_MAJOR = 1;
        public const int EBUR128_VERSION_MINOR = 2;
        public const int EBUR128_VERSION_PATCH = 6;

        const string _ebur128Library = "ebur128";

        static libebur128Native()
        {
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);
        }

        private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            // NativeLibrary.TryLoad("ebur128.so.1", assembly, DllImportSearchPath.System32, out libHandle);
            // On systems with AVX2 support, load a different library.
            // if (System.Runtime.Intrinsics.X86.Avx2.IsSupported)
            // {
            //     return NativeLibrary.Load("ebur128.dll", assembly, searchPath);
            // }
            IntPtr libHandle = IntPtr.Zero;
            if (libraryName == _ebur128Library)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    libHandle = NativeLibrary.Load("ebur128.dll");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    libHandle = NativeLibrary.Load("libebur128.so.1");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    libHandle = NativeLibrary.Load("libebur128.dylib");
                }
            }
            return libHandle;
        }


        /// <summary>
        /// Use these values when setting the channel map with ebur128_set_channel().
        /// See definitions in ITU R-REC-BS 1770-4 
        /// </summary>
        public enum channel
        {
            /// <summary>
            /// unused channel (for example LFE channel) 
            /// </summary>
            EBUR128_UNUSED = 0,

            EBUR128_LEFT = 1,

            /// <summary>
            /// itu M+030
            /// </summary>
            EBUR128_Mp030 = 1,

            EBUR128_RIGHT = 2,

            /// <summary>
            /// itu M-030
            /// </summary>
            EBUR128_Mm030 = 2,

            EBUR128_CENTER = 3,

            /// <summary>
            /// itu M+000
            /// </summary>
            EBUR128_Mp000 = 3,

            EBUR128_LEFT_SURROUND = 4,

            EBUR128_Mp110 = 4,          /**< itu M+110 */

            EBUR128_RIGHT_SURROUND = 5, /**<           */

            EBUR128_Mm110 = 5,          /**< itu M-110 */

            EBUR128_DUAL_MONO,          /**< a channel that is counted twice */
            EBUR128_MpSC,               /**< itu M+SC  */

            EBUR128_MmSC,               /**< itu M-SC  */

            EBUR128_Mp060,              /**< itu M+060 */

            EBUR128_Mm060,              /**< itu M-060 */

            EBUR128_Mp090,              /**< itu M+090 */

            EBUR128_Mm090,              /**< itu M-090 */

            EBUR128_Mp135,              /**< itu M+135 */

            EBUR128_Mm135,              /**< itu M-135 */

            EBUR128_Mp180,              /**< itu M+180 */

            EBUR128_Up000,              /**< itu U+000 */

            EBUR128_Up030,              /**< itu U+030 */

            EBUR128_Um030,              /**< itu U-030 */

            EBUR128_Up045,              /**< itu U+045 */

            EBUR128_Um045,              /**< itu U-030 */

            EBUR128_Up090,              /**< itu U+090 */

            EBUR128_Um090,              /**< itu U-090 */

            EBUR128_Up110,              /**< itu U+110 */

            EBUR128_Um110,              /**< itu U-110 */

            EBUR128_Up135,              /**< itu U+135 */

            EBUR128_Um135,              /**< itu U-135 */

            EBUR128_Up180,              /**< itu U+180 */

            EBUR128_Tp000,              /**< itu T+000 */

            EBUR128_Bp000,              /**< itu B+000 */

            EBUR128_Bp045,              /**< itu B+045 */

            EBUR128_Bm045               /**< itu B-045 */
        }

        /// <summary>
        /// Error return values.
        /// </summary>
        public enum error
        {
            EBUR128_SUCCESS = 0,
            EBUR128_ERROR_NOMEM,
            EBUR128_ERROR_INVALID_MODE,
            EBUR128_ERROR_INVALID_CHANNEL_INDEX,
            EBUR128_ERROR_NO_CHANGE
        }

        [Flags]
        /// <summary>
        /// Use these values in ebur128_init (or'ed). Try to use the lowest possible
        /// modes that suit your needs, as performance will be better.
        /// </summary>
        public enum mode
        {
            /// <summary>
            ///  can call ebur128_loudness_momentary
            /// </summary>
            EBUR128_MODE_M = 1 << 0,

            /// <summary>
            /// can call ebur128_loudness_shortterm
            /// </summary>
            EBUR128_MODE_S = 1 << 1 | EBUR128_MODE_M,

            /// <summary>
            /// can call ebur128_loudness_global_* and ebur128_relative_threshold
            /// </summary>
            EBUR128_MODE_I = 1 << 2 | EBUR128_MODE_M,

            /// <summary>
            /// can call ebur128_loudness_range
            /// </summary>
            EBUR128_MODE_LRA = 1 << 3 | EBUR128_MODE_S,

            /// <summary>
            /// can call ebur128_sample_peak
            /// </summary>
            EBUR128_MODE_SAMPLE_PEAK = 1 << 4 | EBUR128_MODE_M,

            /// <summary>
            /// can call ebur128_true_peak
            /// </summary>
            EBUR128_MODE_TRUE_PEAK = 1 << 5 | EBUR128_MODE_M | EBUR128_MODE_SAMPLE_PEAK,

            /// <summary>
            /// uses histogram algorithm to calculate loudness
            /// </summary>
            EBUR128_MODE_HISTOGRAM = 1 << 6
        }



        [StructLayout(LayoutKind.Sequential)]
        public struct ebur128_state_internal
        {

        }

        /// <summary>
        /// Contains information about the state of a loudness measurement.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ebur128_state
        {
            /// <summary>
            /// The current mode.
            /// </summary>
            public int mode;

            /// <summary>
            /// The number of channels.
            /// </summary>
            public uint channels;

            /// <summary>
            /// The sample rate.
            /// </summary>
            public /* ulong */ uint samplerate;

            /// <summary>
            /// Internal state.
            /// </summary>
            //public unsafe ebur128_state_internal* d;
            public System.IntPtr d;
        }

        /// <summary>
        /// Get library version number. Do not pass null pointers here.
        /// </summary>
        /// <param name="major">major major version number of library</param>
        /// <param name="minor">minor minor version number of library</param>
        /// <param name="patch">patch patch version number of library</param>
        [DllImport(_ebur128Library, EntryPoint = "ebur128_get_version", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern void ebur128_get_version(out int major, out int minor, out int patch);

        /// <summary>
        /// Initialize library state.
        /// </summary>
        /// <param name="channels">Channels the number of channels.</param>
        /// <param name="sampleRate">samplerate the sample rate.</param>
        /// <param name="modes">Mode see the mode enum for possible values.</param>
        /// <returns>Initialized library state, or NULL on error</returns>
        [DllImport(_ebur128Library, EntryPoint = "ebur128_init", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern IntPtr ebur128_init(uint channels, System.UInt32 sampleRate, int mode);

        /// <summary>
        /// Destroy library state.
        /// </summary>
        /// <param name="st">st pointer to a library state.</param>
        [DllImport(_ebur128Library, EntryPoint = "ebur128_destroy", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern void ebur128_destroy(ref IntPtr st);

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
        [DllImport(_ebur128Library, EntryPoint = "ebur128_set_channel", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_set_channel(IntPtr st, 
            uint channel_number, 
            int value);

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
        [DllImport(_ebur128Library, EntryPoint = "ebur128_change_parameters", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]

        internal static extern int ebur128_change_parameters(IntPtr st,
            uint channels,
            /* ulong */ uint sampleRate);



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
        [DllImport(_ebur128Library, EntryPoint = "ebur128_set_max_window", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_set_max_window(IntPtr st, /* ulong */ uint window);

        /// <summary>
        /// Set the maximum history.
        /// 
        ///   Set the maximum history that will be stored for loudness integration.
        ///   More history provides more accurate results, but requires more resources.
        /// 
        ///   Applies to ebur128_loudness_range() and ebur128_loudness_global() when
        ///   EBUR128_MODE_HISTOGRAM is not set.
        /// 
        ///   Default is ULONG_MAX (at least ~50 days).
        ///   Minimum is 3000ms for EBUR128_MODE_LRA and 400ms for EBUR128_MODE_M.
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="history">duration of history in ms.</param>
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_NO_CHANGE if history not changed.
        /// </returns>
        [DllImport(_ebur128Library, EntryPoint = "ebur128_set_max_history", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_set_max_history(IntPtr st, /* ulong */ uint history);



        /// <summary>
        /// Add frames to be processed.
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="src">array of source frames. Channels must be interleaved.</param>       
        /// <param name="frames">number of frames. Not number of samples! frames = samples / channels</param>
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_NOMEM on memory allocation error.
        /// </returns>
        [DllImport(_ebur128Library, EntryPoint = "ebur128_add_frames_short", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_add_frames_short(IntPtr st, in short src, UIntPtr frames);

        /// <summary>
        ///See \ref ebur128_add_frames_short
        /// </summary>
        [DllImport(_ebur128Library, EntryPoint = "ebur128_add_frames_int", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_add_frames_int(IntPtr st, in int src, UIntPtr frames);

        /// <summary>
        ///See \ref ebur128_add_frames_short 
        /// </summary>
        [DllImport(_ebur128Library, EntryPoint = "ebur128_add_frames_float", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_add_frames_float(IntPtr st, in float source, UIntPtr frames);

        /// <summary>
        ///See \ref ebur128_add_frames_short
        /// </summary>
        [DllImport(_ebur128Library, EntryPoint = "ebur128_add_frames_double", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_add_frames_double(IntPtr st, 
            in double src,  
            UIntPtr frames);

        /// <summary>
        /// Get global integrated loudness in LUFS.
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="result">out integrated loudness in LUFS. -HUGE_VAL if result is negative infinity.</param>       
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_INVALID_MODE if mode "EBUR128_MODE_I" has not been set.
        /// </returns>
        [DllImport(_ebur128Library, EntryPoint = "ebur128_loudness_global", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_loudness_global(IntPtr st, 
            out double result);

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
        [DllImport(_ebur128Library, EntryPoint = "ebur128_loudness_global_multiple", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_loudness_global_multiple(IntPtr[] sts,
            UIntPtr count,
            out double result);


        /// <summary>
        ///  Get momentary loudness (last 400ms) in LUFS.
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="result">out momentary loudness in LUFS. -HUGE_VAL if result is negative infinity.</param>       
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// </returns>
        [DllImport(_ebur128Library, EntryPoint = "ebur128_loudness_momentary", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_loudness_momentary(IntPtr st, out double result);



        /// <summary>
        ///   Get short-term loudness (last 3s) in LUFS.
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="result">out short-term loudness in LUFS. -HUGE_VAL if result is negative infinity.</param>       
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_INVALID_MODE if mode "EBUR128_MODE_S" has not been set.
        /// </returns>
        [DllImport(_ebur128Library, EntryPoint = "ebur128_loudness_shortterm", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_loudness_shortterm(IntPtr st, out double result);

        /// <summary>
        /// Get loudness of the specified window in LUFS.
        ///   window must not be larger than the current window set in st.
        ///   The current window can be changed by calling ebur128_set_max_window().
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="window">window in ms to calculate loudness.</param>
        /// <param name="result">out loudness in LUFS. -HUGE_VAL if result is negative infinity.</param>       
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_INVALID_MODE if window larger than current window in st.
        /// </returns>
        [DllImport(_ebur128Library, EntryPoint = "ebur128_loudness_window", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_loudness_window(IntPtr st,
            /* ulong */ uint window,
            out double result);

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
        [DllImport(_ebur128Library, EntryPoint = "ebur128_loudness_range", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_loudness_range(IntPtr st, out double result);


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
        [DllImport(_ebur128Library, EntryPoint = "ebur128_loudness_range_multiple", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_loudness_range_multiple(IntPtr[] sts,
            UIntPtr size,
            out double result);



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
        [DllImport(_ebur128Library, EntryPoint = "ebur128_sample_peak", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_sample_peak(IntPtr st,
            uint channel_number,
            out double result);

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
        [DllImport(_ebur128Library, EntryPoint = "ebur128_prev_sample_peak", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_prev_sample_peak(IntPtr st,
            uint channel_number,
            out double result);

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
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="channel_number">channel to analyse</param>
        /// <param name="result">out maximum true peak in float format (1.0 is 0 dBTP)</param>
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_INVALID_MODE if mode "EBUR128_MODE_TRUE_PEAK" has not been set.
        /// EBUR128_ERROR_INVALID_CHANNEL_INDEX if invalid channel index.
        /// </returns>
        [DllImport(_ebur128Library, EntryPoint = "ebur128_true_peak", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_true_peak(IntPtr st,
            uint channel_number,
            out double result);

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
        [DllImport(_ebur128Library, EntryPoint = "ebur128_prev_true_peak", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_prev_true_peak(IntPtr st,
            uint channel_number,
            out double result);

      
        /// <summary>
        /// Get relative threshold in LUFS.
        /// </summary>
        /// <param name="st">library state</param>
        /// <param name="result">relative threshold in LUFS</param>
        /// <returns>
        /// EBUR128_SUCCESS on success.
        /// EBUR128_ERROR_INVALID_MODE if mode "EBUR128_MODE_I" has not been set.
        /// </returns>
        [DllImport(_ebur128Library, EntryPoint = "ebur128_relative_threshold", CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        internal static extern int ebur128_relative_threshold(IntPtr st, out double result);


    }
}