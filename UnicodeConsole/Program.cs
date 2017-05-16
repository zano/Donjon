using System;
using System.IO;
using System.Globalization;
using System.Text;

public static class DisplayChars
{
    private static void Main(string[] args)
    {
        uint rangeStart = 0;
        uint rangeEnd = 0;
        bool setOutputEncodingToUnicode = true;
        // Get the current encoding so we can restore it.
        Encoding originalOutputEncoding = Console.OutputEncoding;

        try
        {
            switch (args.Length)
            {
                case 2:
                    rangeStart = uint.Parse(args[0], NumberStyles.HexNumber);
                    rangeEnd = uint.Parse(args[1], NumberStyles.HexNumber);
                    setOutputEncodingToUnicode = true;
                    break;
                case 3:
                    if (!uint.TryParse(args[0], NumberStyles.HexNumber, null, out rangeStart))
                        throw new ArgumentException(String.Format("{0} is not a valid hexadecimal number.", args[0]));

                    if (!uint.TryParse(args[1], NumberStyles.HexNumber, null, out rangeEnd))
                        throw new ArgumentException(String.Format("{0} is not a valid hexadecimal number.", args[1]));

                    bool.TryParse(args[2], out setOutputEncodingToUnicode);
                    break;
                default:
                    rangeStart = 0x400;
                    rangeEnd = 0x4FF;
                    setOutputEncodingToUnicode = true;
                    //Console.WriteLine("Usage: {0} <{1}> <{2}> [{3}]",
                    //                  Environment.GetCommandLineArgs()[0],
                    //                  "startingCodePointInHex",
                    //                  "endingCodePointInHex",
                    //                  "<setOutputEncodingToUnicode?{true|false, default:false}>");
                    //return;
                    break;
            }

            if (setOutputEncodingToUnicode)
            {
                // This won't work before .NET Framework 4.5.
                try
                {
                    // Set encoding using endianness of this system.
                    // We're interested in displaying individual Char objects, so 
                    // we don't want a Unicode BOM or exceptions to be thrown on
                    // invalid Char values.
                    Console.OutputEncoding = new UnicodeEncoding(!BitConverter.IsLittleEndian, false);
                    Console.WriteLine("\nOutput encoding set to UTF-16");
                }
                catch (IOException)
                {
                    Console.OutputEncoding = new UTF8Encoding();
                    Console.WriteLine("Output encoding set to UTF-8");
                }
            }
            else
            {
                Console.WriteLine("The console encoding is {0} (code page {1})",
                                  Console.OutputEncoding.EncodingName,
                                  Console.OutputEncoding.CodePage);
            }
            DisplayRange(rangeStart, rangeEnd);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            // Restore console environment.
            Console.OutputEncoding = originalOutputEncoding;
        }
    }

    public static void DisplayRange(uint start, uint end)
    {
        const uint upperRange = 0x10FFFF;
        const uint surrogateStart = 0xD800;
        const uint surrogateEnd = 0xDFFF;

        if (end <= start)
        {
            uint t = start;
            start = end;
            end = t;
        }

        // Check whether the start or end range is outside of last plane.
        if (start > upperRange)
            throw new ArgumentException(String.Format("0x{0:X5} is outside the upper range of Unicode code points (0x{1:X5})",
                                                      start, upperRange));
        if (end > upperRange)
            throw new ArgumentException(String.Format("0x{0:X5} is outside the upper range of Unicode code points (0x{0:X5})",
                                                      end, upperRange));

        // Since we're using 21-bit code points, we can't use U+D800 to U+DFFF.
        if ((start < surrogateStart & end > surrogateStart) || (start >= surrogateStart & start <= surrogateEnd))
            throw new ArgumentException(String.Format("0x{0:X5}-0x{1:X5} includes the surrogate pair range 0x{2:X5}-0x{3:X5}",
                                                      start, end, surrogateStart, surrogateEnd));
        uint last = RoundUpToMultipleOf(0x10, end);
        uint first = RoundDownToMultipleOf(0x10, start);

        uint rows = (last - first) / 0x10;

        for (uint r = 0; r < rows; ++r)
        {
            // Display the row header.
            Console.Write("{0:x5} ", first + 0x10 * r);

            for (uint c = 0; c < 0x10; ++c)
            {
                uint cur = (first + 0x10 * r + c);
                if (cur < start)
                {
                    Console.Write(" {0} ", Convert.ToChar(0x20));
                }
                else if (end < cur)
                {
                    Console.Write(" {0} ", Convert.ToChar(0x20));
                }
                else
                {
                    // the cast to int is safe, since we know that val <= upperRange.
                    String chars = Char.ConvertFromUtf32((int)cur);
                    // Display a space for code points that are not valid characters.
                    if (CharUnicodeInfo.GetUnicodeCategory(chars[0]) ==
                                                    UnicodeCategory.OtherNotAssigned)
                        Console.Write(" {0} ", Convert.ToChar(0x20));
                    // Display a space for code points in the private use area.
                    else if (CharUnicodeInfo.GetUnicodeCategory(chars[0]) ==
                                                   UnicodeCategory.PrivateUse)
                        Console.Write(" {0} ", Convert.ToChar(0x20));
                    // Is surrogate pair a valid character?
                    // Note that the console will interpret the high and low surrogate
                    // as separate (and unrecognizable) characters.
                    else if (chars.Length > 1 && CharUnicodeInfo.GetUnicodeCategory(chars, 0) ==
                                                 UnicodeCategory.OtherNotAssigned)
                        Console.Write(" {0} ", Convert.ToChar(0x20));
                    else
                        Console.Write(" {0} ", chars);
                }

                switch (c)
                {
                    case 3:
                    case 11:
                        Console.Write("-");
                        break;
                    case 7:
                        Console.Write("--");
                        break;
                }
            }

            Console.WriteLine();
            if (0 < r && r % 0x10 == 0)
                Console.WriteLine();
        }
    }

    private static uint RoundUpToMultipleOf(uint b, uint u)
    {
        return RoundDownToMultipleOf(b, u) + b;
    }

    private static uint RoundDownToMultipleOf(uint b, uint u)
    {
        return u - (u % b);
    }
}
// If the example is run with the command line
//       DisplayChars 0400 04FF true
// the example displays the Cyrillic character set as follows:
//       Output encoding set to UTF-16
//       00400  Ð€  Ð  Ð‚  Ðƒ - Ð„  Ð…  Ð†  Ð‡ -- Ðˆ  Ð‰  ÐŠ  Ð‹ - ÐŒ  Ð  ÐŽ  Ð
//       00410  Ð  Ð‘  Ð’  Ð“ - Ð”  Ð•  Ð–  Ð— -- Ð˜  Ð™  Ðš  Ð› - Ðœ  Ð  Ðž  ÐŸ
//       00420  Ð   Ð¡  Ð¢  Ð£ - Ð¤  Ð¥  Ð¦  Ð§ -- Ð¨  Ð©  Ðª  Ð« - Ð¬  Ð­  Ð®  Ð¯
//       00430  Ð°  Ð±  Ð²  Ð³ - Ð´  Ðµ  Ð¶  Ð· -- Ð¸  Ð¹  Ðº  Ð» - Ð¼  Ð½  Ð¾  Ð¿
//       00440  Ñ€  Ñ  Ñ‚  Ñƒ - Ñ„  Ñ…  Ñ†  Ñ‡ -- Ñˆ  Ñ‰  ÑŠ  Ñ‹ - ÑŒ  Ñ  ÑŽ  Ñ
//       00450  Ñ  Ñ‘  Ñ’  Ñ“ - Ñ”  Ñ•  Ñ–  Ñ— -- Ñ˜  Ñ™  Ñš  Ñ› - Ñœ  Ñ  Ñž  ÑŸ
//       00460  Ñ   Ñ¡  Ñ¢  Ñ£ - Ñ¤  Ñ¥  Ñ¦  Ñ§ -- Ñ¨  Ñ©  Ñª  Ñ« - Ñ¬  Ñ­  Ñ®  Ñ¯
//       00470  Ñ°  Ñ±  Ñ²  Ñ³ - Ñ´  Ñµ  Ñ¶  Ñ· -- Ñ¸  Ñ¹  Ñº  Ñ» - Ñ¼  Ñ½  Ñ¾  Ñ¿
//       00480  Ò€  Ò  Ò‚  Òƒ - Ò„  Ò…  Ò†  Ò‡ -- Òˆ  Ò‰  ÒŠ  Ò‹ - ÒŒ  Ò  ÒŽ  Ò
//       00490  Ò  Ò‘  Ò’  Ò“ - Ò”  Ò•  Ò–  Ò— -- Ò˜  Ò™  Òš  Ò› - Òœ  Ò  Òž  ÒŸ
//       004a0  Ò   Ò¡  Ò¢  Ò£ - Ò¤  Ò¥  Ò¦  Ò§ -- Ò¨  Ò©  Òª  Ò« - Ò¬  Ò­  Ò®  Ò¯
//       004b0  Ò°  Ò±  Ò²  Ò³ - Ò´  Òµ  Ò¶  Ò· -- Ò¸  Ò¹  Òº  Ò» - Ò¼  Ò½  Ò¾  Ò¿
//       004c0  Ó€  Ó  Ó‚  Óƒ - Ó„  Ó…  Ó†  Ó‡ -- Óˆ  Ó‰  ÓŠ  Ó‹ - ÓŒ  Ó  ÓŽ  Ó
//       004d0  Ó  Ó‘  Ó’  Ó“ - Ó”  Ó•  Ó–  Ó— -- Ó˜  Ó™  Óš  Ó› - Óœ  Ó  Óž  ÓŸ
//       004e0  Ó   Ó¡  Ó¢  Ó£ - Ó¤  Ó¥  Ó¦  Ó§ -- Ó¨  Ó©  Óª  Ó« - Ó¬  Ó­  Ó®  Ó¯
//       004f0  Ó°  Ó±  Ó²  Ó³ - Ó´  Óµ  Ó¶  Ó· -- Ó¸  Ó¹  Óº  Ó» - Ó¼  Ó½  Ó¾  Ó¿
