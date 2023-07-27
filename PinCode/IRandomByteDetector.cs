using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PinCode;
public interface IRandomByteDetector
{
    public byte GetRandomByte(byte maxValue);
}