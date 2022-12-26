using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace LitJson
{
	internal class Lexer
	{
		private delegate bool StateHandler(FsmContext ctx);

		private static int[] fsm_return_table;

		private static StateHandler[] fsm_handler_table;

		private bool allow_comments;

		private bool allow_single_quoted_strings;

		private bool end_of_input;

		private FsmContext fsm_context;

		private int input_buffer;

		private int input_char;

		private TextReader reader;

		private int state;

		private StringBuilder string_buffer;

		private string string_value;

		private int token;

		private int unichar;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache2;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache3;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache4;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache5;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache6;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache7;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache8;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache9;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cacheA;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cacheB;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cacheC;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cacheD;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cacheE;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cacheF;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache10;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache11;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache12;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache13;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache14;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache15;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache16;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache17;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache18;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache19;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache1A;

		[CompilerGenerated]
		private static StateHandler _003C_003Ef__mg_0024cache1B;

		public bool AllowComments
		{
			get
			{
				return allow_comments;
			}
			set
			{
				allow_comments = value;
			}
		}

		public bool AllowSingleQuotedStrings
		{
			get
			{
				return allow_single_quoted_strings;
			}
			set
			{
				allow_single_quoted_strings = value;
			}
		}

		public bool EndOfInput
		{
			get
			{
				return end_of_input;
			}
		}

		public int Token
		{
			get
			{
				return token;
			}
		}

		public string StringValue
		{
			get
			{
				return string_value;
			}
		}

		public Lexer(TextReader reader)
		{
			allow_comments = true;
			allow_single_quoted_strings = true;
			input_buffer = 0;
			string_buffer = new StringBuilder(128);
			state = 1;
			end_of_input = false;
			this.reader = reader;
			fsm_context = new FsmContext();
			fsm_context.L = this;
		}

		static Lexer()
		{
			PopulateFsmTables();
		}

		private static int HexValue(int digit)
		{
			switch (digit)
			{
			case 65:
			case 97:
				return 10;
			case 66:
			case 98:
				return 11;
			case 67:
			case 99:
				return 12;
			case 68:
			case 100:
				return 13;
			case 69:
			case 101:
				return 14;
			case 70:
			case 102:
				return 15;
			default:
				return digit - 48;
			}
		}

		private static void PopulateFsmTables()
		{
			StateHandler[] array = new StateHandler[28];
			if (_003C_003Ef__mg_0024cache0 == null)
			{
				_003C_003Ef__mg_0024cache0 = State1;
			}
			array[0] = _003C_003Ef__mg_0024cache0;
			if (_003C_003Ef__mg_0024cache1 == null)
			{
				_003C_003Ef__mg_0024cache1 = State2;
			}
			array[1] = _003C_003Ef__mg_0024cache1;
			if (_003C_003Ef__mg_0024cache2 == null)
			{
				_003C_003Ef__mg_0024cache2 = State3;
			}
			array[2] = _003C_003Ef__mg_0024cache2;
			if (_003C_003Ef__mg_0024cache3 == null)
			{
				_003C_003Ef__mg_0024cache3 = State4;
			}
			array[3] = _003C_003Ef__mg_0024cache3;
			if (_003C_003Ef__mg_0024cache4 == null)
			{
				_003C_003Ef__mg_0024cache4 = State5;
			}
			array[4] = _003C_003Ef__mg_0024cache4;
			if (_003C_003Ef__mg_0024cache5 == null)
			{
				_003C_003Ef__mg_0024cache5 = State6;
			}
			array[5] = _003C_003Ef__mg_0024cache5;
			if (_003C_003Ef__mg_0024cache6 == null)
			{
				_003C_003Ef__mg_0024cache6 = State7;
			}
			array[6] = _003C_003Ef__mg_0024cache6;
			if (_003C_003Ef__mg_0024cache7 == null)
			{
				_003C_003Ef__mg_0024cache7 = State8;
			}
			array[7] = _003C_003Ef__mg_0024cache7;
			if (_003C_003Ef__mg_0024cache8 == null)
			{
				_003C_003Ef__mg_0024cache8 = State9;
			}
			array[8] = _003C_003Ef__mg_0024cache8;
			if (_003C_003Ef__mg_0024cache9 == null)
			{
				_003C_003Ef__mg_0024cache9 = State10;
			}
			array[9] = _003C_003Ef__mg_0024cache9;
			if (_003C_003Ef__mg_0024cacheA == null)
			{
				_003C_003Ef__mg_0024cacheA = State11;
			}
			array[10] = _003C_003Ef__mg_0024cacheA;
			if (_003C_003Ef__mg_0024cacheB == null)
			{
				_003C_003Ef__mg_0024cacheB = State12;
			}
			array[11] = _003C_003Ef__mg_0024cacheB;
			if (_003C_003Ef__mg_0024cacheC == null)
			{
				_003C_003Ef__mg_0024cacheC = State13;
			}
			array[12] = _003C_003Ef__mg_0024cacheC;
			if (_003C_003Ef__mg_0024cacheD == null)
			{
				_003C_003Ef__mg_0024cacheD = State14;
			}
			array[13] = _003C_003Ef__mg_0024cacheD;
			if (_003C_003Ef__mg_0024cacheE == null)
			{
				_003C_003Ef__mg_0024cacheE = State15;
			}
			array[14] = _003C_003Ef__mg_0024cacheE;
			if (_003C_003Ef__mg_0024cacheF == null)
			{
				_003C_003Ef__mg_0024cacheF = State16;
			}
			array[15] = _003C_003Ef__mg_0024cacheF;
			if (_003C_003Ef__mg_0024cache10 == null)
			{
				_003C_003Ef__mg_0024cache10 = State17;
			}
			array[16] = _003C_003Ef__mg_0024cache10;
			if (_003C_003Ef__mg_0024cache11 == null)
			{
				_003C_003Ef__mg_0024cache11 = State18;
			}
			array[17] = _003C_003Ef__mg_0024cache11;
			if (_003C_003Ef__mg_0024cache12 == null)
			{
				_003C_003Ef__mg_0024cache12 = State19;
			}
			array[18] = _003C_003Ef__mg_0024cache12;
			if (_003C_003Ef__mg_0024cache13 == null)
			{
				_003C_003Ef__mg_0024cache13 = State20;
			}
			array[19] = _003C_003Ef__mg_0024cache13;
			if (_003C_003Ef__mg_0024cache14 == null)
			{
				_003C_003Ef__mg_0024cache14 = State21;
			}
			array[20] = _003C_003Ef__mg_0024cache14;
			if (_003C_003Ef__mg_0024cache15 == null)
			{
				_003C_003Ef__mg_0024cache15 = State22;
			}
			array[21] = _003C_003Ef__mg_0024cache15;
			if (_003C_003Ef__mg_0024cache16 == null)
			{
				_003C_003Ef__mg_0024cache16 = State23;
			}
			array[22] = _003C_003Ef__mg_0024cache16;
			if (_003C_003Ef__mg_0024cache17 == null)
			{
				_003C_003Ef__mg_0024cache17 = State24;
			}
			array[23] = _003C_003Ef__mg_0024cache17;
			if (_003C_003Ef__mg_0024cache18 == null)
			{
				_003C_003Ef__mg_0024cache18 = State25;
			}
			array[24] = _003C_003Ef__mg_0024cache18;
			if (_003C_003Ef__mg_0024cache19 == null)
			{
				_003C_003Ef__mg_0024cache19 = State26;
			}
			array[25] = _003C_003Ef__mg_0024cache19;
			if (_003C_003Ef__mg_0024cache1A == null)
			{
				_003C_003Ef__mg_0024cache1A = State27;
			}
			array[26] = _003C_003Ef__mg_0024cache1A;
			if (_003C_003Ef__mg_0024cache1B == null)
			{
				_003C_003Ef__mg_0024cache1B = State28;
			}
			array[27] = _003C_003Ef__mg_0024cache1B;
			fsm_handler_table = array;
			fsm_return_table = new int[28]
			{
				65542, 0, 65537, 65537, 0, 65537, 0, 65537, 0, 0,
				65538, 0, 0, 0, 65539, 0, 0, 65540, 65541, 65542,
				0, 0, 65541, 65542, 0, 0, 0, 0
			};
		}

		private static char ProcessEscChar(int esc_char)
		{
			switch (esc_char)
			{
			case 34:
			case 39:
			case 47:
			case 92:
				return Convert.ToChar(esc_char);
			case 110:
				return '\n';
			case 116:
				return '\t';
			case 114:
				return '\r';
			case 98:
				return '\b';
			case 102:
				return '\f';
			default:
				return '?';
			}
		}

		private static bool State1(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
				{
					continue;
				}
				if (ctx.L.input_char >= 49 && ctx.L.input_char <= 57)
				{
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					ctx.NextState = 3;
					return true;
				}
				switch (ctx.L.input_char)
				{
				case 34:
					ctx.NextState = 19;
					ctx.Return = true;
					return true;
				case 44:
				case 58:
				case 91:
				case 93:
				case 123:
				case 125:
					ctx.NextState = 1;
					ctx.Return = true;
					return true;
				case 45:
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					ctx.NextState = 2;
					return true;
				case 48:
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					ctx.NextState = 4;
					return true;
				case 102:
					ctx.NextState = 12;
					return true;
				case 110:
					ctx.NextState = 16;
					return true;
				case 116:
					ctx.NextState = 9;
					return true;
				case 39:
					if (!ctx.L.allow_single_quoted_strings)
					{
						return false;
					}
					ctx.L.input_char = 34;
					ctx.NextState = 23;
					ctx.Return = true;
					return true;
				case 47:
					if (!ctx.L.allow_comments)
					{
						return false;
					}
					ctx.NextState = 25;
					return true;
				default:
					return false;
				}
			}
			return true;
		}

		private static bool State2(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char >= 49 && ctx.L.input_char <= 57)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 3;
				return true;
			}
			int num = ctx.L.input_char;
			if (num == 48)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 4;
				return true;
			}
			return false;
		}

		private static bool State3(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
				{
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					continue;
				}
				if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
				{
					ctx.Return = true;
					ctx.NextState = 1;
					return true;
				}
				switch (ctx.L.input_char)
				{
				case 44:
				case 93:
				case 125:
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 1;
					return true;
				case 46:
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					ctx.NextState = 5;
					return true;
				case 69:
				case 101:
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					ctx.NextState = 7;
					return true;
				default:
					return false;
				}
			}
			return true;
		}

		private static bool State4(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			switch (ctx.L.input_char)
			{
			case 44:
			case 93:
			case 125:
				ctx.L.UngetChar();
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			case 46:
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 5;
				return true;
			case 69:
			case 101:
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 7;
				return true;
			default:
				return false;
			}
		}

		private static bool State5(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 6;
				return true;
			}
			return false;
		}

		private static bool State6(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
				{
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					continue;
				}
				if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
				{
					ctx.Return = true;
					ctx.NextState = 1;
					return true;
				}
				switch (ctx.L.input_char)
				{
				case 44:
				case 93:
				case 125:
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 1;
					return true;
				case 69:
				case 101:
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					ctx.NextState = 7;
					return true;
				default:
					return false;
				}
			}
			return true;
		}

		private static bool State7(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 8;
				return true;
			}
			int num = ctx.L.input_char;
			if (num == 43 || num == 45)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 8;
				return true;
			}
			return false;
		}

		private static bool State8(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
				{
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					continue;
				}
				if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
				{
					ctx.Return = true;
					ctx.NextState = 1;
					return true;
				}
				int num = ctx.L.input_char;
				if (num == 44 || num == 93 || num == 125)
				{
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 1;
					return true;
				}
				return false;
			}
			return true;
		}

		private static bool State9(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num == 114)
			{
				ctx.NextState = 10;
				return true;
			}
			return false;
		}

		private static bool State10(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num == 117)
			{
				ctx.NextState = 11;
				return true;
			}
			return false;
		}

		private static bool State11(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num == 101)
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		private static bool State12(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num == 97)
			{
				ctx.NextState = 13;
				return true;
			}
			return false;
		}

		private static bool State13(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num == 108)
			{
				ctx.NextState = 14;
				return true;
			}
			return false;
		}

		private static bool State14(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num == 115)
			{
				ctx.NextState = 15;
				return true;
			}
			return false;
		}

		private static bool State15(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num == 101)
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		private static bool State16(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num == 117)
			{
				ctx.NextState = 17;
				return true;
			}
			return false;
		}

		private static bool State17(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num == 108)
			{
				ctx.NextState = 18;
				return true;
			}
			return false;
		}

		private static bool State18(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num == 108)
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		private static bool State19(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				switch (ctx.L.input_char)
				{
				case 34:
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 20;
					return true;
				case 92:
					ctx.StateStack = 19;
					ctx.NextState = 21;
					return true;
				}
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
			}
			return true;
		}

		private static bool State20(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num == 34)
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		private static bool State21(FsmContext ctx)
		{
			ctx.L.GetChar();
			switch (ctx.L.input_char)
			{
			case 117:
				ctx.NextState = 22;
				return true;
			case 34:
			case 39:
			case 47:
			case 92:
			case 98:
			case 102:
			case 110:
			case 114:
			case 116:
				ctx.L.string_buffer.Append(ProcessEscChar(ctx.L.input_char));
				ctx.NextState = ctx.StateStack;
				return true;
			default:
				return false;
			}
		}

		private static bool State22(FsmContext ctx)
		{
			int num = 0;
			int num2 = 4096;
			ctx.L.unichar = 0;
			while (ctx.L.GetChar())
			{
				if ((ctx.L.input_char >= 48 && ctx.L.input_char <= 57) || (ctx.L.input_char >= 65 && ctx.L.input_char <= 70) || (ctx.L.input_char >= 97 && ctx.L.input_char <= 102))
				{
					ctx.L.unichar += HexValue(ctx.L.input_char) * num2;
					num++;
					num2 /= 16;
					if (num == 4)
					{
						ctx.L.string_buffer.Append(Convert.ToChar(ctx.L.unichar));
						ctx.NextState = ctx.StateStack;
						return true;
					}
					continue;
				}
				return false;
			}
			return true;
		}

		private static bool State23(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				switch (ctx.L.input_char)
				{
				case 39:
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 24;
					return true;
				case 92:
					ctx.StateStack = 23;
					ctx.NextState = 21;
					return true;
				}
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
			}
			return true;
		}

		private static bool State24(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num == 39)
			{
				ctx.L.input_char = 34;
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		private static bool State25(FsmContext ctx)
		{
			ctx.L.GetChar();
			switch (ctx.L.input_char)
			{
			case 42:
				ctx.NextState = 27;
				return true;
			case 47:
				ctx.NextState = 26;
				return true;
			default:
				return false;
			}
		}

		private static bool State26(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char == 10)
				{
					ctx.NextState = 1;
					return true;
				}
			}
			return true;
		}

		private static bool State27(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char == 42)
				{
					ctx.NextState = 28;
					return true;
				}
			}
			return true;
		}

		private static bool State28(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char == 42)
				{
					continue;
				}
				if (ctx.L.input_char == 47)
				{
					ctx.NextState = 1;
					return true;
				}
				ctx.NextState = 27;
				return true;
			}
			return true;
		}

		private bool GetChar()
		{
			if ((input_char = NextChar()) != -1)
			{
				return true;
			}
			end_of_input = true;
			return false;
		}

		private int NextChar()
		{
			if (input_buffer != 0)
			{
				int result = input_buffer;
				input_buffer = 0;
				return result;
			}
			return reader.Read();
		}

		public bool NextToken()
		{
			fsm_context.Return = false;
			while (true)
			{
				StateHandler stateHandler = fsm_handler_table[state - 1];
				if (!stateHandler(fsm_context))
				{
					throw new JsonException(input_char);
				}
				if (end_of_input)
				{
					return false;
				}
				if (fsm_context.Return)
				{
					break;
				}
				state = fsm_context.NextState;
			}
			string_value = string_buffer.ToString();
			string_buffer.Remove(0, string_buffer.Length);
			token = fsm_return_table[state - 1];
			if (token == 65542)
			{
				token = input_char;
			}
			state = fsm_context.NextState;
			return true;
		}

		private void UngetChar()
		{
			input_buffer = input_char;
		}
	}
}
