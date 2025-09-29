//-----------------------------------------------------------------------
// <copyright file="ArrayFormatter.cs" company="Sirenix IVS">
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

using System;

namespace OdinSerializer
{
    /// <summary>
    ///     Formatter for all non-primitive one-dimensional arrays.
    /// </summary>
    /// <typeparam name="T">The element type of the formatted array.</typeparam>
    /// <seealso cref="BaseFormatter{T[]}" />
    public sealed class ArrayFormatter<T> : BaseFormatter<T[]>
    {
        private static readonly Serializer<T> valueReaderWriter = Serializer.Get<T>();

        /// <summary>
        ///     Returns null.
        /// </summary>
        /// <returns>
        ///     A null value.
        /// </returns>
        protected override T[] GetUninitializedObject() => null;

        /// <summary>
        ///     Provides the actual implementation for deserializing a value of type <see cref="T" />.
        /// </summary>
        /// <param name="value">
        ///     The uninitialized value to serialize into. This value will have been created earlier using
        ///     <see cref="BaseFormatter{T}.GetUninitializedObject" />.
        /// </param>
        /// <param name="reader">The reader to deserialize with.</param>
        protected override void DeserializeImplementation(ref T[] value, IDataReader reader)
        {
            string name;
            EntryType entry = reader.PeekEntry(out name);

            if (entry == EntryType.StartOfArray)
            {
                long length;
                reader.EnterArray(out length);

                value = new T[length];

                // We must remember to register the array reference ourselves, since we return null in GetUninitializedObject
                RegisterReferenceID(value, reader);

                // There aren't any OnDeserializing callbacks on arrays.
                // Hence we don't invoke this.InvokeOnDeserializingCallbacks(value, reader, context);
                for (var i = 0; i < length; i++)
                {
                    if (reader.PeekEntry(out name) == EntryType.EndOfArray)
                    {
                        reader.Context.Config.DebugContext.LogError("Reached end of array after " + i +
                                                                    " elements, when " + length +
                                                                    " elements were expected.");
                        break;
                    }

                    value[i] = valueReaderWriter.ReadValue(reader);

                    if (reader.PeekEntry(out name) == EntryType.EndOfStream)
                    {
                        break;
                    }
                }

                reader.ExitArray();
            }
            else
            {
                reader.SkipEntry();
            }
        }

        /// <summary>
        ///     Provides the actual implementation for serializing a value of type <see cref="T" />.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="writer">The writer to serialize with.</param>
        protected override void SerializeImplementation(ref T[] value, IDataWriter writer)
        {
            try
            {
                writer.BeginArrayNode(value.Length);

                for (var i = 0; i < value.Length; i++)
                {
                    valueReaderWriter.WriteValue(value[i], writer);
                }
            }
            finally
            {
                writer.EndArrayNode();
            }
        }
    }

    public sealed class WeakArrayFormatter : WeakBaseFormatter
    {
        private readonly Type ElementType;
        private readonly Serializer ValueReaderWriter;

        public WeakArrayFormatter(Type arrayType, Type elementType) : base(arrayType)
        {
            ValueReaderWriter = Serializer.Get(elementType);
            ElementType = elementType;
        }

        protected override object GetUninitializedObject() => null;

        protected override void DeserializeImplementation(ref object value, IDataReader reader)
        {
            string name;
            EntryType entry = reader.PeekEntry(out name);

            if (entry == EntryType.StartOfArray)
            {
                long length;
                reader.EnterArray(out length);

                var array = Array.CreateInstance(ElementType, length);
                value = array;

                // We must remember to register the array reference ourselves, since we return null in GetUninitializedObject
                RegisterReferenceID(value, reader);

                // There aren't any OnDeserializing callbacks on arrays.
                // Hence we don't invoke this.InvokeOnDeserializingCallbacks(value, reader, context);
                for (var i = 0; i < length; i++)
                {
                    if (reader.PeekEntry(out name) == EntryType.EndOfArray)
                    {
                        reader.Context.Config.DebugContext.LogError("Reached end of array after " + i +
                                                                    " elements, when " + length +
                                                                    " elements were expected.");
                        break;
                    }

                    array.SetValue(ValueReaderWriter.ReadValueWeak(reader), i);

                    if (reader.PeekEntry(out name) == EntryType.EndOfStream)
                    {
                        break;
                    }
                }

                reader.ExitArray();
            }
            else
            {
                reader.SkipEntry();
            }
        }

        protected override void SerializeImplementation(ref object value, IDataWriter writer)
        {
            var array = (Array)value;

            try
            {
                var length = array.Length;
                writer.BeginArrayNode(length);

                for (var i = 0; i < length; i++)
                {
                    ValueReaderWriter.WriteValueWeak(array.GetValue(i), writer);
                }
            }
            finally
            {
                writer.EndArrayNode();
            }
        }
    }
}
