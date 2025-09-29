//-----------------------------------------------------------------------
// <copyright file="MultiDimensionalArrayFormatter.cs" company="Sirenix IVS">
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
using System.Globalization;
using System.Text;

namespace OdinSerializer
{
    /// <summary>
    ///     Formatter for all arrays with more than one dimension.
    /// </summary>
    /// <typeparam name="TArray">The type of the formatted array.</typeparam>
    /// <typeparam name="TElement">The element type of the formatted array.</typeparam>
    /// <seealso cref="BaseFormatter{TArray}" />
    public sealed class MultiDimensionalArrayFormatter<TArray, TElement> : BaseFormatter<TArray> where TArray : class
    {
        private const string RANKS_NAME = "ranks";
        private const char RANKS_SEPARATOR = '|';

        private static readonly int ArrayRank;
        private static readonly Serializer<TElement> ValueReaderWriter = Serializer.Get<TElement>();

        static MultiDimensionalArrayFormatter()
        {
            if (typeof(TArray).IsArray == false)
            {
                throw new ArgumentException("Type " + typeof(TArray).Name + " is not an array.");
            }

            if (typeof(TArray).GetElementType() != typeof(TElement))
            {
                throw new ArgumentException("Array of type " + typeof(TArray).Name +
                                            " does not have the required element type of " + typeof(TElement).Name +
                                            ".");
            }

            ArrayRank = typeof(TArray).GetArrayRank();

            if (ArrayRank <= 1)
            {
                throw new ArgumentException("Array of type " + typeof(TArray).Name + " only has one rank.");
            }
        }

        /// <summary>
        ///     Returns null.
        /// </summary>
        /// <returns>
        ///     A null value.
        /// </returns>
        protected override TArray GetUninitializedObject() => null;

        /// <summary>
        ///     Provides the actual implementation for deserializing a value of type <see cref="T" />.
        /// </summary>
        /// <param name="value">
        ///     The uninitialized value to serialize into. This value will have been created earlier using
        ///     <see cref="BaseFormatter{T}.GetUninitializedObject" />.
        /// </param>
        /// <param name="reader">The reader to deserialize with.</param>
        protected override void DeserializeImplementation(ref TArray value, IDataReader reader)
        {
            string name;
            EntryType entry = reader.PeekEntry(out name);

            if (entry == EntryType.StartOfArray)
            {
                long length;
                reader.EnterArray(out length);

                entry = reader.PeekEntry(out name);

                if (entry != EntryType.String || name != RANKS_NAME)
                {
                    value = default;
                    reader.SkipEntry();
                    return;
                }

                string lengthStr;
                reader.ReadString(out lengthStr);

                var lengthsStrs = lengthStr.Split(RANKS_SEPARATOR);

                if (lengthsStrs.Length != ArrayRank)
                {
                    value = default;
                    reader.SkipEntry();
                    return;
                }

                var lengths = new int[lengthsStrs.Length];

                for (var i = 0; i < lengthsStrs.Length; i++)
                {
                    int rankVal;
                    if (int.TryParse(lengthsStrs[i], out rankVal))
                    {
                        lengths[i] = rankVal;
                    }
                    else
                    {
                        value = default;
                        reader.SkipEntry();
                        return;
                    }
                }

                long rankTotal = lengths[0];

                for (var i = 1; i < lengths.Length; i++)
                {
                    rankTotal *= lengths[i];
                }

                if (rankTotal != length)
                {
                    value = default;
                    reader.SkipEntry();
                    return;
                }

                value = (TArray)(object)Array.CreateInstance(typeof(TElement), lengths);

                // We must remember to register the array reference ourselves, since we return null in GetUninitializedObject
                RegisterReferenceID(value, reader);

                // There aren't any OnDeserializing callbacks on arrays.
                // Hence we don't invoke this.InvokeOnDeserializingCallbacks(value, reader, context);
                var elements = 0;

                try
                {
                    IterateArrayWrite(
                        (Array)(object)value,
                        () =>
                        {
                            if (reader.PeekEntry(out name) == EntryType.EndOfArray)
                            {
                                reader.Context.Config.DebugContext.LogError("Reached end of array after " + elements +
                                                                            " elements, when " + length +
                                                                            " elements were expected.");
                                throw new InvalidOperationException();
                            }

                            TElement v = ValueReaderWriter.ReadValue(reader);

                            if (reader.IsInArrayNode == false)
                            {
                                reader.Context.Config.DebugContext.LogError("Reading array went wrong. Data dump: " +
                                                                            reader.GetDataDump());
                                throw new InvalidOperationException();
                            }

                            elements++;
                            return v;
                        });
                }
                catch (InvalidOperationException)
                {
                }
                catch (Exception ex)
                {
                    reader.Context.Config.DebugContext.LogException(ex);
                }

                reader.ExitArray();
            }
            else
            {
                value = default;
                reader.SkipEntry();
            }
        }

        /// <summary>
        ///     Provides the actual implementation for serializing a value of type <see cref="T" />.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="writer">The writer to serialize with.</param>
        protected override void SerializeImplementation(ref TArray value, IDataWriter writer)
        {
            var array = value as Array;

            try
            {
                writer.BeginArrayNode(array.LongLength);

                var lengths = new int[ArrayRank];

                for (var i = 0; i < ArrayRank; i++)
                {
                    lengths[i] = array.GetLength(i);
                }

                var sb = new StringBuilder();

                for (var i = 0; i < ArrayRank; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(RANKS_SEPARATOR);
                    }

                    sb.Append(lengths[i].ToString(CultureInfo.InvariantCulture));
                }

                var lengthStr = sb.ToString();

                writer.WriteString(RANKS_NAME, lengthStr);

                IterateArrayRead(
                    (Array)(object)value,
                    v => { ValueReaderWriter.WriteValue(v, writer); });
            }
            finally
            {
                writer.EndArrayNode();
            }
        }

        private void IterateArrayWrite(Array a, Func<TElement> write)
        {
            var indices = new int[ArrayRank];
            IterateArrayWrite(a, 0, indices, write);
        }

        private void IterateArrayWrite(Array a, int rank, int[] indices, Func<TElement> write)
        {
            for (var i = 0; i < a.GetLength(rank); i++)
            {
                indices[rank] = i;

                if (rank + 1 < a.Rank)
                {
                    IterateArrayWrite(a, rank + 1, indices, write);
                }
                else
                {
                    a.SetValue(write(), indices);
                }
            }
        }

        private void IterateArrayRead(Array a, Action<TElement> read)
        {
            var indices = new int[ArrayRank];
            IterateArrayRead(a, 0, indices, read);
        }

        private void IterateArrayRead(Array a, int rank, int[] indices, Action<TElement> read)
        {
            for (var i = 0; i < a.GetLength(rank); i++)
            {
                indices[rank] = i;

                if (rank + 1 < a.Rank)
                {
                    IterateArrayRead(a, rank + 1, indices, read);
                }
                else
                {
                    read((TElement)a.GetValue(indices));
                }
            }
        }
    }

    public sealed class WeakMultiDimensionalArrayFormatter : WeakBaseFormatter
    {
        private const string RANKS_NAME = "ranks";
        private const char RANKS_SEPARATOR = '|';

        private readonly int ArrayRank;
        private readonly Type ElementType;
        private readonly Serializer ValueReaderWriter;

        public WeakMultiDimensionalArrayFormatter(Type arrayType, Type elementType) : base(arrayType)
        {
            ArrayRank = arrayType.GetArrayRank();
            ElementType = elementType;
            ValueReaderWriter = Serializer.Get(elementType);
        }

        /// <summary>
        ///     Returns null.
        /// </summary>
        /// <returns>
        ///     A null value.
        /// </returns>
        protected override object GetUninitializedObject() => null;

        /// <summary>
        ///     Provides the actual implementation for deserializing a value of type <see cref="T" />.
        /// </summary>
        /// <param name="value">
        ///     The uninitialized value to serialize into. This value will have been created earlier using
        ///     <see cref="BaseFormatter{T}.GetUninitializedObject" />.
        /// </param>
        /// <param name="reader">The reader to deserialize with.</param>
        protected override void DeserializeImplementation(ref object value, IDataReader reader)
        {
            string name;
            EntryType entry = reader.PeekEntry(out name);

            if (entry == EntryType.StartOfArray)
            {
                long length;
                reader.EnterArray(out length);

                entry = reader.PeekEntry(out name);

                if (entry != EntryType.String || name != RANKS_NAME)
                {
                    value = null;
                    reader.SkipEntry();
                    return;
                }

                string lengthStr;
                reader.ReadString(out lengthStr);

                var lengthsStrs = lengthStr.Split(RANKS_SEPARATOR);

                if (lengthsStrs.Length != ArrayRank)
                {
                    value = null;
                    reader.SkipEntry();
                    return;
                }

                var lengths = new int[lengthsStrs.Length];

                for (var i = 0; i < lengthsStrs.Length; i++)
                {
                    int rankVal;
                    if (int.TryParse(lengthsStrs[i], out rankVal))
                    {
                        lengths[i] = rankVal;
                    }
                    else
                    {
                        value = null;
                        reader.SkipEntry();
                        return;
                    }
                }

                long rankTotal = lengths[0];

                for (var i = 1; i < lengths.Length; i++)
                {
                    rankTotal *= lengths[i];
                }

                if (rankTotal != length)
                {
                    value = null;
                    reader.SkipEntry();
                    return;
                }

                value = Array.CreateInstance(ElementType, lengths);

                // We must remember to register the array reference ourselves, since we return null in GetUninitializedObject
                RegisterReferenceID(value, reader);

                // There aren't any OnDeserializing callbacks on arrays.
                // Hence we don't invoke this.InvokeOnDeserializingCallbacks(value, reader, context);
                var elements = 0;

                try
                {
                    IterateArrayWrite(
                        (Array)value,
                        () =>
                        {
                            if (reader.PeekEntry(out name) == EntryType.EndOfArray)
                            {
                                reader.Context.Config.DebugContext.LogError("Reached end of array after " + elements +
                                                                            " elements, when " + length +
                                                                            " elements were expected.");
                                throw new InvalidOperationException();
                            }

                            var v = ValueReaderWriter.ReadValueWeak(reader);

                            if (reader.IsInArrayNode == false)
                            {
                                reader.Context.Config.DebugContext.LogError("Reading array went wrong. Data dump: " +
                                                                            reader.GetDataDump());
                                throw new InvalidOperationException();
                            }

                            elements++;
                            return v;
                        });
                }
                catch (InvalidOperationException)
                {
                }
                catch (Exception ex)
                {
                    reader.Context.Config.DebugContext.LogException(ex);
                }

                reader.ExitArray();
            }
            else
            {
                value = null;
                reader.SkipEntry();
            }
        }

        /// <summary>
        ///     Provides the actual implementation for serializing a value of type <see cref="T" />.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="writer">The writer to serialize with.</param>
        protected override void SerializeImplementation(ref object value, IDataWriter writer)
        {
            var array = value as Array;

            try
            {
                writer.BeginArrayNode(array.LongLength);

                var lengths = new int[ArrayRank];

                for (var i = 0; i < ArrayRank; i++)
                {
                    lengths[i] = array.GetLength(i);
                }

                var sb = new StringBuilder();

                for (var i = 0; i < ArrayRank; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(RANKS_SEPARATOR);
                    }

                    sb.Append(lengths[i].ToString(CultureInfo.InvariantCulture));
                }

                var lengthStr = sb.ToString();

                writer.WriteString(RANKS_NAME, lengthStr);

                IterateArrayRead(
                    (Array)value,
                    v => { ValueReaderWriter.WriteValueWeak(v, writer); });
            }
            finally
            {
                writer.EndArrayNode();
            }
        }

        private void IterateArrayWrite(Array a, Func<object> write)
        {
            var indices = new int[ArrayRank];
            IterateArrayWrite(a, 0, indices, write);
        }

        private void IterateArrayWrite(Array a, int rank, int[] indices, Func<object> write)
        {
            for (var i = 0; i < a.GetLength(rank); i++)
            {
                indices[rank] = i;

                if (rank + 1 < a.Rank)
                {
                    IterateArrayWrite(a, rank + 1, indices, write);
                }
                else
                {
                    a.SetValue(write(), indices);
                }
            }
        }

        private void IterateArrayRead(Array a, Action<object> read)
        {
            var indices = new int[ArrayRank];
            IterateArrayRead(a, 0, indices, read);
        }

        private void IterateArrayRead(Array a, int rank, int[] indices, Action<object> read)
        {
            for (var i = 0; i < a.GetLength(rank); i++)
            {
                indices[rank] = i;

                if (rank + 1 < a.Rank)
                {
                    IterateArrayRead(a, rank + 1, indices, read);
                }
                else
                {
                    read(a.GetValue(indices));
                }
            }
        }
    }
}
