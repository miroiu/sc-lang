﻿using System;
using System.Collections.Generic;

namespace StringMath.Tests
{
    static class Extensions
    {
        public static List<Token> ReadAllTokens(this string input)
        {
            ITokenizer tokenizer = new Tokenizer(input);
            List<Token> tokens = new();

            Token t;
            do
            {
                t = tokenizer.ReadToken();
                tokens.Add(t);
            }
            while (t.Type != TokenType.EndOfCode);

            return tokens;
        }

        public static IReadOnlyCollection<string> GetVariables<TNum>(this string input, IMathContext<TNum> context) where TNum : INumber<TNum>
        {
            ITokenizer tokenzier = new Tokenizer(input);
            IParser parser = new Parser(tokenzier, context);

            // Populate the variables collection
            parser.Parse();

            return parser.Variables;
        }
    }
}