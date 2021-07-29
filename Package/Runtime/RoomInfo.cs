using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace LANMatching
{
    public class RoomInfo
    {
        private const int MAX_BYTES = 512;

        internal bool isOpen;
        public byte capacity;
        public byte currentUser;
        public int port;
        public string name
        {
            get { return nameStr; }
            set
            {
                if (this.nameStr == value)
                {
                    return;
                }
                this.SetBytes(value);
                this.nameStr = value;
            }
        }

        private string nameStr;
        private int nameByteNum;
        private byte[] rawNameBin = new byte[MAX_BYTES];
        


        public RoomInfo()
        {
            this.port = 11111;
            this.isOpen = true;
        }
        public RoomInfo(string n, int p, byte cap)
        {
            this.name = n;
            this.port = p;
            this.capacity = cap;
            this.isOpen = true;
        }

        internal int WriteToByteArray(byte[] data)
        {
            if (isOpen)
            {
                data[0] = 1;
            }
            else
            {
                data[0] = 0;
            }
            data[1] = capacity;
            data[2] = currentUser;

            data[3] = (byte)((this.port >> 0) & 0xff);
            data[4] = (byte)((this.port >> 8) & 0xff);
            data[5] = (byte)((this.port >> 16) & 0xff);
            data[6] = (byte)((this.port >> 24) & 0xff);

            data[7] = (byte)((this.nameByteNum >> 0) & 0xff);
            data[8] = (byte)((this.nameByteNum >> 8) & 0xff);

            for (int i = 0; i < this.nameByteNum; ++i)
            {
                data[i + 9] = this.rawNameBin[i];
            }
            return 9 + this.nameByteNum;
        }

        internal bool ReadFromByteArray(byte[] data, int idx)
        {
            bool isChanged = false;
            bool prevIsOpen = this.isOpen;
            if (data[idx + 0] == 0)
            {
                this.isOpen = false;
            }
            else
            {
                this.isOpen = true;
            }
            if(this.isOpen != prevIsOpen) { isChanged = true; }
            if (this.capacity != data[idx + 1]) { isChanged = true; }
            if (this.currentUser != data[idx + 2]) { isChanged = true; }

            this.capacity = data[idx + 1];
            this.currentUser = data[idx + 2];
            this.port = (data[idx + 3] << 0) +(data[idx + 4] << 8 )+
                (data[idx + 5] << 16 )+(data[idx + 6] << 24);

            int dataNum = data[idx + 7] + (data[idx + 8] << 8);
            if (!SetRawByts(dataNum, data, idx + 9))
            {
                this.nameStr = System.Text.Encoding.UTF8.GetString(this.rawNameBin, 0, this.nameByteNum);
                isChanged = true;
            }
            return isChanged;
        }

        private bool SetRawByts(int dataNum, byte[] data, int idx)
        {
            bool isSame = (this.nameByteNum == dataNum);
            for (int i = 0; i < dataNum; ++i)
            {
                if (data[i + idx] != this.rawNameBin[i])
                {
                    isSame = false;
                }
                this.rawNameBin[i] = data[i + idx];
            }
            this.nameByteNum = dataNum;
            return isSame;
        }

        private void SetBytes(string str)
        {
            int length = str.Length;
            if (length > 256) { length = 256; }
            this.nameByteNum = System.Text.Encoding.UTF8.GetBytes(str, 0, length, rawNameBin, 0);
        }
    }
}