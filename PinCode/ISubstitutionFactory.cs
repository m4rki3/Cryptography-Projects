using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PinCode;
public interface ISubstitutionFactory
{
    public byte[] MakeNewSubstitutionOnSet(byte setSize, byte maxValue);
    public byte[] MakeNewSubstitutionBasedOnDiscreteLogarithm();
}