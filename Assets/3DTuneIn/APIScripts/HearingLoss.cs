﻿/**
*** API for 3D-Tune-In Toolkit HL Simulation Unity Wrapper ***
*
* version beta 1.0
* Created on: November 2016
* 
* Author: 3DI-DIANA Research Group / University of Malaga / Spain
* Contact: areyes@uma.es
* 
* Project: 3DTI (3D-games for TUNing and lEarnINg about hearing aids)
* Module: 3DTI Toolkit Unity Wrapper
**/

using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;   // For ReadOnlyCollection
using API_3DTI;
using System;

using static API_3DTI.HearingLoss.Parameter;

namespace API_3DTI
{
    public class HearingLoss : AbstractMixerEffect
    {
        // Global variables
        public AudioMixer hlMixer;  // Drag&drop here the HAHL_3DTI_Mixer

        public const int NumMultibandExpansionBands = 9;
        public enum Parameter
        {
            [Parameter(pluginNameLeft = "HLONL", pluginNameRight = "HLONR", mixerNameLeft = "HL3DTI_Process_LeftOn", mixerNameRight = "HL3DTI_Process_RightOn", label = "Hearing loss enabled", description = "Switch on hearing loss simulation", type = typeof(bool))]
            HLOn,

            [Parameter(pluginNameLeft = "HLCAL", pluginNameRight = "HLCAL", mixerNameLeft = "HL3DTI_Calibration", mixerNameRight = "HL3DTI_Calibration", units = "dBSPL", label = "Calibration", description = "Calibration: dBSPL equivalent to 0 dBFS", type = typeof(float))]
            Calibration,

            [Parameter(pluginNameLeft = "HL0L", pluginNameRight = "HL0R", mixerNameLeft = "HL3DTI_HL_Band_0_Left", mixerNameRight = "HL3DTI_HL_Band_0_Right", units = "dBHL", label = "62.5 Hz", description = "Hearing loss level for 62.5 Hz band (dB HL)", type = typeof(float))]
            MultibandExpansionBand0,

            [Parameter(pluginNameLeft = "HL1L", pluginNameRight = "HL1R", mixerNameLeft = "HL3DTI_HL_Band_1_Left", mixerNameRight = "HL3DTI_HL_Band_1_Right", units = "dBHL", label = "125 Hz", description = "Hearing loss level for 125 Hz band (dB HL)", type = typeof(float))]
            MultibandExpansionBand1,

            [Parameter(pluginNameLeft = "HL2L", pluginNameRight = "HL2R", mixerNameLeft = "HL3DTI_HL_Band_2_Left", mixerNameRight = "HL3DTI_HL_Band_2_Right", units = "dBHL", label = "250 Hz", description = "Hearing loss level for 250 Hz band (dB HL)", type = typeof(float))]
            MultibandExpansionBand2,

            [Parameter(pluginNameLeft = "HL3L", pluginNameRight = "HL3R", mixerNameLeft = "HL3DTI_HL_Band_3_Left", mixerNameRight = "HL3DTI_HL_Band_3_Right", units = "dBHL", label = "500 Hz", description = "Hearing loss level for 500 Hz band (dB HL)", type = typeof(float))]
            MultibandExpansionBand3,

            [Parameter(pluginNameLeft = "HL4L", pluginNameRight = "HL4R", mixerNameLeft = "HL3DTI_HL_Band_4_Left", mixerNameRight = "HL3DTI_HL_Band_4_Right", units = "dBHL", label = "1 KHz", description = "Hearing loss level for 1 KHz band (dB HL)", type = typeof(float))]
            MultibandExpansionBand4,

            [Parameter(pluginNameLeft = "HL5L", pluginNameRight = "HL5R", mixerNameLeft = "HL3DTI_HL_Band_5_Left", mixerNameRight = "HL3DTI_HL_Band_5_Right", units = "dBHL", label = "2 KHz", description = "Hearing loss level for 2 KHz band (dB HL)", type = typeof(float))]
            MultibandExpansionBand5,

            [Parameter(pluginNameLeft = "HL6L", pluginNameRight = "HL6R", mixerNameLeft = "HL3DTI_HL_Band_6_Left", mixerNameRight = "HL3DTI_HL_Band_6_Right", units = "dBHL", label = "4 KHz", description = "Hearing loss level for 4 KHz band (dB HL)", type = typeof(float))]
            MultibandExpansionBand6,

            [Parameter(pluginNameLeft = "HL7L", pluginNameRight = "HL7R", mixerNameLeft = "HL3DTI_HL_Band_7_Left", mixerNameRight = "HL3DTI_HL_Band_7_Right", units = "dBHL", label = "8 KHz", description = "Hearing loss level for 8 KHz band (dB HL)", type = typeof(float))]
            MultibandExpansionBand7,

            [Parameter(pluginNameLeft = "HL8L", pluginNameRight = "HL8R", mixerNameLeft = "HL3DTI_HL_Band_8_Left", mixerNameRight = "HL3DTI_HL_Band_8_Right", units = "dBHL", label = "16 KHz", description = "Hearing loss level for 16 KHz band (dB HL)", type = typeof(float))]
            MultibandExpansionBand8,
            // NB if adding multiband expansion bands then update NumMultibandExpansionBands constant.

            [Parameter(pluginNameLeft = "HLMBEAPPROACHL", pluginNameRight = "HLMBEAPPROACHR", mixerNameLeft = "HL3DTI_HL_MBE_Approach_Left", mixerNameRight = "HL3DTI_HL_MBE_Approach_Right", label = "Algorithm", description = "Multiband expander algorithm", type = typeof(T_MultibandExpanderApproach))]
            MultibandExpansionApproach,

            [Parameter(pluginNameLeft = "HLMBEFGL", pluginNameRight = "HLMBEFGR", label = "Filter Grouping", description = "Multiband expander filter grouping", type = typeof(bool))]
            MultibandExpansionFilterGrouping,

            [Parameter(pluginNameLeft = "HLMBEFPBL", pluginNameRight = "HLMBEFPBR", label = "Filters per band", description = "Multiband expander number of filters per band (odd number)", type = typeof(int))]
            MultibandExpansionNumFiltersPerBand,

            [Parameter(pluginNameLeft = "HLATKL", pluginNameRight = "HLATKR", mixerNameLeft = "HL3DTI_Attack_Left", mixerNameRight = "HL3DTI_Attack_Right", units = "ms", label = "Attack", description = "Attack time envelope detectors for all bands (ms)", type = typeof(float))]
            MultibandExpansionAttack,

            [Parameter(pluginNameLeft = "HLRELL", pluginNameRight = "HLRELR", mixerNameLeft = "HL3DTI_Release_Left", mixerNameRight = "HL3DTI_Release_Right", units = "ms", label = "Release", description = "Release time envelope detectors for all bands (ms)", type = typeof(float))]
            MultibandExpansionRelease,

            [Parameter(pluginNameLeft = "HLMBEONL", pluginNameRight = "HLMBEONR", mixerNameLeft = "HL3DTI_MBE_LeftOn", mixerNameRight = "HL3DTI_MBE_RightOn", label = "Multiband Expander", description = "Switch on multiband expander", type = typeof(bool))]
            MultibandExpansionOn,

            [Parameter(pluginNameLeft = "HLTAONL", pluginNameRight = "HLTAONR", mixerNameLeft = "HL3DTI_TA_LeftOn", mixerNameRight = "HL3DTI_TA_RightOn", label = "Temporal Distortion", description = "Switch on temporal distortion simulation", type = typeof(bool))]
            TemporalDistortionOn,

            [Parameter(pluginNameLeft = "HLTABANDL", pluginNameRight = "HLTABANDR", mixerNameLeft = "HL3DTI_TA_Band_Left", mixerNameRight = "HL3DTI_TA_Band_Right", units = "Hz", label = "Upper band limit", description = "Upper band limit for temporal distortion simulation (Hz)", type = typeof(float), validValues = new float[] { 200, 400, 800, 1600, 3200, 6400, 12800 })]
            TemporalDistortionBandUpperLimit,

            [Parameter(pluginNameLeft = "HLTALPFL", pluginNameRight = "HLTALPFR", mixerNameLeft = "HL3DTI_TA_Noise_LPF_Left", mixerNameRight = "HL3DTI_TA_Noise_LPF_Right", units = "Hz", label = "LPF Cutoff frequency", description = "Cutoff frequency of temporal distortion jitter noise autocorrelation LPF (Hz)", type = typeof(float))]
            TemporalDistortionNoiseBandwidth,

            [Parameter(pluginNameLeft = "HLTAPOWL", pluginNameRight = "HLTAPOWR", mixerNameLeft = "HL3DTI_TA_Noise_Power_Left", mixerNameRight = "HL3DTI_TA_Noise_Power_Right", units = "ms", label = "White noise power", description = "Power of temporal distortion jitter white noise (ms)", type = typeof(float))]
            TemporalDistortionWhiteNoisePower,

            [Parameter(pluginNameLeft = "HLTALR", mixerNameLeft = "HL3DTI_TA_LRSync", pluginNameRight = "HLTALR", mixerNameRight = "HL3DTI_TA_LRSync", label = "Left/Right sync amount", description = "Synchronise the noise source for Temporal Distortion between left and right ears", type = typeof(float))]
            TemporalDistortionLRSyncAmount,

            [Parameter(pluginNameLeft = "HLTALRON", mixerNameLeft = "HL3DTI_TA_LRSync_On", pluginNameRight = "HLTALRON", mixerNameRight = "HL3DTI_TA_LRSync_On", label = "Left/Right sync", description = "Make the right ear use the same parameters for temporal distortion as the left ear", type = typeof(bool))]
            TemporalDistortionLRSyncOn,

            [Parameter(pluginNameLeft = "HLTA0GL", pluginNameRight = "HLTA0GR", mixerNameLeft = "HL3DTI_TA_Autocor0_Get_Left", mixerNameRight = "HL3DTI_TA_Autocor0_Get_Right", label = "Autocorrelation coefficient zero", description = "Autocorrelation coefficient zero in left temporal distortion noise source?", type = typeof(float))]
            TemporalDistortionAutocorrelation0Get,

            [Parameter(pluginNameLeft = "HLTA1GL", pluginNameRight = "HLTA1GR", mixerNameLeft = "HL3DTI_TA_Autocor1_Get_Left", mixerNameRight = "HL3DTI_TA_Autocor1_Get_Right", label = "Autocorrelation coefficient one", description = "Autocorrelation coefficient one in left temporal distortion noise source?", type = typeof(float))]
            TemporalDistortionAutocorrelation1Get,

            [Parameter(pluginNameLeft = "HLFSONL", pluginNameRight = "HLFSONR", mixerNameLeft = "HL3DTI_FS_LeftOn", mixerNameRight = "HL3DTI_FS_RightOn", label = "Frequency Smearing", description = "Switch on frequency smearing simulation", type = typeof(bool))]
            FrequencySmearingOn,

            [Parameter(pluginNameLeft = "HLFSAPPROACHL", pluginNameRight = "HLFSAPPROACHR", mixerNameLeft = "HL3DTI_HL_FS_Approach_Left", mixerNameRight = "HL3DTI_HL_FS_Approach_Right", label = "Algorithm", description = "Approach used for Frequency smearing", type = typeof(T_HLFrequencySmearingApproach))]
            FrequencySmearingApproach,

            [Parameter(pluginNameLeft = "HLFSDOWNSZL", pluginNameRight = "HLFSDOWNSZR", mixerNameLeft = "HL3DTI_FS_Size_Down_Left", mixerNameRight = "HL3DTI_FS_Size_Down_Right", label = "Downward smearing window size", description = "Size of downward section of smearing window", type = typeof(int))]
            FrequencySmearingDownSize,

            [Parameter(pluginNameLeft = "HLFSUPSZL", pluginNameRight = "HLFSUPSZR", mixerNameLeft = "HL3DTI_FS_Size_Up_Left", mixerNameRight = "HL3DTI_FS_Size_Up_Right", label = "Upward smearing window size", description = "Size of upward section of smearing window", type = typeof(int))]
            FrequencySmearingUpSize,

            [Parameter(pluginNameLeft = "HLFSDOWNHZL", pluginNameRight = "HLFSDOWNHZR", mixerNameLeft = "HL3DTI_FS_Hz_Down_Left", mixerNameRight = "HL3DTI_FS_Hz_Down_Right", units = "Hz", label = "Downward smearing amount", description = "Amount of downward smearing effect (in Hz)", type = typeof(float))]
            FrequencySmearingDownHz,

            [Parameter(pluginNameLeft = "HLFSUPHZL", pluginNameRight = "HLFSUPHZR", mixerNameLeft = "HL3DTI_FS_Hz_Up_Left", mixerNameRight = "HL3DTI_FS_Hz_Up_Right", units = "Hz", label = "Upward smearing amount", description = "Amount of upward smearing effect (in Hz)", type = typeof(float))]
            FrequencySmearingUpHz,


        };



        // Public constant and type definitions 
        //public static readonly ReadOnlyCollection<float> AUDIOMETRY_PRESET_MILD = new ReadOnlyCollection<float>(new[] { 7f, 7f, 12f, 17f, 28f, 31f, 31f, 31f, 31f });
        //public static readonly ReadOnlyCollection<float> AUDIOMETRY_PRESET_MODERATE = new ReadOnlyCollection<float>(new[] { 32f, 32f, 37f, 42f, 53f, 56f, 56f, 56f, 56f });
        //public static readonly ReadOnlyCollection<float> AUDIOMETRY_PRESET_SEVERE = new ReadOnlyCollection<float>(new[] { 62f, 62f, 67f, 72f, 83f, 86f, 86f, 86f, 86f });
        //public static readonly ReadOnlyCollection<float> AUDIOMETRY_PRESET_NORMAL = new ReadOnlyCollection<float>(new[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f });        
        public enum T_HLBand { HZ_62 = 0, HZ_125 = 1, HZ_250 = 2, HZ_500 = 3, HZ_1K = 4, HZ_2K = 5, HZ_4K = 6, HZ_8K = 7, HZ_16K = 8 };
        public const int NUM_HL_BANDS = 9;
        public enum T_HLTemporalDistortionBandUpperLimit { HZ_UL_200 = 0, HZ_UL_400 = 1, HZ_UL_800 = 2, HZ_UL_1600 = 3, HZ_UL_3200 = 4, HZ_UL_6400 = 5, HZ_UL_12800 = 6, HZ_UL_WRONG = -1 };
        public enum T_HLClassificationScaleCurve
        {
            HL_CS_UNDEFINED = -1, HL_CS_NOLOSS = 0, HL_CS_A = 1, HL_CS_B = 2, HL_CS_C = 3, HL_CS_D = 4, HL_CS_E = 5, HL_CS_F = 6,
            HL_CS_G = 7, HL_CS_H = 8, HL_CS_I = 9, HL_CS_J = 10, HL_CS_K = 11
        };
        public enum T_HLPreset { HL_PRESET_NORMAL = 0, HL_PRESET_MILD = 1, HL_PRESET_MODERATE = 2, HL_PRESET_SEVERE = 3, HL_PRESET_CUSTOM = -1 };
        public enum T_HLClassificationScaleSeverity
        {
            HL_CS_SEVERITY_NOLOSS = 0, HL_CS_SEVERITY_MILD = 1, HL_CS_SEVERITY_MILDMODERATE = 2,
            HL_CS_SEVERITY_MODERATE = 3, HL_CS_SEVERITY_MODERATESEVERE = 4, HL_CS_SEVERITY_SEVERE = 5,
            HL_CS_SEVERITY_PROFOUND = 6, HL_CS_SEVERITY_UNDEFINED = -1
        };

        public enum T_HLFrequencySmearingApproach : int
        {
            BaerMoore,
            Graf
        };
        public enum T_MultibandExpanderApproach : int
        {
            Butterworth,
            Gammatone,
        };

 
        [HideInInspector]
        public T_HLClassificationScaleCurve PARAM_CLASSIFICATION_CURVE_LEFT = T_HLClassificationScaleCurve.HL_CS_NOLOSS;      // For internal use, DO NOT USE IT DIRECTLY
        [HideInInspector]
        public int PARAM_CLASSIFICATION_SLOPE_LEFT = 0;      // For internal use, DO NOT USE IT DIRECTLY
        [HideInInspector]
        public T_HLClassificationScaleSeverity PARAM_CLASSIFICATION_SEVERITY_LEFT = 0;      // For internal use, DO NOT USE IT DIRECTLY
        [HideInInspector]
        public T_HLClassificationScaleCurve PARAM_CLASSIFICATION_CURVE_RIGHT = T_HLClassificationScaleCurve.HL_CS_NOLOSS;     // For internal use, DO NOT USE IT DIRECTLY
        [HideInInspector]
        public int PARAM_CLASSIFICATION_SLOPE_RIGHT = 0;     // For internal use, DO NOT USE IT DIRECTLY
        [HideInInspector]
        public T_HLClassificationScaleSeverity PARAM_CLASSIFICATION_SEVERITY_RIGHT = 0;      // For internal use, DO NOT USE IT DIRECTLY



        public bool SetParameter<T>(Parameter p, T value, T_ear ear = T_ear.BOTH) where T : IConvertible
        {
            return _SetParameter(hlMixer, p, value, ear);
        }

        public T GetParameter<T>(Parameter p, T_ear ear)
        {
            return _GetParameter<Parameter, T>(hlMixer, p, ear);
        }

        ///////////////////////////////////////
        // AUDIOMETRY 
        ///////////////////////////////////////

        /// <summary>
        /// Set all hearing loss levels (full audiometry) for one ear
        /// </summary>
        /// <param name="ear"></param>
        /// <param name="hearingLevels (dBHL[])"></param>
        /// <returns></returns>
        public bool SetAudiometry(T_ear ear, List<float> hearingLevels)
        {
            for (int b = 0; b < Math.Min(9, hearingLevels.Count); b++)
            {
                if (!SetParameter(MultibandExpansionBand0 + b, hearingLevels[b], ear)) return false;
                //if (!SetHearingLevel(ear, b, hearingLevels[b])) return false;
            }

            return true;
        }


        /// <summary>
        /// Set audiometry from a curve and slope level using HL Classification Scale
        /// </summary>
        /// <param name="ear"></param>
        /// <param name="curve"></param>
        /// <param name="slope"></param>
        /// <param name="severity"></param>
        /// <returns></returns>
        public bool SetAudiometryFromClassificationScale(T_ear ear, T_HLClassificationScaleCurve curve, int slope, T_HLClassificationScaleSeverity severity)
        {

            if (ear.HasFlag(T_ear.LEFT))
            {
                PARAM_CLASSIFICATION_CURVE_LEFT = curve;
                PARAM_CLASSIFICATION_SLOPE_LEFT = slope;
                PARAM_CLASSIFICATION_SEVERITY_LEFT = severity;
            }
            if (ear.HasFlag(T_ear.RIGHT))
            {
                PARAM_CLASSIFICATION_CURVE_RIGHT = curve;
                PARAM_CLASSIFICATION_SLOPE_RIGHT = slope;
                PARAM_CLASSIFICATION_SEVERITY_RIGHT = severity;
            }
            List<float> hl;
            GetClassificationScaleHL(curve, slope, severity, out hl);

            return SetParameter(MultibandExpansionBand0, hl[0], ear)
                && SetParameter(MultibandExpansionBand1, hl[1], ear)
                && SetParameter(MultibandExpansionBand2, hl[2], ear)
                && SetParameter(MultibandExpansionBand3, hl[3], ear)
                && SetParameter(MultibandExpansionBand4, hl[4], ear)
                && SetParameter(MultibandExpansionBand5, hl[5], ear)
                && SetParameter(MultibandExpansionBand6, hl[6], ear)
                && SetParameter(MultibandExpansionBand7, hl[7], ear)
                && SetParameter(MultibandExpansionBand8, hl[8], ear);

        }


        ///////////////////////////////////////
        // TEMPORAL DISTORTION SIMULATION
        ///////////////////////////////////////


        /// <summary>
        /// Get one float value of Hz from a T_HLTemporalDistortionBandUpperLimit enum value
        /// </summary>
        /// <param name="bandLimit"></param>
        /// <returns></returns>
        public static float FromBandUpperLimitEnumToFloat(T_HLTemporalDistortionBandUpperLimit bandLimit)
        {
            switch (bandLimit)
            {
                case T_HLTemporalDistortionBandUpperLimit.HZ_UL_200:
                    return 200.0f;
                case T_HLTemporalDistortionBandUpperLimit.HZ_UL_400:
                    return 400.0f;
                case T_HLTemporalDistortionBandUpperLimit.HZ_UL_800:
                    return 800.0f;
                case T_HLTemporalDistortionBandUpperLimit.HZ_UL_1600:
                    return 1600.0f;
                case T_HLTemporalDistortionBandUpperLimit.HZ_UL_3200:
                    return 3200.0f;
                case T_HLTemporalDistortionBandUpperLimit.HZ_UL_6400:
                    return 6400.0f;
                case T_HLTemporalDistortionBandUpperLimit.HZ_UL_12800:
                    return 12800.0f;
                default:
                    return 0.0f;
            }
        }

        /// <summary>
        /// Get one T_HLTemporalDistortionBandUpperLimit enum value from a float value in Hz
        /// </summary>
        /// <param name="bandLimitHz"></param>
        /// <returns></returns>
        public static T_HLTemporalDistortionBandUpperLimit FromFloatToBandUpperLimitEnum(float bandLimitHz)
        {
            if (Mathf.Abs(bandLimitHz - 200.0f) < 0.01)
                return T_HLTemporalDistortionBandUpperLimit.HZ_UL_200;
            if (Mathf.Abs(bandLimitHz - 400.0f) < 0.01)
                return T_HLTemporalDistortionBandUpperLimit.HZ_UL_400;
            if (Mathf.Abs(bandLimitHz - 800.0f) < 0.01)
                return T_HLTemporalDistortionBandUpperLimit.HZ_UL_800;
            if (Mathf.Abs(bandLimitHz - 1600.0f) < 0.01)
                return T_HLTemporalDistortionBandUpperLimit.HZ_UL_1600;
            if (Mathf.Abs(bandLimitHz - 3200.0f) < 0.01)
                return T_HLTemporalDistortionBandUpperLimit.HZ_UL_3200;
            if (Mathf.Abs(bandLimitHz - 6400.0f) < 0.01)
                return T_HLTemporalDistortionBandUpperLimit.HZ_UL_6400;
            if (Mathf.Abs(bandLimitHz - 12800.0f) < 0.01)
                return T_HLTemporalDistortionBandUpperLimit.HZ_UL_12800;

            return T_HLTemporalDistortionBandUpperLimit.HZ_UL_WRONG;
        }

        /// <summary>
        /// Get the zero and one autocorrelation coefficients of the jitter noise source for Temporal distortion in one ear.
        /// The coefficient one is normalized with respect to coefficient zero.
        /// </summary>
        /// <param name="ear"></param>
        /// <param name="coef0"></param>
        /// <param name="coef1"></param>
        /// <returns></returns>
        public bool GetAutocorrelationCoefficients(T_ear ear, out float coef0, out float coef1)
        {
            coef0 = 0.0f;
            coef1 = 0.0f;
            if (ear == T_ear.LEFT)
            {
                if (!hlMixer.GetFloat("HL3DTI_TA_Autocor0_Get_Left", out coef0)) return false;
                if (!hlMixer.GetFloat("HL3DTI_TA_Autocor1_Get_Left", out coef1)) return false;
                coef1 = coef1 / coef0;
                return true;
            }
            if (ear == T_ear.RIGHT)
            {
                if (!hlMixer.GetFloat("HL3DTI_TA_Autocor0_Get_Right", out coef0)) return false;
                if (!hlMixer.GetFloat("HL3DTI_TA_Autocor1_Get_Right", out coef1)) return false;
                coef1 = coef1 / coef0;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set all parameters of temporal distortion module from one of the hardcoded presets
        /// </summary>
        /// <param name="ear"></param>
        /// <param name="preset"></param>
        /// <returns></returns>
        public bool SetTemporalDistortionFromPreset(T_ear ear, T_HLPreset preset)
        {
            //if (ear == T_ear.BOTH)
            //{
            //    if (!SetTemporalDistortionFromPreset(T_ear.LEFT, preset)) return false;
            //    return SetTemporalDistortionFromPreset(T_ear.RIGHT, preset);
            //}

            T_HLTemporalDistortionBandUpperLimit bandUpperLimit;
            float whiteNoisePower;
            float bandwidth;
            float LRSync;
            GetTemporalDistortionPresetValues(preset, out bandUpperLimit, out whiteNoisePower, out bandwidth, out LRSync);

            //        if (!SetTemporalDistortionBandUpperLimit(ear, bandUpperLimit)) return false;
            //if (!SetTemporalDistortionWhiteNoisePower(ear, whiteNoisePower)) return false;
            //if (!SetTemporalDistortionBandwidth(ear, bandwidth)) return false;
            //return SetTemporalDistortionLeftRightSynchronicity(LRSync);


            return SetParameter(TemporalDistortionBandUpperLimit, FromBandUpperLimitEnumToFloat(bandUpperLimit), ear)
                && SetParameter(TemporalDistortionWhiteNoisePower, whiteNoisePower, ear)
                && SetParameter(TemporalDistortionNoiseBandwidth, bandwidth, ear)
                && SetParameter(TemporalDistortionLRSyncAmount, LRSync);

        }

        /// <summary>
        /// Get all parameter values from one of the hardcoded presets for temporal distortion
        /// </summary>
        /// <param name="preset"></param>
        /// <param name="bandUpperLimit"></param>
        /// <param name="whiteNoisePower"></param>
        /// <param name="bandWidth"></param>
        /// <param name="LRSync"></param>
        public static void GetTemporalDistortionPresetValues(T_HLPreset preset, out T_HLTemporalDistortionBandUpperLimit bandUpperLimit, out float whiteNoisePower, out float bandWidth, out float LRSync)
        {
            switch (preset)
            {
                case T_HLPreset.HL_PRESET_MILD:
                    bandUpperLimit = T_HLTemporalDistortionBandUpperLimit.HZ_UL_1600;
                    whiteNoisePower = 0.4f;
                    bandWidth = 700.0f;
                    LRSync = 0.0f;
                    break;

                case T_HLPreset.HL_PRESET_MODERATE:
                    bandUpperLimit = T_HLTemporalDistortionBandUpperLimit.HZ_UL_3200;
                    whiteNoisePower = 0.8f;
                    bandWidth = 850.0f;
                    LRSync = 0.0f;
                    break;

                case T_HLPreset.HL_PRESET_SEVERE:
                    bandUpperLimit = T_HLTemporalDistortionBandUpperLimit.HZ_UL_12800;
                    whiteNoisePower = 1.0f;
                    bandWidth = 1000.0f;
                    LRSync = 0.0f;
                    break;

                case T_HLPreset.HL_PRESET_NORMAL:
                default:
                    bandUpperLimit = T_HLTemporalDistortionBandUpperLimit.HZ_UL_1600;
                    whiteNoisePower = 0.0f;
                    bandWidth = 500.0f;
                    LRSync = 0.0f;
                    break;
            }
        }

        ///////////////////////////////////////
        // FREQUENCY SMEARING SIMULATION
        ///////////////////////////////////////


        /// <summary>
        /// Set all parameters of frequency smearing module from one of the hardcoded presets
        /// </summary>
        /// <param name="ear"></param>
        /// <param name="preset"></param>
        /// <returns></returns>
        public bool SetFrequencySmearingFromPreset(T_ear ear, T_HLPreset preset)
        {
            if (ear == T_ear.BOTH)
            {
                if (!SetFrequencySmearingFromPreset(T_ear.LEFT, preset)) return false;
                return SetFrequencySmearingFromPreset(T_ear.RIGHT, preset);
            }

            int downSize, upSize;
            float downHz, upHz;

            T_HLFrequencySmearingApproach approach = GetParameter<T_HLFrequencySmearingApproach>(FrequencySmearingApproach, ear);

            if (approach == T_HLFrequencySmearingApproach.Graf)
            {
                GetFrequencySmearingGrafPresetValues(preset, out downSize, out upSize, out downHz, out upHz);

                return SetParameter(Parameter.FrequencySmearingDownSize, downSize, ear)
                    && SetParameter(Parameter.FrequencySmearingDownHz, downHz, ear)
                    && SetParameter(Parameter.FrequencySmearingUpSize, upSize, ear)
                    && SetParameter(Parameter.FrequencySmearingUpHz, upHz, ear);
            }
            else
            {
                Debug.Assert(approach == T_HLFrequencySmearingApproach.BaerMoore);
                GetFrequencySmearingBaerMoorePresetValues(preset, out downHz, out upHz);
                return SetParameter(FrequencySmearingDownHz, downHz, ear)
                    && SetParameter(FrequencySmearingUpHz, upHz, ear);

            }
        }

        /// <summary>
        /// Get all parameter values from one of the hardcoded presets for frequency smearing
        /// </summary>
        /// <param name="preset"></param>
        /// <param name="bandUpperLimit"></param>
        /// <param name="whiteNoisePower"></param>
        /// <param name="bandWidth"></param>
        /// <param name="LRSync"></param>
        public static void GetFrequencySmearingGrafPresetValues(T_HLPreset preset, out int downSize, out int upSize, out float downHz, out float upHz)
        {
            switch (preset)
            {
                case T_HLPreset.HL_PRESET_MILD:
                    downSize = 15;
                    upSize = 15;
                    downHz = 35.0f;
                    upHz = 35.0f;
                    break;

                case T_HLPreset.HL_PRESET_MODERATE:
                    downSize = 100;
                    upSize = 100;
                    downHz = 150.0f;
                    upHz = 150.0f;
                    break;

                case T_HLPreset.HL_PRESET_SEVERE:
                    downSize = 150;
                    upSize = 150;
                    downHz = 650.0f;
                    upHz = 650.0f;
                    break;

                case T_HLPreset.HL_PRESET_NORMAL:
                default:
                    downSize = 15;
                    upSize = 15;
                    downHz = 0.0f;
                    upHz = 0.0f;
                    break;
            }
        }

        public static void GetFrequencySmearingBaerMoorePresetValues(T_HLPreset preset, out float downHz, out float upHz)
        {
            switch (preset)
            {
                case T_HLPreset.HL_PRESET_MILD:
                    downHz = 1.1f;
                    upHz = 1.6f;
                    break;
                case T_HLPreset.HL_PRESET_MODERATE:
                    downHz = 1.6f;
                    upHz = 2.4f;
                    break;
                case T_HLPreset.HL_PRESET_SEVERE:
                    downHz = 2.0f;
                    upHz = 4.0f;
                    break;
                case T_HLPreset.HL_PRESET_NORMAL:
                default:
                    downHz = 1.01f;
                    upHz = 1.01f;
                    break;
            }
        }

        /////////////////////////////////////////////////////////////////////
        // AUXILIARY FUNCTIONS
        /////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Get char with the letter corresponding to one curve of HL Classification Scale
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static char FromClassificationScaleCurveToChar(T_HLClassificationScaleCurve curve)
        {
            char result = ' ';
            switch (curve)
            {
                case T_HLClassificationScaleCurve.HL_CS_A:
                    result = 'A';
                    break;
                case T_HLClassificationScaleCurve.HL_CS_B:
                    result = 'B';
                    break;
                case T_HLClassificationScaleCurve.HL_CS_C:
                    result = 'C';
                    break;
                case T_HLClassificationScaleCurve.HL_CS_D:
                    result = 'D';
                    break;
                case T_HLClassificationScaleCurve.HL_CS_E:
                    result = 'E';
                    break;
                case T_HLClassificationScaleCurve.HL_CS_F:
                    result = 'F';
                    break;
                case T_HLClassificationScaleCurve.HL_CS_G:
                    result = 'G';
                    break;
                case T_HLClassificationScaleCurve.HL_CS_H:
                    result = 'H';
                    break;
                case T_HLClassificationScaleCurve.HL_CS_I:
                    result = 'I';
                    break;
                case T_HLClassificationScaleCurve.HL_CS_J:
                    result = 'J';
                    break;
                case T_HLClassificationScaleCurve.HL_CS_K:
                    result = 'K';
                    break;
                default:
                    result = ' ';
                    break;
            }
            return result;
        }

        /// <summary>
        /// Get string with the letter and description of one curve of HL Classification Scale
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static string FromClassificationScaleCurveToString(T_HLClassificationScaleCurve curve)
        {
            string result = "";
            switch (curve)
            {
                case T_HLClassificationScaleCurve.HL_CS_NOLOSS:
                    result = "No hearing loss";
                    break;
                case T_HLClassificationScaleCurve.HL_CS_A:
                    result = "A (Loss only on frequencies starting from 4000Hz and above)";
                    break;
                case T_HLClassificationScaleCurve.HL_CS_B:
                    result = "B (Loss only on frequencies starting from 2000Hz and above)";
                    break;
                case T_HLClassificationScaleCurve.HL_CS_C:
                    result = "C (Loss only on frequencies starting from 1000Hz and above)";
                    break;
                case T_HLClassificationScaleCurve.HL_CS_D:
                    result = "D (Loss only on frequencies starting from 500Hz and above)";
                    break;
                case T_HLClassificationScaleCurve.HL_CS_E:
                    result = "E (Loss only on frequencies starting from 250Hz and above)";
                    break;
                case T_HLClassificationScaleCurve.HL_CS_F:
                    result = "F (Peak loss at 250Hz)";
                    break;
                case T_HLClassificationScaleCurve.HL_CS_G:
                    result = "G (Peak loss at 500Hz)";
                    break;
                case T_HLClassificationScaleCurve.HL_CS_H:
                    result = "H (Peak loss at 1000Hz)";
                    break;
                case T_HLClassificationScaleCurve.HL_CS_I:
                    result = "I (Peak loss at 2000Hz)";
                    break;
                case T_HLClassificationScaleCurve.HL_CS_J:
                    result = "J (Peak loss at 4000Hz)";
                    break;
                case T_HLClassificationScaleCurve.HL_CS_K:
                    result = "K (Constant Slope)";
                    break;
                default:
                    result = "Unknown curve!";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Get string with the name of one severity of HL Classification Scale
        /// </summary>
        /// <param name="severity"></param>
        /// <returns></returns>
        public static string FromClassificationScaleSeverityToString(T_HLClassificationScaleSeverity severity)
        {
            string result = "";
            switch (severity)
            {
                case T_HLClassificationScaleSeverity.HL_CS_SEVERITY_NOLOSS:
                    result = "No loss";
                    break;
                case T_HLClassificationScaleSeverity.HL_CS_SEVERITY_MILD:
                    result = "Mild";
                    break;
                case T_HLClassificationScaleSeverity.HL_CS_SEVERITY_MILDMODERATE:
                    result = "Mild-moderate";
                    break;
                case T_HLClassificationScaleSeverity.HL_CS_SEVERITY_MODERATE:
                    result = "Moderate";
                    break;
                case T_HLClassificationScaleSeverity.HL_CS_SEVERITY_MODERATESEVERE:
                    result = "Moderate-severe";
                    break;
                case T_HLClassificationScaleSeverity.HL_CS_SEVERITY_SEVERE:
                    result = "Severe";
                    break;
                case T_HLClassificationScaleSeverity.HL_CS_SEVERITY_PROFOUND:
                    result = "Profound";
                    break;
                default:
                    result = "Unknown severity!";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Get float value which codes one curve of the HL Classification Scale
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static float FromClassificationScaleCurveToFloat(T_HLClassificationScaleCurve curve)
        {
            float result = (float)(int)curve;
            return result;
        }

        /// <summary>
        /// Get int value which codes one severity of the HL Classification Scale
        /// </summary>
        /// <param name="severity"></param>
        /// <returns></returns>
        public static int FromClassificationScaleSeverityToInt(T_HLClassificationScaleSeverity severity)
        {
            return (int)severity;
        }

        /// <summary>
        /// Private method to get the HL value for one slope of the HL Classification scale
        /// </summary>
        /// <param name="slope"></param>
        /// <returns></returns>
        static float GetHLForSlope(int slope)
        {
            switch (slope)
            {
                case 0: return 0;
                case 1: return 10;
                case 2: return 20;
                case 3: return 30;
                case 4: return 40;
                case 5: return 50;
                case 6: return 60;
            }
            return 0;
        }

        /// <summary>
        /// Private method to get the HL value for one severity of the HL Classification scale
        /// </summary>
        /// <param name="severity"></param>
        /// <returns></returns>
        static float GetHLForSeverity(T_HLClassificationScaleSeverity severity)
        {
            switch (severity)
            {
                case T_HLClassificationScaleSeverity.HL_CS_SEVERITY_NOLOSS: return 0;
                case T_HLClassificationScaleSeverity.HL_CS_SEVERITY_MILD: return 21;
                case T_HLClassificationScaleSeverity.HL_CS_SEVERITY_MILDMODERATE: return 33;
                case T_HLClassificationScaleSeverity.HL_CS_SEVERITY_MODERATE: return 48;
                case T_HLClassificationScaleSeverity.HL_CS_SEVERITY_MODERATESEVERE: return 63;
                case T_HLClassificationScaleSeverity.HL_CS_SEVERITY_SEVERE: return 81;
                case T_HLClassificationScaleSeverity.HL_CS_SEVERITY_PROFOUND: return 100;
            }
            return 0;
        }

        /// <summary>
        /// Get all hearing loss values (in dBHL) for one curve, slope and severity of HL Classification scale
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="slope"></param>
        /// <param name="hl"></param>
        static public void GetClassificationScaleHL(T_HLClassificationScaleCurve curve, int slope, T_HLClassificationScaleSeverity severity, out List<float> hl)
        {
            float x = GetHLForSlope(slope);
            hl = new List<float>();

            // Apply curve and slope
            switch (curve)
            {
                case T_HLClassificationScaleCurve.HL_CS_A: hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(x / 2); hl.Add(x); hl.Add(x); break;
                case T_HLClassificationScaleCurve.HL_CS_B: hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(x / 2); hl.Add(x); hl.Add(x); hl.Add(x); break;
                case T_HLClassificationScaleCurve.HL_CS_C: hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(x / 2); hl.Add(x); hl.Add(x); hl.Add(x); hl.Add(x); break;
                case T_HLClassificationScaleCurve.HL_CS_D: hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(x / 2); hl.Add(x); hl.Add(x); hl.Add(x); hl.Add(x); hl.Add(x); break;
                case T_HLClassificationScaleCurve.HL_CS_E: hl.Add(0); hl.Add(0); hl.Add(x / 2); hl.Add(x); hl.Add(x); hl.Add(x); hl.Add(x); hl.Add(x); hl.Add(x); break;
                case T_HLClassificationScaleCurve.HL_CS_F: hl.Add(0); hl.Add(0); hl.Add(x); hl.Add(x / 2); hl.Add(x / 2); hl.Add(x / 2); hl.Add(x / 2); hl.Add(x / 2); hl.Add(x / 2); break;
                case T_HLClassificationScaleCurve.HL_CS_G: hl.Add(0); hl.Add(0); hl.Add(x / 2); hl.Add(x); hl.Add(x / 2); hl.Add(x / 2); hl.Add(x / 2); hl.Add(x / 2); hl.Add(x / 2); break;
                case T_HLClassificationScaleCurve.HL_CS_H: hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(x / 2); hl.Add(x); hl.Add(x / 2); hl.Add(x / 2); hl.Add(x / 2); hl.Add(x / 2); break;
                case T_HLClassificationScaleCurve.HL_CS_I: hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(x / 2); hl.Add(x); hl.Add(x / 2); hl.Add(x / 2); hl.Add(x / 2); break;
                case T_HLClassificationScaleCurve.HL_CS_J: hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(x / 2); hl.Add(x); hl.Add(x / 2); hl.Add(x / 2); break;
                case T_HLClassificationScaleCurve.HL_CS_K: hl.Add(0); hl.Add(0); hl.Add(x / 6); hl.Add(2 * x / 6); hl.Add(3 * x / 6); hl.Add(4 * x / 6); hl.Add(5 * x / 6); hl.Add(x); hl.Add(x); break;
                default: hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(0); hl.Add(0); break;
            }

            // Apply severity
            for (int c = 0; c < hl.Count; c++)
                hl[c] += GetHLForSeverity(severity);
        }


    }
}