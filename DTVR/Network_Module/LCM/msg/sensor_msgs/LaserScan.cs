/* LCM type definition class file
 * This file was automatically generated by lcm-gen
 * DO NOT MODIFY BY HAND!!!!
 */

using System;
using System.Collections.Generic;
using System.IO;
using LCM.LCM;
 
namespace sensor_msgs
{
    public sealed class LaserScan : LCM.LCM.LCMEncodable
    {
        public double angle_min;
        public double angle_max;
        public double angle_increment;
        public double time_increment;
        public double scan_time;
        public double range_min;
        public double range_max;
        public int ranges_size;
        public int intensities_size;
        public double[] ranges;
        public double[] intensities;
 
        public LaserScan()
        {
        }
 
        public static readonly ulong LCM_FINGERPRINT;
        public static readonly ulong LCM_FINGERPRINT_BASE = 0x28181e5e3bee59e8L;
 
        static LaserScan()
        {
            LCM_FINGERPRINT = _hashRecursive(new List<String>());
        }
 
        public static ulong _hashRecursive(List<String> classes)
        {
            if (classes.Contains("sensor_msgs.LaserScan"))
                return 0L;
 
            classes.Add("sensor_msgs.LaserScan");
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
            outs.Write(this.angle_min); 
 
            outs.Write(this.angle_max); 
 
            outs.Write(this.angle_increment); 
 
            outs.Write(this.time_increment); 
 
            outs.Write(this.scan_time); 
 
            outs.Write(this.range_min); 
 
            outs.Write(this.range_max); 
 
            outs.Write(this.ranges_size); 
 
            outs.Write(this.intensities_size); 
 
            for (int a = 0; a < this.ranges_size; a++) {
                outs.Write(this.ranges[a]); 
            }
 
            for (int a = 0; a < this.intensities_size; a++) {
                outs.Write(this.intensities[a]); 
            }
 
        }
 
        public LaserScan(byte[] data) : this(new LCMDataInputStream(data))
        {
        }
 
        public LaserScan(LCMDataInputStream ins)
        {
            if ((ulong) ins.ReadInt64() != LCM_FINGERPRINT)
                throw new System.IO.IOException("LCM Decode error: bad fingerprint");
 
            _decodeRecursive(ins);
        }
 
        public static sensor_msgs.LaserScan _decodeRecursiveFactory(LCMDataInputStream ins)
        {
            sensor_msgs.LaserScan o = new sensor_msgs.LaserScan();
            o._decodeRecursive(ins);
            return o;
        }
 
        public void _decodeRecursive(LCMDataInputStream ins)
        {
            this.angle_min = ins.ReadDouble();
 
            this.angle_max = ins.ReadDouble();
 
            this.angle_increment = ins.ReadDouble();
 
            this.time_increment = ins.ReadDouble();
 
            this.scan_time = ins.ReadDouble();
 
            this.range_min = ins.ReadDouble();
 
            this.range_max = ins.ReadDouble();
 
            this.ranges_size = ins.ReadInt32();
 
            this.intensities_size = ins.ReadInt32();
 
            this.ranges = new double[(int) ranges_size];
            for (int a = 0; a < this.ranges_size; a++) {
                this.ranges[a] = ins.ReadDouble();
            }
 
            this.intensities = new double[(int) intensities_size];
            for (int a = 0; a < this.intensities_size; a++) {
                this.intensities[a] = ins.ReadDouble();
            }
 
        }
 
        public sensor_msgs.LaserScan Copy()
        {
            sensor_msgs.LaserScan outobj = new sensor_msgs.LaserScan();
            outobj.angle_min = this.angle_min;
 
            outobj.angle_max = this.angle_max;
 
            outobj.angle_increment = this.angle_increment;
 
            outobj.time_increment = this.time_increment;
 
            outobj.scan_time = this.scan_time;
 
            outobj.range_min = this.range_min;
 
            outobj.range_max = this.range_max;
 
            outobj.ranges_size = this.ranges_size;
 
            outobj.intensities_size = this.intensities_size;
 
            outobj.ranges = new double[(int) ranges_size];
            for (int a = 0; a < this.ranges_size; a++) {
                outobj.ranges[a] = this.ranges[a];
            }
 
            outobj.intensities = new double[(int) intensities_size];
            for (int a = 0; a < this.intensities_size; a++) {
                outobj.intensities[a] = this.intensities[a];
            }
 
            return outobj;
        }
    }
}

