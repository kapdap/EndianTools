// SPDX-FileCopyrightText: 2024 Kapdap <kapdap@pm.me>
//
// SPDX-License-Identifier: MIT
/*  Kapdap.EndianTools.Tests
 *  
 *  Copyright 2024 Kapdap <kapdap@pm.me>
 *
 *  Use of this source code is governed by an MIT-style
 *  license that can be found in the LICENSE file or at
 *  https://opensource.org/licenses/MIT.
 */

using Kapdap.EndianTools.IO;
using System.Text;

namespace Kapdap.EndianTools.Tests
{
    [TestClass]
    public class EndianWriterTests
    {
        [TestMethod]
        public void Write_ValuesShouldWriteToBigEndianData()
        {
            var stream = new MemoryStream();
            var writer = new EndianWriter(stream, ByteOrder.BigEndian);

            writer.Write((short)EndianTestData.TestValues[0]);
            writer.Write((int)EndianTestData.TestValues[1]);
            writer.Write((long)EndianTestData.TestValues[2]);
            writer.Write((ushort)EndianTestData.TestValues[3]);
            writer.Write((uint)EndianTestData.TestValues[4]);
            writer.Write((ulong)EndianTestData.TestValues[5]);
            writer.Write((double)EndianTestData.TestValues[6]);
            writer.Write((float)EndianTestData.TestValues[7]);
            writer.Write((string)EndianTestData.TestValues[8], 12);
            writer.Write((bool)EndianTestData.TestValues[9]);
            writer.Write((bool)EndianTestData.TestValues[10]);

            writer.Position = 0;

            Span<byte> data = EndianTestData.BigEndianData;
            Span<byte> buffer = new byte[stream.Length];

            stream.Read(buffer);

            Assert.AreEqual(true, data.SequenceEqual(buffer));
        }

        [TestMethod]
        public void Write_ValuesShouldWriteToLittleEndianData()
        {
            var stream = new MemoryStream();
            var writer = new EndianWriter(stream, ByteOrder.LittleEndian);

            writer.Write((short)EndianTestData.TestValues[0]);
            writer.Write((int)EndianTestData.TestValues[1]);
            writer.Write((long)EndianTestData.TestValues[2]);
            writer.Write((ushort)EndianTestData.TestValues[3]);
            writer.Write((uint)EndianTestData.TestValues[4]);
            writer.Write((ulong)EndianTestData.TestValues[5]);
            writer.Write((double)EndianTestData.TestValues[6]);
            writer.Write((float)EndianTestData.TestValues[7]);
            writer.Write((string)EndianTestData.TestValues[8], 12);
            writer.Write((bool)EndianTestData.TestValues[9]);
            writer.Write((bool)EndianTestData.TestValues[10]);

            writer.Position = 0;

            Span<byte> data = EndianTestData.LittleEndianData;
            Span<byte> buffer = new byte[stream.Length];

            stream.Read(buffer);

            Assert.AreEqual(true, data.SequenceEqual(buffer));
        }

        [TestMethod]
        public void Write_StringTests()
        {
            var stream = new MemoryStream();
            var writer = new EndianWriter(stream);

            var value = (string)EndianTestData.TestValues[8];

            writer.Write(value);

            writer.Write(value, value.Length);
            writer.Write(value, value.Length + 1);
            writer.Write(value, value.Length + 2);
            writer.Write(value, value.Length + 2);
            writer.Write(value, value.Length / 2);

            writer.WriteNullTerminatedString(value);
            writer.WriteNullTerminatedString(value, value.Length / 2);

            writer.Write(value, Encoding.Unicode);

            writer.Write(value, value.Length + 1, Encoding.Unicode);
            writer.Write(value, value.Length + 2, Encoding.Unicode);
            writer.Write(value, value.Length / 2, Encoding.Unicode);

            writer.WriteNullTerminatedString(value, Encoding.Unicode);
            writer.WriteNullTerminatedString(value, Encoding.Unicode);
            writer.WriteNullTerminatedString(value, value.Length / 2, Encoding.Unicode);

            writer.Position = 0;

            var reader = new EndianReader(stream);

            Assert.AreEqual(value,                                reader.ReadString(),                                  "Prefixed length string didn't match");

            Assert.AreEqual(value,                                reader.ReadFixedLengthString(value.Length),           "Implied fixed length string didn't match");
            Assert.AreEqual(value + '\0',                         reader.ReadFixedLengthString(value.Length + 1),       "Fixed length string with padding didn't match");
            Assert.AreEqual(value + '\0' + '\0',                  reader.ReadFixedLengthString(value.Length + 2),       "Fixed length string with double padding didn't match");
            Assert.AreEqual(value,                                reader.ReadFixedLengthString(value.Length + 2, true), "Fixed length string with trimmed padding didn't match");
            Assert.AreEqual(value.Substring(0, value.Length / 2), reader.ReadFixedLengthString(value.Length / 2),       "Half length fixed length string didn't match");

            Assert.AreEqual(value,                                reader.ReadNullTerminatedString(),                                      "Null terminated string didn't match");
            Assert.AreEqual(value.Substring(0, value.Length / 2), reader.ReadNullTerminatedString(value.Length / 2),                      "Max length null terminated string didnt match");

            reader.Position += 1;

            Assert.AreEqual(value,                                reader.ReadString(Encoding.Unicode),                                    "Unicode prefixed length string didn't match");

            Assert.AreEqual(value + '\0',                         reader.ReadFixedLengthString(value.Length + 1, Encoding.Unicode),       "Unicode fixed length string with padding didn't match");
            Assert.AreEqual(value,                                reader.ReadFixedLengthString(value.Length + 2, true, Encoding.Unicode), "Unicode fixed length string with trimmed padding didn't match");
            Assert.AreEqual(value.Substring(0, value.Length / 2), reader.ReadFixedLengthString(value.Length / 2, Encoding.Unicode),       "Unicode half length fixed length string didn't match");

            Assert.AreEqual(value,                                reader.ReadNullTerminatedString(Encoding.Unicode),                      "Unicode null terminated string didn't match");
            Assert.AreEqual(value.Substring(0, value.Length / 2), reader.ReadNullTerminatedString(value.Length / 2, Encoding.Unicode),    "Unicode max length null terminated string didn't match");

            reader.Position += 14;

            Assert.AreEqual(value.Substring(0, value.Length / 2), reader.ReadNullTerminatedString(Encoding.Unicode),                      "Unicode half length null terminated string didn't match");
        }

        [TestMethod]
        public void Write_ExceptionTests()
        {
            var stream = new MemoryStream();
            var writer = new EndianWriter(stream);

            Assert.ThrowsException<ArgumentNullException>(() => new EndianWriter(null));
            Assert.ThrowsException<ArgumentNullException>(() => new EndianWriter(stream, null));

            Assert.ThrowsException<ArgumentNullException>(() => writer.Write((string)null));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Write("H", -1));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Write(EndianTestData.LittleEndianData, 0, EndianTestData.LittleEndianData.Length + 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Write(EndianTestData.LittleEndianData, EndianTestData.LittleEndianData.Length + 1, EndianTestData.LittleEndianData.Length));

            Assert.ThrowsException<ArgumentNullException>(() => writer.Write(null, 0, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Write(Array.Empty<byte>(), 1, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Write(Array.Empty<byte>(), 0, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Write(Array.Empty<byte>(), -1, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Write(Array.Empty<byte>(), 0, -1));

            Assert.ThrowsException<ArgumentNullException>(() => writer.Write((char[])null, 0, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Write(Array.Empty<char>(), 1, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Write(Array.Empty<char>(), 0, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Write(Array.Empty<char>(), -1, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Write(Array.Empty<char>(), 0, -1));
        }

        [TestMethod]
        public void Write_ArrayTests()
        {
            char[] chars = new char[EndianTestData.LittleEndianData.Length];
            for (int i = 0; i < chars.Length; i++)
                chars[i] = (char)EndianTestData.LittleEndianData[i];

            var stream = new MemoryStream();
            var writer = new EndianWriter(stream, Encoding.ASCII);

            writer.Write(EndianTestData.LittleEndianData, 0, 2);
            writer.Write(EndianTestData.LittleEndianData, 2, 2);
            writer.Write(EndianTestData.LittleEndianData.AsSpan().Slice(0, 2));
            writer.Write(EndianTestData.LittleEndianData.AsSpan().Slice(2, 2));

            writer.Write(chars, 0, 2);
            writer.Write(chars, 2, 2);
            writer.Write(chars.AsSpan().Slice(0, 2));
            writer.Write(chars.AsSpan().Slice(2, 2));

            writer.Position = 0;

            Span<byte> data = [0x5F, 0x50, 0x5F, 0xCC, 0x5F, 0x50, 0x5F, 0xCC, 0x5F, 0x50, 0x5F, 0x3F, 0x5F, 0x50, 0x5F, 0x3F];
            Span<byte> buffer = new byte[stream.Length];

            stream.Read(buffer);

            Assert.AreEqual(true, data.SequenceEqual(buffer));
        }
    }
}