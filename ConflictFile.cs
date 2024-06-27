using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace resolve_trivial_conflicts
{
    public class ConflictFile
    {
        private TextReader reader;
        private TextWriter writer;

        public const string START  = "<<<<<<<";
        public const string MIDDLE = "=======";
        public const string END    = ">>>>>>>";

        private enum State
        {
            outside,
            head,
            change,
        }


        public ConflictFile(TextReader reader, TextWriter writer)
        {
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        internal void ResolveSimple()
        {
            State state = State.outside;
            Conflict conflict = new Conflict();


            //<<<<<<< HEAD
            //                            {
            //=======
            //							{
            //>>>>>>> 7d6bfed3 (Found that the OutstandingMEdiaRequest for the DTMF event is never completed. Causing some logic when Nuance completes to miss that this was DTMF induced.)

            try
            {
                TextWriter current_writer = this.writer;

                for (int lineNumber = 0; ;)
                {
                    string? line = reader.ReadLine();
                    if (line == null)
                        break;
                    ++lineNumber;

                    if (line.StartsWith(START))
                    {
                        if (state != State.outside) throw new InvalidOperationException($"Read a {START} when in state {state}");
                        state = State.head;
                        conflict.HeadLineNo = lineNumber;
                        conflict.HeadLine = line;
                        current_writer = conflict.head;
                    }
                    else if (line.StartsWith(MIDDLE))
                    {
                        if (state != State.head) throw new InvalidOperationException($"Read a {MIDDLE} when in state {state}");
                        state = State.change;
                        conflict.MiddleLineNo = lineNumber;
                        conflict.MiddleLine = line;
                        current_writer = conflict.change;
                    }
                    else if (line.StartsWith(END))
                    {
                        if (state != State.change) throw new InvalidOperationException($"Read a {END} when in state {state}");
                        conflict.EndLineNo = lineNumber;
                        conflict.EndLine = line;
                        if (conflict.EqualsIgnoreWhitespace())
                        {
                            Console.Error.WriteLine($"Conflict between {conflict.HeadLineNo} -- {conflict.EndLineNo} are same. Hiding change");
                            writer.Write(conflict.head);
                        }
                        else
                        {
                            Console.Error.WriteLine($"Conflict between {conflict.HeadLineNo} -- {conflict.EndLineNo} are differnt. Leaving in");
                            conflict.WriteTo(writer);
                        }

                        // reset
                        conflict = new Conflict();
                        state = State.outside;
                        current_writer = this.writer;
                    }
                    else
                    {
                        current_writer.WriteLine(line);
                    }
                }
            }
            catch ( Exception ex )
            {
                Console.Error.WriteLine($"Error when handling change {conflict.HeadLineNo}-{conflict.EndLineNo}: " + ex.Message);
                Console.Error.WriteLine(ex);
                
            }
        }
    }
}
