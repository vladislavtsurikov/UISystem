//-----------------------------------------------------------------------
// <copyright file="UnsafeUtilities.cs" company="Sirenix IVS">
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
using System.Runtime.InteropServices;

namespace OdinSerializer.Utilities.Unsafe
{
    /// <summary>
    ///     Contains utilities for performing common unsafe operations.
    /// </summary>
    public static class UnsafeUtilities
    {
        /// <summary>
        ///     Blindly creates an array of structs from an array of bytes via direct memory copy/blit.
        /// </summary>
        public static T[] StructArrayFromBytes<T>(byte[] bytes, int byteLength) where T : struct =>
            StructArrayFromBytes<T>(bytes, 0, 0);

        /// <summary>
        ///     Blindly creates an array of structs from an array of bytes via direct memory copy/blit.
        /// </summary>
        public static T[] StructArrayFromBytes<T>(byte[] bytes, int byteLength, int byteOffset) where T : struct
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            if (byteLength <= 0)
            {
                throw new ArgumentException("Byte length must be larger than zero.");
            }

            if (byteOffset < 0)
            {
                throw new ArgumentException("Byte offset must be larger than or equal to zero.");
            }

            var typeSize = Marshal.SizeOf(typeof(T));

            if (byteOffset % sizeof(ulong) != 0)
            {
                throw new ArgumentException(
                    "Byte offset must be divisible by " + sizeof(ulong) + " (IE, sizeof(ulong))");
            }

            if (byteLength + byteOffset >= bytes.Length)
            {
                throw new ArgumentException("Given byte array of size " + bytes.Length +
                                            " is not large enough to copy requested number of bytes " + byteLength +
                                            ".");
            }

            if ((byteLength - byteOffset) % typeSize != 0)
            {
                throw new ArgumentException("The length in the given byte array (" + bytes.Length + ", and " +
                                            (bytes.Length - byteOffset) + " minus byteOffset " + byteOffset +
                                            ") to convert to type " + typeof(T).Name +
                                            " is not divisible by the size of " + typeof(T).Name + " (" + typeSize +
                                            ").");
            }

            var elementCount = (bytes.Length - byteOffset) / typeSize;
            var array = new T[elementCount];
            MemoryCopy(bytes, array, byteLength, byteOffset, 0);
            return array;
        }

        /// <summary>
        ///     Blindly copies an array of structs into an array of bytes via direct memory copy/blit.
        /// </summary>
        public static byte[] StructArrayToBytes<T>(T[] array) where T : struct
        {
            byte[] bytes = null;
            return StructArrayToBytes(array, ref bytes, 0);
        }

        /// <summary>
        ///     Blindly copies an array of structs into an array of bytes via direct memory copy/blit.
        /// </summary>
        public static byte[] StructArrayToBytes<T>(T[] array, ref byte[] bytes, int byteOffset) where T : struct
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (byteOffset < 0)
            {
                throw new ArgumentException("Byte offset must be larger than or equal to zero.");
            }

            var typeSize = Marshal.SizeOf(typeof(T));
            var byteCount = typeSize * array.Length;

            if (bytes == null)
            {
                bytes = new byte[byteCount + byteOffset];
            }
            else if (bytes.Length + byteOffset > byteCount)
            {
                throw new ArgumentException("Byte array must be at least " + (bytes.Length + byteOffset) +
                                            " long with the given byteOffset.");
            }

            MemoryCopy(array, bytes, byteCount, 0, byteOffset);
            return bytes;
        }

        /// <summary>
        ///     Creates a new string from the contents of a given byte buffer.
        /// </summary>
        public static unsafe string StringFromBytes(byte[] buffer, int charLength, bool needs16BitSupport)
        {
            var byteCount = needs16BitSupport ? charLength * 2 : charLength;

            if (buffer.Length < byteCount)
            {
                throw new ArgumentException(
                    "Buffer is not large enough to contain the given string; a size of at least " + byteCount +
                    " is required.");
            }

            var toHandle = default(GCHandle);
            var result = new string(' ', charLength); // Creaty empty string of required length

            try
            {
                toHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

                if (needs16BitSupport)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        fixed (char* charPtr1 = result)
                        {
                            var fromPtr1 = (ushort*)toHandle.AddrOfPinnedObject().ToPointer();
                            var toPtr1 = (ushort*)charPtr1;

                            for (var i = 0; i < byteCount; i += sizeof(ushort))
                            {
                                *toPtr1++ = *fromPtr1++;
                            }
                        }
                    }
                    else
                    {
                        fixed (char* charPtr2 = result)
                        {
                            var fromPtr2 = (byte*)toHandle.AddrOfPinnedObject().ToPointer();
                            var toPtr2 = (byte*)charPtr2;

                            for (var i = 0; i < byteCount; i += sizeof(ushort))
                            {
                                *toPtr2 = *(fromPtr2 + 1);
                                *(toPtr2 + 1) = *fromPtr2;

                                fromPtr2 += 2;
                                toPtr2 += 2;
                            }
                        }
                    }
                }
                else
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        fixed (char* charPtr3 = result)
                        {
                            var fromPtr3 = (byte*)toHandle.AddrOfPinnedObject().ToPointer();
                            var toPtr3 = (byte*)charPtr3;

                            for (var i = 0; i < byteCount; i += sizeof(byte))
                            {
                                *toPtr3++ = *fromPtr3++;
                                toPtr3++; // Skip every other string byte
                            }
                        }
                    }
                    else
                    {
                        fixed (char* charPtr4 = result)
                        {
                            var fromPtr4 = (byte*)toHandle.AddrOfPinnedObject().ToPointer();
                            var toPtr4 = (byte*)charPtr4;

                            for (var i = 0; i < byteCount; i += sizeof(byte))
                            {
                                toPtr4++; // Skip every other string byte
                                *toPtr4++ = *fromPtr4++;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (toHandle.IsAllocated)
                {
                    toHandle.Free();
                }
            }

            // Retrieve proper string reference from the intern pool.
            // This code removed for now, as the slight decrease in memory use is not considered worth the performance cost of the intern lookup and the potential extra garbage to be collected.
            // Might eventually become a global config option, if this is considered necessary.
            //result = string.Intern(result);

            return result;
        }

        /// <summary>
        ///     Writes the contents of a string into a given byte buffer.
        /// </summary>
        public static unsafe int StringToBytes(byte[] buffer, string value, bool needs16BitSupport)
        {
            var byteCount = needs16BitSupport ? value.Length * 2 : value.Length;

            if (buffer.Length < byteCount)
            {
                throw new ArgumentException(
                    "Buffer is not large enough to contain the given string; a size of at least " + byteCount +
                    " is required.");
            }

            var toHandle = default(GCHandle);

            try
            {
                toHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

                if (needs16BitSupport)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        fixed (char* charPtr1 = value)
                        {
                            var fromPtr1 = (ushort*)charPtr1;
                            var toPtr1 = (ushort*)toHandle.AddrOfPinnedObject().ToPointer();

                            for (var i = 0; i < byteCount; i += sizeof(ushort))
                            {
                                *toPtr1++ = *fromPtr1++;
                            }
                        }
                    }
                    else
                    {
                        fixed (char* charPtr2 = value)
                        {
                            var fromPtr2 = (byte*)charPtr2;
                            var toPtr2 = (byte*)toHandle.AddrOfPinnedObject().ToPointer();

                            for (var i = 0; i < byteCount; i += sizeof(ushort))
                            {
                                *toPtr2 = *(fromPtr2 + 1);
                                *(toPtr2 + 1) = *fromPtr2;

                                fromPtr2 += 2;
                                toPtr2 += 2;
                            }
                        }
                    }
                }
                else
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        fixed (char* charPtr3 = value)
                        {
                            var fromPtr3 = (byte*)charPtr3;
                            var toPtr3 = (byte*)toHandle.AddrOfPinnedObject().ToPointer();

                            for (var i = 0; i < byteCount; i += sizeof(byte))
                            {
                                fromPtr3++; // Skip every other string byte
                                *toPtr3++ = *fromPtr3++;
                            }
                        }
                    }
                    else
                    {
                        fixed (char* charPtr4 = value)
                        {
                            var fromPtr4 = (byte*)charPtr4;
                            var toPtr4 = (byte*)toHandle.AddrOfPinnedObject().ToPointer();

                            for (var i = 0; i < byteCount; i += sizeof(byte))
                            {
                                *toPtr4++ = *fromPtr4++;
                                fromPtr4++; // Skip every other string byte
                            }
                        }
                    }
                }
            }
            finally
            {
                if (toHandle.IsAllocated)
                {
                    toHandle.Free();
                }
            }

            return byteCount;
        }

        public static unsafe void MemoryCopy(void* from, void* to, int bytes)
        {
            var end = (byte*)to + bytes;

            var fromBigPtr = (Struct256Bit*)from;
            var toBigPtr = (Struct256Bit*)to;

            while (toBigPtr + 1 <= end)
            {
                *toBigPtr++ = *fromBigPtr++;
            }

            var fromSmallPtr = (byte*)fromBigPtr;
            var toSmallPtr = (byte*)toBigPtr;

            while (toSmallPtr < end)
            {
                *toSmallPtr++ = *fromSmallPtr++;
            }
        }

        /// <summary>
        ///     Blindly mem-copies a given number of bytes from the memory location of one object to another. WARNING: This method
        ///     is ridiculously dangerous. Only use if you know what you're doing.
        /// </summary>
        public static unsafe void MemoryCopy(object from, object to, int byteCount, int fromByteOffset,
            int toByteOffset)
        {
            var fromHandle = default(GCHandle);
            var toHandle = default(GCHandle);

            if (fromByteOffset % sizeof(ulong) != 0 || toByteOffset % sizeof(ulong) != 0)
            {
                throw new ArgumentException(
                    "Byte offset must be divisible by " + sizeof(ulong) + " (IE, sizeof(ulong))");
            }

            try
            {
                var restBytes = byteCount % sizeof(ulong);
                var ulongCount = (byteCount - restBytes) / sizeof(ulong);
                var fromOffsetCount = fromByteOffset / sizeof(ulong);
                var toOffsetCount = toByteOffset / sizeof(ulong);

                fromHandle = GCHandle.Alloc(from, GCHandleType.Pinned);
                toHandle = GCHandle.Alloc(to, GCHandleType.Pinned);

                var fromUlongPtr = (ulong*)fromHandle.AddrOfPinnedObject().ToPointer();
                var toUlongPtr = (ulong*)toHandle.AddrOfPinnedObject().ToPointer();

                if (fromOffsetCount > 0)
                {
                    fromUlongPtr += fromOffsetCount;
                }

                if (toOffsetCount > 0)
                {
                    toUlongPtr += toOffsetCount;
                }

                for (var i = 0; i < ulongCount; i++)
                {
                    *toUlongPtr++ = *fromUlongPtr++;
                }

                if (restBytes > 0)
                {
                    var fromBytePtr = (byte*)fromUlongPtr;
                    var toBytePtr = (byte*)toUlongPtr;

                    for (var i = 0; i < restBytes; i++)
                    {
                        *toBytePtr++ = *fromBytePtr++;
                    }
                }
            }
            finally
            {
                if (fromHandle.IsAllocated)
                {
                    fromHandle.Free();
                }

                if (toHandle.IsAllocated)
                {
                    toHandle.Free();
                }
            }
        }

        private struct Struct256Bit
        {
            public decimal d1;
            public decimal d2;
        }
    }
}
