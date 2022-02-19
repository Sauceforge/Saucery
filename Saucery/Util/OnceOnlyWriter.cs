﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Saucery.Util
{
    internal class OnceOnlyWriter {
        private static readonly HashSet<string> WrittenMessages = new HashSet<string>();

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void WriteLine(string message) {
            if (!WrittenMessages.Contains(message)) {
                Console.WriteLine(message);
                Console.Out.Flush();
                WrittenMessages.Add(message);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void WriteLine(string messageWithParams, object o1) {
            WriteLine(string.Format(messageWithParams, o1));
        }

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void WriteLine(string messageWithParams, object o1, object o2) {
        //    WriteLine(string.Format(messageWithParams, o1, o2));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void WriteLine(bool value) {
        //    WriteLine(value ? "true" : "false");
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void WriteLine(char value) {
        //    WriteLine(value.ToString());
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void WriteLine(char[] buffer) {
        //    WriteLine(buffer.ToString());
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void WriteLine(decimal value) {
        //    WriteLine(value.ToString(CultureInfo.InvariantCulture));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void WriteLine(double value) {
        //    WriteLine(value.ToString(CultureInfo.InvariantCulture));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void WriteLine(float value) {
        //    WriteLine(value.ToString(CultureInfo.InvariantCulture));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void WriteLine(int value) {
        //    WriteLine(value.ToString(CultureInfo.InvariantCulture));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void WriteLine(uint value) {
        //    WriteLine(value.ToString(CultureInfo.InvariantCulture));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void WriteLine(long value) {
        //    WriteLine(value.ToString(CultureInfo.InvariantCulture));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void WriteLine(ulong value) {
        //    WriteLine(value.ToString(CultureInfo.InvariantCulture));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void WriteLine(object value) {
        //    WriteLine(value.ToString());
        //}


        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void WriteLine(string format, object arg0, object arg1, object arg2) {
        //    WriteLine(string.Format(format, arg0, arg1, arg2));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void WriteLine(string format, params object[] arg) {
        //    WriteLine(arg == null ? string.Format(format, null, null) : string.Format(format, arg));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void Write(string format, object arg0) {
        //    Write(string.Format(format, arg0));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void Write(string format, object arg0, object arg1) {
        //    Write(string.Format(format, arg0, arg1));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void Write(string format, object arg0, object arg1, object arg2) {
        //    Write(string.Format(format, arg0, arg1, arg2));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void Write(string format, params object[] arg) {
        //    Write(arg == null ? string.Format(format, null, null) : string.Format(format, arg));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void Write(bool value) {
        //    Write(value ? "true" : "false");
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void Write(char value) {
        //    Write(value.ToString());
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void Write(char[] buffer) {
        //    Write(buffer.ToString());
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void Write(double value) {
        //    Write(value.ToString(CultureInfo.InvariantCulture));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void Write(decimal value) {
        //    Write(value.ToString(CultureInfo.InvariantCulture));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void Write(float value) {
        //    Write(value.ToString(CultureInfo.InvariantCulture));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void Write(int value) {
        //    Write(value.ToString(CultureInfo.InvariantCulture));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void Write(uint value) {
        //    Write(value.ToString(CultureInfo.InvariantCulture));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void Write(long value) {
        //    Write(value.ToString(CultureInfo.InvariantCulture));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void Write(ulong value) {
        //    Write(value.ToString(CultureInfo.InvariantCulture));
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static void Write(object value) {
        //    Write(value.ToString());
        //}
    }
}

/*
 * Author: Andrew Gray, SauceForge
 * Date: 17th December 2015
 * 
 */