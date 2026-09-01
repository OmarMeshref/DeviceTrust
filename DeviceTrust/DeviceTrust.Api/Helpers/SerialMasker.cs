namespace DeviceTrust.Api.Helpers;

public static class SerialMasker
{
    public static string Mask(string serial)
    {
        if (serial.Length <= 4) return new string('*', serial.Length);
        var prefix = serial[..2];
        var suffix = serial[^2..];
        return $"{prefix}{new string('*', serial.Length - 4)}{suffix}";
    }
}