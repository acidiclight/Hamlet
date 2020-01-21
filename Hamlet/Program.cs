using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hamlet
{
    class Program
    {
        static readonly string intro = @"Hamlet - Text Adventure
=======================

This program is what happens when you tell a programmer to summarize Hamlet in 45 seconds as an English assignment.  Please note that it took way more than 45 seconds to write this and will probably take more than 45 seconds to play through this.

I wanted to do a full text adventure similar to the Zork series where you are able to freely type anything but that'd be way too much work even for me.  So I decided to approach this more like the text adventures seen in Saints Row 4.  It's more like a multiple choice, but still, a text adventure nonetheless.  Also, this wouldn't be my program if there weren't a few bugs so be careful.  And it wouldn't be my program if it didn't have my sense of humor embedded straight into the code, so you're going to KNOW when you've chosen a wrong path in the game. 

     [Press any key to continue.]";

        static GameData gameData = null;
        static bool inGame = false;
        static int currentState = 0;
        static int currentChoice = 0;
        static bool success = false;
        static string result = "";

        static void WriteLine()
        {
            int spacesToClear = (Console.WindowWidth - Console.CursorLeft) - 1;

            var bg = Console.BackgroundColor;
            var fg = Console.ForegroundColor;

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;

            for(int i = 0; i < spacesToClear; i++)
            {
                Console.Write(" ");
            }

            Console.WriteLine();

            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
        }

        static void WriteLine(string text)
        {
            Console.Write(text);
            WriteLine();
        }

        static void WriteLine(string text, params object[] data)
        {
            Console.Write(text, data);
            WriteLine();
        }

        static void Main(string[] args)
        {
            Intro();
        }

        static void Intro()
        {
            WriteLine(WordWrap(intro, Console.WindowWidth));
            Console.ReadKey(true);

            Clear();

            GameLoop();
        }

        static void GameOver()
        {
            bool waiting = true;
            int wChoice = 0;
            string[] wChoices =
            {
                "Start at the beginning",
                "Exit the game"
            };

            while (waiting)
            {
                Console.SetCursorPosition(0, 0);

                if (success)
                {
                    WriteLine("You have completed HAMLET.");
                }
                else
                {
                    WriteLine("GAME OVER");
                }
                if (!string.IsNullOrWhiteSpace(result))
                {
                    WriteLine();
                    WriteLine(WordWrap(result, Console.WindowWidth));
                }
                WriteLine();
                WriteLine("Thanks for playing.");
                WriteLine();

                WriteLine("What would you like to do?");
                WriteLine();

                for(int i = 0; i < wChoices.Length; i++)
                {
                    if(i == wChoice)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                    } else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }

                    WriteLine(" {0}. {1}", i + 1, wChoices[i]);
                }

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;

                WriteLine();
                WriteLine("[ENTER] Select   [UP/DOWN] Choose");

                Clear();

                var kinf = Console.ReadKey(true);

                switch(kinf.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.DownArrow:
                        if (wChoice == 0)
                            wChoice++;
                        else
                            wChoice--;
                        break;
                    case ConsoleKey.Enter:
                        waiting = false;
                        break;
                }

            }

            if (wChoice == 0)
            {
                Console.SetCursorPosition(0, 0);
                Intro();
            }
        }

        static void Clear()
        {
            int rowsLeft = (Console.WindowHeight - Console.CursorTop) - 1;
            for(int i = 0; i < rowsLeft; i++)
            {
                WriteLine();
            }
        }

        static void GameLoop()
        {
            result = "";
            inGame = true;
            currentState = 0;
            gameData = JsonConvert.DeserializeObject<GameData>(Properties.Resources.gamedata);

            while (inGame)
            {
                var state = gameData.SceneStates[currentState];

                Console.SetCursorPosition(0, 0);
                WriteLine("HAMLET - ACT {0}, SCENE {1}", state.Act + 1, state.Scene + 1);
                WriteLine("============");
                if (!string.IsNullOrWhiteSpace(result))
                {
                    WriteLine();
                    WriteLine(WordWrap(result, Console.WindowWidth));
                }
                WriteLine();
                WriteLine(WordWrap(state.Prompt, Console.WindowWidth));
                WriteLine();
                for (int i = 0; i < state.Choices.Length; i++)
                {
                    if (i == currentChoice)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    WriteLine(WordWrap(string.Format(" {0}. {1}", i + 1, state.Choices[i]), Console.WindowWidth));
                }

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;

                WriteLine();
                WriteLine();
                WriteLine("[ENTER] Choose   [UP/DOWN] Select choice");

                Clear();

                var kinf = Console.ReadKey(true);

                switch (kinf.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (currentChoice > 0)
                            currentChoice--;
                        else 
                            currentChoice = state.Choices.Length - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        if (currentChoice < state.Choices.Length - 1)
                            currentChoice++;
                        else 
                            currentChoice = 0;
                        break;
                    case ConsoleKey.Enter:
                        result = state.Responses[currentChoice];
                        if (currentChoice == state.CorrectChoice)
                        {
                            currentState++;
                            if (currentState >= gameData.SceneStates.Length)
                            {
                                inGame = false;
                                success = true;
                            }
                        }
                        else
                        {
                            if (!state.LoopBack)
                            {
                                inGame = false;
                                success = false;
                            }
                        }


                        currentChoice = 0;
                        break;
                }
            }

            GameOver();
        }

        static string WordWrap(string text, int width)
        {
            if (width <= 0) return text;

            var sb = new StringBuilder();

            int textPtr = 0;
            int lineWidth = 0;

            while(textPtr < text.Length)
            {
                string word = "";
                for(int i = textPtr; i < text.Length; i++)
                {
                    word += text[i];
                    if (char.IsWhiteSpace(text[i])) break;
                }

                int wordWidth = word.TrimUnnecessaryWhiteSpace().Length;

                if(lineWidth + wordWidth >= width && lineWidth > 0)
                {
                    sb.Append(Environment.NewLine);
                    lineWidth = 0;
                }

                int wordPtr = 0;
                while(wordWidth >= width)
                {
                    int i = 0;
                    int lw = 0;
                    int p = 0;
                    for(i = wordPtr; i < word.Length; i++)
                    {
                        int w = 1;
                        if(lw + w >= width)
                        {
                            wordPtr += p;
                            wordWidth -= lw;
                            sb.Append(Environment.NewLine);
                            break;
                        }

                        sb.Append(word[wordPtr + p]);

                        lw += w;
                        p++;
                    }
                }

                sb.Append(word.Substring(wordPtr));
                lineWidth += wordWidth;

                if (word.EndsWith("\n"))
                    lineWidth = 0;

                textPtr += word.Length;
            }

            return sb.ToString();
        }
    }

    public static class MuffinUtilities
    {
        public static string TrimUnnecessaryWhiteSpace(this string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            List<char> unneeded = new List<char>();

            for(char c = '\0'; c < char.MaxValue; c++)
            {
                if(char.IsWhiteSpace(c) && c != ' ')
                {
                    unneeded.Add(c);
                }
            }

            string result = text;

            while (!string.IsNullOrEmpty(result) && unneeded.Contains(result[0])) result = result.Remove(0, 1);
            while (!string.IsNullOrEmpty(result) && unneeded.Contains(result[result.Length - 1])) result = result.Remove(result.Length - 1, 1);

            return result;
        }
    }
}
