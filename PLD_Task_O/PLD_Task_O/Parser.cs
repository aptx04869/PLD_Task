﻿
using System;
using System.IO;
using System.Runtime.Serialization;
using com.calitha.goldparser.lalr;
using com.calitha.commons;
using System.Windows.Forms;

namespace com.calitha.goldparser
{

    [Serializable()]
    public class SymbolException : System.Exception
    {
        public SymbolException(string message) : base(message)
        {
        }

        public SymbolException(string message,
            Exception inner) : base(message, inner)
        {
        }

        protected SymbolException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

    }

    [Serializable()]
    public class RuleException : System.Exception
    {

        public RuleException(string message) : base(message)
        {
        }

        public RuleException(string message,
                             Exception inner) : base(message, inner)
        {
        }

        protected RuleException(SerializationInfo info,
                                StreamingContext context) : base(info, context)
        {
        }

    }

    enum SymbolConstants : int
    {
        SYMBOL_EOF        =  0, // (EOF)
        SYMBOL_ERROR      =  1, // (Error)
        SYMBOL_WHITESPACE =  2, // Whitespace
        SYMBOL_MINUS      =  3, // '-'
        SYMBOL_EXCLAMEQ   =  4, // '!='
        SYMBOL_PERCENT    =  5, // '%'
        SYMBOL_LPAREN     =  6, // '('
        SYMBOL_RPAREN     =  7, // ')'
        SYMBOL_TIMES      =  8, // '*'
        SYMBOL_TIMESTIMES =  9, // '**'
        SYMBOL_COMMA      = 10, // ','
        SYMBOL_DIV        = 11, // '/'
        SYMBOL_COLON      = 12, // ':'
        SYMBOL_PLUS       = 13, // '+'
        SYMBOL_LT         = 14, // '<'
        SYMBOL_EQ         = 15, // '='
        SYMBOL_EQEQ       = 16, // '=='
        SYMBOL_GT         = 17, // '>'
        SYMBOL_DIGIT      = 18, // Digit
        SYMBOL_ELSE       = 19, // else
        SYMBOL_END        = 20, // End
        SYMBOL_FOR        = 21, // for
        SYMBOL_ID         = 22, // ID
        SYMBOL_IF         = 23, // if
        SYMBOL_IN         = 24, // in
        SYMBOL_RANGE      = 25, // range
        SYMBOL_START      = 26, // Start
        SYMBOL_ASSIGN     = 27, // <assign>
        SYMBOL_COND       = 28, // <cond>
        SYMBOL_DIGIT2     = 29, // <digit>
        SYMBOL_EXP        = 30, // <exp>
        SYMBOL_EXPR       = 31, // <expr>
        SYMBOL_FACTOR     = 32, // <factor>
        SYMBOL_FOR_STMT   = 33, // <for_stmt>
        SYMBOL_ID2        = 34, // <id>
        SYMBOL_IF_STMT    = 35, // <if_stmt>
        SYMBOL_OP         = 36, // <op>
        SYMBOL_PROGRAM    = 37, // <program>
        SYMBOL_STMT       = 38, // <stmt>
        SYMBOL_STMT_LIST  = 39, // <stmt_list>
        SYMBOL_TERM       = 40  // <term>
    };

    enum RuleConstants : int
    {
        RULE_PROGRAM_START_END                                     =  0, // <program> ::= Start <stmt_list> End
        RULE_STMT_LIST                                             =  1, // <stmt_list> ::= <stmt>
        RULE_STMT_LIST2                                            =  2, // <stmt_list> ::= <stmt> <stmt_list>
        RULE_STMT                                                  =  3, // <stmt> ::= <assign>
        RULE_STMT2                                                 =  4, // <stmt> ::= <if_stmt>
        RULE_STMT3                                                 =  5, // <stmt> ::= <for_stmt>
        RULE_ASSIGN_EQ                                             =  6, // <assign> ::= <id> '=' <expr>
        RULE_ID_ID                                                 =  7, // <id> ::= ID
        RULE_EXPR_PLUS                                             =  8, // <expr> ::= <expr> '+' <term>
        RULE_EXPR_MINUS                                            =  9, // <expr> ::= <expr> '-' <term>
        RULE_EXPR                                                  = 10, // <expr> ::= <term>
        RULE_TERM_TIMES                                            = 11, // <term> ::= <term> '*' <factor>
        RULE_TERM_DIV                                              = 12, // <term> ::= <term> '/' <factor>
        RULE_TERM_PERCENT                                          = 13, // <term> ::= <term> '%' <factor>
        RULE_TERM                                                  = 14, // <term> ::= <factor>
        RULE_FACTOR_TIMESTIMES                                     = 15, // <factor> ::= <factor> '**' <exp>
        RULE_FACTOR                                                = 16, // <factor> ::= <exp>
        RULE_EXP_LPAREN_RPAREN                                     = 17, // <exp> ::= '(' <expr> ')'
        RULE_EXP                                                   = 18, // <exp> ::= <id>
        RULE_EXP2                                                  = 19, // <exp> ::= <digit>
        RULE_DIGIT_DIGIT                                           = 20, // <digit> ::= Digit
        RULE_IF_STMT_IF_COLON                                      = 21, // <if_stmt> ::= if <cond> ':' <stmt_list>
        RULE_IF_STMT_IF_COLON_ELSE_COLON                           = 22, // <if_stmt> ::= if <cond> ':' <stmt_list> else ':' <stmt_list>
        RULE_COND                                                  = 23, // <cond> ::= <expr> <op> <expr>
        RULE_OP_LT                                                 = 24, // <op> ::= '<'
        RULE_OP_GT                                                 = 25, // <op> ::= '>'
        RULE_OP_EQEQ                                               = 26, // <op> ::= '=='
        RULE_OP_EXCLAMEQ                                           = 27, // <op> ::= '!='
        RULE_FOR_STMT_FOR_IN_RANGE_LPAREN_COMMA_RPAREN_COLON       = 28, // <for_stmt> ::= for <id> in range '(' <expr> ',' <expr> ')' ':' <stmt_list>
        RULE_FOR_STMT_FOR_IN_RANGE_LPAREN_COMMA_COMMA_RPAREN_COLON = 29  // <for_stmt> ::= for <id> in range '(' <expr> ',' <expr> ',' <expr> ')' ':' <stmt_list>
    };

    public class MyParser
    {
        private LALRParser parser;
        ListBox lst, lst2;

        public MyParser(string filename, ListBox lst, ListBox lst2)
        {
            FileStream stream = new FileStream(filename,
                                               FileMode.Open,
                                               FileAccess.Read,
                                               FileShare.Read);
            this.lst = lst;
            this.lst2 = lst2;
            Init(stream);
            stream.Close();
        }

        public MyParser(string baseName, string resourceName)
        {
            byte[] buffer = ResourceUtil.GetByteArrayResource(
                System.Reflection.Assembly.GetExecutingAssembly(),
                baseName,
                resourceName);
            MemoryStream stream = new MemoryStream(buffer);
            Init(stream);
            stream.Close();
        }

        public MyParser(Stream stream)
        {
            Init(stream);
        }

        private void Init(Stream stream)
        {
            CGTReader reader = new CGTReader(stream);
            parser = reader.CreateNewParser();
            parser.TrimReductions = false;
            parser.StoreTokens = LALRParser.StoreTokensMode.NoUserObject;

            parser.OnTokenError += new LALRParser.TokenErrorHandler(TokenErrorEvent);
            parser.OnParseError += new LALRParser.ParseErrorHandler(ParseErrorEvent);
            parser.OnTokenRead += new LALRParser.TokenReadHandler(TokenReadEvent);
        }

        public void Parse(string source)
        {
            NonterminalToken token = parser.Parse(source);
            if (token != null)
            {
                Object obj = CreateObject(token);
                //todo: Use your object any way you like
            }
        }

        private Object CreateObject(Token token)
        {
            if (token is TerminalToken)
                return CreateObjectFromTerminal((TerminalToken)token);
            else
                return CreateObjectFromNonterminal((NonterminalToken)token);
        }

        private Object CreateObjectFromTerminal(TerminalToken token)
        {
            switch (token.Symbol.Id)
            {
                case (int)SymbolConstants.SYMBOL_EOF :
                //(EOF)
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_ERROR :
                //(Error)
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_WHITESPACE :
                //Whitespace
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_MINUS :
                //'-'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_EXCLAMEQ :
                //'!='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_PERCENT :
                //'%'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_LPAREN :
                //'('
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_RPAREN :
                //')'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_TIMES :
                //'*'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_TIMESTIMES :
                //'**'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_COMMA :
                //','
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_DIV :
                //'/'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_COLON :
                //':'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_PLUS :
                //'+'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_LT :
                //'<'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_EQ :
                //'='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_EQEQ :
                //'=='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_GT :
                //'>'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_DIGIT :
                //Digit
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_ELSE :
                //else
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_END :
                //End
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_FOR :
                //for
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_ID :
                //ID
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_IF :
                //if
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_IN :
                //in
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_RANGE :
                //range
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_START :
                //Start
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_ASSIGN :
                //<assign>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_COND :
                //<cond>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_DIGIT2 :
                //<digit>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_EXP :
                //<exp>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_EXPR :
                //<expr>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_FACTOR :
                //<factor>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_FOR_STMT :
                //<for_stmt>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_ID2 :
                //<id>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_IF_STMT :
                //<if_stmt>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_OP :
                //<op>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_PROGRAM :
                //<program>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_STMT :
                //<stmt>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_STMT_LIST :
                //<stmt_list>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_TERM :
                //<term>
                //todo: Create a new object that corresponds to the symbol
                return null;

            }
            throw new SymbolException("Unknown symbol");
        }

        public Object CreateObjectFromNonterminal(NonterminalToken token)
        {
            switch (token.Rule.Id)
            {
                case (int)RuleConstants.RULE_PROGRAM_START_END :
                //<program> ::= Start <stmt_list> End
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_STMT_LIST :
                //<stmt_list> ::= <stmt>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_STMT_LIST2 :
                //<stmt_list> ::= <stmt> <stmt_list>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_STMT :
                //<stmt> ::= <assign>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_STMT2 :
                //<stmt> ::= <if_stmt>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_STMT3 :
                //<stmt> ::= <for_stmt>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_ASSIGN_EQ :
                //<assign> ::= <id> '=' <expr>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_ID_ID :
                //<id> ::= ID
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_EXPR_PLUS :
                //<expr> ::= <expr> '+' <term>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_EXPR_MINUS :
                //<expr> ::= <expr> '-' <term>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_EXPR :
                //<expr> ::= <term>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_TERM_TIMES :
                //<term> ::= <term> '*' <factor>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_TERM_DIV :
                //<term> ::= <term> '/' <factor>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_TERM_PERCENT :
                //<term> ::= <term> '%' <factor>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_TERM :
                //<term> ::= <factor>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_FACTOR_TIMESTIMES :
                //<factor> ::= <factor> '**' <exp>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_FACTOR :
                //<factor> ::= <exp>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_EXP_LPAREN_RPAREN :
                //<exp> ::= '(' <expr> ')'
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_EXP :
                //<exp> ::= <id>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_EXP2 :
                //<exp> ::= <digit>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_DIGIT_DIGIT :
                //<digit> ::= Digit
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_IF_STMT_IF_COLON :
                //<if_stmt> ::= if <cond> ':' <stmt_list>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_IF_STMT_IF_COLON_ELSE_COLON :
                //<if_stmt> ::= if <cond> ':' <stmt_list> else ':' <stmt_list>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_COND :
                //<cond> ::= <expr> <op> <expr>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_OP_LT :
                //<op> ::= '<'
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_OP_GT :
                //<op> ::= '>'
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_OP_EQEQ :
                //<op> ::= '=='
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_OP_EXCLAMEQ :
                //<op> ::= '!='
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_FOR_STMT_FOR_IN_RANGE_LPAREN_COMMA_RPAREN_COLON :
                //<for_stmt> ::= for <id> in range '(' <expr> ',' <expr> ')' ':' <stmt_list>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_FOR_STMT_FOR_IN_RANGE_LPAREN_COMMA_COMMA_RPAREN_COLON :
                //<for_stmt> ::= for <id> in range '(' <expr> ',' <expr> ',' <expr> ')' ':' <stmt_list>
                //todo: Create a new object using the stored tokens.
                return null;

            }
            throw new RuleException("Unknown rule");
        }

        private void TokenErrorEvent(LALRParser parser, TokenErrorEventArgs args)
        {
            string message = "Token error with input: '"+args.Token.ToString()+"'";
            //todo: Report message to UI?
        }

        private void ParseErrorEvent(LALRParser parser, ParseErrorEventArgs args)
        {
            string message = "Parse error caused by token: '" + args.UnexpectedToken.ToString() + "in line: " + args.UnexpectedToken.Location.LineNr;
            lst.Items.Add(message);
            string message2 = args.ExpectedTokens.ToString();
            lst.Items.Add("Expected Token: " + message2);
            //todo: Report message to UI?
        }

        private void TokenReadEvent(LALRParser parser, TokenReadEventArgs args)
        {
            string message = args.Token.Text + "  \t \t " + (SymbolConstants)args.Token.Symbol.Id;
            lst2.Items.Add(message);
            //todo: Report message to UI?
        }

    }
}
