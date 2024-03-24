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
using System.IO;
using System.Text;

namespace Kapdap.EndianTools.IO
{
    public class EndianReader : IDisposable
    {
        #region Properties

        private bool _leaveOpen;

        public ByteOrder ByteOrder = ByteOrder.LittleEndian;

        public Stream BaseStream { get; }

        public long Position { get => BaseStream.Position; set => BaseStream.Position = value; }
        public long Length { get => BaseStream.Length; }

        public Encoding _encoding = Encoding.UTF8;
        public Encoding Encoding
        {
            get => _encoding;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                _encoding = value;
            }
        }

        #endregion

        #region Constructors

        public EndianReader(Stream stream) : this(stream, Encoding.UTF8) { }

        public EndianReader(Stream stream, Encoding encoding) : this(stream, encoding, false) { }

        public EndianReader(Stream stream, Encoding encoding, bool leaveOpen) : this(stream, ByteOrder.LittleEndian, encoding, leaveOpen) { }

        public EndianReader(Stream stream, ByteOrder order) : this(stream, order, Encoding.UTF8, false) { }

        public EndianReader(Stream stream, ByteOrder order, Encoding encoding) : this(stream, order, encoding, false) { }

        public EndianReader(Stream stream, ByteOrder order, Encoding encoding, bool leaveOpen)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            Encoding = encoding;
            ByteOrder = order;
            BaseStream = stream;

            _leaveOpen = leaveOpen;
        }

        #endregion

        #region Internals

        private char InternalReadChar(Encoding encoding)
        {
            encoding = InternalGetEncoding(encoding);

            int charSize = InternalGetEncodingSize(encoding);
            var charBytes = new byte[charSize];

            if (charSize == 2)
                charBytes[1] = (byte)BaseStream.ReadByte();

            if (Position < Length)
                charBytes[0] = (byte)BaseStream.ReadByte();

            return encoding.GetChars(charBytes, 0, charBytes.Length)[0];
        }

        private void InternalReadChars(char[] chars, int offset, int count, Encoding encoding)
        {
            if (chars.Length <= 0)
                return;

            if (chars == null)
                throw new ArgumentNullException(nameof(chars));

            if (offset > chars.Length || offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            encoding = InternalGetEncoding(encoding);

            int charSize = InternalGetEncodingSize(encoding);
            var charBytes = new byte[charSize];

            int maxLength = Math.Min(chars.Length, count);

            int i = offset;
            while (Position < Length)
            {
                if (charSize == 2)
                    charBytes[1] = (byte)BaseStream.ReadByte();

                if (Position < Length)
                    charBytes[0] = (byte)BaseStream.ReadByte();

                chars[i++] = encoding.GetChars(charBytes, 0, charBytes.Length)[0];

                if (i >= maxLength)
                    return;
            }
        }

        private string InternalReadString(int length, Encoding encoding)
        {
            int i = 0;

            encoding = InternalGetEncoding(encoding);

            int charSize = InternalGetEncodingSize(encoding);
            int numBytes = length < 0 ? Read7BitEncodedInt() : length *= charSize;

#if NETSTANDARD2_0
            var bytes = new byte[numBytes];

            while (Position < Length)
            {
                bytes[i] = (byte)BaseStream.ReadByte();
                    
                if (charSize == 2 && Position < Length)
                    bytes[i + 1] = (byte)BaseStream.ReadByte();

                if ((i += charSize) >= bytes.Length)
                    break;
            }

            return encoding.GetString(bytes, 0, bytes.Length);
#else
            Span<byte> bytes = new byte[numBytes];

            while (Position < Length)
            {
                bytes[i] = (byte)BaseStream.ReadByte();

                if (charSize == 2 && Position < Length)
                    bytes[i + 1] = (byte)BaseStream.ReadByte();

                if ((i += charSize) >= bytes.Length)
                    break;
            }

            return encoding.GetString(bytes);
#endif
        }

        private string InternalReadString(Encoding encoding, int maxLength = -1)
        {
            char ch;

            encoding = InternalGetEncoding(encoding);

            int charSize = InternalGetEncodingSize(encoding);
            var charBytes = new byte[charSize];

            var builder = new StringBuilder();

            while (Position < Length)
            {
                charBytes[0] = (byte)BaseStream.ReadByte();

                if (charSize == 2 && Position < Length)
                    charBytes[1] = (byte)BaseStream.ReadByte();

                ch = encoding.GetChars(charBytes, 0, charBytes.Length)[0];

                if (ch == '\0')
                    break;

                builder.Append(ch);

                if (--maxLength == 0)
                    break;
            }

            return builder.ToString();
        }

#if NETSTANDARD2_0
        private T InternalReadValue<T>(int count, ByteOrder? order, bool reverse, Func<byte[], int, T> converter)
            where T : struct =>
            converter(InternalReadBytes(count, order, reverse), 0);

        private byte[] InternalReadBytes(int count, ByteOrder? order, bool reverse)
        {
            var buffer = new byte[count];

            BaseStream.Read(buffer, 0, buffer.Length);

            if (reverse || EndianUtilities.IsByteReversalRequired(order ?? ByteOrder))
                Array.Reverse(buffer);

            return buffer;
        }
#else
        private void InternalReadChars(Span<char> chars, Encoding encoding)
        {
            if (chars.Length <= 0)
                throw new ArgumentOutOfRangeException(nameof(chars));

            encoding = InternalGetEncoding(encoding);

            int charSize = InternalGetEncodingSize(encoding);
            var charBytes = new byte[charSize];

            int i = 0;
            while (Position < Length)
            {
                if (charSize == 2)
                    charBytes[1] = (byte)BaseStream.ReadByte();

                if (Position < Length)
                    charBytes[0] = (byte)BaseStream.ReadByte();

                chars[i++] = encoding.GetChars(charBytes, 0, charSize)[0];

                if (i >= chars.Length)
                    return;
            }
        }

        private T InternalReadValue<T>(int count, ByteOrder? order, bool reverse, Func<byte[], int, T> converter)
            where T : struct
        {
            ReadOnlySpan<byte> buffer = InternalReadBytes(count, order, reverse);
            return converter(buffer.ToArray(), 0);
        }

        private Span<byte> InternalReadBytes(int count, ByteOrder? order, bool reverse)
        {
            Span<byte> buffer = new byte[count];

            BaseStream.Read(buffer);

            if (reverse || EndianUtilities.IsByteReversalRequired(order ?? ByteOrder))
                buffer.Reverse();

            return buffer;
        }
#endif

        private Encoding InternalGetEncoding(Encoding encoding) =>
            encoding ?? Encoding;

        private int InternalGetEncodingSize(Encoding encoding) =>
            (encoding is UnicodeEncoding) ? 2 : 1;

        #endregion

        #region Stream

        public virtual void Flush() =>
            BaseStream.Flush();

        public virtual long Seek(int offset, SeekOrigin origin) =>
            BaseStream.Seek(offset, origin);

        #endregion

        #region Read

        public byte[] ReadBytes(int count)
        {
            var buffer = new byte[count];
            BaseStream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public char[] ReadChars(int count, Encoding encoding = null)
        {
            var chars = new char[count];
            InternalReadChars(chars, 0, chars.Length, encoding);
            return chars;
        }

        public bool ReadBoolean() =>
            BaseStream.ReadByte() != 0;

        public char ReadChar(Encoding encoding = null) =>
            InternalReadChar(encoding);

        public byte ReadByte() =>
            (byte)BaseStream.ReadByte();

        public sbyte ReadSByte() =>
            (sbyte)BaseStream.ReadByte();

        public short ReadInt16(bool reverse) =>
            ReadInt16(null, reverse);

        public short ReadInt16(ByteOrder? order = null, bool reverse = false) =>
            InternalReadValue(2, order, reverse, BitConverter.ToInt16);

        public ushort ReadUInt16(bool reverse) =>
            ReadUInt16(null, reverse);

        public ushort ReadUInt16(ByteOrder? order = null, bool reverse = false) =>
            InternalReadValue(2, order, reverse, BitConverter.ToUInt16);

        public int ReadInt32(bool reverse) =>
            ReadInt32(null, reverse);

        public int ReadInt32(ByteOrder? order = null, bool reverse = false) =>
            InternalReadValue(4, order, reverse, BitConverter.ToInt32);

        public uint ReadUInt32(bool reverse) =>
            ReadUInt32(null, reverse);

        public uint ReadUInt32(ByteOrder? order = null, bool reverse = false) =>
            InternalReadValue(4, order, reverse, BitConverter.ToUInt32);

        public long ReadInt64(bool reverse) =>
            ReadInt64(null, reverse);

        public long ReadInt64(ByteOrder? order = null, bool reverse = false) =>
            InternalReadValue(8, order, reverse, BitConverter.ToInt64);

        public ulong ReadUInt64(bool reverse) =>
            ReadUInt64(null, reverse);

        public ulong ReadUInt64(ByteOrder? order = null, bool reverse = false) =>
            InternalReadValue(8, order, reverse, BitConverter.ToUInt64);

        public float ReadFloat(bool reverse) =>
            ReadFloat(null, reverse);

        public float ReadFloat(ByteOrder? order = null, bool reverse = false) =>
            InternalReadValue(4, order, reverse, BitConverter.ToSingle);

        public double ReadDouble(bool reverse) =>
            ReadDouble(null, reverse);

        public double ReadDouble(ByteOrder? order = null, bool reverse = false) =>
            InternalReadValue(8, order, reverse, BitConverter.ToDouble);

        public string ReadString(Encoding encoding = null) =>
            InternalReadString(-1, encoding);

        public string ReadFixedLengthString(int length, Encoding encoding) =>
            ReadFixedLengthString(length, false, encoding);

        public string ReadFixedLengthString(int length, bool trim = false, Encoding encoding = null)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            string value = InternalReadString(length, encoding);

            return trim ? value.TrimEnd('\0') : value;
        }

        public string ReadNullTerminatedString(Encoding encoding) =>
            ReadNullTerminatedString(-1, encoding);

        public string ReadNullTerminatedString(int maxLength = -1, Encoding encoding = null) =>
            InternalReadString(encoding, maxLength);

        public void Read(char[] chars, Encoding encoding = null) =>
            InternalReadChars(chars, 0, chars.Length, encoding);

        public void Read(char[] chars, int offset, int count, Encoding encoding = null) =>
            InternalReadChars(chars, offset, count, encoding);

        public void Read(byte[] buffer) =>
            BaseStream.Read(buffer, 0, buffer.Length);

        public void Read(byte[] buffer, int offset, int count) =>
            BaseStream.Read(buffer, offset, count);
        
#if NETSTANDARD2_0
#else
        public void Read(Span<byte> buffer) =>
            BaseStream.Read(buffer);

        public void Read(Span<char> chars, Encoding encoding = null) =>
            InternalReadChars(chars, encoding);
#endif

        protected int Read7BitEncodedInt()
        {
            // https://github.com/microsoft/referencesource/blob/master/mscorlib/system/io/binaryreader.cs

            // Read out an Int32 7 bits at a time.  The high bit
            // of the byte when on means to continue reading more bytes.
            int count = 0;
            int shift = 0;
            byte b;

            do
            {
                // Check for a corrupted stream.  Read a max of 5 bytes.
                // In a future version, add a DataFormatException.
                if (shift == 5 * 7)  // 5 bytes max per Int32, shift += 7
                    throw new FormatException("Bad Int32 format");

                // ReadByte handles end of stream cases for us.
                b = (byte)BaseStream.ReadByte();
                count |= (b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);

            return count;
        }

        #endregion

        #region Peek

        public bool PeekBoolean()
        {
            long position = Position;
            bool value = ReadBoolean();
            Position = position;

            return value;
        }

        public byte PeekByte()
        {
            long position = Position;
            byte value = ReadByte();
            Position = position;

            return value;
        }

        public sbyte PeekSByte()
        {
            long position = Position;
            sbyte value = ReadSByte();
            Position = position;

            return value;
        }

        public byte[] PeekBytes(int count)
        {
            long position = Position;
            byte[] value = ReadBytes(count);
            Position = position;

            return value;
        }

        public char PeekChar()
        {
            long position = Position;
            char value = PeekChar();
            Position = position;

            return value;
        }

        public char[] PeekChars(int count)
        {
            long position = Position;
            char[] value = ReadChars(count);
            Position = position;

            return value;
        }

        public short PeekInt16(bool reverse = false) =>
            PeekInt16(null, reverse);

        public short PeekInt16(ByteOrder? order = null, bool reverse = false)
        {
            long position = Position;
            short value = ReadInt16(order, reverse);
            Position = position;

            return value;
        }

        public ushort PeekUInt16(bool reverse = false) =>
            PeekUInt16(null, reverse);

        public ushort PeekUInt16(ByteOrder? order = null, bool reverse = false)
        {
            long position = Position;
            ushort value = ReadUInt16(order, reverse);
            Position = position;

            return value;
        }

        public int PeekInt32(bool reverse = false) =>
            PeekInt32(null, reverse);

        public int PeekInt32(ByteOrder? order = null, bool reverse = false)
        {
            long position = Position;
            int value = ReadInt32(order, reverse);
            Position = position;

            return value;
        }

        public uint PeekUInt32(bool reverse = false) =>
            PeekUInt32(null, reverse);

        public uint PeekUInt32(ByteOrder? order = null, bool reverse = false)
        {
            long position = Position;
            uint value = ReadUInt32(order, reverse);
            Position = position;

            return value;
        }

        public long PeekInt64(bool reverse = false) =>
            PeekInt64(null, reverse);

        public long PeekInt64(ByteOrder? order = null, bool reverse = false)
        {
            long position = Position;
            long value = ReadInt64(order, reverse);
            Position = position;

            return value;
        }

        public ulong PeekUInt64(bool reverse = false) =>
            PeekUInt64(null, reverse);

        public ulong PeekUInt64(ByteOrder? order = null, bool reverse = false)
        {
            long position = Position;
            ulong value = ReadUInt64(order, reverse);
            Position = position;

            return value;
        }

        public float PeekFloat(bool reverse = false) =>
            PeekFloat(null, reverse);

        public float PeekFloat(ByteOrder? order = null, bool reverse = false)
        {
            long position = Position;
            float value = ReadFloat(order, reverse);
            Position = position;

            return value;
        }

        public double PeekDouble(bool reverse = false) =>
            PeekDouble(null, reverse);

        public double PeekDouble(ByteOrder? order = null, bool reverse = false)
        {
            long position = Position;
            double value = ReadDouble(order, reverse);
            Position = position;

            return value;
        }

        public string PeekString(Encoding encoding = null)
        {
            long position = Position;
            string value = ReadString(encoding);
            Position = position;

            return value;
        }

        public string PeekFixedLengthString(int length, Encoding encoding) =>
            PeekFixedLengthString(length, false, encoding);

        public string PeekFixedLengthString(int length, bool trim = false, Encoding encoding = null)
        {
            long position = Position;
            string value = ReadFixedLengthString(length, trim, encoding);
            Position = position;

            return value;
        }

        public string PeekNullTerminatedString(Encoding encoding) =>
            PeekNullTerminatedString(-1, encoding);

        public string PeekNullTerminatedString(int maxLength, Encoding encoding = null)
        {
            long position = Position;
            string value = ReadNullTerminatedString(maxLength, encoding);
            Position = position;

            return value;
        }

        #endregion

        #region Close

        public void Close() =>
            BaseStream.Close();

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_leaveOpen)
                    BaseStream.Flush();
                else
                    BaseStream.Close();
            }
        }

        public void Dispose() =>
            Dispose(true);

        #endregion
    }
}