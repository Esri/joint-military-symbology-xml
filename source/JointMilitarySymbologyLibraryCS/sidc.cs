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
    public class sidc
    {
        private UInt32 _first10;
        private UInt32 _second10;

        public sidc(UInt32 partA = 1000000000, UInt32 partB = 1000000000)
        {
            this._first10 = partA;
            this._second10 = partB;
        }

        public UInt32 partAUInt
        {
            get
            {
                return this._first10;
            }

            set
            {
                this._first10 = value;
            }
        }

        public UInt32 partBUInt
        {
            get
            {
                return this._second10;
            }

            set
            {
                this._second10 = value;
            }
        }

        public string partAString
        {
            get
            {
                return this._first10.ToString();
            }

            set
            {
                this._first10 = Convert.ToUInt32(value);
            }
        }

        public string partBString
        {
            get
            {
                return this._second10.ToString();
            }

            set
            {
                this._second10 = Convert.ToUInt32(value);
            }
        }
    }
}
