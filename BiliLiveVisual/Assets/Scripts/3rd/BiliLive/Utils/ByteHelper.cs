using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class ByteHelper
{
    /// <summary>
    /// 获取取第index位 
    /// </summary>
    /// <param name="b"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static int GetBit(byte b, int index)
    {
        return ((b & (1 << index)) > 0) ? 1 : 0;
    }
    /// <summary>
    /// 将第index位设为1 
    /// </summary>
    /// <param name="b"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static byte SetBit(byte b, int index)
    {
        return (byte)(b | (1 << index));
    }
    /// <summary>
    /// 将第index位设为0 
    /// </summary>
    /// <param name="b"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static byte ClearBit(byte b, int index)
    {
        return (byte)(b & (byte.MaxValue - (1 << index)));
    }
    /// <summary>
    /// 将第index位取反 
    /// </summary>
    /// <param name="b"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static byte ReverseBit(byte b, int index)
    {
        return (byte)(b ^ (byte)(1 << index));
    }
    /// <summary>
    /// 将字节A写入目标字节B的指定位 
    /// </summary>
    /// <param name="Value">字节B 超过8位时低位数组索引小于高位</param>
    /// <param name="Index">起始位置</param>
    /// <param name="Leng">占位长度</param>
    /// <param name="OriginalValue">A字节</param>
    /// <returns></returns>
    public static byte[] BitProcessing(byte[] Value, int Index, int Leng, byte OriginalValue)
    {
        bool Med;
        for (int index = 1; index <= Leng; index++)
        {
            byte Weight = (byte)Math.Pow(2, index - 1);
            Med = ((OriginalValue & Weight) == Weight);
            int Cursor = Index + index - 1;
            Value[Cursor / 8] = set_bit(Value[Cursor / 8], Cursor % 8, Med);
        }
        return Value;
    }
    /// <summary>
    /// 设置字节任意位
    /// </summary>
    /// <param name="data"></param>
    /// <param name="index"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    private static byte set_bit(byte data, int index, bool flag)
    {
        ++index;
        if (index > 8 || index < 1)
            throw new ArgumentOutOfRangeException();
        int v = index < 2 ? index : (2 << (index - 2));
        return flag ? (byte)(data | v) : (byte)(data & ~v);
    }

    ///// <summary>
    /////  [StructLayout(LayoutKind.Sequential, Pack = 1)]设置对齐方式
    ///// </summary>
    ///// <param name="structObj"></param>
    ///// <returns></returns>
    //public static byte[] StructToBytes(object structObj)
    //{

    //    //返回类的非托管大小（以字节为单位）  
    //    int size = Marshal.SizeOf(structObj);

    //    //分配大小  
    //    byte[] bytes = new byte[size];

    //    //从进程的非托管堆中分配内存给structPtr  
    //    IntPtr structPtr = Marshal.AllocHGlobal(size);
    //    try
    //    {
    //        //将数据从托管对象structObj封送到非托管内存块structPtr  
    //        Marshal.StructureToPtr(structObj, structPtr, false);

    //        //Marshal.StructureToPtr(structObj, structPtr, true);  
    //        //将数据从非托管内存指针复制到托管 8 位无符号整数数组  
    //        Marshal.Copy(structPtr, bytes, 0, size);

    //        return bytes;
    //    }
    //    catch (Exception _)
    //    {
    //        return null;
    //    }
    //    finally
    //    {
    //        //释放以前使用 AllocHGlobal 从进程的非托管内存中分配的内存  
    //        Marshal.FreeHGlobal(structPtr);
    //    }
    //}
    //public static object BytesToStruct(byte[] bytes, Type strType)
    //{
    //    //获取结构体的大小（以字节为单位）  
    //    int size = Marshal.SizeOf(strType);
    //    //简单的判断（可以去掉）  
    //    if (size > bytes.Length)
    //    {
    //        return null;
    //    }

    //    //从进程的非托管堆中分配内存给structPtr  
    //    IntPtr strPtr = Marshal.AllocHGlobal(size);
    //    try
    //    {

    //        //将数据从一维托管数组bytes复制到非托管内存指针strPtr  
    //        Marshal.Copy(bytes, 0, strPtr, size);

    //        //将数据从非托管内存块封送到新分配的指定类型的托管对象  
    //        //将内存空间转换为目标结构体  
    //        object obj = Marshal.PtrToStructure(strPtr, strType);

    //        return obj;
    //    }
    //    catch (Exception _)
    //    {
    //        return null;
    //    }
    //    finally
    //    {
    //        //释放以前使用 AllocHGlobal 从进程的非托管内存中分配的内存 
    //        Marshal.FreeHGlobal(strPtr);
    //    }
    //}

    //规定转换起始位置和长度
    public static void ReverseBytes(byte[] bytes, int start, int len)
    {
        int end = start + len - 1;
        byte tmp;
        int i = 0;
        for (int index = start; index < start + len / 2; index++, i++)
        {
            tmp = bytes[end - i];
            bytes[end - i] = bytes[index];
            bytes[index] = tmp;
        }
    }

    //合并字节数组
    public static byte[] CombineBytes(byte[] bytes1, byte[] bytes2)
    {
        int allLen = ((bytes1 != null) ? bytes1.Length : 0) + ((bytes2 != null) ? bytes2.Length : 0);
        byte[] byteAll = new byte[allLen];
        if (bytes1 != null && bytes1.Length > 0)
        {
            Array.Copy(bytes1, 0, byteAll, 0, bytes1.Length);
            if (bytes2 != null && bytes2.Length > 0)
            {
                Array.Copy(bytes2, 0, byteAll, bytes1.Length, bytes2.Length);
            }
        }

        return byteAll;
    }

    //分割字节数组
    public static bool SpliteBytes(byte[] data ,in byte[] bytes1, in byte[] bytes2)
    {
        if (data == null || data.Length <= 0)
        {
            return false;
        }

        if (bytes1 != null)
        {
            Array.Copy(data, 0, bytes1, 0, Math.Min(data.Length, bytes1.Length));
            if (bytes2 != null)
            {
                Array.Copy(data, bytes1.Length, bytes2, 0, Math.Min(data.Length - bytes1.Length, bytes2.Length));
            }
        }
        return true;
    }

    //截取
    public static byte[] SubBytes(byte[] srcArray, int start, int length)
    {
        if (length <= 0)
            return default;

        byte[] retArray = new byte[length];
        Array.Copy(srcArray, start, retArray, 0, length);
        return retArray;
    }

    /// <summary>
    /// 结构体转字节数组（按小端模式）
    /// </summary>
    /// <param name="obj">struct type</param>
    /// <returns></returns>
    public static byte[] StructureToByteArray(object obj)
    {
        int len = Marshal.SizeOf(obj);
        byte[] arr = new byte[len];
        IntPtr ptr = Marshal.AllocHGlobal(len);
        Marshal.StructureToPtr(obj, ptr, true);
        Marshal.Copy(ptr, arr, 0, len);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }

    /// <summary>
    /// 结构体转字节数组（按大端模式）
    /// </summary>
    /// <param name="obj">struct type</param>
    /// <returns></returns>
    public static byte[] StructureToByteArrayEndian(object obj)
    {
        object thisBoxed = obj;   //copy ，将 struct 装箱
        Type test = thisBoxed.GetType();

        int offset = 0;
        byte[] data = new byte[Marshal.SizeOf(thisBoxed)];

        object fieldValue;
        TypeCode typeCode;
        byte[] temp;
        // 列举结构体的每个成员，并Reverse
        foreach (var field in test.GetFields())
        {
            fieldValue = field.GetValue(thisBoxed); // Get value

            typeCode = Type.GetTypeCode(fieldValue.GetType());  // get type

            switch (typeCode)
            {
                case TypeCode.Single: // float
                    {
                        temp = BitConverter.GetBytes((Single)fieldValue);
                        Array.Reverse(temp);
                        Array.Copy(temp, 0, data, offset, sizeof(Single));
                        break;
                    }
                case TypeCode.Int32:
                    {
                        temp = BitConverter.GetBytes((Int32)fieldValue);
                        Array.Reverse(temp);
                        Array.Copy(temp, 0, data, offset, sizeof(Int32));
                        break;
                    }
                case TypeCode.UInt32:
                    {
                        temp = BitConverter.GetBytes((UInt32)fieldValue);
                        Array.Reverse(temp);
                        Array.Copy(temp, 0, data, offset, sizeof(UInt32));
                        break;
                    }
                case TypeCode.Int16:
                    {
                        temp = BitConverter.GetBytes((Int16)fieldValue);
                        Array.Reverse(temp);
                        Array.Copy(temp, 0, data, offset, sizeof(Int16));
                        break;
                    }
                case TypeCode.UInt16:
                    {
                        temp = BitConverter.GetBytes((UInt16)fieldValue);
                        Array.Reverse(temp);
                        Array.Copy(temp, 0, data, offset, sizeof(UInt16));
                        break;
                    }
                case TypeCode.Int64:
                    {
                        temp = BitConverter.GetBytes((Int64)fieldValue);
                        Array.Reverse(temp);
                        Array.Copy(temp, 0, data, offset, sizeof(Int64));
                        break;
                    }
                case TypeCode.UInt64:
                    {
                        temp = BitConverter.GetBytes((UInt64)fieldValue);
                        Array.Reverse(temp);
                        Array.Copy(temp, 0, data, offset, sizeof(UInt64));
                        break;
                    }
                case TypeCode.Double:
                    {
                        temp = BitConverter.GetBytes((Double)fieldValue);
                        Array.Reverse(temp);
                        Array.Copy(temp, 0, data, offset, sizeof(Double));
                        break;
                    }
                case TypeCode.Byte:
                    {
                        data[offset] = (Byte)fieldValue;
                        break;
                    }
                default:
                    {
                        //System.Diagnostics.Debug.Fail("No conversion provided for this type : " + typeCode.ToString());
                        break;
                    }
            }; // switch
            if (typeCode == TypeCode.Object)
            {
                int length = ((byte[])fieldValue).Length;
                Array.Copy(((byte[])fieldValue), 0, data, offset, length);
                offset += length;
            }
            else
            {
                offset += Marshal.SizeOf(fieldValue);
            }
        } // foreach

        return data;
    } // Swap


    /// <summary>
    /// 字节数组转结构体(按小端模式)
    /// </summary>
    /// <param name="bytearray">字节数组</param>
    /// <param name="obj">目标结构体</param>
    /// <param name="startoffset">bytearray内的起始位置</param>
    public static void ByteArrayToStructure(byte[] bytearray, ref object obj, int startoffset)
    {
        int len = Marshal.SizeOf(obj);
        IntPtr i = Marshal.AllocHGlobal(len);
        // 从结构体指针构造结构体
        obj = Marshal.PtrToStructure(i, obj.GetType());
        try
        {
            // 将字节数组复制到结构体指针
            Marshal.Copy(bytearray, startoffset, i, len);
        }
        catch (Exception ex) { Console.WriteLine("ByteArrayToStructure FAIL: error " + ex.ToString()); }
        obj = Marshal.PtrToStructure(i, obj.GetType());
        Marshal.FreeHGlobal(i);  //释放内存，与 AllocHGlobal() 对应

    }

    /// <summary>
    /// 字节数组转结构体(按大端模式)
    /// </summary>
    /// <param name="bytearray">字节数组</param>
    /// <param name="obj">目标结构体</param>
    /// <param name="startoffset">bytearray内的起始位置</param>
    public static void ByteArrayToStructureEndian(byte[] bytearray, ref object obj, int startoffset)
    {
        int len = Marshal.SizeOf(obj);
        IntPtr i = Marshal.AllocHGlobal(len);
        byte[] temparray = (byte[])bytearray.Clone();
        // 从结构体指针构造结构体
        obj = Marshal.PtrToStructure(i, obj.GetType());
        // 做大端转换
        object thisBoxed = obj;
        Type test = thisBoxed.GetType();
        int reversestartoffset = startoffset;
        // 列举结构体的每个成员，并Reverse
        foreach (var field in test.GetFields())
        {
            object fieldValue = field.GetValue(thisBoxed); // Get value

            TypeCode typeCode = Type.GetTypeCode(fieldValue.GetType());  //Get Type
            if (typeCode != TypeCode.Object)  //如果为值类型
            {
                Array.Reverse(temparray, reversestartoffset, Marshal.SizeOf(fieldValue));
                reversestartoffset += Marshal.SizeOf(fieldValue);
            }
            else  //如果为引用类型
            {
                reversestartoffset += ((byte[])fieldValue).Length;
            }
        }
        try
        {
            //将字节数组复制到结构体指针
            Marshal.Copy(temparray, startoffset, i, len);
        }
        catch (Exception ex) { Console.WriteLine("ByteArrayToStructure FAIL: error " + ex.ToString()); }
        obj = Marshal.PtrToStructure(i, obj.GetType());
        Marshal.FreeHGlobal(i);  //释放内存
    }
}
