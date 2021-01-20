using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanonTxtCvt
{
    public static class Characters
    {
        static Dictionary<byte, string> Mode13Map = new Dictionary<byte, string>()
        {
            { 0x7F, "░" },

            { 0x80, "á" },
            { 0x81, "à" },
            { 0x82, "ä" },
            { 0x83, "â" },
            { 0x84, "ï" },
            { 0x85, "ì" },
            { 0x86, "í" },
            { 0x87, "î" },
            { 0x88, "ú" },
            { 0x89, "ù" },
            { 0x8A, "ü" },
            { 0x8B, "û" },
            { 0x8C, "é" },
            { 0x8D, "è" },
            { 0x8E, "ë" },
            { 0x8F, "ê" },

            { 0x90, "ó" },
            { 0x91, "ò" },
            { 0x92, "ö" },
            { 0x93, "ô" },
            { 0x94, "å" },
            { 0x95, "Å" },
            { 0x96, "Ä" },
            { 0x97, "Ü" },
            { 0x98, "É" },
            { 0x99, "Ö" },
            { 0x9A, "ñ" },
            { 0x9B, "Ñ" },
            { 0x9C, "ç" },
            { 0x9D, "Ç" },
            { 0x9E, "æ" },
            { 0x9F, "Æ" },

            { 0xA0, "ø" },
            { 0xA1, "Ø" },
            { 0xA2, "§" },
            { 0xA3, "¶" },
            { 0xA4, "¿" },
            { 0xA5, "¡" },
            { 0xA6, "£" },
            { 0xA7, "¢" },
            { 0xA8, "¥" },
            { 0xA9, "\x20A7" }, // Peseta
            { 0xAA, "ŉ" },
            { 0xAB, "?" },      // unknown loop thingie
            { 0xAC, "ß" },
            { 0xAD, "\x0192" }, // florin
            { 0xAE, "Bs" },     // Bs = Bolivar
            { 0xAF, "B/." },    // B/. = Panamanian Balboa

            { 0xB0, "½" },
            { 0xB1, "¼" },
            { 0xB2, "¾"}, 
            { 0xB3, "⅛"}, 
            { 0xB4, "⅜"}, 
            { 0xB5, "⅝"}, 
            { 0xB6, "⅞"},
            { 0xB7, "²" }, 
            { 0xB8, "³" },
            { 0xB9, "°" },
            { 0xBA, "±" },
            { 0xBB, "ª" },
            { 0xBC, "º" },
            { 0xBD, "÷" },
            { 0xBF, "\x2017" }, // double "low line"


            { 0xC0, "\xAD" },  // soft hyphen
            { 0xC1, "\xA0" },  // non-breaking space
            { 0xC2, "ŀ" },
            { 0xC3, "Ŀ" },
            { 0xC4, "ı" },
            { 0xC5, "·" },
            { 0xC6, "˜"},
            { 0xC7, "´"},
            { 0xC8, "`"},
            { 0xC9, "¨" },
            { 0xCA, "ˆ"},

            { 0xD0, "¯"},
            { 0xD1, "˘"},
            { 0xD2, "˙"},
            { 0xD3, "˚"},
            { 0xD4, "˝"},
            { 0xD5, "ˇ"},
            { 0xD6, "¸"}, 
            { 0xD7, "˛"},

            { 0xE0, "þ" },
            { 0xE1, "Þ" },
            { 0xE2, "ð" },
            { 0xE3, "Ð" },
            { 0xE4, "ł" },
            { 0xE5, "Ł" },
            { 0xE6, "\x0133" }, // ij ligature
            { 0xE7, "\x0132" }, // IJ ligature
            { 0xE8, "ď" },
            { 0xE9, "Đ" },
            { 0xEA, "ľ" },
            { 0xEB, "Ľ" },
            { 0xEC, "ť" },
            { 0xED, "Ť" },
            { 0xEE, "₣" },
            { 0xEF, "µ" },

            { 0xF0, "œ" },
            { 0xF1, "Œ" },


        };

        //
        // Special characters:  2 text bytes are combined into ushort for lookup here
        //
        static Dictionary<ushort, char> SpecialChar = new Dictionary<ushort, char>
        {


            { 0xC661, 'ã' },
            { 0xC66F, 'õ' },

            { 0xC763, 'ć' },
            { 0xC76E, 'ń' },
            { 0xC772, 'ŕ' },
            { 0xC773, 'ś' },
            { 0xC779, 'ý' },
            { 0xC77A, 'ź' },

            { 0xCA73, 'ŝ' },

            { 0xCB41, 'Ã' },
            { 0xCB4F, 'Õ' },

            { 0xCC41, 'Á' },
            { 0xCC43, 'Ć' },
            { 0xCC49, 'Í' },
            { 0xCC4E, 'Ń' },
            { 0xCC4F, 'Ó' },
            { 0xCC52, 'Ŕ' },
            { 0xCC53, 'Ś' },
            { 0xCC55, 'Ú' },
            { 0xCC59, 'Ý' },
            { 0xCC5A, 'Ź' },

            { 0xCD41, 'À' },
            { 0xCD45, 'È' },
            { 0xCD49, 'Ì' },
            { 0xCD4F, 'Ò' },
            { 0xCD55, 'Ù' },

            { 0xCE45, 'Ë' },
            { 0xCE49, 'Ï' },

            { 0xCF41, 'Â' },
            { 0xCF45, 'Ê' },
            { 0xCF49, 'Î' },
            { 0xCF4F, 'Ô' },
            { 0xCF53, 'Ŝ' },
            { 0xCF55, 'Û' },

            { 0xD061, 'ā' },
            { 0xD065, 'ē' },
            { 0xD06F, 'ō' },
            { 0xD075, 'ū' },
            { 0xD0C4, 'ī' },

            { 0xD161, 'ă' },
            { 0xD165, 'ĕ' },
            { 0xD167, 'ğ' },
            { 0xD16E, 'ň' },    // n w/breve - substituted caron for now...
            { 0xD16F, 'ŏ' },
            { 0xD175, 'ŭ' },

            { 0xD27A, 'ż' },

            { 0xD375, 'ů' },

            { 0xD46F, 'ő' },
            { 0xD475, 'ű' },

            { 0xD563, 'č' },
            { 0xD565, 'ě' },
            { 0xD567, 'ǧ' },
            { 0xD56E, 'ň' },
            { 0xD572, 'ř' },
            { 0xD573, 'š' },
            { 0xD57A, 'ž' },

            { 0xD673, 'ş' },
            { 0xD674, 'ţ' },

            { 0xD761, 'ą' },
            { 0xD765, 'ę' },

            { 0xD841, 'Ā' },
            { 0xD845, 'Ē' },
            { 0xD849, 'Ī' },
            { 0xD84F, 'Ō' },
            { 0xD855, 'Ū' },

            { 0xD941, 'Ă' },
            { 0xD945, 'Ĕ' },
            { 0xD947, 'Ğ' },
            { 0xD94E, 'Ň' },    // N w/breve - substituted caron for now...
            { 0xD94F, 'Ŏ' },
            { 0xD955, 'Ŭ' },

            { 0xDA49, 'İ' },
            { 0xDA5A, 'Ż' },

            { 0xDB55, 'Ů' },

            { 0xDC4F, 'Ő' },
            { 0xDC55, 'Ű' },

            { 0xDD43, 'Č' },
            { 0xDD45, 'Ě' },
            { 0xDD47, 'Ǧ' },
            { 0xDD4E, 'Ň' },
            { 0xDD52, 'Ř' },
            { 0xDD53, 'Š' },
            { 0xDD5A, 'Ž' },

            { 0xDE53, 'Ş' },
            { 0xDE54, 'Ţ' },

            { 0xDF41, 'Ą' },
            { 0xDF45, 'Ę' },



        };


        static Dictionary<byte, char> Mode14Map = new Dictionary<byte, char>()
        {
            // Symbols - Page 1 - Greek
            { 0x41, 'Α' },      { 0x4B, 'Λ' },      { 0x55, 'Φ' },      { 0x67, 'η' },      { 0x71, 'ρ' },
            { 0x42, 'Β' },      { 0x4C, 'Μ' },      { 0x56, 'Χ' },      { 0x68, 'θ' },      { 0x72, 'σ' },
            { 0x43, 'Γ' },      { 0x4D, 'Ν' },      { 0x57, 'Ψ' },      { 0x69, 'ι' },      { 0x73, 'τ' },
            { 0x44, 'Δ' },      { 0x4E, 'Ξ' },      { 0x58, 'Ω' },      { 0x6A, 'κ' },      { 0x74, 'υ' },
            { 0x45, 'Ε' },      { 0x4F, 'Ο' },      { 0x61, 'α' },      { 0x6B, 'λ' },      { 0x75, 'φ' },
            { 0x46, 'Ζ' },      { 0x50, 'Π' },      { 0x62, 'β' },      { 0x6C, 'μ' },      { 0x76, 'χ' },
            { 0x47, 'Η' },      { 0x51, 'Ρ' },      { 0x63, 'γ' },      { 0x6D, 'ν' },      { 0x77, 'ψ' },
            { 0x48, 'Θ' },      { 0x52, 'Σ' },      { 0x64, 'δ' },      { 0x6E, 'ξ' },      { 0x78, 'ω' },
            { 0x49, 'Ι' },      { 0x53, 'Τ' },      { 0x65, 'ε' },      { 0x6F, 'ο' },
            { 0x4A, 'Κ' },      { 0x54, 'Υ' },      { 0x66, 'ζ' },      { 0x70, 'π' },

            // Symbols - Page 1 - Greek etc.
            { 0x3A, 'e' },  // ?
            { 0x3B, 'ε' },  // epsilon?
            { 0x59, '∇' }, // nabla
            { 0x5A, '∂' },  // partial differential?
            { 0x5B, 'ς' },  // final sigma
            { 0x79, 'ϑ' },  // theta
            { 0x7A, 'ϕ' },  // phi
            { 0x7B, 'ω' },  // omega?

            // Symbols - Page 2 - Math
            { 0x2B, '+' },      { 0x3C, '<' },      { 0x7E, '≢' },      { 0xB7, '∈' },      { 0xC5, '∧' },      { 0x40, '∴' },
            { 0x2D, '-' },      { 0x3E, '>' },                          { 0xB8, '∋' },      { 0xC6, '∨' },      { 0x60, '∵' },
            { 0x2A, '×' },      { 0x5C, '≤' },      { 0x3F, '≈' },      { 0xB9, '∉' },       { 0xB1, '∀' },      { 0x2E, '․' },
            { 0x25, '÷' },      { 0x5E, '≥' },      { 0x7C, '≃' },      { 0xBA, '⊂' },      { 0xB2, '∃' },      { 0x23, '⋅' },
            { 0x3D, '=' },                          { 0x24, '∞' },      { 0xBB, '⊃' },      { 0xB4, '⊥' },      { 0xCB, '·' },
            { 0x2C, ',' },                          { 0x26, '∝' },     { 0xBC, '⊄' },                            
            { 0x28, '(' },      { 0x5D, '≠' },                          { 0xBD, '⊅' },       { 0xD5, '∫' },       
            { 0x29, ')' },                          { 0xB5, '∪' },     { 0xBE, '⊆' },      { 0xD6, '∮' },
            { 0x27, '\'' },                         { 0xB6, '∩' },      { 0xBF, '⊇' },      { 0xD7, '∠' },
            { 0x22, '"' },      { 0x7D, '≡' },      { 0x2F, '∕' },      { 0xC8, '¬' },       { 0x21, '√' },

            { 0x30, '⁰' },  // 0 super
            { 0x31, '¹' },  // 1 super
            { 0x32, '²' },  // 2 super
            { 0x33, '³' },  // 3 super
            { 0x34, '⁴' },  // 4 super
            { 0x35, '⁵' },  // 5 super
            { 0x36, '⁶' },  // 6 super
            { 0x37, '⁷' },  // 7 super
            { 0x38, '⁸' },  // 8 super
            { 0x39, '⁹' },  // 9 super

            { 0xA1, '↑' },
            { 0xA2, '→' },
            { 0xA3, '↓' },
            { 0xA4, '←' },
            { 0xA9, '↕' },
            { 0xAA, '↔' },

            { 0x5F, '─' },  // box - bottom line
            { 0xB0, '─' },  // box - top line
            { 0xE0, '┌' },  // box - left top
            { 0xE1, '└' },  // box - left bottom
            { 0xF0, '┐' },  // box - right top
            { 0xF1, '┘' },  // box - right bottom
            { 0xF6, '│' },  // box - vertical line
            { 0xE9, '║' },  // box - double vertical

            { 0xE2, '\x23A7' }, // brace - left top
            { 0xE3, '\x23A8' }, // brace - left middle
            { 0xE4, '\x23A9' }, // brace - left bottom
            { 0xF2, '\x23AB' }, // brace - right top
            { 0xF3, '\x23AC' }, // brace - right middle
            { 0xF4, '\x23AD' }, // brace - right bottom

            { 0xE5, '\x2320' }, // integral - top
            { 0xE6, '\x23AE' }, // integral - contour extension (regular, unicode doesn't have contour extension)
            { 0xF5, '\x23AE' }, // integral - plain extension
            { 0xE7, '\x2321' }, // integral - bottom

            { 0xF7, '√' },

    };

        static Dictionary<byte, string> Mode15Map = new Dictionary<byte, string>()
        {
            { 0x21, "\x2160" },
            { 0x22, "\x2161" },
            { 0x23, "\x2162" },
            { 0x24, "\x2163" },
            { 0x25, "\x2164" },
            { 0x26, "\x2165" },
            { 0x27, "\x2166" },
            { 0x28, "\x2167" },
            { 0x29, "\x2168" },
            { 0x2A, "\x2169" },
            { 0x2B, "\x216A" },
            { 0x2C, "\x216B" },
            { 0x2D, "(\x2169\x2162)" },  // no Unicode
            { 0x2E, "(\x2169\x2163)" },  // no Unicode
            { 0x2F, "(\x2169\x2164)" },  // no Unicode

            { 0x30, "‼" },
            { 0x31, "†" },
            
            { 0x32, "♂" },
            { 0x33, "♀" },

            { 0x34, "♡" },      // white suits
            { 0x35, "♢" },
            { 0x36, "\x2667" },
            { 0x37, "\x2664" },

            { 0x38, "○" },
            { 0x39, "□" },
            { 0x3A, "\x25B3" },
            { 0x3B, "\x2606" },

            { 0x3C, "\x25B3" },
            { 0x3D, "\x25B7" },
            { 0x3E, "\x25BD" },
            { 0x3F, "\x25C1" },

            { 0x40, "st " },    // ?
            { 0x41, "nd " },    // ?
            { 0x42, "rd " },    // ?
            { 0x43, "th " },    // ?
            { 0x44, "©" },
            { 0x45, "®" },
            { 0x46, "™" },
            { 0x47, "ϗ" },
            { 0x48, "ⁿ" },

            { 0x49, "∾" },  // Canon has "lazy s" - this is Unicode's "Inverted Lazy S"
            { 0x4A, "∼" },
            { 0x4B, "≒" },
            { 0x4C, "≠" },  // Unicode doesn't seem to have not-equal w/backslash
            { 0x4D, "«" },
            { 0x4E, "»" },
            { 0x4F, "∑" },

            { 0x50, "↗" },
            { 0x51, "↘" },
            { 0x52, "↙" },
            { 0x53, "↖" },            

            { 0x54, "\x21E7" },     // Few heavy arrows in BMP... so "white" arrows instead
            { 0x55, "\x21E8" },
            { 0x56, "\x21E9" },
            { 0x57, "\x21E6" },

            { 0x60, "\x27EE" }, // left tortoise shell bracket
            { 0x61, "\x27EF" }, // right tortoise shell bracket
            { 0x62, "{" },      // left curly brace
            { 0x63, "}" },      // right curly brace

            { 0x70, "♈" },
            { 0x71, "♉" },
            { 0x72, "♊" },
            { 0x73, "♋" },
            { 0x74, "♌" },
            { 0x75, "♍" },
            { 0x76, "♎" },
            { 0x77, "♏" },
            { 0x78, "♐" },
            { 0x79, "♑" },
            { 0x7A, "♒" },
            { 0x7B, "\x2653" },
            { 0x7C, "'" },
            { 0x7D, "\"" },

            { 0xA1, "①" },
            { 0xA2, "②" },
            { 0xA3, "③" },
            { 0xA4, "④" },
            { 0xA5, "⑤" },
            { 0xA6, "⑥" },
            { 0xA7, "⑦" },
            { 0xA8, "⑧" },
            { 0xA9, "⑨" },
            { 0xAA, "⑩" },

            { 0xD5, "⇒" },
            { 0xD7, "⇐" },
            { 0xD4, "⇑" },
            { 0xD6, "⇓" },
            { 0xD3, "⇔" },
            { 0xD2, "⇕" },
            { 0xDB, "✓" },
            { 0xDA, "✔" },
            { 0xDC, "☑" },
            { 0xE7, "☐" },

            { 0xBF, "“" },
            { 0xBE, "”" },
            { 0xB8, "●" },
            { 0xB9, "◼" },
            { 0xBB, "★" },
            { 0xBD, "▶" },
            { 0xB4, "♥" },
            { 0xB5, "♦" },
            { 0xB6, "♣" },
            { 0xB7, "♠" },

            { 0xA0, "✽" },
            { 0xB1, "⚜" },
            { 0xD9, "~" },  // unidentified thingie
            { 0xE1, "$" },  // bag w/money (U+1f4b0)
            { 0xE8, "?" },  // cake w/candles (U+1f382)
            { 0xC6, "?" },  // printer (U+1f5a8)
            { 0xC4, "☏" },
            { 0xCE, "\x26fe" },  // cup
            { 0xCF, "?" },  // knife/fork (U+1f374)
            { 0xC8, "✄" },

            { 0xC2, "✈" },
            { 0xE4, "⌂" },  // house
            { 0xE2, "!" },  // lightbulb (U+1f4a1)
            { 0xC0, "?" },  // circle w/backslash
            { 0xE0, "?" },  // no smoking
            { 0xB2, "?" },  // man
            { 0xB3, "?" },  // woman
            { 0xC1, "♿" },
            { 0xC9, "☠" },
            { 0xE6, "?" }, // pet

            { 0xDD, "?" },  // treble clef (U+1d11e)
            { 0xDE, "♪" }, 
            { 0xD1, "✍" }, 
            { 0xEB, "☜" }, 
            { 0xEC, "☞" }, 
            { 0xD8, "✎" }, 
            { 0xCD, "⏰" }, 
            { 0xCA, "\x263a" }, // smiley face
            { 0xE9, "?" }, // Christmas tree (U+1f332)
            { 0xE5, "?" }, // Oak? tree
        };


        public static String GetChar(byte mode, byte[] data, MainForm form)
        {
            char result;
            string resStr;
            switch (mode)
            {
                case 0x13:
                    if (data.Length == 2)   // Special char pair
                    {
                        // combine bytes -> short
                        ushort lookup = (ushort)((data[0] * 256) + data[1]);
                        if (SpecialChar.TryGetValue(lookup, out result))
                            return result.ToString();
                        else
                        {
                            form.Log("GetChar() - Failed char map: mode=13, pair= " + data[0].ToString("X2") + "/" + data[1].ToString("X2"));
                            return "13{ 0x" + lookup.ToString("X4") + ", '' }, ";
                        }
                    }
                    else if (data[0] >= 0x20 && data[0] < 0x7F)    // ASCII - use directly
                    { 
                        return ((char)data[0]).ToString();         
                    }
                    else
                    {
                        // Lookup single-byte mapping for high byte
                       if (Mode13Map.TryGetValue(data[0], out resStr))
                            return resStr;
                        else
                        {
                            form.Log("GetChar() - Failed char map: mode=13, char=0x" + data[0].ToString("X2"));
                            return "13{ 0x" + data[0].ToString("X2") + ", \"\"}, ";
                        }
                    }

                case 0x14:
                    {
                        // Lookup single-byte mapping
                        if (Mode14Map.TryGetValue(data[0], out result))
                            return result.ToString();
                        else
                        {
                            form.Log("GetChar() - Failed char map: mode=14, char=0x" + data[0].ToString("X2"));
                            return "14{ 0x" + data[0].ToString("X2") + ", '' }, ";
                        }
                    }

                case 0x15:
                    {
                        // Lookup single-byte mapping
                        if (Mode15Map.TryGetValue(data[0], out resStr))
                            return resStr;
                        else
                        {
                            form.Log("GetChar() - Failed char map: mode=15, char=0x" + data[0].ToString("X2"));
                            return "15{ 0x" + data[0].ToString("X2") + ", \"\" }, ";
                        }
                    }

                default:
                    form.Log("GetChar() - Unrecognized mode: " + mode);
                    return "";
            }
        }
    }
}
