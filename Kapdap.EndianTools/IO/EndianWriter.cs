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
    public class EndianWriter : IDisposable
    {
        #region Properties

        private bool _leaveOpen;

        public ByteOrder ByteOrder = ByteOrder.LittleEndian;

        protected Stream _baseStream;
        public virtual Stream BaseStream
        {
            get
            {
                Flush();
                return _baseStream;
            }
        }

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

        public EndianWriter(Stream stream) : this(stream, Encoding.UTF8) { }

        public EndianWriter(Stream stream, Encoding encoding) : this(stream, encoding, false) { }

        public EndianWriter(Stream stream, Encoding encoding, bool leaveOpen) : this(stream, ByteOrder.LittleEndian, encoding, leaveOpen) { }

        public EndianWriter(Stream stream, ByteOrder order) : this(stream, order, Encoding.UTF8, false) { }

        public EndianWriter(Stream stream, ByteOrder order, Encoding encoding) : this(stream, order, encoding, false) { }

        public EndianWriter(Stream stream, ByteOrder order, Encoding encoding, bool leaveOpen)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            Encoding = encoding;
            ByteOrder = order;

            _baseStream = stream;
            _leaveOpen = leaveOpen;
        }

        #endregion

        #region Internals

#if NETSTANDARD2_0
        protected void InternalWriteBytes(byte[] buffer, ByteOrder? order, bool reverse)
        {
            if (reverse || EndianUtilities.IsByteReversalRequired(order ?? ByteOrder))
                Array.Reverse(buffer);
            _baseStream.Write(buffer, 0, buffer.Length);
        }
#else
        protected void InternalWriteBytes(Span<byte> buffer, ByteOrder? order, bool reverse)
        {
            if (reverse || EndianUtilities.IsByteReversalRequired(order ?? ByteOrder))
                buffer.Reverse();
            _baseStream.Write(buffer);
        }
#endif

        private Encoding InternalGetEncoding(Encoding encoding) =>
            encoding ?? Encoding;

        #endregion

        #region Stream

        public virtual void Flush() =>
            _baseStream.Flush();

        public virtual long Seek(int offset, SeekOrigin origin) =>
            _baseStream.Seek(offset, origin);

        #endregion

        #region Write

        public void Write(bool value) =>
            _baseStream.WriteByte((byte)(value ? 1 : 0));

        public void Write(byte value) =>
            _baseStream.WriteByte(value);

        public void Write(sbyte value) =>
            _baseStream.WriteByte((byte)value);

        public void Write(short value, bool reverse) =>
            Write(value, null, reverse);

        public void Write(short value, ByteOrder? order = null, bool reverse = false) =>
            InternalWriteBytes(BitConverter.GetBytes(value), order, reverse);

        public void Write(ushort value, bool reverse) =>
            Write(value, null, reverse);

        public void Write(ushort value, ByteOrder? order = null, bool reverse = false) =>
            InternalWriteBytes(BitConverter.GetBytes(value), order, reverse);

        public void Write(int value, bool reverse) =>
            Write(value, null, reverse);

        public void Write(int value, ByteOrder? order = null, bool reverse = false) =>
            InternalWriteBytes(BitConverter.GetBytes(value), order, reverse);

        public void Write(uint value, bool reverse) =>
            Write(value, null, reverse);

        public void Write(uint value, ByteOrder? order = null, bool reverse = false) =>
            InternalWriteBytes(BitConverter.GetBytes(value), order, reverse);

        public void Write(long value, bool reverse) =>
            Write(value, null, reverse);

        public void Write(long value, ByteOrder? order = null, bool reverse = false) =>
            InternalWriteBytes(BitConverter.GetBytes(value), order, reverse);

        public void Write(ulong value, bool reverse) =>
            Write(value, null, reverse);

        public void Write(ulong value, ByteOrder? order = null, bool reverse = false) =>
            InternalWriteBytes(BitConverter.GetBytes(value), order, reverse);

        public void Write(double value, bool reverse) =>
            Write(value, null, reverse);

        public void Write(double value, ByteOrder? order = null, bool reverse = false) =>
            InternalWriteBytes(BitConverter.GetBytes(value), order, reverse);

        public void Write(float value, bool reverse) =>
            Write(value, null, reverse);

        public void Write(float value, ByteOrder? order = null, bool reverse = false) =>
            InternalWriteBytes(BitConverter.GetBytes(value), order, reverse);

        public void Write(char ch, Encoding encoding = null) =>
            Write(new char[] { ch }, encoding);

#if NETSTANDARD2_0
        public void Write(byte[] buffer) =>
            _baseStream.Write(buffer, 0, buffer.Length);

        public void Write(char[] chars, Encoding encoding = null) =>
            Write(chars, 0, chars.Length, encoding);
#else
        public void Write(Span<byte> buffer) =>
            _baseStream.Write(buffer);

        public void Write(Span<char> chars, Encoding encoding = null)
        {
            encoding = InternalGetEncoding(encoding);
            Span<byte> bytes = new byte[encoding.GetByteCount(chars)];
            _ = encoding.GetBytes(chars, bytes);
            _baseStream.Write(bytes);
        }
#endif

        public void Write(byte[] buffer, int index, int count) =>
            _baseStream.Write(buffer, index, count);

        public void Write(char[] chars, int index, int count, Encoding encoding = null)
        {
            encoding = InternalGetEncoding(encoding);
            byte[] bytes = encoding.GetBytes(chars, index, count);
            _baseStream.Write(bytes, 0, bytes.Length);
        }

        // TODO: Handle large strings
        public void Write(string value, Encoding encoding = null)
        {
            encoding = InternalGetEncoding(encoding);
            int length = encoding.GetByteCount(value);
            Write7BitEncodedInt(length);
            Write(value, value.Length, encoding);
        }

        public void Write(string value, int length, Encoding encoding = null)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            encoding = InternalGetEncoding(encoding);

#if NETSTANDARD2_0
            char[] chars = length > value.Length ? value.ToCharArray() : value.Substring(0, length).ToCharArray();
            byte[] bytes;
            
            int numBytes = encoding.GetByteCount(chars);

            if (length > chars.Length)
            {
                char[] pad = new char[length - chars.Length];
                numBytes += encoding.GetByteCount(pad);
            }

            bytes = new byte[numBytes];

            _ = encoding.GetBytes(chars, 0, chars.Length, bytes, bytes.Length);

            _baseStream.Write(bytes, 0, bytes.Length);
#else
            ReadOnlySpan<char> chars = length > value.Length ? value.AsSpan() : value.AsSpan().Slice(0, length);
            Span<byte> bytes;

            int numBytes = encoding.GetByteCount(chars);

            if (length > chars.Length)
            {
                ReadOnlySpan<char> pad = new char[length - chars.Length];
                numBytes += encoding.GetByteCount(pad);
            }

            bytes = new byte[numBytes];

            _ = encoding.GetBytes(chars, bytes);

            _baseStream.Write(bytes);
#endif
        }

        public void WriteNullTerminatedString(string value, Encoding encoding) =>
            WriteNullTerminatedString(value, -1, encoding);

        public void WriteNullTerminatedString(string value, int maxLength = -1, Encoding encoding = null)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            encoding = InternalGetEncoding(encoding);

            if (maxLength <= 0)
                maxLength = value.Length;

            char[] pad = new char[] { '\0' };

#if NETSTANDARD2_0
            var chars = value.Length < maxLength ? value.ToCharArray() : value.Substring(0, maxLength).ToCharArray();
            var bytes = new byte[encoding.GetByteCount(chars) + encoding.GetByteCount(pad)];

            _ = encoding.GetBytes(chars, 0, chars.Length, bytes, 0);

            _baseStream.Write(bytes, 0, bytes.Length);
#else
            ReadOnlySpan<char> chars = value.Length < maxLength ? value.AsSpan() : value.AsSpan().Slice(0, maxLength) ;
            Span<byte> bytes = new byte[encoding.GetByteCount(chars) + encoding.GetByteCount(pad)];

            _ = encoding.GetBytes(chars, bytes);

            _baseStream.Write(bytes);
#endif
        }
        
        protected void Write7BitEncodedInt(int value)
        {
            // https://github.com/microsoft/referencesource/blob/master/mscorlib/system/io/binarywriter.cs

            // Write out an int 7 bits at a time.  The high bit of the byte,
            // when on, tells reader to continue reading more bytes.
            uint v = (uint)value;   // support negative numbers
            while (v >= 0x80)
            {
                Write((byte)(v | 0x80));
                v >>= 7;
            }
            Write((byte)v);
        }

        #endregion

        #region Close

        public void Close() =>
            _baseStream.Close();

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_leaveOpen)
                    _baseStream.Flush();
                else
                    _baseStream.Close();
            }
        }

        public void Dispose() =>
            Dispose(true);

        #endregion
    }
}