using System;
using System.IO;
using System.Text;

namespace BFInterpret {

    class Interpreter {

        // Amount of memory needed, I try to aim for as small as possible
        const int MEM_SIZE = 512;

        byte[] memory;
        int pointer = 0, highestUsedMemory = 0;
        string output = "";

        public Interpreter(string script) {
            memory = new byte[MEM_SIZE];
            doActions(script);

            // Spitting out memory information
            Console.WriteLine("Dump:");
            for(int i = 0; i <= highestUsedMemory; i++) {
                // Sticking 0s on the front for neatness
                string num = memory[i] < 10 ? "00" + memory[i].ToString() : memory[i] < 100 ? "0" + memory[i].ToString() : memory[i].ToString();
                Console.Write("{0} ", num);
            }
            Console.Write("\n");
            for(int i = 0; i < pointer; i++) // Putting spaces before the pointer indicator
                Console.Write("    ");
            Console.Write(" ^\nOutput:\n{0}", output);
        }

        int looppointer = -1; // Used to show where loops start
        private void doActions(string ac) {
            for(int i = 0; i < ac.Length; i++) {
                Console.WriteLine("At index {0}", i); // To keep track in big scripts
                int p = performSingleAction(ac.ToCharArray()[i]);
                if(p == 1) // Start of a loop
                    looppointer = i;
                if(looppointer != -1 && p == 2) { // Checking that we're in a loop (looppointer) and at the end of the loop
                    if(memory[pointer] != 0) // Loops only end if the resulting address is at 0
                        i = looppointer;
                    else
                        looppointer = -1; // Set to -1 meaning we're out of the loop
                }
                Console.Clear();
            }
        }

        // Performs the action of a single character
        // Ignores non-brainfuck characters too!
        // Returns 1 if start of loop, 2 if end of loop and 0 otherwise
        private int performSingleAction(char c) {
            switch(c) {
            case '<':
                pointer--;
                if(pointer < 0)
                    pointer = MEM_SIZE - 1;
                break;
            case '>':
                pointer++;
                if(pointer >= MEM_SIZE)
                    pointer = 0;
                // This is used to display only relevant bytes, not *all* of them
                highestUsedMemory = Math.Max(highestUsedMemory, pointer);
                break;
            case '+':
                if(memory[pointer] == 255)
                    memory[pointer] = 0;
                else
                    memory[pointer]++;
                break;
            case '-':
                if(memory[pointer] == 0)
                    memory[pointer] = 255;
                else
                    memory[pointer]--;
                break;
            case '.':
                output += Encoding.UTF8.GetString(new byte[] { memory[pointer] });
                break;
            case '[':
                return 1;
            case ']':
                return 2;
            case ',':
                Console.Write("Input (press a key): ");
                char input = Console.ReadKey().KeyChar;
                memory[pointer] = Encoding.UTF8.GetBytes(new char[] { input })[0];
                break;
            }
            return 0;
        }

        static void Main(string[] args) {
            if(args.Length < 1) {
                Console.WriteLine("Need an input file!\nFormat: bf.exe script.bf");
                return;
            }
            if(!File.Exists(args[0])) {
                Console.WriteLine("{0} doesn't exist!", args[0]);
                return;
            }
            new Interpreter(File.ReadAllText(args[0]));
        }

    }

}
