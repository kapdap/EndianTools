// SPDX-FileCopyrightText: 2024 Kapdap <kapdap@pm.me>
//
// SPDX-License-Identifier: MIT
/*  Kapdap.EndianTools
 *  
 *  Copyright (c) 2024 Kapdap <kapdap@pm.me>
 *
 *  Use of this source code is governed by an MIT-style
 *  license that can be found in the LICENSE file or at
 *  https://opensource.org/licenses/MIT.
 */

using System;

namespace Kapdap.EndianTools
{
    public static class EndianUtilities
    {
        public static bool IsByteReversalRequired(ByteOrder order) =>
            (!BitConverter.IsLittleEndian && order == ByteOrder.LittleEndian) || (BitConverter.IsLittleEndian && order == ByteOrder.BigEndian);
    }
}