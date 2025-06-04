﻿using Sigil.Impl;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace Sigil
{
    /// <summary>
    /// <para>A SigilVerificationException is thrown whenever a CIL stream becomes invalid.</para>
    /// <para>There are many possible causes of this including: operator type mismatches, underflowing the stack, and branching from one stack state to another.</para>
    /// <para>Invalid arguments, non-sensical parameters, and other non-correctness related errors will throw more specific exceptions.</para>
    /// <para>SigilVerificationException will typically include the state of the stack (or stacks) at the instruction in error.</para>
    /// </summary>
    [Serializable]
    public class SigilVerificationException : Exception, ISerializable
    {
        private string[] Instructions;
        private VerificationResult VerificationFailure;
        private ReturnTracerResult ReturnFailure;

        internal SigilVerificationException(string message, ReturnTracerResult failure, string[] instructions)
            : this(message, instructions)
        {
            ReturnFailure = failure;
        }

        internal SigilVerificationException(string method, VerificationResult failure, string[] instructions)
            : this(GetMessage(method, failure), instructions)
        {
            VerificationFailure = failure;
        }

        internal SigilVerificationException(string message, string[] instructions) : base(message)
        {
            Instructions = instructions;
        }

        private static string GetMessage(string method, VerificationResult failure)
        {
            if (failure.IsStackUnderflow)
            {
                if (failure.ExpectedStackSize == 1)
                {
                    return method + " expects a value on the stack, but it was empty";
                }

                return method + " expects " + failure.ExpectedStackSize + " values on the stack";
            }

            if (failure.IsTypeMismatch)
            {
                if (failure.ExpectedAtStackIndex != null)
                {
                    var expected = ErrorMessageString(failure.ExpectedAtStackIndex);
                    var found = ErrorMessageString(failure.Stack.ElementAt(failure.StackIndex));

                    return method + " expected " + (ExtensionMethods.StartsWithVowel(expected) ? "an " : "a ") + expected + "; found " + found;
                }

                var ex = ErrorMessageString(failure.ExpectedOnStack);
                var ac = ErrorMessageString(failure.ActuallyOnStack);

                return method + " expected " + (ExtensionMethods.StartsWithVowel(ex) ? "an " : "a ") + ex + "; found " + ac;
            }

            if (failure.IsStackMismatch)
            {
                return method + " resulted in stack mismatches";
            }

            if (failure.IsStackSizeFailure)
            {
                if (failure.ExpectedStackSize == 0)
                {
                    return method + " expected the stack of be empty";
                }

                if (failure.ExpectedStackSize == 1)
                {
                    return method + " expected the stack to have 1 value";
                }

                return method + " expected the stack to have " + failure.ExpectedStackSize + " values";
            }

            throw new Exception("Shouldn't be possible!");
        }

        private static string ErrorMessageString(LinqRoot<TypeOnStack> types)
        {
            var names = types.Select(t => t.ToString()).OrderBy(n => n).ToArray();

            if (names.Length == 1) return names[0];

            var ret = new StringBuilder();
            ret.Append(names[0]);

            for (var i = 1; i < names.Length - 1; i++)
            {
                ret.Append(", " + names[i]);
            }

            ret.Append(", or " + names[names.Length - 1]);

            return ret.ToString();
        }

        /// <summary>
        /// <para>Returns a string representation of any stacks attached to this exception.</para>
        /// <para>This is meant for debugging purposes, and should not be called during normal operation.</para>
        /// </summary>
        public string GetDebugInfo()
        {
            var ret = new StringBuilder();

            if (VerificationFailure != null)
            {

                if (VerificationFailure.IsStackMismatch)
                {
                    ret.AppendLine("Expected Stack");
                    ret.AppendLine("==============");
                    PrintStack(VerificationFailure.ExpectedStack, ret);

                    ret.AppendLine();
                    ret.AppendLine("Incoming Stack");
                    ret.AppendLine("==============");
                    PrintStack(VerificationFailure.IncomingStack, ret);
                }

                if (VerificationFailure.IsTypeMismatch && VerificationFailure.Stack != null)
                {
                    ret.AppendLine("Stack");
                    ret.AppendLine("=====");
                    PrintStack(VerificationFailure.Stack, ret, "// bad value", VerificationFailure.StackIndex);
                }

                if ((VerificationFailure.IsStackUnderflow || VerificationFailure.IsStackSizeFailure) && VerificationFailure.Stack != null)
                {
                    ret.AppendLine("Stack");
                    ret.AppendLine("=====");
                    PrintStack(VerificationFailure.Stack, ret);
                }

                ret.AppendLine();
            }

            if (ReturnFailure != null)
            {
                foreach (var path in ReturnFailure.FailingPaths.AsEnumerable())
                {
                    ret.AppendLine("Bad Path");
                    ret.AppendLine("========");
                    foreach (var label in path.AsEnumerable())
                    {
                        ret.AppendLine(label.Name);
                    }

                    ret.AppendLine();
                }
            }

            ret.AppendLine("Instructions");
            ret.AppendLine("============");

            var instrIx = VerificationFailure != null && VerificationFailure.TransitionIndex != null ? VerificationFailure.Verifier.GetInstructionIndex(VerificationFailure.TransitionIndex.Value) : -1;

            for (var i = 2; i < Instructions.Length; i++)
            {
                var line = Instructions[i];

                if (i == instrIx) line = line + "  // relevant instruction";

                if (!string.IsNullOrEmpty(line))
                {
                    ret.AppendLine(line);
                }
            }

            return ret.ToString();
        }

        private static void PrintStack(LinqStack<LinqList<TypeOnStack>> stack, StringBuilder sb, string mark = null, int? markAt = null)
        {
            if (stack.Count == 0)
            {
                sb.AppendLine("--empty--");
                return;
            }

            for (var i = 0; i < stack.Count; i++)
            {
                var asStr =
                    string.Join(", or",
                        stack.ElementAt(i).Select(s => s.ToString()).ToArray()
                    );

                if (i == markAt)
                {
                    asStr += "  " + mark;
                }

                sb.AppendLine(asStr);
            }
        }

        /// <summary>
        /// Implementation for ISerializable.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [Obsolete("Formatter-based serialization is obsolete", DiagnosticId = "SYSLIB0051")]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Returns the message and stacks on this exception, in string form.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return
                Message + Environment.NewLine + Environment.NewLine + GetDebugInfo();
        }
    }
}
