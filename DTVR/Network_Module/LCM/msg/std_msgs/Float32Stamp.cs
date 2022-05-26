/* LCM type definition class file
 * This file was automatically generated by lcm-gen
 * DO NOT MODIFY BY HAND!!!!
 */

using System;
using System.Collections.Generic;
using System.IO;
using LCM.LCM;
 
namespace std_msgs
{
    public sealed class Float32Stamp : LCM.LCM.LCMEncodable
    {
        public double timestamp;
        public float data;
 
        public Float32Stamp()
        {
        }
 
        public static readonly ulong LCM_FINGERPRINT;
        public static readonly ulong LCM_FINGERPRINT_BASE = 0x5edb9a2ffe2d43f9L;
 
        static Float32Stamp()
        {
            LCM_FINGERPRINT = _hashRecursive(new List<String>());
        }
 
        public static ulong _hashRecursive(List<String> classes)
        {
            if (classes.Contains("std_msgs.Float32Stamp"))
                return 0L;
 
            classes.Add("std_msgs.Float32Stamp");
            ulong hash = LCM_FINGERPRINT_BASE
                ;
            classes.RemoveAt(classes.Count - 1);
            return (hash<<1) + ((hash>>63)&1);
        }
 
        public void Encode(LCMDataOutputStream outs)
        {
            outs.Write((long) LCM_FINGERPRINT);
            _encodeRecursive(outs);
        }
 
        public void _encodeRecursive(LCMDataOutputStream outs)
        {
            outs.Write(this.timestamp); 
 
            outs.Write(this.data); 
 
        }
 
        public Float32Stamp(byte[] data) : this(new LCMDataInputStream(data))
        {
        }
 
        public Float32Stamp(LCMDataInputStream ins)
        {
            if ((ulong) ins.ReadInt64() != LCM_FINGERPRINT)
                throw new System.IO.IOException("LCM Decode error: bad fingerprint");
 
            _decodeRecursive(ins);
        }
 
        public static std_msgs.Float32Stamp _decodeRecursiveFactory(LCMDataInputStream ins)
        {
            std_msgs.Float32Stamp o = new std_msgs.Float32Stamp();
            o._decodeRecursive(ins);
            return o;
        }
 
        public void _decodeRecursive(LCMDataInputStream ins)
        {
            this.timestamp = ins.ReadDouble();
 
            this.data = ins.ReadSingle();
 
        }
 
        public std_msgs.Float32Stamp Copy()
        {
            std_msgs.Float32Stamp outobj = new std_msgs.Float32Stamp();
            outobj.timestamp = this.timestamp;
 
            outobj.data = this.data;
 
            return outobj;
        }
    }
}

