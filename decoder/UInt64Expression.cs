/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 29.09.2026
 * Time: 18:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Globalization;


namespace decoder {
    
    public static class UInt64Expression {
        public static ulong Evaluate ( string expression ) {
            Parser parser = new Parser(expression);
            ulong result = parser.ParseExpression();

            parser.SkipWhiteSpace();

            if ( !parser.End ) throw new FormatException(
                    "Nieoczekiwany znak na pozycji " + parser.Position);

            return result;
        }

        private class Parser {
            private readonly string _text;
            private int _position;

            public Parser(string text) {
                _text = text;
                _position = 0;
            }

            public int Position {
                get { return _position; }
            }

            public bool End {
                get { return _position >= _text.Length; }
            }

            public void SkipWhiteSpace () {
                while ( !End && char.IsWhiteSpace( _text [ _position ] ) ) _position++;
            }

            // + -
            public ulong ParseExpression () {
                return ParseBitwiseOr();
            }

            // |
            private ulong ParseBitwiseOr () {
                ulong value = ParseBitwiseXor();

                while ( true ) {
                    SkipWhiteSpace();

                    if ( Match( "|" ) ) value |= ParseBitwiseXor();
                    else break;
                }

                return value;
            }

            // ^
            private ulong ParseBitwiseXor () {
                ulong value = ParseBitwiseAnd();

                while ( true ) {
                    SkipWhiteSpace();

                    if ( Match( "^" ) ) value ^= ParseBitwiseAnd();
                    else break;
                }

                return value;
            }

            // &
            private ulong ParseBitwiseAnd () {
                ulong value = ParseShift();

                while ( true ) {
                    SkipWhiteSpace();

                    if ( Match( "&" ) ) value &= ParseShift();
                    else break;
                }

                return value;
            }

            // << >>
            private ulong ParseShift () {
                ulong value = ParseAddSubtract();

                while ( true ) {
                    SkipWhiteSpace();

                    if ( Match( "<<" ) ) {
                        ulong shift = ParseAddSubtract();

                        if ( shift > 63 ) throw new OverflowException(
                                "Przesunięcie musi być z zakresu 0..63.");

                        value <<= (int)shift;
                    } else if ( Match( ">>" ) ) {
                        ulong shift = ParseAddSubtract();

                        if ( shift > 63 ) throw new OverflowException(
                                "Przesunięcie musi być z zakresu 0..63.");

                        value >>= (int)shift;
                    } else {
                        break;
                    }
                }

                return value;
            }

            // + -
            private ulong ParseAddSubtract () {
                ulong value = ParseMultiply();

                while ( true ) {
                    SkipWhiteSpace();

                    if ( Match( "+" ) ) {
                        value += ParseMultiply();
                    } else if ( Match( "-" ) ) {
                        value -= ParseMultiply();
                    } else {
                        break;
                    }
                }

                return value;
            }

            // * / %
            private ulong ParseMultiply () {
                ulong value = ParseUnary();

                while ( true ) {
                    SkipWhiteSpace();

                    if ( Match( "*" ) ) {
                        value *= ParseUnary();
                    } else if ( Match( "/" ) ) {
                        ulong divisor = ParseUnary();

                        if ( divisor == 0 ) throw new DivideByZeroException();

                        value /= divisor;
                    } else if ( Match( "%" ) ) {
                        ulong divisor = ParseUnary();

                        if ( divisor == 0 ) throw new DivideByZeroException();

                        value %= divisor;
                    } else {
                        break;
                    }
                }

                return value;
            }

            // unary + -
            private ulong ParseUnary () {
                SkipWhiteSpace();

                if ( Match( "+" ) ) return ParseUnary();

                if ( Match( "-" ) ) return 0 - ParseUnary();

                return ParsePrimary();
            }

            // liczba / nawias
            private ulong ParsePrimary () {
                SkipWhiteSpace();

                if ( Match( "(" ) ) {
                    ulong value = ParseExpression();

                    SkipWhiteSpace();

                    if ( !Match( ")" ) ) throw new FormatException(
                            "Brak zamykającego nawiasu.");

                    return value;
                }

                return ParseNumber();
            }

            private ulong ParseNumber () {
                SkipWhiteSpace();

                int start = _position;

                // 0x123ABC
                if ( _position + 2 <= _text.Length &&
                 _text [ _position ] == '0' &&
                 ( _text [ _position + 1 ] == 'x' ||
                 _text [ _position + 1 ] == 'X' ) ) {
                    _position += 2;

                    int hexStart = _position;

                    while ( !End && IsHexDigit( _text [ _position ] ) ) _position++;

                    if ( hexStart == _position ) throw new FormatException(
                            "Nieprawidłowa liczba szesnastkowa.");

                    string hex = _text.Substring(
                                 hexStart,
                                 _position - hexStart );

                    return UInt64.Parse(
                        hex,
                        NumberStyles.AllowHexSpecifier,
                        CultureInfo.InvariantCulture );
                }

                // liczba dziesiętna
                while ( !End && char.IsDigit( _text [ _position ] ) ) _position++;

                if ( start == _position ) throw new FormatException(
                        "Oczekiwano liczby na pozycji " + _position);

                return UInt64.Parse(
                    _text.Substring( start, _position - start ),
                    CultureInfo.InvariantCulture );
            }

            private bool Match ( string value ) {
                SkipWhiteSpace();

                if ( _position + value.Length > _text.Length ) return false;

                for ( int i = 0; i < value.Length; i++ ) {
                    if ( _text [ _position + i ] != value [ i ] ) return false;
                }

                _position += value.Length;
                return true;
            }

            private static bool IsHexDigit ( char c ) {
                return ( c >= '0' && c <= '9' ) ||
                ( c >= 'a' && c <= 'f' ) ||
                ( c >= 'A' && c <= 'F' );
            }
        }
    }
    
    public static class IntExpression {
        public static int Evaluate ( string expression ) {
            Parser parser = new Parser(expression);
            int result = parser.ParseExpression();

            parser.SkipWhiteSpace();

            if ( !parser.End ) {
                throw new FormatException(
                    "Nieoczekiwany znak na pozycji " + parser.Position);
            }

            return result;
        }

        private class Parser {
            private readonly string _text;
            private int _position;

            public Parser(string text) {
                _text = text;
                _position = 0;
            }

            public int Position {
                get { return _position; }
            }

            public bool End {
                get { return _position >= _text.Length; }
            }

            public void SkipWhiteSpace () {
                while ( !End && char.IsWhiteSpace( _text [ _position ] ) ) {
                    _position++;
                }
            }

            // |
            public int ParseExpression () {
                return ParseBitwiseOr();
            }

            // |
            private int ParseBitwiseOr () {
                int value = ParseBitwiseXor();

                while ( true ) {
                    SkipWhiteSpace();

                    if ( Match( "|" ) ) {
                        value |= ParseBitwiseXor();
                    } else {
                        break;
                    }
                }

                return value;
            }

            // ^
            private int ParseBitwiseXor () {
                int value = ParseBitwiseAnd();

                while ( true ) {
                    SkipWhiteSpace();

                    if ( Match( "^" ) ) {
                        value ^= ParseBitwiseAnd();
                    } else {
                        break;
                    }
                }

                return value;
            }

            // &
            private int ParseBitwiseAnd () {
                int value = ParseShift();

                while ( true ) {
                    SkipWhiteSpace();

                    if ( Match( "&" ) ) {
                        value &= ParseShift();
                    } else {
                        break;
                    }
                }

                return value;
            }

            // << >>
            private int ParseShift () {
                int value = ParseAddSubtract();

                while ( true ) {
                    SkipWhiteSpace();

                    if ( Match( "<<" ) ) {
                        int shift = ParseAddSubtract();

                        if ( shift < 0 || shift > 31 ) {
                            throw new OverflowException(
                                "Przesunięcie musi być z zakresu 0..31.");
                        }

                        value <<= shift;
                    } else if ( Match( ">>" ) ) {
                        int shift = ParseAddSubtract();

                        if ( shift < 0 || shift > 31 ) {
                            throw new OverflowException(
                                "Przesunięcie musi być z zakresu 0..31.");
                        }

                        value >>= shift;
                    } else {
                        break;
                    }
                }

                return value;
            }

            // + -
            private int ParseAddSubtract () {
                int value = ParseMultiply();

                while ( true ) {
                    SkipWhiteSpace();

                    if ( Match( "+" ) ) {
                        value += ParseMultiply();
                    } else if ( Match( "-" ) ) {
                        value -= ParseMultiply();
                    } else {
                        break;
                    }
                }

                return value;
            }

            // * / %
            private int ParseMultiply () {
                int value = ParseUnary();

                while ( true ) {
                    SkipWhiteSpace();

                    if ( Match( "*" ) ) {
                        value *= ParseUnary();
                    } else if ( Match( "/" ) ) {
                        int divisor = ParseUnary();

                        if ( divisor == 0 ) {
                            throw new DivideByZeroException();
                        }

                        value /= divisor;
                    } else if ( Match( "%" ) ) {
                        int divisor = ParseUnary();

                        if ( divisor == 0 ) {
                            throw new DivideByZeroException();
                        }

                        value %= divisor;
                    } else {
                        break;
                    }
                }

                return value;
            }

            // unary + -
            private int ParseUnary () {
                SkipWhiteSpace();

                if ( Match( "+" ) ) {
                    return ParseUnary();
                }

                if ( Match( "-" ) ) {
                    return -ParseUnary();
                }

                return ParsePrimary();
            }

            // liczba / nawias
            private int ParsePrimary () {
                SkipWhiteSpace();

                if ( Match( "(" ) ) {
                    int value = ParseExpression();

                    SkipWhiteSpace();

                    if ( !Match( ")" ) ) {
                        throw new FormatException(
                            "Brak zamykającego nawiasu.");
                    }

                    return value;
                }

                return ParseNumber();
            }

            private int ParseNumber () {
                SkipWhiteSpace();

                int start = _position;

                // 0x123ABC
                if ( _position + 2 <= _text.Length &&
                 _text [ _position ] == '0' &&
                 ( _text [ _position + 1 ] == 'x' ||
                 _text [ _position + 1 ] == 'X' ) ) {
                    _position += 2;

                    int hexStart = _position;

                    while ( !End && IsHexDigit( _text [ _position ] ) ) {
                        _position++;
                    }

                    if ( hexStart == _position ) {
                        throw new FormatException(
                            "Nieprawidłowa liczba szesnastkowa.");
                    }

                    string hex = _text.Substring(
                                 hexStart,
                                 _position - hexStart );

                    return Int32.Parse(
                        hex,
                        NumberStyles.AllowHexSpecifier,
                        CultureInfo.InvariantCulture );
                }

                // liczba dziesiętna
                while ( !End && char.IsDigit( _text [ _position ] ) ) {
                    _position++;
                }

                if ( start == _position ) {
                    throw new FormatException(
                        "Oczekiwano liczby na pozycji " + _position);
                }

                return Int32.Parse(
                    _text.Substring( start, _position - start ),
                    CultureInfo.InvariantCulture );
            }

            private bool Match ( string value ) {
                SkipWhiteSpace();

                if ( _position + value.Length > _text.Length ) {
                    return false;
                }

                for ( int i = 0; i < value.Length; i++ ) {
                    if ( _text [ _position + i ] != value [ i ] ) {
                        return false;
                    }
                }

                _position += value.Length;
                return true;
            }

            private static bool IsHexDigit ( char c ) {
                return ( c >= '0' && c <= '9' ) ||
                ( c >= 'a' && c <= 'f' ) ||
                ( c >= 'A' && c <= 'F' );
            }
        }
    }
}
