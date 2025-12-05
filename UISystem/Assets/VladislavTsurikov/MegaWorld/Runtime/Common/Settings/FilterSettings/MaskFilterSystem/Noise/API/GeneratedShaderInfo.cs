using System.Text;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.API
{
    /// <summary>
    ///     Utility struct for creating all the different shader snippets that are
    ///     used when generating the shaders for the various NoiseTypes and FractalTypes
    /// </summary>
    internal struct GeneratedShaderInfo
    {
        public FractalTypeDescriptor FractalDesc;
        public NoiseTypeDescriptor NoiseDesc;
        public string generatedIncludePath { get; private set; }
        public string additionalIncludePaths { get; }
        public string noiseIncludeStr { get; }
        public string variantName { get; }
        public string outputDir { get; private set; }
        public string noiseStructName { get; }
        public string noiseStructDef { get; private set; }
        public string fractalStructName { get; }
        public string fractalStructDef { get; }
        public string fractalParamStr { get; }
        public string noiseParamStr { get; }
        public string functionInputStr { get; }
        public string functionParamStr { get; }
        public string getDefaultInputsStr { get; }
        public string getInputsStr { get; }
        public string getNoiseInputStr { get; }
        public string getFractalInputStr { get; }
        public string getDefaultFractalInputStr { get; }
        public string getDefaultNoiseInputStr { get; }
        public string fractalPropertyDefStr { get; }
        public int numFractalInputs { get; }
        public int numNoiseInputs { get; }

        public GeneratedShaderInfo(IFractalType fractalType, INoiseType noiseType)
        {
            FractalDesc = fractalType.GetDescription();
            NoiseDesc = noiseType.GetDescription();

            noiseIncludeStr = string.Format("#include \"{0}\"", NoiseDesc.SourcePath);

            if (!string.IsNullOrEmpty(FractalDesc.Name))
            {
                variantName = string.Format("{0}{1}", FractalDesc.Name, NoiseDesc.Name);
            }
            else
            {
                variantName = NoiseDesc.Name;
            }

            // set the path of the generated file. this will be used when writing the file
            // to disk and when adding the include in any generated shaders that use this
            // fractal and noise type variant
            generatedIncludePath = string.Format("{0}/{1}/{2}.hlsl", NoiseDesc.OutputDir,
                FractalDesc.Name,
                NoiseDesc.Name);
            outputDir = string.Format("{0}/{1}", NoiseDesc.OutputDir, FractalDesc.Name);

            fractalStructName = string.Format("{0}FractalInput", FractalDesc.Name);
            noiseStructName = string.Format("{0}NoiseInput", NoiseDesc.Name);
            numFractalInputs = FractalDesc.InputStructDefinition == null ? 0 : FractalDesc.InputStructDefinition.Count;
            numNoiseInputs = NoiseDesc.InputStructDefinition == null ? 0 : NoiseDesc.InputStructDefinition.Count;
            fractalParamStr = null;
            noiseParamStr = null;
            functionInputStr = "";

            // construct include paths string
            additionalIncludePaths = "\n";

            for (var i = 0; i < FractalDesc.AdditionalIncludePaths.Count; ++i)
            {
                additionalIncludePaths += $"#include \"{FractalDesc.AdditionalIncludePaths[i]}\"\n";
            }

            additionalIncludePaths += "\n";

            // generate the string for the fractal type structure as it would appear as a parameter
            // in an HLSL function declaration
            if (numFractalInputs > 0)
            {
                fractalParamStr = string.Format("{0} {1}", fractalStructName, "fractalInput");
            }

            // generate the string for the noise type structure as it would appear as a parameter
            // in an HLSL function declaration
            if (numNoiseInputs > 0)
            {
                noiseParamStr = string.Format("{0} {1}", noiseStructName, "noiseInput");
            }

            // generate the argument string for an HLSL function declaration that would be 
            // using this combination of noise and fractal type structure definitions
            functionParamStr = "";

            if (fractalParamStr != null)
            {
                functionParamStr += fractalParamStr;
                functionInputStr += "fractalInput";
            }

            if (fractalParamStr != null && noiseParamStr != null)
            {
                functionParamStr += ", ";
                functionInputStr += ", ";
            }

            if (noiseParamStr != null)
            {
                functionParamStr += noiseParamStr;
                functionInputStr += "noiseInput";
            }

            fractalStructDef = "";

            if (numFractalInputs > 0)
            {
                fractalStructDef = NoiseLib.BuildStructString(fractalStructName, FractalDesc.InputStructDefinition);

                var getDefaultFuncStr =
                    NoiseLib.GetDefaultFunctionString(fractalStructName, FractalDesc.InputStructDefinition);
                fractalStructDef += $"\n\n{getDefaultFuncStr}\n\n";
            }

            noiseStructDef = "";

            if (numNoiseInputs > 0)
            {
                noiseStructDef = NoiseLib.BuildStructString(noiseStructName, NoiseDesc.InputStructDefinition);
            }

            // get input str construction
            getInputsStr = "";
            getFractalInputStr = NoiseLib.GetInputFunctionCallString(fractalStructName);
            getNoiseInputStr = NoiseLib.GetInputFunctionCallString(fractalStructName);

            if (numFractalInputs > 0)
            {
                getInputsStr += getFractalInputStr;
            }

            if (numFractalInputs > 0 && numNoiseInputs > 0)
            {
                getInputsStr += ", ";
            }

            if (numNoiseInputs > 0)
            {
                getInputsStr += getNoiseInputStr;
            }

            // get default input str construction
            getDefaultInputsStr = "";
            getDefaultFractalInputStr = NoiseLib.GetDefaultInputFunctionCallString(fractalStructName);
            getDefaultNoiseInputStr = NoiseLib.GetDefaultInputFunctionCallString(noiseStructName);

            if (numFractalInputs > 0)
            {
                getDefaultInputsStr += getDefaultFractalInputStr;
            }

            if (numFractalInputs > 0 && numNoiseInputs > 0)
            {
                getDefaultInputsStr += ", ";
            }

            if (numNoiseInputs > 0)
            {
                getDefaultInputsStr += getDefaultNoiseInputStr;
            }

            fractalPropertyDefStr = "";

            if (FractalDesc.InputStructDefinition != null &&
                FractalDesc.InputStructDefinition.Count > 0)
            {
                fractalPropertyDefStr =
                    NoiseLib.GetPropertyDefinitionStr(FractalDesc.Name, FractalDesc.InputStructDefinition);
                fractalPropertyDefStr += "\n" + NoiseLib.GetPropertyFunctionStr(fractalStructName, FractalDesc.Name,
                    FractalDesc.InputStructDefinition);
            }
        }

        public void ReplaceTags(StringBuilder sb)
        {
            var fractalMacroDef = fractalStructName.ToUpper() + "_DEF";
            var guardedFractalDataDefinitions =
                $@"

#ifndef {fractalMacroDef} // [ {fractalMacroDef}
#define {fractalMacroDef}

{fractalStructDef}
{fractalPropertyDefStr}

#endif // ] {fractalMacroDef}

";
#if UNITY_EDITOR
            sb.Replace(NoiseLib.Strings.KTagIncludes,
                noiseIncludeStr + additionalIncludePaths); // add the noise include
            sb.Replace(NoiseLib.Strings.KTagFractalName, FractalDesc.Name); // add fractal name
            sb.Replace(NoiseLib.Strings.KTagNoiseName, NoiseDesc.Name); // add noise name
            sb.Replace(NoiseLib.Strings.KTagVariantName, variantName); // add combined fractal and noise name
            sb.Replace(NoiseLib.Strings.KTagFractalDataDefinitions, guardedFractalDataDefinitions);
            sb.Replace(NoiseLib.Strings.KTagFunctionParams, functionParamStr);
            sb.Replace(NoiseLib.Strings.KTagFunctionInputs, functionInputStr);
            sb.Replace(NoiseLib.Strings.KTagGetDefaultFractalInput, getDefaultFractalInputStr);
            sb.Replace(NoiseLib.Strings.KTagGetDefaultNoiseInput, getDefaultNoiseInputStr);
            sb.Replace(NoiseLib.Strings.KTagGetDefaultInputs, getDefaultInputsStr);
            sb.Replace(NoiseLib.Strings.KTagGetInputs, getInputsStr);
#endif
        }
    }
}
