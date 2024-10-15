namespace BookishAPI;

public class CodeGenerator
{
    private readonly Random random = new Random();

    public string Generate4DigitCode()
    {
        int code = random.Next(0, 10000);
        return code.ToString("D4");
    }
}