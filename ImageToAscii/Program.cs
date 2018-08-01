using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Globalization;

namespace ImageToAscii {
	class Program {
		//static string Chars = "MNmdhyso+/:-. ";
		static string Chars = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~<>i!lI;:,\" ^`'. ";

		static bool InvertColors = false;
		static bool PrintToConsole = false;
		static bool NoAscii = false;
		static bool PrintColor = false;
		static float Scale = 1.0f;

		static Random Rnd = new Random();

		static void Main(string[] Args) {
			List<string> FileNames = new List<string>();

			foreach (var Arg in Args)
				if (Arg == "--stdout")
					PrintToConsole = true;
				else if (Arg == "--invert")
					InvertColors = true;
				else if (Arg == "--noascii")
					NoAscii = true;
				else if (Arg == "--color")
					PrintColor = true;
				else if (Arg.StartsWith("--s")) {
					Scale = float.Parse(Arg.Substring(3), CultureInfo.InvariantCulture);
				} else
					FileNames.Add(Arg);

			foreach (var FileName in FileNames)
				if (File.Exists(FileName))
					ConvertImage(FileName, Path.ChangeExtension(FileName, ".txt"));

			if (PrintColor)
				Console.ResetColor();
		}

		static void ConvertImage(string InFile, string OutFile) {
			FileStream FS = null;

			if (File.Exists(OutFile))
				File.Delete(OutFile);

			if (!PrintToConsole)
				FS = File.OpenWrite(OutFile);

			StreamWriter SW = null;

			if (!PrintToConsole)
				SW = new StreamWriter(FS);

			Bitmap Bmp = new Bitmap(Image.FromFile(InFile));
			//if (PrintToConsole || Scale != 1.0f) {
			//if (Bmp.Width > 80) {
			//float Scale = 80.0f / Bmp.Width;

			int NewWidth = (int)(Bmp.Width * Scale);
			int NewHeight = (int)(Bmp.Height * Scale);

			//if (PrintToConsole)
			NewHeight = (int)(NewHeight * 0.5f);

			if (NewWidth <= 0)
				NewWidth = 1;
			if (NewHeight <= 0)
				NewHeight = 1;

			Bmp = new Bitmap(Bmp, new Size(NewWidth, NewHeight));
			//}
			//}

			for (int Y = 0; Y < Bmp.Height; Y++) {
				for (int X = 0; X < Bmp.Width; X++) {
					char C = ColorToChar(Bmp.GetPixel(X, Y), out ConsoleColor CClr);

					if (PrintToConsole) {
						if (PrintColor) {
							if (NoAscii)
								Console.BackgroundColor = CClr;
							else
								Console.ForegroundColor = CClr;
						}

						Console.Write(C);
					} else
						SW.Write(C);
				}

				string NewLine = "\r\n";
				if (PrintToConsole)
					Console.Write(NewLine);
				else
					SW.Write(NewLine);
			}
		}

		static char ColorToChar(Color Clr, out ConsoleColor CClr) {
			CClr = ColorToCColor(Clr);

			if (NoAscii)
				return ' ';

			byte Greyscale = (byte)(((Clr.R + Clr.G + Clr.B) / 3) * (Clr.A / 255.0f));

			if (InvertColors)
				Greyscale = (byte)(255 - Greyscale);

			float SingleUnitPerc = 1 / 256.0f;
			float Percentage = (Greyscale / 256.0f);


			Percentage = (float)(Percentage + ((Rnd.NextDouble() - 0.5) * 0.05));

			if (Percentage > 1.0f - SingleUnitPerc)
				Percentage = 1.0f - SingleUnitPerc;
			if (Percentage < 0.0f)
				Percentage = 0.0f;

			int Val = (int)(Chars.Length * Percentage);
			return Chars[Val];
		}

		static ConsoleColor ColorToCColor(Color Clr) {
			int Idx = (Clr.R > 128 | Clr.G > 128 | Clr.B > 128) ? 8 : 0; // Bright bit
			Idx |= (Clr.R > 64) ? 4 : 0; // Red bit
			Idx |= (Clr.G > 64) ? 2 : 0; // Green bit
			Idx |= (Clr.B > 64) ? 1 : 0; // Blue bit
			return (ConsoleColor)Idx;
		}

	}
}
