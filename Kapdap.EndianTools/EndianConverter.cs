// SPDX-FileCopyrightText: 2024 Kapdap <kapdap@pm.me>
//
// SPDX-License-Identifier: MIT
/*  Kapdap.EndianTools
 *  
 *  Copyright (c) 2024 Kapdap <kapdap@pm.me>
 *  Copyright (c) Microsoft Corporation
 *
 *  Use of this source code is governed by an MIT-style
 *  license that can be found in the LICENSE file or at
 *  https://opensource.org/licenses/MIT.
 */

using System;
using System.Text;

namespace Kapdap.EndianTools
{
    public static class EndianConverter
    {
        #region Bytes

        public static byte[] GetBytes(bool value) =>
            BitConverter.GetBytes(value);

        public static byte[] GetBytes(char value) =>
            BitConverter.GetBytes(value);

        public static byte[] GetBytes(short value, bool reverse = false)
        {
            var bytes = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(short value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            if (EndianUtilities.IsByteReversalRequired(byteOrder))
                Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(ushort value, bool reverse = false)
        {
            var bytes = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(ushort value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            if (EndianUtilities.IsByteReversalRequired(byteOrder))
                Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(int value, bool reverse = false)
        {
            var bytes = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(int value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            if (EndianUtilities.IsByteReversalRequired(byteOrder))
                Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(uint value, bool reverse = false)
        {
            var bytes = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(uint value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            if (EndianUtilities.IsByteReversalRequired(byteOrder))
                Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(long value, bool reverse = false)
        {
            var bytes = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(long value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            if (EndianUtilities.IsByteReversalRequired(byteOrder))
                Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(ulong value, bool reverse = false)
        {
            var bytes = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(ulong value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            if (EndianUtilities.IsByteReversalRequired(byteOrder))
                Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(double value, bool reverse = false)
        {
            var bytes = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(double value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            if (EndianUtilities.IsByteReversalRequired(byteOrder))
                Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(float value, bool reverse = false)
        {
            var bytes = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(float value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            if (EndianUtilities.IsByteReversalRequired(byteOrder))
                Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(string value, int length = -1, Encoding encoding = null)
        {
            var bytes = (encoding ?? Encoding.UTF8).GetBytes(value);
            if (length > 0) Array.Resize(ref bytes, length);
            return bytes;
        }

        #endregion

        #region Reverse

        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Buffers/Binary/BinaryPrimitives.ReverseEndianness.cs

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="sbyte" /> value, which effectively does nothing for an <see cref="sbyte" />.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The passed-in value, unmodified.</returns>
        /// <remarks>This method effectively does nothing and was added only for consistency.</remarks>
        public static sbyte ReverseEndianness(sbyte value) =>
            value;

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="short" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        public static short ReverseEndianness(short value) =>
            (short)ReverseEndianness((ushort)value);

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="int" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        public static int ReverseEndianness(int value) =>
            (int)ReverseEndianness((uint)value);

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="long" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        public static long ReverseEndianness(long value) =>
            (long)ReverseEndianness((ulong)value);

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="byte" /> value, which effectively does nothing for an <see cref="byte" />.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The passed-in value, unmodified.</returns>
        /// <remarks>This method effectively does nothing and was added only for consistency.</remarks>
        public static byte ReverseEndianness(byte value) =>
            value;

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="ushort" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        public static ushort ReverseEndianness(ushort value) =>
            (ushort)((value >> 8) + (value << 8));

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="char" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        internal static char ReverseEndianness(char value) =>
            (char)ReverseEndianness((ushort)value);

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="uint" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        public static uint ReverseEndianness(uint value) =>
            RotateRight(value & 0x00FF00FFu, 8) +
            RotateLeft(value & 0xFF00FF00u, 8);

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="ulong" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        public static ulong ReverseEndianness(ulong value) =>
            ((ulong)ReverseEndianness((uint)value) << 32) +
                    ReverseEndianness((uint)(value >> 32));

        #endregion

        #region Rotate

        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Numerics/BitOperations.cs

        /// <summary>
        /// Rotates the specified value left by the specified number of bits.
        /// Similar in behavior to the x86 instruction ROL.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.
        /// Any value outside the range [0..31] is treated as congruent mod 32.</param>
        /// <returns>The rotated value.</returns>
        public static uint RotateLeft(uint value, int offset)
            => (value << offset) | (value >> (32 - offset));

        /// <summary>
        /// Rotates the specified value left by the specified number of bits.
        /// Similar in behavior to the x86 instruction ROL.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.
        /// Any value outside the range [0..63] is treated as congruent mod 64.</param>
        /// <returns>The rotated value.</returns>
        public static ulong RotateLeft(ulong value, int offset)
            => (value << offset) | (value >> (64 - offset));

        /// <summary>
        /// Rotates the specified value right by the specified number of bits.
        /// Similar in behavior to the x86 instruction ROR.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.
        /// Any value outside the range [0..31] is treated as congruent mod 32.</param>
        /// <returns>The rotated value.</returns>
        public static uint RotateRight(uint value, int offset)
            => (value >> offset) | (value << (32 - offset));

        /// <summary>
        /// Rotates the specified value right by the specified number of bits.
        /// Similar in behavior to the x86 instruction ROR.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.
        /// Any value outside the range [0..63] is treated as congruent mod 64.</param>
        /// <returns>The rotated value.</returns>
        public static ulong RotateRight(ulong value, int offset)
            => (value >> offset) | (value << (64 - offset));

        #endregion
    }
}