using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    Integer, Read, Write, Repeat, IF, String, ELSEIF, ELSE, ENDL, Then,
    Idenifier, Operator, comment, FLOAT, RETURN, LeftBrances, Dot, Semicolon, DivideOp,
    MultiplyOp, MinusOp, PlusOp, NotEqualOp, GreaterThanOp, LessThanOp, EqualOp, RParanthesis,
    LParanthesis, Comma, RightBrances, Assign, Number, or, and, END, UNTIL,MAIN
}
namespace JASON_Compiler
{

    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("int", Token_Class.Integer);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("float", Token_Class.FLOAT);
            ReservedWords.Add("if", Token_Class.IF);
            ReservedWords.Add("elseif", Token_Class.ELSEIF);
            ReservedWords.Add("else", Token_Class.ELSE);
            ReservedWords.Add("end", Token_Class.END);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("endl", Token_Class.ENDL);
            ReservedWords.Add("until", Token_Class.UNTIL);
            ReservedWords.Add("return", Token_Class.RETURN);
            ReservedWords.Add("main", Token_Class.MAIN);

            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("{", Token_Class.LeftBrances);
            Operators.Add("}", Token_Class.RightBrances);
            Operators.Add(":=", Token_Class.Assign);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("&&", Token_Class.and);
            Operators.Add("||", Token_Class.or);

        }

        public void StartScanning(string SourceCode)
        {

            // j: Inner loop to check on each character in a single lexeme.   
            for (int i = 0; i < SourceCode.Length; i++)
            {

                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                // an Idenifier or a Reserved Word 
                if (char.IsLetter(CurrentChar))
                {
                    string token = "";
                    while (char.IsLetter(CurrentChar) || char.IsDigit(CurrentChar))
                    {

                        token += CurrentChar.ToString();
                        j++;
                        if (j >= SourceCode.Length)
                            break;
                        CurrentChar = SourceCode[j];
                    }
                    FindTokenClass(token);
                    i = j - 1;
                }

                //number
                else if (char.IsDigit(CurrentChar) )
                {
                    string token = "";
                    while ( (CurrentChar == '.' || char.IsDigit(SourceCode[j]) ) )
                    {
                        token += CurrentChar.ToString();
                        j++;
                        if (j >= SourceCode.Length)
                            break;
                        CurrentChar = SourceCode[j];
                    }
                    FindTokenClass(token);
                    i = j - 1;
                }

                //string 
                else if (CurrentChar == '"')
                {
                    Token tok = new Token();
                    tok.lex = CurrentChar.ToString();
                    bool found = false;
                    j++;
                    CurrentChar = SourceCode[j];
                    while (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        tok.lex += CurrentChar.ToString();

                        if (CurrentChar == '"')
                        {
                            found = true;
                            tok.token_type = Token_Class.String;
                            Tokens.Add(tok);
                            break;
                        }
                        j++;
                    }
                    i = j + 1;
                    if (found == false)
                    {
                        Errors.Error_List.Add(tok.lex);
                    }
                }

                //comment 
                else if (CurrentChar == '/' && SourceCode[i + 1] == '*')
                {
                    Token tok = new Token();
                    tok.lex = "/*";
                    j = j + 2;
                    //     CurrentChar = SourceCode[j];
                    bool found = false;
                    while (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        tok.lex += CurrentChar.ToString();
                        if (CurrentChar == '*' && SourceCode[j + 1] == '/')
                        {
                            found = true;
                            tok.lex += SourceCode[j + 1].ToString();
                            tok.token_type = Token_Class.comment;
                            Tokens.Add(tok);
                            break;
                        }
                        j++;
                    }
                    i = j + 2;
                    if (found == false)
                    {
                        Errors.Error_List.Add(tok.lex);
                    }
                }

                //operator
                else
                {
                    string str = CurrentChar.ToString();
                    Token tok = new Token();

                    if ((CurrentChar == ':' && SourceCode[i + 1] == '=') || (CurrentChar == '<' && SourceCode[i + 1] == '>') || (CurrentChar == '&' && SourceCode[i + 1] == '&') || (CurrentChar == '|' && SourceCode[i + 1] == '|'))
                    {
                        str += SourceCode[i + 1].ToString();
                        tok.lex = str;
                        tok.token_type = Operators[str];
                        Tokens.Add(tok);
                        i++;
                    }

                    else if (Operators.ContainsKey(str))
                    {
                        tok.lex = str;
                        tok.token_type = Operators[str];
                        Tokens.Add(tok);
                    }

                    else
                    {
                        Errors.Error_List.Add(str);
                    }

                }
            }

            JASON_Compiler.TokenStream = Tokens;
        }

        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;

            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }

            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
            }


            //Is it an Number?
            else if (isNumber(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }


            //Is it an undefined?

            else
            {
                Errors.Error_List.Add(Lex);
                return;

            }

        }

        bool isIdentifier(string lex)
        {
            bool isValid = false;
            var rx = new Regex(@"^[a-z|A-Z]([a-z|A-Z][0-9])*", RegexOptions.Compiled);
            if (rx.IsMatch(lex))
                isValid = true;

            return isValid;
        }

        bool isNumber(string lex)
        {
            bool isValid = false;
            var rx = new Regex(@"^[0-9]+(\.[0-9]+)?$", RegexOptions.Compiled);
            if (rx.IsMatch(lex))
                isValid = true;

            return isValid;
        }

    }
}
