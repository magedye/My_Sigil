using System;
using System.Text;
using Sigil;

namespace SigilTestApp;

public class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        while (true)
        {
            Console.WriteLine("\nاختَر الاختبار الذي تريد تشغيله:");
            Console.WriteLine("1) العمليات الحسابية الأساسية");
            Console.WriteLine("2) المتغيرات المحلية");
            Console.WriteLine("3) التحكم بالتدفق");
            Console.WriteLine("4) معالجة الاستثناءات");
            Console.WriteLine("5) إنشاء الكائنات");
            Console.WriteLine("6) التحقق من الأخطاء");
            Console.WriteLine("7) تحسين الفروع");
            Console.WriteLine("8) عرض التعليمات (Disassembly)");
            Console.WriteLine("0) خروج");
            Console.Write("اختيارك: ");

            var choice = Console.ReadLine();
            Console.WriteLine();
            if (string.IsNullOrWhiteSpace(choice)) continue;
            if (choice == "0") break;

            switch (choice)
            {
                case "1": TestBasicArithmetic(); break;
                case "2": TestLocalVariables(); break;
                case "3": TestControlFlow(); break;
                case "4": TestExceptionHandling(); break;
                case "5": TestObjectCreation(); break;
                case "6": TestErrorValidation(); break;
                case "7": TestOptimizations(); break;
                case "8": TestDisassembly(); break;
                default: Console.WriteLine("خيار غير معروف"); break;
            }
        }
    }

    static void TestBasicArithmetic()
    {
        Console.WriteLine("-- اختبار العمليات الحسابية الأساسية --");

        var add = Emit<Func<int, int, int>>.NewDynamicMethod("Add");
        add.LoadArgument(0);
        add.LoadArgument(1);
        add.Add();
        add.Return();
        var addDel = add.CreateDelegate();
        Console.WriteLine($"3 + 5 = {addDel(3, 5)}");

        var mul = Emit<Func<int, int, int>>.NewDynamicMethod("Multiply");
        mul.LoadArgument(0);
        mul.LoadArgument(1);
        mul.Multiply();
        mul.Return();
        var mulDel = mul.CreateDelegate();
        Console.WriteLine($"4 * 6 = {mulDel(4, 6)}");

        var expr = Emit<Func<int, int, int, int>>.NewDynamicMethod("Expression");
        expr.LoadArgument(0);
        expr.LoadArgument(1);
        expr.LoadArgument(2);
        expr.Multiply();
        expr.Add();
        expr.Return();
        var exprDel = expr.CreateDelegate();
        Console.WriteLine($"2 + 3 * 4 = {exprDel(2, 3, 4)}");
    }

    static void TestLocalVariables()
    {
        Console.WriteLine("-- اختبار المتغيرات المحلية --");

        var emit = Emit<Func<int, int>>.NewDynamicMethod("LocalDemo");
        var temp = emit.DeclareLocal<int>("temp");
        emit.LoadArgument(0);
        emit.LoadConstant(2);
        emit.Multiply();
        emit.StoreLocal(temp);
        emit.LoadLocal(temp);
        emit.LoadConstant(5);
        emit.Add();
        emit.Return();
        var del = emit.CreateDelegate();
        Console.WriteLine($"(7 * 2) + 5 = {del(7)}");
    }

    static void TestControlFlow()
    {
        Console.WriteLine("-- اختبار التحكم بالتدفق --");

        var emit = Emit<Func<int, string>>.NewDynamicMethod("FlowDemo");
        var pos = emit.DefineLabel("POS");
        var neg = emit.DefineLabel("NEG");
        var end = emit.DefineLabel("END");

        emit.LoadArgument(0);
        emit.LoadConstant(0);
        emit.BranchIfGreater(pos);

        emit.LoadArgument(0);
        emit.LoadConstant(0);
        emit.BranchIfLess(neg);

        emit.LoadConstant("صفر");
        emit.Branch(end);

        emit.MarkLabel(pos);
        emit.LoadConstant("موجب");
        emit.Branch(end);

        emit.MarkLabel(neg);
        emit.LoadConstant("سالب");

        emit.MarkLabel(end);
        emit.Return();

        var del = emit.CreateDelegate();
        Console.WriteLine($"5 => {del(5)}");
        Console.WriteLine($"-2 => {del(-2)}");
        Console.WriteLine($"0 => {del(0)}");
    }

    static void TestExceptionHandling()
    {
        Console.WriteLine("-- اختبار معالجة الاستثناءات --");

        var emit = Emit<Func<int, int, int>>.NewDynamicMethod("SafeDivide");
        var result = emit.DeclareLocal<int>("result");
        emit.LoadConstant(0);
        emit.StoreLocal(result);

        var tryBlock = emit.BeginExceptionBlock();
        emit.LoadArgument(0);
        emit.LoadArgument(1);
        emit.Divide();
        emit.StoreLocal(result);

        var c = emit.BeginCatchBlock<DivideByZeroException>(tryBlock);
        emit.Pop();
        emit.EndCatchBlock(c);
        emit.EndExceptionBlock(tryBlock);

        emit.LoadLocal(result);
        emit.Return();

        var del = emit.CreateDelegate();
        Console.WriteLine($"6 / 3 = {del(6, 3)}");
        Console.WriteLine($"6 / 0 = {del(6, 0)}");
    }

    static void TestObjectCreation()
    {
        Console.WriteLine("-- اختبار إنشاء الكائنات --");

        var ctor = typeof(Person).GetConstructor(Type.EmptyTypes)!;
        var setName = typeof(Person).GetProperty(nameof(Person.Name))!.GetSetMethod()!;
        var setAge = typeof(Person).GetProperty(nameof(Person.Age))!.GetSetMethod()!;

        var emit = Emit<Func<string, int, Person>>.NewDynamicMethod("MakePerson");
        var person = emit.DeclareLocal<Person>("p");
        emit.NewObject(ctor);
        emit.StoreLocal(person);

        emit.LoadLocal(person);
        emit.LoadArgument(0);
        emit.CallVirtual(setName);

        emit.LoadLocal(person);
        emit.LoadArgument(1);
        emit.CallVirtual(setAge);

        emit.LoadLocal(person);
        emit.Return();

        var del = emit.CreateDelegate();
        var p = del("أحمد", 30);
        Console.WriteLine($"الاسم: {p.Name}, العمر: {p.Age}");
    }

    static void TestErrorValidation()
    {
        Console.WriteLine("-- اختبار التحقق من الأخطاء --");

        var emit = Emit<Func<int>>.NewDynamicMethod("BadMethod");
        emit.Return();

        try
        {
            var del = emit.CreateDelegate();
            Console.WriteLine(del());
        }
        catch (Exception ex)
        {
            Console.WriteLine("تم التقاط الخطأ كما هو متوقع: " + ex.Message);
        }
    }

    static Emit<Action> BuildBranchDemo()
    {
        var e = Emit<Action>.NewDynamicMethod("Branches");
        var lbl = e.DefineLabel("LBL");

        for (var i = 0; i < 130; i++) e.Nop();
        e.Branch(lbl);
        for (var i = 0; i < 5; i++) e.Nop();
        e.MarkLabel(lbl);
        e.Return();

        return e;
    }

    static void TestOptimizations()
    {
        Console.WriteLine("-- اختبار تحسين الفروع --");

        var noOpt = BuildBranchDemo();
        noOpt.CreateDelegate(out string normal, OptimizationOptions.None);

        var opt = BuildBranchDemo();
        opt.CreateDelegate(out string optimized, OptimizationOptions.All);

        Console.WriteLine("بدون تحسين:");
        Console.WriteLine(normal);
        Console.WriteLine("مع تحسين:");
        Console.WriteLine(optimized);
    }

    static void TestDisassembly()
    {
        Console.WriteLine("-- عرض التعليمات --");

        var emit = Emit<Func<int, int, int>>.NewDynamicMethod("Add");
        emit.LoadArgument(0);
        emit.LoadArgument(1);
        emit.Add();
        emit.Return();

        Console.WriteLine(emit.Instructions());
    }
}
