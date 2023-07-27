namespace PinCode
{
    public interface ICalculator
    {
        public double[,] CreateNewInvolutiveMatrix(byte setSize);
        public void FeistelTranformation(byte firstElement, byte secondElement, params byte[] keys);
        public void InverseOfFeistelTransformation(byte firstElement, byte secondElement, params byte[] keys);
        public void ShiftLeftBytes(byte[] bytes);
        public void ShiftRightBytes(byte[] bytes);
    }
}