//-----------------------------------------------------------------------
// <copyright file="GlobalSerializationConfig.cs" company="Sirenix IVS">
// Copyright (c) 2018 Sirenix IVS
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

namespace OdinSerializer
{
    /// <summary>
    ///     Contains global configuration options for the serialization system.
    /// </summary>
    public class GlobalSerializationConfig
    {
        /// <summary>
        ///     Gets the global configuration instance.
        /// </summary>
        public static GlobalSerializationConfig Instance { get; } = new();

        /// <summary>
        ///     Gets the logger.
        /// </summary>
        public ILogger Logger => DefaultLoggers.UnityLogger;

        /// <summary>
        ///     Gets the editor serialization format.
        /// </summary>
        public DataFormat EditorSerializationFormat => DataFormat.Nodes;

        /// <summary>
        ///     Gets the build serialization format.
        /// </summary>
        public DataFormat BuildSerializationFormat => DataFormat.Binary;

        /// <summary>
        ///     Gets the logging policy.
        /// </summary>
        public LoggingPolicy LoggingPolicy => LoggingPolicy.LogErrors;

        /// <summary>
        ///     Gets the error handling policy.
        /// </summary>
        public ErrorHandlingPolicy ErrorHandlingPolicy => ErrorHandlingPolicy.Resilient;

        internal static bool HasInstanceLoaded =>
            // TODO: @Integration: If you store your config in an asset or file somewhere, return true here if it is loaded, otherwise false.
            // If your config is stored in a Unity asset, do NOT load it here; this property is often called from the
            // serialization thread, meaning you are not allowed to use Unity's API for loading assets here.
            // If this value is false, default configuration values will be used - the same defaults as are set in this class.
            true;

        internal static void LoadInstanceIfAssetExists()
        {
            // TODO: @Integration: If you store your config in an asset or file somewhere, load it here.
        }
    }
}
