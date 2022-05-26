/* LCM type definition class file
 * This file was automatically generated by lcm-gen
 * DO NOT MODIFY BY HAND!!!!
 */

using System;
using System.Collections.Generic;
using System.IO;
using LCM.LCM;
 
namespace geometry_msgs
{
    public sealed class Pose2D : LCM.LCM.LCMEncodable
    {
        public double x;
        public double y;
        public double theta;
 
        public Pose2D()
        {
        }
 
        public static readonly ulong LCM_FINGERPRINT;
        public static readonly ulong LCM_FINGERPRINT_BASE = 0x7491c1074c104593L;
 
        static Pose2D()
        {
            LCM_FINGERPRINT = _hashRecursive(new List<String>());
        }
 
        public static ulong _hashRecursive(List<String> classes)
        {
            if (classes.Contains("geometry_msgs.Pose2D"))
                return 0L;
 
            classes.Add("geometry_msgs.Pose2D");
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
            outs.Write(this.x); 
 
            outs.Write(this.y); 
 
            outs.Write(this.theta); 
 
        }
 
        public Pose2D(byte[] data) : this(new LCMDataInputStream(data))
        {
        }
 
        public Pose2D(LCMDataInputStream ins)
        {
            if ((ulong) ins.ReadInt64() != LCM_FINGERPRINT)
                throw new System.IO.IOException("LCM Decode error: bad fingerprint");
 
            _decodeRecursive(ins);
        }
 
        public static geometry_msgs.Pose2D _decodeRecursiveFactory(LCMDataInputStream ins)
        {
            geometry_msgs.Pose2D o = new geometry_msgs.Pose2D();
            o._decodeRecursive(ins);
            return o;
        }
 
        public void _decodeRecursive(LCMDataInputStream ins)
        {
            this.x = ins.ReadDouble();
 
            this.y = ins.ReadDouble();
 
            this.theta = ins.ReadDouble();
 
        }
 
        public geometry_msgs.Pose2D Copy()
        {
            geometry_msgs.Pose2D outobj = new geometry_msgs.Pose2D();
            outobj.x = this.x;
 
            outobj.y = this.y;
 
            outobj.theta = this.theta;
 
            return outobj;
        }
    }
}
