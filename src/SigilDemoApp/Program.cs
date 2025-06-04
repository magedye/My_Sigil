using Sigil;
using System;

class Program
{
    static void Main()
    {
        var emit = Emit<Func<int, int, int>>.NewDynamicMethod("Add");
        emit.LoadArgument(0);
        emit.LoadArgument(1);
        emit.Add();
        emit.Return();

        var adder = emit.CreateDelegate();

        Console.WriteLine($"2 + 3 = {adder(2, 3)}");
    }
}
