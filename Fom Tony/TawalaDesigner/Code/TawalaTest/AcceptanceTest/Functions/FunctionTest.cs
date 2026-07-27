// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Functions
{
    /// <summary>
    /// Base class for other test classes in this theme.
    /// </summary>
    public class FunctionTest : FunctionTestBase
    {
        protected const string NEWLINE = "\r\n";

        protected string rtfPrefixString = @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + NEWLINE + @"{\f0\fswiss\fcharset0\fprq2 Arial;}" +
                                           NEWLINE + @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + NEWLINE +
                                           @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + NEWLINE +
                                           @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + NEWLINE +
                                           @"{\*\generator TX_RTF32 12.0.500.502;}" + NEWLINE;
    }
}