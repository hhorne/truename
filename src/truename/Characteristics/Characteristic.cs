namespace truename.Characteristics;

public interface ICharacteristic
{
    object Value { get; }
}

public interface ICharacteristic<T> : ICharacteristic
{
    new T Value { get; }
}

public class Characteristic<T> : ICharacteristic<T>
{
    public T Value { get; }

    object ICharacteristic.Value => Value ?? throw new NullReferenceException();

    public Characteristic(T value)
    {
        Value = value;
    }
}