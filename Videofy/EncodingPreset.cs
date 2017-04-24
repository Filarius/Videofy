using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videofy.Main
{
    enum EncodingPreset
    {
        ultrafast,
        superfast,
        veryfast,
        faster,
        fast,
        medium,
        slow,
        slower,
        veryslow,
        placebo
    }
    static partial class ExtensionMethods

    {
        public static string ToString(this EncodingPreset preset)
        {
            switch (preset)
            {
                case EncodingPreset.ultrafast: return "ultrafast";
                case EncodingPreset.superfast: return "superfast";
                case EncodingPreset.veryfast: return "veryfast";
                case EncodingPreset.faster: return "faster";
                case EncodingPreset.fast: return "fast";
                case EncodingPreset.medium: return "medium";
                case EncodingPreset.slow: return "slow";
                case EncodingPreset.slower: return "slower";
                case EncodingPreset.veryslow: return "veryslow";
                case EncodingPreset.placebo: return "placebo";                   
            }
            throw new System.ArgumentOutOfRangeException();
        }

        public static EncodingPreset FromString(this EncodingPreset preset, string s)
        {
            switch (s)
            {
                case "ultrafast": return EncodingPreset.ultrafast;
                case "superfast": return EncodingPreset.superfast;
                case "veryfast": return EncodingPreset.veryfast;
                case "faster": return EncodingPreset.faster;
                case "fast": return EncodingPreset.fast;
                case "medium": return EncodingPreset.medium;
                case "slow": return EncodingPreset.slow;
                case "slower": return EncodingPreset.slower;
                case "veryslow": return EncodingPreset.veryslow;
                case "placebo": return EncodingPreset.placebo;
            }
            throw new System.ArgumentOutOfRangeException();
        }
    }

    }
