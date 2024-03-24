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

namespace Kapdap.EndianTools.Tests
{
    [TestClass]
    public class EndianReaderTests
    {
        [TestMethod]
        public void Read_ValuesShouldReadFormBigEndianData()
        {
            // Copy values to a stream
            var stream = new MemoryStream();
            stream.Write(EndianTestData.BigEndianData, 0, EndianTestData.BigEndianData.Length);
            stream.Position = 0;

            // Create a reader using big endian format
            var reader = new EndianReader(stream, ByteOrder.BigEndian);

            // Asserts
            Assert.AreEqual(EndianTestData.TestValues[0], reader.ReadInt16());
            Assert.AreEqual(EndianTestData.TestValues[1], reader.ReadInt32());
            Assert.AreEqual(EndianTestData.TestValues[2], reader.ReadInt64());
            Assert.AreEqual(EndianTestData.TestValues[3], reader.ReadUInt16());
            Assert.AreEqual(EndianTestData.TestValues[4], reader.ReadUInt32());
            Assert.AreEqual(EndianTestData.TestValues[5], reader.ReadUInt64());
            Assert.AreEqual(EndianTestData.TestValues[6], reader.ReadDouble());
            Assert.AreEqual(EndianTestData.TestValues[7], reader.ReadFloat());
            Assert.AreEqual(EndianTestData.TestValues[8], reader.ReadFixedLengthString(12));
            Assert.AreEqual(EndianTestData.TestValues[9], reader.ReadBoolean());
            Assert.AreEqual(EndianTestData.TestValues[10], reader.ReadBoolean());
        }

        [TestMethod]
        public void Read_ValuesShouldReadFormLittleEndianData()
        {
            // Copy values to a stream
            var stream = new MemoryStream();
            stream.Write(EndianTestData.LittleEndianData, 0, EndianTestData.LittleEndianData.Length);
            stream.Position = 0;

            // Create a reader using little endian format
            var reader = new EndianReader(stream, ByteOrder.LittleEndian);

            // Asserts
            Assert.AreEqual(EndianTestData.TestValues[0], reader.ReadInt16());
            Assert.AreEqual(EndianTestData.TestValues[1], reader.ReadInt32());
            Assert.AreEqual(EndianTestData.TestValues[2], reader.ReadInt64());
            Assert.AreEqual(EndianTestData.TestValues[3], reader.ReadUInt16());
            Assert.AreEqual(EndianTestData.TestValues[4], reader.ReadUInt32());
            Assert.AreEqual(EndianTestData.TestValues[5], reader.ReadUInt64());
            Assert.AreEqual(EndianTestData.TestValues[6], reader.ReadDouble());
            Assert.AreEqual(EndianTestData.TestValues[7], reader.ReadFloat());
            Assert.AreEqual(EndianTestData.TestValues[8], reader.ReadFixedLengthString(12));
            Assert.AreEqual(EndianTestData.TestValues[9], reader.ReadBoolean());
            Assert.AreEqual(EndianTestData.TestValues[10], reader.ReadBoolean());
        }
    }
}