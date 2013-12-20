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
