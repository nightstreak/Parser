  /*  
	Marumo g17m4529,
	Pawandiwa g17p0542
	Kangomba g17k7372
  
  */
  
  using Library;
  using System;
  using System.Text;

  class Token {
    public int kind;
    public string val;

    public Token(int kind, string val) {
      this.kind = kind;
      this.val = val;
    }

  } // Token

class Declarations {

    // +++++++++++++++++++++++++ File Handling and Error handlers ++++++++++++++++++++

    static InFile input;
    static OutFile output;

    static string NewFileName(string oldFileName, string ext) {
        // Creates new file name by changing extension of oldFileName to ext
        int i = oldFileName.LastIndexOf('.');
        if (i < 0) return oldFileName + ext; else return oldFileName.Substring(0, i) + ext;
    } // NewFileName

    static void ReportError(string errorMessage) {
        // Displays errorMessage on standard output and on reflected output
        Console.WriteLine(errorMessage);
        output.WriteLine(errorMessage);
    } // ReportError

    static void Abort(string errorMessage) {
        // Abandons parsing after issuing error message
        ReportError(errorMessage);
        output.Close();
        System.Environment.Exit(1);
    } // Abort

    // +++++++++++++++++++++++  token kinds enumeration +++++++++++++++++++++++++

    const int
      noSym = 0,
      EOFSym = 1,
      typeSym = 2,
      varSym = 3,
      semicolonSym = 4,
      equalSym = 5,
      colonSym = 6,
      identSym = 7,
      periodSym = 8,
      LsquarebraceSym = 9,
      RsquarebraceSym = 10,
      setSym = 11,
      ofSym = 12,
      LbracketSym = 13,
      RbracketSym = 14,
      endSym = 15,
      arraySym = 16,
      recordSym = 17,
      pointerSym = 18,
      commaSym = 19,
      toSym = 20,
      doubleperiodSym = 21,
      numberSym = 22,
      commentSym = 23,
      LcurlySym = 24,
      RcurlySym = 25;


    // and others like this

    // +++++++++++++++++++++++++++++ Character Handler ++++++++++++++++++++++++++

    const char EOF = '\0';
    static bool atEndOfFile = false;

    // Declaring ch as a global variable is done for expediency - global variables
    // are not always a good thing

    static char ch;    // look ahead character for scanner

    static void GetChar() {
        // Obtains next character ch from input, or CHR(0) if EOF reached
        // Reflect ch to output
        if (atEndOfFile) ch = EOF;
        else {
            ch = input.ReadChar();
            atEndOfFile = ch == EOF;
            if (!atEndOfFile) output.Write(ch);
        }
    } // GetChar

    // +++++++++++++++++++++++++++++++ Scanner ++++++++++++++++++++++++++++++++++

    // Declaring sym as a global variable is done for expediency - global variables
    // are not always a good thing

    static Token sym;

    static void GetSym() {
        // Scans for next sym from input
        while (ch > EOF && ch <= ' ') GetChar();
        StringBuilder symLex = new StringBuilder();
        int symKind = 0;

        if (Char.IsLetter(ch)) {
            do {
                symLex.Append(ch);
                GetChar();
            }
            while (Char.IsLetterOrDigit(ch));
            string Sym_Lex_string = symLex.ToString();
            switch (Sym_Lex_string) {
                case "ARRAY":
                    symKind = arraySym;
                    break;
                case "OF":
                    symKind = ofSym;
                    break;
                case "TO":
                    symKind = toSym;
                    break;
                case "SET":
                    symKind = arraySym;
                    break;
                case "POINTER":
                    symKind = pointerSym;
                    break;
                case "VAR":
                    symKind = varSym;
                    break;
                case "END":
                    symKind = endSym;
                    break;
                case "TYPE":
                    symKind = typeSym;
                    break;
                case "RECORD":
                    symKind = recordSym;
                    break;
                default:
                    symKind = identSym;
                    break;
            }
        }
        else if (Char.IsDigit(ch)) {
            do {
                symLex.Append(ch);
                GetChar();
            }
            while (Char.IsDigit(ch));
            symKind = numberSym;
        }
        else {
            switch (ch) {
                case ';':
                    symKind = semicolonSym;
                    symLex = symLex.Append(';');
                    GetChar();
                    break;
                case ':':
                    symKind = colonSym;
                    symLex = symLex.Append(':');
                    GetChar();
                    break;
                case '.':
                    symLex = symLex.Append('.');
                    GetChar();
                    if (ch == '.') {
                        symLex = symLex.Append('.');
                        symKind = doubleperiodSym;
                        GetChar();
                    }
                    else {
                        symKind = periodSym;
                        GetChar();
                    }
                    break;
                case '=':
                    symKind = equalSym;
                    symLex = symLex.Append('=');
                    GetChar();
                    break;
                case '(':
                    symLex = symLex.Append('(');
                    GetChar();
                    if (ch == '*')
                    {
                        GetChar();
                        while (ch != '*')
                        {
                            GetChar();
                        }
                        GetChar();
                        if (ch == ')')
                        {
                            symKind = commentSym;
                            GetChar();
                        }
                        break;
                    }
                    else
                    {
                        symKind = LbracketSym;
                        GetChar();
                        break;
                    }
                case ')':
                    symKind = RbracketSym;
                    symLex = symLex.Append(')');
                    GetChar();
                    break;
                case '[':
                    symKind = LsquarebraceSym;
                    symLex = symLex.Append('[');
                    GetChar();
                    break;
                case ']':
                    symKind = RsquarebraceSym;
                    symLex = symLex.Append(']');
                    GetChar();
                    break;
                case ',':
                    symKind = commaSym;
                    symLex = symLex.Append(',');
                    GetChar();
                    break;
                case '{':
                    symKind = LcurlySym;
                    symLex = symLex.Append('{');
                    GetChar();
                    break;
                case '}':
                    symKind = RcurlySym;
                    symLex = symLex.Append('}');
                    GetChar();
                    break;
                case EOF:
                    symKind = EOFSym;
                    symLex = symLex.Append(EOF);
                    break;
                default:
                    symKind = noSym;
                    GetChar();
                    break;
            }

        }
        sym = new Token(symKind, symLex.ToString());
        // over to you!
    }

    // GetSym


    // +++++++++++++++++++++++++++++++ Parser +++++++++++++++++++++++++++++++++++

    static void Accept(int wantedSym, string errorMessage) {
        // Checks that lookahead token is wantedSym
        if (sym.kind == wantedSym) GetSym(); else Abort(errorMessage);
    } // Accept

    static void Accept(IntSet allowedSet, string errorMessage) {
        // Checks that lookahead token is in allowedSet
        if (allowedSet.Contains(sym.kind)) GetSym(); else Abort(errorMessage);
    } // Accept


    static void Declaration() {
        if (sym.kind == typeSym)
        {
            Accept(typeSym, "Type Expected");
            GetSym();
            while (sym.kind == identSym)
            {
                TypeDecl();
                Accept(semicolonSym, "SEMI COLON EXPECTED ");
            }
        }
        else if (sym.kind == varSym)
        {
            Accept(varSym, "Var Expected");
            GetSym();
            while (sym.kind == identSym)
            {
                VarDecl();
                Accept(semicolonSym, "SEMI COLON EXPECTED");
            }
        } else
        {
            Abort("error wrong declaration");
        }

    }
    static void Mod2Decl()
    {
        // Mod2Decl = { Declaration } 
        while (sym.kind == typeSym || sym.kind == varSym)
        {
            Declaration();
            GetSym();
        }
    }
    static void TypeDecl() {
        Accept(identSym, "Identifier expected");
        GetSym();
        Accept(equalSym, "Equal symbol missing");
        Type();
        GetSym();
    }

    static void VarDecl() {
        IdentList();
        Accept(colonSym, "Colon expected ");
        Type();
        GetSym();

    }

    static void Type() {
        if (sym.kind == identSym || sym.kind == LbracketSym || sym.kind == LsquarebraceSym)
        {
            SimpleType();
        }
        else if (sym.kind == arraySym)
        {
            ArrayType();
        }
        else if (sym.kind == recordSym)
        {
            RecordType();
        } else if (sym.kind == setSym)
        {
            SetType();
        } else if (sym.kind == pointerSym)
        {
            PointerType();
         }
    }

    static void SimpleType() {
        if (sym.kind == identSym)
        {
            QualIdent();
            if (sym.kind == LsquarebraceSym)
            {
                Subrange();
            }
            else
            {
                Abort("error wrong declaration");
            }
        }
        else
        {
            Abort("error wrong declaration");
        }
        if (sym.kind == LbracketSym)
        {
            Enumeration();
        }
        else
        {
            Abort("error wrong declaration");
        }
        if (sym.kind == LsquarebraceSym)
        {
            Subrange();
        }
        else
        {
            Abort("error wrong declaration");
        }


    }

    static void QualIdent()
    {
        Accept(identSym, "Identifier expected.");
        GetSym();
        while(sym.kind == periodSym)
        {
            Accept(periodSym, "Period expected.");
            GetSym();
            Accept(identSym, "Identifier expected.");
            GetSym();
        }
    }

    static void Subrange()
    {
        Accept(LsquarebraceSym, "Left Square Bracket expected.");
        Constant();
        Accept(doubleperiodSym, "Double Period expected.");
        Constant();
        Accept(RbracketSym, "Right Square Bracket expected.");

    }

    static void Constant()
    {
        if(sym.kind == numberSym)
        {
            Accept(numberSym, "Number expected");
            GetSym();
        }
        else
        {
            Abort("error wrong declaration");
        }
        if (sym.kind == identSym)
        {
            Accept(identSym, "Identifier expected.");
            GetSym();
        }
        else
        {
            Abort("error wrong declaration");
        }
    }

    static void Enumeration()
    {
        Accept(LbracketSym, "Left Bracket expected");
        IdentList();
        Accept(RbracketSym, "Right Bracket expected");

    }

    static void IdentList()
    {
        Accept(identSym, "Identifier expected.");
        while (sym.kind == commaSym)
        {
            Accept(commaSym, "Comma expected.");
            Accept(identSym, "identifier expected");

        }
    }

    static void ArrayType()
    {
        Accept(arraySym, "Array expected.");
        SimpleType();
        while (sym.kind == commaSym)
        {
            Accept(commaSym, "Comma expected.");
            SimpleType();
        }
        Accept(ofSym, "Off expected.");
        Type();


    }

    static void RecordType()
    {
        Accept(recordSym, "Record expected.");
        FieldLists();
        Accept(endSym, "End expected.");
    }

    static void FieldLists()
    {
        FieldList();
        while(sym.kind == semicolonSym)
        {
            Accept(semicolonSym, "Semicolon expected");
            FieldList();
        }
    }

    static void FieldList()
    {
        if(sym.kind == identSym)
        {
            IdentList();
            Accept(colonSym, "Colon expected.");
            Type();
        }
        else
        {
            Abort("error wrong declaration");
        }
    }

    static void SetType()
    {
        Accept(setSym, "Set expected.");
        Accept(ofSym, "Of expected.");
        SimpleType();

    }

    static void PointerType()
    {
        Accept(pointerSym, "Pointer expected.");
        Accept(toSym, "To expected.");
        Type();
    }

  

    // +++++++++++++++++++++ Main driver function +++++++++++++++++++++++++++++++

    public static void Main(string[] args) {
      // Open input and output files from command line arguments
      if (args.Length == 0) {
        Console.WriteLine("Usage: Declarations FileName");
        System.Environment.Exit(1);
      }
      input = new InFile(args[0]);
      output = new OutFile(NewFileName(args[0], ".out"));

      GetChar();                                  // Lookahead character

  //  To test the scanner we can use a loop like the following:

      /*do {
        GetSym();                                 // Lookahead symbol
        OutFile.StdOut.Write(sym.kind, 3);
        OutFile.StdOut.WriteLine(" " + sym.val);  // See what we got
      } while (sym.kind != EOFSym);
      */
   //After the scanner is debugged we shall substitute this code:

      GetSym();                                   // Lookahead symbol
      Mod2Decl();                                 // Start to parse from the goal symbol
      // if we get back here everything must have been satisfactory
      Console.WriteLine("Parsed correctly");

 
      output.Close();
    } // Main

  } // Declarations

