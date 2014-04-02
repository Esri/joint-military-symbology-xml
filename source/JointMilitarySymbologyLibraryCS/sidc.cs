/* Copyright 2014 Esri
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JointMilitarySymbologyLibrary
{
    public class SIDC
    {
        private UInt32 _first10;
        private UInt32 _second10;

        private static UInt32 _smallest = 1000000000;
        private static UInt32 _specialPartA = 1000980000;
        private static UInt32 _invalidPartB = 1000000000;
        private static UInt32 _retiredPartB = 1100000000;

        public static SIDC INVALID = new SIDC(_specialPartA, _invalidPartB);
        public static SIDC RETIRED = new SIDC(_specialPartA, _retiredPartB);

        public SIDC(UInt32 partA = 1000980000, UInt32 partB = 1000000000)
        {
            if (partA >= _smallest && partB >= _smallest)
            {
                this._first10 = partA;
                this._second10 = partB;
            }
            else
            {
                this._first10 = _specialPartA;
                this._second10 = _invalidPartB;
            }
        }

        public SIDC(string partA, string partB)
        {
            UInt32 p1;
            UInt32 p2;

            if(partA.Length != 10 || partB.Length != 10)
            {
                partA = SIDC.INVALID.PartAString;
                partB = SIDC.INVALID.PartBString;
            }

            try
            {
                p1 = Convert.ToUInt32(partA);
                p2 = Convert.ToUInt32(partB);
            }
            catch
            {
                p1 = _specialPartA;
                p2 = _invalidPartB;
            }

            if (p1 >= _smallest && p2 >= _smallest)
            {
                this._first10 = p1;
                this._second10 = p2;
            }
            else
            {
                this._first10 = _specialPartA;
                this._second10 = _invalidPartB;
            }
        }

        public UInt32 PartAUInt
        {
            get
            {
                return this._first10;
            }

            set
            {
                if (value >= _smallest)
                {
                    this._first10 = value;
                }
            }
        }

        public UInt32 PartBUInt
        {
            get
            {
                return this._second10;
            }

            set
            {
                if (value >= _smallest)
                {
                    this._second10 = value;
                }
            }
        }

        public string PartAString
        {
            get
            {
                return this._first10.ToString();
            }

            set
            {
                if (value.Length == 10)
                {
                    try
                    {
                        this._first10 = Convert.ToUInt32(value);
                    }
                    catch
                    {
                        this._first10 = _specialPartA;
                    }
                }
            }
        }

        public string PartBString
        {
            get
            {
                return this._second10.ToString();
            }

            set
            {
                if (value.Length == 10)
                {
                    try
                    {
                        this._second10 = Convert.ToUInt32(value);
                    }
                    catch
                    {
                        this._second10 = _invalidPartB;
                    }
                }
            }
        }
    }
}
