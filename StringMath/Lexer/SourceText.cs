﻿using System.Collections;
using System.Collections.Generic;

namespace StringMath
{
    internal sealed class SourceText : IEnumerator<char>
    {
        public string Text { get; }
        public int Position { get; private set; }
        public char Current => Text[Position];
        object IEnumerator.Current => Current;

        // The string terminator is used by the lexer to produce EndOfCode tokens
        public SourceText(string source)
            => Text = $"{source}\0";

        public bool MoveNext()
        {
            if (Position + 1 < Text.Length)
            {
                Position++;
                return true;
            }

            return false;
        }

        public char Peek(int count = 1)
        {
            int location = Position + count;

            char result = location < Text.Length && location >= 0 ? Text[location] : '\0';
            return result;
        }

        public void Reset()
        {
            Position = 0;
        }

        public void Dispose()
        {
            Reset();
        }
    }
}
