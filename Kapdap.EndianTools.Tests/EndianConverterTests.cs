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

namespace Kapdap.EndianTools.Tests
{
    [TestClass]
    public class EndianConverterTests
    {
        [TestMethod]
        public void GetBytes_BytesShouldBeBigEndian()
        {
            Assert.AreEqual(true, EndianConverter.GetBytes((short)EndianTestData.TestValues[0],  ByteOrder.BigEndian).AsSpan().SequenceEqual(EndianTestData.TestValuesBytesBE[0]));
            Assert.AreEqual(true, EndianConverter.GetBytes((int)EndianTestData.TestValues[1],    ByteOrder.BigEndian).AsSpan().SequenceEqual(EndianTestData.TestValuesBytesBE[1]));
            Assert.AreEqual(true, EndianConverter.GetBytes((long)EndianTestData.TestValues[2],   ByteOrder.BigEndian).AsSpan().SequenceEqual(EndianTestData.TestValuesBytesBE[2]));
            Assert.AreEqual(true, EndianConverter.GetBytes((ushort)EndianTestData.TestValues[3], ByteOrder.BigEndian).AsSpan().SequenceEqual(EndianTestData.TestValuesBytesBE[3]));
            Assert.AreEqual(true, EndianConverter.GetBytes((uint)EndianTestData.TestValues[4],   ByteOrder.BigEndian).AsSpan().SequenceEqual(EndianTestData.TestValuesBytesBE[4]));
            Assert.AreEqual(true, EndianConverter.GetBytes((ulong)EndianTestData.TestValues[5],  ByteOrder.BigEndian).AsSpan().SequenceEqual(EndianTestData.TestValuesBytesBE[5]));
            Assert.AreEqual(true, EndianConverter.GetBytes((double)EndianTestData.TestValues[6], ByteOrder.BigEndian).AsSpan().SequenceEqual(EndianTestData.TestValuesBytesBE[6]));
            Assert.AreEqual(true, EndianConverter.GetBytes((float)EndianTestData.TestValues[7],  ByteOrder.BigEndian).AsSpan().SequenceEqual(EndianTestData.TestValuesBytesBE[7]));
        }

        [TestMethod]
        public void GetBytes_BytesShouldBeLittleEndian()
        {
            Assert.AreEqual(true, EndianConverter.GetBytes((short)EndianTestData.TestValues[0],  ByteOrder.LittleEndian).AsSpan().SequenceEqual(EndianTestData.TestValuesBytesLE[0]));
            Assert.AreEqual(true, EndianConverter.GetBytes((int)EndianTestData.TestValues[1],    ByteOrder.LittleEndian).AsSpan().SequenceEqual(EndianTestData.TestValuesBytesLE[1]));
            Assert.AreEqual(true, EndianConverter.GetBytes((long)EndianTestData.TestValues[2],   ByteOrder.LittleEndian).AsSpan().SequenceEqual(EndianTestData.TestValuesBytesLE[2]));
            Assert.AreEqual(true, EndianConverter.GetBytes((ushort)EndianTestData.TestValues[3], ByteOrder.LittleEndian).AsSpan().SequenceEqual(EndianTestData.TestValuesBytesLE[3]));
            Assert.AreEqual(true, EndianConverter.GetBytes((uint)EndianTestData.TestValues[4],   ByteOrder.LittleEndian).AsSpan().SequenceEqual(EndianTestData.TestValuesBytesLE[4]));
            Assert.AreEqual(true, EndianConverter.GetBytes((ulong)EndianTestData.TestValues[5],  ByteOrder.LittleEndian).AsSpan().SequenceEqual(EndianTestData.TestValuesBytesLE[5]));
            Assert.AreEqual(true, EndianConverter.GetBytes((double)EndianTestData.TestValues[6], ByteOrder.LittleEndian).AsSpan().SequenceEqual(EndianTestData.TestValuesBytesLE[6]));
            Assert.AreEqual(true, EndianConverter.GetBytes((float)EndianTestData.TestValues[7],  ByteOrder.LittleEndian).AsSpan().SequenceEqual(EndianTestData.TestValuesBytesLE[7]));
        }

        [TestMethod]
        public void ReverseEndianness_ValuesShouldBeReversed()
        {
            List<object> expectedResults = new()
            {
                (short)0x5F50,
                0x5FCCAA00,
                0x5FCCAA8866442200,
                (ushort)0xFF00,
                0xFFCCAA00,
                0xFFCCAA8866442200
            };

            Assert.AreEqual((short)expectedResults[0],  EndianConverter.ReverseEndianness((short)EndianTestData.TestValues[0]));
            Assert.AreEqual((int)expectedResults[1],    EndianConverter.ReverseEndianness((int)EndianTestData.TestValues[1]));
            Assert.AreEqual((long)expectedResults[2],   EndianConverter.ReverseEndianness((long)EndianTestData.TestValues[2]));
            Assert.AreEqual((ushort)expectedResults[3], EndianConverter.ReverseEndianness((ushort)EndianTestData.TestValues[3]));
            Assert.AreEqual((uint)expectedResults[4],   EndianConverter.ReverseEndianness((uint)EndianTestData.TestValues[4]));
            Assert.AreEqual((ulong)expectedResults[5],  EndianConverter.ReverseEndianness((ulong)EndianTestData.TestValues[5]));
        }
    }
}