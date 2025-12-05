using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.API
{
    /// <summary>
    ///     Class responsible for loading all the NoiseType and FractalType implementations
    ///     and generating the associated shaders
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class NoiseLib
    {
        private static INoiseType[] _sNoiseTypes;
        private static string[] _sNoiseNames;
        private static IFractalType[] _sFractalTypes;
        private static string[] _sFractalNames;

        private static readonly BindingFlags _sBindingFlags = BindingFlags.Public |
                                                              BindingFlags.NonPublic |
                                                              BindingFlags.Static |
                                                              BindingFlags.Instance |
                                                              BindingFlags.FlattenHierarchy;

        private static Dictionary<Type, INoiseShaderGenerator> _sGenerators;

        // generator type => fractal type => shader
        private static Dictionary<Type, Dictionary<Type, Shader>> _sGeneratedShaderMap;

        // generator type => fractal type => material
        private static Dictionary<Type, Dictionary<Type, Material>> _sGeneratedMaterialMap;

        static NoiseLib()
        {
            GenerateHeaderFiles();
            GenerateShaders();
        }

        /*==========================================================================================

            Get Noise

        ==========================================================================================*/

        // TODO(wyatt): this needs to be read-only
        private static INoiseType[] GetAllNoiseTypes() => _sNoiseTypes;

        // TODO(wyatt): this needs to be read-only
        private static string[] GetNoiseNames() => _sNoiseNames;

        /// <summary>
        ///     Returns the Singleton instance for the specified NoiseType
        /// </summary>
        /// <param name="noiseName"> The name of the NoiseType </param>
        public static INoiseType GetNoiseTypeInstance(string noiseName)
        {
            var index = GetNoiseIndex(noiseName);

            return index == -1 ? null : _sNoiseTypes[index];
        }

        /// <summary>
        ///     Returns the global index for the specified NoiseType
        /// </summary>
        /// <param name="noiseName"> The name of the NoiseType </param>
        public static int GetNoiseIndex(string noiseName)
        {
            var index = -1;

            INoiseType[] instances = _sNoiseTypes;

            for (var i = 0; i < instances.Length; ++i)
            {
                NoiseTypeDescriptor desc = instances[i].GetDescription();

                if (noiseName.CompareTo(desc.Name) == 0)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /*==========================================================================================

            Get Fractals

        ==========================================================================================*/

        // TODO(wyatt): this needs to be a read-only collection
        private static IFractalType[] GetAllFractalTypes() => _sFractalTypes;

        // TODO(wyatt): this needs to be a read-only collection
        private static string[] GetFractalNames() => _sFractalNames;

        /// <summary>
        ///     Returns the Singleton instance for the specified FractalType implementation
        /// </summary>
        public static IFractalType GetFractalTypeInstance(string fractalName)
        {
            var index = GetFractalIndex(fractalName);

            return index == -1 ? null : _sFractalTypes[index];
        }

        /// <summary>
        ///     Returns the Singleton instance for the specified FractalType
        /// </summary>
        /// <param name="t"> The Type for the FractalType implementation </param>
        public static IFractalType GetFractalTypeInstance(Type t)
        {
            IFractalType[] instances = _sFractalTypes;

            for (var i = 0; i < instances.Length; ++i)
            {
                IFractalType fractalType = instances[i];

                if (fractalType.GetType() == t)
                {
                    return instances[i];
                }
            }

            return null;
        }

        /// <summary>
        ///     Returns the global FractalType index associated with provided NoiseSettings instance
        /// </summary>
        /// <param name="noise"> The NoiseSettings instance </param>
        public static int GetFractalIndex(NoiseSettings noise) => GetFractalIndex(noise.DomainSettings.FractalTypeName);

        /// <summary>
        ///     Returns the global index for the specified FractalType
        /// </summary>
        /// <param name="fractalName"> The name of the FractalType </param>
        public static int GetFractalIndex(string fractalName)
        {
            if (string.IsNullOrEmpty(fractalName))
            {
                return -1;
            }

            IFractalType[] instances = _sFractalTypes;

            var fractalNames = GetFractalNames();

            for (var i = 0; i < instances.Length && i < fractalNames.Length; ++i)
            {
                if (fractalName.CompareTo(fractalNames[i]) == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        /*=========================================================================

            Gather Types

        =========================================================================*/

        private static bool IsSubclassOfGenericType(Type t, Type genericType)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == genericType)
            {
                return false;
            }

            for (t = t.BaseType; t != null; t = t.BaseType)
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<Type> GetSubclassesOfGenericType(Type[] types, Type genericType) =>
            types.Where(t => IsSubclassOfGenericType(t, genericType));

        private static void GatherNoiseTypes()
        {
            var instances = new List<INoiseType>();
            var names = new List<string>();

            var types = new List<Type>();

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] assemblyTypes = null;

                try
                {
                    assemblyTypes = asm.GetTypes();
                }
                catch (Exception)
                {
                    Debug.Log("NoiseLib::GatherNoiseTypes: Failed to get types from assembly: " + asm);
                    assemblyTypes = null;
                }

                if (assemblyTypes != null)
                {
                    types.AddRange(GetSubclassesOfGenericType(assemblyTypes, typeof(NoiseType<>)));
                }
            }

            foreach (Type t in types)
            {
                PropertyInfo propertyInfo = t.GetProperty("Instance", _sBindingFlags);
                MethodInfo methodInfo = propertyInfo.GetGetMethod();
                var instance = (INoiseType)methodInfo.Invoke(null, null);

                NoiseTypeDescriptor desc = instance.GetDescription();

                if (string.IsNullOrEmpty(desc.Name))
                {
                    Debug.LogError("NoiseType name cannot be null or empty! Skipping noise type: " + t);
                    continue;
                }

                instances.Add(instance);
                names.Add(desc.Name);
            }

            _sNoiseTypes = instances.ToArray();
            _sNoiseNames = names.ToArray();
        }

        private static void GatherFractalTypes()
        {
            var instances = new List<IFractalType>();
            var names = new List<string>();

            var types = new List<Type>();

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] assemblyTypes = null;

                try
                {
                    assemblyTypes = asm.GetTypes();
                }
                catch (Exception)
                {
                    Debug.Log("NoiseLib::GatherFractalTypes: Failed to get types from assembly: " + asm);
                    assemblyTypes = null;
                }

                if (assemblyTypes != null)
                {
                    types.AddRange(GetSubclassesOfGenericType(assemblyTypes, typeof(FractalType<>)));
                }
            }

            foreach (Type t in types)
            {
                PropertyInfo propertyInfo = t.GetProperty("Instance", _sBindingFlags);
                MethodInfo methodInfo = propertyInfo.GetGetMethod();
                var instance = (IFractalType)methodInfo.Invoke(null, null);
                FractalTypeDescriptor desc = instance.GetDescription();

                if (string.IsNullOrEmpty(desc.Name))
                {
                    Debug.LogError("FractalType name cannot be null or empty! Skipping fractal type: " + desc.Name);
                    continue;
                }

                instances.Add(instance);
                names.Add(desc.Name);
            }

            _sFractalTypes = instances.ToArray();
            _sFractalNames = names.ToArray();
        }

        /*==========================================================================================

            Load Source

        ==========================================================================================*/

        private static string[] LoadNoiseSource(INoiseType[] noiseTypes)
        {
            // load noise source
            var noiseSource = new List<string>();
            foreach (INoiseType noise in noiseTypes)
            {
                NoiseTypeDescriptor desc = noise.GetDescription();
                var path = desc.SourcePath;
                string source = null;

                if (!File.Exists(path))
                {
                    Debug.LogError("NoiseLib: Noise Include File could not be found! Skipping generation of: " +
                                   desc.Name);
                }
                else
                {
                    using (var sr = new StreamReader(path))
                    {
                        source = sr.ReadToEnd();
                    }
                }

                noiseSource.Add(source);
            }

            return noiseSource.ToArray();
        }

        private static string[] LoadFractalSource(IFractalType[] fractalTypes)
        {
            // load fractal templates
            var fractalSource = new List<string>();
            foreach (IFractalType fractal in fractalTypes)
            {
                FractalTypeDescriptor desc = fractal.GetDescription();
                var path = desc.TemplatePath;
                string source = null;

                if (!File.Exists(path))
                {
                    Debug.LogError("NoiseLib: Fractal Template File at \"" + path +
                                   "\" could not be found! Skipping generation of: " + desc.Name);
                }
                else
                {
                    using (var sr = new StreamReader(path))
                    {
                        source = sr.ReadToEnd();
                    }
                }

                fractalSource.Add(source);
            }

            return fractalSource.ToArray();
        }

        internal static string GetPropertyDefinitionStr(string prefix, List<HlslInput> inputs)
        {
            var ret = "";

            var charArray = prefix.ToCharArray();
            charArray[0] = char.ToUpper(charArray[0]);
            prefix = new string(charArray);

            for (var i = 0; i < inputs.Count; ++i)
            {
                var name = inputs[i].Name;
                charArray = name.ToCharArray();
                charArray[0] = char.ToUpper(charArray[0]);
                name = new string(charArray);

                ret += string.Format("{0} {1};\n", inputs[i].GetHlslValueTypeString(),
                    string.Format("_{0}{1}", prefix, name));
            }

            return ret;
        }

        internal static string BuildStructString(string structName, List<HlslInput> inputList)
        {
            var structDefStr = "struct " + structName + "\n{\n";

            for (var i = 0; i < inputList.Count; ++i)
            {
                HlslInput input = inputList[i];

                var valueString = input.GetHlslValueTypeString();

                structDefStr += string.Format("\t{0} {1};\n", valueString, input.Name);
            }

            structDefStr += "};\n\n";

            return structDefStr;
        }

        internal static string GetDefaultInputFunctionCallString(string structName) =>
            string.Format("GetDefault{0}()", structName);

        internal static string GetInputFunctionCallString(string structName) => string.Format("Get{0}()", structName);

        internal static string GetDefaultFunctionString(string structName, List<HlslInput> inputList)
        {
            var getDefaultFunctionString =
                string.Format("{0} {1}", structName, GetDefaultInputFunctionCallString(structName)) + "\n{\n";

            getDefaultFunctionString += string.Format("\t{0} ret;\n\n", structName);

            for (var i = 0; i < inputList.Count; ++i)
            {
                HlslInput input = inputList[i];
                var defaultValueString = input.GetDefaultValueString();
                getDefaultFunctionString += string.Format("\tret.{0} = {1};\n", input.Name, defaultValueString);
            }

            getDefaultFunctionString += "\n\treturn ret;\n}\n\n";

            return getDefaultFunctionString;
        }

        internal static string GetPropertyFunctionCallString(string structName) =>
            string.Format("Get{0}()", structName);

        internal static string GetPropertyFunctionStr(string structName, string propertyPrefix, List<HlslInput> inputs)
        {
            var ret = string.Format("{0} {1}", structName, GetPropertyFunctionCallString(structName)) + "\n{\n";

            var charArray = propertyPrefix.ToCharArray();
            charArray[0] = char.ToUpper(charArray[0]);
            propertyPrefix = new string(charArray);

            ret += string.Format("\t{0} ret;\n\n", structName);

            for (var i = 0; i < inputs.Count; ++i)
            {
                var name = inputs[i].Name;
                charArray = name.ToCharArray();
                charArray[0] = char.ToUpper(charArray[0]);
                name = new string(charArray);

                ret += string.Format("\tret.{0} = {1};\n", inputs[i].Name,
                    string.Format("_{0}{1}", propertyPrefix, name));
            }

            ret += "\n\treturn ret;\n}\n\n";

            return ret;
        }

        /*==========================================================================================

            Generate HLSL

        ==========================================================================================*/

        /// <summary>
        ///     Forces generation of the NoiseType and FractalType variant HLSL header files
        /// </summary>
        public static void GenerateHeaderFiles()
        {
            CultureInfo prevCultureInfo = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            GatherNoiseTypes();
            GatherFractalTypes();

            INoiseType[] noiseTypes = _sNoiseTypes;
            IFractalType[] fractalTypes = _sFractalTypes;

            var fractalContents = LoadFractalSource(fractalTypes);
            var noiseContents = LoadNoiseSource(noiseTypes);

            for (var f = 0; f < fractalTypes.Length; ++f)
            {
                var fractalStr = fractalContents[f];

                // dont generate for this fractal type if the source could not be found
                if (fractalStr == null)
                {
                    continue;
                }

                IFractalType fractal = fractalTypes[f];

                for (var n = 0; n < noiseTypes.Length; ++n)
                {
                    var noiseStr = noiseContents[n];

                    // dont generate for this noise type if the source could not be found
                    if (noiseStr == null)
                    {
                        continue;
                    }

                    INoiseType noise = noiseTypes[n];
                    var info = new GeneratedShaderInfo(fractal, noise);

                    var sb = new StringBuilder();

                    sb.Append(Strings.KWarningHeader); // add the DO NOT EDIT warning
                    sb.Append(fractalStr); // add the fractal template

                    info.ReplaceTags(sb);

                    var newContents = sb.ToString();

                    // do some code cleanup
                    newContents = Regex.Replace(newContents, Strings.KRegexDupCommas, ", ");
                    newContents = Regex.Replace(newContents, Strings.KEmptyArgsRight, " )");
                    newContents = Regex.Replace(newContents, Strings.KEmptyArgsLeft, "( ");

                    newContents = NormalizeLineEndings(newContents);

                    var outputDir = info.outputDir;

                    // TODO(wyatt): need to verify this is actually a directory and not a file
                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }

                    string oldContents = null;

                    var fi = new FileInfo(info.generatedIncludePath);

                    if (File.Exists(info.generatedIncludePath))
                    {
                        using (var sr = new StreamReader(info.generatedIncludePath))
                        {
                            oldContents = sr.ReadToEnd();
                            oldContents = NormalizeLineEndings(oldContents);
                        }
                    }

                    if (!fi.IsReadOnly)
                    {
                        if (oldContents == null || newContents.CompareTo(oldContents) != 0)
                        {
                            try
                            {
                                using (var sw = new StreamWriter(info.generatedIncludePath))
                                {
                                    sw.Write(newContents);
                                }
                            }
                            catch (Exception)
                            {
                                // restore previous cultureinfo
                                Thread.CurrentThread.CurrentCulture = prevCultureInfo;
                            }
                        }
                    }
                }
            }

            // restore previous cultureinfo
            Thread.CurrentThread.CurrentCulture = prevCultureInfo;

            //UnityEditor.AssetDatabase.Refresh();
        }

        /*==========================================================================================

            Generate Tool Shaders

        ==========================================================================================*/

        /// <summary>
        ///     Returns a Material associated with the provided Type of NoiseShaderGenerator
        ///     and Type of FractalType
        /// </summary>
        /// <param name="generatorType"> The Type of a NoiseShaderGenerator </param>
        /// <param name="fractalType"> The Type of a FractalType </param>
        public static Material GetGeneratedMaterial(Type generatorType, Type fractalType)
        {
            LoadShadersAndCreateMaterials();

            if (_sGeneratedMaterialMap.ContainsKey(generatorType))
            {
                if (_sGeneratedMaterialMap[generatorType].ContainsKey(fractalType))
                {
                    return _sGeneratedMaterialMap[generatorType][fractalType];
                }
            }

            return null;
        }

        private static string GetShaderName(ShaderGeneratorDescriptor generatorDesc, FractalTypeDescriptor fractalDesc)
        {
            var shaderStr = string.Format("{0}/{1}{2}", generatorDesc.ShaderCategory,
                generatorDesc.Name,
                fractalDesc.Name);

            return shaderStr;
        }

        private static void GatherGenerators()
        {
            IEnumerable<Type> generatorTypes = AllTypesDerivedFrom<INoiseShaderGenerator>.Types
                .Where(t => !t.IsAbstract);

            _sGenerators = new Dictionary<Type, INoiseShaderGenerator>();

            foreach (Type t in generatorTypes)
            {
                PropertyInfo propertyInfo = t.GetProperty("Instance", _sBindingFlags);
                MethodInfo methodInfo = propertyInfo.GetGetMethod();
                var generator = (INoiseShaderGenerator)methodInfo.Invoke(null, null);
                _sGenerators.Add(t, generator);
            }
        }

        private static string NormalizeLineEndings(string str) =>
            str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");

        /// <summary>
        ///     Forces the generation of any shaders that make use of generated noise header files. Gathers all
        ///     the NoiseShaderGenerators and generates shaders based on the ".noisehlsltemplate" file
        ///     provided by that particular NoiseShaderGenerator implementation
        /// </summary>
        public static void GenerateShaders()
        {
            CultureInfo prevCultureInfo = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            GatherGenerators();

            IFractalType[] fractalTypes = _sFractalTypes;
            INoiseType[] noiseTypes = _sNoiseTypes;
            Dictionary<Type, INoiseShaderGenerator> generators = _sGenerators;

            var shaderSb = new StringBuilder();
            var passesSb = new StringBuilder();

            foreach (KeyValuePair<Type, INoiseShaderGenerator> pair in generators)
            {
                shaderSb.Clear();
                passesSb.Clear();

                string shaderTemplateStr = null;

                INoiseShaderGenerator generator = pair.Value;
                ShaderGeneratorDescriptor generatorDesc = generator.GetDescription();

                if (!File.Exists(generatorDesc.TemplatePath))
                {
                    Debug.LogError("Could not find specified template file for noise shader generator: " + generator);
                    Debug.LogError("Path: " + generatorDesc.TemplatePath);
                    continue;
                }

                // load contents of shader template
                using (var sr = new StreamReader(generatorDesc.TemplatePath))
                {
                    shaderTemplateStr = sr.ReadToEnd();
                }

                // find the pass template using regex matching
                Match passTemplateMatch = Regex.Match(shaderTemplateStr, Strings.KRegexPassTemplate1);

                if (!passTemplateMatch.Success)
                {
                    Debug.LogError(
                        $"Could not find pass template in {generatorDesc.TemplatePath}. Skipping noise shader generation for this generator type!!");
                    continue;
                }

                var passTemplateStr = passTemplateMatch.Value;

                // generate shaders for each fractal type
                foreach (IFractalType fractal in fractalTypes)
                {
                    FractalTypeDescriptor fractalDesc = fractal.GetDescription();
                    var fullShaderCategory = GetShaderName(generatorDesc, fractalDesc);

                    shaderSb.Append(Strings.KWarningHeader);
                    shaderSb.Append(shaderTemplateStr);
                    shaderSb.Replace(Strings.KTagShaderCategory, $"\"{fullShaderCategory}\"");

                    // add passes for each noise type
                    foreach (INoiseType noise in noiseTypes)
                    {
                        var info = new GeneratedShaderInfo(fractal, noise);

                        // add to passes string builer
                        passesSb.Append(passTemplateStr);
                        passesSb.AppendLine();
                        passesSb.Replace(Strings.KTagIncludes,
                            string.Format("#include \"{0}\"", info.generatedIncludePath));

                        info.ReplaceTags(passesSb);
                    }

                    // replace template with generated passes
                    var newContents = Regex.Replace(shaderSb.ToString(), Strings.KRegexPassTemplate2,
                        passesSb.ToString());

                    newContents = newContents.Replace(Strings.KTagFractalName, fractalDesc.Name);

                    // load shader contents from disk if it exists
                    var fileName = string.Format("{0}{1}.shader", generatorDesc.Name, fractalDesc.Name);
                    var filePath = string.Format("{0}/{1}", generatorDesc.OutputDir, fileName);

                    if (!Directory.Exists(generatorDesc.OutputDir))
                    {
                        Directory.CreateDirectory(generatorDesc.OutputDir);
                    }

                    string currentContents = null;

                    var fi = new FileInfo(filePath);

                    if (File.Exists(filePath))
                    {
                        using (var sr = new StreamReader(filePath))
                        {
                            currentContents = sr.ReadToEnd();
                            currentContents = NormalizeLineEndings(currentContents);
                        }
                    }

                    // do some code cleanup
                    newContents = Regex.Replace(newContents, Strings.KRegexDupCommas, ", ");
                    newContents = Regex.Replace(newContents, Strings.KEmptyArgsRight, " )");
                    newContents = Regex.Replace(newContents, Strings.KEmptyArgsLeft, "( ");

                    newContents = NormalizeLineEndings(newContents);

                    // only write to file if it is not read-only, ie. if it is one of the generated
                    // shader files that we ship with the TerrainTools package
                    if (!fi.IsReadOnly)
                    {
                        if (currentContents == null || currentContents.CompareTo(newContents) != 0)
                        {
                            try
                            {
                                using (var sw = new StreamWriter(filePath))
                                {
                                    sw.Write(newContents);
                                }
                            }
                            catch (Exception)
                            {
                                // restore previous cultureinfo
                                Thread.CurrentThread.CurrentCulture = prevCultureInfo;
                            }
                        }
                    }

                    shaderSb.Clear();
                    passesSb.Clear();
                }
            }

            // restore previous cultureinfo
            Thread.CurrentThread.CurrentCulture = prevCultureInfo;

            //UnityEditor.AssetDatabase.Refresh();
        }

        private static void LoadShadersAndCreateMaterials()
        {
            IFractalType[] fractalTypes = _sFractalTypes;

            // CHECK IF MATERIALS HAVE ALREADY BEEN INITIALIZED BUT LOST REFERENCE FOR SOME REASON
            // this happens when running brush tests, for example
            if (_sGeneratedShaderMap != null && _sGeneratedMaterialMap != null)
            {
                foreach (KeyValuePair<Type, INoiseShaderGenerator> pair in _sGenerators)
                foreach (IFractalType fractal in fractalTypes)
                {
                    Shader s = _sGeneratedShaderMap[pair.Key][fractal.GetType()];

                    if (s == null)
                    {
                        var shaderPath = GetShaderName(pair.Value.GetDescription(), fractal.GetDescription());
                        s = Shader.Find(shaderPath);

                        _sGeneratedShaderMap[pair.Key][fractal.GetType()] = s;
                    }

                    if (_sGeneratedMaterialMap[pair.Key][fractal.GetType()] == null)
                    {
                        _sGeneratedMaterialMap[pair.Key][fractal.GetType()] = new Material(s);
                    }
                }

                return;
            }

            _sGeneratedShaderMap = new Dictionary<Type, Dictionary<Type, Shader>>();
            _sGeneratedMaterialMap = new Dictionary<Type, Dictionary<Type, Material>>();

            // load related shaders
            foreach (KeyValuePair<Type, INoiseShaderGenerator> pair in _sGenerators)
            {
                _sGeneratedShaderMap.Add(pair.Key, new Dictionary<Type, Shader>());
                _sGeneratedMaterialMap.Add(pair.Key, new Dictionary<Type, Material>());

                foreach (IFractalType fractal in fractalTypes)
                {
                    var shaderPath = GetShaderName(pair.Value.GetDescription(), fractal.GetDescription());
                    var s = Shader.Find(shaderPath);

                    if (s == null)
                    {
                        Debug.LogError($"Could not find shader: {shaderPath}");
                        continue;
                    }

                    _sGeneratedShaderMap[pair.Key].Add(fractal.GetType(), s);
                    _sGeneratedMaterialMap[pair.Key].Add(fractal.GetType(), new Material(s));
                }
            }
        }

        internal static class Strings
        {
            public static readonly string KRegexPassTemplate1 = @"(?<=BEGINPASSTEMPLATE)[^\\]*(?=ENDPASSTEMPLATE)";
            public static readonly string KRegexPassTemplate2 = @"BEGINPASSTEMPLATE[^\\]*ENDPASSTEMPLATE";
            public static readonly string KEmptyArgsLeft = @"\(\s*,";
            public static readonly string KEmptyArgsRight = @",\s*\)";
            public static readonly string KRegexDupCommas = @",\s*,";
            public static readonly string KTagPasses = "${Passes}";
            public static readonly string KTagIncludes = "${Includes}";
            public static readonly string KTagNoiseName = "${NoiseName}";
            public static readonly string KTagFractalName = "${FractalName}";
            public static readonly string KTagVariantName = "${VariantName}";
            public static readonly string KTagShaderCategory = "${ShaderCategory}";
            public static readonly string KTagFunctionParams = "${FunctionParams}";
            public static readonly string KTagFunctionInputs = "${FunctionInputs}";
            public static readonly string KTagGetDefaultFractalInput = "${GetDefaultFractalInput}";
            public static readonly string KTagGetDefaultNoiseInput = "${GetDefaultNoiseInput}";
            public static readonly string KTagGetDefaultInputs = "${GetDefaultInputs}";
            public static readonly string KTagGetInputs = "${GetInputs}";
            public static readonly string KTagFractalDataDefinitions = "${FractalDataDefinitions}";

            public static readonly string KWarningHeader =
                "//////////////////////////////////////////////////////////////////////////\n" +
                "//\n" +
                "//      DO NOT EDIT THIS FILE!! THIS IS AUTOMATICALLY GENERATED!!\n" +
                "//      DO NOT EDIT THIS FILE!! THIS IS AUTOMATICALLY GENERATED!!\n" +
                "//      DO NOT EDIT THIS FILE!! THIS IS AUTOMATICALLY GENERATED!!\n" +
                "//\n" +
                "//////////////////////////////////////////////////////////////////////////\n\n";
        }

        /*============================================================================================

            UI Helpers

        ============================================================================================*/
#if UNITY_EDITOR
        public static string NoiseTypePopup(Rect rect, GUIContent label, string selectedName)
        {
            var names = GetNoiseNames();
            var index = GetNoiseIndex(selectedName);
            index = index < 0 ? 0 : index;

            var newIndex = EditorGUI.Popup(rect, label.text, index, names);
            var newName = names[newIndex];

            if (newName.CompareTo(selectedName) != 0)
            {
                selectedName = newName;
            }

            return selectedName;
        }

        /// <summary>
        ///     Renders a Popup using EditorGUILayout.Popup for all loaded FractalType implementations
        /// </summary>
        /// <param name="label"> Label prefix for the Popup </param>
        /// <param name="selectedName"> The currently selected FractalType name </param>
        public static string FractalTypePopup(Rect rect, GUIContent label, string selectedName)
        {
            var names = GetFractalNames();
            var index = GetFractalIndex(selectedName);
            index = index < 0 ? 0 : index;

            var newIndex = EditorGUI.Popup(rect, label.text, index, names);
            var newName = names[newIndex];

            if (newName.CompareTo(selectedName) != 0)
            {
                selectedName = newName;
            }

            return selectedName;
        }
#endif
    }
}
