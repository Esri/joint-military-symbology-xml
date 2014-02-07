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

        public SIDC(UInt32 partA = 1000980000, UInt32 partB = 1000000000)
        {
            if (partA >= 1000000000 && partB >= 1000000000)
            {
                this._first10 = partA;
                this._second10 = partB;
            }
            else
            {
                this._first10 = 1000980000;
                this._second10 = 1000000000;
            }
        }

        public SIDC(string partA, string partB)
        {
            UInt32 p1;
            UInt32 p2;

            if(partA.Length != 10 || partB.Length != 10)
            {
                partA = "1000980000";
                partB = "1000000000";
            }

            try
            {
                p1 = Convert.ToUInt32(partA);
                p2 = Convert.ToUInt32(partB);
            }
            catch
            {
                p1 = 1000980000;
                p2 = 1000000000;
            }

            if (p1 >= 1000000000 && p2 >= 1000000000)
            {
                this._first10 = p1;
                this._second10 = p2;
            }
            else
            {
                this._first10 = 1000980000;
                this._second10 = 1000000000;
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
                if (value >= 1000000000)
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
                if (value >= 1000000000)
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
                        this._first10 = 1000980000;
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
                        this._second10 = 1000000000;
                    }
                }
            }
        }
    }
}
